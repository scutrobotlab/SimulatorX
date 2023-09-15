using UnityEngine;

namespace Misc
{
    public class ClientVideoObject:MonoBehaviour
    {
        public GameObject videoPlayer;
        private void Start()
        {
            var isServerOnly = false;
#if UNITY_SERVER
            isServerOnly = true;
#endif
            if (!isServerOnly) videoPlayer.SetActive(true);
        }
    }
}