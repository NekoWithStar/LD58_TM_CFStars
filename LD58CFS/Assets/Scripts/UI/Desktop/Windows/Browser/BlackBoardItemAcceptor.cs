#region

//文件创建者：Egg
//创建时间：10-06 10:42

#endregion

using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LD58.UI
{
    public abstract class BlackBoardItemAcceptor : AbstractController, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image       _bk;
        private                  IUnRegister _unRegister;

        protected abstract void Accept(BlackBoardItem blackBoardItem);
        
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (Selection.SelectedBlackBoardItem)
            {
                _bk.color = Color.gray;
                _unRegister = this.RegisterEvent<ReleaseBlackBoardItemEvent>(e =>
                {
                    if (Selection.SelectedBlackBoardItem)
                    {
                        Accept(Selection.SelectedBlackBoardItem);
                 
                    }
                });
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _bk.color = Color.white;
            _unRegister?.UnRegister();
        }
    }
}