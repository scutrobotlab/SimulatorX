using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Mirror;

namespace Gameplay
{
    /// <summary>
    /// 支持更多地图类型。
    /// </summary>
    public enum MapType
    {
        None,
        RMUC2022,
        RMUL2022,
        DeDust2
    }
    
    /// <summary>
    /// 声明一个类来匹配不同的载入场景，不同地图类型对应不同的roomScene与arenaScene场景
    /// </summary>
    [Serializable]
    public class MapInfo
    {
        public MapType type;
        [Scene]
        public string roomScene;
        [Scene]
        public string arenaScene;
    }

    /// <summary>
    /// 用于保存地图选择信息
    /// </summary>
    public class MapInfoManager : Singleton<MapInfoManager>
    {
        public List<MapInfo> maps = new List<MapInfo>();
        
        /// <summary>
        /// 获取指定游戏场景类型的MapInfo以便于更改roomScene与arenaScene场景
        /// </summary>
        /// <param name="type">指定的游戏场景的类型</param>
        /// <returns></returns>
        /// <exception cref="Exception">所选的场景不存在</exception>
        public MapInfo MapInfo(MapType type)
        {
            if (maps.Any(m => m.type == type))
            {
                return maps.First(m => m.type == type);
            }

            throw new Exception("Getting non-exist map.");
        }
    }
}