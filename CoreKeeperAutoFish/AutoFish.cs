using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.Collections.Generic;
using Il2CppSystem;

namespace CoreKeeperAutoFish
{
    public static class AutoFish
    {
        public static ManualLogSource Log => AutoFishPlugin.LogSource;
        public static ConfigEntry<bool> SpawnCoolTextOnFishing;
        public static ConfigEntry<bool> EnableAutoFish;
        public static ConfigEntry<bool> EnableAutoThrow;
        public static ConfigEntry<bool> EnableFishItem;

        private static Manager mgr;
        public static Manager Mgr
        {
            get
            {
                if (mgr == null)
                {
                    mgr = GameObject.FindObjectOfType<Manager>();
                }
                return mgr;
            }
        }

        public static void Init()
        {
            Log.LogInfo("初始化自动钓鱼");
            InitConfig();
            Harmony.CreateAndPatchAll(typeof(AutoFishPatch));
        }

        public static void InitConfig()
        {
            SpawnCoolTextOnFishing = AutoFishPlugin.Inst.Config.Bind("config", "SpawnCoolTextOnFishing", true, "是否喊出鱼的种类\nSpeak while fishing?");
            EnableAutoFish = AutoFishPlugin.Inst.Config.Bind("config", "EnableAutoFish", true, "是否启用自动钓鱼\nEnable AutoFish?");
            EnableAutoThrow = AutoFishPlugin.Inst.Config.Bind("config", "EnableAutoThrow", true, "是否启用自动抛竿\nEnable Auto-Cast?");
            EnableFishItem = AutoFishPlugin.Inst.Config.Bind("config", "EnableFishItem", true, "是否启用钓物品\nEnable catching items?");
        }

        /// <summary>
        /// 获取随机的钓鱼喊话文本
        /// </summary>
        /// <param name="rarity">鱼的品质</param>
        /// <param name="fishName">鱼的名字</param>
        /// <returns></returns>
        public static string GetRandomFishSay(Rarity rarity, string fishName)
        {
            string format = Strings.GetRandomPhrase($"Rarity{rarity}");
            return String.IsNullOrEmpty(format)
                ? fishName
                : string.Format(format, fishName);
        }

        /// <summary>
        /// 获取随机的抛竿文本
        /// </summary>
        /// <returns></returns>
        public static string GetRandomAutoThrowSay()
        {
            string phrase = Strings.GetRandomPhrase("AutoThrow");
            return phrase;
        }

        /// <summary>
        /// 说话
        /// </summary>
        public static void Say(string text, Color color)
        {
            Vector3 textPos = Mgr.player.RenderPosition + new Vector3(0, 2f, 0);
            Mgr._textManager.SpawnCoolText(text, textPos, color, TextManager.FontFace.button, 0.3f, 1, 3, 0.8f, 0.8f);
        }

        /// <summary>
        /// 说话，但是根据稀有度适应颜色
        /// </summary>
        public static void Say(string text, Rarity colorRarity)
        {
            Color color = Mgr._textManager.GetRarityColor(colorRarity);
            Say(text, color);
        }
    }
}