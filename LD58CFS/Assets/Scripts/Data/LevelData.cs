#region

//文件创建者：Egg
//创建时间：10-06 01:02

#endregion

using System;
using System.Collections.Generic;
using LD58.UI;
using LD58.UI.CommandBoard;
using Sirenix.OdinInspector;

namespace LD58.Data
{
    [Serializable]
    public sealed class LevelData
    {
        [LabelText("关卡名")]    public string       LevelName;
        [LabelText("关卡工具配置")] public CommandData  CommandData;
        [LabelText("关卡预设字段")] public List<string> InitParams = new();

        [FoldoutGroup("浏览器预设")] [LabelText("搜索栏历史记录")]
        public List<BrowserWindow.SearchItem> SearchItems = new();
        [FoldoutGroup("浏览器预设")] [LabelText("链接历史记录")]
        public List<BrowserWindow.LinkItem> LinkItems = new();
        [FoldoutGroup("浏览器预设")] [LabelText("可以被链接栏接受的值")]
        public List<string> AcceptLinkValues = new();
    }
}