using Gameplay.Attribute;
using Infrastructure;
using UnityEngine;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class DroneControl
        {
            //public const string UpdateDrone = "UpdateDrone";
            public const string RequestDroneSupport = "RequestDroneSupport";
            public const string AgreeDroneSupport = "AgreeDroneSupport";
            public const string StartDroneCount = "StartDroneCount";
            public const string AutoDriveTarget = "AutoDriveTarget";
        }
    }
}

namespace Gameplay.Events
{
    /*public class UpdateDrone : IAction
    {
        public string ActionName() => ActionID.DroneControl.UpdateDrone;
        public Identity.Camps Camp;
        public Vector2 primaryAxisInput;
        public Vector2 secondAxisInput;
    }*/
    public class RequestDroneSupport : IAction
    {
        public string ActionName() => ActionID.DroneControl.RequestDroneSupport;
        public Identity.Camps Camp;
        public double RequestTime ;
    }
    public class AgreeDroneSupport : IAction
    {
        public string ActionName() => ActionID.DroneControl.AgreeDroneSupport;
        public Identity.Camps Camp;
        public bool Agree;
        public bool Timestart;
        public double StopTime;
    }
    
    public class StartDroneCount: IAction
    {
        public string ActionName() => ActionID.DroneControl.StartDroneCount;
    }
    public class AutoDriveTarget : IAction
    {
        public string ActionName() => ActionID.DroneControl.AutoDriveTarget;
        public Identity.Camps Camp;
        public Vector3 Pos;
    }

    
}