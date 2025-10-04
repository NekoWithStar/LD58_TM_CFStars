#region

//文件创建者：Egg
//创建时间：10-04 06:41

#endregion

using System;
using System.Collections.Generic;
using EggFramework.Util;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class ConsoleItem : AbstractController, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Text       _text;
        [SerializeField] private GameObject _cursor;
        [SerializeField] private Vector2    _leftOffset;
        [SerializeField] private Vector2    _centerOffset;
        public                   string     Text          => _text.text;
        [ShowInInspector] public bool       DisplayCursor { get; set; } = false;

        public bool DisplayLeft { get; set; } = false;

        public bool PendingInput { get; set; } = false;

        public string Key   { get; set; }
        public string Value { get; set; }

        public List<string> AcceptedBlackBoardKey { get; set; }

        private void Awake()
        {
            _text ??= GetComponentInChildren<Text>();
        }

        public void SetText(string text)
        {
            _text.text = text;
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,
                ((int)(text.Length / 46) + 1) * 30);
        }

        public void SetColor(Color color)
        {
            _text.color = color;
        }

        private void Update()
        {
            _cursor.GetComponent<RectTransform>().anchoredPosition = DisplayLeft ? _leftOffset : _centerOffset;
            _cursor.SetActive(DisplayCursor && (int)(Time.time * 2) % 2 == 0);

            if (PendingInput)
            {
                _text.SetAlpha(Time.time % 1);
            }
        }

        private IUnRegister _unRegister;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (PendingInput && Selection.SelectedBlackBoardItem)
            {
                _text.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, _text.color.a);
                _unRegister = this.RegisterEvent<ReleaseBlackBoardItemEvent>(e =>
                {
                    if (PendingInput && Selection.SelectedBlackBoardItem)
                    {
                        if (AcceptedBlackBoardKey.Count == 0 ||
                            AcceptedBlackBoardKey.Contains(Selection.SelectedBlackBoardItem.Key))
                        {
                            PendingInput = false;
                            _text.color  = Color.white;
                            _text.text = _text.text.Replace("_____",
                                $"BlackBoard.GetValue(\"{Selection.SelectedBlackBoardItem.Key}\") `");
                            Value = Selection.SelectedBlackBoardItem.Value;
                            Key   = Selection.SelectedBlackBoardItem.Key;
                        }
                    }
                });
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (PendingInput)
                _text.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, _text.color.a);
            _unRegister?.UnRegister();
        }
    }
}