namespace Controllers.RobotSensor
{
    /// <summary>
    /// <c>SensorConfig</c> 定义了传感器的配置选项。
    /// </summary>
    public static class SensorConfig
    {
        /// <summary>
        /// 哪一方机器人可以触发传感器。
        /// </summary>
        public enum Accessor
        {
            None,
            Red,
            Blue,
            Both
        }

        /// <summary>
        /// 多少机器人可以同时触发传感器。
        /// </summary>
        public enum Quantity
        {
            First,
            Camp,
            All
        }
    }
}