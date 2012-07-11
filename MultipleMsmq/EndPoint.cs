using System;
using NServiceBus;
using Wonga.QA.Framework.Msmq.Messages.Risk.Business;

namespace MultipleMsmq
{
    class EndPoint
    {
        private string _name;
        private MyConfigurationSource _configEndPoint;


        public EndPoint(string endpointName)
        {
            _name = endpointName;
        }

        public void Start()
        {
            _configEndPoint = new MyConfigurationSource(_name);
        }

        public void AddHandler<T>(Action<T, IBus> action) where T : IMessage
        {
            AddHandler<T>(null, action);
        }

        public void AddHandler<T>(Func<T, bool> filter, Action<T, IBus> action) where T : IMessage
        {
            GenericHandler<T>.Add(new OnTheFlyHandler<T>(filter, action));
            _configEndPoint.AddReceiverBus(_name, typeof(GenericHandler<T>));
        }
    }
}
