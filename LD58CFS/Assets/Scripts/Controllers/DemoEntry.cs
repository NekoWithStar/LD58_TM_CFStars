using UnityEngine;
using EggFramework.Modules.Launch;
using QFramework;

namespace LD58
{
    public sealed class DemoEntry : AbstractController
    {
        private void Awake()
        {
            var lfsm = new LaunchFSM(LD58App.Interface);
            lfsm.OnLaunchComplete(() => this.SendEvent(new ArchitectureInitFinishEvent()));
            lfsm.Start();
        }
    }
}