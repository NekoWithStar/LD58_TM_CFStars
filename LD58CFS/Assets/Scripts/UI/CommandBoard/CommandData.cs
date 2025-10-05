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
        public void Test()
        {
            CommandFileParser.Test();
        }
#endif
    }
}