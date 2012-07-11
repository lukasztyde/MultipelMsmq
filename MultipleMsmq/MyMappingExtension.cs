using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NServiceBus;
using NServiceBus.Unicast;
using NServiceBus.Unicast.Config;

namespace MultipleMsmq
{
    public static class MyMappingExtension
    {

        public static Hashtable CreateMapping()
        {
            string pathToMsmqMessages = AssemblyDirectory + "\\Wonga.QA.Framework.Msmq.dll";

            List<Type> messagesTypes = ReflectionHelper.GetIMessageTypes(pathToMsmqMessages);

            Hashtable mapping = new Hashtable
            {
               { GetName(typeof(IMessage)), "IMessage" }
            };

            return mapping;
        }

        public static ConfigUnicastBus AddMapping(this ConfigUnicastBus config, Hashtable mapping)
        {
            config.RunCustomAction(() =>
                    Configure.Instance.Configurer.ConfigureProperty<UnicastBus>(x => x.MessageOwners, mapping)
                );
            return config;
        }
      
        #region helpers method
        private static string GetName(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        #endregion
    }
}
