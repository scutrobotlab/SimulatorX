using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArmBehavior;
using Mirror;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AdditionalAssets.RobotArm.Scripts
{
    public enum Grade
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }

    public class ArmController : NetworkBehaviour
    {
        public List<RobotArmBehavior> concludedDig;
    
        [Header("element4的armtrue设flase让兑换槽对准正前方")]
        public List<ArmDig> controlValue;
    
        public Grade originState = Grade.Zero;
    
        [SyncVar(hook = nameof(CmdChangeGrade))]
        public Grade currentState = Grade.Zero;
    
        public List<ArmDig> zeroState;
        public List<ArmDig> oneState;
        public List<ArmDig> twoState;
        public List<ArmDig> threeState;
        public List<ArmDig> fourState;

        //TODO：指示灯

        private void Start()
        {
            foreach (var VARIABLE in concludedDig)
            {
                controlValue.Add(VARIABLE.dig);
            }
        }

        private void Update()
        {
            if(originState == currentState)
                return;
            switch (currentState)
            {
                case Grade.Zero:
                    foreach (var dig in concludedDig)
                    {
                        dig.dig.ArmOn = true;
                        dig.dig.target = 0.0f;
                    }
                    break;
                case Grade.One:
                    foreach (var dig in concludedDig)
                    {
                        dig.dig.ArmOn = true;
                        dig.dig.target = oneState[concludedDig.IndexOf(dig)].target;
                    }
                    break;
                case Grade.Two:
                    foreach (var dig in concludedDig)
                    {
                        dig.dig.ArmOn = true;
                        dig.dig.target = oneState[concludedDig.IndexOf(dig)].target;
                    }
                    break;
                case Grade.Three:
                    foreach (var dig in concludedDig)
                    {
                        dig.dig.ArmOn = true;
                        dig.dig.target = oneState[concludedDig.IndexOf(dig)].target;
                    }          
                    break;
                case Grade.Four:
                    foreach (var dig in concludedDig)
                    {
                        dig.dig.ArmOn = true;
                        dig.dig.target = oneState[concludedDig.IndexOf(dig)].target;
                    }
                    break;
                default:
                    break;
            }

            originState = currentState;
        }

        [Command]
        public void CmdChangeGrade(Grade oldValue, Grade newValue)
        {
            currentState = newValue;
        }
    }
}