using System;
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
    [AutoloadEquip(EquipType.Wings)]
    public class GoozmaWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.wingSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Wings);
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(10, -1, 0.2f);
        }

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

        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse) {
                Dust d = Dust.NewDustDirect(player.Center - new Vector2(30), 60, 60, DustID.Sand, 0, 0, 100, Color.Black, 1f);
                d.noGravity = true;
                d.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }

            return false;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 1f;
            maxAscentMultiplier = 4f;
            constantAscend = 0.2f;
            ascentWhenRising = 0.2f;
            maxCanAscendMultiplier = 1.4f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 12f;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<GoozmaWingsPlayer>().trail = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual) {
                player.GetModPlayer<GoozmaWingsPlayer>().trail = true;
            }
        }
        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<GoozmaWingsPlayer>().active = true;
        }
    }

    public class GoozmaWingsPlayer : ModPlayer
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

    public class GoozmaWingsAfterimage : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.JimsCloak);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.GetModPlayer<GoozmaWingsPlayer>().trail;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            int trailLength = drawInfo.drawPlayer.GetModPlayer<GoozmaWingsPlayer>().trailOldPos.Length;
            Vector2[] trailOldPos = drawInfo.drawPlayer.GetModPlayer<GoozmaWingsPlayer>().trailOldPos;

            // make homonculus child. he will be a perfect replica of the player, for drawing
            Player afterimage = new Player();
            afterimage.CopyVisuals(drawInfo.drawPlayer);
            //afterimage.isFirstFractalAfterImage = true;
            //afterimage.firstFractalAfterImageOpacity = 0.1f;

            // make all frames equal the origina lguy
            afterimage.beetleFrame = drawInfo.drawPlayer.beetleFrame;
            afterimage.bodyFrame = drawInfo.drawPlayer.bodyFrame;
            afterimage.carpetFrame = drawInfo.drawPlayer.carpetFrame;
            afterimage.flameRingFrame = drawInfo.drawPlayer.flameRingFrame;
            afterimage.hairFrame = drawInfo.drawPlayer.hairFrame;
            afterimage.headFrame = drawInfo.drawPlayer.headFrame;
            afterimage.iceBarrierFrame = drawInfo.drawPlayer.iceBarrierFrame;
            afterimage.legFrame = drawInfo.drawPlayer.legFrame;
            afterimage.pulleyFrame = drawInfo.drawPlayer.pulleyFrame;
            afterimage.rocketFrame = drawInfo.drawPlayer.rocketFrame;
            afterimage.wingFrame = drawInfo.drawPlayer.wingFrame;

            // make it all accurate
            afterimage.ResetEffects();
            afterimage.ResetVisibleAccessories();
            afterimage.DisplayDollUpdate();
            afterimage.UpdateSocialShadow();
            afterimage.UpdateDyes();
            afterimage.PlayerFrame();

            for (int ii = 0; ii < afterimage.dye.Length - 1; ii++) {
                afterimage.dye[ii].SetDefaults(ItemID.NebulaDye);
            }

            for (int i = 0; i < trailLength - 1; i++) {
                Vector2 oldPos = drawInfo.drawPlayer.GetModPlayer<GoozmaWingsPlayer>().trailOldPos[i] + drawInfo.drawPlayer.Size * 0.5f;
                Color trailColor = (new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly - i * 5f)) with { A = 170 };
                trailColor = Color.Lerp(trailColor, Color.Transparent, MathF.Pow((float)i / trailLength, 0.3f)) * 0.5f;

                for (int ii = 0; ii < afterimage.dye.Length; ii++) {
                    afterimage.dye[ii].SetDefaults(ItemID.NebulaDye);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
                Main.pixelShader.CurrentTechnique.Passes["ColorOnly"].Apply();
                Main.PlayerRenderer.DrawPlayer(Main.Camera, afterimage, trailOldPos[i], 0f, afterimage.fullRotationOrigin, 0f, 1f);
                Main.spriteBatch.End();

                //spriteBatch.Draw(goozmaTexture, oldPos + drawOffset - screenPos, drawInfo.drawPlayer.Frame(), trailColor, NPC.oldRot[i], goozmaTexture.Size() * 0.5f, NPC.scale, direction, 0);
            }
        }
    }

}
