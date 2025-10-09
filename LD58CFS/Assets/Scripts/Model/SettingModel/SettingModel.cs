#region

//文件创建者：Egg
//创建时间：10-01 08:21

#endregion

using EggFramework;
using EggFramework.Util;
using QFramework;
using UnityEngine;

namespace LD58.Model
{
    public sealed class SettingModel : AbstractModel, ISettingModel
    {
        protected override void OnInit()
        {
            BgmVolume.Value = StorageUtil.LoadByJson(nameof(BgmVolume), 1f);
            SfxVolume.Value = StorageUtil.LoadByJson(nameof(SfxVolume), 1f);
            Language.Value  = StorageUtil.LoadByJson(nameof(Language),  SystemLanguage.English);

            BgmVolume.Register(val => StorageUtil.SaveByJson(nameof(BgmVolume), val));
            SfxVolume.Register(val => StorageUtil.SaveByJson(nameof(SfxVolume), val));
            Language.Register(val => StorageUtil.SaveByJson(nameof(Language),   val));
        }

        public BsProperty<SystemLanguage> Language  { get; } = new(SystemLanguage.English);
        public BsProperty<float>          BgmVolume { get; } = new(1f);
        public BsProperty<float>          SfxVolume { get; } = new(1f);
    }
}