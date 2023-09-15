using Infrastructure;

namespace Infrastructure
{
    public static partial class EffectID
    {
        public static partial class Buffs
        {
            // 有且只有 Buff 类型 Effect 的字符串内容以 Buff_ 开头
            public const string Base = "Buff_Base";
            public const string DroneBase = "Buff_DroneBase";
            public const string Outpost = "Buff_Outpost";
            public const string HighlandCool = "Buff_HighlandCool";
            public const string HighlandCoin = "Buff_HighlandCoin";
            public const string Revive = "Buff_Revive";
            public const string Snipe = "Buff_Snipe";
            public const string LaunchRamp = "Buff_LaunchRamp";
            public const string SmallPowerRune = "Buff_SmallPowerRune";
            public const string LargePowerRune = "Buff_LargePowerRune";
            public const string Revival = "Buff_Revival";
            public const string Medical = "Buff_Medical";
            public const string CardRevive = "Buff_CardRevive";
            public const string OneLargePowerRune = "Buff_OneLargePowerRune";
            public const string TwoLargePowerRune = "Buff_TwoLargePowerRune";
            public const string ThreeLargePowerRune = "Buff_ThreeLargePowerRune";
            public const string FourLargePowerRune = "Buff_FourLargePowerRune";
            public const string FiveLargePowerRune = "Buff_FiveLargePowerRune";
            public const string SixLargePowerRune = "Buff_SixLargePowerRune";
            public const string SevenLargePowerRune = "Buff_SevenLargePowerRune";
            public const string EightLargePowerRune = "Buff_EightLargePowerRune";
            public const string NineLargePowerRune = "Buff_NineLargePowerRune";
            public const string TenLargePowerRune = "Buff_TenLargePowerRune";
            public const string ResourceIsland = "Buff_ResourceIsland";
            public const string EngineerBuff = "Buff_EngineerBuff";
        }
    }
}

namespace Gameplay.Effects
{
    /// <summary>
    /// 用于传值的数值容器。
    /// </summary>
    public class BuffAttributes : IBuff
    {
        public BuffAttributes(float d, float s, float c, float r)
        {
            damage = d;
            shield = s;
            cooling = c;
            recover = r;
        }

        public float damage { get; }
        public float shield { get; }
        public float cooling { get; }
        public float recover { get; }
    }

    /// <summary>
    /// <c>BaseBuff</c> 是基地增益区的加成。
    /// <br/>50% 防御力加成。
    /// <br/>300% 冷却加成。
    /// </summary>
    public class BaseBuff : EffectBase, IBuff
    {
        /// <summary>
        /// 明确加成类型。
        /// </summary>
        public BaseBuff() : base(EffectID.Buffs.Base)
        {
        }

        public float damage => 1;
        public float shield => 0.5f;
        public float cooling => 3;
        public float recover => 0;
    }
    
    /// <summary>
    /// <c>BaseBuff</c> 无人机初始加成。
    /// <br/>无防御力加成。
    /// <br/>无冷却加成。
    /// </summary>
    public class DroneBaseBuff : EffectBase, IBuff
    {
        /// <summary>
        /// 明确加成类型。
        /// </summary>
        public DroneBaseBuff() : base(EffectID.Buffs.DroneBase,30)
        {
        }

        public float damage => 0;
        public float shield => 0f;
        public float cooling => 0;
        public float recover => 0;
    }

    /// <summary>
    /// <c>OutpostBuff</c> 是前哨站增益区的加成。
    /// <br/>500% 冷却加成。
    /// </summary>
    public class OutpostBuff : EffectBase, IBuff
    {
        /// <summary>
        /// 明确加成类型。
        /// </summary>
        public OutpostBuff() : base(EffectID.Buffs.Outpost)
        {
        }

        public float damage => 1;
        public float shield => 0;
        public float cooling => 5;
        public float recover => 0;
    }

    /// <summary>
    /// 梯形高地增益同样是 500% 冷却增益。
    /// </summary>
    public class HighlandCoolBuff : OutpostBuff
    {
        public HighlandCoolBuff()
        {
            type = EffectID.Buffs.HighlandCool;
        }
    }
    
    /// <summary>
    /// 英雄梯形高地吊射返金币
    /// </summary>
    public class HighlandCoinBuff : OutpostBuff
    {
        public HighlandCoinBuff()
        {
            type = EffectID.Buffs.HighlandCoin;
        }
    }

    /// <summary>
    /// 补血点增益。
    /// TODO: 改成 StatusEffect？
    /// </summary>
    public class ReviveBuff : EffectBase, IBuff
    {
        public ReviveBuff() : base(EffectID.Buffs.Revive)
        {
        }

        public float damage => 1;
        public float shield => 0;
        public float cooling => 1;
        public float recover => 0.05f;
    }

    /// <summary>
    /// 工程刷卡救援
    /// </summary>
    public class CardReviveBuff : EffectBase, IBuff
    {
        public CardReviveBuff() : base(EffectID.Buffs.CardRevive)
        {
        }

        public float damage => 1;
        public float shield => 0;
        public float cooling => 1;
        public float recover => 0.05f;
    }
    
    /// <summary>
    /// 狙击点增益。
    /// </summary>
    public class SnipeBuff : EffectBase, IBuff
    {
        public SnipeBuff() : base(EffectID.Buffs.Snipe)
        {
        }

        public float damage => 1;
        public float shield => 0;
        public float cooling => 1;
        public float recover => 0;
    }

