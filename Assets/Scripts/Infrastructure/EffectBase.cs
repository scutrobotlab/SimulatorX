using System;
using UnityEngine;

namespace Infrastructure
{
    /// <summary>
    /// <c>EffectBase</c> 用于标识持有者的某种状态。
    /// <br/>典型用例是 Buff。
    /// </summary>
    [Serializable]
    public class EffectBase
    {
        public string type;
        public float timeout;
        public float createTime;
        //标记可以修改状态(目前赛规暂不需要)
       // public bool isDown;

        /// <summary>
        /// 默认初始化函数。
        /// </summary>
        public EffectBase()
        {
            type = EffectID.NoEffect;
            timeout = -1;
            createTime = 0;
           // isDown = false;
        }

        /// <summary>
        /// 初始化效果。
        /// 将 <c>Timeout</c> 设为正数表示该效果定时消失。
        /// </summary>
        /// <param name="type">效果类型</param>
        /// <param name="timeout">效果过期时间</param>
        protected EffectBase(string type, int timeout = -1)
        {
            this.type = type;
            createTime = Time.time;
            this.timeout = timeout;
            //isDown = false;
        }
    }
}