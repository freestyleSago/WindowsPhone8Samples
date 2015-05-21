using System;
using System.Reflection;

namespace Telerik.Core
{
    internal static class ReflectionHelper
    {
        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            var property = typeInfo.GetDeclaredProperty(propertyName);
            if (property != null)
            {
                return property;
            }

            Type baseType = typeInfo.BaseType;
            if (baseType == null)
            {
                return null;
            }

            return GetProperty(baseType, propertyName);
        }

        public static MethodInfo GetMethod(Type type, string methodName)
        {
            TypeInfo typeInfo = type.GetTypeInfo();

            var method = typeInfo.GetDeclaredMethod(methodName);
            if (method != null)
            {
                return method;
            }

            Type baseType = typeInfo.BaseType;
            if (baseType == null)
            {
                return null;
            }

            return GetMethod(baseType, methodName);
        }
    }
}
