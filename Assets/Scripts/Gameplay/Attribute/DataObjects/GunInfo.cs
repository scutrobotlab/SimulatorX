using UnityEngine;

namespace Gameplay.Attribute.DataObjects
{
    /// <summary>
    /// 发射机构类型影响的属性。
    /// </summary>
    [CreateAssetMenu(fileName = "机器人-发射机构", menuName = "等级系统/发射机构信息", order = 4)]
    public class GunInfo : ScriptableObject
    {
        [Header("热量上限")] public float maxHeat;
        [Header("枪口初速上限")] public float maxMuzzleVelocity;
        [Header("每秒热量冷却")] public float muzzleCoolDownRate;
    }
}