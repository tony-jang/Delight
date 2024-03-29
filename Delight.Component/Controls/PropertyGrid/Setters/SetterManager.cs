﻿using System;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;

using Delight.Core.Extensions;

namespace Delight.Component.Controls
{
    public static class SetterManager
    {
        static Dictionary<Type, Type> setterTypes;
        static Dictionary<string, Type> setterNameTypes;

        static SetterManager()
        {
            setterTypes = new Dictionary<Type, Type>();
            setterNameTypes = new Dictionary<string, Type>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Name.Contains("PercentageSetter"))
                {
                }
                if (type.HasAttribute<SetterAttribute>())
                {
                    var attr = type.GetAttribute<SetterAttribute>();

                    if (string.IsNullOrEmpty(attr.Key))
                        setterTypes[attr.Type] = type;
                    else
                        setterNameTypes[attr.Key] = type;
                }
            }
        }

        public static ISetter CreateSetter(object[] targets, PropertyInfo[] propertyInfos)
        {
            Type type = propertyInfos[0].PropertyType;
            
            if (!setterTypes.ContainsKey(type))
            {
                if (type.IsEnum)
                    type = typeof(Enum);
                else
                    return null;
            }

            return (ISetter)Activator.CreateInstance(
                setterTypes[type],
                new object[] { targets, propertyInfos });
        }

        public static ISetter CreateSetter(object[] targets, PropertyInfo[] propertyInfos, string name)
        {
            if (!setterNameTypes.ContainsKey(name))
                return null;
            
            return (ISetter)Activator.CreateInstance(
                setterNameTypes[name],
                new object[] { targets, propertyInfos });
        }

        public static ISetter CreateSetter(object target, PropertyInfo propertyInfo)
        {
            return CreateSetter(new[] { target }, new[] { propertyInfo });
        }

        public static ISetter CreateSetter(object target, PropertyInfo propertyInfo, string name)
        {
            return CreateSetter(new[] { target }, new[] { propertyInfo }, name);
        }
    }
}