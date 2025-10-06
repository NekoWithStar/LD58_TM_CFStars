#region

//文件创建者：Egg
//创建时间：10-04 06:41

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using EggFramework.Util;
using LD58.UI.CommandBoard;
using LD58.Util;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class ConsoleItem : AbstractController, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler
    {
        [SerializeField] private Text       _text;
        [SerializeField] private GameObject _cursor;
        [SerializeField] private Vector2    _leftOffset;
        [SerializeField] private Vector2    _centerOffset;
        public                   string     Text          => _text.text;
        [ShowInInspector] public bool       DisplayCursor { get; set; } = false;

        public bool      DisplayLeft  { get; set; } = false;
        public Parameter Parameter    { get; set; }
        public bool      PendingInput { get; set; } = false;

        public string Key   => Parameter.Name;
        public string Value => Parameter.GetValue();

        public readonly Dictionary<string, string> BlackBoardItems = new();

        public List<string> AcceptedBlackBoardKey => Parameter.Accept;

        private bool _showReject;

        private void Awake()
        {
            _text ??= GetComponentInChildren<Text>();
        }

        public void SetText(string text)
        {
            _text.text = text;
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,
                ((int)(StripRichTextTags(text).Length / 46) + 1) * 30);
        }

        public static string StripRichTextTags(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // 正则表达式模式，用于匹配常见的富文本标签
            string pattern = @"<[^>]+>|&\w+;";

            // 移除所有匹配到的标签
            string result = Regex.Replace(input, pattern, string.Empty);

            return result;
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
                if (!_showReject)
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
                            AcceptedBlackBoardKey.Contains(Selection.SelectedBlackBoardItem.Value))
                        {
                            PendingInput    = false;
                            Parameter.Value = Selection.SelectedBlackBoardItem.Value;
                            _text.color     = Color.white;
                            _text.text = _text.text.Replace("_____",
                                $"{Parameter.GetValue()} `");
                        }
                        else
                        {
                            _showReject = true;
                            var originalColor = _text.color;
                            _text.DOColor(Color.red, 0.05f)
                                .SetLoops(3 * 2, LoopType.Yoyo) // Yoyo: 红↔原色交替
                                .OnComplete(() =>
                                {
                                    _text.color = originalColor; // 确保最终颜色正确（防浮点误差）
                                    _showReject = false;
                                });
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

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            foreach (var (key, value) in BlackBoardItems)
            {
                FindFirstObjectByType<BlackBoardController>().SetItem(key, value);
            }
        }
    }
}