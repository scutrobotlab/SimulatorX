using AdditionalAssets.RobotArm.Scripts;
using Infrastructure;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class Exchange
        {
            public const string DoExchange = "DoExchange";
            public const string ExchangeHealth = "ExchangeHealth";
            public const string ChangeGrade = "ChangeGrade";
        }
    }
}

namespace Gameplay.Events
{
    /// <summary>
    /// 将矿石兑换成经济。
    /// </summary>
    public class DoExchange : IAction
    {
        public Identity.Camps Camp;
        public int OreSinglePrice;
        public string ActionName() => ActionID.Exchange.DoExchange;
    }
    
    /// <summary>
    /// 将经济换成生命。
    /// </summary>
    public class ExchangeHealth : IAction
    {
        public Identity Id;
        public int Money;
        public string ActionName() => ActionID.Exchange.ExchangeHealth;
    }
    
    /// <summary>
    /// 将经济换成生命。
    /// </summary>
    public class ChangeGrade : IAction
    {
        public Identity Id;
        public Grade Gr;
        public string ActionName() => ActionID.Exchange.ChangeGrade;
    }
}