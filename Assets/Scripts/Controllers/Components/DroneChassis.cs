using System;
using UnityEngine;

namespace Controllers.Components
{
    /// <summary>
    /// 无人机底盘组件。
    /// </summary>
    public class DroneChassis
    {
        private readonly Rigidbody _rigidbody;
        private readonly Transform _transform;

        private float _upForce;
        private float _tiltAmountForward;
        private float _tiltVelocityForward;
        private float _tiltAmountSideways;
        private float _tiltAmountVelocity;
        private Vector3 _velocityToSmoothDampingToZero;
        private Quaternion _rotation;
        private float _wantedYRotation;
        private float _currentYRotation;
        private float _rotationYVelocity;
        private Quaternion _newRotation;

        private const float SideMovementAmount = 1000f;
        private const float RotateAmountByKeys = 2.5f;
        
        private readonly Transform _ptz;

        /// <summary>
        /// 初始化无人机底盘。
        /// </summary>
        /// <param name="drone">机身</param>
        /// <param name="ptz">云台</param>
        /// <exception cref="Exception"></exception>
        public DroneChassis(GameObject drone,Transform ptz =null)
        {
            _rigidbody = drone.GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                throw new Exception("Initiating chassis without a Rigidbody component.");
            }

            _transform = drone.GetComponent<Transform>();
            if (_transform == null)
            {
                throw new Exception("Initiating chassis without a Transform component.");
            }

            if (_transform.GetComponent<Collider>() == null)
            {
                throw new Exception("Initiating chassis without a Collider component.");
            }

            _ptz = ptz;
        }

        /// <summary>
        /// 给无人机添加浮力，注意WASD时，因为要有倾斜效果，所以会有向下的力，所以浮力要略大一些
        /// </summary>
        /// TODO:螺旋桨
        /// <param name="primaryAxis">主轴输入</param>
        /// <param name="secondaryAxis">副轴输入</param>
        public void Buoyancy(Vector2 primaryAxis, Vector2 secondaryAxis)
        {
            //旧
            /*
             if (primaryAxis.y > 0.02f || primaryAxis.x > 0.02f || primaryAxis.y < -0.02f || primaryAxis.x < -0.02f)
            {
                if (secondaryAxis.y != 0)
                {
                    _rigidbody.velocity = _rigidbody.velocity;
                }

                if (secondaryAxis.magnitude == 0)
                {
                    var velocity = _rigidbody.velocity;
                    _rigidbody.velocity = new Vector3(velocity.x,
                        Mathf.Lerp(velocity.y, 0, Time.fixedDeltaTime * 5), _rigidbody.velocity.z);
                }
            }
            if (secondaryAxis.y > 0.02f)
            {
                _upForce = 3250;
            }
            else if (secondaryAxis.y < -0.02)
            {
                _upForce = 2800;
            }
            else _upForce = 3050f;
            
            _rigidbody.AddRelativeForce(Vector3.up * (_upForce * 3.7f));
            */
            
            //新（暂时固定方案）:移动transfrom
        }

