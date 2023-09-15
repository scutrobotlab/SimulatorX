using UnityEngine;

namespace Gameplay.Attribute.DataObjects
{
    /// <summary>
    /// 经验信息。
    /// </summary>
    [CreateAssetMenu(fileName = "机器人-经验", menuName = "等级系统/经验信息", order = 2)]
    public class ExperienceInfo : ScriptableObject
    {
        [Header("经验价值")] public float experienceValue;
        [Header("升级所需经验")] public float experienceToUpgrade;
    }
}