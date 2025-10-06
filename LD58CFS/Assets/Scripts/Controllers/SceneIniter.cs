#region

//文件创建者：Egg
//创建时间：10-06 12:39

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework;
using EggFramework.SimpleAudioSystem.Constant;
using EggFramework.Util;
using LD58.Data;
using LD58.UI;
using LD58.UI.CommandBoard;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58
{
    public sealed class SceneIniter : AbstractController
    {
        [SerializeField] private LevelDatas _levelDatas;

        [ValueDropdown("GetLevelNames"), LabelText("启动关卡")]
        public string LevelName;

        [SerializeField, ValueDropdown("GetBGM")]
        public string InitBGM;

#if UNITY_EDITOR
        private IEnumerable<string> GetBGM()
        {
            return StorageUtil.LoadFromSettingFile("AudioConstant.BGMIds", new List<string>());
        }

        private IEnumerable<string> GetLevelNames()
        {
            return _levelDatas?.Datas.Select(data => data.LevelName);
        }
#endif
        private void Awake()
        {
            var levelData = _levelDatas.Datas.Find(data => data.LevelName == LevelName);
            if (levelData == null)
            {
                Debug.LogError($"没有找到关卡数据,关卡名{LevelName}");
                return;
            }

            this.ExecuteAfterAsyncInitedOrInstant(() =>
            {
                if (!string.IsNullOrEmpty(InitBGM))
                    this.PlayBGM(InitBGM);
            });

            FindFirstObjectByType<BlackBoardController>(FindObjectsInactive.Include).Clear();

            foreach (var levelDataInitParam in levelData.InitParams)
            {
                FindFirstObjectByType<BlackBoardController>(FindObjectsInactive.Include)
                    .SetItem(levelDataInitParam, levelDataInitParam);
            }

            FindFirstObjectByType<CommandController>(FindObjectsInactive.Include).Clear();
            FindFirstObjectByType<CommandController>(FindObjectsInactive.Include).SetCommandData(levelData.CommandData);

            FindFirstObjectByType<BrowserWindow>(FindObjectsInactive.Include).Clear();
            FindFirstObjectByType<BrowserWindow>(FindObjectsInactive.Include).SetBrowserInitData(levelData.SearchItems,
                levelData.LinkItems, levelData.AcceptLinkValues);
        }
    }
}