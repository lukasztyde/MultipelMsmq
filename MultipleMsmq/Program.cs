using System;
using System.Collections;
using Wonga.QA.Framework.Msmq.Messages.BankGateway.PublicMessages;
using Wonga.QA.Framework.Msmq.Messages.Risk.Business;

namespace MultipleMsmq
{
    class Program
    {
        static void Main()
        {
            Hashtable mapping = MyMappingExtension.CreateMapping();
            StructureMap.ObjectFactory.Inject(MyConfigurationSender.CreateSenderBus(mapping));

            var appId = new Guid("cdaf96f4-7d57-4e91-ab7d-d1dbd6908e2c");
            bool thisMessageWasSentToSalesforce = false;

            EndPoint riskservice = new EndPoint("riskservice");
            riskservice.Start();

            riskservice.AddHandler<IBusinessApplicationAcceptedEvent>(
                filter: x => x.ApplicationId == appId,
                action: (x, bus) => thisMessageWasSentToSalesforce = true
            );


            EndPoint salesforce = new EndPoint("salesforce");
            salesforce.Start();

            salesforce.AddHandler<IBreTransactionRecievedEvent>(
                //filter: x => x.ApplicationId == appId,
                action: (x, bus) => thisMessageWasSentToSalesforce = true
            );

            Console.ReadLine();
        }
    }
}
