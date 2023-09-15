using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.Signals.ScriptableObjects;
using Doozy.Runtime;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using UnityEditor;
namespace Doozy.Editor.Signals.Automation.Generators
{
    public class StreamIdExtensionGenerator
    {
        private static string templateName => nameof(StreamIdExtensionGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Signals/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{RuntimePath.path}/Signals/{targetFileNameWithExtension}";
        
        public static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false, bool silent = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            if (data.IsNullOrEmpty()) return false;
            StreamIdDataGroup dataGroup = StreamIdDatabase.instance.database;
            if (!StreamIdDatabase.instance.database.isEmpty)
                data = InjectContent(data, dataGroup.GetCategories, category => dataGroup.GetNames(category));
            bool result = FileGenerator.WriteFile(targetFilePath, data, silent);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }

          private static string InjectContent(string templateData, Func<IEnumerable<string>> getCategories, Func<string, IEnumerable<string>> getNames)
        {
            var accessorStringBuilder = new StringBuilder();
            var dataStringBuilder = new StringBuilder();
            var categories = getCategories.Invoke().ToList();
            int categoriesCount = categories.Count;
            for (int categoryIndex = 0; categoryIndex < categories.Count; categoryIndex++)
            {
                string category = categories[categoryIndex];
                if (category.Equals(CategoryNameItem.k_DefaultCategory)) continue;
                var names = getNames.Invoke(category).ToList();

                //ACCESSOR//
                {
                    accessorStringBuilder.AppendLine($"        public static bool Send({nameof(StreamId)}.{category} id, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), message);");
                    accessorStringBuilder.AppendLine($"        public static bool Send({nameof(StreamId)}.{category} id, GameObject signalSource, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalSource, message);");
                    accessorStringBuilder.AppendLine($"        public static bool Send({nameof(StreamId)}.{category} id, SignalProvider signalProvider, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalProvider, message);");
                    accessorStringBuilder.AppendLine($"        public static bool Send({nameof(StreamId)}.{category} id, Object signalSender, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalSender, message);");
                    accessorStringBuilder.AppendLine($"        public static bool Send<T>({nameof(StreamId)}.{category} id, T signalValue, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalValue, message);");
                    accessorStringBuilder.AppendLine($"        public static bool Send<T>({nameof(StreamId)}.{category} id, T signalValue, GameObject signalSource, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalValue, signalSource, message);");
                    accessorStringBuilder.AppendLine($"        public static bool Send<T>({nameof(StreamId)}.{category} id, T signalValue, SignalProvider signalProvider, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalValue, signalProvider, message);");
                    accessorStringBuilder.AppendLine($"        public static bool Send<T>({nameof(StreamId)}.{category} id, T signalValue, Object signalSender, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalValue, signalSender, message);");
                    if (categoryIndex < categoriesCount - 1) accessorStringBuilder.AppendLine();
                }

                //DATA//
                {
                    dataStringBuilder.AppendLine($"        public enum {category}");
                    dataStringBuilder.AppendLine("        {");
                    for (int nameIndex = 0; nameIndex < names.Count; nameIndex++)
                    {
                        string name = names[nameIndex];
                        dataStringBuilder.AppendLine($"            {name}{(nameIndex < names.Count - 1 ? "," : "")}");
                    }
                    dataStringBuilder.AppendLine("        }");
                    if (categoryIndex < categoriesCount - 2) dataStringBuilder.AppendLine();
                }
            }

            templateData = templateData.Replace("//ACCESSOR//", accessorStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            templateData = templateData.Replace("//DATA//", dataStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            return templateData;
        }
    }
}
