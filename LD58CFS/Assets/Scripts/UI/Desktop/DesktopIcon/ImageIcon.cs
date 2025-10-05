#region

//文件创建者：Egg
//创建时间：10-05 01:23

#endregion

using UnityEngine;

namespace LD58.UI.Desktop
{
    public sealed class ImageIcon : DesktopIcon
    {
        [SerializeField] private Sprite _image;
        protected override void BeforeOpen()
        {
            base.BeforeOpen();
            Selection.DisplayImage = _image;
        }
    }
}