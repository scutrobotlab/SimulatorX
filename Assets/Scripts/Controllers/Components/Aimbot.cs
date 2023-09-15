using System;
using System.Linq;
using Misc;
using UnityEngine;

namespace Controllers.Components
{
    /// <summary>
    /// <c>Aimbot</c> 实现自动瞄准功能。
    /// <br/>根据所给的瞄准目标、弹丸初速数据，计算击打目标需要的预判瞄准点。
    /// <br/>理想抛物线模型：
    /// <br/>https://robomaster-oss.github.io/rmoss_tutorials/#/rmoss_core/rmoss_projectile_motion/projectile_motion_iteration
    /// </summary>
    public class Aimbot
    {
        /// <summary>
        /// 弹道解算水平。
        /// </summary>
        public enum BallisticLevel
        {
            /// <summary>
            /// 按直线计算。
            /// </summary>
            None,

            /// <summary>
            /// 按抛物线计算。
            /// </summary>
            Parabola
        }

        /// <summary>
        /// 运动预测水平。
        /// </summary>
        public enum PredictionLevel
        {
            /// <summary>
            /// 不进行预测。
            /// </summary>
            None,

            /// <summary>
            /// 预测目标直线运动。
            /// </summary>
            Vector,

            /// <summary>
            /// 预测直线运动和神符旋转。
            /// </summary>
            VectorAndPowerRune
        }

        // 在特定情况下（锁定、失去锁定等）触发的回调
        public delegate void AimbotCallback();

        // 在识别阶段过滤目标的回调
        public delegate bool FilterTarget(object target);

        private readonly Camera _camera;
        private GameObject _target;
        private FilterTarget _targetFilter;
        private AimbotCallback _loseTargetCallback;
        private AimbotCallback _targetLockedCallback;

        private float _bulletVelocity;
        private readonly float _multiplier;

        // 辅助瞄准水平
        private BallisticLevel _ballisticLevel;
        private PredictionLevel _predictionLevel;

        // 是否正在追踪目标
        private bool _tracking;

        // 根据采样间隔动态采样
        private float _sampleInterval;
        private float _sampleDelta;
        private readonly RandomAccessQueue<Vector3> _samples = new RandomAccessQueue<Vector3>(32);

        /// <summary>
        /// 初始化辅助瞄准组件。  
        /// </summary>
        /// <param name="camera">辅助瞄准摄像机</param>
        /// <param name="bulletVelocity">子弹初速</param>
        /// <param name="multiplier">控制速度加成</param>
        public Aimbot(Camera camera, float bulletVelocity = 30, float multiplier = 1)
        {
            _camera = camera;
            _bulletVelocity = bulletVelocity;
            _multiplier = multiplier;
        }

        /// <summary>
        /// 更新参数。
        /// </summary>
        /// <param name="vel">枪口初速</param>
        public void UpdateAttribute(float vel)
        {
            _bulletVelocity = vel;
        }

        /// <summary>
        /// 是否正锁定目标。
        /// </summary>
        /// <returns>锁定状态</returns>
        public bool IsTracking() => _tracking;

        /// <summary>
        /// 尝试开始锁定目标。
        /// </summary>
        /// <param name="ballisticLevel">弹道解算水平</param>
        /// <param name="predictionLevel">运动预测水平</param>
        /// <param name="targetFilter">目标过滤函数</param>
        /// <param name="loseTargetCallback">失去目标回调</param>
        /// <param name="targetLockedCallback">锁定目标回调</param>
        /// <returns>锁定的目标</returns>
        /// TODO: 针对 Armor 优化
        public GameObject StartSession<T>(BallisticLevel ballisticLevel, PredictionLevel predictionLevel,
            FilterTarget targetFilter = null, AimbotCallback loseTargetCallback = null,
            AimbotCallback targetLockedCallback = null) where T : MonoBehaviour
        {
            _targetFilter = targetFilter;
            _ballisticLevel = ballisticLevel;
            _predictionLevel = predictionLevel;

            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var potentialTargets = GameObject.FindObjectsOfType<T>().Where(
                go => IfVisualOnTarget.HasVisualOnTarget(_camera, go.gameObject)).Where(
                go => targetFilter == null || targetFilter(go)).OrderBy(
                pt =>
                {
                    var position = _camera.WorldToViewportPoint(pt.transform.position) - Vector3.one * 0.5f;
                    return new Vector2(position.x, position.y).magnitude;
                }).ToList();
            if (potentialTargets.Count == 0) return null;
            _target = potentialTargets[0].gameObject;

            _loseTargetCallback = loseTargetCallback;
            _targetLockedCallback = targetLockedCallback;
            _tracking = true;
            return _target;
        }

