using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Infrastructure;
using Infrastructure.Child;
using Gameplay.Events.Child;
using UnityEngine;

namespace Controllers.Child
{
    public class Rectifier : StoreChildBase
    {
        public bool rotationOnly;
        private float _timer;

        protected override void Identify()
        {
        }

        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.Recorder.Rectify
            }).ToList();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            _timer += Time.fixedDeltaTime;
            if (_timer > 0.2f)
            {
                DispatcherSend(new Rectify
                {
                    Receiver = id,
                    Position = transform.position,
                    // ReSharper disable once Unity.InefficientPropertyAccess
                    Rotation = transform.rotation
                });
                _timer = 0;
            }
        }

        public override void Receive(IChildAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                case ChildActionID.Recorder.Rectify:
                    if (!Dispatcher.Instance().replay) break;
                    var rectifyAction = (Rectify) action;
                    if (rectifyAction.Receiver != id) break;
                    if (!rotationOnly)
                    {
                        transform.position = rectifyAction.Position;
                        // ReSharper disable once Unity.InefficientPropertyAccess
                        transform.rotation = rectifyAction.Rotation;
                    }
                    else
                    {
                        if (!ClockStore.Instance().playing) break;
                        var rotateAngles = transform.rotation.eulerAngles - rectifyAction.Rotation.eulerAngles;
                        rotateAngles.x = ClampAngle(rotateAngles.x);
                        rotateAngles.y = ClampAngle(rotateAngles.y);
                        rotateAngles.z = ClampAngle(rotateAngles.z);
                        if (rotateAngles.magnitude > 10)
                            transform.rotation = rectifyAction.Rotation;
                    }

                    break;
            }
        }

        private static float ClampAngle(float angle)
        {
            var result = angle;
            result %= 360;
            if (Mathf.Abs(result) > 180) result += result > 0 ? -360 : 360;
            return result;
        }
    }
}