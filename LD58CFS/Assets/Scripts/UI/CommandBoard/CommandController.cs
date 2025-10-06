#region

//文件创建者：Egg
//创建时间：10-04 08:30

#endregion

using System;
using EggFramework;
using EggFramework.Util.EggCMD;
using LD58.Constant;
using LD58.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58.UI.CommandBoard
{
    public sealed class CommandController : MonoBehaviour
    {
        [SerializeField] private CommandBoardItem _commandBoardItemPrefab;
        [SerializeField] private Transform        _content;
        [SerializeField] private CommandData      _commandData;

#if UNITY_EDITOR
        [SerializeField] private bool _debug;
#endif
        private void Awake()
        {
            _content.DestroyChild();
#if UNITY_EDITOR
            if (_debug)
            {
                AddCommand(CommandConstant.WEB_SCAN);
                AddCommand(CommandConstant.SEND_MAIL);
            }
#endif
        }

        public void SetCommandData(CommandData commandData)
        {
            Clear();
            _commandData = commandData;
            foreach (var cmd in commandData.Commands)
            {
                AddCommand(cmd.CommandName);
            }
        }

        public void Clear()
        {
            _content.DestroyChild();
        }

        [Button]
        public CommandBoardItem AddCommand(string command)
        {
            foreach (var cmd in _commandData.Commands)
            {
                if (cmd.CommandName == command)
                {
                    var item = Instantiate(_commandBoardItemPrefab, _content);
                    item.Command = CloneMapUtil<CommandDefinition>.Clone(cmd);
                    item.SetText(command);
                    return item;
                }
            }

            return null;
        }
    }
}