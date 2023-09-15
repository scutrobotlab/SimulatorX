using UnityEngine;

namespace Michsky.UI.Shift
{
    public class ExitToSystem : MonoBehaviour
    {
        public void ExitGame()
        {
            Debug.Log("Exit function is working on build mode.");
            Application.Quit();
        }
    }
}