#region

//文件创建者：Egg
//创建时间：10-05 11:40

#endregion

using System;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LD58.UI
{
    public abstract class AbstractWindowController : AbstractController, IDragHandler, IPointerDownHandler
    {
        [SerializeField] protected string WindowName;
        [SerializeField] private   int    WindowNameLocalizeId;
        [SerializeField] private   Button CloseButton;
        [SerializeField] private   Button MinimizeButton;
        [SerializeField] private   Text   TitleText;
        [SerializeField] private   Image  TitleBar;
        [SerializeField] private   Image  Mask;
        
        public Vector2 CachedPosition { get; set; }
        public bool Focus { get; set; }

        protected virtual void Awake()
        {
            MinimizeButton.onClick.AddListener(() =>
            {
                this.SendEvent(new WindowOperationEvent
                {
                    WindowName = WindowName,
                    Operation  = EWindowOperation.Minimize
                });
            });
            CloseButton.onClick.AddListener(() =>
            {
                this.SendEvent(new WindowOperationEvent
                {
                    WindowName = WindowName,
                    Operation  = EWindowOperation.Close
                });
            });
            TitleText.text = FindFirstObjectByType<LocalizationHandle>().GetLocalizedStringById(WindowNameLocalizeId);
        }
        
        public virtual void OnFocus()
        {
            
        }

        private void Update()
        {
            TitleBar.color  = Focus ? new Color(11 / 255f, 13 / 255f, 219 / 255f) : Color.white;
            TitleText.color = Focus ? Color.white : Color.black;
            Mask.enabled   = !Focus;
        }

        protected virtual void OnDestroy()
        {
            MinimizeButton.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            var rect = GetComponent<RectTransform>();
            rect.anchoredPosition += eventData.delta;
            rect.anchoredPosition = new Vector2(
                Mathf.Clamp(rect.anchoredPosition.x, -425, 425),
                Mathf.Clamp(rect.anchoredPosition.y, -220, 220)
            );
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            this.SendEvent(new WindowOperationEvent
            {
                Operation  = EWindowOperation.Focus,
                WindowName = WindowName
            });
        }
    }
}