#region

//文件创建者：Egg
//创建时间：10-04 06:48

#endregion

using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LD58.UI.CommandBoard;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class ConsoleController : MonoBehaviour
    {
        [SerializeField] private GameObject _consoleItemPrefab;

        [SerializeField] private Transform _powerShellItemParent;
        [SerializeField] private Transform _browserConsoleItemParent;

        [SerializeField] private Button     _powerShellButton;
        [SerializeField] private Button     _browerConsoleButton;
        [SerializeField] private Button     _clearButton;
        [SerializeField] private ScrollRect _scrollRect;

        public bool        IsPowerShell { get; private set; } = true;
        public ConsoleItem LastItem     { get; private set; }

        public  bool    ListenForCommand { get; private set; }
        private Command _command;

        public bool DisplayCursor
        {
            get => LastItem && LastItem.DisplayCursor;
            set
            {
                if (LastItem)
                    LastItem.DisplayCursor = value;
            }
        }

        private void Awake()
        {
            _clearButton?.onClick.AddListener(() =>
            {
                Clear();
                AddFinishLine();
            });

            _browerConsoleButton?.onClick.AddListener(() => { Switch(false); });

            _powerShellButton?.onClick.AddListener(() => { Switch(true); });
            SetUp();
        }

        [Button]
        public void Switch(bool isPowerShell)
        {
            if (IsPowerShell == isPowerShell) return;
            IsPowerShell = isPowerShell;
            _scrollRect.content = isPowerShell
                ? _powerShellItemParent.GetComponent<RectTransform>()
                : _browserConsoleItemParent.GetComponent<RectTransform>();
            _powerShellItemParent.gameObject.SetActive(IsPowerShell);
            _browserConsoleItemParent.gameObject.SetActive(!IsPowerShell);
        }

        public void StartListeningForCommand(Command command)
        {
            if (ListenForCommand) return;
            ListenForCommand = true;
            if (LastItem)
                LastItem.DisplayCursor = false;
            _command = command;
            AddItemWithoutNewLine(command.CommandName);
        }

        public void SetUp()
        {
            SetUpPowerShell();
            SetUpBrowserConsole();
            Switch(true);
        }

        private void SetUpBrowserConsole()
        {
            IsPowerShell = false;
            Clear();
            AddItem("Mozilla Firefox");
            AddItem("Version 102.0.1 (64-bit)");
            AddItem("Copyright (C) Mozilla Corporation. All rights reserved.");
            AddItemLine();
            AddFinishLine();
        }

        private void SetUpPowerShell()
        {
            IsPowerShell = true;
            Clear();
            AddItem("Windows PowerShell");
            AddItem("Copyright (C) Microsoft Corporation. All rights reserved.");
            AddItemLine();
            AddFinishLine();
        }

        [Button]
        public void AddItemWithoutNewLine(string text)
        {
            if (LastItem)
            {
                LastItem.SetText(LastItem.Text + text);
                LastItem.DisplayCursor = false;
            }
            else
            {
                AddItem(text);
            }
        }

        public void SetLastItem(string text)
        {
            if (LastItem)
            {
                LastItem.SetText(text);
            }
        }

        [Button]
        public ConsoleItem AddItem(string text)
        {
            if (LastItem)
                LastItem.DisplayCursor = false;
            var go = Instantiate(_consoleItemPrefab, IsPowerShell ? _powerShellItemParent : _browserConsoleItemParent);
            LastItem = go.GetComponent<ConsoleItem>();
            LastItem.SetText(text);
            DelayFixed2Bottom();
            return LastItem;

            async void DelayFixed2Bottom()
            {
                await UniTask.DelayFrame(3);
                _scrollRect.normalizedPosition = new Vector2(1, 0);
            }
        }

        [Button]
        public void AddItemLine()
        {
            AddItem("");
        }

        [Button]
        public void AddFinishLine()
        {
            AddItem(IsPowerShell ? "PS C:\\Users\\Administrator>" : ">");
            LastItem.DisplayCursor = true;
            LastItem.DisplayLeft   = !IsPowerShell;
        }

        [Button]
        public void Clear()
        {
            foreach (Transform child in IsPowerShell ? _powerShellItemParent : _browserConsoleItemParent)
            {
                Destroy(child.gameObject);
            }

            LastItem = null;
        }

        private void OnDestroy()
        {
            _clearButton?.onClick.RemoveAllListeners();
            _browerConsoleButton?.onClick.RemoveAllListeners();
            _powerShellButton?.onClick.RemoveAllListeners();
        }
    }
}