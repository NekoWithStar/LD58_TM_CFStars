#region

//文件创建者：Egg
//创建时间：10-04 08:26

#endregion

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LD58.UI
{
    public sealed class BlackBoardController : MonoBehaviour
    {
        [SerializeField] private GameObject _blackBoardItemPrefab;
        [SerializeField] private Transform  _content;

        private readonly Dictionary<string, BlackBoardItem> _boardItems = new();

        [Button]
        public void SetItem(string key, string value)
        {
            if (_boardItems.TryGetValue(key, out var bbi))
            {
                bbi.SetText($"{key}:{value}");
                bbi.Value = value;
                return;
            }
            var item = Instantiate(_blackBoardItemPrefab, _content);
            bbi  = item.GetComponent<BlackBoardItem>();
            bbi.SetText($"{key}:{value}");
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