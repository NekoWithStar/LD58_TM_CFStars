#region

//文件创建者：Egg
//创建时间：02-11 12:06

#endregion

using System.Collections.Generic;
using LD58.Model;
using Newtonsoft.Json;
using QFramework;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD58
{
    public sealed class LocalizationHandle : AbstractController
    {
        public List<TextAsset>           LocalizationConfigTextAssets = new();
        public List<LocalizationPackage> Packages                     = new();

        private bool _loaded;

        [ShowInInspector]
        public SystemLanguage Language
        {
            get => this.GetModel<ISettingModel>().Language.Value;
            set => this.GetModel<ISettingModel>().Language.Value = value;
        }

        [Button]
        public void ReloadAllConfig()
        {
            Packages.Clear();
            foreach (var ta in LocalizationConfigTextAssets)
            {
                Packages.Add(JsonConvert.DeserializeObject<LocalizationPackage>(ta.text));
            }

            _loaded = true;
        }

        public string GetLocalizedStringByChineseSimplified(string chinese)
        {
            if (!_loaded) ReloadAllConfig();
            var target = this.GetModel<ISettingModel>().Language.Value;
            if (target == SystemLanguage.ChineseSimplified) return chinese;
            LocalizationPackage chinesePackage = null;
            foreach (var package in Packages)
            {
                if (package.Language == SystemLanguage.ChineseSimplified)
                {
                    chinesePackage = package;
                }
            }

            if (chinesePackage == null) return chinese;
            foreach (var package in Packages)
            {
                if (package.Language == target)
                {
                    foreach (var (key, value) in chinesePackage.LocalizationData)
                    {
                        if (value == chinese)
                        {
                            return package.LocalizationData.GetValueOrDefault(key, chinese);
                        }
                    }
                }
            }

            return chinese;
        }
#if UNITY_EDITOR
        [Button]
        public void DispatchLocalizationDataInScene()
        {
            var ctrls = FindObjectsOfType<LocalizationController>(true);
            foreach (var ctrl in ctrls)
            {
                var rawText = ctrl.Content;
                if (ctrl.LocalizationId != 0 && ctrl.AbleToUpdateAutomatically)
                {
                    ctrl.LanguageItems.Clear();
                    foreach (var package in Packages)
                    {
                        ctrl.LanguageItems.Add(new LocalizationController.LanguageItem
                        {
                            Language = package.Language,
                            Content  = package.LocalizationData.GetValueOrDefault(ctrl.LocalizationId, rawText)
                        });
                    }

                    EditorUtility.SetDirty(ctrl);
                }
            }

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        [Button]
        public void AssignLocalizationIdAndOutputSample()
        {
            LocalizationUtil.AssignLocalizationIdInSceneAndOutputSample();
        }

        [Button]
        public void ResetAll()
        {
            LocalizationUtil.ResetAll();
        }

        public TextAsset Asset;

        [Button]
        public void CollectLevelData()
        {
            LocalizationUtil.CollectLevelData(Asset);
        }

        [Button]
        public void ResetLocalizationIdInScene()
        {
            LocalizationUtil.ResetLocalizationIdInScene();
        }

        [Button]
        public void AssignLocalizationId()
        {
            LocalizationUtil.AssignLocalizationId();
        }
#endif
    }
}