        /// <summary>
        /// 移动函数以及视觉上的无人机倾斜。
        /// </summary>
        /// <param name="primaryAxis">主轴输入</param>
        public void Move(Vector2 primaryAxis)
        {
            /*
            if (primaryAxis.x < -0.02 || primaryAxis.x > 0.02)
            {
                _rigidbody.AddRelativeForce(-_ptz.right * (primaryAxis.x * SideMovementAmount));
            }

            if (primaryAxis.y < -0.02 || primaryAxis.y > 0.02)
            {
                _rigidbody.AddRelativeForce(-_ptz.forward * (primaryAxis.y * SideMovementAmount));
            }
            

            var velocity = _rigidbody.velocity;
            float targetVelocity = 30f;
            if (primaryAxis.y > .2f && primaryAxis.x > .2f)
            {
                _rigidbody.velocity =
                    Vector3.ClampMagnitude(velocity, Mathf.Lerp(velocity.magnitude, targetVelocity, Time.deltaTime * 5f));
            }

            if (primaryAxis.y > .2f && primaryAxis.x < .2f)
            {
                _rigidbody.velocity =
                    Vector3.ClampMagnitude(velocity, Mathf.Lerp(velocity.magnitude, targetVelocity, Time.deltaTime * 5f));
            }

            if (primaryAxis.y < .2f && primaryAxis.x > .2f)
            {
                _rigidbody.velocity =
                    Vector3.ClampMagnitude(velocity, Mathf.Lerp(velocity.magnitude, targetVelocity, Time.deltaTime * 5f));
            }

            if (primaryAxis.y < .2f && primaryAxis.x < .2f)
            {
                _rigidbody.velocity =
                    Vector3.SmoothDamp(velocity, Vector3.zero, ref _velocityToSmoothDampingToZero, .95f);
            }
             
            /*if (primaryAxis.x < -0.02 || primaryAxis.x > 0.02)
            {
                _rigidbody.AddRelativeForce(_transform.forward * (primaryAxis.x * SideMovementAmount));
                _tiltAmountForward = Mathf.SmoothDamp(_tiltAmountForward, -10 * primaryAxis.x, ref _tiltVelocityForward,
                    0.1f);
            }

            if (primaryAxis.y < -0.02 || primaryAxis.y > 0.02)
            {
                _rigidbody.AddRelativeForce(_transform.right * (primaryAxis.y * SideMovementAmount));
                _tiltAmountSideways =
                    Mathf.SmoothDamp(_tiltAmountSideways, -10 * primaryAxis.y, ref _tiltAmountVelocity, .1f);
            }

            if (primaryAxis.magnitude < 0.02)
            {
                _tiltAmountSideways = Mathf.SmoothDamp(_tiltAmountSideways, 0, ref _tiltAmountVelocity, .1f);
            }

            _rotation = Quaternion.Euler(new Vector3(_tiltAmountForward, _currentYRotation, _tiltAmountSideways ));
            _transform.rotation = _rotation;


            var velocity = _rigidbody.velocity;
            if (primaryAxis.y > .2f && primaryAxis.x > .2f)
            {
                _rigidbody.velocity =
                    Vector3.ClampMagnitude(velocity, Mathf.Lerp(velocity.magnitude, 10.0f, Time.deltaTime * 5f));
            }

            if (primaryAxis.y > .2f && primaryAxis.x < .2f)
            {
                _rigidbody.velocity =
                    Vector3.ClampMagnitude(velocity, Mathf.Lerp(velocity.magnitude, 10.0f, Time.deltaTime * 5f));
            }

            if (primaryAxis.y < .2f && primaryAxis.x > .2f)
            {
                _rigidbody.velocity =
                    Vector3.ClampMagnitude(velocity, Mathf.Lerp(velocity.magnitude, 10.0f, Time.deltaTime * 5f));
            }

            if (primaryAxis.y < .2f && primaryAxis.x < .2f)
            {
                _rigidbody.velocity =
                    Vector3.SmoothDamp(velocity, Vector3.zero, ref _velocityToSmoothDampingToZero, .95f);
            }*/
            
        }

        /// <summary>
        /// 机身旋转
        /// </summary>
        /// <param name="secondaryAxis">副轴输入</param>
        public void Rotate(Vector2 secondaryAxis)
        {
            /*if (secondaryAxis.x < 0.0f)
            {
                _wantedYRotation -= RotateAmountByKeys;
            }

            if (secondaryAxis.x > 0.0f)
            {
                _wantedYRotation += RotateAmountByKeys;
            }*/

            _currentYRotation = Mathf.SmoothDamp(_currentYRotation, _wantedYRotation, ref _rotationYVelocity, 5f);
            // _rotation = Quaternion.Euler(new Vector3(_tiltAmountForward, _currentYRotation, _tiltAmountSideways));

            _transform.Rotate(new Vector3(0, _currentYRotation, 0));
        }
    }
}