using System;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus;

namespace MultipleMsmq
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

            return types;
        }
    }

}
