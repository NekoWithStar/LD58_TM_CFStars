#region

//文件创建者：Egg
//创建时间：10-05 01:13

#endregion

using System;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI.RecycleBin
{
    public sealed class DisplayImage : AbstractController
    {
        [SerializeField] private Button _button;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private string _param;

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                Selection.DisplayImage = _sprite;
                Selection.ImageParam   = _param;
                this.SendEvent(new WindowOperationEvent
                {
                    Operation  = EWindowOperation.Open,
                    WindowName = "ImageViewer"
                });
            });
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}