#region

//文件创建者：Egg
//创建时间：10-05 11:50

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using LD58.Extension;
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
        [SerializeField] private Text     _linkLabel;

#if UNITY_EDITOR
        [SerializeField] private bool _debug;
#endif
        [Serializable]
        public class SearchItem
        {
            public string       Desc;
            public List<string> Gains = new();
        }

        [Serializable]
        public class LinkItem
        {
            public string Desc;
            public bool   CanNavigate;
        }

        [Serializable]
        public class NavigateConfig
        {
            public string     Link;
            public GameObject Panel;
        }

        [SerializeField] private List<string>         AcceptedLinkValues = new();
        [SerializeField] private List<SearchItem>     _valuePairs        = new();
        [SerializeField] private List<LinkItem>       _linkItems         = new();
        [SerializeField] private List<NavigateConfig> _navigateConfigs   = new();

        private int _preLinkIndex;

        protected override void Awake()
        {
            base.Awake();
            Clear();

            foreach (var linkItem in _linkItems)
            {
                _linkDropdown.options.Add(new Dropdown.OptionData
                {
                    text  = linkItem.Desc,
                    image = null
                });
            }

            foreach (var keyValuePair in _valuePairs)
            {
                _searchDropdown.options.Add(new Dropdown.OptionData
                {
                    text  = keyValuePair.Desc,
                    image = null
                });
            }

            _searchDropdown.onValueChanged.AddListener(OnSearch);
            _linkDropdown.onValueChanged.AddListener(OnLink);

            foreach (var navigateConfig in _navigateConfigs)
            {
                navigateConfig.Panel.SetActive(false);
            }

            _navigateConfigs[0].Panel.SetActive(true);

#if UNITY_EDITOR
            if (_debug)
            {
                AddBrowserLinkHistoryItem("www.goodinternet.com",      true);
                AddBrowserLinkHistoryItem("www.bmain.com",             false);
                AddBrowserLinkHistoryItem("www.meow-awards.com/login", false);
            }
#endif
        }

        protected override void OnDestroy()
        {
            _linkDropdown.onValueChanged.RemoveAllListeners();
            _searchDropdown.onValueChanged.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnSearch(int index)
        {
            foreach (var gain in _valuePairs[index].Gains)
            {
                FindFirstObjectByType<BlackBoardController>().SetItem(gain, gain);
            }
        }

        public void SetBrowserInitData(List<SearchItem> searchItems, List<LinkItem> linkItems, List<string> acceptLinks)
        {
            Clear();
            AcceptedLinkValues.AddRange(acceptLinks);
            foreach (var searchItem in searchItems)
            {
                AddBrowserSearchHistoryItem(searchItem.Desc, searchItem.Gains);
            }

            foreach (var linkItem in linkItems)
            {
                AddBrowserLinkHistoryItem(linkItem.Desc, linkItem.CanNavigate, true);
            }
        }

        private void OnLink(int index)
        {
            FindFirstObjectByType<BlackBoardController>().SetItem(_linkItems[index].Desc, _linkItems[index].Desc);
            var console = FindFirstObjectByType<ConsoleController>();
            console.Switch(false);
            if (_linkItems[index].CanNavigate)
            {
                _preLinkIndex = index;
                Navigate(_linkItems[index].Desc);
                console.AddItemWithoutNewLine($"window.location.href = \"{_linkItems[index].Desc}\"");
                console.AddFinishLine();
            }
            else
            {
                console.AddItemWithoutNewLine("[ERROR] : Access is invalid!".ToRed());
                console.AddFinishLine();
                _linkDropdown.SetValueWithoutNotify(_preLinkIndex);
            }
        }

        [Button]
        private void Navigate(string linkItem)
        {
            var config = _navigateConfigs.Find(nav => nav.Link == linkItem);
            if (config != null)
            {
                foreach (var navigateConfig in _navigateConfigs)
                {
                    navigateConfig.Panel.SetActive(false);
                }

                config.Panel.SetActive(true);
            }
        }

        [Button]
        public void AddBrowserLinkHistoryItem(string history, bool canNavigate, bool init = false)
        {
            if (!AcceptedLinkValues.Contains(history) && !init) return;
            if (_linkItems.Any(item => item.Desc == history)) return;

            _linkDropdown.options.Add(new Dropdown.OptionData
            {
                text  = history,
                image = null
            });

            _linkItems.Add(new LinkItem
            {
                Desc        = history,
                CanNavigate = canNavigate
            });
        }

        [Button]
        public void AddBrowserSearchHistoryItem(string history, List<string> gains)
        {
            if (_valuePairs.Any(ky => ky.Desc == history)) return;
            _searchDropdown.options.Add(new Dropdown.OptionData
            {
                text  = history,
                image = null
            });

            _valuePairs.Add(new SearchItem
            {
                Desc  = history,
                Gains = gains
            });
        }

        public void Clear()
        {
            _linkDropdown.options.Clear();
            _searchDropdown.options.Clear();
            _linkItems.Clear();
            _valuePairs.Clear();
        }
    }
}