using DataBuffer.MessageExchangeSerializer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Threading;
using DataBuffer.BusClient.RabbitMq.Pool;
using System.Collections.Generic;

namespace DataBuffer.BusClient.RabbitMq
{

    public class RabbitMQQueueConsumer
    {
        private RabbitMqConnection _rabbitMqConnection;

        /// <summary>
        /// Канал для подключения к главной очереди
        /// </summary>
        private IModel _queueChannel;

        private string _consumerTag;

        private readonly RabbitMqConnectionPool _rabbitMqConnectionPool;

        private readonly object _syncObj = new object();

        private readonly ManualResetEventSlim _handleMessageEvent;

        private string _queueName;

        private IMessageExchangeSerializer _messageExchangeSerializer;

        private Action<MemoryStream, IMessageExchangeSerializer> _handler;

        public RabbitMQQueueConsumer(string connectionString, string queueName, RabbitMqConnectionPool connectionPool,
            Action<MemoryStream, IMessageExchangeSerializer> handler)
        {
            _rabbitMqConnectionPool = connectionPool;
            _queueName = queueName;
            _handler = handler;
            _messageExchangeSerializer = new JsonNetMessageExchangeSerializer();
            _handleMessageEvent = new ManualResetEventSlim(true);
        }

        public void Start()
        {
            Console.WriteLine($"Запуск обработчика очереди {_queueName}...");

            CheckExistQueue();
            StartListen();

            Console.WriteLine($"Обработчик очереди {_queueName} успешно запущен!");
        }

        public void Stop()
        {
            try
            {
                lock (_syncObj)
                {
                    Console.WriteLine($"Остановка обработчика очереди {_queueName}...");

                    StopListen();
                    Console.WriteLine($"Обработчик очереди {_queueName} успешно остановлен!");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка остановки обработчика очереди {_queueName}", ex);
            }
        }

        private void StopListen()
        {
            if (_rabbitMqConnection != null)
                _rabbitMqConnection.BrokerConnection.CallbackException -= OnConnectionCallbackException;

            if (_queueChannel != null)
            {
                _queueChannel.CallbackException -= OnCallbackException;
                //_queueChannel.ModelShutdown -= OnModelShutdown;
            }

            StopListenQueue(ref _consumerTag, ref _queueChannel);
            _rabbitMqConnection = null;
        }

        private void StartListen()
        {
            _rabbitMqConnection = _rabbitMqConnectionPool.Get();
            _rabbitMqConnection.BrokerConnection.CallbackException += OnConnectionCallbackException;

            _queueChannel = _rabbitMqConnection.CreateChannel();
            _queueChannel.CallbackException += OnCallbackException;
            //_queueChannel.ModelShutdown += OnModelShutdown;
            // возвращаем канал в пул
            _rabbitMqConnectionPool.Return(_rabbitMqConnection);

            var consumer = new EventingBasicConsumer(_queueChannel);
            consumer.Received += (ch, ea) =>
            {
                _handleMessageEvent.Reset();
                var cts = new CancellationTokenSource();
               
                try
                {
                    using (var body = new MemoryStream(ea.Body.ToArray(), 0, ea.Body.Length))
                    {
                        _handler(body, _messageExchangeSerializer);
                        ConfirmMessage(ea);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    try
                    {
                        AbandonMessage(ea, ex.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("При отклонении сообщения возникла ошибка", e);
                    }
                }
                finally
                {
                    cts.Cancel();
                    _handleMessageEvent.Set();
                }
            };

            _consumerTag = _queueChannel.BasicConsume(_queueName, false, consumer);
        }

        private void CheckExistQueue()
        {
            var rabbitMqConnection = _rabbitMqConnectionPool.Get();
            var queueChannel = rabbitMqConnection.CreateChannel();
            // возвращаем канал, могут другие использовать
            _rabbitMqConnectionPool.Return(rabbitMqConnection);
            try
            {
                _ = queueChannel.QueueDeclarePassive(_queueName);
            }
            catch (Exception)
            {
                // очереди нет, создаем очередь
                rabbitMqConnection.CloseChannel(queueChannel);
                rabbitMqConnection = _rabbitMqConnectionPool.Get();
                queueChannel = rabbitMqConnection.CreateChannel();
                // возвращаем канал, могут другие использовать
                _rabbitMqConnectionPool.Return(rabbitMqConnection);
                //queues
                queueChannel.QueueDeclare(_queueName, true, false, false, new Dictionary<string, object>()
                { });
            }
            rabbitMqConnection.CloseChannel(queueChannel);
        }

        private void StopListenQueue(ref string consumerTag, ref IModel channel)
        {
            if (!string.IsNullOrEmpty(consumerTag))
            {
                channel.BasicCancel(consumerTag);
                consumerTag = null;
            }

            if (!_handleMessageEvent.IsSet)
            {
                TimeSpan waitTimeOut = new TimeSpan(0, 1, 30);
                Console.WriteLine(
                    $"В текущий момент времени идет обработка сообщения. Ожидание окончания обработки сообщения в течении {waitTimeOut.TotalSeconds} секунд");
                if (_handleMessageEvent.Wait(waitTimeOut) == true)
                {
                    Console.WriteLine("Ожидание обработки сообщения завершилось успешно!");
                }
                else
                {
                    Console.WriteLine(
                        "Не удалось дождаться обработки сообщения. Закрываем соединение с RabbitMQ сервером");
                }
            }

            _rabbitMqConnection.CloseChannel(channel);
            channel = null;
        }

        private void ConfirmMessage(BasicDeliverEventArgs arg)
        {
            _queueChannel.BasicAck(arg.DeliveryTag, false);
        }

        private void AbandonMessage(BasicDeliverEventArgs arg, string errorMessage)
        {
            _queueChannel.BasicReject(arg.DeliveryTag, !arg.Redelivered);
        }

        private void OnConnectionCallbackException(object sender, CallbackExceptionEventArgs args)
        {
            Console.WriteLine("Возникло исключение на уровне соединения к брокеру очередей RabbitMQ", args.Exception);
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs args)
        {
            Console.WriteLine("Возникло исключение на уровне канала", args.Exception);
        }
    }
}
