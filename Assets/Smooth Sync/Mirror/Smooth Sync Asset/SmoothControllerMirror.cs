using Mirror;
using UnityEngine;

namespace Smooth
{
    public class SmoothControllerMirror : MonoBehaviour
    {
        public static bool isHandlerRegistered = false;

        private void Update()
        {
            if ((NetworkServer.active || NetworkClient.active) && !isHandlerRegistered)
            {
                RegisterHandlers();
            }
            if (!NetworkServer.active && !NetworkClient.active && isHandlerRegistered)
            {
                isHandlerRegistered = false;
            }
        }

        public static void RegisterHandlers()
        {
            NetworkServer.ReplaceHandler<NetworkStateMirror>(SmoothSyncMirror.HandleSync);
            NetworkClient.ReplaceHandler<NetworkStateMirror>(SmoothSyncMirror.HandleSync);
            isHandlerRegistered = true;
        }
    }
}
