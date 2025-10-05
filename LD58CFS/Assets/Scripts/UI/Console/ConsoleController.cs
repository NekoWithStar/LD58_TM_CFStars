#region

//文件创建者：Egg
//创建时间：10-04 06:48

#endregion

using Cysharp.Threading.Tasks;
using LD58.Model;
using LD58.Util;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed partial class ConsoleController : AbstractController
    {
        [SerializeField] private GameObject _consoleItemPrefab;

        [SerializeField] private Transform _powerShellItemParent;
        [SerializeField] private Transform _browserConsoleItemParent;

        [SerializeField] private Button _powerShellButton;
        [SerializeField] private Button _browerConsoleButton;
        [SerializeField] private Button _clearButton;
        [SerializeField] private Button _recallButton;
        [SerializeField] private Button _enterButton;

        [SerializeField] private ScrollRect _scrollRect;
        
        private ISettingModel _settingModel;

        public bool IsPowerShell { get; private set; } = true;

        public ConsoleItem LastItem
        {
            get => IsPowerShell ? _powerShellLastItem : _browserConsoleLastItem;
            private set
            {
                if (IsPowerShell) _powerShellLastItem = value;
                else _browserConsoleLastItem          = value;
            }
        }

        private ConsoleItem       _powerShellLastItem;
        private ConsoleItem       _browserConsoleLastItem;
        public  bool              ListenForCommand { get; private set; }
        private CommandDefinition _command;
        private int               _pendingListenCommandParameterIndex = 0;
        private bool              _isPendingInput                     = false;
        private ConsoleItem       _pendingInputItem;
        private bool              _isExecutingCommand = false;
        public  CommandDefinition Command => _command;

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

            _enterButton?.onClick.AddListener(() =>
            {
                if (!_isExecutingCommand && !_isPendingInput)
                    AddFinishLine();
            });

            _recallButton?.onClick.AddListener(Recall);
            _settingModel = this.GetModel<ISettingModel>();
            SetUp();
        }

        private void Recall()
        {
            if (!_isExecutingCommand)
            {
                ListenForCommand               = false;
                _isPendingInput                = false;
                _pendingInputItem.PendingInput = false;
                _pendingInputItem.SetColor(Color.gray);
                AddItem("^C");
                AddFinishLine();
            }
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

        public void StartListeningForCommand(CommandDefinition command)
        {
            if (ListenForCommand)
            {
                Recall();
            }
            ListenForCommand = true;
            if (LastItem)
                LastItem.DisplayCursor = false;
            _command = command;
            AddItemWithoutNewLine(command.Parameters.Count == 0 ? command.CommandName : $"{command.CommandName} `");
            _pendingListenCommandParameterIndex = 0;
            _isPendingInput                     = true;
            if (command.Parameters.Count == 0)
            {
                DoCommand();
                _isPendingInput = false;
            }
            else
            {
                PendingNextParameter();
            }
        }

        private void PendingNextParameter()
        {
            if (_pendingListenCommandParameterIndex >= _command.Parameters.Count)
            {
                _isPendingInput = false;
                DoCommand();
                return;
            }

            _pendingInputItem =
                AddItem($"  -{_command.Parameters[_pendingListenCommandParameterIndex].Parameter.Name}: _____");
            _pendingInputItem.PendingInput = true;
            _pendingInputItem.SetColor(Color.yellow);
            _pendingInputItem.Parameter = _command.Parameters[_pendingListenCommandParameterIndex];
        }

        private void Update()
        {
            if (_isPendingInput)
            {
                if (!_pendingInputItem.PendingInput)
                {
                    _pendingListenCommandParameterIndex++;
                    PendingNextParameter();
                }
            }
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
            _recallButton?.onClick.RemoveAllListeners();
            _enterButton?.onClick.RemoveAllListeners();
        }
    }
}