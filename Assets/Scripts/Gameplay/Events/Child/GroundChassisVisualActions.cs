using Infrastructure.Child;
using UnityEngine;

namespace Infrastructure.Child
{
    public static partial class ChildActionID
    {
        public static class GroundChassisVisual
        {
            public const string UpdateStatus = "UpdateStatus";
        }
    }
}

namespace Gameplay.Events.Child
{
    /// <summary>
    /// 更新车轮转速。
    /// </summary>
    public class UpdateStatus : IChildAction
    {
        // TODO：更真实的分轮转速。
        public string ActionName() => ChildActionID.GroundChassisVisual.UpdateStatus;

        public ChildIdentity ReceiverChildType() => new ChildIdentity(ChildType.GroundChassisVisual);

        public Vector3 Speed;

        public bool Spinning;
    }
}