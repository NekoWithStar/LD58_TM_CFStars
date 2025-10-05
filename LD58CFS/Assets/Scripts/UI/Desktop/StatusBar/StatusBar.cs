#region

//文件创建者：Egg
//创建时间：10-05 10:24

#endregion

using System;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class StatusBar : MonoBehaviour
    {
        [SerializeField] private Text _text;

        private void Update()
        {
            _text.text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }
}