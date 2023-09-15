using System.Collections;
using Gameplay;
using Gameplay.Customize;
using UnityEngine;

namespace UI
{
    public class ReviveButton : MonoBehaviour
    {
        [SerializeField] private int countDown;
        [SerializeField] private int requiredMoney;
        [SerializeField] private PlayerStore player;
        [SerializeField] private RobotStoreBase data;
        [SerializeField] private bool _first = true;
        [SerializeField] private bool _second = true;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => EntityManager.Instance()?.LocalPlayer() != null);
            player = EntityManager.Instance().LocalPlayer();
            data = player.localRobot;

            Debug.Log("has player" + player.ToString());
            Debug.Log("has data" + data.ToString());
        }

        // Update is called once per frame
        private int CalculateRequiredMoney()
        {
            return (420 + 59 - countDown) / 60 * 100 + 50 * data.level; //向上取整
        }


        public void Select()
        {
            if (data == null || player == null)
                Start();

            countDown = ClockStore.Instance().countDown;
            requiredMoney = CalculateRequiredMoney();

            //两次机会，金币够，已死亡，非哨兵
            if ((_first || _second) && CoinStore.Instance().CoinLeft(data.id.camp) > requiredMoney &&
                data.health == 0 && data.id.serial != 4)
                // if (true) 
            {
                // Dispatcher.Instance().Send(new AskRewardCoin()
                // {
                //      Camp = data.id.camp,
                //      Coin = -50
                // });

                CustomizeManager.Instance().CmdSendExchange(data.id, requiredMoney);

                //立即复活标志位
                // data.revivalProcessRequired =  1 - data.requiredAdded;

                if (_first)
                    _first = false;
                else
                    _second = false;
            }
        }
    }
}