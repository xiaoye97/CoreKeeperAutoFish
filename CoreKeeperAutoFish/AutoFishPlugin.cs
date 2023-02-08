using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using CoreLib;
using CoreLib.Submodules.Localization;

namespace CoreKeeperAutoFish
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [CoreLibSubmoduleDependency(nameof(LocalizationModule))]
    internal class AutoFishPlugin : BasePlugin
    {
        public const string GUID = "me.xiaoye97.plugin.CoreKeeperAutoFish";
        public const string NAME = "CoreKeeperAutoFish";
        public const string AUTHOR = "xiaoye97";
        public const string VERSION = "1.8.0";

        public static AutoFishPlugin Inst { get; private set; }
        public static ManualLogSource LogSource => Inst.Log;

        public override void Load()
        {
            Inst = this;
            Strings.Init(Log);
            AutoFish.Init();
            //TestPatch.Init();
        }
    }
}
