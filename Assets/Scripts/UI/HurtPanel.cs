using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HurtPanel : MonoBehaviour
    {
        public Image top;
        public Image bottom;
        public Image left;
        public Image right;

        private void Start()
        {
            var transparent = new Color(1, 1, 1, 0);
            left.color = transparent;
            right.color = transparent;
            top.color = transparent;
            bottom.color = transparent;
        }

        public void StartSession(int armorIndex)
        {
            switch (armorIndex)
            {
                case 0:
                    StartCoroutine(Hurt(left));
                    break;
                case 1:
                    StartCoroutine(Hurt(right));
                    break;
                case 2:
                    StartCoroutine(Hurt(top));
                    break;
                case 3:
                    StartCoroutine(Hurt(bottom));
                    break;
            }
        }

        private static IEnumerator Hurt(Graphic indicator)
        {
            indicator.color = Color.white;
            yield return new WaitForSeconds(0.5f);
            indicator.color = new Color(1, 1, 1, 0);
        }
    }
}