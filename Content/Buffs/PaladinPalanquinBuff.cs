using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Buffs
{
    public class PaladinPalanquinBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<Mounts.PaladinPalanquinMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
