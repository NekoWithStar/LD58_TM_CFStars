#region

//文件创建者：Egg
//创建时间：10-04 08:13

#endregion

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LD58.UI.CommandBoard
{
    public sealed class CommandBoardItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Text _text;

        public Command Command { get; set; }
        public void SetText(string text)
        {
            _text.text = text;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}