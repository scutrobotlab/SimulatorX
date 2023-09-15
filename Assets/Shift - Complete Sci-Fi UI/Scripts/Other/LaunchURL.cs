using UnityEngine;

namespace Michsky.UI.Shift
{
    public class LaunchURL : MonoBehaviour
    {
        public string URL;

        public void GoToURL()
        {
            Application.OpenURL(URL);
        }
    }
}