using System.IO;
using System.Threading.Tasks;

namespace OTUS.HomeWork.Common
{
    public interface IMessageHandler
    {
        public string MessageType { get; }

        public Task HandleAsync(MemoryStream body);
    }
}
