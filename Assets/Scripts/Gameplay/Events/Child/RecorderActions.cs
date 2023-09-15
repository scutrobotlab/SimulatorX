using Infrastructure;
using Infrastructure.Child;
using UnityEngine;
using ActionID = Infrastructure.ActionID;

namespace Infrastructure.Child
{
    public static partial class ChildActionID
    {
        public static class Recorder
        {
            public const string Rectify = "Rectify";
        }
    }
}

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Recorder
        {
            public const string MechanicSelect = "MechanicSelect";
            public const string RobotSpawn = "RobotSpawn";
            public const string SentinelMove = "SentinelMove";
        }
    }
}

namespace Gameplay.Events.Child
{
    public class Rectify : IChildAction
    {
        public ChildIdentity Receiver;
        public Vector3 Position;
        public Quaternion Rotation;
        public string ActionName() => ChildActionID.Recorder.Rectify;
        public ChildIdentity ReceiverChildType() => Receiver;
    }

    public class MechanicSelect : IAction
    {
        public Identity Receiver;
        public string Gun;
        public string Chassis;
        public string ActionName() => ActionID.Recorder.MechanicSelect;
    }

    public class RobotSpawn : IAction
    {
        public Identity Robot;
        public string ActionName() => ActionID.Recorder.RobotSpawn;
    }

    public class SentinelMove : IAction
    {
        public Identity Receiver;
        public int CurrentPosition;
        public float Duration;
        public string ActionName() => ActionID.Recorder.SentinelMove;
    }
}