#region

//文件创建者：Egg
//创建时间：10-05 10:40

#endregion

using System;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class TaskItem : AbstractController
    {
        [SerializeField] private Image  _icon;
        [SerializeField] private Text   _text;
        [SerializeField] private Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                this.SendEvent(new WindowOperationEvent
                {
                    WindowName = _text.text,
                    Operation = EWindowOperation.ToggleActive
                });
            });
        }

        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
        
        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}