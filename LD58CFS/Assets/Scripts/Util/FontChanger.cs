#region

//文件创建者：Egg
//创建时间：10-06 02:12

#endregion

using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.Util
{
    public sealed class FontChanger : MonoBehaviour
    {
        [SerializeField] private Font       _font;
        [SerializeField] private GameObject _target;

        private void Awake()
        {
            Destroy(gameObject);
        }
#if UNITY_EDITOR
        [Button("替换所有字体")]
        private void ChangeFontInScene()
        {
            if (!_target)
                foreach (var text in FindObjectsByType<Text>(FindObjectsSortMode.None))
                {
                    text.font = _font;
                    EditorUtility.SetDirty(text);
                }
            else
            {
                foreach (var componentsInChild in _target.GetComponentsInChildren<Text>(true))
                {
                    componentsInChild.font = _font;
                    EditorUtility.SetDirty(componentsInChild);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}