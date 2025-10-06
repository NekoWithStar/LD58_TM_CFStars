#region

//文件创建者：Egg
//创建时间：10-06 11:52

#endregion

using System;
using EggFramework;
using EggFramework.SimpleAudioSystem.Constant;
using LD58.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI.Login
{
    public sealed class LoginBtn : AbstractController
    {
        [SerializeField] private Button   _button;
        [SerializeField] private LoginPSD _accountText;
        [SerializeField] private LoginPSD _psdText;
        [SerializeField] private string   account  = "Kesi@Bmail.com";
        [SerializeField] private string   password = "qwertyuiop1234567890";
        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                var console = FindFirstObjectByType<ConsoleController>();
                console.AddItemWithoutNewLine("Log In");
                console.AddItem($"- account = {account}");
                console.AddItem($"- password = {password}");
                if (_accountText.Value == account && _psdText.Value == password)
                {
                    this.PlaySFX(AudioConstant.SFX.SIGN);
                    console.AddItem("Success".ToGreen());
                    console.AddFinishLine();
                    Debug.Log("通过第一关");
                }
                else
                {
                    this.PlaySFX(AudioConstant.SFX.SYSTEMERROR);
                    console.AddItem("Log In Fail".ToRed());
                    console.AddFinishLine();
                    _accountText.Clear();
                    _psdText.Clear();
                }
            });
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}