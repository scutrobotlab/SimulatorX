using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI.ScriptableObjects;
using UnityEditor;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Processors
{
    [InitializeOnLoad]
    public class EditorUIRefresher
    {
        static EditorUIRefresher()
        {
            ExecuteProcessor();
        }
        
        private static void ExecuteProcessor()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                DelayedCall.Run(2f, ExecuteProcessor);
                return;
            }
            
            if(!EditorUISettings.instance.AutoRefresh)
                return;
            
            DelayedCall.Run(3f, Run);
        }

        public static void Run()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                DelayedCall.Run(2f, Run);
                return;
            }
            
            EditorUISettings.instance.AutoRefresh = false;
            EditorUISettings.instance.Refresh();
            EditorUtility.SetDirty(EditorUISettings.instance);
            AssetDatabase.SaveAssets();
        }
    }
}
