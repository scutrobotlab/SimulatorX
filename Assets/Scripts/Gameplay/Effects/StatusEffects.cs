using Infrastructure;

namespace Infrastructure
{
    public static partial class EffectID
    {
        public static partial class Status
        {
            public const string AtSupply = "AtSupply";
            public const string AtExchange = "AtExchange";
            public const string TimeOut = "TimeOut";
            public const string LaunchRampActivating = "LaunchRampActivating";
            public const string OverHeat = "OverHeat";
            public const string CentralBuff = "CentralBuff";
            public const string Ejected = "Edjected";
            public const string Stabilize = "Stabilize";
            public const string DartAttack = "DartAttack";
            public const string ControlBuff = "ControlBuff";
            public const string LeavePatrol = "LeavePatrol";
        }
    }
}

namespace Gameplay.Effects
{
    // TODO：创建 AddEffectAction 和 RemoveEffectAction，附带 Action 信息

    /// <summary>
    /// <c>AtSupply</c> 效果表示机器人正在补给区内。
    /// </summary>
    public class AtSupply : EffectBase
    {
        public AtSupply() : base(EffectID.Status.AtSupply)
        {
        }
    }
    
    /// <summary>
    /// 表示机器人在兑换站
    /// </summary>
    public class AtExchange : EffectBase
    {
        public AtExchange() : base(EffectID.Status.AtExchange)
        {
        }
    }
    
    /// <summary>
    /// 效果表示机器人遭到了飞镖打击
    /// </summary>
    public class DartAttack : EffectBase
    {
        public DartAttack() : base(EffectID.Status.DartAttack)
        {
        }
    }
    
    /// <summary>
    /// <c>TimeOut</c>效果表示三秒后，撤销矿石的兑换资格
    /// </summary>
    public class TimeOut : EffectBase
    {
        public TimeOut() : base(EffectID.Status.TimeOut)
        {
        }
    }

    /// <summary>
    /// 飞坡正在激活，10秒后激活失败。
    /// </summary>
    public class LaunchRampActivating : EffectBase
    {
        public LaunchRampActivating() :
            base(
                EffectID.Status.LaunchRampActivating,
                10)
        {
        }
    }

    /// <summary>
    /// 枪口过热状态。
    /// </summary>
    public class OverHeat : EffectBase
    {
        public OverHeat() : base(EffectID.Status.OverHeat)
        {
        }
    }

    /// <summary>
    /// UL占领中央增益区。
    /// </summary>
    public class CentralBuff : EffectBase
    {
        public CentralBuff() : base(EffectID.Status.CentralBuff, 2)
        {
        }
    }

    /// <summary>
    /// UC占领中央控制区。
    /// </summary>
    public class ControlBuff : EffectBase
    {
        public ControlBuff() : base(EffectID.Status.ControlBuff)
        {
        }
    }
    /// <summary>
    /// 机器人被罚下。
    /// </summary>
    public class Ejected : EffectBase
    {
        public Ejected() : base(EffectID.Status.Ejected)
        {
        }
    }

    /// <summary>
    /// 机器人在场内（防止飞天）。
    /// </summary>
    public class Stabilize : EffectBase
    {
        public Stabilize() : base(EffectID.Status.Stabilize)
        {
        }
    }
    
    /// <summary>
    /// 哨兵离开巡逻区。
    /// </summary>
    public class LeavePatrol : EffectBase
    {
        public LeavePatrol() : base(EffectID.Status.LeavePatrol)
        {
        }
    }
}