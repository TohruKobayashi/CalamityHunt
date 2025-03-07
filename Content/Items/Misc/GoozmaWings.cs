using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using CalamityHunt;
using CalamityHunt.Common.Players;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Armor.Shogun;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static log4net.Appender.ColoredConsoleAppender;

namespace CalamityHunt.Content.Items.Misc
{
    public class ChromaSphere : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 32;
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<ChromaSpherePlayer>().active = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual) {
                player.GetModPlayer<ChromaSpherePlayer>().active = true;
            }
        }
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<ChromaSpherePlayer>().active = true;
        }
    }

    public class ChromaSpherePlayer : ModPlayer
    {
        public bool active;
        public override void ResetEffects()
        {
            active = false;
        }

        public override void FrameEffects()
        {
            if (active) {
                Player.armorEffectDrawShadow = true;
                Player.armorEffectDrawOutlines = true;
            }
        }
    }

    public class ChromaSphereAfterimage : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.LastVanillaLayer);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<ChromaSpherePlayer>().active && drawInfo.shadow != 0;
        

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            List<DrawData> existingDrawData = drawInfo.DrawDataCache;
            float milesPerHour = drawPlayer.velocity.Length() * 5.11f; // 225f / 44f
            float movementSpeedInterpolant = Utils.GetLerpValue(0f, 80, milesPerHour, true);
            movementSpeedInterpolant = (float)Math.Pow(movementSpeedInterpolant, 1.67f); // 5 / 3
            int colorOnlyShaderIndex = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
            List<DrawData> drawcache = new List<DrawData>();
            float completionRatio = (float)drawInfo.shadow;
            float scale = MathHelper.Lerp(1f, 0.6f, completionRatio);
            float opacity = MathHelper.Lerp(0.48f, 0.12f, completionRatio) * movementSpeedInterpolant;
            Color trailColor = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly - drawInfo.shadow * 25f + Main.GlobalTimeWrappedHourly * 96)) with { A = 180 };
            // go through every player layer and apply effects
            for (int j = 0; j < existingDrawData.Count; j++) {
                var drawData = existingDrawData[j];
                Vector2 pos = existingDrawData[j].position - drawPlayer.position + drawPlayer.oldPosition;
                drawData.color = trailColor * opacity;
                drawData.shader = colorOnlyShaderIndex;
                drawInfo.DrawDataCache[j] = drawData;
            }
        }
    }

    public class ChromaSphereOutline : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.LastVanillaLayer);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<ChromaSpherePlayer>().active && drawInfo.shadow == 0;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            List<DrawData> existingDrawData = drawInfo.DrawDataCache;
            int colorOnlyShaderIndex = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            Color trailColor = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 48)) with { A = 170 };

            List<DrawData> outlines = new List<DrawData>();

            // go through every player layer and apply effects
            for (int j = 0; j < existingDrawData.Count; j++) {
                // create a glowy outline
                for (int k = 0; k < 4; k++) {
                    var outlineData = existingDrawData[j];
                    Vector2 off = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 4f * k + drawPlayer.fullRotation);
                    Vector2 newPosition = outlineData.position + off;
                    outlineData.shader = colorOnlyShaderIndex;
                    outlineData.color = trailColor;
                    outlineData.position += off;
                    outlines.Add(outlineData);
                }
            }
            drawInfo.DrawDataCache.InsertRange(0, outlines);
        }        
    }
}
