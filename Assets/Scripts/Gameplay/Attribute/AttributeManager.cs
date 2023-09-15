using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Attribute.DataObjects;
using Infrastructure;
using UnityEngine;

namespace Gameplay.Attribute
{
    /// <summary>
    /// 由等级定义的属性。
    /// </summary>
    [Serializable]
    public class LevelDefine
    {
        public int level;
        public CaliberDefine caliber42;
        public CaliberDefine caliber17;
        public RoleDefine hero;
        public RoleDefine infantry;
        public RoleDefine balance;
        public RoleDefine autoInfantry;
    }

    /// <summary>
    /// 由等级、发射机构口径定义的属性。
    /// </summary>
    [Serializable]
    public class CaliberDefine
    {
        public GunInfo burst;
        public GunInfo coolDown;
        public GunInfo velocity;
        public GunInfo auto;
    }

    /// <summary>
    /// 由等级、角色定义的属性。
    /// </summary>
    [Serializable]
    public class RoleDefine
    {
        public ExperienceInfo experience;
        public ChassisInfo armor;
        public ChassisInfo power;
        public ChassisInfo balance;
    }
   
    /// <summary>
    /// 用于返回的，由等级和角色定义的属性。
    /// </summary>
    public class LevelRoleAttributes
    {
        public readonly float MaxHealth;
        public readonly float MaxChassisPower;
        public readonly float ExperienceValue;
        public readonly float ExperienceToUpgrade;

        public LevelRoleAttributes(RobotFullAttribute template)
        {
            MaxHealth = template.maxHealth;
            MaxChassisPower = template.maxChassisPower;
            ExperienceValue = template.experienceValue;
            ExperienceToUpgrade = template.experienceToUpgrade;
        }

        public LevelRoleAttributes(ExperienceInfo experienceInfo, ChassisInfo chassisInfo)
        {
            MaxHealth = chassisInfo.maxHealth;
            MaxChassisPower = chassisInfo.maxChassisPower;
            ExperienceValue = experienceInfo.experienceValue;
            ExperienceToUpgrade = experienceInfo.experienceToUpgrade;
        }
    }

    /// <summary>
    /// 用于返回的，由等级和发射机构口径定义的属性。
    /// </summary>
    public class LevelCaliberAttributes
    {
        public readonly float MaxHeat;
        public readonly float MaxMuzzleVelocity;
        public readonly float MuzzleCoolDownRate;

        public LevelCaliberAttributes(GunInfo gunInfo)
        {
            MaxHeat = gunInfo.maxHeat;
            MaxMuzzleVelocity = gunInfo.maxMuzzleVelocity;
            MuzzleCoolDownRate = gunInfo.muzzleCoolDownRate;
        }
    }

    /// <summary>
    /// 用于管理机器人等级、机构类型及其相关属性。
    /// </summary>
    public class AttributeManager : Singleton<AttributeManager>
    {
        [Header("工程属性")] public RobotFullAttribute engineer;
        //[Header("哨兵属性")] public RobotFullAttribute sentinel;
        [Header("无人机属性")] public RobotFullAttribute drone;
        [Header("步兵与英雄属性")] public GunInfo smallCaliberDefault;
        public GunInfo largeCaliberDefault;
        public ChassisInfo infantry;
        public ChassisInfo autoInfantry;
        public ChassisInfo balance;
        public ChassisInfo hero;
        public List<LevelDefine> attributes = new List<LevelDefine>();

