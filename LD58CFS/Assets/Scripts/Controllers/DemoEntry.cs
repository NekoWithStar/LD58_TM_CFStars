using EggFramework.AudioSystem;
using UnityEngine;
using EggFramework.Modules.Launch;
using LD58.Model;
using QFramework;

namespace LD58
{
    public sealed class DemoEntry : AbstractController
    {
        private static bool _inited;

        private void Awake()
        {
            if (_inited) return;
            _inited = true;
            var lfsm = new LaunchFSM(LD58App.Interface);
            lfsm.OnLaunchComplete(() => this.SendEvent(new ArchitectureInitFinishEvent()));
            lfsm.Start();

            this.GetSystem<IAudioSystem>().BindBGMVolume(this.GetModel<ISettingModel>().BgmVolume);
            this.GetSystem<IAudioSystem>().BindSFXVolume(this.GetModel<ISettingModel>().SfxVolume);
        }
    }
}