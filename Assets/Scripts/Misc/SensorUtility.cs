using System.Collections.Generic;
using System.Linq;
using Controllers.RobotSensor;
using Doozy.Runtime.Common.Extensions;
using Infrastructure;

namespace Misc
{
    public class SensorUtility
    {
        public delegate void EnterExitCallback(StoreBase store);

        private readonly EnterExitCallback _enterCallback;
        private readonly EnterExitCallback _exitCallback;
        
        private HashSet<StoreBase> _lastTargets = new HashSet<StoreBase>();

        public SensorUtility(EnterExitCallback enter = null, EnterExitCallback exit = null)
        {
            _enterCallback = enter;
            _exitCallback = exit;
        }

        public void Update(ISensor sensor)
        {
            var targetsSet = sensor.targets.ToHashSet();
            if (_enterCallback != null)
            {
                foreach (var enter in targetsSet.Except(_lastTargets))
                {
                    _enterCallback(enter);
                }
            }

            if (_exitCallback != null)
            {
                foreach (var exit in _lastTargets.Except(targetsSet))
                {
                    _exitCallback(exit);
                }
            }

            _lastTargets = targetsSet;
        }
    }
}