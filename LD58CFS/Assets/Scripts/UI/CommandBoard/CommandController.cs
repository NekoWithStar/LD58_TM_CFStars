#region

//文件创建者：Egg
//创建时间：10-04 08:30

#endregion

using EggFramework;
using UnityEngine;

namespace LD58.UI.CommandBoard
{
    public sealed class CommandController : MonoBehaviour
    {
        [SerializeField] private CommandBoardItem _commandBoardItemPrefab;
        [SerializeField] private Transform        _content;
        [SerializeField] private CommandData      _commandData;

        public CommandBoardItem AddCommand(string command)
        {
            foreach (var cmd in _commandData.Commands)
            {
                if (cmd.CommandName == command)
                {
                    var item = Instantiate(_commandBoardItemPrefab, _content);
                    item.Command = CloneMapUtil<Command>.Clone(cmd);
                    item.SetText(command);
                    return item;
                }
            }

            return null;
        }
    }
}