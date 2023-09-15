using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class FavoriteButton : MonoBehaviour
    {
        [Header("Settings")]
        public FavoriteItem isFavorite;

        Image iconObj;
        Image iconFilledObj;
        Button mainButton;

        public enum FavoriteItem
        {
            FALSE,
            TRUE
        }

        void Start()
        {
            iconObj = gameObject.transform.Find("Icon").GetComponent<Image>();
            iconFilledObj = gameObject.transform.Find("Icon Filled").GetComponent<Image>();
            mainButton = gameObject.GetComponent<Button>();
            UpdateUI();
            mainButton.onClick.AddListener(ClickEvents);
            mainButton.onClick.AddListener(UpdateUI);
        }

        public void ClickEvents()
        {
            if (isFavorite == FavoriteItem.FALSE)
                isFavorite = FavoriteItem.TRUE;

            else
                isFavorite = FavoriteItem.FALSE;
        }

        public void UpdateUI()
        {
            if (isFavorite == FavoriteItem.FALSE)
            {
                iconObj.gameObject.SetActive(true);
                iconFilledObj.gameObject.SetActive(false);
            }

            else
            {
                iconObj.gameObject.SetActive(false);
                iconFilledObj.gameObject.SetActive(true);
            }
        }
    }
}