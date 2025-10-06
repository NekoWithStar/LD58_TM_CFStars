#region

//文件创建者：Egg
//创建时间：10-06 10:59

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI.DataBase
{
    public sealed class DataBaseRow : MonoBehaviour
    {
        [SerializeField] private List<Text> _texts = new();

        public void SetText(List<string> text)
        {
            for (var i = 0; i < text.Count; i++)
            {
                _texts[i].text = text[i];
            }
        }
    }
}