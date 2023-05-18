﻿using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Gores.CrystalShieldGores;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Magic
{
    public class CrystalGauntletBall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = DamageClass.Magic;
        }

        public ref float Time => ref Projectile.ai[0];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            bool canKill = false;

            Owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            Owner.SetDummyItemTime(1);
            SetMagicHands();
            Owner.heldProj = Projectile.whoAmI;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Owner.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.Zero) * Owner.HeldItem.shootSpeed, 0.04f);
            Projectile.Center = Owner.MountedCenter + Projectile.velocity * (2f + 8f * Projectile.scale);

            if ((Time - 8) % (8 + (int)(Owner.itemAnimationMax * 1.77f)) < 12)
            {
                if ((Time - 8) % (8 + (int)(Owner.itemAnimationMax * 1.77f)) == 2)
                    Owner.CheckMana(35, true);

                if ((Time - 8) % 4 == 1)
                {
                    SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);

                    for (int i = 0; i < 7; i++)
                    {
                        Color color = Main.hslToRgb((Time + i) * 0.03f % 1f, 0.5f, 0.5f, 128);
                        Dust sparkle = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.PortalBolt, Projectile.velocity * Main.rand.NextFloat(2f), 0, color, 1f + Main.rand.NextFloat());
                        sparkle.noGravity = true;
                        sparkle.noLightEmittence = true;
                    }

                    Particle.NewParticle(Particle.ParticleType<CrossSparkle>(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, Main.hslToRgb(Time * 0.03f % 1f, 0.5f, 0.5f, 128), 0.5f + Main.rand.NextFloat());

                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity * 5f + Main.rand.NextVector2Circular(16, 16), Projectile.velocity, ModContent.ProjectileType<CrystalLightning>(), Owner.HeldItem.damage, 1f, Owner.whoAmI, ai1: Projectile.whoAmI);
                }
            }
            else
            {
                if (!Owner.channel || !Owner.CheckMana(35))
                    canKill = true;
            }

            if (!canKill)
                Projectile.timeLeft = 10000;

            if (canKill && Projectile.timeLeft > 10)
                Projectile.timeLeft = 10;

            if (canKill && Owner.GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge > 0.99f && Projectile.timeLeft > 3)
            {
                Projectile.timeLeft = 3;
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge = 0;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 3f, ModContent.ProjectileType<CrystalGauntletBallThrown>(), Owner.HeldItem.damage, 2f, Owner.whoAmI);
            }
            else
            {
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge += 0.002f + Owner.GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge * 0.01f;
                Owner.GetModPlayer<GoozmaWeaponsPlayer>().crystalGauntletsWaitTime = 50;
            }

            if (Projectile.timeLeft < 10)
            {
                for (int i = 0; i < 3; i++)
                {
                    Color glowColor = new GradientColor(CrystalGauntlets.SpectralColor, 0.3f, 0.3f).ValueAt(Projectile.timeLeft);
                    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(6, 6) * Projectile.scale, 0, glowColor, 2f * Projectile.scale);
                    mainGlow.noGravity = true;
                    mainGlow.noLightEmittence = true;
                }

                for (int i = 0; i < 4; i++)
                {
                    Color glowColor = Main.hslToRgb((Time * 0.03f + i * 0.01f) % 1f, 0.5f, 0.5f, 128);
                    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowRod, Main.rand.NextVector2Circular(9, 9) * Projectile.scale, 0, glowColor, 2f * Projectile.scale);
                    mainGlow.noGravity = true;
                    mainGlow.noLightEmittence = true;
                }
            }

            Time++;

            float modRotB = compArmRotBack;
            float modRotF = compArmRotFront;
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, modRotB);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, modRotF);

            Projectile.scale = MathF.Sqrt(Utils.GetLerpValue(2, 30, Time, true) * Utils.GetLerpValue(0, 25, Projectile.timeLeft, true));
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation -= Owner.direction * 0.2f;
        }

        private float compArmRotBack;
        private float compArmRotFront;

        private void SetMagicHands()
        {
            float wobbleBack = -MathF.Sin(Time * 0.04f + 0.7f) * 0.2f - MathF.Sin(Time * 0.3f + 0.5f) * 0.04f;
            float offBack = -0.3f * Owner.direction;
            compArmRotBack = Projectile.velocity.ToRotation() - MathHelper.PiOver2 + offBack + wobbleBack;

            float wobbleFront = MathF.Sin(Time * 0.04f) * 0.2f + MathF.Sin(Time * 0.3f) * 0.04f;
            float offFront = 0.7f * Owner.direction;
            compArmRotFront = Projectile.velocity.ToRotation() - MathHelper.PiOver2 + offFront + wobbleFront;

            if (Time > 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Color glowColor = new GradientColor(CrystalGauntlets.SpectralColor, 0.1f, 0.1f).ValueAt(Time * 0.5f);
                    Vector2 off = new Vector2(6f + MathF.Sin(Time * 0.5f - i * 0.02f) * 6f, 0).RotatedBy((Time - i / 2f) * 0.15f * Owner.direction) * Projectile.scale;
                    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center + off, DustID.PortalBoltTrail, off.SafeNormalize(Vector2.Zero) * 1.5f * MathF.Pow(Projectile.scale, 1.5f) + Owner.velocity, 0, glowColor, 1.5f * Projectile.scale);
                    mainGlow.noGravity = true;
                    mainGlow.noLightEmittence = true;
                }

                for (int i = 0; i < 3; i++)
                {
                    Color glowColor = Main.hslToRgb((Time * 0.03f + i * 0.1f) % 1f, 0.5f, 0.5f, 128);
                    Vector2 off = new Vector2(15 + MathF.Sin(Time * 0.1f - i * MathHelper.TwoPi / 3f) * 12f, 0).RotatedBy((Time * 0.14f + i * MathHelper.PiOver2 / 5f) * (i % 2 == 1 ? (-1f) : 1f) * Owner.direction) * Projectile.scale;
                    Dust mainGlow = Dust.NewDustPerfect(Projectile.Center + off, DustID.RainbowRod, off.SafeNormalize(Vector2.Zero) * MathF.Pow(Projectile.scale, 1.5f) + Owner.velocity, 0, glowColor, 1.1f * Projectile.scale);
                    mainGlow.noGravity = true;
                    mainGlow.noLightEmittence = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>($"{nameof(CalamityHunt)}/Assets/Textures/Goozma/GlowSoft").Value;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float scale = MathF.Sqrt(Projectile.scale) * (1f + MathF.Sin(Time * 0.8f) * 0.1f) * 0.6f;

            Color rainbowColor = Main.hslToRgb(Time * 0.03f % 1f, 0.5f, 0.7f, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(rainbowColor.R, rainbowColor.G, rainbowColor.B), Projectile.rotation, texture.Size() * 0.5f, scale * 1.3f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor, Projectile.rotation * 1.3f, texture.Size() * 0.5f, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), rainbowColor * 0.6f, Projectile.rotation * 0.7f, texture.Size() * 0.5f, scale * 1.2f, spriteEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor, Projectile.rotation * 0.5f, glow.Size() * 0.5f, scale * 1.5f, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), rainbowColor * 0.15f, Projectile.rotation * 0.5f, glow.Size() * 0.5f, scale * 4f, 0, 0);

            return false;
        }
    }
}