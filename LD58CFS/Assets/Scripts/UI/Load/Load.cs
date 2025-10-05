#region

//文件创建者：Egg
//创建时间：10-05 11:16

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LD58.UI
{
    public sealed class Load : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _icons = new();
        [SerializeField] private Image        _icon;

        private int _currentIndex = 0;

        public static void ShowLoading()
        {
            FindFirstObjectByType<Load>(FindObjectsInactive.Include).gameObject.SetActive(true);
        }
        
        public static void HideLoading()
        {
            FindFirstObjectByType<Load>(FindObjectsInactive.Include).gameObject.SetActive(false);
        }
        private void Awake()
        {
            _icon.sprite = _icons[_currentIndex];
        }

        private void OnEnable()
        {
            _currentIndex = 0;
            _icon.sprite  =  _icons[_currentIndex];
        }

        private void Update()
        {
            if (Mathf.Floor((Time.time + Time.deltaTime) * 3) > Mathf.Floor(Time.time * 3))
            {
                _currentIndex++;
                _currentIndex %= _icons.Count;
                _icon.sprite  =  _icons[_currentIndex];
            }
        }
    }
}