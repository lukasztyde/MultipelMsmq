using System;
using System.Configuration;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

namespace NServiceBusDemo
{
    sealed class MyConfigurationSource : IConfigurationSource
    {
        private readonly string _inputQueue;

        public MyConfigurationSource(string inputQueue)
        {
            _inputQueue = inputQueue;
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