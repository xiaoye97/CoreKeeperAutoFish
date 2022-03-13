using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
using System.IO;

namespace CoreKeeperAutoFish
{
    internal class AutoFishPatch
    {
        private static bool autoFishControlInput = false;
        private static bool autoFishNeedPress = false;

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerState.Fishing), "UpdateFishOnTheHook")]
        public static bool AutoFishPatch1(PlayerState.Fishing __instance)
        {
            // 如果有鱼在钩子上，则开始判断
            if (__instance.fishOnTheHook)
            {
                // 自动钓起
                // 如果不在小游戏，则开始拉钩
                if (!__instance.isInFishingMiniGame)
                {
                    var mgr = AutoFish.Mgr;
                    var info = PugDatabase.GetObjectInfo(__instance.fishStruggleInfo.fishID);
                    // 获取翻译名字
                    string fishName = PugText.ProcessText($"Items/{info.objectID}", new UnhollowerBaseLib.Il2CppStringArray(new string[] { }), true, false);
                    string coolText = AutoFish.GetRandomFishSay(info.rarity, fishName);
                    //AutoFish.Log.LogInfo(fishName);
                    Vector3 textPos = mgr.player.RenderPosition + new Vector3(0, 2f, 0);
                    mgr._textManager.SpawnCoolText(coolText, textPos, mgr._textManager.GetRarityColor(info.rarity), TextManager.FontFace.button, 0.3f, 1, 3, 0.8f, 0.8f);
                    __instance.BeginPullUp();
                    autoFishControlInput = true;
                    autoFishNeedPress = false;
                    //AutoFish.Log.LogInfo("开始拉杆");
                }
                // 如果在小游戏，则根据鱼的状态进行拉钩
                else
                {
                    autoFishNeedPress = !__instance.fishIsStruggling;
                }
            }
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.Fishing), "OnExitState")]
        public static void AutoFishPatch2()
        {
            autoFishControlInput = false;
            //AutoFish.Log.LogInfo("钓鱼完毕");
        }


        [HarmonyPostfix, HarmonyPatch(typeof(PlayerInput), "IsButtonCurrentlyDown")]
        public static void AutoFishPatch3(PlayerInput __instance, PlayerInput.InputType inputType, ref bool __result)
        {
            // 如果检测的是使用物品按键，并且现在是钓鱼管控阶段，则根据钓鱼管控返回结果
            if (inputType == PlayerInput.InputType.SECOND_INTERACT)
            {
                if (autoFishControlInput)
                {
                    __result = autoFishNeedPress;
                }
            }
        }
    }
}