        /// <summary>
        /// 获得机器人属性。
        /// </summary>
        /// <param name="robot"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public LevelRoleAttributes RobotAttributes(RobotStoreBase robot)
        {
            var identity = robot.id;
            var level = robot.level;
            var isAuto = robot.id.role == Identity.Roles.AutoSentinel;

            // 处理英雄和步兵和哨兵情况
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (identity.role)
            {
                case Identity.Roles.Hero:
                {
                    foreach (var levelAttribute in attributes)
                    {
                        if (levelAttribute.level != level) continue;
                        var chassis = robot.chassisType switch
                        {
                            MechanicType.Chassis.Armor => levelAttribute.hero.armor,
                            MechanicType.Chassis.Power => levelAttribute.hero.power,
                            _ => null
                        };

                        if (chassis == null)
                        {
                            chassis = hero;
                        }

                        return new LevelRoleAttributes(levelAttribute.hero.experience, chassis);
                    }
                }
                    break;


                case Identity.Roles.BalanceInfantry:
                {
                    foreach (var levelAttribute in attributes)
                    {
                        if (levelAttribute.level != level) continue;
                        var chassis = robot.chassisType switch
                        {
                            MechanicType.Chassis.Armor => levelAttribute.balance.armor,
                            MechanicType.Chassis.Power => levelAttribute.balance.power,
                            MechanicType.Chassis.Balance => levelAttribute.balance.balance,
                            _ => null
                        };

                        if (chassis == null)
                        {
                            chassis = balance;
                        }

                        return new LevelRoleAttributes(levelAttribute.balance.experience, chassis);
                    }
                }
                    break;

                case Identity.Roles.Infantry:
                {
                    foreach (var levelAttribute in attributes)
                    {
                        if (levelAttribute.level != level) continue;
                        /*var chassis = robot.chassisType switch
                        {
                            MechanicType.Chassis.Armor => levelAttribute.infantry.armor,
                            MechanicType.Chassis.Power => levelAttribute.infantry.power,
                            _ => null
                        };*/

                        //临时版本更改
                        if (identity.serial == 1)
                        {
                            var chassis = robot.chassisType switch
                            {
                                MechanicType.Chassis.Armor => levelAttribute.balance.armor,
                                MechanicType.Chassis.Power => levelAttribute.balance.power,
                                MechanicType.Chassis.Balance => levelAttribute.balance.balance,
                                _ => null
                            };
                            
                            if (chassis == null)
                            {
                                chassis = balance;
                            }
                            return new LevelRoleAttributes(levelAttribute.infantry.experience, chassis);
                        }
                        else
                        {
                            var chassis = robot.chassisType switch
                            {
                                MechanicType.Chassis.Armor => levelAttribute.infantry.armor,
                                MechanicType.Chassis.Power => levelAttribute.infantry.power,
                                _ => null
                            };
                            
                            if (chassis == null)
                            {
                                chassis = infantry;
                            }
                            return new LevelRoleAttributes(levelAttribute.infantry.experience, chassis);
                        }
                        
                        /*if (chassis == null)
                        {
                            chassis = infantry;
                        }
                        return new LevelRoleAttributes(levelAttribute.infantry.experience, chassis);*/
                    }
                } break;

                case Identity.Roles.AutoSentinel:
                {
                    foreach (var levelAttribute in attributes)
                    {
                        if (levelAttribute.level != level) continue;
                        var chassis = robot.chassisType switch
                        {
                            MechanicType.Chassis.Armor => levelAttribute.autoInfantry.armor,
                            MechanicType.Chassis.Power => levelAttribute.autoInfantry.power,
                            _ => null
                        };

                        if (chassis == null)
                        {
                            chassis = autoInfantry;
                        }

                        return new LevelRoleAttributes(levelAttribute.autoInfantry.experience, chassis);
                    }
                }
                    break;
            }
            

            // 处理其余情况
            var robotAttributes = identity.role switch
            {
                Identity.Roles.Engineer => new LevelRoleAttributes(engineer),
               // Identity.Roles.Sentinel => new LevelRoleAttributes(sentinel),
                Identity.Roles.Drone => new LevelRoleAttributes(drone),
                Identity.Roles.DroneManipulator => new LevelRoleAttributes(drone),
                _ => null
            };
            
            //  裁判与OB无所谓
            if (identity.role == Identity.Roles.Judge || identity.role == Identity.Roles.Spectator)
            {
                return new LevelRoleAttributes(drone);
            }

            if (attributes == null)
            {
                throw new Exception("Getting attributes with unknown robot type.");
            }

            return robotAttributes;
        }

        /// <summary>
        /// 获得发射机构影响的属性。
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="gunIndex"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public LevelCaliberAttributes GunAttributes(RobotStoreBase robot, int gunIndex = 0)
        {
            if (gunIndex >= robot.Guns.Count)
            {
                throw new Exception("Getting gun attributes with invalid gun index");
            }

            var level = robot.level;
            var caliber = robot.Guns[gunIndex].caliber;
            var type = robot.Guns[gunIndex].type;
            foreach (var levelAttribute in attributes.Where(
                         levelAttribute => levelAttribute.level == level))
            {
                LevelCaliberAttributes caliberAttributes = null;

                // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                switch (caliber)
                {
                    case MechanicType.CaliberType.Large:
                        if (type == MechanicType.GunType.CoolDown)
                        {
                            throw new Exception("Getting gun attribute of large caliber with coolDown.");
                        }

                        caliberAttributes = type switch
                        {
                            MechanicType.GunType.Burst => new LevelCaliberAttributes(levelAttribute.caliber42.burst),
                            MechanicType.GunType.Velocity => new LevelCaliberAttributes(levelAttribute.caliber42
                                .velocity),
                            _ => new LevelCaliberAttributes(largeCaliberDefault)
                        };
                        break;
                    case MechanicType.CaliberType.Small:
                        if (robot.id.role==Identity.Roles.AutoSentinel)
                        {
                            caliberAttributes = type switch
                            {
                                MechanicType.GunType.Burst => new LevelCaliberAttributes(levelAttribute.caliber17.auto),
                                MechanicType.GunType.Velocity => new LevelCaliberAttributes(levelAttribute.caliber17
                                    .auto),
                                MechanicType.GunType.CoolDown => new LevelCaliberAttributes(levelAttribute.caliber17
                                    .auto),
                                _ => new LevelCaliberAttributes(smallCaliberDefault)
                            };
                        }
                        else
                        {
                            caliberAttributes = type switch
                            {
                                MechanicType.GunType.Burst =>
                                    new LevelCaliberAttributes(levelAttribute.caliber17.burst),
                                MechanicType.GunType.Velocity => new LevelCaliberAttributes(levelAttribute.caliber17
                                    .velocity),
                                MechanicType.GunType.CoolDown => new LevelCaliberAttributes(levelAttribute.caliber17
                                    .coolDown),
                                _ => new LevelCaliberAttributes(smallCaliberDefault)
                            };
                        }

                        break;
                }

                return caliberAttributes;
            }

            throw new Exception("Getting gun attributes without valid preset.");
        }
    }
}