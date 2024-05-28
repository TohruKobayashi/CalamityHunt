using System;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged;

public class AntiMassAccumulatorProj : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.friendly = true;
        Projectile.timeLeft = 10000;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ownerHitCheck = true;
        Projectile.hide = true;
        Projectile.manualDirectionChange = true;
        Projectile.DamageType = DamageClass.Ranged;
    }

    public ref float Time => ref Projectile.ai[0];

    public ref Player Player => ref Main.player[Projectile.owner];

    public static Color MainColor => Color.Goldenrod with { A = 40 };

    public override void AI()
    {
        Player.heldProj = Projectile.whoAmI;

        if (!Player.active || Player.dead || Player.noItems || Player.CCed) {
            Projectile.Kill();
            return;
        }

        Vector2 muzzlePosition = Projectile.Center + new Vector2(20, 2 * Projectile.direction).RotatedBy(Projectile.velocity.ToRotation());
        int firstShot = (int)(10 * Player.GetAttackSpeed(DamageClass.Ranged));
        int secondShot = (int)(55 * Player.GetAttackSpeed(DamageClass.Ranged));

        if (Time < firstShot) {
            SoundEngine.PlaySound((SoundID.DD2_PhantomPhoenixShot with { MaxInstances = 0 }).WithVolumeScale(Time / firstShot * 0.5f).WithPitchOffset(Time / firstShot), Projectile.Center);
        }

        if (Time == firstShot) {
            SoundEngine.PlaySound(AssetDirectory.Sounds.Weapons.AntiMassColliderFire.WithPitchOffset(Main.rand.NextFloat(0.1f)).WithVolumeScale(0.7f), Projectile.Center);
            recoilFactor = 1f;
        }

        if (Time == secondShot) {
            SoundEngine.PlaySound(AssetDirectory.Sounds.Weapons.AntiMassColliderLaserBlast, Projectile.Center);
            recoilFactor = 1f;
        }

        if (Projectile.owner == Main.myPlayer) {
            if (Time == 0) {
                Projectile.velocity = Player.MountedCenter.DirectionTo(Main.MouseWorld) * Player.HeldItem.shootSpeed;
                Projectile.netUpdate = true;
            }

            if (Time == firstShot) {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), muzzlePosition, Projectile.velocity.SafeNormalize(Vector2.Zero) * 15f, ModContent.ProjectileType<AntiMassBioBall>(), 1 + Projectile.damage / 5, Projectile.knockBack, Player.whoAmI, 0, -1);

                Player.velocity -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 2f;
                Projectile.netUpdate = true;
            }

            if (Time > (int)(25 * Player.GetAttackSpeed(DamageClass.Ranged)) && Time < (int)(60 * Player.GetAttackSpeed(DamageClass.Ranged))) {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Player.MountedCenter.DirectionTo(Main.MouseWorld) * Player.HeldItem.shootSpeed, 0.2f);
                Projectile.netUpdate = true;

                if (Time > (int)(35 * Player.GetAttackSpeed(DamageClass.Ranged)) && !Player.channel) {
                    Player.SetDummyItemTime(25);
                    Projectile.Kill();
                }
            }

            if (Time == secondShot) {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), muzzlePosition, Projectile.velocity.SafeNormalize(Vector2.Zero) * 15f, ModContent.ProjectileType<AntiMassDeathLaser>(), Projectile.damage, Projectile.knockBack, Player.whoAmI);

                Player.velocity -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 3f;
                Projectile.netUpdate = true;
            }

            if (Time > secondShot + 10) {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Player.DirectionTo(Main.MouseWorld) * Player.HeldItem.shootSpeed, 0.1f);
                Projectile.netUpdate = true;
            }
        }

        //plasmaball chaingun
        //if (Time > firstShot + 3 && Player.channel) {
        //    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Player.DirectionTo(Main.MouseWorld) * Player.HeldItem.shootSpeed, 0.5f);
        //    Time = firstShot - 1;
        //}

        //railcannon chaingun
        //if (Time > secondShot + 6 && Player.channel) {
        //    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Player.DirectionTo(Main.MouseWorld) * Player.HeldItem.shootSpeed, 0.5f);
        //    Time = secondShot - 1;
        //}

        Projectile.rotation = Projectile.velocity.ToRotation() - recoilFactor * 0.01f * Projectile.direction;

        Player.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
        Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
        Player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        Projectile.Center = Player.MountedCenter - new Vector2(0, 6 * Player.gravDir) + Projectile.velocity.SafeNormalize(Vector2.Zero) * 10;
        recoilFactor = Math.Max(0, recoilFactor - 0.025f);

        Time++;

        for (int i = 0; i < 8; i++) {
            if (recoilFactor > 0.0005f && Main.rand.NextBool((int)(25 - recoilFactor * 24))) {
                CalamityHunt.particles.Add(Particle.Create<LightningParticle>(particle => {
                    particle.position = muzzlePosition + Main.rand.NextVector2Circular(27, 10).RotatedBy(Projectile.rotation);
                    particle.rotation = Projectile.rotation + Main.rand.NextFloat(-1f, 1f);
                    particle.velocity = particle.rotation.ToRotationVector2();
                    particle.scale = Main.rand.NextFloat(0.1f, 0.7f);
                    particle.color = Time < (int)(25 * Player.GetAttackSpeed(DamageClass.Ranged)) ? Color.MediumTurquoise with { A = 40 } : MainColor;
                    particle.anchor = () => Player.velocity;
                }));
            }
        }

        Lighting.AddLight(Projectile.Center, MainColor.ToVector3() * 0.2f);

        if (Time <= Player.HeldItem.useTime) {
            Player.SetDummyItemTime(3);
        }

        if (Time > Player.HeldItem.useTime + 1) {
            Projectile.Kill();
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

    public override bool? CanCutTiles() => false;

    public float recoilFactor;

    public static Asset<Texture2D> glowmask;

    public override void Load()
    {
        glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow", AssetRequestMode.ImmediateLoad);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Texture2D glow = glowmask.Value;

        Rectangle baseFrame = texture.Frame(1, 2, 0, 0);
        Rectangle barrelFrame = texture.Frame(1, 2, 0, 1);

        Vector2 origin = baseFrame.Size() * new Vector2(0.1f, 0.5f + 0.4f * Projectile.direction);
        SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

        Vector2 drawScale = Projectile.scale * Vector2.Lerp(Vector2.One, new Vector2(0.97f, 1.05f), recoilFactor);
        Vector2 holdOffset = new Vector2(35 + recoilFactor * 5f, -16 * Projectile.direction).RotatedBy(Projectile.rotation) * drawScale;
        Vector2 barrelOffset = new Vector2(MathF.Pow(Utils.GetLerpValue(0f, 0.4f, recoilFactor, true), 0.5f) * 16, 0).RotatedBy(Projectile.rotation) * drawScale;

        Color barrelColor = Time < (int)(55 * Player.GetAttackSpeed(DamageClass.Ranged)) ? Color.MediumTurquoise with { A = 40 } : MainColor;

        Main.EntitySpriteDraw(texture, Projectile.Center - holdOffset - barrelOffset - Main.screenPosition, barrelFrame, lightColor, Projectile.rotation, origin, drawScale, spriteEffects, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - holdOffset - barrelOffset - Main.screenPosition, barrelFrame, barrelColor * Utils.GetLerpValue(0.3f, 0.7f, recoilFactor, true), Projectile.rotation, origin, drawScale, spriteEffects, 0);
        Main.EntitySpriteDraw(texture, Projectile.Center - holdOffset - Main.screenPosition, baseFrame, lightColor, Projectile.rotation, origin, drawScale, spriteEffects, 0);
        Main.EntitySpriteDraw(glow, Projectile.Center - holdOffset - Main.screenPosition, baseFrame, MainColor, Projectile.rotation, origin, drawScale, spriteEffects, 0);

        return false;
    }
}
