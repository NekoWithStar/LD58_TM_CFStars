#region

//文件创建者：Egg
//创建时间：10-04 06:41

#endregion

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class ConsoleItem : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private GameObject _cursor;
        [SerializeField] private Vector2 _leftOffset;
        [SerializeField] private Vector2 _centerOffset;
        public string Text => _text.text;
        [ShowInInspector]
        public  bool DisplayCursor { get; set; } = false;

        public bool DisplayLeft { get; set; } = false;
        
        private void Awake()
        {
            _text ??= GetComponentInChildren<Text>();
        }
        
        public void SetText(string text)
        {
            _text.text = text;
        }

        private void Update()
        {
            _cursor.GetComponent<RectTransform>().anchoredPosition = DisplayLeft ? _leftOffset : _centerOffset;
            _cursor.SetActive(DisplayCursor && (int)(Time.time * 2) % 2 == 0);
        }
    }
}