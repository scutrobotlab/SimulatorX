using Misc;

namespace Gameplay.Embedded
{
    /// <summary>
    /// 电控仿真单连接TCP服务器
    /// </summary>
    public class EmbeddedSingleCon : SingleConTCPServer
    {
        public EmbeddedSingleCon(int port) : base(port)
        {
        }

        public EmbeddedSingleCon(int port, SingleConStrategy strategy) : base(port, strategy)
        {
        }

        protected override void OnSocketForcedClosed()
        {
        }
    }
}