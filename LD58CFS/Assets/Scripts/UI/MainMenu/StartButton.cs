#region

//文件创建者：Egg
//创建时间：10-06 06:04

#endregion

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LD58.UI.MainMenu
{
    public sealed class StartButton : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private Button _button;
        private                  int    _clickCount;

        [SerializeField] private List<Vector2> _positions = new();

        private void Awake()
        {
            _button.onClick.AddListener(async () =>
            {
                if (_clickCount >= _positions.Count)
                {
                    FindFirstObjectByType<SceneLoadIn>().OnButtonClick();
                }
                else
                {
                    if (_clickCount == 0)
                    {
                        gameObject.SetActive(false);
                        GetComponent<RectTransform>().anchoredPosition = _positions[_clickCount];
                        await UniTask.Delay(1000);
                        gameObject.SetActive(true);
                        _clickCount++;
                    }
                }
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_clickCount != 0 && _clickCount <= _positions.Count - 1)
            {
                GetComponent<RectTransform>().DOAnchorPos(_positions[_clickCount], 0.2f);
                _clickCount++;
            }
        }
    }
}