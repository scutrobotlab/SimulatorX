using Gameplay.Attribute;
using UnityEngine;

namespace Controllers.Components
{
    /// <summary>
    /// <c>Gun</c> 实现了发射机构组件。
    /// <br/>其功能是按照初速度、发射间隔、发射者、热量限制等进行发射。
    /// </summary>
    public class Gun
    {
        public delegate bool SyncLaunch(Vector3 velocity);

        private readonly SyncLaunch _launchCallback;
        private readonly Transform _muzzle;
        private readonly MechanicType.CaliberType _caliber;
        private bool _triggered;

        public float Velocity;
        public float HeatLimit;
        public float CoolDown;
        public bool FullAuto;
        public float Heat;
        public bool IsDrone = false;
        
        private float _interval;

        /// <summary>
        /// 初始化发射机构
        /// </summary>
        /// <param name="caliber">弹丸类型</param>
        /// <param name="muzzle">枪口位置</param>
        /// <param name="launchCallback">开火回调</param>
        /// <param name="velocity">理想初速</param>
        /// <param name="heatLimit">热量上限</param>
        /// <param name="coolDown">每秒冷却值</param>
        /// <param name="fullAuto">小弹丸是否全自动</param>
        public Gun(
            MechanicType.CaliberType caliber,
            Transform muzzle,
            SyncLaunch launchCallback,
            float velocity = 30.0f,
            float heatLimit = 100,
            float coolDown = 10,
            bool fullAuto = false
        )
        {
            _caliber = caliber;
            _muzzle = muzzle;
            _launchCallback = launchCallback;
            Velocity = velocity;
            HeatLimit = heatLimit;
            CoolDown = coolDown;
            FullAuto = fullAuto;
        }

        /// <summary>
        /// 更新发射机构参数。
        /// </summary>
        /// <param name="vel">枪口初速</param>
        /// <param name="cd">每秒冷却值</param>
        /// <param name="hl">热量上限</param>
        public void UpdateAttributes(float vel, float cd, float hl)
        {
            Velocity = vel;
            CoolDown = cd;
            HeatLimit = hl;
        }

        /// <summary>
        /// 切换全自动、半自动开火。
        /// </summary>
        /// <param name="fullAuto">是否为全自动</param>
        public void ToggleFullAuto(bool fullAuto)
        {
            FullAuto = fullAuto;
        }

        /// <summary>
        /// 扣下扳机（不一定能够发射）
        /// </summary>
        public void Trigger()
        {
            if (!FullAuto)
            {
                if (_triggered) return;
                _triggered = true;
            }

            if (_caliber == MechanicType.CaliberType.Large)
            {
                if (_interval == 0)
                {
                    if (_launchCallback(_muzzle.forward * Velocity))
                    {
                        Heat += 100;
                    }

                    _interval += 0.25f;
                }
            }
            else
            {
                if (Heat + 10 <= HeatLimit && _interval == 0)
                {
                    if (_launchCallback(_muzzle.forward * Velocity))
                    {
                        Heat += 10;
                    }

                    if (!IsDrone)
                        _interval += 0.1f;
                    else
                        _interval += 0.04f;

                }
            }
        }

        /// <summary>
        /// 释放扳机（用于半自动）
        /// </summary>
        public void Release()
        {
            _triggered = false;
        }

        /// <summary>
        /// 更新热量
        /// </summary>
        /// <param name="delta">帧时间</param>
        /// <returns>因过热扣血占生命上限比例</returns>
        public float Update(float delta)
        {
            float hurt = 0;
            _interval -= delta;
            if (_interval < 0) _interval = 0;
            if (Heat > HeatLimit)
            {
                if (Heat <= HeatLimit * 2)
                {
                    hurt = (Heat - HeatLimit) / 250 * delta;
                }
                else
                {
                    hurt = (Heat - HeatLimit * 2) / 250;
                    Heat = HeatLimit * 2;
                    return hurt;
                }
            }

            Heat -= CoolDown * delta;
            if (Heat < 0) Heat = 0;
            return hurt;
        }
    }
}