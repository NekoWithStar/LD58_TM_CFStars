#region

//文件创建者：Egg
//创建时间：02-11 11:16

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace LD58
{
    [Serializable]
    public class LocalizationPackage
    {
        public SystemLanguage          Language;
        public Dictionary<int, string> LocalizationData;
    }

    public static class LocalizationUtil
    {
#if UNITY_EDITOR
        public static void ResetAll()
        {
            PlayerPrefs.SetInt("NextLocalizationId", 100000);
            ResetLocalizationIdInScene();
        }

        public static void ResetLocalizationIdInScene()
        {
            var ctrls = Object.FindObjectsOfType<LocalizationController>(true);
            foreach (var ctrl in ctrls)
            {
                ctrl.LocalizationId = 0;
                EditorUtility.SetDirty(ctrl);
            }

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        private static int _nextLocalizationId
        {
            get => PlayerPrefs.GetInt("NextLocalizationId", 100000);
            set => PlayerPrefs.SetInt("NextLocalizationId", value);
        }

        public static void CollectLevelData(TextAsset asset)
        {
            var dirPath  = "Assets/Data/Localization";
            var fileName = "ChineseSimplified.json";

            LocalizationPackage package;

            if (File.Exists($"{dirPath}/{fileName}"))
            {
                package = JsonConvert.DeserializeObject<LocalizationPackage>(File.ReadAllText($"{dirPath}/{fileName}"));
            }
            else
            {
                package = new LocalizationPackage
                {
                    Language         = SystemLanguage.ChineseSimplified,
                    LocalizationData = new Dictionary<int, string>()
                };
            }
            
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            File.WriteAllText($"{dirPath}/{fileName}", JsonConvert.SerializeObject(package));
        }

        public static void AssignLocalizationIdInSceneAndOutputSample()
        {
            var dirPath  = "Assets/Data/Localization";
            var fileName = "ChineseSimplified.json";

            LocalizationPackage package;

            if (File.Exists($"{dirPath}/{fileName}"))
            {
                package = JsonConvert.DeserializeObject<LocalizationPackage>(File.ReadAllText($"{dirPath}/{fileName}"));
            }
            else
            {
                package = new LocalizationPackage
                {
                    Language         = SystemLanguage.ChineseSimplified,
                    LocalizationData = new Dictionary<int, string>()
                };
            }

            var ctrls = Object.FindObjectsOfType<LocalizationController>(true);
            foreach (var ctrl in ctrls)
            {
                if (ctrl.LocalizationId == 0)
                {
                    ctrl.LocalizationId = _nextLocalizationId;
                    EditorUtility.SetDirty(ctrl);
                    package.LocalizationData[_nextLocalizationId] = ctrl.Content;
                    _nextLocalizationId++;
                }
                else
                {
                    package.LocalizationData[ctrl.LocalizationId] = ctrl.Content;
                }
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            File.WriteAllText($"{dirPath}/{fileName}", JsonConvert.SerializeObject(package));
            AssetDatabase.ImportAsset($"{dirPath}/{fileName}");
            AssetDatabase.Refresh();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        public static void AssignLocalizationId()
        {
            var ctrls = Object.FindObjectsByType<LocalizationController>(FindObjectsSortMode.None);
            foreach (var ctrl in ctrls)
            {
                if (ctrl.LocalizationId == 0)
                {
                    ctrl.LocalizationId = _nextLocalizationId;
                    EditorUtility.SetDirty(ctrl);
                    _nextLocalizationId++;
                }
            }

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
#endif
    }
}