using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace  ArmBehavior
{
    public class RobotArmBehavior : MonoBehaviour
    {
        public ArmDig dig;                          //储存方向，现在角度，限制
        public Transform rotationRelatePos;        //围绕物体

        private Vector3 _dire;
        private float _differenceAngle;
        private float _originAngle;
        private float rotateVelocity = 200;
        
        private void Awake()
        {
            switch (dig.RotateAxis)
            {
                case ArmDig.Direction.X:
                    _originAngle = rotationRelatePos.localRotation.x;
                    break;
                case ArmDig.Direction.Y:
                    _originAngle = rotationRelatePos.localRotation.y;
                    break;
                case ArmDig.Direction.Z:
                    _originAngle = rotationRelatePos.localRotation.z;
                    break;
                case ArmDig.Direction.unsure:
                default:
                    Debug.LogWarning("no direction setted in robotarm");
                    break;
            }//初始化开始角度，方向

        }

        private void Update()
        {
            if(!dig.ArmOn) return;
            if(dig.ArmOn)
            {
                switch (dig.RotateAxis) //获取现在角度
                {
                    case ArmDig.Direction.X:
                        _dire = rotationRelatePos.transform.right;
                        dig.CurrenteAngle = transform.localRotation.x;
                        break;

                    case ArmDig.Direction.Y:
                        _dire = rotationRelatePos.transform.up;
                        dig.CurrenteAngle = transform.localRotation.y;
                        break;

                    case ArmDig.Direction.Z:
                        _dire = rotationRelatePos.transform.forward;
                        dig.CurrenteAngle = transform.localRotation.z;
                        break;

                    case ArmDig.Direction.unsure:
                    default:
                        Debug.LogWarning("no direction setted in robotarm");
                        break;
                } //获取现在角度

                //target = Math.Max(-0.9f + _originAngle, Math.Min(target, 0.9f + _originAngle));   //保持范围          用于调整constrain
                //if (Math.Abs(dig.ConstrainHigh - dig.ConstrainLow) > 0.001f)                      //防止constrain未初始化
                dig.target = Math.Max(dig.ConstrainLow, Math.Min(dig.ConstrainHigh, dig.target)); //保持范围

                RotateObj(-_originAngle + dig.target);
            }

            if (Mathf.Abs(dig.CurrenteAngle - dig.target) < 0.01f)
                dig.ArmOn = false;
        }

        public void RotateObj(float angle)
        {
            
            _differenceAngle = angle - dig.CurrenteAngle;
            if (Math.Abs(_differenceAngle) < 0.5f && Math.Abs(_differenceAngle) > 0.001f)
                _differenceAngle *= 5.0f;

            transform.RotateAround(rotationRelatePos.transform.position, 
                _dire,Time.deltaTime*rotateVelocity*_differenceAngle);//效果很像PID但不是PIDhhh
        }
    }

}
