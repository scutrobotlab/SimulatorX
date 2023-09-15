using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Gameplay.Attribute;
using Gameplay.Events;
using Gameplay.Events.Child;
using Infrastructure;
using Infrastructure.Input;
using Mirror;
using Misc;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    /// <summary>
    /// <c>PlayerStore</c> 代表玩家。
    /// <br/>进行对应机器人的确认和后续输入的打包和转发。
    /// </summary>
    public class PlayerStore : StoreBase
    {
        [Header("Fake transmission delay")] public float fakeDelay = 0.05f;

        // 所控制机器人
        [SyncVar] private GameObject _controlledRobot;
        public RobotStoreBase localRobot;
        public RobotStoreBase originalLocalRobot;

        // 主摄像机初始化
        private bool _cameraInitialized = false;

        private bool _registeredToEntityManager;

        // 灵敏度
        public float sensitivity = 5;

        /// <summary>
        /// 确认身份。
        /// </summary>
        protected override void Identify()
        {
            id = new Identity(role: Identity.Roles.Player);
        }

        /// <summary>
        /// 确认本地机器人。
        /// </summary>
        /// <param name="robot">本地机器人</param>
        public void SetControlledRobot(GameObject robot)
        {
            var robotStoreBase = robot.GetComponent<RobotStoreBase>();
            if (!robotStoreBase)
            {
                throw new Exception("Fail to recognize local robot");
            }

            _controlledRobot = robot;
            // 服务端初始化
            localRobot = robotStoreBase;
            originalLocalRobot = localRobot;
        }

        [Client]
        public void ChangeLocalRobot(RobotStoreBase robot)
        {
            localRobot = robot;
            CmdChangeLocalRobot(robot.id);
        }
        
        [Command]
        private void CmdChangeLocalRobot(Identity robot)
        {
            localRobot = (RobotStoreBase) EntityManager.Instance().Ref(robot);
        }

        /// <summary>
        /// 仅打开所控制机器人摄像机
        /// </summary>
        private void ConfirmCamera()
        {
            foreach (var cameraComponent in FindObjectsOfType<Camera>())
            {
                if (cameraComponent.targetTexture != null) continue;

                var rootStore = cameraComponent.GetComponent<StoreBase>();
                var enable = false;
                if (rootStore == null)
                {
                    rootStore = cameraComponent.GetComponentInParent<StoreBase>();
                }

                if (rootStore != null && rootStore.id == localRobot.id)
                {
                    if (cameraComponent == localRobot.fpvCamera)
                    {
                        enable = true;
                    }
                }

                cameraComponent.enabled = enable;
                var audioListener = cameraComponent.GetComponent<AudioListener>();
                if (audioListener != null) audioListener.enabled = enable;
            }
        }

        private IEnumerator LateInitiateCamera(float time)
        {
            yield return new WaitForSeconds(time);
            _cameraInitialized = true;
            Debug.Log("camera initialize over");
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(LateInitiateCamera(10f));
        }

        /// <summary>
        /// 获取拥有的机器人并转发输入。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isServer)
            {
                if (!localRobot)
                {
                    localRobot = _controlledRobot.GetComponent<RobotStoreBase>();
                    originalLocalRobot = localRobot;
                }

                if (!isClient) return;
            }

            if (_controlledRobot == null) return;

            if (isLocalPlayer)
            {
                if (!localRobot)
                {
                    localRobot = _controlledRobot.GetComponent<RobotStoreBase>();
                    originalLocalRobot = localRobot;
                }

                if(!_cameraInitialized)
                {
                    ConfirmCamera();
                    // _cameraInitialized = true;
                }
                
                if (!_registeredToEntityManager)
                {
                    EntityManager.Instance().RegisterLocal(this, localRobot);
                    _registeredToEntityManager = true;
                }

                if (localRobot.id == originalLocalRobot.id && localRobot.id.serial != 4)
                {
                    // 在客户端打包输入数据，发送到服务端。
                    var primaryAxis = BitUtil.CompressAxisInput(InputManager.Instance().primaryAxis);
                    var secondaryAxis = BitUtil.CompressAxisInput(InputManager.Instance().secondaryAxis);
                    var input = new BitMap32();

                    input.SetByte(2, primaryAxis);
                    input.SetByte(3, secondaryAxis);
                    input.SetBit(2, InputManager.Instance().ButtonStatus[InputActionID.FunctionA]);
                    input.SetBit(3, InputManager.Instance().ButtonStatus[InputActionID.FunctionB]);
                    input.SetBit(4, InputManager.Instance().ButtonStatus[InputActionID.FunctionC]);
                    input.SetBit(5, InputManager.Instance().ButtonStatus[InputActionID.FunctionD]);
                    input.SetBit(6, InputManager.Instance().ButtonStatus[InputActionID.FunctionE]);
                    input.SetBit(7, InputManager.Instance().ButtonStatus[InputActionID.FunctionF]);
                    input.SetBit(8, InputManager.Instance().ButtonStatus[InputActionID.FunctionG]);
                    input.SetBit(9, InputManager.Instance().ButtonStatus[InputActionID.FunctionH]);
                    input.SetBit(10, InputManager.Instance().ButtonStatus[InputActionID.FunctionI]);
                    input.SetBit(11, InputManager.Instance().ButtonStatus[InputActionID.FunctionJ]);
                    input.SetBit(12, InputManager.Instance().ButtonStatus[InputActionID.FunctionK]);
                    input.SetBit(13, InputManager.Instance().ButtonStatus[InputActionID.FunctionL]);
                    input.SetBit(14, InputManager.Instance().ButtonStatus[InputActionID.FunctionM]);
                    input.SetBit(15, InputManager.Instance().ButtonStatus[InputActionID.FunctionN]);

                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        input.SetBit(0, InputManager.Instance().ButtonStatus[InputActionID.Fire]);
                        input.SetBit(1, InputManager.Instance().ButtonStatus[InputActionID.SecondaryFire]);
                        StartCoroutine(
                            SendDelayedViewInput(InputManager.Instance().RawView() * sensitivity, fakeDelay));
                    }

                    StartCoroutine(SendDelayedInput(input.GetData(), fakeDelay));
                }
            }
        }
        
        /// <summary>
        /// 在发送视角控制输入前模拟图传延迟。
        /// </summary>
        /// <param name="input">视角控制输入</param>
        /// <param name="delay">延迟时间</param>
        /// <returns></returns>
        private IEnumerator SendDelayedViewInput(Vector2 input, float delay)
        {
            if (localRobot != null && localRobot.id.IsRobot())
            {
                if (delay > (float) NetworkTime.rtt)
                {
                    yield return new WaitForSeconds(delay - (float) NetworkTime.rtt);
                }
            }

            // 防止 Delay 执行报错
            if (NetworkManager.singleton.isNetworkActive)
            {
                CmdSendViewInput(input);
            }
        }

        /// <summary>
        /// 在发送其他输入前模拟图传延迟。
        /// </summary>
        /// <param name="input">输入数据</param>
        /// <param name="delay">延迟时间</param>
        /// <returns></returns>
        private IEnumerator SendDelayedInput(uint input, float delay)
        {
            if (localRobot != null && localRobot.id.IsRobot())
            {
                if (delay > (float) NetworkTime.rtt)
                {
                    yield return new WaitForSeconds(delay - (float) NetworkTime.rtt);
                }
            }

            // 防止 Delay 执行报错
            if (NetworkManager.singleton.isNetworkActive)
            {
                CmdSendInput(input);
            }
        }

        /// <summary>
        /// 发送视角控制数据。
        /// </summary>
        /// <param name="data">输入数据</param>
        [Command(requiresAuthority = false)]
        private void CmdSendViewInput(Vector2 data)
        {
            if (!(localRobot.id.IsRobot()||localRobot.id.IsJudge()||localRobot.id.role == Identity.Roles.Spectator)) return;
            Dispatcher.Instance().Send(new ViewControl
            {
                Receiver = localRobot.id,
                X = data.x,
                Y = data.y
            });
        }

        /// <summary>
        /// 发送其他输入数据。
        /// </summary>
        /// <param name="data">输入数据</param>
        [Command(requiresAuthority = false)]
        private void CmdSendInput(uint data)
        {
            if (!(localRobot.id.IsRobot()||localRobot.id.IsJudge()||localRobot.id.role == Identity.Roles.Spectator)) return;
            var input = new BitMap32(data);
            var fire = input.GetBit(0);
            var secondaryFire = input.GetBit(1);
            var functionA = input.GetBit(2);
            var functionB = input.GetBit(3);
            var functionC = input.GetBit(4);
            var functionD = input.GetBit(5);
            var functionE = input.GetBit(6);
            var functionF = input.GetBit(7);
            var functionG = input.GetBit(8);
            var functionH = input.GetBit(9);
            var functionI = input.GetBit(10);
            var functionJ = input.GetBit(11);
            var functionK = input.GetBit(12);
            var functionL = input.GetBit(13);
            var functionM = input.GetBit(14);
            var functionN = input.GetBit(15);
            var primaryAxis = BitUtil.DecompressAxisInput(input.GetByte(2));
            var secondaryAxis = BitUtil.DecompressAxisInput(input.GetByte(3));
            Dispatcher.Instance().Send(new PrimaryAxis
            {
                Receiver = localRobot.id,
                X = primaryAxis.x,
                Y = primaryAxis.y
            });
            Dispatcher.Instance().Send(new SecondaryAxis
            {
                Receiver = localRobot.id,
                X = secondaryAxis.x,
                Y = secondaryAxis.y
            });
            Dispatcher.Instance().Send(new StateControl
            {
                Receiver = localRobot.id,
                Fire = fire,
                SecondaryFire = secondaryFire,
                FunctionA = functionA,
                FunctionB = functionB,
                FunctionC = functionC,
                FunctionD = functionD,
                FunctionE = functionE,
                FunctionF = functionF,
                FunctionG = functionG,
                FunctionH = functionH,
                FunctionI = functionI,
                FunctionJ = functionJ,
                FunctionK = functionK,
                FunctionL = functionL,
                FunctionM = functionM,
                FunctionN = functionN
            });
        }
    }
}