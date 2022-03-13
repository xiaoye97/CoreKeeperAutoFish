using System;
using BepInEx;
using UnityEngine;
using BepInEx.IL2CPP;
using BepInEx.Logging;

namespace CoreKeeperAutoFish
{
    [BepInPlugin(GUID, NAME, VERSION)]
    internal class AutoFishPlugin : BasePlugin
    {
        public const string GUID = "me.xiaoye97.plugin.CoreKeeperAutoFish";
        public const string NAME = "CoreKeeperAutoFish";
        public const string AUTHOR = "xiaoye97";
        public const string VERSION = "1.0.0";

        public static AutoFishPlugin Inst { get; private set; }
        public static ManualLogSource LogSource => Inst.Log;

        public override void Load()
        {
            Inst = this;
            AutoFish.Init();
        }
    }
}
