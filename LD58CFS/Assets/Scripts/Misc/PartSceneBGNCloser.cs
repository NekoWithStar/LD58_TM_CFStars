#region

//文件创建者：Egg
//创建时间：10-06 09:29

#endregion

using System;
using LD58.Model;
using QFramework;

namespace LD58.Misc
{
    public sealed class PartSceneBGNCloser : AbstractController
    {
        private ISettingModel _settingModel;

        private float _cachedValue;
        private void Awake()
        {
            _settingModel                 = this.GetModel<ISettingModel>();
            _cachedValue                  = _settingModel.BgmVolume.Value;
            _settingModel.BgmVolume.Value = 0;
        }

        private void OnDestroy()
        {
            _settingModel.BgmVolume.Value = _cachedValue;
        }
    }
}