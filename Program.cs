using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NServiceBus.Unicast.Transport;
using Wonga.QA.Framework.Core;
using Wonga.QA.Framework.Msmq.Messages.Risk.Business;
using log4net;
using NServiceBus;
using NServiceBus.Unicast;

namespace NServiceBusDemo
{
    class Program
    {
        static string GetName(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        static void Main()
        {
            SetLoggingLibrary.Log4Net(log4net.Config.XmlConfigurator.Configure);
            Hashtable mapping = CreateMapping();

            //var appId = new Guid("cdaf96f4-7d57-4e91-ab7d-d1dbd6908e2c");
            //var thisMessageWasSentToSalesforce = false;

            //AddHandler<IBusinessApplicationAcceptedEvent>(
            //    filter: x => x.ApplicationId == appId,
            //    action: (x, bus) => thisMessageWasSentToSalesforce = true);

            AddReceiverBus("riskservice", typeof(UniversalMessagesHandler));
            AddReceiverBus("salesforce", typeof(UniversalMessagesHandler));

            StructureMap.ObjectFactory.Inject(CreateSenderBus(mapping));

            //SendMessageOnTheBus();

            //Do.Until(() => thisMessageWasSentToSalesforce);
            Console.ReadLine();
        }

        public static void AddHandler<T>(Action<T, IBus> action) where T : IMessage
        {
            AddHandler<T>(null, action);
        }
        public static void AddHandler<T>(Func<T, bool> filter, Action<T, IBus> action) where T : IMessage
        {
            GenericHandler.Add(new OnTheFlyHandler<T>(filter, action));
        }

        public static Hashtable CreateMapping()
        {
            string pathToMsmqMessages = AssemblyDirectory + "\\Wonga.QA.Framework.Msmq.dll";
            
            List<Type> messagesTypes = ReflectionHelper.GetIMessageTypes(pathToMsmqMessages);

            Hashtable mapping = new Hashtable
            {
               { GetName(typeof(IMessage)), "IMessage" }
            };

            foreach (Type type in messagesTypes)
            {
                try
                {
                    mapping.Add(GetName(type), "riskservice");
                    mapping.Add(GetName(type), "salesforce");
                }
                catch(Exception ex)
                {
                   
                }
            }
            return mapping;
        }

        static private string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private static IBus CreateSenderBus(Hashtable mapping)
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

        private static void AddReceiverBus(string inputQueue, Type handler)
        {
            var types = typeof(UnicastBus).Assembly.GetTypes().Union(
                new[] { handler });

           Configure.With(
                types
                //typeof(IMessage).Assembly,
                //typeof(CompletionMessage).Assembly,
                //typeof(OnTheFlyHandler).Assembly,
                //typeof(Wonga.QA.Framework.Msmq.Messages.Risk.IRiskEvent).Assembly
                )

                .CustomConfigurationSource(new MyConfigurationSource(inputQueue))
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

        private static void SendMessageOnTheBus()
        {
            //send a massage by NQyeueStuffer <- (from Wonga.QA.Framework.Msmq.dll)
        }
    }
}
