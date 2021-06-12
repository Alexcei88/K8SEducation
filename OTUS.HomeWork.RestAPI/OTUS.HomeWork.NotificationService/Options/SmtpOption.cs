using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTUS.HomeWork.NotificationService.Options
{
    public class SmtpOption
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string SourceEmail { get; set; }

        public string SourceEmailName { get; set; }

        public string Password { get; set; }
    }
}
