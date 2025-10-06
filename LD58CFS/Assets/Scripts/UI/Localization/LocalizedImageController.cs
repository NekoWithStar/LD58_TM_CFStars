#region

//文件创建者：Egg
//创建时间：10-06 05:11

#endregion

using System;
using LD58.Model;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace LD58
{
    public sealed class LocalizedImageController : AbstractController
    {
        private                  ISettingModel _settingModel;
        [SerializeField] private Image         _target;
        [SerializeField] private Sprite        _chinese;
        [SerializeField] private Sprite        _english;
         private void Awake()
        {
            _settingModel = this.GetModel<ISettingModel>();
            _settingModel.Language.RegisterWithInitValue(val =>
            {
                if (val == SystemLanguage.ChineseSimplified)
                {
                    _target.sprite = _chinese;
                }

                if (val == SystemLanguage.English)
                {
                    _target.sprite = _english;
                }
            });
        }
    }
}