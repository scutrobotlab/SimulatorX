using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    /// <summary>
    /// 记录特定角色出生点。
    /// </summary>
    [Serializable]
    public class RobotSpawn
    {
        public Identity.Camps camp;

        [FormerlySerializedAs("serial")] public int order;

        public Transform location;
    }

    //通过使用Singleton<>及配合GetInstance()来获取GameObject的位置。
    public class SpawnManager : Singleton<SpawnManager>
    {
        public List<RobotSpawn> robotSpawns = new List<RobotSpawn>();
        public RobotSpawn judgeSpawn;

        /// <summary>
        /// 通过传入的机器人类别，返回相应的生成位置。
        /// </summary>
        /// <param name="robot">查询的机器人</param>
        /// <returns>返回机器人相应的位置。</returns>
        public Transform LoadLocation(Identity robot)
        {
            if (robot.role == Identity.Roles.Judge || robot.role == Identity.Roles.Spectator)
            {
                return judgeSpawn.location;
            }
            foreach (var spawn in robotSpawns.Where(
                         spawn => robot.camp == spawn.camp && robot.order == spawn.order))
            {
                return spawn.location;
            }

            throw new Exception("Getting non-exist spawn spot.");
        }
    }
}