using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems
{
    public class ConditionalValue : ModSystem
    {
        public static bool ExpertMode = false;
        public static bool MasterMode = false;
        public static bool RevengeanceMode = false;
        public static bool DeathMode = false;
        public static bool BossRush = false;

        public override void PreUpdateNPCs()
        {
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                Mod Calamity = ModLoader.GetMod(HUtils.CalamityMod);
                BossRush = ((bool)Calamity.Call("GetDifficultyActive", "bossrush")) ? true : false;
                DeathMode = ((bool)Calamity.Call("GetDifficultyActive", "death") || (bool)Calamity.Call("GetDifficultyActive", "bossrush")) ? true : false;
                RevengeanceMode = ((bool)Calamity.Call("GetDifficultyActive", "revengeance") || (bool)Calamity.Call("GetDifficultyActive", "bossrush")) ? true : false;
                ExpertMode = (Main.expertMode || (bool)Calamity.Call("GetDifficultyActive", "bossrush")) ? true : false;
                MasterMode = (Main.masterMode || (bool)Calamity.Call("GetDifficultyActive", "bossrush")) ? true : false;
            }
            else {
                RevengeanceMode = Main.expertMode;
                ExpertMode = Main.expertMode;
            }
        }

        public static int BalanceToggleValue(int vanilla, int? nonVanilla) => (int)BalanceToggleValue(vanilla, nonVanilla);

        public static float DifficultyBasedValue(float? normal = null, float? expert = null, float? revengeance = null, float? death = null, float? ftw = null, float? gfb = null)
        {
            if (Main.zenithWorld && gfb != null) {
                return (float)gfb;
            }
            else if (Main.getGoodWorld && ftw != null) {
                return (float)ftw;
            }
            else if (DeathMode && death != null) {
                return (float)death;
            }
            else if (RevengeanceMode && revengeance != null) {
                return (float)revengeance;
            }
            else if (ExpertMode && expert != null) {
                return (float)expert;
            }
            else {
                return (float)normal;
            }
        }
    }
}
