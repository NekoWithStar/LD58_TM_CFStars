#region

//文件创建者：Egg
//创建时间：10-06 11:35

#endregion

using System.Collections.Generic;
using EggFramework;
using EggFramework.SimpleAudioSystem.Constant;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI.Login
{
    public sealed class LoginPSD : BlackBoardItemAcceptor
    {
        [SerializeField] private string _desc = "password";
        [SerializeField] private Text   _text;
        [SerializeField] private bool   _passwordMode;
        public                   string Value { get; private set; }

        public void Clear()
        {
            Value      = string.Empty;
            _text.text = string.Empty;
        }
        protected override void Accept(BlackBoardItem blackBoardItem)
        {
            this.PlaySFX(AudioConstant.SFX.BALLOON);
            var console = FindFirstObjectByType<ConsoleController>();
            console.Switch(false);
            console.AddItemWithoutNewLine($"document.getElementById(\"{_desc}\").value = \"{blackBoardItem.Value}\"");
            console.AddFinishLine();
            _text.text = _passwordMode ? new string('*', blackBoardItem.Value.Length) : blackBoardItem.Value;

            Value = blackBoardItem.Value;
        }
    }
}