using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityHunt.Common.Systems;

public class ShadowspecLocker : GlobalItem
{
    public static bool unlockItems;

    public static LocalizedText curseText;

    public override void Load()
    {
        curseText = Language.GetOrRegister(Mod.GetLocalizationKey("GoozmaShadowspecCurse"));
    }

    public override bool CanUseItem(Item item, Player player)
    {
        bool usable = !ShadowspecItemFinder.ShadowspecItem(item.type);

        //remove if needed elsewhere
        unlockItems = ModContent.GetInstance<BossDownedSystem>().GoozmaDowned || Config.Instance.shadowspecCurse;

        return unlockItems || usable;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!CanUseItem(item, Main.LocalPlayer) && ShadowspecItemFinder.ShadowspecItem(item.type))
            tooltips.Add(new TooltipLine(Mod, "GoozmaShadowspecCurse", curseText.Value));
    }

    //public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
    //{
    //    if (line.Name == "GoozmaShadowspecCurse")
    //    {
    //        //meh do some custom funny glowy line shit later
    //        return false;
    //    }

    //    return base.PreDrawTooltipLine(item, line, ref yOffset);
    //}

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (!CanUseItem(item, Main.LocalPlayer) && ShadowspecItemFinder.ShadowspecItem(item.type))
        {
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            Texture2D qmark = AssetDirectory.Textures.Extras.QuestionMark.Value;

            spriteBatch.Draw(glow, position, glow.Frame(), Color.Black * 0.5f, 0, glow.Size() * 0.5f, scale * 5f, 0, 0);

            spriteBatch.Draw(qmark, position, qmark.Frame(), Color.White, 0, qmark.Size() * 0.5f, scale * 3f, 0, 0);
            spriteBatch.Draw(qmark, position, qmark.Frame(), new Color(100, 90, 130, 0), 0, qmark.Size() * 0.5f, scale * 3.6f + MathF.Sin(Main.GlobalTimeWrappedHourly * 2.5f) * 0.1f, 0, 0);
        }
    }
}

public class ShadowspecItemFinder : ModSystem
{
    public static HashSet<int> shadowspecItemIDs;

    public static bool ShadowspecItem(int type) => shadowspecItemIDs.Contains(type);

    public override void PostAddRecipes()
    {
        shadowspecItemIDs = new HashSet<int>();

        if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
        {
            //Rarity method
            //Recipe method
            int shadowspecType = calamity.Find<ModItem>("ShadowspecBar").Type;
            foreach (Recipe recipe in Main.recipe.Where(n => n.HasIngredient(shadowspecType)))
                shadowspecItemIDs.Add(recipe.createItem.type);
        }
    }
}
