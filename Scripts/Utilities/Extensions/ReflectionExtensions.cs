﻿using System;
using System.Reflection;

namespace FPTemplate.Utilities.Extensions
{
	public static class ReflectionExtensions
    {
        public static T GetAttribute<T>(this FieldInfo field) where T: Attribute
        {
            var allAttributes = field.GetCustomAttributes(typeof(T), true);
            if (allAttributes.Length == 0)
            {
                return null;
            }
            return allAttributes[0] as T;
        }

        public static bool HasAttribute<T>(this FieldInfo field) where T : Attribute
        {
            var allAttributes = field.GetCustomAttributes(typeof(T), true);
            return allAttributes.Length > 0;
        }

    }
}