#region

//文件创建者：Egg
//创建时间：10-04 08:13

#endregion

using System;
using EggFramework;
using LD58.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LD58.UI.CommandBoard
{
    public sealed class CommandBoardItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Text  _text;
        [SerializeField] private Image _bk;

        private ConsoleController _consoleController;
        public CommandDefinition Command { get; set; }
        public void SetText(string text)
        {
            _text.text = text;
        }

        private void Awake()
        {
            _consoleController = FindFirstObjectByType<ConsoleController>();
        }

        private void Update()
        {
            if(_consoleController.ListenForCommand && _consoleController.Command.CommandName == Command.CommandName)
                _bk.color = Color.gray;
            else
                _bk.color = Color.white;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _consoleController.StartListeningForCommand(CloneMapUtil<CommandDefinition>.Clone(Command));
        }
    }
}