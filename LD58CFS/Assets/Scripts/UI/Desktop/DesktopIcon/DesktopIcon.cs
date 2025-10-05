#region

//文件创建者：Egg
//创建时间：10-05 10:50

#endregion

using System;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI.Desktop
{
    public sealed class DesktopIcon : AbstractController
    {
        [SerializeField] private string _windowName;
        [SerializeField] private Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                this.SendEvent(new WindowOpenEvent
                {
                    WindowName = _windowName
                });
            });
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}