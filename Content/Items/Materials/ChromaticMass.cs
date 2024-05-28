using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Materials
{
    public class ChromaticMass : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;

            ItemID.Sets.ItemNoGravity[Type] = true;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 30);
            Item.rare = ModContent.RarityType<VioletRarity>();
            Item.maxStack = Item.CommonMaxStack;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override void PostUpdate()
        {
            if (Main.rand.NextBool(5)) {
                Dust dark = Dust.NewDustPerfect(Item.Center + Main.rand.NextVector2Circular(18, 18), DustID.TintableDust, -Vector2.UnitY.RotatedByRandom(0.1f) * Main.rand.NextFloat(3f), 150, Color.Black, 1f + Main.rand.NextFloat());
                dark.noGravity = true;
            }

            if (Main.rand.NextBool(20)) {
                CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                    particle.position = Item.Center + Main.rand.NextVector2Circular(15, 15);
                    particle.velocity = -Vector2.UnitY * Main.rand.NextFloat(2f);
                    particle.scale = Main.rand.NextFloat(0.7f, 1.7f);
                    particle.color = Color.White;
                    particle.colorData = new ColorOffsetData(true, Main.GlobalTimeWrappedHourly * 40f);
                }));
            }
        }

        public static Asset<Texture2D> glowTexture;
        public static Asset<Texture2D> auraTexture;

        public override void Load()
        {
            glowTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Glow");
            auraTexture = AssetUtilities.RequestImmediate<Texture2D>(Texture + "Aura");
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float backScale = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f % MathHelper.TwoPi) * 0.1f;
            spriteBatch.Draw(auraTexture.Value, position, auraTexture.Value.Frame(), Color.Black * 0.3f, 1f, auraTexture.Value.Size() * 0.5f, scale * backScale * 1.2f, 0, 0);

            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            float backScale = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f % MathHelper.TwoPi) * 0.1f;
            spriteBatch.Draw(auraTexture.Value, Item.Center - Main.screenPosition, auraTexture.Value.Frame(), Color.Black * 0.4f, 1f, auraTexture.Value.Size() * 0.5f, scale * backScale * 1.2f, 0, 0);
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Color color = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 40f);
            spriteBatch.Draw(glowTexture.Value, position, frame, Color.White, 0, origin, scale, 0, 0);
            spriteBatch.Draw(glowTexture.Value, position, frame, new Color(50, 50, 50, 0), 0, origin, scale, 0, 0);
            spriteBatch.Draw(auraTexture.Value, position, auraTexture.Value.Frame(), new Color(color.R, color.G, color.B, 0) * 0.1f, 0, auraTexture.Value.Size() * 0.5f, scale, 0, 0);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Rectangle frame = Main.itemAnimations[Type].GetFrame(glowTexture.Value, Main.itemFrameCounter[whoAmI]);
            Color color = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Main.GlobalTimeWrappedHourly * 40f);
            spriteBatch.Draw(glowTexture.Value, Item.Center - Main.screenPosition, frame, Color.White, rotation, frame.Size() * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glowTexture.Value, Item.Center - Main.screenPosition, frame, new Color(50, 50, 50, 0), rotation, frame.Size() * 0.5f, scale, 0, 0);
            spriteBatch.Draw(auraTexture.Value, Item.Center - Main.screenPosition, auraTexture.Value.Frame(), new Color(color.R, color.G, color.B, 0) * 0.1f, rotation, auraTexture.Value.Size() * 0.5f, scale, 0, 0);
        }
    }
}
