#region

//文件创建者：Egg
//创建时间：10-05 11:50

#endregion

using System;
using System.Collections.Generic;
using LD58.Model;
using LD58.Util;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class BrowserWindow : AbstractWindowController
    {
        [SerializeField] private Dropdown _linkDropdown;
        [SerializeField] private Dropdown _searchDropdown;

#if UNITY_EDITOR
        [SerializeField] private bool _debug;
#endif
        [Serializable]
        public class SearchItem
        {
            public string       Desc;
            public List<string> Gains = new();
        }

        [SerializeField] private List<SearchItem> _valuePairs = new();

        protected override void Awake()
        {
            base.Awake();
            Clear();
#if UNITY_EDITOR
            AddBrowserLinkHistoryItem("www.bmain.com");
            AddBrowserLinkHistoryItem("www.meow-awards.com/login");
#endif
            foreach (var keyValuePair in _valuePairs)
            {
                AddBrowserSearchHistoryItem(keyValuePair.Desc);
            }

            _searchDropdown.onValueChanged.AddListener(OnSearch);
        }

        private void OnSearch(int index)
        {
            foreach (var gain in _valuePairs[index].Gains)
            {
                FindFirstObjectByType<BlackBoardController>().SetItem(gain, gain);
            }
        }

        [Button]
        public void AddBrowserLinkHistoryItem(string history)
        {
            _linkDropdown.options.Add(new Dropdown.OptionData
            {
                text  = history,
                image = null
            });
        }

        [Button]
        public void AddBrowserSearchHistoryItem(string history)
        {
            _searchDropdown.options.Add(new Dropdown.OptionData
            {
                text  = history,
                image = null
            });
        }

        public void Clear()
        {
            _linkDropdown.options.Clear();
            _searchDropdown.options.Clear();
            _searchDropdown.onValueChanged.RemoveAllListeners();
        }
    }
}