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
            player.GetModPlayer<ChromaSpherePlayer>().trail = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual) {
                player.GetModPlayer<ChromaSpherePlayer>().trail = true;
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
        
        public bool trail;
        public Vector2[] trailOldPos = new Vector2[10];

        public override void ResetEffects()
        {
            active = false;
            trail = false;
            //trailOldPos = new Vector2[10];
        }

        public override void PreUpdate()
        {
            for (int i = 0; i < trailOldPos.Length - 1; i++) {
                trailOldPos[i] =  trailOldPos[i + 1];
            }
            trailOldPos[9] = Player.position;
        }
    }

    public class ChromaSphereAfterimage : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.LastVanillaLayer);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<ChromaSpherePlayer>().trail;
        
        public Player clone;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Vector2[] positions = drawInfo.drawPlayer.GetModPlayer<ChromaSpherePlayer>().trailOldPos;
            Player drawPlayer = drawInfo.drawPlayer;
            List<DrawData> existingDrawData = drawInfo.DrawDataCache;
            float milesPerHour = drawPlayer.velocity.Length() * 225f / 44f;
            float movementSpeedInterpolant = Utils.GetLerpValue(0f, 80, milesPerHour, true);
            movementSpeedInterpolant = (float)Math.Pow(movementSpeedInterpolant, 5D / 3D);
            for (float i = 0f; i < positions.Length; i += 1.7f) {
                float completionRatio = i / (float)positions.Length;
                float scale = MathHelper.Lerp(1f, 0.6f, completionRatio);
                float opacity = MathHelper.Lerp(0.12f, 0.03f, completionRatio) * movementSpeedInterpolant;
                List<DrawData> afterimages = new List<DrawData>();
                for (int j = 0; j < existingDrawData.Count; j++) {
                    var drawData = existingDrawData[j];
                    drawData.position = existingDrawData[j].position - drawPlayer.position + drawPlayer.oldPosition;
                    Color trailColor = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly - i * 5f + Main.GlobalTimeWrappedHourly * 48)) with { A = 170 };
                    drawData.color = trailColor * opacity;
                    drawData.scale = new Vector2(scale);
                    int colorOnlyShaderIndex = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;
                    drawData.shader = colorOnlyShaderIndex;
                    afterimages.Add(drawData);
                }
                drawInfo.DrawDataCache.InsertRange(0, afterimages);
            }
        }
    }
}