        /// <summary>
        /// 主动停止锁定目标。
        /// </summary>
        public void EndSession()
        {
            _target = null;
            _tracking = false;
            _samples.Clear();
        }

        /// <summary>
        /// 更新辅助瞄准状态。
        /// </summary>
        /// <returns>瞄准控制量</returns>
        public Vector2 Update(float deltaTime, bool auto = false)
        {
            if (!_tracking)
            {
                _samples.Clear();
                return Vector2.zero;
            }

            _sampleDelta = deltaTime;
            _samples.Enqueue(_target.transform.position);
            if (_samples.count > 30) _samples.Dequeue();

            if (_target == null || !IfVisualOnTarget.HasVisualOnTarget(_camera, _target))
            {
                EndSession();
                _loseTargetCallback?.Invoke();
                return Vector2.zero;
            }

            if (_targetFilter != null && !_targetFilter(_target))
            {
                EndSession();
                _loseTargetCallback?.Invoke();
                return Vector2.zero;
            }

            var controlValue = Iterate();
            var threshold = _sampleInterval * -1 + 0.2;
            var speed = 6 + 20 * _sampleInterval;
            if (controlValue.magnitude < threshold) _targetLockedCallback?.Invoke();
            return controlValue * (_multiplier * (auto ? speed : 1));
        }

        /// <summary>
        /// 辅助瞄准算法。
        /// </summary>
        /// <returns>瞄准控制量</returns>
        private Vector2 Iterate()
        {
            // 位置
            var position = _camera.transform.position;
            var targetPosition = _target.transform.position;
            var secondPosition = targetPosition;
            var firstPosition = targetPosition;

            if (_samples.count > 0)
            {
                if (float.IsNaN(_sampleInterval))
                    _sampleInterval = (targetPosition - position).magnitude / _bulletVelocity;
                var desire = (int) (_sampleInterval / _sampleDelta) * 2 + 6; // 装甲板中心调整值
                if (desire > _samples.count) desire = _samples.count;
                var section = desire / 2;
                if (section != 0)
                {
                    secondPosition = _samples.Get(_samples.count - section);
                    firstPosition = _samples.Get(_samples.count - section * 2);
                }
            }

            var prediction = Vector3.zero;

            switch (_predictionLevel)
            {
                case PredictionLevel.None:
                    prediction = Vector3.zero;
                    break;
                case PredictionLevel.Vector:
                    prediction = targetPosition - secondPosition;
                    break;
                case PredictionLevel.VectorAndPowerRune:
                    var pedalPosition = Vector3.zero;
                    var vec0 = secondPosition - firstPosition;
                    var vec1 = targetPosition - secondPosition;
                    var forecastAngleCos = Vector3.Dot(vec0.normalized, vec1.normalized);
                    if (Mathf.Abs(forecastAngleCos) > 0.98 || firstPosition == targetPosition)
                    {
                        // fallback 到直线
                        prediction = targetPosition - secondPosition;
                    }
                    else
                    {
                        var circlePosition = Circle(firstPosition, secondPosition, targetPosition);
                        if (float.IsNaN(circlePosition.x) || float.IsNaN(circlePosition.y) ||
                            float.IsNaN(circlePosition.z))
                        {
                            // 求圆心失败
                            prediction = targetPosition - secondPosition;
                        }
                        else
                        {
                            var dx = circlePosition.x - targetPosition.x;
                            var dy = circlePosition.y - targetPosition.y;
                            var dz = circlePosition.z - targetPosition.z;

                            var u = (secondPosition.x - circlePosition.x) * (circlePosition.x - targetPosition.x) +
                                    (secondPosition.y - circlePosition.y) * (circlePosition.y - targetPosition.y) +
                                    (secondPosition.z - circlePosition.z) * (circlePosition.z - targetPosition.z);
                            u /= dx * dx + dy * dy + dz * dz;
                            pedalPosition.x = circlePosition.x + u * dx;
                            pedalPosition.y = circlePosition.y + u * dy;
                            pedalPosition.z = circlePosition.z + u * dz;
                            Debug.DrawLine(circlePosition, pedalPosition, Color.red);
                            Debug.DrawLine(secondPosition, secondPosition + (pedalPosition - secondPosition) * 2,
                                Color.red);
                            var forecastPosition = pedalPosition - secondPosition + pedalPosition;
                            prediction = forecastPosition - targetPosition;
                        }
                    }

                    break;
            }

            // 预测
            targetPosition += prediction;

            // 参数初始化
            var distance = (targetPosition - position).magnitude;
            var alpha = Mathf.Asin((targetPosition.y - position.y) / distance);
            var theta = alpha;
            const float g = -9.8f;

            // 迭代
            for (var i = 0; i < 100; i++)
            {
                var vX0 = Mathf.Cos(theta) * _bulletVelocity;
                var vY0 = Mathf.Sin(theta) * _bulletVelocity;
                var t = Mathf.Cos(alpha) * distance / vX0;
                _sampleInterval = t;
                var sY = vY0 * t + 0.5f * g * Mathf.Pow(t, 2);
                var err = Mathf.Sin(alpha) * distance - sY;
                if (err < 1e-3) break;
                var adjust = 1.0f / (1 + Mathf.Pow((float) Math.E, -err)) - 0.5f;
                theta += 0.025f * (float) Math.PI * adjust;
            }

            // 下坠补偿
            var tY = Mathf.Tan(theta) * Mathf.Cos(alpha) * distance + position.y;
            var vTargetPos = new Vector3(targetPosition.x, tY, targetPosition.z);

            if (_ballisticLevel == BallisticLevel.None)
            {
                vTargetPos = targetPosition;
            }

            // 转化为控制量
            var delta = _camera.WorldToScreenPoint(vTargetPos);
            // 虚拟目标点可能在屏幕之外，那么先向目标方向转动云台
            if (delta.magnitude == 0) delta = _camera.WorldToScreenPoint(targetPosition);
            delta.x /= Screen.width;
            delta.y /= Screen.height;
            delta -= Vector3.one * 0.5f;

            // 可视化
            Debug.DrawRay(position, targetPosition - position, Color.red);
            Debug.DrawRay(position, vTargetPos - position, Color.yellow);
            Debug.DrawRay(vTargetPos, targetPosition - vTargetPos, Color.yellow);
            Debug.DrawRay(position, targetPosition - position, Color.yellow);
            Debug.DrawRay(vTargetPos, prediction, Color.magenta);
            Debug.DrawRay(vTargetPos + prediction, targetPosition - vTargetPos - prediction,
                Color.magenta);
            Debug.DrawRay(position, vTargetPos + prediction - position, Color.magenta);

            return new Vector2(delta.x, delta.y);
        }

