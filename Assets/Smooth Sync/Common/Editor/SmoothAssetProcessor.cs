using System.Collections.Generic;
using UnityEditor;

namespace Smooth
{
    class SmoothAssetProcessor : AssetPostprocessor
    {
        // Checks for changes to the README file, which is guaranteed to change for each version of Smooth Sync since it has the version number in it
        // Automatically updates the networking system specific files if the folders are present
        // Otherwise shows the first time setup window so the user can select their networking system 
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            int index = ArrayUtility.IndexOf(importedAssets, "Assets/Smooth Sync/README.txt");
            if (index != -1)
            {
                List<string> validSystems = new List<string>();
                var systems = new string[] { "UNet", "Mirror", "MLAPI", "NetcodeForGameObjects", "PUN", "PUN2" };
                bool updating = false;
                foreach (var system in systems)
                {
                    if (AssetDatabase.IsValidFolder("Assets/Smooth Sync/" + system))
                    {
                        updating = true;
                        validSystems.Add(system);
                    }
                }

                if (updating)
                {
                    UpdateConfirmationWindow.ShowUpdateConfirmation(validSystems);
                }
                else
                {
                    // Brand new install, open the setup window
                    SelectNetworkingSystem.SetNetworkingSystem();
                }
            }
        }
    }
}