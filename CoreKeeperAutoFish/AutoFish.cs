using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.Collections.Generic;

namespace CoreKeeperAutoFish
{
    public static class AutoFish
    {
        public static ManualLogSource Log => AutoFishPlugin.LogSource;
        public static ConfigEntry<bool> SpawnCoolTextOnFishing;
        public static ConfigEntry<bool> EnableAutoFish;
        public static ConfigEntry<bool> EnableAutoThrow;

        public static ConfigEntry<string> RarityPoorTexts;
        public static ConfigEntry<string> RarityCommonTexts;
        public static ConfigEntry<string> RarityUncommonTexts;
        public static ConfigEntry<string> RarityRareTexts;
        public static ConfigEntry<string> RarityEpicTexts;
        public static ConfigEntry<string> RarityLegendaryTexts;
        public static ConfigEntry<string> AutoThrowTexts;

        private static Dictionary<Rarity, List<string>> RarityTextDict;
        private static List<string> AutoThrowTextList;


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
            InitRarityText();
            InitAutoThrowText();
            Harmony.CreateAndPatchAll(typeof(AutoFishPatch));
        }

        public static void InitConfig()
        {
            SpawnCoolTextOnFishing = AutoFishPlugin.Inst.Config.Bind("config", "SpawnCoolTextOnFishing", true, "是否喊出鱼的种类");
            EnableAutoFish = AutoFishPlugin.Inst.Config.Bind("config", "EnableAutoFish", true, "是否启用自动钓鱼");
            EnableAutoThrow = AutoFishPlugin.Inst.Config.Bind("config", "EnableAutoThrow", true, "是否启用自动抛竿");
            RarityPoorTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityPoorTexts", "额，{0}你在逗我吗", "遇到垃圾的鱼时说的话");
            RarityCommonTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityCommonTexts", "一条{0}|{0}，普普通通|原来是{0}|额，{0}|随处可见的{0}|希望下次不是你，{0}", "遇到寻常的鱼时说的话");
            RarityUncommonTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityUncommonTexts", "咦，{0}|吼吼，是{0}|还行，一条{0}|运气不错，是一条{0}", "遇到不寻常的鱼时说的话");
            RarityRareTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityRareTexts", "好耶!是{0}|居然是{0}!|走大运了，是{0}!|我爱死你了，{0}", "遇到稀有的鱼时说的话");
            RarityEpicTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityEpicTexts", "卧槽!!!是{0}!!!|史诗!{0}!|太帅了，居然是{0}！", "遇到史诗的鱼时说的话");
            RarityLegendaryTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityLegendaryTexts", "传说级{0}!!!|是真正的鱼之王者！{0}!", "遇到传说的鱼时说的话");
            AutoThrowTexts = AutoFishPlugin.Inst.Config.Bind("say", "AutoThrowTexts", "就决定是你了|大的要来了|看我这招如何|吃我一竿|这钓鱼多是一件美事|Hey|冲呀", "自动抛竿时说的话");
        }

        /// <summary>
        /// 初始化稀有度文本
        /// </summary>
        private static void InitRarityText()
        {
            RarityTextDict = new Dictionary<Rarity, List<string>>();
            InitRarityTextByRarity(Rarity.Poor, RarityPoorTexts.Value);
            InitRarityTextByRarity(Rarity.Common, RarityCommonTexts.Value);
            InitRarityTextByRarity(Rarity.Uncommon, RarityUncommonTexts.Value);
            InitRarityTextByRarity(Rarity.Rare, RarityRareTexts.Value);
            InitRarityTextByRarity(Rarity.Epic, RarityEpicTexts.Value);
            InitRarityTextByRarity(Rarity.Legendary, RarityLegendaryTexts.Value);
        }

        private static void InitRarityTextByRarity(Rarity rarity, string str)
        {
            RarityTextDict.Add(rarity, new List<string>());
            var texts = str.Split('|');
            foreach (var text in texts)
            {
                RarityTextDict[rarity].Add(text);
            }
        }

        /// <summary>
        /// 初始化自动抛竿文本
        /// </summary>
        private static void InitAutoThrowText()
        {
            AutoThrowTextList = new List<string>();
            var texts = AutoThrowTexts.Value.Split('|');
            foreach (string text in texts)
            {
                AutoThrowTextList.Add(text);
            }
        }

        /// <summary>
        /// 获取随机的钓鱼喊话文本
        /// </summary>
        /// <param name="rarity">鱼的品质</param>
        /// <param name="fishName">鱼的名字</param>
        /// <returns></returns>
        public static string GetRandomFishSay(Rarity rarity, string fishName)
        {
            if (RarityTextDict[rarity].Count > 0)
            {
                int r = UnityEngine.Random.Range(0, RarityTextDict[rarity].Count);
                string format = RarityTextDict[rarity][r];
                return string.Format(format, fishName);
            }
            else
            {
                return fishName;
            }
        }

        /// <summary>
        /// 获取随机的抛竿文本
        /// </summary>
        /// <returns></returns>
        public static string GetRandomAutoThrowSay()
        {
            if (AutoThrowTextList.Count > 0)
            {
                int r = UnityEngine.Random.Range(0, AutoThrowTextList.Count);
                return AutoThrowTextList[r];
            }
            else
            {
                return "";
            }
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
