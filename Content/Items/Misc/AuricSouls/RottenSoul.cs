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
using CalamityHunt.Common;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityHunt.Common.Utilities;

namespace CalamityHunt.Content.Items.Misc.AuricSouls
{
    public class RottenSoul : ModItem
    {
        public static Texture2D chainTexture;
        public static Texture2D iconTexture;

        public override void Load()
        {
            chainTexture = AssetDirectory.Textures.AuricSouls.OldDukeSoulChain.Value;
            iconTexture = AssetDirectory.Textures.RottenSoulEye.Value;
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
                calamity.TryFind("PureGreen", out r);
                Item.rare = r.Type;
            }
        }

        public override bool OnPickup(Player player)
        {
            for (int i = 0; i < 150; i++)
            {
                Color glowColor = Color.Lerp(GetAlpha(Color.White).Value, Color.Green, Main.rand.NextFloat(0.5f) + 0.3f);
                glowColor.A = 0;

                Dust soul = Dust.NewDustPerfect(Item.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(10, 10), 0, glowColor, Main.rand.NextFloat(2f));
                soul.noGravity = true;
            }
            player.GetModPlayer<AuricSoulPlayer>().olddukeSoul = true;

            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color[] array = new Color[]
            {
                new Color(79, 255, 120),
                new Color(59, 245, 102),
                new Color(32, 230, 78),
                new Color(11, 219, 59),
                new Color(34, 240, 81),
                new Color(71, 237, 109)
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
                heartbeatSound = new LoopingSound(AssetDirectory.Sounds.Souls.YharonSoulHeartbeat, new HUtils.ItemAudioTracker(Item).IsActiveAndInGame);
            heartbeatSound.PlaySound(() => Item.position, () => 1f, () => 0f);

            if (droneSound == null)
                droneSound = new LoopingSound(AssetDirectory.Sounds.Souls.YharonSoulDrone, new HUtils.ItemAudioTracker(Item).IsActiveAndInGame);
            droneSound.PlaySound(() => Item.position, () => 1.5f, () => 0f);

            if (breathSoundCounter-- <= 0)
            {
                SoundEngine.PlaySound(AssetDirectory.Sounds.Souls.YharonSoulBreathe, Item.Center);
                breathSoundCounter = Main.rand.Next(500, 800);
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
            Texture2D glowTexture = AssetDirectory.Textures.Glow[1].Value;

            Color glowColor = GetAlpha(Color.White).Value;
            glowColor.A = 0;
            Color darkColor = Color.Lerp(glowColor, Color.Turquoise, 0.7f);
            darkColor.A = 0;

            float soulScale = scale;
            scale = 1f + MathF.Sin(Main.GlobalTimeWrappedHourly * 3f % MathHelper.TwoPi) * 0.01f;

            if (includeChains)
            {
                VertexStrip strip1 = new VertexStrip();
                VertexStrip strip2 = new VertexStrip();

                int count = 80;
                Vector2[] offs1 = new Vector2[count];
                Vector2[] offs2 = new Vector2[count];
                float[] offRots1 = new float[count];
                float[] offRots2 = new float[count];

                float time = Main.GlobalTimeWrappedHourly * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    Vector2 x = new Vector2(95 + MathF.Sin(time - i / (float)count * MathHelper.TwoPi * 2) * 10f, 0).RotatedBy(MathHelper.TwoPi / (count - 1f) * 0.35f * i - time);
                    x.X *= 1f + MathF.Cos(time * 1.5f) * 0.1f;
                    Vector2 y = new Vector2(95 + MathF.Sin(time - i / (float)count * MathHelper.TwoPi * 3) * 10f, 0).RotatedBy(MathHelper.TwoPi / (count - 1f) * 0.35f * i - time + 3f);
                    y.Y *= 1f + MathF.Sin(time * 1.5f) * 0.1f;
                    offs1[i] = x.RotatedBy(time);
                    offs2[i] = y.RotatedBy(time);
                }

                for (int i = 1; i < count; i++)
                {
                    offRots1[i] = offs1[i - 1].AngleTo(offs1[i]);
                    offRots2[i] = offs2[i - 1].AngleTo(offs2[i]);
                }
                offRots1[0] = offRots1[1];
                offRots2[0] = offRots2[1];

                Color StripColor(float p)
                {
                    Color[] array = new Color[]
                    {
                        new Color(79, 255, 120),
                        new Color(59, 245, 102),
                        new Color(32, 230, 78),
                        new Color(11, 219, 59),
                        new Color(34, 240, 81),
                        new Color(71, 237, 109)
                    };
                    Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt((p + Main.GlobalTimeWrappedHourly) * 25f);
                    return final * 0.6f;
                }
                Color StripColor2(float p)
                {
                    Color[] array = new Color[]
                    {
                        new Color(79, 40, 79),
                        new Color(107, 58, 107),
                        new Color(138, 78, 138),
                        new Color(181, 107, 181),
                        new Color(145, 81, 145),
                        new Color(99, 51, 99)
                    };
                    Color final = new GradientColor(array, 0.15f, 0.5f).ValueAt((p + Main.GlobalTimeWrappedHourly) * 25f);
                    return final * 0.6f;
                }
                float StripWidth(float p) => 10f * (1f + MathF.Sin(p * MathHelper.TwoPi * 3) * 0.3f) * p * (1f - p) * 2f;

                strip1.PrepareStrip(offs1, offRots1, StripColor, StripWidth, Item.Center - Main.screenPosition, offs1.Length, true);
                strip2.PrepareStrip(offs2, offRots2, StripColor, StripWidth, Item.Center - Main.screenPosition, offs2.Length, true);

                Effect effect = AssetDirectory.Effects.CrystalLightning.Value;
                effect.Parameters["uTransformMatrix"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
                effect.Parameters["uTexture"].SetValue(chainTexture);
                effect.Parameters["uGlow"].SetValue(TextureAssets.Extra[197].Value);
                effect.Parameters["uColor"].SetValue(Vector3.One);
                effect.Parameters["uTime"].SetValue(-time * 1.1f % 1f);
                effect.CurrentTechnique.Passes[0].Apply();

                strip1.DrawTrail();
                strip2.DrawTrail();

                for (int p = 0; p < 4; p++) {
                    List<(VertexStrip, float, float)> whirlStrips = new List<(VertexStrip, float, float)>();

                    float totStrips = 7;
                    whirlStrips.Add((new VertexStrip(), 2, 40));
                    whirlStrips.Add((new VertexStrip(), 2.2f, 31));
                    whirlStrips.Add((new VertexStrip(), 3.7f, 34));
                    whirlStrips.Add((new VertexStrip(), 2.2f, 28));
                    whirlStrips.Add((new VertexStrip(), 2.4f, 26));
                    whirlStrips.Add((new VertexStrip(), 4f, 22));

                    for (int i = 0; i < whirlStrips.Count; i++) {
                        VertexStrip v = whirlStrips[i].Item1;

                        float counte = 80;
                        Vector2[] offs = new Vector2[count];
                        float[] offRots = new float[count];

                        for (int j = 0; j < counte; j++) {
                            Vector2 x = new Vector2(MathHelper.Lerp(whirlStrips[i].Item3, whirlStrips[i].Item3 - (10), (j + 1) / counte) + (p > 1 ? 10 : 0)).RotatedBy(MathHelper.TwoPi / (count - 1f) * 0.35f * j - time);
                            x.X *= 1f + MathF.Cos(time * 1.5f) * 0.1f;
                            offs[j] = x.RotatedBy((p > 2 ? -1 : 1) *time * whirlStrips[i].Item2 + (whirlStrips[i].Item2) + (p * MathHelper.PiOver4));
                        }

                        for (int j = 1; j < counte; j++) {
                            offRots[j] = offs1[j - 1].AngleTo(offs1[j]);
                        }
                        offRots[0] = offRots[1];

                        v.PrepareStrip(offs, offRots, p > 2 ? StripColor2 : StripColor, StripWidth, Item.Center - Main.screenPosition, offs.Length, true);
                        v.DrawTrail();
                    }
                }

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.1f, 0, glowTexture.Size() * 0.5f, scale, 0, 0);
            }

            if (includeLensFlare)
            {
                float lensScale = scale + MathF.Sin(Main.GlobalTimeWrappedHourly * 2f) + 2f;
                float time = Main.GlobalTimeWrappedHourly * 0.1f;
                float lensRotation = time % MathHelper.TwoPi;

                for (int i = 0; i < 6; i++) {
                    float rotOff = (i + 1) * MathHelper.TwoPi / 6f;
                    spriteBatch.Draw(sparkTexture, Item.Center - Main.screenPosition, sparkTexture.Frame(), darkColor, rotOff, new Vector2(0, sparkTexture.Height / 2), new Vector2(0.3f, 1f + lensScale * 1f), 0, 0);
                }

                spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.05f, 0, glowTexture.Size() * 0.5f, lensScale * 1.5f, 0, 0);
            }

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.1f, 0, glowTexture.Size() * 0.5f, scale, 0, 0);

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), darkColor * 0.7f, 0, glowTexture.Size() * 0.5f, scale * 0.2f, 0, 0);

            spriteBatch.Draw(iconTexture, Item.Center - Main.screenPosition, iconTexture.Frame(), GetAlpha(Color.White).Value * 0.3f, 0, iconTexture.Size() * 0.5f, scale * 0.8f, 0, 0);
            spriteBatch.Draw(iconTexture, Item.Center - Main.screenPosition, iconTexture.Frame(), new Color(255, 255, 255, 0), 0, iconTexture.Size() * 0.5f, scale * 0.8f, 0, 0);
            spriteBatch.Draw(iconTexture, Item.Center - Main.screenPosition, iconTexture.Frame(), glowColor, 0, iconTexture.Size() * 0.5f, scale * 0.9f, 0, 0);

            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.4f, 0, glowTexture.Size() * 0.5f, 0.2f + scale * 0.4f, 0, 0);
            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.2f, 0, glowTexture.Size() * 0.5f, scale, 0, 0);
            spriteBatch.Draw(glowTexture, Item.Center - Main.screenPosition, glowTexture.Frame(), glowColor * 0.6f, 0, glowTexture.Size() * 0.5f, 0.1f + scale * 0.1f, 0, 0);

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Type].Value;
            Texture2D glowTexture = AssetDirectory.Textures.Glow[1].Value;

            Color glowColor = GetAlpha(Color.White).Value;
            glowColor.A = 0;

            spriteBatch.Draw(texture, position, frame, GetAlpha(Color.White).Value, 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);
            spriteBatch.Draw(texture, position, frame, new Color(255, 255, 255, 0), 0, frame.Size() * 0.5f, scale + 0.2f, 0, 0);

            spriteBatch.Draw(glowTexture, position, glowTexture.Frame(), glowColor * 0.7f, 0, glowTexture.Size() * 0.5f, scale * 0.2f, 0, 0);

            return false;
        }
    }
}
