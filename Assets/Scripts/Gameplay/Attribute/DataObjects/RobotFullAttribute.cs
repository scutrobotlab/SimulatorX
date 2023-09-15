using UnityEngine;

namespace Gameplay.Attribute.DataObjects
{
    /// <summary>
    /// 全量机器人属性。
    /// </summary>
    [CreateAssetMenu(fileName = "机器人-完整属性", menuName = "等级系统/完整属性", order = 1)]
    public class RobotFullAttribute : ScriptableObject
    {
        [Header("热量上限")] public float maxHeat;
        [Header("枪口初速上限")] public float maxMuzzleVelocity;
        [Header("每秒热量冷却")] public float muzzleCoolDownRate;

        [Header("血量上限")] public float maxHealth;
        [Header("底盘功率上限")] public float maxChassisPower;

        [Header("经验价值")] public float experienceValue;
        [Header("升级所需经验")] public float experienceToUpgrade;
    }
}