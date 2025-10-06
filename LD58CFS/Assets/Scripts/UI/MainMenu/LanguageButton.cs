#region

//文件创建者：Egg
//创建时间：10-06 05:41

#endregion

using System;
using LD58.Model;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI.MainMenu
{
    public sealed class LanguageButton : AbstractController
    {
        [SerializeField] private Button        _button;
        private                  ISettingModel _settingModel;

        private void Awake()
        {
            _settingModel = this.GetModel<ISettingModel>();
            _button.onClick.AddListener(() =>
            {
                if (_settingModel.Language.Value == SystemLanguage.ChineseSimplified)
                {
                    _settingModel.Language.Value = SystemLanguage.English;
                }
                else if (_settingModel.Language.Value == SystemLanguage.English)
                {
                    _settingModel.Language.Value = SystemLanguage.ChineseSimplified;
                }
            });
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}