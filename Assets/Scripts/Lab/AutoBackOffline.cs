using Gameplay.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lab
{
    /// <summary>
    /// 在 <c>Arena</c> 场景启动游戏时，自动跳转回主页。
    /// </summary>
    public class AutoBackOffline : MonoBehaviour
    {
        private void Start()
        {
            if (FindObjectsOfType<NetworkRoomPlayerExt>().Length == 0)
            {
                SceneManager.LoadScene("Offline");
            }
        }
    }
}