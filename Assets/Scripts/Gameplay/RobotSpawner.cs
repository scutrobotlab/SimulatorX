using System.Collections.Generic;
using System.Linq;
using Gameplay.Networking;
using Infrastructure;
using Mirror;

namespace Gameplay
{
    public class RobotSpawner : StoreBase
    {
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Recorder.RobotSpawn
            }).ToList();
        }

        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Recorder.RobotSpawn:
                    if (!Dispatcher.Instance().replay) break;
                    var spawnAction = (Gameplay.Events.Child.RobotSpawn) action;
                    ((NetworkRoomManagerExt) NetworkManager.singleton).SpawnRobot(spawnAction.Robot, null);
                    Dispatcher.Instance().UpdateStoreCache();
                    break;
            }
        }
    }
}