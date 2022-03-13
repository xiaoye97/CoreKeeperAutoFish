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
    public static class AutoFish
    {
        public static ManualLogSource Log => AutoFishPlugin.LogSource;
        public static ConfigEntry<bool> SpawnCoolTextOnFishing;
        public static ConfigEntry<bool> EnableAutoFish;

        public static ConfigEntry<string> RarityPoorTexts;
        public static ConfigEntry<string> RarityCommonTexts;
        public static ConfigEntry<string> RarityUncommonTexts;
        public static ConfigEntry<string> RarityRareTexts;
        public static ConfigEntry<string> RarityEpicTexts;
        public static ConfigEntry<string> RarityLegendaryTexts;

        private static Dictionary<Rarity, List<string>> RarityTextDict;


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
            Harmony.CreateAndPatchAll(typeof(AutoFishPatch));
        }

        public static void InitConfig()
        {
            SpawnCoolTextOnFishing = AutoFishPlugin.Inst.Config.Bind("config", "SpawnCoolTextOnFishing", true, "是否喊出鱼的种类");
            EnableAutoFish = AutoFishPlugin.Inst.Config.Bind("config", "EnableAutoFish", true, "是否启用自动钓鱼");

            RarityPoorTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityPoorTexts", "呃，{0}你在逗我吗", "遇到垃圾的鱼时说的话");
            RarityCommonTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityCommonTexts", "一条{0}|{0}，普普通通|原来是{0}|呃，{0}|随处可见的{0}|希望下次不是你，{0}", "遇到寻常的鱼时说的话");
            RarityUncommonTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityUncommonTexts", "咦，{0}|吼吼，是{0}|还行，一条{0}|运气不错，是一条{0}", "遇到不寻常的鱼时说的话");
            RarityRareTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityRareTexts", "好耶!是{0}|居然是{0}!|走大运了，是{0}!|我爱死你了，{0}", "遇到稀有的鱼时说的话");
            RarityEpicTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityEpicTexts", "卧槽!!!是{0}!!!|史诗!{0}!|我就是今天最靓的钓鱼佬，居然是{0}！", "遇到史诗的鱼时说的话");
            RarityLegendaryTexts = AutoFishPlugin.Inst.Config.Bind("say", "RarityLegendaryTexts", "传说级{0}!!!|是真正的鱼之王者！{0}!", "遇到传说的鱼时说的话");
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
    }
}
