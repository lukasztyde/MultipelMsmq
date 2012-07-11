using System;
using System.Configuration;
using System.Linq;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.Unicast;

namespace MultipleMsmq
{
    sealed class MyConfigurationSource : IConfigurationSource
    {
        private readonly string _inputQueue;

        public MyConfigurationSource(string inputQueue)
        {
            _inputQueue = inputQueue;
        }

        public void AddReceiverBus(string inputQueue, Type handler)
        {
            var types = typeof(UnicastBus).Assembly.GetTypes().Union(new[] { handler });

            Configure.With(types)
                 //.CustomConfigurationSource(new MyConfigurationSource(inputQueue))
                 .CustomConfigurationSource(this)
                 .StructureMapBuilder()
                 .XmlSerializer()
                 .MsmqTransport()
                     .IsTransactional(true)
                     .MsmqSubscriptionStorage()
                 .UnicastBus()
                     .ImpersonateSender(false)
                     .LoadMessageHandlers()
                 .CreateBus()
                 .Start();
        }

        public T GetConfiguration<T>() where T : class
        {
            Console.WriteLine("==== {0} ==== ", typeof(T));

            if (typeof(T) == typeof(MsmqTransportConfig))
                return new MsmqTransportConfig
                {
                    ErrorQueue = _inputQueue + "_error",
                    InputQueue = _inputQueue,
                    MaxRetries = 5,
                    NumberOfWorkerThreads = 1
                } as T;

            return ConfigurationManager.GetSection(typeof(T).Name) as T;
        }
    }
}
