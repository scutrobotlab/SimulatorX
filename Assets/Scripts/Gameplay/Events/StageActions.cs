using Infrastructure;
using UnityEngine;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Stage
        {
            public const string Crash = "Crash";
            public const string StartCountdown = "StartCountdown";
            public const string PowerRuneActivating = "PowerRuneActivating";
            public const string PowerRuneActivated = "PowerRuneActivated";
            public const string PowerRuneDelay = "PowerRuneDaly";
            public const string OutpostFall = "OutpostFall";
            public const string SentinelFall = "SentinelFall";
            public const string CentralBuffActivated = "CentralBuffActivated";
            public const string GameOver = "GameOver";
            public const string Penalty = "Penalty";
            public const string Ejected = "Ejected";
            public const string Kill = "Kill";
            public const string TheCommandingPoint = "TheCommandingPoint";
            public const string LeaveHighLand = "LeaveHighLand";
            public const string DartFire = "DartFire";
            public const string DroneWarning = "DroneWarning";
            public const string OpenLaunchStation = "OpenLaunchStation";
            public const string OccupyControlArea = "OccupyControlArea";
            public const string LeaveControlArea = "LeaveControlArea";
            public const string AtPatrol = "AtPatrol";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// 5秒倒计时开始。
    /// </summary>
    public class StartCountdown : IAction
    {
        public string ActionName() => ActionID.Stage.StartCountdown;
    }

    /// <summary>
    /// 飞镖闸门打开指令
    /// </summary>
    public class OpenLaunchStation : IAction
    {
        public Identity.Camps Camp;
        public Identity.Roles Role;
        public string ActionName() => ActionID.Stage.OpenLaunchStation;
    }
    
    /// <summary>
    /// 飞镖发射指令
    /// </summary>
    public class DartFire : IAction
    {
        public Identity.Camps Camp;
        public Identity.Roles Role;
        public float Error = 0;
        public string ActionName() => ActionID.Stage.DartFire;
    }
    
    
    /// <summary>
    /// 占领环形高地。
    /// </summary>
    public class TheCommandingPoint : IAction
    {
        public Identity.Camps Camp;
        public string ActionName() => ActionID.Stage.TheCommandingPoint;
    }
    
    /// <summary>
    /// 离开环形高地
    /// </summary>
    public class LeaveHighLand : IAction
    {
        public Identity.Camps Camp;
        public string ActionName() => ActionID.Stage.LeaveHighLand;
    }
    
    /// <summary>
    /// 能量机关正在激活状态。
    /// </summary>
    public class PowerRuneActivating : IAction
    {
        public Identity.Camps Camp;
        public bool Activating;
        public string ActionName() => ActionID.Stage.PowerRuneActivating;
    }
    
    /// <summary>
    /// 能量机关已被激活。
    /// </summary>
    public class PowerRuneActivated : IAction
    {
        public Identity.Camps Camp;
        public bool IsLarge;
        public double ActivatedTime;
        public int Score;
        public string ActionName() => ActionID.Stage.PowerRuneActivated;
    }
    
    /// <summary>
    /// 对方阵营能量机关延迟。
    /// </summary>
    public class PowerRuneDelay : IAction
    {
        public Identity.Camps Camp;
        public double ActivatedTime;
        public int Score;
        public string ActionName() => ActionID.Stage.PowerRuneDelay;
    }

    /// <summary>
    /// 前哨站被击毁。
    /// </summary>
    public class OutpostFall : IAction
    {
        public Identity.Camps Camp;
        public string ActionName() => ActionID.Stage.OutpostFall;
    }
    
    
    /// <summary>
    /// 车辆猛烈碰撞。
    /// </summary>
    public class Crash : IAction
    {
        public Identity.Camps Camp;
        public Identity.Roles Role;
        public string ActionName() => ActionID.Stage.Crash;
    }

    /// <summary>
    /// 哨兵被击毁。
    /// </summary>
    public class SentinelFall : IAction
    {
        public Identity.Camps Camp;
        public string ActionName() => ActionID.Stage.SentinelFall;
    }

    /// <summary>
    /// RMUL 中心增益区已激活。
    /// </summary>
    public class CentralBuffActivated : IAction
    {
        public string ActionName() => ActionID.Stage.CentralBuffActivated;
    }

    /// <summary>
    /// RMUC 占领中央控制区。
    /// </summary>
    public class OccupyControlArea : IAction
    {
        public Identity.Camps Camp;
        public string ActionName() => ActionID.Stage.OccupyControlArea;
    }
    
    /// <summary>
    /// RMUC 离开中央控制区。
    /// </summary>
    public class LeaveControlArea : IAction
    {
        public Identity.Camps Camp;
        public string ActionName() => ActionID.Stage.LeaveControlArea;
    }
    
    /// <summary>
    /// 比赛结束。
    /// </summary>
    public class GameOver : IAction
    {
        public Identity.Camps WinningCamp;
        public string Description;
        public string ActionName() => ActionID.Stage.GameOver;
    }

    /// <summary>
    /// 机器人被罚下。
    /// </summary>
    public class Ejected : IAction
    {
        public Identity target;
        public string Description;
        public string ActionName() => ActionID.Stage.Ejected;
    }

    /// <summary>
    /// 机器人受到判罚。
    /// </summary>
    public class Penalty : IAction
    {
        public Identity target;
        public string Description;
        public string ActionName() => ActionID.Stage.Penalty;
    }

    /// <summary>
    /// 实体被击杀/摧毁。
    /// </summary>
    public class Kill : IAction
    {
        public Identity killer;
        public Identity victim;
        public string method;
        public string ActionName() => ActionID.Stage.Kill;
    }
    
    /// <summary>
    /// 给云台手的提醒
    /// </summary>
    public class DroneWarning : IAction
    {
        public enum WarningType
        {
            OreFall,
            PowerRune,
            Other
        }
        public WarningType warningType;
        public Identity.Roles Role;
        public Identity.Camps Camps;
        public string ActionName() => ActionID.Stage.DroneWarning;
        
    }
    
    /// <summary>
    /// 机器人在巡逻区内。
    /// </summary>
    public class AtPatrol : IAction
    {
        public Identity Id;
        public bool IsIn;
        public string ActionName() => ActionID.Stage.AtPatrol;
    }
}