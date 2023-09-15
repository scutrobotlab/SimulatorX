using UnityEngine;

namespace Misc
{
    public class ClientAudioObject : MonoBehaviour
    {
        public GameObject audioPlayer;

        private void Start()
        {
#if UNITY_SERVER
           audioPlayer.SetActive(false);
#endif
        }
    }
}