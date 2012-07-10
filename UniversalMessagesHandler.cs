using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using log4net;

namespace NServiceBusDemo
{
    class UniversalMessagesHandler: IMessageHandler<IMessage>
    {
        private static readonly ILog Logger;

        public void Handle(IMessage message)
        {
            Console.Out.WriteLine("Handler Works");
        }
    }
}
