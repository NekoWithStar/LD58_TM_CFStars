#region

//文件创建者：Egg
//创建时间：10-05 11:32

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LD58.UI
{
    [Serializable]
    public class WindowConfig
    {
        public string     WindowName;
        public Sprite     Icon;
        public GameObject Window;
    }

    public sealed class WindowsController : AbstractController
    {
        public List<WindowConfig> Windows = new();

        private enum EWindowState
        {
            Close,
            Minimize,
            Focused,
            Unfocused
        }

        private readonly Dictionary<string, EWindowState> _windowStates  = new();
        private readonly Dictionary<string, GameObject>   _windowObjects = new();

        private void Awake()
        {
            this.RegisterEvent<WindowOperationEvent>(async e =>
            {
                switch (e.Operation)
                {
                    case EWindowOperation.Open:
                        if (_windowStates.TryGetValue(e.WindowName, out var state))
                        {
                            if (state != EWindowState.Close)
                                FocusWindow(e.WindowName);
                            else
                            {
                                Load.ShowLoading();
                                await UniTask.Delay(Random.Range(500, 2000));
                                Load.HideLoading();
                                OpenWindow(e.WindowName);
                            }
                        }

                        break;
                    case EWindowOperation.Close:
                        CloseWindow(e.WindowName);
                        break;
                    case EWindowOperation.Minimize:
                        MinimizeWindow(e.WindowName);
                        break;
                    case EWindowOperation.Focus:
                        FocusWindow(e.WindowName);
                        break;
                    case EWindowOperation.ToggleActive:
                        ToggleActive(e.WindowName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            foreach (var windowConfig in Windows)
            {
                _windowStates[windowConfig.WindowName]  = EWindowState.Close;
                _windowObjects[windowConfig.WindowName] = windowConfig.Window;
                windowConfig.Window.SetActive(false);
            }
        }

        private void FocusWindow(string windowName)
        {
            var windowConfig = Windows.Find(w => w.WindowName == windowName);
            if (windowConfig == null)
            {
                Debug.LogError($"Window {windowName} not found");
                return;
            }

            if (_windowStates[windowName] == EWindowState.Close)
            {
                Debug.LogWarning($"Window {windowName} is closed, cannot focus");
                return;
            }
            
            var cachedState = _windowStates[windowName];

            windowConfig.Window.SetActive(true);
            windowConfig.Window.transform.SetAsLastSibling();
            windowConfig.Window.GetComponent<AbstractWindowController>().Focus = true;
            windowConfig.Window.GetComponent<AbstractWindowController>().OnFocus();
            _windowStates[windowName]                                          = EWindowState.Focused;
            foreach (var eWindowState in _windowStates.ToDictionary(kv => kv.Key, kv => kv.Value))
            {
                if (eWindowState.Key != windowName && eWindowState.Value == EWindowState.Focused)
                {
                    _windowStates[eWindowState.Key] = EWindowState.Unfocused;
                    _windowObjects[eWindowState.Key].GetComponent<AbstractWindowController>().Focus = false;
                }
            }
            if (cachedState != EWindowState.Minimize) return;

            var taskItem = FindFirstObjectByType<TaskBar>().GetTaskItem(windowName);
            _windowObjects[windowName].transform.position   = taskItem.transform.position;
            _windowObjects[windowName].transform.localScale = Vector3.one * 0.05f;
            var rawPos = _windowObjects[windowName].GetComponent<RectTransform>().anchoredPosition;
            DOTween.To(val =>
            {
                _windowObjects[windowName].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(rawPos,
                    _windowObjects[windowName]
                        .GetComponent<AbstractWindowController>().CachedPosition, val);
                _windowObjects[windowName].transform.localScale = Vector3.one * (0.05f + 0.95f * val);
            }, 0, 1, 0.2f).onComplete += () =>
            {
                _windowObjects[windowName].transform.localScale = Vector3.one;
            };
        }

        private void MinimizeWindow(string windowName)
        {
            var windowConfig = Windows.Find(w => w.WindowName == windowName);
            if (windowConfig == null)
            {
                Debug.LogError($"Window {windowName} not found");
                return;
            }

            if (_windowStates[windowName] == EWindowState.Close)
            {
                Debug.LogWarning($"Window {windowName} is closed, cannot minimize");
                return;
            }
            
            _windowObjects[windowName].GetComponent<AbstractWindowController>().Focus = false;
            _windowStates[windowName]                                                 = EWindowState.Minimize;

            var taskItem = FindFirstObjectByType<TaskBar>().GetTaskItem(windowName);
            _windowObjects[windowName].GetComponent<AbstractWindowController>().CachedPosition =
                _windowObjects[windowName].GetComponent<RectTransform>().anchoredPosition;
            var rawPos = _windowObjects[windowName].transform.position;
            DOTween.To(val =>
            {
                _windowObjects[windowName].transform.position = Vector3.Lerp(
                    rawPos,
                    taskItem.transform.position, val);
                _windowObjects[windowName].transform.localScale = Vector3.one * (1 - 0.95f * val);
            }, 0, 1, 0.2f).onComplete += () => { windowConfig.Window.SetActive(false); };
        }

        private void CloseWindow(string windowName)
        {
            var windowConfig = Windows.Find(w => w.WindowName == windowName);
            if (windowConfig == null)
            {
                Debug.LogError($"Window {windowName} not found");
                return;
            }

            if (_windowStates[windowName] == EWindowState.Close)
            {
                Debug.LogWarning($"Window {windowName} is already closed");
                return;
            }

            windowConfig.Window.SetActive(false);
            _windowStates[windowName]                                                 = EWindowState.Close;
            _windowObjects[windowName].GetComponent<AbstractWindowController>().Focus = false;
            FindFirstObjectByType<TaskBar>()?.RemoveTaskItem(windowName);
        }

        private void ToggleActive(string windowName)
        {
            var windowConfig = Windows.Find(w => w.WindowName == windowName);
            if (windowConfig == null)
            {
                Debug.LogError($"Window {windowName} not found");
                return;
            }

            if (_windowStates.TryGetValue(windowName, out var state))
            {
                switch (state)
                {
                    case EWindowState.Close:
                        return;
                    case EWindowState.Minimize:
                    case EWindowState.Unfocused:
                        FocusWindow(windowName);
                        break;
                    case EWindowState.Focused:
                        MinimizeWindow(windowName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OpenWindow(string windowName)
        {
            var windowConfig = Windows.Find(w => w.WindowName == windowName);
            if (windowConfig == null)
            {
                Debug.LogError($"Window {windowName} not found");
                return;
            }

            if (_windowStates[windowName] != EWindowState.Close)
            {
                Debug.LogWarning($"Window {windowName} is already open");
                return;
            }

            windowConfig.Window.SetActive(true);
            windowConfig.Window.transform.SetAsLastSibling();
            _windowStates[windowName]                                          = EWindowState.Focused;
            windowConfig.Window.GetComponent<AbstractWindowController>().Focus = true;
            windowConfig.Window.GetComponent<AbstractWindowController>().OnFocus();

            foreach (var eWindowState in _windowStates.ToDictionary(kv => kv.Key, kv => kv.Value))
            {
                if (eWindowState.Key != windowName && eWindowState.Value == EWindowState.Focused)
                {
                    _windowStates[eWindowState.Key] = EWindowState.Unfocused;
                    _windowObjects[eWindowState.Key].GetComponent<AbstractWindowController>().Focus = false;
                }
            }

            FindFirstObjectByType<TaskBar>()?.AddTaskItem(windowName, windowConfig.Icon);
        }
    }
}