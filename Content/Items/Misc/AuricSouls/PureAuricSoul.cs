using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc.AuricSouls
{
    public class PureAuricSoul : ModItem
    {
        public static Texture2D chainTexture;

		public override bool IsLoadingEnabled(Mod mod) => false;

        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>(Texture + "Chain", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemIconPulse[Type] = true;
            ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod("CalamityMod"))
            {
                ModRarity r;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind("Violet", out r);
                Item.rare = r.Type;
            }
        }

        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < 150; i++)
            {
                if (Main.rand.NextBool(5))
                {
                    Vector2 off = Main.rand.NextVector2Circular(20, 20);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(5, 5), GetAlpha(Color.White).Value * 0.2f, 1f + Main.rand.NextFloat());
                }

                if (Main.rand.NextBool(8))
                {
                    Vector2 off = Main.rand.NextVector2Circular(20, 20);
                    float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<PrettySparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(7, 7), GetAlpha(Color.White).Value * 0.2f, scale * 0.6f);
                }

                Dust soul = Dust.NewDustPerfect(Item.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(10, 10), 0, GetAlpha(Color.White).Value, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
            player.GetModPlayer<AuricSoulPlayer>().pureSoul = true;

            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color[] array = new Color[]
            {
                new Color(255, 160, 20),
                new Color(255, 200, 80),
                new Color(255, 160, 20),
                new Color(255, 80, 120),
                new Color(255, 160, 20),
                new Color(255, 80, 120),
                new Color(30, 120, 255),
                new Color(255, 80, 120),
            };
            Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 15f);
            return final;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.rand.NextBool(15))
            {
                Vector2 off = Main.rand.NextVector2Circular(30, 30);
                float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(1, 1), GetAlpha(Color.White).Value * 0.2f, scale);
            }

            if (Main.rand.NextBool(25))
            {
                Vector2 off = Main.rand.NextVector2Circular(20, 20);
                float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                ParticleBehavior.NewParticle(ModContent.GetInstance<PrettySparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(4, 4), GetAlpha(Color.White).Value * 0.2f, scale * 0.6f);
            }

            if (Main.rand.NextBool(5))
            {
                Dust soul = Dust.NewDustDirect(Item.Center - new Vector2(15), 30, 30, DustID.PortalBoltTrail, 0f, -Main.rand.NextFloat(1f, 2f), 0, GetAlpha(Color.White).Value, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            bool includeChains = true;
            bool includeLensFlare = true;

            Texture2D texture = TextureAssets.Item[Type].Value;
            Texture2D sparkTexture = AssetDirectory.Textures.Sparkle.Value;
            Texture2D glowTexture = AssetDirectory.Textures.GlowBig.Value;

            Color glowColor = GetAlpha(Color.White).Value;
            glowColor.A = 0;

            float soulScale = scale;
            scale = 1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 3f % MathHelper.TwoPi) * 0.01f;

            //spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), Color.Black * 0.1f, -MathHelper.PiOver4, sparkTexture.Size() * 0.5f, scale * new Vector2(3f, 0.8f), 0, 0);
            //spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), Color.Black * 0.1f, MathHelper.PiOver4, sparkTexture.Size() * 0.5f, scale * new Vector2(3f, 0.8f), 0, 0);
            //spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), Color.Black * 0.1f, 0, sparkTexture.Size() * 0.5f, scale * new Vector2(3f, 0.8f), 0, 0);
            //spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), Color.Black * 0.1f, MathHelper.PiOver2, sparkTexture.Size() * 0.5f, scale * new Vector2(3f, 0.8f), 0, 0);

            if (includeChains)
            {
                VertexStrip strip1 = new VertexStrip();
                VertexStrip strip2 = new VertexStrip();
                VertexStrip strip3 = new VertexStrip();

                int count = 120;
                Vector2[] offs1 = new Vector2[count];
                Vector2[] offs2 = new Vector2[count];
                Vector2[] offs3 = new Vector2[count];
                float[] offRots1 = new float[count];
                float[] offRots2 = new float[count];
                float[] offRots3 = new float[count];

                float time = Main.GlobalTimeWrappedHourly;
                for (int i = 0; i < count; i++)
                {
                    Vector2 x = new Vector2(35 + MathF.Sin(time - i * 0.04f) * 10f, 0).RotatedBy(MathHelper.TwoPi / 120f * i - time);
                    x.X *= 1f + MathF.Cos(time * 1.5f) * 0.2f;
                    Vector2 y = new Vector2(40 + MathF.Cos(time - i * 0.07f) * 17f, 0).RotatedBy(MathHelper.TwoPi / 130f * i - time * 0.7f + 2f);
                    y.Y *= 0.5f + MathF.Sin(time * 1.2f) * 0.1f;
                    Vector2 z = new Vector2(60 + MathF.Cos(time - i * 0.01f) * 10f, 0).RotatedBy(MathHelper.TwoPi / 150f * i + time * 2f + 2f);
                    z.Y *= 0.8f + MathF.Sin(time) * 0.1f;
                    offs1[i] = x.RotatedBy(-time * 1f) * 0.8f;
                    offs2[i] = y.RotatedBy(time * 0.8f) * 0.9f;
                    offs3[i] = z.RotatedBy(-time * 0.1f) * new Vector2(1f, 0.8f) * 0.7f;

                    Vector2 f = new Vector2(20 + time * 0.4f % 1f * 150f, 0).RotatedBy(MathHelper.TwoPi / 80f * i - time + 2f);
                    f.Y *= 1f + MathF.Sin(time * 2f) * 0.1f;
                }

                offRots1[0] = offs1[0].AngleTo(offs1[1]);
                offRots2[0] = offs2[0].AngleTo(offs2[1]);
                offRots3[0] = offs2[0].AngleTo(offs2[1]);
                for (int i = 1; i < count; i++)
                {
                    offRots1[i] = offs1[i - 1].AngleTo(offs1[i]);
                    offRots2[i] = offs2[i - 1].AngleTo(offs2[i]);
                    offRots3[i] = offs3[i - 1].AngleTo(offs3[i]);
                }

                Color StripColor(float p)
                {
                    Color[] array = new Color[]
                    {
                        new Color(255, 160, 20),
                        new Color(255, 200, 80),
                        new Color(255, 160, 20),
                        new Color(255, 80, 120),
                        new Color(255, 160, 20),
                        new Color(255, 80, 120),
                        new Color(30, 120, 255),
                        new Color(255, 80, 120),
                    };
                    Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt((p * 0.6f + Main.GlobalTimeWrappedHourly) * 15f);
                    return final * 0.6f;
                }
                float StripWidth(float p) => Utils.GetLerpValue(0f, 0.3f, p, true) * Utils.GetLerpValue(1f, 0.7f, p, true) * 25f * (1f + MathF.Sin(p * MathHelper.TwoPi * 8) * 0.1f);

                strip1.PrepareStrip(offs1, offRots1, StripColor, StripWidth, Item.Center - Main.screenPosition, offs1.Length, true);
                strip2.PrepareStrip(offs2, offRots2, StripColor, StripWidth, Item.Center - Main.screenPosition, offs2.Length, true);
                strip3.PrepareStrip(offs3, offRots3, StripColor, StripWidth, Item.Center - Main.screenPosition, offs3.Length, true);

                Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CrystalLightningEffect", AssetRequestMode.ImmediateLoad).Value;
                effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                effect.Parameters["uTexture"].SetValue(chainTexture);
                effect.Parameters["uGlow"].SetValue(TextureAssets.Extra[197].Value);
                effect.Parameters["uColor"].SetValue(Vector3.One);
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f % 1f);
                effect.CurrentTechnique.Passes[0].Apply();

                strip1.DrawTrail();
                strip2.DrawTrail();
                strip3.DrawTrail();

                spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.1f, 0, glowTexture.Size() * 0.5f, scale, 0, 0);

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }

            if (includeLensFlare)
            {
                float lensScale = scale + MathF.Sin(Main.GlobalTimeWrappedHourly * 35) * 0.4f;
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor * 0.1f, -MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.5f, 4f + lensScale * 5f), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor * 0.1f, MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.5f, 4f + lensScale * 5f), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, -MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.5f, 1f + lensScale), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.5f, 1f + lensScale), 0, 0);
                spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.05f, 0, glowTexture.Size() * 0.5f, lensScale * 1.5f, 0, 0);
            }

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.1f, 0, glowTexture.Size() * 0.5f, scale, 0, 0);

            Rectangle frame = Main.itemAnimations[Type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, GetAlpha(Color.White).Value, 0, frame.Size() * 0.5f, soulScale + 0.2f, 0, 0);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, new Color(255, 255, 255, 0), 0, frame.Size() * 0.5f, soulScale + 0.2f, 0, 0);

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.7f, 0, glowTexture.Size() * 0.5f, scale * 0.2f, 0, 0);

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Texture2D glowTexture = AssetDirectory.Textures.GlowBig.Value;

            Color glowColor = GetAlpha(Color.White).Value;
            glowColor.A = 0;

            spriteBatch.Draw(texture, position, frame, GetAlpha(Color.White).Value, 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);
            spriteBatch.Draw(texture, position, frame, new Color(255, 255, 255, 0), 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);

            spriteBatch.Draw(glowTexture, position, glowTexture.Frame(), glowColor * 0.7f, 0, glowTexture.Size() * 0.5f, scale * 0.2f, 0, 0);

            return false;
        }
    }
}
