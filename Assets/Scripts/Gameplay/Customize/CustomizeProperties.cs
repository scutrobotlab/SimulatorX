namespace Gameplay.Customize
{
    /// <summary>
    /// 自定义信息名称。
    /// </summary>
    public static class CustomizeProperties
    {
        // 弹舱
        public static class Magazine
        {
            // 弹舱容量
            public const string Capacity = "Magazine.Capacity";
        }

        // 底盘
        public static class Chassis
        {
            // TODO：功率稳定性
            // 移动速度
            public const string Velocity = "Chassis.Velocity";

            // 自旋速度
            public const string Spinning = "Chassis.Spinning";

            // 传感器温漂
            public const string SensorDrift = "Chassis.SensorDrift";

            //超级电容
            public const string SuperBattery = "Chassis.SuperBattery";
            // TODO: 刹车
            // TODO: 重心
        }

        // 云台
        public static class Ptz
        {
            // 姿态稳定性
            public const string Stability = "Ptz.Stability";

            // 超调程度
            public const string Overshoot = "Ptz.Overshoot";
        }

        // 辅助瞄准
        public static class Aimbot
        {
            // 识别灵敏度
            public const string Stability = "Aimbot.Stability";

            // 弹道解算水平
            public const string Ballistic = "Aimbot.Ballistic";

            // 运动预测水平
            public const string Prediction = "Aimbot.Prediction";
        }

        // 发射机构
        public static class Gun
        {
            // 初速稳定性
            public const string MuzzleVelocity = "Gun.MuzzleVelocity";

            // 横向弹道抖动
            public const string HorizontalBallisticJitter = "Gun.HBJ";

            // 纵向弹道抖动
            public const string VerticalBallisticJitter = "Gun.VBJ";
            // TODO: 最高射频
            // TODO: 卡弹几率
            // TODO: 热量控制水平
        }
        
        public static class EngnieerSet
        {
            // 取矿模式
            public const string Ore = "EngnieerSet.Ore";
            //兑换矿石模式
            public const string Exchanging = "EngnieerSet.Exchanging";

        }
        
        public static class DartError
        {
            // 飞镖误差
            public const string DartErr = "Dart.Error";

        }

        // TODO：其他
    }
}