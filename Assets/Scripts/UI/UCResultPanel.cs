using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Controllers;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Networking;
using Infrastructure;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 结算面板。
    /// </summary>
    public class UCResultPanel : MonoBehaviour
    {
        public Image panel;
        public TMP_Text winOrLose;
        public TMP_Text description;
        private bool _resultShown;

        public Image redWin;
        public Image blueWin;
        public TMP_Text redTeam;
        public TMP_Text blueTeam;

        public TMP_Text redBase;
        public Image redBaseBar;
        public TMP_Text blueBase;
        public Image blueBaseBar;

        public TMP_Text redSentinel;
        public Image redSentinelBar;
        public TMP_Text blueSentinel;
        public Image blueSentinelBar;
        
        public TMP_Text redOutpost;
        public Image redOutpostBar;
        public TMP_Text blueOutpost;
        public Image blueOutpostBar;

        public TMP_Text redDamage;
        public TMP_Text redKills;
        public TMP_Text blueDamage;
        public TMP_Text blueKills;

        public void Start()
        {
            EndSession();
            _resultShown = false;
        }

        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public void StartSession(string result, string desc, Identity.Camps winner)
        {
            if (_resultShown) return;
            panel.gameObject.SetActive(true);
            winOrLose.text = result;
            description.text = desc;

            redWin.enabled = winner == Identity.Camps.Red;
            blueWin.enabled = winner == Identity.Camps.Blue;
            var roomManager = (NetworkRoomManagerExt) NetworkManager.singleton;
            redTeam.text = roomManager.redTeam;
            blueTeam.text = roomManager.blueTeam;

            if (EntityManager.Instance() != null)
            {
                // 基地信息
                var redBaseStore = (BaseStore) EntityManager.Instance()
                    .Ref(new Identity(Identity.Camps.Red, Identity.Roles.Base));
                var blueBaseStore = (BaseStore) EntityManager.Instance()
                    .Ref(new Identity(Identity.Camps.Blue, Identity.Roles.Base));
                redBase.text = redBaseStore.health.ToString();
                blueBase.text = blueBaseStore.health.ToString();
                switch (EntityManager.Instance().CurrentMap())
                {
                    case MapType.RMUL2022:
                        redBaseBar.fillAmount = redBaseStore.health / 2000;
                        blueBaseBar.fillAmount = blueBaseStore.health / 2000;
                        break;
                    case MapType.RMUC2022:
                        redBaseBar.fillAmount = redBaseStore.health / 5000;
                        blueBaseBar.fillAmount = blueBaseStore.health / 5000;
                        break;
                }

                // 哨兵信息
                foreach (var robot in EntityManager.Instance().RobotRef().
                             Where(robot => robot.id.role == Identity.Roles.AutoSentinel))
                {
                    if (robot.id.camp == Identity.Camps.Red)
                    {
                        var robotStore = (RobotStoreBase)robot;
                        redSentinel.text = robotStore.health.ToString();
                        redSentinelBar.fillAmount = robotStore.health / AttributeManager.Instance().RobotAttributes(robotStore).MaxHealth;
                    }
                    else
                    {
                        var robotStore = (RobotStoreBase)robot;
                        blueSentinel.text = robotStore.health.ToString();
                        blueSentinelBar.fillAmount = robotStore.health / AttributeManager.Instance().RobotAttributes(robotStore).MaxHealth;
                    }
                }
                
                // 前哨站信息
                var redOutpostStore = (OutpostStore) EntityManager.Instance()
                    .Ref(new Identity(Identity.Camps.Red, Identity.Roles.Outpost));
                var blueOutpostStore = (OutpostStore) EntityManager.Instance()
                    .Ref(new Identity(Identity.Camps.Blue, Identity.Roles.Outpost));
                redOutpost.text = redOutpostStore.health.ToString();
                blueOutpost.text = blueOutpostStore.health.ToString();
                var outpostMaxHealth = 1500f;
                redOutpostBar.fillAmount = redOutpostStore.health / outpostMaxHealth;
                blueOutpostBar.fillAmount = blueOutpostStore.health / outpostMaxHealth;
                
                // 伤害、击杀信息
                redDamage.text = ((int)EntityManager.Instance().DamageRecords[Identity.Camps.Red]).ToString();
                blueDamage.text = ((int)EntityManager.Instance().DamageRecords[Identity.Camps.Blue]).ToString();
                redKills.text = EntityManager.Instance().KillRecords
                    .Count(kr => kr.killer.camp == Identity.Camps.Red && kr.victim.IsRobot()).ToString();
                blueKills.text = EntityManager.Instance().KillRecords
                    .Count(kr => kr.killer.camp == Identity.Camps.Blue && kr.victim.IsRobot()).ToString();
            }

            _resultShown = true;
        }

        private void EndSession()
        {
            panel.gameObject.SetActive(false);
        }
    }
}