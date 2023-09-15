using Infrastructure;
using Mirror;
using UI;
using UnityEngine;

namespace Gameplay.Networking
{
    /// <summary>
    /// 用于存储已选择角色的 RoomPlayer。
    /// </summary>
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        public Identity chosenRobot = new Identity();

        /// <summary>
        /// 选择角色时通知服务器。
        /// </summary>
        /// <param name="robot">选择的角色</param>
        [Command]
        public void CmdChooseRobot(Identity robot)
        {
            chosenRobot = robot;
            ChooseRobotRpc(robot);
        }

        /// <summary>
        /// 服务器通知所有客户端。
        /// </summary>
        /// <param name="robot">选择的角色</param>
        /// TODO：奇怪的 Identity 同步 bug。
        [ClientRpc]
        private void ChooseRobotRpc(Identity robot)
        {
            chosenRobot = robot;
        }

        /// <summary>
        /// 初始化界面。
        /// </summary>
        public override void OnClientEnterRoom()
        {
            if (isLocalPlayer)
            {
                var roleSelectionUI = FindObjectOfType<RoleSelectionPanel>();
                if (roleSelectionUI)
                {
                    roleSelectionUI.localRoomPlayer = this;
                }
            }
        }

        /// <summary>
        /// 防止绘制默认 GUI。
        /// </summary>
        public override void OnGUI()
        {
        }
    }
}