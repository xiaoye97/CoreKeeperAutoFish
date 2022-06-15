using System;
using HarmonyLib;
using UnityEngine;

namespace CoreKeeperAutoFish
{
    internal class AutoFishPatch
    {
        private static bool autoFishControlInput, autoFishNeedPress, autoThrowPullUp, waitPressedThrow, isInFishing, canPullUp;

        [HarmonyPrefix, HarmonyPatch(typeof(PlayerState.Fishing), "UpdateFishOnTheHook")]
        public static bool AutoFish_Fishing_UpdateFishOnTheHook_Patch(PlayerState.Fishing __instance)
        {
            // 如果有鱼在钩子上，则开始判断
            if (__instance.fishOnTheHook)
            {
                // 自动钓起
                // 如果不在小游戏，则开始拉钩
                if (!__instance.isInFishingMiniGame)
                {
                    if (AutoFish.SpawnCoolTextOnFishing.Value)
                    {
                        var info = PugDatabase.GetObjectInfo(__instance.fishStruggleInfo.fishID);
                        // 获取翻译名字
                        string fishName = PugText.ProcessText($"Items/{info.objectID}", new UnhollowerBaseLib.Il2CppStringArray(new string[] { }), true, false);
                        string coolText = AutoFish.GetRandomFishSay(info.rarity, fishName);
                        // 喊出鱼的名字
                        AutoFish.Say(coolText, info.rarity);
                    }
                    if (AutoFish.EnableAutoFish.Value)
                    {
                        // 发现鱼，拉杆
                        __instance.BeginPullUp();
                        autoFishControlInput = true;
                        autoFishNeedPress = false;
                    }
                }
                // 如果在小游戏，则根据鱼的状态进行拉钩
                else
                {
                    // 根据鱼是否挣扎决定是否按住按键
                    autoFishNeedPress = !__instance.fishIsStruggling;
                }
            }
            else
            {
                if (AutoFish.EnableFishItem.Value)
                {
                    if (canPullUp)
                    {
                        __instance.BeginPullUp();
                    }
                }
            }
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AudioSource), "Play", new Type[] { })]
        public static void AudioSource_Patch(AudioSource __instance)
        {
            if (isInFishing)
            {
                if (AutoFish.EnableAutoFish.Value)
                {
                    // 如果播放了冒泡音效，并且位置在玩家位置，说明可以拉杆了
                    if (__instance.clip.name == "bubble")
                    {
                        float distance = Vector3.Distance(__instance.transform.position, AutoFish.Mgr.player.transform.position);
                        //string log = $"检测到冒泡音效，音效位置:{__instance.transform.position} 玩家位置:{AutoFish.Mgr.player.transform.position} 距离:{distance}";
                        //AutoFish.Log.LogInfo(log);
                        if (distance < 5f)
                        {
                            canPullUp = true;
                        }
                    }
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.Fishing), "OnEnterState")]
        public static void AutoFish_Fishing_OnEnterState_Patch()
        {
            isInFishing = true;
            canPullUp = false;
            autoThrowPullUp = false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.Fishing), "OnExitState")]
        public static void AutoFish_Fishing_OnExitState_Patch()
        {
            isInFishing = false;
            canPullUp = false;
            autoFishControlInput = false;
            // 如果退出钓鱼状态时此值为true，说明需要进行自动抛竿
            if (autoThrowPullUp)
            {
                // 喊出抛竿语
                AutoFish.Say(AutoFish.GetRandomAutoThrowSay(), Color.white);
                waitPressedThrow = true;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.Fishing), "PullUp")]
        public static void AutoFish_Fishing_PullUp_Patch(bool failedThrow)
        {
            if (AutoFish.EnableAutoThrow.Value)
            {
                if (!failedThrow)
                {
                    // 如果触发了这里，说明钓鱼成功，才可以继续抛竿，如果没触发这里就触发了OnExitState，说明中断
                    autoThrowPullUp = true;
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerInput), "IsButtonCurrentlyDown")]
        public static void AutoFish_PlayerInput_IsButtonCurrentlyDown_Patch(PlayerInput __instance, PlayerInput.InputType inputType, ref bool __result)
        {
            if (AutoFish.EnableAutoFish.Value)
            {
                // 如果检测的是SECOND_INTERACT按键，并且现在是钓鱼管控阶段，则根据钓鱼管控返回结果
                if (inputType == PlayerInput.InputType.SECOND_INTERACT)
                {
                    if (autoFishControlInput)
                    {
                        __result = autoFishNeedPress;
                    }
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerInput), "WasButtonPressedDownThisFrame")]
        public static void AutoFish_PlayerInput_WasButtonPressedDownThisFrame_Patch(PlayerInput __instance, PlayerInput.InputType inputType, ref bool __result)
        {
            if (AutoFish.EnableAutoThrow.Value)
            {
                if (waitPressedThrow)
                {
                    if (inputType == PlayerInput.InputType.SECOND_INTERACT)
                    {
                        __result = true;
                        waitPressedThrow = false;
                    }
                }
            }
        }
    }
}
