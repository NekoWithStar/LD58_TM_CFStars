#region

//文件创建者：Egg
//创建时间：10-01 08:20

#endregion

using EggFramework;
using QFramework;
using UnityEngine;

namespace LD58.Model
{
    public interface ISettingModel : IModel
    {
        BsProperty<SystemLanguage> Language  { get; }
        BsProperty<float>          BgmVolume { get; }
        BsProperty<float>          SfxVolume { get; }
    }
}