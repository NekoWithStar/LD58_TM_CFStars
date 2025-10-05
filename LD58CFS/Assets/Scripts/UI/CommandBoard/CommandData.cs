#region

//文件创建者：Egg
//创建时间：10-04 08:37

#endregion

using System.Collections.Generic;
using LD58.Util;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace LD58.UI.CommandBoard
{
    [CreateAssetMenu(fileName = "CommandData", menuName = "LD58/CommandData", order = 0)]
    public sealed class CommandData : ScriptableObject
    {
        public List<CommandDefinition> Commands = new();
#if UNITY_EDITOR
        [InlineButton("读取所有文件")]
        public List<TextAsset> TextAssets = new();

        private void 读取所有文件()
        {
            foreach (var textAsset in TextAssets)
            {
                ReadFromFile(textAsset);
            }
        }
        [Button]
        public void ReadFromFile(TextAsset textAsset)
        {
            var command = new CommandFileParser().ParseFile(textAsset);
            var find    = Commands.Find(cmd => cmd.CommandName == command.CommandName);
            if (find != null)
                Commands.Remove(find);
            Commands.Add(command);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Button]
        [InfoBox("从JSON文件读取命令定义")]
        public void ReadFromJsonFile(TextAsset jsonAsset)
        {
            // 创建临时文件路径
            var tempPath = "Temp/" + jsonAsset.name + ".json";
            System.IO.File.WriteAllText(tempPath, jsonAsset.text);
            
            try
            {
                var command = JsonCommandParser.ParseFile(tempPath);
                var find    = Commands.Find(cmd => cmd.CommandName == command.CommandName);
                if (find != null)
                    Commands.Remove(find);
                Commands.Add(command);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"成功从JSON加载命令: {command.CommandName}");
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
            }
        }

        [Button]
        public void Test()
        {
            CommandFileParser.Test();
        }
#endif
    }
}