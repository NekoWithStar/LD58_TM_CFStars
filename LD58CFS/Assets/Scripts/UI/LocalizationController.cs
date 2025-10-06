#region

//文件创建者：Egg
//创建时间：02-10 10:31

#endregion

using System;
using System.Collections.Generic;
using LD58.Model;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LD58
{
    [RequireComponent(typeof(Text))]
    public sealed class LocalizationController : AbstractController
    {
        [LabelText("本地化字段Id，不要手动改动")] public int LocalizationId;

        private Text _text
        {
            get
            {
                if (!_textInst)
                {
                    _textInst = GetComponent<Text>();
                }

                return _textInst;
            }
        }

        public string Content => _text.text;

        private Text _textInst;

        private string _contentToTranslate;

        public List<LanguageItem> LanguageItems = new();

        public bool AbleToUpdateAutomatically = true;

        private bool   _overriding;
        private string _preservedContent;

        public void OverrideContent(string content)
        {
            _overriding = true;
            _text.text  = content;
        }

        public void ModifyOverrideContent(string content)
        {
            if (_overriding)
                _text.text = content;
        }

        public void ResetContent()
        {
            _overriding = false;
            _text.text  = _preservedContent;
        }
        
        [Serializable]
        public class LanguageItem
        {
            public SystemLanguage Language;
            public string         Content;
        }

        private ISettingModel _settingModel;

        private void Awake()
        {
            _settingModel = this.GetModel<ISettingModel>();
            _settingModel.Language.RegisterWithInitValue(val =>
            {
                var item = LanguageItems.Find(item => item.Language == val);
                if (item != null)
                {
                    if (!_overriding)
                        _text.text = item.Content;
                    else
                    {
                        _preservedContent = item.Content;
                    }
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }
}