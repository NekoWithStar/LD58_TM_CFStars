#region

//文件创建者：Egg
//创建时间：10-04 08:26

#endregion

using System;
using System.Collections.Generic;
using EggFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58.UI
{
    public sealed class BlackBoardController : MonoBehaviour
    {
        [SerializeField] private GameObject _blackBoardItemPrefab;
        [SerializeField] private Transform  _content;

        [SerializeField] private bool _debug;
        private void Awake()
        {
            _content.DestroyChild();
#if UNITY_EDITOR
            if (_debug)
            {
                SetItem("qwertyuiop1234567890", "qwertyuiop1234567890");
                SetItem("who u r", "Cat D");
                SetItem("MOON_CAKE","MoonCake");
                SetItem("ADDRESS","www.bmail.com");
                SetItem("www.bmail.com/admin/login","www.bmail.com/admin/login");
                SetItem("Kesi@Bmail.com","Kesi@Bmail.com");
            }
#endif
        }

        private readonly Dictionary<string, BlackBoardItem> _boardItems = new();

        [Button]
        public void SetItem(string key, string value)
        {
            if (_boardItems.TryGetValue(key, out var bbi))
            {
                bbi.SetText($"{value}");
                bbi.Value = value;
                return;
            }
            var item = Instantiate(_blackBoardItemPrefab, _content);
            bbi  = item.GetComponent<BlackBoardItem>();
            bbi.SetText($"{value}");
            bbi.Key   = key;
            bbi.Value = value;
            _boardItems[key] = bbi;
        }

        [Button]
        public void Clear()
        {
            foreach (Transform trans in _content.transform)
            {
                Destroy(trans.gameObject);
            }
            _boardItems.Clear();
        }
    }
}