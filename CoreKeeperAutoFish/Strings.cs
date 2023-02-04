using CoreLib.Submodules.Localization;
using Il2CppSystem;
using BepInEx.Logging;

namespace CoreKeeperAutoFish
{
    public static class Strings
    {
        private static readonly Random Random = new();
        private static ManualLogSource _log;

        public static void Init(ManualLogSource log)
        {
            _log = log;
            LocalizationModule.AddTerm("AutoFish/Say/RarityPoor[0]", "Ugh, {0}", "额，{0}你在逗我吗");
            LocalizationModule.AddTerm("AutoFish/Say/RarityPoor[1]", "I hate {0}", "额，{0}你在逗我吗");
            LocalizationModule.AddTerm("AutoFish/Say/RarityPoor[2]", "How boring - a {0}", "额，{0}你在逗我吗");

            LocalizationModule.AddTerm("AutoFish/Say/RarityCommon[0]", "A {0}", "一条{0}");
            LocalizationModule.AddTerm("AutoFish/Say/RarityCommon[1]", "Ordinary {0}", "{0}，普普通通");
            LocalizationModule.AddTerm("AutoFish/Say/RarityCommon[2]", "It was a {0}", "原来是{0}");
            LocalizationModule.AddTerm("AutoFish/Say/RarityCommon[3]", "How plain - a {0}", "额，{0}");
            LocalizationModule.AddTerm("AutoFish/Say/RarityCommon[4]", "A {0}", "随处可见的{0}");
            LocalizationModule.AddTerm("AutoFish/Say/RarityCommon[5]", "A {0}", "希望下次不是你，{0}");

            LocalizationModule.AddTerm("AutoFish/Say/RarityUncommon[0]", "Oh, a {0}", "咦，{0}");
            LocalizationModule.AddTerm("AutoFish/Say/RarityUncommon[1]", "One {0}", "吼吼，是{0}");
            LocalizationModule.AddTerm("AutoFish/Say/RarityUncommon[2]", "Hey, a {0}", "还行，一条{0}");
            LocalizationModule.AddTerm("AutoFish/Say/RarityUncommon[3]", "{0} makes a nice meal.", "运气不错，是一条{0}");

            LocalizationModule.AddTerm("AutoFish/Say/RarityRare[0]", "Yay, it's {0}!", "好耶!是{0}|居然是{0}!");
            LocalizationModule.AddTerm("AutoFish/Say/RarityRare[1]", "It's {0}!", "居然是{0}!|走大运了，是{0}!");
            LocalizationModule.AddTerm("AutoFish/Say/RarityRare[2]", "Lucky me, it's a {0}!", "我爱死你了，{0}");

            LocalizationModule.AddTerm("AutoFish/Say/RarityEpic[0]", "It's {0}!", "卧槽!!!是{0}!!!");
            LocalizationModule.AddTerm("AutoFish/Say/RarityEpic[1]", "Epic! {0}!", "史诗!{0}!");
            LocalizationModule.AddTerm("AutoFish/Say/RarityEpic[2]", "That's a huge {0}!", "太帅了，居然是{0}!");

            LocalizationModule.AddTerm("AutoFish/Say/RarityLegendary[0]", "A Legendary {0}!", "传说级{0}!!!");
            LocalizationModule.AddTerm("AutoFish/Say/RarityLegendary[1]", "Wow - a {0}!!!", "是真正的鱼之王者！{0}!");

            LocalizationModule.AddTerm("AutoFish/Say/AutoThrow[0]", "Come on...", "就决定是你了");
            LocalizationModule.AddTerm("AutoFish/Say/AutoThrow[1]", "Hey", "Hey");
            LocalizationModule.AddTerm("AutoFish/Say/AutoThrow[2]", "Fishing is so relaxing", "看我这招如何");
            LocalizationModule.AddTerm("AutoFish/Say/AutoThrow[3]", "Hoping for a big catch", "吃我一竿");
            LocalizationModule.AddTerm("AutoFish/Say/AutoThrow[4]", "", "大的要来了");
            LocalizationModule.AddTerm("AutoFish/Say/AutoThrow[5]", "", "这钓鱼多是一件美事");
            LocalizationModule.AddTerm("AutoFish/Say/AutoThrow[6]", "", "冲呀");
        }

        public static string GetRandomPhrase(string prefix)
        {
            string phrase;
            int max = 8;
            do
            {
                int i = Random.Next(max);
                max = i - 1;
                string key = $"AutoFish/Say/{prefix}[{i}]";
                phrase = PugText.ProcessText(key, null, true, false);
            } while (phrase == "<missing>" && max >= 0);

            if (phrase == "<missing>")
            {
                phrase = String.Empty;
                _log.LogWarning($"Did not find any phrases for {prefix}");
            }

            return phrase;
        }
    }
}