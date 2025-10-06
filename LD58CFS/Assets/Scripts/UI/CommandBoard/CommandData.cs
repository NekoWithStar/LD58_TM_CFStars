#region

//文件创建者：Egg
//创建时间：10-04 08:37

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        [FolderPath]
        [InfoBox("选择包含JSON文件的文件夹")]
        public string JsonFolderPath = "Assets/Data/Level1";

        [Button("从文件夹批量读取JSON")]
        [InfoBox("清空现有命令并从指定文件夹批量加载所有JSON文件")]
        public void LoadAllJsonFromFolder()
        {
            if (string.IsNullOrEmpty(JsonFolderPath))
            {
                Debug.LogError("请先设置JsonFolderPath!");
                return;
            }

            if (!Directory.Exists(JsonFolderPath))
            {
                Debug.LogError($"文件夹不存在: {JsonFolderPath}");
                return;
            }

            // 清空现有命令列表
            int oldCount = Commands.Count;
            Commands.Clear();
            Debug.Log($"已清空 {oldCount} 个现有命令");

            // 获取所有JSON文件
            var jsonFiles = Directory.GetFiles(JsonFolderPath, "*.json", SearchOption.TopDirectoryOnly)
                                     .Where(f => !f.EndsWith(".meta"))
                                     .ToList();

            if (jsonFiles.Count == 0)
            {
                Debug.LogWarning($"在文件夹 {JsonFolderPath} 中未找到JSON文件");
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                return;
            }

            Debug.Log($"开始加载 {jsonFiles.Count} 个JSON文件...");
            int successCount = 0;
            int failCount = 0;

            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    var command = JsonCommandParser.ParseFile(jsonFile);
                    Commands.Add(command);
                    successCount++;
                    Debug.Log($"✓ 成功加载: {Path.GetFileName(jsonFile)} -> {command.CommandName}");
                }
                catch (System.Exception ex)
                {
                    failCount++;
                    Debug.LogError($"✗ 加载失败: {Path.GetFileName(jsonFile)}\n错误: {ex.Message}");
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"<color=green>批量加载完成!</color> 成功: {successCount}, 失败: {failCount}, 总计: {Commands.Count} 个命令");
        }

        [Button]
        public void Test()
        {
            CommandFileParser.Test();
        }
#endif
    }
}