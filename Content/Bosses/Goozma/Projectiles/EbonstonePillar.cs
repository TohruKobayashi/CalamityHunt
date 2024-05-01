using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Bosses.Goozma.Projectiles
{
    public class EbonstonePillar : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 20000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Height => ref Projectile.ai[1];
        public ref float MaxTime => ref Projectile.ai[2];
        private List<Vector2> position;
        private List<int> variant;
        private List<float> scale;
        private List<float> rotation;
        private List<int> time;

        public override void OnSpawn(IEntitySource source)
        {
            position = new List<Vector2>();
            variant = new List<int>();
            scale = new List<float>();
            rotation = new List<float>();
            time = new List<int>();
        }

        private int maxHeight;
        private int waitTime = 220;
        private float speed = 2;
        private int stoneCount;

        public override void AI()
        {
            Projectile.velocity *= 0.1f;
            if (Time == 0)
                maxHeight = (int)Height;
            if (Time >= 0 && Time % speed == 0 && Height > 0)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(20, 25) * Main.rand.NextFloatDirection(), 0);
                position.Add(Projectile.Center + new Vector2(offset.X * 0.5f, -35 * (Time / speed) - offset.Y));
                variant.Add(Main.rand.Next(4));
                rotation.Add(offset.X * 0.04f);
                scale.Add(0.4f);
                time.Add(0);
                for (int i = 0; i < Main.rand.Next(3, 5); i++)
                {
                    offset = new Vector2(Main.rand.NextFloat(38, 40) * Main.rand.NextFloatDirection(), Main.rand.NextFloat(4, 8) * i);

                    position.Add(Projectile.Bottom + new Vector2(offset.X * 0.7f, -35 * (Time / speed) - offset.Y));
                    variant.Add(Main.rand.Next(4));
                    rotation.Add(offset.X * 0.04f);
                    scale.Add(0.2f);
                    time.Add(0);
                }
                Height--;
                stoneCount = position.Count;
            }

            if (position.Count > 0)
            {
                if (Height <= 0 && Time > (maxHeight * speed) + MaxTime)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Dust.NewDustPerfect(position[position.Count - 1] + Main.rand.NextVector2Circular(12, 18) + Vector2.UnitY * 16, DustID.Stone, Main.rand.NextVector2Circular(2, 2) - Vector2.UnitY * 3, 0, new Color(215, 200, 255), 1.5f);
                        if (Main.rand.NextBool())
                            Dust.NewDustPerfect(position[position.Count - 1] + Main.rand.NextVector2Circular(20, 42), DustID.Shadowflame, Main.rand.NextVector2Circular(6, 1), 0, Color.White, 2f).noGravity = true;

                        if (position.Count > 0)
                        {
                            position.RemoveAt(position.Count - 1);
                            variant.RemoveAt(position.Count - 1);
                            rotation.RemoveAt(position.Count - 1);
                            scale.RemoveAt(position.Count - 1);
                            time.RemoveAt(position.Count - 1);
                        }

                        if (position.Count < 10 && Time > 5)
                            Projectile.Kill();
                    }
                }

                for (int i = 0; i < position.Count; i++)
                {
                    scale[i] += 0.1f;
                    if (scale[i] > 1f)
                        scale[i] = 1f;
                    time[i]++;
                }
            }

            Time++;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (position.Count > 2)
            {
                if (Height > 0 && Time < (maxHeight * speed) + MaxTime)
                {
                    hitbox.Y = (int)(Projectile.Bottom.Y - 35 * (maxHeight - Height));
                    hitbox.Height = (int)(35 * (maxHeight - Height));
                }
                else if (Height <= 0 && position.Count > 2)
                {
                    hitbox.Y = (int)(Projectile.Bottom.Y - 35 * maxHeight);
                    hitbox.Height = (int)(Utils.GetLerpValue(0, stoneCount, position.Count, true) * maxHeight) * 35;
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> glow = ModContent.Request<Texture2D>(Texture + "Glow");
            Asset<Texture2D> tell = TextureAssets.Extra[178];

            Color glowColor = new Color(100, 60, 255, 0);
            if (Time < 0)
            {
                float width = Projectile.width * 1.2f;
                Color tellColor = glowColor * (float)Math.Pow(Utils.GetLerpValue(-70, -30, Time, true), 2f) * Utils.GetLerpValue(0, -60, Time, true) * 0.6f;
                Main.EntitySpriteDraw(tell.Value, new Vector2(Projectile.Center.X - Main.screenPosition.X, Main.screenHeight + 5), null, tellColor, Projectile.rotation - MathHelper.PiOver2, tell.Size() * new Vector2(0f, 0.5f), new Vector2(1.5f, width), 0, 0);
                return false;

            }

            if (position.Count > 2)
            {
                for (int i = position.Count - 1; i > 0; i--)
                {
                    Rectangle frame = texture.Frame(4, 1, variant[i], 0);
                    float rockScale = scale[i] + (float)Math.Sin((time[i] * 0.1f + i * 0.1f) % MathHelper.TwoPi) * 0.1f;
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 off = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi / 4f * j) * rockScale;
                        Main.EntitySpriteDraw(glow.Value, position[i] + off + frame.Size() * new Vector2(0f, 0.3f) - Main.screenPosition, frame, glowColor * 0.7f, rotation[i], frame.Size() * new Vector2(0.5f, 0.6f), Projectile.scale * rockScale, 0, 0);
                    }
                }                
                for (int i = position.Count - 1; i > 0; i--)
                {
                    Rectangle frame = texture.Frame(4, 1, variant[i], 0);
                    float rockScale = scale[i] + (float)Math.Sin((time[i] * 0.1f + i * 0.1f) % MathHelper.TwoPi) * 0.1f;
                    Color drawColor = Lighting.GetColor(position[i].ToTileCoordinates());
                    //drawColor = drawColor.MultiplyRGBA(Color.Lerp(Color.White, Color.DarkOrchid, 0.4f + (float)Math.Sin((time[i] * 0.1f + i * 0.1f) % MathHelper.TwoPi) * 0.4f));
                    Main.EntitySpriteDraw(texture.Value, position[i] + frame.Size() * new Vector2(0f, 0.3f) - Main.screenPosition, frame, drawColor, rotation[i], frame.Size() * new Vector2(0.5f, 0.6f), Projectile.scale * rockScale, 0, 0);
                }
            }

            return false;
        }
    }
}
