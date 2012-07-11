using System.Collections;
using NServiceBus;

namespace MultipleMsmq
{
    public static class MyConfigurationSender
    {
        public static IBus CreateSenderBus(Hashtable mapping)
        {
            return Configure.With()
                .StructureMapBuilder()
                .XmlSerializer()
                .MsmqTransport()
                .IsTransactional(true)
                .UnicastBus()
                .ImpersonateSender(false)
                .AddMapping(mapping)
                .CreateBus()
                .Start();
        }
    }
}
