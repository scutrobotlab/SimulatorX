using Mirror;
using UnityEngine;

namespace Lab
{
    /// <summary>
    /// 启动游戏自动进入单机调试模式。
    /// </summary>
    public class AutoStartHost : MonoBehaviour
    {
        private void Start()
        {
            NetworkManager.singleton.StartHost();
        }
    }
}