        /// <summary>
        /// 求圆心
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        private Vector3 Circle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            float x1 = point1.x, x2 = point2.x, x3 = point3.x;
            float y1 = point1.y, y2 = point2.y, y3 = point3.y;
            float z1 = point1.z, z2 = point2.z, z3 = point3.z;
            float a1 = (y1 * z2 - y2 * z1 - y1 * z3 + y3 * z1 + y2 * z3 - y3 * z2),
                b1 = -(x1 * z2 - x2 * z1 - x1 * z3 + x3 * z1 + x2 * z3 - x3 * z2),
                c1 = (x1 * y2 - x2 * y1 - x1 * y3 + x3 * y1 + x2 * y3 - x3 * y2),
                d1 = -(x1 * y2 * z3 - x1 * y3 * z2 - x2 * y1 * z3 + x2 * y3 * z1 + x3 * y1 * z2 -
                       x3 * y2 * z1),
                a2 = 2 * (x2 - x1),
                b2 = 2 * (y2 - y1),
                c2 = 2 * (z2 - z1),
                d2 = x1 * x1 + y1 * y1 + z1 * z1 - x2 * x2 - y2 * y2 - z2 * z2,
                a3 = 2 * (x3 - x1),
                b3 = 2 * (y3 - y1),
                c3 = 2 * (z3 - z1),
                d3 = x1 * x1 + y1 * y1 + z1 * z1 - x3 * x3 - y3 * y3 - z3 * z3;
            //求圆心
            Vector3 circleCenter;
            circleCenter.x =
                -(b1 * c2 * d3 - b1 * c3 * d2 - b2 * c1 * d3 + b2 * c3 * d1 + b3 * c1 * d2 - b3 * c2 * d1) /
                (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);
            circleCenter.y =
                (a1 * c2 * d3 - a1 * c3 * d2 - a2 * c1 * d3 + a2 * c3 * d1 + a3 * c1 * d2 - a3 * c2 * d1) /
                (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);
            circleCenter.z =
                -(a1 * b2 * d3 - a1 * b3 * d2 - a2 * b1 * d3 + a2 * b3 * d1 + a3 * b1 * d2 - a3 * b2 * d1) /
                (a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1);
            return circleCenter;
        }
    }
}