using System.Collections.Generic;
using System.Linq;
using Gameplay.Events;
using Honeti;
using Infrastructure;
using Mirror;
using UI;

namespace Gameplay
{
    /// <summary>
    /// 管理滚动文字提示。
    /// </summary>
    public class InfoManager : StoreBase
    {
        // 无敌状态
        private readonly Dictionary<Identity.Camps, bool> _firstBlooded =
            new Dictionary<Identity.Camps, bool>();

        private AnnouncementPanel _announcementPanel;

        /// <summary>
        /// 记录一血（基地无敌解除）状态（RMUL）。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _firstBlooded[Identity.Camps.Blue] = false;
            _firstBlooded[Identity.Camps.Red] = false;
        }

        /// <summary>
        /// 加载面板。
        /// </summary>
        protected override void LoadUI()
        {
            base.LoadUI();
            _announcementPanel = UIManager.Instance().UI<AnnouncementPanel>();
        }

        /// <summary>
        /// 声明输入事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.Kill,
                ActionID.Stage.Penalty,
                ActionID.Stage.Ejected,
                ActionID.Clock.CoinNaturalGrowth
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
                case ActionID.Stage.Kill:
                    var killAction = (Kill)action;
                    if (!_firstBlooded[killAction.victim.camp])
                    {
                        _firstBlooded[killAction.victim.camp] = true;
                        AddRpc(killAction.victim.camp.Translate() + I18N.instance.getValue("^base_can_be_attacked"),
                            NoticeIcon.Tag.Base);
                    }

                    break;

                case ActionID.Stage.Penalty:
                    var penaltyAction = (Penalty)action;
                    AddRpc(penaltyAction.target.Describe() + penaltyAction.Description +
                           I18N.instance.getValue("^penalty"), NoticeIcon.Tag.Penalty);
                    break;

                case ActionID.Stage.Ejected:
                    var ejectedAction = (Ejected)action;
                    AddRpc(ejectedAction.target.Describe() + ejectedAction.Description +
                           I18N.instance.getValue("^send_off"), NoticeIcon.Tag.SendOff);
                    if (!_firstBlooded[ejectedAction.target.camp])
                    {
                        _firstBlooded[ejectedAction.target.camp] = true;
                        AddRpc(ejectedAction.target.camp.Translate() + I18N.instance.getValue("^base_can_be_attacked"),
                            NoticeIcon.Tag.Base);
                    }

                    break;

                case ActionID.Clock.CoinNaturalGrowth:
                    var coinGrowAction = (CoinNaturalGrowth)action;
                    AddRpc($"{I18N.instance.getValue("^natural_economic_growth")} +" + coinGrowAction.MinuteCoin,
                        NoticeIcon.Tag.Coin);
                    break;
            }
        }

        /// <summary>
        /// 添加滚动信息。
        /// </summary>
        /// <param name="info"></param>
        [ClientRpc]
        private void AddRpc(string info, NoticeIcon.Tag tag)
        {
            //_infosPanel.AddInfo(info);
            _announcementPanel.AddRollingNotice(info, tag);
        }
    }
}