#region

//文件创建者：Egg
//创建时间：10-04 04:31

#endregion

using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class BlackBoardItem : AbstractController, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private Text _dragText;
        [SerializeField] private Text _text;

        public string Key;
        public string Value;
        
        private Vector2 _offset;
        private Vector2 _startPos;

        private Canvas _canvas
        {
            get
            {
                if (!_canvasInst)
                {
                    _canvasInst = FindFirstObjectByType<Canvas>();
                }

                return _canvasInst;
            }
        }
        private Canvas _canvasInst;
        private void Awake()
        {
            _dragText.enabled = false;
        }
        
        public void SetText(string text)
        {
            _text.text     = text;
            _dragText.text = text;
            _text.transform.DOShakeScale(0.2f, new Vector3(0.2f,     0.2f));
            _dragText.transform.DOShakeScale(0.2f, new Vector3(0.2f, 0.2f));
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _offset   = Vector2.zero;
            _startPos = _dragText.GetComponent<RectTransform>().anchoredPosition;
            _dragText.transform.SetParent(_canvas.transform, true);
            _dragText.enabled                = true;
            Selection.SelectedBlackBoardItem = this;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _dragText.transform.SetParent(transform, true);
            _dragText.GetComponent<RectTransform>().anchoredPosition = _startPos;
            _dragText.enabled                                        = false;
            this.SendEvent(new ReleaseBlackBoardItemEvent
            {
                Item = this
            });
            Selection.SelectedBlackBoardItem                         = null;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _offset                                                  += eventData.delta;
            _dragText.GetComponent<RectTransform>().anchoredPosition += eventData.delta;
        }
    }
}