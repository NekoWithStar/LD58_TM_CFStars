#region

//文件创建者：Egg
//创建时间：10-06 05:56

#endregion

using System;
using LD58.Model;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI.MainMenu
{
    public sealed class TitleController : AbstractController
    {
        [SerializeField] private Text _mainTitle;
        [SerializeField] private Text _subTitle;
        
        private ISettingModel _settingModel;

        private void Awake()
        {
            _settingModel = this.GetModel<ISettingModel>();
            _settingModel.Language.RegisterWithInitValue(val =>
            {
                if (val == SystemLanguage.ChineseSimplified)
                {
                    _mainTitle.text = "不速之客";
                    _subTitle.text  = "The Internet Collector";
                }

                if (val == SystemLanguage.English)
                {
                    _subTitle.text  = "不速之客";
                    _mainTitle.text = "The Internet Collector";
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }
}