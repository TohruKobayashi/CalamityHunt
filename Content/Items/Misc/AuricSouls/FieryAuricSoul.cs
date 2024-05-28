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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc.AuricSouls
{
    public class FieryAuricSoul : ModItem
    {
        public static Texture2D chainTexture;

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
                Color glowColor = Color.Lerp(GetAlpha(Color.White).Value, Color.OrangeRed, Main.rand.NextFloat(0.5f) + 0.3f);
                glowColor.A = 0;

                if (Main.rand.NextBool(5))
                {
                    Vector2 off = Main.rand.NextVector2Circular(20, 20);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(5, 5), glowColor * 0.2f, 1f + Main.rand.NextFloat());
                }

                if (Main.rand.NextBool(8))
                {
                    Vector2 off = Main.rand.NextVector2Circular(20, 20);
                    float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                    ParticleBehavior.NewParticle(ModContent.GetInstance<PrettySparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(7, 7), glowColor * 0.2f, scale * 0.6f);
                }

                Dust soul = Dust.NewDustPerfect(Item.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(10, 10), 0, glowColor, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
            player.GetModPlayer<AuricSoulPlayer>().yharonSoul = true;

            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color[] array = new Color[]
            {
                new Color(255, 120, 20),
                new Color(255, 180, 60),
                new Color(255, 120, 20),
                new Color(255, 80, 60),
                new Color(255, 160, 20),
                new Color(255, 80, 60)
            };
            Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt(Main.GlobalTimeWrappedHourly * 25f);
            return final;
        }

        public LoopingSound heartbeatSound;
        public LoopingSound droneSound;

        public int breathSoundCounter;

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (heartbeatSound == null)
                heartbeatSound = new LoopingSound(AssetDirectory.Sounds.YharonAuricSoulHeartbeat, new HuntOfTheOldGodUtils.ItemAudioTracker(Item).IsActiveAndInGame);
            heartbeatSound.Update(() => Item.position, () => 1f, () => 0f);

            if (droneSound == null)
                droneSound = new LoopingSound(AssetDirectory.Sounds.YharonAuricSoulDrone, new HuntOfTheOldGodUtils.ItemAudioTracker(Item).IsActiveAndInGame);
            droneSound.Update(() => Item.position, () => 1.5f, () => 0f);

            if (breathSoundCounter-- <= 0)
            {
                SoundEngine.PlaySound(AssetDirectory.Sounds.YharonAuricSoulBreathe, Item.Center);
                breathSoundCounter = Main.rand.Next(500, 800);
            }

            if (Main.rand.NextBool(15))
            {
                Vector2 off = Main.rand.NextVector2Circular(40, 40);
                float scale = Main.rand.NextFloat() + Utils.GetLerpValue(50, 0, off.Length(), true);
                ParticleBehavior.NewParticle(ModContent.GetInstance<CrossSparkleParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(1, 1), GetAlpha(Color.White).Value * 0.2f, scale);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 off = Main.rand.NextVector2Circular(30, 30);
                float scale = Main.rand.NextFloat(2f) + Utils.GetLerpValue(50, 0, off.Length(), true);
                Color glowColor = Color.Lerp(GetAlpha(Color.White).Value, Color.OrangeRed, 0.6f);
                glowColor.A = 0;
                ParticleBehavior.NewParticle(ModContent.GetInstance<CosmicSmokeParticleBehavior>(), Item.Center + off, Main.rand.NextVector2Circular(2, 2), glowColor, scale * 0.6f);
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
            Color darkColor = Color.Lerp(glowColor, Color.OrangeRed, 0.7f);
            darkColor.A = 0;

            float soulScale = scale;
            scale = 1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 3f % MathHelper.TwoPi) * 0.01f;

            if (includeChains)
            {
                VertexStrip strip1 = new VertexStrip();
                VertexStrip strip2 = new VertexStrip();
                VertexStrip strip3 = new VertexStrip();

                int count = 180;
                Vector2[] offs1 = new Vector2[count];
                Vector2[] offs2 = new Vector2[count];
                Vector2[] offs3 = new Vector2[count];
                float[] offRots1 = new float[count];
                float[] offRots2 = new float[count];
                float[] offRots3 = new float[count];

                float time = Main.GlobalTimeWrappedHourly * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    Vector2 x = new Vector2(95 + MathF.Sin(time - i / (float)count * MathHelper.TwoPi * 2) * 10f, 0).RotatedBy(MathHelper.TwoPi / (count - 1f) * 0.4f * i - time);
                    x.X *= 1f + MathF.Cos(time * 1.5f) * 0.1f;
                    Vector2 y = new Vector2(100 + MathF.Cos(time - i / (float)count * MathHelper.TwoPi * 3) * 10f, 0).RotatedBy(MathHelper.TwoPi / (count - 1f) * 0.45f * i - time + 3f);
                    y.Y *= 0.7f + MathF.Sin(time * 1.2f) * 0.1f;
                    Vector2 z = new Vector2(50 + MathF.Cos(time - i / (float)count * MathHelper.TwoPi * 3f) * 2f, 0).RotatedBy(-MathHelper.TwoPi / (count - 1f) * i + time + 2f);
                    z.Y *= 0.9f + MathF.Sin(time) * 0.1f;
                    offs1[i] = x.RotatedBy(time * 1f) * 0.8f;
                    offs2[i] = y.RotatedBy(time) * 0.9f;
                    offs3[i] = z.RotatedBy(-time * 0.1f) * new Vector2(1f, 0.8f);

                    Vector2 f = new Vector2(20 + time * 0.4f % 1f * 150f, 0).RotatedBy(MathHelper.TwoPi / 80f * i - time + 2f);
                    f.Y *= 1f + MathF.Sin(time * 2f) * 0.1f;
                }

                for (int i = 1; i < count; i++)
                {
                    offRots1[i] = offs1[i - 1].AngleTo(offs1[i]);
                    offRots2[i] = offs2[i - 1].AngleTo(offs2[i]);
                    offRots3[i] = offs3[i - 1].AngleTo(offs3[i]);
                }
                offRots1[0] = offRots1[1];
                offRots2[0] = offRots2[1];
                offRots3[0] = offRots3[1];

                Color StripColor(float p)
                {
                    Color[] array = new Color[]
                    {
                        new Color(255, 120, 20),
                        new Color(255, 180, 60),
                        new Color(255, 120, 20),
                        new Color(255, 80, 60),
                        new Color(255, 160, 20),
                        new Color(255, 80, 60)
                    };
                    Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt((p + Main.GlobalTimeWrappedHourly) * 25f);
                    return final * 0.6f;
                }
                float StripWidth(float p) => 25f * (1f + MathF.Sin(p * MathHelper.TwoPi * 3) * 0.5f) * p * (1f - p) * 5f;

                strip1.PrepareStrip(offs1, offRots1, StripColor, StripWidth, Item.Center - Main.screenPosition, offs1.Length, true);
                strip2.PrepareStrip(offs2, offRots2, StripColor, StripWidth, Item.Center - Main.screenPosition, offs2.Length, true);
                strip3.PrepareStrip(offs3, offRots3, StripColor, StripWidth, Item.Center - Main.screenPosition, offs3.Length, true);

                Effect effect = ModContent.Request<Effect>($"{nameof(CalamityHunt)}/Assets/Effects/CrystalLightningEffect", AssetRequestMode.ImmediateLoad).Value;
                effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                effect.Parameters["uTexture"].SetValue(chainTexture);
                effect.Parameters["uGlow"].SetValue(TextureAssets.Extra[197].Value);
                effect.Parameters["uColor"].SetValue(Vector3.One);
                effect.Parameters["uTime"].SetValue(-time * 1.1f % 1f);
                effect.CurrentTechnique.Passes[0].Apply();

                strip1.DrawTrail();
                strip2.DrawTrail();
                strip3.DrawTrail();

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.1f, 0, glowTexture.Size() * 0.5f, scale, 0, 0);
            }

            if (includeLensFlare)
            {
                float lensScale = scale + MathF.Sin(Main.GlobalTimeWrappedHourly * 24) * 0.2f;
                float lensScaleSlow = scale + MathF.Sin(Main.GlobalTimeWrappedHourly * 24 + 3) * 0.15f;
                float time = Main.GlobalTimeWrappedHourly * 0.1f;
                float lensRotation = time % MathHelper.TwoPi;
                float lensRotationSlow = (time + MathF.Sin(time * 2) * 0.3f) % MathHelper.TwoPi;

                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, lensRotation - MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.3f, 1f + lensScale * 5f), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, lensRotation + MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(0.3f, 1f + lensScale * 5f), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, lensRotation - MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(1f, 1f + lensScale), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, lensRotation + MathHelper.PiOver4, sparkTexture.Size() * 0.5f, new Vector2(1f, 1f + lensScale), 0, 0);

                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, lensRotationSlow, sparkTexture.Size() * 0.5f, new Vector2(0.3f, 2f + lensScaleSlow * 5f), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, lensRotationSlow + MathHelper.PiOver2, sparkTexture.Size() * 0.5f, new Vector2(0.3f, 2f + lensScaleSlow * 5f), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, lensRotationSlow, sparkTexture.Size() * 0.5f, new Vector2(1f, 2f + lensScaleSlow), 0, 0);
                spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), glowColor, lensRotationSlow + MathHelper.PiOver2, sparkTexture.Size() * 0.5f, new Vector2(1f, 2f + lensScaleSlow), 0, 0);

                spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.05f, 0, glowTexture.Size() * 0.5f, lensScale * 1.5f, 0, 0);
            }

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.1f, 0, glowTexture.Size() * 0.5f, scale, 0, 0);

            Rectangle frame = Main.itemAnimations[Type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, GetAlpha(Color.White).Value, 0, frame.Size() * 0.5f, soulScale + 0.2f, 0, 0);
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, new Color(255, 255, 255, 0), 0, frame.Size() * 0.5f, soulScale + 0.2f, 0, 0);

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.7f, 0, glowTexture.Size() * 0.5f, scale * 0.2f, 0, 0);

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