    /// <summary>
    /// 飞坡增益。
    /// </summary>
    public class LaunchRampBuff : EffectBase, IBuff
    {
        public LaunchRampBuff() : base(EffectID.Buffs.LaunchRamp, 20)
        {
        }

        public float damage => 1;
        public float shield => 0.5f;
        public float cooling => 3;
        public float recover => 0;

        // TODO: 缓冲能量250J
    }

    /// <summary>
    /// 小能量机关增益。
    /// </summary>
    public class SmallPowerRuneBuff : EffectBase, IBuff
    {
        public SmallPowerRuneBuff() : base(EffectID.Buffs.SmallPowerRune, 45)
        {
        }

        public float damage => 1;
        public float shield => 0.25f;
        public float cooling => 1;
        public float recover => 0;
    }

    /// <summary>
    /// 大能量机关增益。
    /// </summary>
    public class LargePowerRuneBuff : EffectBase, IBuff
    {
        public LargePowerRuneBuff() : base(EffectID.Buffs.LargePowerRune, 45)
        {
        }
        
        public float damage => 1;
        public float shield => 0;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class OneLargePowerRuneBuff : EffectBase, IBuff
    {
        public OneLargePowerRuneBuff() : base(EffectID.Buffs.OneLargePowerRune, 45)
        {
        }
        
        public float damage => 1.5f;
        public float shield => 0.25f;
        public float cooling => 0;
        public float recover => 0;
    }

    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class TwoLargePowerRuneBuff : EffectBase, IBuff
    {
        public TwoLargePowerRuneBuff() : base(EffectID.Buffs.TwoLargePowerRune, 45)
        {
        }
        
        public float damage => 1.55f;
        public float shield => 0.25f;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class ThreeLargePowerRuneBuff : EffectBase, IBuff
    {
        public ThreeLargePowerRuneBuff() : base(EffectID.Buffs.ThreeLargePowerRune, 45)
        {
        }
        
        public float damage => 1.6f;
        public float shield => 0.25f;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class FourLargePowerRuneBuff : EffectBase, IBuff
    {
        public FourLargePowerRuneBuff() : base(EffectID.Buffs.FourLargePowerRune, 45)
        {
        }
        
        public float damage => 1.75f;
        public float shield => 0.25f;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class FiveLargePowerRuneBuff : EffectBase, IBuff
    {
        public FiveLargePowerRuneBuff() : base(EffectID.Buffs.FiveLargePowerRune, 45)
        {
        }
        
        public float damage => 2f;
        public float shield => 0.25f;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class SixLargePowerRuneBuff : EffectBase, IBuff
    {
        public SixLargePowerRuneBuff() : base(EffectID.Buffs.SixLargePowerRune, 45)
        {
        }
        
        public float damage => 2.2f;
        public float shield => 0.3f;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class SevenLargePowerRuneBuff : EffectBase, IBuff
    {
        public SevenLargePowerRuneBuff() : base(EffectID.Buffs.SevenLargePowerRune, 45)
        {
        }
        
        public float damage => 2.4f;
        public float shield => 0.35f;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class EightLargePowerRuneBuff : EffectBase, IBuff
    {
        public EightLargePowerRuneBuff() : base(EffectID.Buffs.EightLargePowerRune, 45)
        {
        }
        
        public float damage => 2.6f;
        public float shield => 0.4f;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class NineLargePowerRuneBuff : EffectBase, IBuff
    {
        public NineLargePowerRuneBuff() : base(EffectID.Buffs.NineLargePowerRune, 45)
        {
        }
        
        public float damage => 2.8f;
        public float shield => 0.45f;
        public float cooling => 0;
        public float recover => 0;
    }
    /// <summary>
    /// 部分大能量机关增益。
    /// </summary>
    public class TenLargePowerRuneBuff : EffectBase, IBuff
    {
        public TenLargePowerRuneBuff() : base(EffectID.Buffs.TenLargePowerRune, 45)
        {
        }
        
        public float damage => 3.0f;
        public float shield => 0.5f;
        public float cooling => 0;
        public float recover => 0;
    }
    
    /// <summary>
    /// 资源岛增益。
    /// </summary>
    public class ResourceIsland : EffectBase, IBuff
    {
        public ResourceIsland() : base(EffectID.Buffs.ResourceIsland, -1)
        {
        }

        public float damage => 1;
        public float shield => 0.75f;
        public float cooling => 1;
        public float recover => 0;
    }
    
    /// <summary>
    /// 工程开局增益。
    /// </summary>
    public class EngineerBuff : EffectBase, IBuff
    {
        public EngineerBuff() : base(EffectID.Buffs.EngineerBuff, -1)
        {
        }

        public float damage => 1;
        public float shield => 0.5f;
        public float cooling => 1;
        public float recover => 0;
    }
    
    /// <summary>
    /// 复活后无敌时间。
    /// </summary>
    public class RevivalBuff : EffectBase, IBuff
    {
        public RevivalBuff() : base(EffectID.Buffs.Revival, 10)
        {
        }

        public float damage => 1;
        public float shield => 1;
        public float cooling => 1;
        public float recover => 0;
    }

    /// <summary>
    /// RMUL 使用血包增益。
    /// </summary>
    public class MedicalBuff : EffectBase, IBuff
    {
        public MedicalBuff() : base(EffectID.Buffs.Medical, 5)
        {
        }

        public float damage => 1;
        public float shield => 0;
        public float cooling => 1;
        public float recover => 0.1f;
    }
}