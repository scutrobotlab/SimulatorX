using System.Globalization;
using Infrastructure;
using UnityEngine;

namespace Config
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        private bool _match;
        private string[] _sp;

        public static void SetString(string key, string value)
            => PlayerPrefs.SetString(key, value);

        public static string GetString(string key, string defaultValue = "")
            => PlayerPrefs.GetString(key, defaultValue);

        public void ConfigureString(string expr)
        {
            _sp = expr.Split('=');
            SetString(_sp[0], _sp[1]);
        }

        public static void SetFloat(string key, float value)
            => PlayerPrefs.SetFloat(key, value);

        public static float GetFloat(string key, float defaultValue = 0)
            => PlayerPrefs.GetFloat(key, defaultValue);

        public void ConfigureFloat(string expr)
        {
            _sp = expr.Split('=');
            SetFloat(_sp[0], float.Parse(_sp[1], CultureInfo.InvariantCulture));
        }

        public static void SetBool(string key, bool value)
            => PlayerPrefs.SetInt(key, value ? 1 : 0);

        public static bool GetBool(string key, bool defaultValue = false)
            => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;

        public void ConfigureBool(string expr)
        {
            _sp = expr.Split('=');
            SetBool(_sp[0], bool.Parse(_sp[1]));
        }
    }
}