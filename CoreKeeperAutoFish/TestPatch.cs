using System;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace CoreKeeperAutoFish
{
    public static class TestPatch
    {
        public static void Init()
        {
            Harmony.CreateAndPatchAll(typeof(TestPatch));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.PlayerBaseState), "UpdateFishOnTheHook")]
        public static void PlayerBaseState_UpdateFishOnTheHook_Patch(PlayerState.PlayerBaseState __instance)
        {
            //Debug.Log("XY PlayerBaseState.UpdateFishOnTheHook");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.Fishing), "OnEnterState")]
        public static void Fishing_OnEnterState_Patch()
        {
            Debug.Log("XY Fishing.OnEnterState");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.PlayerBaseState), "OnExitState")]
        public static void PlayerBaseState_OnExitState_Patch(PlayerState.PlayerBaseState __instance)
        {
            Debug.Log("XY PlayerBaseState.OnExitState");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.PlayerBaseState), "PullUp")]
        public static void PlayerBaseState_PullUp_Patch(PlayerState.PlayerBaseState __instance, bool failedThrow)
        {
            Debug.Log($"XY PlayerBaseState.PullUp {failedThrow}");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.PlayerBaseState), "StartFishing")]
        public static void PlayerBaseState_StartFishing_Patch(PlayerState.PlayerBaseState __instance)
        {
            Debug.Log("XY PlayerBaseState.StartFishing");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerState.PlayerBaseState), "ExitFishing")]
        public static void PlayerBaseState_ExitFishing_Patch(PlayerState.PlayerBaseState __instance, bool wasExitingState)
        {
            Debug.Log($"XY PlayerBaseState.ExitFishing {wasExitingState}");
        }
    }
}
