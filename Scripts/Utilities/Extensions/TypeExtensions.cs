﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FPTemplate.Utilities.Extensions
{
    public static class TypeExtensions
    {
        public static List<Type> GetAllChildTypes(this Type type)
        {
            List<Type> result = new List<Type>();

            Assembly assembly = Assembly.GetAssembly(type);
            Type[] allTypes = assembly.GetTypes();
            for (int i = 0; i < allTypes.Length; i++)
                if (allTypes[i].IsSubclassOf(type)) result.Add(allTypes[i]); //nb: IsAssignableFrom will return derived classes

            return result;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            var allAttributes = type.GetCustomAttributes(typeof(T), true);
            if (allAttributes.Length == 0)
            {
                return null;
            }
            return allAttributes[0] as T;
        }

        public static List<Type> GetAllTypesImplementingInterface(this Type interfaceType, Type assemblyType = null)
        {
            if (!interfaceType.IsInterface)
            {
                throw new Exception("Must be an interface type!");
            }
            var result = new List<Type>();

            var assembly = Assembly.GetAssembly(assemblyType ?? interfaceType);
            var allTypes = assembly.GetTypes();
            for (var i = 0; i < allTypes.Length; i++)
            {
                if (allTypes[i].GetInterfaces().Contains(interfaceType))
                {
                    result.Add(allTypes[i]);
                }
            }

            return result;
        }

        public static bool HasAttribute<T>(this Type field) where T : Attribute
        {
            var allAttributes = field.GetCustomAttributes(typeof(T), true);
            return allAttributes.Length > 0;
        }
    }
}