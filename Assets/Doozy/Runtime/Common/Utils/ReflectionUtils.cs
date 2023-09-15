// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common.Utils
{
    public static class ReflectionUtils
    {
        private static Assembly[] s_domainAssemblies;

        public static IEnumerable<Assembly> domainAssemblies
        {
            get
            {
                // Debug.Log($"{nameof(ReflectionUtils)} > {nameof(domainAssemblies)}");
                return s_domainAssemblies ??= AppDomain.CurrentDomain.GetAssemblies();
            }
        }

        private static Assembly s_doozyEditorAssembly;
        public static Assembly doozyEditorAssembly
        {
            get
            {
                if (s_doozyEditorAssembly != null) return s_doozyEditorAssembly;
                foreach (Assembly assembly in domainAssemblies)
                {
                    if (!assembly.DefinedTypes.Any(typeInfo => typeInfo.Namespace != null && typeInfo.Namespace.Contains("Doozy.Editor.")))
                        continue;
                    s_doozyEditorAssembly = assembly;
                    return s_doozyEditorAssembly;
                }
                return s_doozyEditorAssembly;
            }
        }

        private static Assembly s_doozyRuntimeAssembly;
        public static Assembly doozyRuntimeAssembly => 
            s_doozyRuntimeAssembly ??= Assembly.GetAssembly(typeof(ReflectionUtils));

        private static IEnumerable<Type> s_doozyRuntimeTypes;
        public static IEnumerable<Type> doozyRuntimeTypes => 
            s_doozyRuntimeTypes ??= doozyRuntimeAssembly.GetTypes();


        /// <summary>
        /// Get all derived types for the givenBase type from the types collection
        /// </summary>
        /// <param name="types"> Collection of types </param>
        /// <param name="baseType"> Base Type </param>
        public static IEnumerable<Type> GetDerivedTypes(IEnumerable<Type> types, Type baseType)
        {
            var list = new List<Type>();
            foreach (Type typeInAssembly in types)
            {
                if (typeInAssembly.BaseType != baseType)
                    continue;

                if (typeInAssembly.IsAbstract)
                    continue;

                list.Add(typeInAssembly);
            }
            return list;
        }

        /// <summary>
        /// Get all types that implement the given interface
        /// </summary>
        /// <typeparam name="T"> Type of Interface </typeparam>
        public static IEnumerable<Type> GetTypesThatImplementInterface<T>(Assembly fromAssembly) =>
            fromAssembly
                .GetTypes()
                .Where(p => typeof(T).IsAssignableFrom(p) && !p.IsInterface);
        
        /// <summary>
        /// Get all types that implement the given interface
        /// </summary>
        /// <typeparam name="T"> Type of Interface </typeparam>
        public static IEnumerable<Type> GetTypesThatImplementInterface<T>() =>
            domainAssemblies
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(T).IsAssignableFrom(p) && !p.IsInterface);

        /// <summary>
        /// Get all de derived types for the given baseType and search in the given assembly
        /// </summary>
        /// <param name="assembly"> Assembly to search for derived types </param>
        /// <param name="baseType"> Base Type </param>
        public static IEnumerable<Type> GetDerivedTypes(Assembly assembly, Type baseType) =>
            GetDerivedTypes(assembly.GetTypes(), baseType);

        /// <summary>
        /// Get all the derived types for the given type and search in the same assembly as the Base Type
        /// </summary>
        /// <param name="baseType"> Base Type </param>
        public static IEnumerable<Type> GetDerivedTypes(Type baseType) =>
            GetDerivedTypes(Assembly.GetAssembly(baseType), baseType);

        public static IEnumerable<T> GetAttributeReferences<T>(IEnumerable<Type> types) where T : Attribute
        {
            var list = new List<T>();
            foreach (Type type in types)
            {
                MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (MemberInfo member in members)
                {
                    T attribute = member.GetCustomAttribute<T>();
                    if (attribute == null) continue;
                    list.Add(attribute);
                }
            }

            return list;
        }


        private static bool GetAttribute<T>(IEnumerable<object> attributes, out T attributeOut) where T : Attribute
        {
            foreach (object attribute in attributes)
            {
                if (attribute.GetType() != typeof(T))
                {
                    continue;
                }

                attributeOut = attribute as T;
                return true;
            }

            attributeOut = null;
            return false;
        }

        public static bool GetAttribute<T>(Type classType, out T attributeOut) where T : Attribute
        {
            object[] attributes = classType.GetCustomAttributes(typeof(T), false);
            return GetAttribute(attributes, out attributeOut);
        }

        public static bool GetAttribute<T>(Type classType, string fieldName, out T attributeOut) where T : Attribute
        {
            object[] attributes = classType.GetField(fieldName).GetCustomAttributes(typeof(T), false);
            return GetAttribute(attributes, out attributeOut);
        }

        public static bool HasAttribute<T>(IEnumerable<object> attributes) where T : Attribute =>
            attributes.Any(t => t.GetType() == typeof(T));

        /// <summary> Returns true if this can be casted to <see cref="Type" /></summary>
        public static bool IsCastableTo(this Type from, Type to)
        {
            if (to.IsAssignableFrom(from)) return true;
            IEnumerable<MethodInfo> methods = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(
                    m => m.ReturnType == to &&
                        (m.Name == "op_Implicit" ||
                            m.Name == "op_Explicit")
                );
            return methods.Any();
        }

        /// <summary> Return a pretty field type name. </summary>
        public static string PrettyName(this Type type)
        {
            if (type == null) return "null";
            if (type == typeof(object)) return "object";
            if (type == typeof(float)) return "float";
            if (type == typeof(int)) return "int";
            if (type == typeof(long)) return "long";
            if (type == typeof(double)) return "double";
            if (type == typeof(string)) return "string";
            if (type == typeof(bool)) return "bool";

            if (type.IsGenericType)
            {
                string s = "";
                Type genericType = type.GetGenericTypeDefinition();
                s = genericType == typeof(List<>) ? "List" : type.GetGenericTypeDefinition().ToString();

                Type[] types = type.GetGenericArguments();
                string[] stringTypes = new string[types.Length];
                for (int i = 0; i < types.Length; i++) stringTypes[i] = types[i].PrettyName();
                return s + "<" + string.Join(", ", stringTypes) + ">";
            }

            if (!type.IsArray) return type.ToString();
            {
                string rank = "";
                for (int i = 1; i < type.GetArrayRank(); i++) rank += ",";
                Type elementType = type.GetElementType();
                if (elementType is { IsArray: false }) return elementType.PrettyName() + "[" + rank + "]";

                {
                    string s = elementType.PrettyName();
                    int i = s.IndexOf('[');
                    return s.Substring(0, i) + "[" + rank + "]" + s.Substring(i);
                }
            }
        }
    }
}
