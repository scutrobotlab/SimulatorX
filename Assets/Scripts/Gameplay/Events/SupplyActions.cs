using Gameplay.Attribute;
using Infrastructure;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Supply
        {
            public const string AskSupply = "AskSupply";
            public const string DoSupply = "DoSupply";
            public const string AskMedical = "AskMedical";
            public const string AskReturnCoin = "AskReturnCoin";
            public const string AskRewardCoin = "AskRewardCoin";
        }
    }
}

namespace Gameplay.Events
{
    // TODO： 更多兵种

    /*
     * 事件流动
     * Infantry  ---(AskSupply)---> CoinStore
     * CoinStore ---(DoSupply)----> DepotStore
     */

    /// <summary>
    /// <c>AskSupply</c> 事件向 <c>CoinStore</c> 请求补给。
    /// <br/>该事件由 <c>InfantryStore</c> 发出。
    /// </summary>
    public class AskSupply : IAction
    {
        public Identity Target;
        public MechanicType.CaliberType Type;
        //远程供弹
        public bool isFar;
        public string ActionName() => ActionID.Supply.AskSupply;
    }
    
    /// <summary>
    /// 该事件由HeroStore发出，返还金币
    /// </summary>
    public class AskReturnCoin : IAction
    {
        public Identity.Camps Camp;
        public int Coin;
        public string ActionName() => ActionID.Supply.AskReturnCoin;
    }
    
    /// <summary>
    /// 该事件由前哨站发出，奖励金币
    /// </summary>
    public class AskRewardCoin : IAction
    {
        public Identity.Camps Camp;
        public int Coin;
        public string ActionName() => ActionID.Supply.AskRewardCoin;
    }
    
    /// <summary>
    /// <c>DoSupply</c> 事件命令 <c>DepotStore</c> 发放弹丸。
    /// <br/>该事件由 <c>CoinStore</c> 发出。
    /// </summary>
    public class DoSupply : IAction
    {
        public Identity Target;
        public MechanicType.CaliberType Type;
        public int Amount;
        public string ActionName() => ActionID.Supply.DoSupply;
    }

    /// <summary>
    /// 请求使用血包。
    /// </summary>
    public class AskMedical : IAction
    {
        public Identity Target;
        public string ActionName() => ActionID.Supply.AskMedical;
    }
}