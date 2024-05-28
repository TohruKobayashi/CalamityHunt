using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using CalamityHunt.Content.Buffs;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class FissionFlyerProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod("CalamityMod"))
            {
                DamageClass d;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<DamageClass>("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
        }

        public ref float Time => ref Projectile.ai[0];

        public override void AI()
        {
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            if (Projectile.ai[1] == 0)
            {
                Projectile.scale = 0.3f + Utils.GetLerpValue(0, 60, Time, true) * 0.7f;
                if (Time == 105)
                {
                     SoundEngine.PlaySound(AssetDirectory.Sounds.Weapon.FissionFlyerExplode, Projectile.Center);
                    int amt = Projectile.ai[2] == 1 ? 5 : 3;
                    for (int i = 0; i < amt; i++)
                    {
                        Vector2 velocity = new Vector2(0, 10).RotatedBy(MathHelper.TwoPi / amt * i);
                        Projectile ring = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<FissionFlyerMiniRing>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        ring.ai[0] = Time;
                    }

                    if (Projectile.ai[2] == 1)
                    {
                        Projectile ring = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FissionFlyerMiniRing>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        ring.ai[1] = 1;
                    }
                    for (int j = 0; j < 40; j++)
                    {
                        Dust splode = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Main.rand.NextVector2Circular(15, 15), 0, glowColor, 1f + Main.rand.NextFloat());
                        splode.noGravity = true;
                    }
                }

                int target = Projectile.FindTargetWithLineOfSight();
                if (target > -1)
                {
                    Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center) * 0.5f;
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.oldVelocity.Length();
                }

                Projectile.extraUpdates = Time < 40 ? 2 : 0;
                Projectile.velocity *= Time < 40 ? 0.99f : 0.95f;

                if (Time > 60)
                    Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.Y, 0.01f);
                else
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

                if (Time == 60)
                    Gore.NewGorePerfect(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitY, GoreID.TreeLeaf_GemTreeRuby, 1f);

                if (Time > 130)
                    Projectile.ai[1] = 1f;
            }
            if (Projectile.ai[1] == 1f)
            {
                Projectile.tileCollide = false;
                Projectile.extraUpdates = 1;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.scale = 0.5f + Utils.GetLerpValue(0, 600, Projectile.Distance(Main.player[Projectile.owner].MountedCenter), true) * 0.5f;

                Projectile.velocity += Projectile.DirectionTo(Main.player[Projectile.owner].MountedCenter) * 0.2f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[Projectile.owner].MountedCenter) * Projectile.oldVelocity.Length(), 0.1f);

                if (Projectile.Distance(Main.player[Projectile.owner].MountedCenter) < 40)
                    Projectile.Kill();
            }

            if (Time < 70 || Time > 120)
            {
                Vector2 off = new Vector2(22, 0).RotatedBy(Projectile.rotation) * Projectile.scale;
                Dust left = Dust.NewDustPerfect(Projectile.Center - off, DustID.AncientLight, -Projectile.velocity * 0.5f, 0, glowColor, 2f * Projectile.scale);
                left.noGravity = true;              
                Dust right = Dust.NewDustPerfect(Projectile.Center + off, DustID.AncientLight, -Projectile.velocity * 0.5f, 0, glowColor, 2f * Projectile.scale);
                right.noGravity = true;
            }

            if (Main.rand.NextBool(3))
                ParticleBehavior.NewParticle(ModContent.GetInstance<HueLightDustParticleBehavior>(), Projectile.Center - Main.rand.NextVector2Circular(20, 20), Projectile.velocity * Main.rand.NextFloat(), glowColor, 0.5f + Main.rand.NextFloat());

            Dust dust = Dust.NewDustPerfect(Projectile.Center - Main.rand.NextVector2Circular(30, 30), DustID.Sand, Projectile.velocity * Main.rand.NextFloat(), 0, Color.Black, Main.rand.NextFloat());
            dust.noGravity = true;

            Time++;
            Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 40f + Time;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (MathF.Abs(oldVelocity.X - Projectile.velocity.X) > 0)
                Projectile.velocity.X = -oldVelocity.X * 1.33f;            
            if (MathF.Abs(oldVelocity.Y - Projectile.velocity.Y) > 0)
                Projectile.velocity.Y = -oldVelocity.Y * 1.33f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
        }

        public override void Load()
        {
            ringTexture = ModContent.Request<Texture2D>(Texture + "Ring", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            handleTexture = ModContent.Request<Texture2D>(Texture + "Handle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public static Texture2D ringTexture;
        public static Texture2D handleTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;

            if (Time < 60)
                Main.EntitySpriteDraw(handleTexture, Projectile.Center - Main.screenPosition, handleTexture.Frame(), Color.White, Projectile.rotation, handleTexture.Size() * 0.5f, Projectile.scale, 0, 0);

            Color backColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            backColor.A = 170;
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            glowColor.A = 0;

            float expand = MathF.Cbrt(Utils.GetLerpValue(70, 90, Time, true)) * (1f - MathF.Cbrt(Utils.GetLerpValue(100, 110, Time, true)) * 0.5f) * MathF.Sqrt(Utils.GetLerpValue(130, 120, Time, true));

            SpriteEffects spinEffects = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float ringScale = (1f - expand * 0.8f) * Projectile.scale;
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition, ringTexture.Frame(), backColor, Main.GlobalTimeWrappedHourly * 14f * Projectile.direction, ringTexture.Size() * 0.5f, ringScale * 0.55f, spinEffects, 0);
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition, ringTexture.Frame(), new Color(200, 200, 200, 0), Main.GlobalTimeWrappedHourly * 14f * Projectile.direction, ringTexture.Size() * 0.5f, ringScale * 0.5f, spinEffects, 0);
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition, ringTexture.Frame(), glowColor * 0.7f, Main.GlobalTimeWrappedHourly * 9f * Projectile.direction, ringTexture.Size() * 0.5f, ringScale * 0.5f, spinEffects, 0);
            Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition, ringTexture.Frame(), glowColor * 0.2f, Main.GlobalTimeWrappedHourly * 6f * Projectile.direction, ringTexture.Size() * 0.5f, ringScale * 0.7f, spinEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), glowColor * 0.3f, 0, glow.Size() * 0.5f, ringScale * 3f, 0, 0);

            Rectangle left = texture.Frame(2, 1, 0, 0);
            Rectangle right = texture.Frame(2, 1, 1, 0);

            Vector2 off = new Vector2(2f + expand * 30f, 0).RotatedBy(Projectile.rotation) * Projectile.scale;
            Main.EntitySpriteDraw(texture, Projectile.Center - off - Main.screenPosition, left, Color.White, Projectile.rotation, left.Size() * new Vector2(1f, 0.5f), Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center + off - Main.screenPosition, right, Color.White, Projectile.rotation, right.Size() * new Vector2(0f, 0.5f), Projectile.scale, 0, 0);

            return false;
        }
    }
}
