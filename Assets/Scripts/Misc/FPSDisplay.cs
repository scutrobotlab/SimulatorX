using UnityEngine;

namespace Misc
{
    public class FPSDisplay : MonoBehaviour
    {
        private float _deltaTime = 0.0f;

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            var style = new GUIStyle();

            var rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            //new Color (0.0f, 0.0f, 0.5f, 1.0f);
            style.normal.textColor = Color.white;
            var sec = _deltaTime * 1000.0f;
            var fps = 1.0f / _deltaTime;
            var text = $"{sec:0.0} ms ({fps:0.} fps)";
            GUI.Label(rect, text, style);
        }
    }
}