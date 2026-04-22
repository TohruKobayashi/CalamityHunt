using CalamityHunt.Common.Players;
using CalamityHunt.Content.Items.Misc.AuricSouls;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Common.GlobalItems;

/// <summary>
/// Puke out Auric Souls when eating Calamity's Disgusting Meat
/// </summary>
public class DisgustingMeatGlobalItem : GlobalItem
{
    public static int disgustingMeatID = 0;

    public override bool IsLoadingEnabled(Mod mod)
    {
        return ModCompatibility.Calamity.IsLoaded;
    }

    public override void SetStaticDefaults()
    {
        disgustingMeatID = ModCompatibility.Calamity.Mod.Find<ModItem>("DisgustingMeat").Type;
    }

    public override bool AppliesToEntity(Item item, bool lateInstantiation)
    {
        return item.type == disgustingMeatID;
    }

    public override bool? UseItem(Item item, Player player)
    {
        if (item.type == disgustingMeatID) {
            if (player.ItemTimeIsZero) {
                var soulPlayer = player.GetModPlayer<AuricSoulPlayer>();
                if (player.altFunctionUse == 2) {
                    TryDropBoosterItem(player, ref soulPlayer.yharonSoul, ModContent.ItemType<YharonSoul>());
                    TryDropBoosterItem(player, ref soulPlayer.goozmaSoul, ModContent.ItemType<GoozmaSoul>());
                    TryDropBoosterItem(player, ref soulPlayer.olddukeSoul, ModContent.ItemType<RottenSoul>());
                    TryDropBoosterItem(player, ref soulPlayer.pyrogenSoul, ModContent.ItemType<AshenSoul>());
                }
                return true;
            }
        }
        return null;
    }

    private static void TryDropBoosterItem(Player player, ref bool condition, int itemType)
    {
        if (condition) {
            condition = false;
            int drop = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, itemType);
            Main.item[drop].noGrabDelay = 100;
        }
    }
}
