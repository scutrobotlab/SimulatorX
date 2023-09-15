using System.Collections.Generic;
using Infrastructure;

namespace Controllers.RobotSensor
{
    /// <summary>
    /// <c>ISensor</c> 是所有类型传感器的共同接口。
    /// <br/>传感器必须能够返回被感知的目标列表。
    /// </summary>
    public interface ISensor
    {
        public List<StoreBase> targets { get; }
    }
}