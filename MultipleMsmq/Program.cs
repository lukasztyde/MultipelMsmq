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

            EndPoint salesforce = new EndPoint("salesforce");
            salesforce.Start();

            EndPoint riskservice = new EndPoint("riskservice");
            riskservice.Start();

            var appId = new Guid("cdaf96f4-7d57-4e91-ab7d-d1dbd6908e2c");
            bool thisMessageWasSentToSalesforce = false;

            riskservice.AddHandler<IBusinessApplicationAcceptedEvent>(
            filter: x => x.ApplicationId == appId,
            action: (x, bus) => thisMessageWasSentToSalesforce = true);

            salesforce.AddHandler<IBreTransactionRecievedEvent>(
            //filter: x => x.ApplicationId == appId,
            action: (x, bus) => thisMessageWasSentToSalesforce = true);
    
            StructureMap.ObjectFactory.Inject(MyConfigurationSender.CreateSenderBus(mapping));

            Console.ReadLine();
        }
    }
}
