using System;
using Infrastructure;
using Random = UnityEngine.Random;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Clock
        {
            public const string CoinNaturalGrowth = "CoinNaturalGrowth";
            public const string PowerRuneAvailable = "PowerRuneAvailable";
            public const string ActivateOutpost = "ActivateOutpost";
            public const string DropOre = "DropOre";
            public const string StartChecking = "StartChecking";
            public const string PartyTime = "PartyTime";
            public const string EngineerBuff = "EngineerBuff";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// 经济自然增长。
    /// </summary>
    public class CoinNaturalGrowth : IAction
    {
        public int MinuteCoin;
        public bool NotNature;
        public string ActionName() => ActionID.Clock.CoinNaturalGrowth;
    }

    /// <summary>
    /// 能量机关可激活。
    /// </summary>
    public class PowerRuneAvailable : IAction
    {
        public bool IsLarge;
        public bool Available;
        public readonly float A;
        public readonly float B;
        public readonly float W;
        public string ActionName() => ActionID.Clock.PowerRuneAvailable;

        public PowerRuneAvailable()
        {
            A = (float) Math.Round(Random.Range(0.780f, 1.045f), 3);
            B = 2.090f - A;
            W = (float) Math.Round(Random.Range(1.884f, 2.000f), 3);
        }
    }

    /// <summary>
    /// 前哨站装甲板旋转。
    /// </summary>
    public class ActivateOutpost : IAction
    {
        public bool Activate;
        public string ActionName() => ActionID.Clock.ActivateOutpost;
    }

    /// <summary>
    /// 释放矿石。
    /// </summary>
    public class DropOre : IAction
    {
        public int Index;
        public string ActionName() => ActionID.Clock.DropOre;
    }
    
    /// <summary>
    /// 开始检测是否有机器人抢跑。
    /// </summary>
    public class StartChecking : IAction
    {
        public string ActionName() => ActionID.Clock.StartChecking;
    }
    
    /// <summary>
    /// 工程开局增益防御50%。
    /// </summary>
    public class EngineerBuff : IAction
    {
        public bool canGet;
        public string ActionName() => ActionID.Clock.EngineerBuff;
    }
    
    
    /// <summary>
    /// 开始检测是否有机器人抢跑。
    /// </summary>
    public class PartyTime : IAction
    {
        public string ActionName() => ActionID.Clock.PartyTime;
    }
    
}