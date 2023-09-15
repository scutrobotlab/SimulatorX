// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Runtime;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using UnityEditor;

namespace Doozy.Editor.Signals.Automation.Generators
{
    internal static class SignalProviderExtensionGenerator
    {
        // [InitializeOnLoadMethod]
        // private static void Tester() => Run();

        private static string templateName => nameof(SignalProviderExtensionGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Signals/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{RuntimePath.path}/Signals/{targetFileNameWithExtension}";

        private static StringBuilder sb { get; set; }

        private static List<ProviderAttributes> globalProviderByType { get; set; }
        private static List<string> globalProviderCategories { get; set; }
        private static Dictionary<string, List<string>> globalProviderNames { get; set; }

        private static List<ProviderAttributes> localProviderByType { get; set; }
        private static List<string> localProviderCategories { get; set; }
        private static Dictionary<string, List<string>> localProviderNames { get; set; }

        internal static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            if (data.IsNullOrEmpty()) return false;

            #region Prepare Data

            sb = new StringBuilder();
            globalProviderByType = new List<ProviderAttributes>();
            globalProviderCategories = new List<string>();
            globalProviderNames = new Dictionary<string, List<string>>();

            localProviderByType = new List<ProviderAttributes>();
            localProviderCategories = new List<string>();
            localProviderNames = new Dictionary<string, List<string>>();

            foreach (ProviderAttributes attributes in SignalsUtils.providerAttributesSet)
            {
                ProviderType pType = attributes.id.Type;
                string pCategory = attributes.id.Category;
                string pName = attributes.id.Name;

                switch (pType)
                {
                    case ProviderType.Global:
                        globalProviderByType.Add(attributes);

                        if (!globalProviderCategories.Contains(pCategory))
                        {
                            globalProviderCategories.Add(pCategory);
                        }

                        if (!globalProviderNames.ContainsKey(pCategory))
                        {
                            globalProviderNames.Add(pCategory, new List<string> { pName });
                        }
                        else if (!globalProviderNames[pCategory].Contains(pName))
                        {
                            globalProviderNames[pCategory].Add(pName);
                        }
                        break;
                    case ProviderType.Local:
                        localProviderByType.Add(attributes);

                        if (!localProviderCategories.Contains(pCategory))
                        {
                            localProviderCategories.Add(pCategory);
                        }

                        if (!localProviderNames.ContainsKey(pCategory))
                        {
                            localProviderNames.Add(pCategory, new List<string> { pName });
                        }
                        else if (!localProviderNames[pCategory].Contains(pName))
                        {
                            localProviderNames[pCategory].Add(pName);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            globalProviderCategories.Sort();
            foreach (string key in globalProviderNames.Keys)
                globalProviderNames[key].Sort();

            localProviderCategories.Sort();
            foreach (string key in localProviderNames.Keys)
                localProviderNames[key].Sort();

            #endregion

            //LOCAL - GetProviderCategories()
            sb.Clear();
            foreach (string localProviderCategory in localProviderCategories)
                sb.AppendLine($"                    {localProviderCategory}.k_ProviderCategory,");
            data = data.Replace("//Local_GetProviderCategories//", sb.ToString());

            //LOCAL - GetProviderNames()
            sb.Clear();
            foreach (string localProviderCategory in localProviderCategories)
                sb.AppendLine($"                    case {localProviderCategory}.k_ProviderCategory: return {localProviderCategory}.GetProviderNames();");
            data = data.Replace("//Local_GetProviderNames//", sb.ToString());

            //LOCAL - GetAttributesList()
            sb.Clear();
            foreach (string localProviderCategory in localProviderCategories)
                sb.AppendLine($"                    case {localProviderCategory}.k_ProviderCategory: return {localProviderCategory}.AttributesList;");
            data = data.Replace("//Local_GetAttributesList//", sb.ToString());

            //LOCAL PROVIDERS
            sb.Clear();
            foreach (string localProviderCategory in localProviderCategories)
            {
                sb.AppendLine($"            public static class {localProviderCategory}");
                sb.AppendLine("            {");
                sb.AppendLine($"                public const string k_ProviderCategory = nameof({localProviderCategory});");
                sb.AppendLine();
                sb.AppendLine($"                public static IEnumerable<string> GetProviderNames() => Enum.GetValues(typeof(Name)).Cast<Name>().Select(name => name.ToString());");
                sb.AppendLine($"                public static ProviderId GetProviderId(Name providerName) => new ProviderId(k_ProviderType, k_ProviderCategory, providerName.ToString());");
                sb.AppendLine($"                public static ISignalProvider Get(Name providerName, GameObject signalSource) => SignalsService.GetProvider(GetProviderId(providerName), signalSource);");
                sb.AppendLine();
                sb.AppendLine($"                public enum Name");
                sb.AppendLine("                {");
                foreach (string localProviderName in localProviderNames[localProviderCategory])
                    sb.AppendLine($"                    {localProviderName},");
                sb.AppendLine("                }");
                sb.AppendLine();
                sb.AppendLine($"                public static readonly List<ProviderAttributes> AttributesList = new List<ProviderAttributes>");
                sb.AppendLine("                {");
                foreach (string localProviderName in localProviderNames[localProviderCategory])
                {
                    Type providerType = localProviderByType
                        .Where(t => t.id.Type == ProviderType.Local)
                        .Where(t => t.id.Category.Equals(localProviderCategory))
                        .First(t => t.id.Name.Equals(localProviderName))
                        .typeOfProvider;
                    sb.AppendLine($"                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.{localProviderName}.ToString(), typeof({providerType.FullName})),");
                }
                sb.AppendLine("                };");
                sb.AppendLine("            }");
                sb.AppendLine();
            }
            data = data.Replace("//Local_Providers//", sb.ToString());

            //GLOBAL - GetProviderCategories()
            sb.Clear();
            foreach (string globalProviderCategory in globalProviderCategories)
                sb.AppendLine($"                    {globalProviderCategory}.k_ProviderCategory,");
            data = data.Replace("//Global_GetProviderCategories//", sb.ToString());

            //GLOBAL - GetProviderNames()
            sb.Clear();
            foreach (string globalProviderCategory in globalProviderCategories)
                sb.AppendLine($"                    case {globalProviderCategory}.k_ProviderCategory: return {globalProviderCategory}.GetProviderNames();");
            data = data.Replace("//Global_GetProviderNames//", sb.ToString());

            //GLOBAL - GetAttributesList()
            sb.Clear();
            foreach (string globalProviderCategory in globalProviderCategories)
                sb.AppendLine($"                    case {globalProviderCategory}.k_ProviderCategory: return {globalProviderCategory}.AttributesList;");
            data = data.Replace("//Global_GetAttributesList//", sb.ToString());

            //GLOBAL PROVIDERS
            sb.Clear();
            foreach (string globalProviderCategory in globalProviderCategories)
            {
                sb.AppendLine($"            public static class {globalProviderCategory}");
                sb.AppendLine("            {");
                sb.AppendLine($"                public const string k_ProviderCategory = nameof({globalProviderCategory});");
                sb.AppendLine();
                sb.AppendLine($"                public static IEnumerable<string> GetProviderNames() => Enum.GetValues(typeof(Name)).Cast<Name>().Select(name => name.ToString());");
                sb.AppendLine($"                public static ProviderId GetProviderId(Name providerName) => new ProviderId(k_ProviderType, k_ProviderCategory, providerName.ToString());");
                sb.AppendLine($"                public static ISignalProvider Get(Name providerName, GameObject signalSource) => SignalsService.GetProvider(GetProviderId(providerName), signalSource);");
                sb.AppendLine();
                sb.AppendLine($"                public enum Name");
                sb.AppendLine("                {");
                foreach (string globalProviderName in globalProviderNames[globalProviderCategory])
                    sb.AppendLine($"                    {globalProviderName},");
                sb.AppendLine("                }");
                sb.AppendLine();
                sb.AppendLine($"                public static readonly List<ProviderAttributes> AttributesList = new List<ProviderAttributes>");
                sb.AppendLine("                {");
                foreach (string globalProviderName in globalProviderNames[globalProviderCategory])
                {
                    Type providerType = globalProviderByType
                        .Where(t => t.id.Type == ProviderType.Global)
                        .Where(t => t.id.Category.Equals(globalProviderCategory))
                        .First(t => t.id.Name.Equals(globalProviderName))
                        .typeOfProvider;
                    sb.AppendLine($"                    new ProviderAttributes(k_ProviderType, k_ProviderCategory, Name.{globalProviderName}.ToString(), typeof({providerType.FullName})),");
                }
                sb.AppendLine("                };");
                sb.AppendLine("            }");
                sb.AppendLine();
            }
            data = data.Replace("//Global_Providers//", sb.ToString());

            bool result = FileGenerator.WriteFile(targetFilePath, data);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }
    }
}
