using UnityEngine;

namespace Gameplay.Attribute.DataObjects
{
    /// <summary>
    /// 底盘类型决定的属性。
    /// </summary>
    [CreateAssetMenu(fileName = "机器人-底盘", menuName = "等级系统/底盘信息", order = 3)]
    public class ChassisInfo : ScriptableObject
    {
        [Header("血量上限")] public float maxHealth;
        [Header("底盘功率上限")] public float maxChassisPower;
    }
}