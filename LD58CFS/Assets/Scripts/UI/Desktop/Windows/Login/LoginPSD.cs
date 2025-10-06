#region

//文件创建者：Egg
//创建时间：10-06 11:35

#endregion

using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI.Login
{
    public sealed class LoginPSD : BlackBoardItemAcceptor
    {
        [SerializeField] private Text _text;
        protected override void Accept(BlackBoardItem blackBoardItem)
        {
            _text.text = blackBoardItem.Value;
        }
    }
}