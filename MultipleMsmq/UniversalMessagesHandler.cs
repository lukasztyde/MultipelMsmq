using System;
using NServiceBus;
using log4net;

namespace MultipleMsmq
{
    class UniversalMessagesHandler : IMessageHandler<IMessage>
    {
        private static readonly ILog Logger;

        public void Handle(IMessage message)
        {
            Console.Out.WriteLine("Handler Works");
        }
    }
}
