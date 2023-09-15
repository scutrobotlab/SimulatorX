using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.Items;
using Gameplay.Events;
using Gameplay.Events.Child;
using Gameplay.Attribute;
using Infrastructure;
using Infrastructure.Child;
using Misc;
using TMPro;
using UnityEngine;

namespace Controllers.Child
{
    /// <summary>
    /// 装甲板控制器。
    /// </summary>
    public class Armor : StoreChildBase
    {
        public Identity.Camps camp;
        public bool defaultOn;
        public char text;
        [HideInInspector] public bool lightOn;
        private readonly ToggleHelper _lightHelper = new ToggleHelper();
        public GameObject Center = null;//中心点

        /// <summary>
        /// 装甲板被击检测并发送被击事件。
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            CollisionEnter(collision);
        }

        /// <summary>
        /// 代理碰撞输入。
        /// 当用在有刚体的物体上时，需要由根物体代理碰撞输入。
        /// </summary>
        /// <param name="collision"></param>
        public void DelegateCollisionEnter(Collision collision)
        {
            CollisionEnter(collision);
        }

        /// <summary>
        /// 碰撞处理逻辑。
        /// 检测是否是弹丸（TODO：撞死）
        /// 并检测法向速度是否达到检测阈值。
        /// </summary>
        /// <param name="collision"></param>
        private void CollisionEnter(Collision collision)
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet == null) return;
            var hitPosition = bullet.transform.position;
            var isLarge = bullet.caliber == MechanicType.CaliberType.Large;
            var normalVelocity = Vector3.Dot(bullet.lastVelocity, transform.up * -1);
            // 降低法向速度要求以符合真实情况
            // if (!(normalVelocity > (isLarge ? 8 : 12))) return;
            if (normalVelocity > (isLarge ? 5 : 10)||bullet.caliber == MechanicType.CaliberType.Dart)
            {
                // Debug.Log(transform.name);
                
                if(Center!=null)
                    DispatcherSend(new ArmorHit
                    {
                        Receiver = Root.id,
                        Hitter = bullet.owner,
                        Caliber = bullet.caliber,
                        Armor = id,
                        Position = hitPosition,
                        CenterPos = Center.transform.position
                    });
                else 
                    DispatcherSend(new ArmorHit
                    {
                        Receiver = Root.id,
                        Hitter = bullet.owner,
                        Caliber = bullet.caliber,
                        Armor = id,
                        Position = hitPosition
                    });
                // Debug.Log(Root.id.role.ToString());
                // 被击打时播放音效。
                var source = GetComponent<AudioSource>();
                if (source != null)
                {
                    source.loop = false;
                    source.pitch = Random.Range(1.0f, 2.0f);
                    source.Play();
                }
                if(Root.id.role==Identity.Roles.Energy)
                    Destroy(bullet);
            }
            
        }

        /// <summary>
        /// 在编辑器中设置。
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 初始化灯光状态。
        /// </summary>
        protected override void StartWithRoot()
        {
            base.StartWithRoot();
            if (!Root.id.IsGroundRobot())
            {
                RegisterChild(transform);
            }
            lightOn = defaultOn;
            if (Root.id.role != Identity.Roles.Energy)
            {
                DispatcherSend(new TurnLight
                {
                    Receiver = id,
                    IsOn = defaultOn
                });
            }
        }

        /// <summary>
        /// 标识关注事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.Armor.SyncArmor,
                ChildActionID.Armor.TurnArmor
            }).ToList();
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IChildAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                case ChildActionID.Armor.SyncArmor:
                    var syncAction = (SyncArmor) action;
                    camp = syncAction.Camp;
                    text = syncAction.Text;
                    RegisterChild(transform);
                    break;
                case ChildActionID.Armor.TurnArmor:
                    var turnAction = (TurnArmor) action;
                    lightOn = turnAction.IsOn;
                    break;
            }
        }

        /// <summary>
        /// 递归控制灯光和文字组件。
        /// </summary>
        /// <param name="origin"></param>
        private void RegisterChild(Transform origin)
        {
            for (var i = 0; i < origin.childCount; i++)
            {
                RegisterChild(origin.GetChild(i));
            }

            var lightController = origin.GetComponent<LightController>();
            if (lightController != null)
            {
                lightController.id = id;
                lightController.camp = camp;
            }

            var textMeshPro = origin.GetComponent<TMP_Text>();
            if (textMeshPro != null)
            {
                textMeshPro.text = text.ToString();
            }
        }

        /// <summary>
        /// 控制灯光。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            switch (_lightHelper.Toggle(lightOn))
            {
                case ToggleHelper.State.Re:
                    if (Root.id.role != Identity.Roles.Energy)
                    {
                        DispatcherSend(new TurnLight
                        {
                            Receiver = id,
                            IsOn = true
                        });
                    }
                    break;
                case ToggleHelper.State.De:
                    if (Root.id.role != Identity.Roles.Energy)
                    {
                        DispatcherSend(new TurnLight
                        {
                            Receiver = id,
                            IsOn = false
                        });
                    }
                    break;
            }
        }

        /// <summary>
        /// 闪烁但不影响检测。
        /// </summary>
        /// <returns></returns>
        private IEnumerator Blink()
        {
            if (Root.id.role != Identity.Roles.Energy)
            {
                DispatcherSend(new TurnLight
                {
                    Receiver = id,
                    IsOn = false
                });
                yield return new WaitForSeconds(0.1f);
                DispatcherSend(new TurnLight
                {
                    Receiver = id,
                    IsOn = lightOn
                });
            }
        }
    }
}