using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.Core
{
    /// <summary>
    /// Encapsulates helper methods to generate Dynamic methods using System.Reflection.Emit.
    /// </summary>
    public static class DynamicHelper
    {
        /// <summary>
        /// Generates a untyped function to allow retrieving property values for instances of the specified type without using reflection.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static Func<object, object> CreatePropertyValueGetter(Type type, string propertyName)
        {
            if (type == null)
            {
                throw new ArgumentException("type argument is NULL.");
            }

            if (propertyName == null)
            {
                throw new ArgumentException("propertyName argument is NULL.");
            }

            TypeInfo info = type.GetTypeInfo();
            if (info.IsNotPublic)
            {
                throw new ArgumentException("Cannot create dynamic property getter for non-public types.");
            }

            if (info.IsValueType)
            {
                throw new ArgumentException("Dynamic getter is not supported for value type [" + info.Name + "]");
            }

            return BindingExpressionHelper.CreateGetValueFunc(type, propertyName);
        }
    }
}
