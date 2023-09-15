using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Events;
using Infrastructure;
using Mirror;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// 管理游戏中大部分音效、背景音乐等。
    /// </summary>
    public class AudioManager : StoreBase
    {
        public AudioClip countDown;
        public AudioClip inGame;
        public AudioClip gameOver;

        public AudioClip firstBlood;
        public AudioClip kill;
        public AudioClip aced;
        private bool _firstBlooded;
        
        public AudioSource gameStatusSource;
        public AudioSource killSource;
        public AudioSource killStatusSource;
        public AudioSource effectSource;
        
        [Header("音效包")]
        public AudioClip powerRuneActivated;
        public AudioClip launchStationOpen;

        /// <summary>
        /// 初始化组件。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            gameStatusSource.Stop();
            killSource.Stop();
            killStatusSource.Stop();
            
            UpdateVolume();
        }

        /// <summary>
        /// 更新音量
        /// 听说不区分bgm和音效啥的，就统一了。
        /// </summary>
        public void UpdateVolume() => UpdateVolume(PlayerPrefs.GetFloat("MasterVolumeSliderValue", 1.0f));
        
        public void UpdateVolume(float volume)
        { 
            gameStatusSource.volume = volume;
            killSource.volume = volume;
            killStatusSource.volume = volume;
            effectSource.volume = volume;
        }

        /// <summary>
        /// 声明输入事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.StartCountdown,
                ActionID.Stage.Kill,
                ActionID.Stage.Ejected,
                ActionID.Stage.GameOver,
                ActionID.Stage.PowerRuneActivated,
                ActionID.Stage.OpenLaunchStation
            }).ToList();
        }

        /// <summary>
        /// 处理输入事件。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Stage.StartCountdown:
                    StartRpc();
                    break;
                
                case ActionID.Stage.Kill:
                    var killAction = (Kill) action;
                    KillRpc(EntityManager.Instance().CampRef(killAction.victim.camp).All(s =>
                    {
                        if (!s.id.IsRobot()) return true;
                        var robotStore = (RobotStoreBase) s;
                        return robotStore.health == 0;
                    }));
                    break;
                
                case ActionID.Stage.Ejected:
                    var ejectedAction = (Ejected) action;
                    KillRpc(EntityManager.Instance().CampRef(ejectedAction.target.camp).All(s =>
                    {
                        if (!s.id.IsRobot()) return true;
                        var robotStore = (RobotStoreBase) s;
                        return robotStore.health == 0;
                    }));
                    break;
                
                case ActionID.Stage.GameOver:
                    GameOverRpc();
                    break;
                
                case ActionID.Stage.PowerRuneActivated:
                    var powerRuneActivatedAction = (PowerRuneActivated) action;
                    if (EntityManager.Instance().LocalRobot().camp == powerRuneActivatedAction.Camp)
                    {
                        PowerRuneActivatedRpc();
                    }
                    break;
                
                case ActionID.Stage.OpenLaunchStation:
                    OpenLaunchStationRpc();
                    break;
            }
        }

        /// <summary>
        /// 启动游戏时的音效。
        /// </summary>
        [ClientRpc]
        private void StartRpc()
        {
            StartCoroutine(GameBGM());
        }

        /// <summary>
        /// 启动游戏后，先播放倒计时音效，再播放比赛背景音乐。
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameBGM()
        {
            gameStatusSource.clip = countDown;
            gameStatusSource.loop = false;
            gameStatusSource.Play();
            yield return new WaitForSeconds(7.5f);
            gameStatusSource.clip = inGame;
            gameStatusSource.loop = true;
            gameStatusSource.Play();
        }

        /// <summary>
        /// 出现击杀时，播放音效。
        /// </summary>
        /// <param name="isAced"></param>
        [ClientRpc]
        private void KillRpc(bool isAced)
        {
            killStatusSource.clip = null;

            if (!_firstBlooded)
            {
                killStatusSource.clip = firstBlood;
                killStatusSource.loop = false;
                _firstBlooded = true;
            }

            if (isAced) killStatusSource.clip = aced;

            if (killStatusSource.clip != null)
            {
                killStatusSource.Play();
            }

            killSource.clip = kill;
            killSource.loop = false;
            killSource.Play();
        }

        /// <summary>
        /// 游戏结束时，播放背景音乐。
        /// </summary>
        [ClientRpc]
        private void GameOverRpc()
        {
            gameStatusSource.clip = gameOver;
            gameStatusSource.loop = false;
            gameStatusSource.Play();
        }
        
        [ClientRpc]
        private void PowerRuneActivatedRpc()
        {
            
            effectSource.clip = powerRuneActivated;
            effectSource.loop = false;
            effectSource.Play();
        }
        
        [ClientRpc]
        private void OpenLaunchStationRpc()
        {
            effectSource.clip = launchStationOpen;
            effectSource.loop = false;
            effectSource.Play();
        }
    }
}