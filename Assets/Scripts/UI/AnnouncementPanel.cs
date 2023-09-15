using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AnnouncementPanel : MonoBehaviour
    {
        private const uint MaxRollingNoticeCount = 4;

        [Header("Rolling Notice")] public GameObject rollingNoticeLayer;

        public GameObject rollingNoticePrefab;

        [Header("Banner Notice")] public Image bannerNoticeBackground;

        public Animator bannerNoticeAnimator;
        public TMP_Text bannerNoticeText;
        public Image bannerIcon;

        [Header("Data")] public NoticeIcon[] noticeIcons;

        private readonly Dictionary<NoticeIcon.Tag, Sprite> _noticeIcons = new Dictionary<NoticeIcon.Tag, Sprite>();

        private void Start()
        {
            foreach (var icon in noticeIcons)
            {
                _noticeIcons.Add(icon.tag, icon.icon);
            }
        }

        private readonly Queue<GameObject> _rollingNotices = new Queue<GameObject>();
        public void AddRollingNotice(string content, NoticeIcon.Tag iconTag = NoticeIcon.Tag.None)
        {
            // Sync
            while (_rollingNotices.Count > MaxRollingNoticeCount)
            {
                Destroy(_rollingNotices.Dequeue());
            }
            StartCoroutine(AddRollingNoticeCoroutine(content, iconTag));
        }

        
        private IEnumerator AddRollingNoticeCoroutine(string content, NoticeIcon.Tag iconTag = NoticeIcon.Tag.None)
        {
            var notice = Instantiate(rollingNoticePrefab, rollingNoticeLayer.transform);
            notice.GetComponentInChildren<TMP_Text>().text = content;
            notice.GetComponentsInChildren<Image>().Last().sprite = _noticeIcons[iconTag];
            _rollingNotices.Enqueue(notice);

            yield return new WaitForSeconds(25.0f);
            if (notice != null)
            {
                notice.SetActive(false);
            }
        }

        public void AddBannerNotice(
            string content, Color backgroundColor, NoticeIcon.Tag iconTag = NoticeIcon.Tag.None, Color iconColor = default,
            float duration = 2.5f)
        {
            StartCoroutine(AddBannerNoticeCoroutine(content, backgroundColor, iconTag, iconColor, duration));
        }

        private IEnumerator AddBannerNoticeCoroutine(string content, Color backgroundColor,
            NoticeIcon.Tag iconTag = NoticeIcon.Tag.None, Color iconColor = default, float duration = 2.5f)
        {
            
            bannerNoticeText.text = content;
            bannerIcon.sprite = _noticeIcons[iconTag];
            bannerNoticeBackground.color = backgroundColor;
            if (iconTag == NoticeIcon.Tag.None)
            {
                bannerIcon.color = new Color(0f, 0f, 0f, 0f);
            } else if (iconColor != default)
            {
                bannerIcon.color = iconColor;
            }

            bannerNoticeAnimator.Play("Panel In");
            yield return new WaitForSeconds(duration);
            bannerNoticeAnimator.Play("Panel Out");
        }
    }

    [Serializable]
    public struct NoticeIcon
    {
        public enum Tag
        {
            Coin,
            Base,
            PowerRune,
            Penalty,
            SendOff,
            None
        }

        public Tag tag;
        public Sprite icon;
    }
}