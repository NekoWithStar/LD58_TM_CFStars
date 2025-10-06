#region

//文件创建者：Egg
//创建时间：10-05 11:05

#endregion

using System;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class BrowserHistoryItem : MonoBehaviour
    {
        [SerializeField] private Text   _text;
        [SerializeField] private Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(_text.text))
                    FindFirstObjectByType<BlackBoardController>().SetItem(_text.text,_text.text);
            });
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}