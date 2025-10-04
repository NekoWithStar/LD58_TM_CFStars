using EggFramework;
using EggFramework.ObjectPool;
using EggFramework.AudioSystem;
using EggFramework.TimeSystem;
using EggFramework.Storage;
using LD58.Model;
using QFramework;

namespace LD58
{
    public sealed class LD58App : Architecture<LD58App>
    {
        protected override void Init()
        {
            RegisterModel<ISettingModel>(new SettingModel());
            
            RegisterSystem<IAudioSystem>(new AudioSystem());
            RegisterSystem<IFileSystem>(new FileSystem());
       
            RegisterUtility<IStorage>(new JsonStorage());
        }
    }
}