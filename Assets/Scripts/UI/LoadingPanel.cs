using Gameplay;
using Mirror;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 载入中页面。
    /// </summary>
    public class LoadingPanel : NetworkBehaviour
    {
        public Image panel;
        public TMP_Text ready;
        public TMP_Text notReady;

        private void Start()
        {
            ready.text = "";
            notReady.text = "";
        }

        protected void FixedUpdate()
        {
            var entityManager = EntityManager.Instance();
            if (entityManager != null)
            {
                panel.gameObject.SetActive(!entityManager.initialized);
            }
        }

        [ClientRpc]
        public void UpdateListRpc(string readyText, string notReadyText)
        {
            ready.text = readyText;
            notReady.text = notReadyText;
        }
    }
}