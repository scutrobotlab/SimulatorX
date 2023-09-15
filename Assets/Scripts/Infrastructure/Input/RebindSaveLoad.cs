using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Infrastructure.Input
{
    public class RebindSaveLoad : MonoBehaviour
    {
        public InputActionAsset actions;

        private void Awake() => Load();

        private void OnEnable() => Load();

        private void OnDisable() => Save();

        private void OnApplicationQuit() => Save();

        private void Save()
        {
            var rebinds = actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", rebinds);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            var rebinds = PlayerPrefs.GetString("rebinds");
            if (!string.IsNullOrEmpty(rebinds))
                actions.LoadBindingOverridesFromJson(rebinds);
        }
    }
}