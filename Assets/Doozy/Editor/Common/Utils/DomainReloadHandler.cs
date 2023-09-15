// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Utils;
using UnityEngine;

namespace Doozy.Editor.Common.Utils
{
    public static class DomainReloadHandler
    {
        private static Assembly doozyEditorAssembly => ReflectionUtils.doozyEditorAssembly;
        private static Assembly doozyRuntimeAssembly => ReflectionUtils.doozyRuntimeAssembly;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnRuntimeLoad()
        {
            // ReSharper disable NotAccessedVariable
            int clearedValues = 0;
            int executedMethods = 0;
            // ReSharper restore NotAccessedVariable

            foreach (MemberInfo member in GetMembers<ClearOnReloadAttribute>(true))
            {
                //Fields
                {
                    var field = member as FieldInfo;

                    if (field != null && !field.FieldType.IsGenericParameter && field.IsStatic)
                    {
                        Type fieldType = field.FieldType;
                        ClearOnReloadAttribute reloadAttribute = field.GetCustomAttribute<ClearOnReloadAttribute>();
                        object valueOnReload = reloadAttribute?.ValueOnReload;
                        bool createNewInstance = reloadAttribute != null && reloadAttribute.CreateNewInstance;
                        dynamic value = valueOnReload != null ? Convert.ChangeType(valueOnReload, fieldType) : null;
                        if (createNewInstance) value = Activator.CreateInstance(fieldType);

                        try
                        {
                            field.SetValue(null, value);
                        }
                        catch
                        {
                            // ignored
                        }

                        clearedValues++;
                    }
                }

                //Properties
                {

                    var property = member as PropertyInfo;

                    if (property != null && !property.PropertyType.IsGenericParameter && property.GetAccessors(true).Any(x => x.IsStatic))
                    {
                        Type fieldType = property.PropertyType;
                        ClearOnReloadAttribute reloadAttribute = property.GetCustomAttribute<ClearOnReloadAttribute>();
                        object valueOnReload = reloadAttribute?.ValueOnReload;
                        bool createNewInstance = reloadAttribute != null && reloadAttribute.CreateNewInstance;
                        dynamic value = valueOnReload != null ? Convert.ChangeType(valueOnReload, fieldType) : null;
                        if (createNewInstance) value = Activator.CreateInstance(fieldType);

                        try
                        {
                            property.SetValue(null, value);
                        }
                        catch
                        {
                            // ignored
                        }

                        clearedValues++;
                    }
                }
            }

            foreach (MemberInfo member in GetMethodMembers<ExecuteOnReloadAttribute>(true))
            {
                var method = member as MethodInfo;

                if (method == null || method.IsGenericMethod || !method.IsStatic)
                    continue;
                method.Invoke(null, new object[] {});
                executedMethods++;
            }

            // Debug.Log($"Cleared {clearedValues} members, executed {executedMethods} methods");
        }

        private static IEnumerable<MemberInfo> GetMethodMembers<TAttribute>(bool inherit) where TAttribute : System.Attribute
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            var members = new List<MemberInfo>();

            //EDITOR
            try
            {
                //Methods
                members.AddRange(from t in doozyEditorAssembly.GetTypes()
                                 where t.IsClass
                                 where !t.IsGenericParameter
                                 from m in t.GetMethods(flags)
                                 where !m.ContainsGenericParameters
                                 where m.IsDefined(typeof(TAttribute), inherit)
                                 select m);
            }
            catch (ReflectionTypeLoadException)
            {
                //ignored
            }
            
            //RUNTIME
            try
            {
                //Methods
                members.AddRange(from t in doozyRuntimeAssembly.GetTypes()
                                 where t.IsClass
                                 where !t.IsGenericParameter
                                 from m in t.GetMethods(flags)
                                 where !m.ContainsGenericParameters
                                 where m.IsDefined(typeof(TAttribute), inherit)
                                 select m);
            }
            catch (ReflectionTypeLoadException)
            {
                //ignored
            }
            
            // foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            // {
            //     try
            //     {
            //         //Methods
            //         members.AddRange(from t in assembly.GetTypes()
            //                          where t.IsClass
            //                          where !t.IsGenericParameter
            //                          from m in t.GetMethods(flags)
            //                          where !m.ContainsGenericParameters
            //                          where m.IsDefined(typeof(TAttribute), inherit)
            //                          select m);
            //     }
            //     catch (ReflectionTypeLoadException)
            //     {
            //         //ignored
            //     }
            // }
            
            return members;
        }

        private static IEnumerable<MemberInfo> GetMembers<TAttribute>(bool inherit) where TAttribute : System.Attribute
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            var members = new List<MemberInfo>();

            //EDITOR
            try
            {
                foreach (Type type in doozyEditorAssembly.GetTypes())
                {
                    if (!type.IsClass) continue;

                    //Fields
                    members.AddRange(type.GetFields(flags).Cast<MemberInfo>().Where(member => member.IsDefined(typeof(TAttribute), inherit)));

                    //Properties
                    members.AddRange(type.GetProperties(flags).Cast<MemberInfo>().Where(member => member.IsDefined(typeof(TAttribute), inherit)));

                    //Events
                    members.AddRange((from eventInfo in type.GetEvents(flags) where eventInfo.IsDefined(typeof(TAttribute), inherit) select GetEventField(type, eventInfo.Name)).Cast<MemberInfo>());
                }

            }
            catch (ReflectionTypeLoadException)
            {
                //ignored
            }
            
            //RUNTIME
            try
            {
                foreach (Type type in doozyRuntimeAssembly.GetTypes())
                {
                    if (!type.IsClass) continue;

                    //Fields
                    members.AddRange(type.GetFields(flags).Cast<MemberInfo>().Where(member => member.IsDefined(typeof(TAttribute), inherit)));

                    //Properties
                    members.AddRange(type.GetProperties(flags).Cast<MemberInfo>().Where(member => member.IsDefined(typeof(TAttribute), inherit)));

                    //Events
                    members.AddRange((from eventInfo in type.GetEvents(flags) where eventInfo.IsDefined(typeof(TAttribute), inherit) select GetEventField(type, eventInfo.Name)).Cast<MemberInfo>());
                }

            }
            catch (ReflectionTypeLoadException)
            {
                //ignored
            }

            // foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            // {
            //     try
            //     {
            //         foreach (Type type in assembly.GetTypes())
            //         {
            //             if (!type.IsClass) continue;
            //
            //             //Fields
            //             members.AddRange(type.GetFields(flags).Cast<MemberInfo>().Where(member => member.IsDefined(typeof(TAttribute), inherit)));
            //
            //             //Properties
            //             members.AddRange(type.GetProperties(flags).Cast<MemberInfo>().Where(member => member.IsDefined(typeof(TAttribute), inherit)));
            //
            //             //Events
            //             members.AddRange((from eventInfo in type.GetEvents(flags) where eventInfo.IsDefined(typeof(TAttribute), inherit) select GetEventField(type, eventInfo.Name)).Cast<MemberInfo>());
            //         }
            //
            //     }
            //     catch (ReflectionTypeLoadException)
            //     {
            //         //ignored
            //     }
            // }


            return members;
        }

        private static FieldInfo GetEventField(Type type, string eventName)
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo field = null;

            while (type != null)
            {

                //Events defined as field
                field = type.GetField(eventName, flags);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                    break;

                //Events defined as property { add; remove; }
                field = type.GetField(EventName(eventName), flags);
                if (field != null)
                    break;
                type = type.BaseType;
            }
            return field;
        }

        private static string EventName(string eventName)
            => $"EVENT_{eventName.ToUpper()}";
    }
}
