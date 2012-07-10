using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NServiceBus;
using NServiceBus.Unicast.Transport;

namespace NServiceBusDemo
{
    public class ReflectionHelper
    {
        public static List<Type> GetIMessageTypes(string assemblyPath)
        {
            List<Type> types = new List<Type>();

            Assembly assembly = Assembly.LoadFile(assemblyPath);

            Type[] assemblyTypes;

            try
            {
                assemblyTypes = assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                assemblyTypes = ex.Types;
            }


            foreach (Type type in assemblyTypes)
            {
                if (type == null) continue;

                Type myType = type.GetInterface(typeof(IMessage).ToString());
                if (myType != null)
                {
                    types.Add(type);
                }
            }


            //!!
            //types.Add(typeof(CompletionMessage));
            types.Add(typeof(Wonga.QA.Framework.Msmq.Messages.Risk.IRiskEvent));
            //!!

            return types;
        }
    }
}
