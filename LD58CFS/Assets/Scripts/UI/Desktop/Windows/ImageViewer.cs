#region

//文件创建者：Egg
//创建时间：10-05 01:15

#endregion

using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LD58.UI
{
    public sealed class ImageViewer : AbstractWindowController
    {
        [SerializeField] private Image _image;
        [SerializeField] private Vector2 _bounds = new(800, 600);

      
        public override void OnFocus()
        {
            base.OnFocus();
            _image.sprite = Selection.DisplayImage;
            Adjust();
        }
        [Button]
        private void Adjust()
        {
            if (_image.sprite)
            {
                // 获取 Sprite 的原始像素尺寸
                Vector2 spritePixelSize = new Vector2(_image.sprite.rect.width, _image.sprite.rect.height);
    
                // 获取该 Sprite 的 Pixels Per Unit
                float pixelsPerUnit = _image.sprite.pixelsPerUnit;
    
                // 计算在 Unity UI 中的实际“逻辑尺寸”（单位：RectTransform 的单位，即像素）
                Vector2 spriteSizeInUI = spritePixelSize / pixelsPerUnit;

                // 如果 Canvas 是 Screen Space，RectTransform 的尺寸单位 ≈ 屏幕像素
                // 所以可以直接用 _bounds（假设它是目标容器的可用尺寸，如 Panel 的 rect.size）

                float scale = Mathf.Min(_bounds.x / spriteSizeInUI.x, _bounds.y / spriteSizeInUI.y);
    
                // 设置 Image 的 sizeDelta 为缩放后的逻辑尺寸
                _image.rectTransform.sizeDelta = spriteSizeInUI * scale;
            }
            else
            {
                _image.rectTransform.sizeDelta = Vector2.zero;
            }
        }
    }
}