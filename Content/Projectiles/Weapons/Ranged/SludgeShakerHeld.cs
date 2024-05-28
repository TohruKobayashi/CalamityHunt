using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.UI;
using CalamityHunt.Content.Particles;
using CalamityHunt.Content.Projectiles.Weapons.Melee;
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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class SludgeShakerHeld : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.friendly = true;
            Projectile.timeLeft = 10000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            //Projectile.hide = true;
            Projectile.manualDirectionChange = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.noEnchantmentVisuals = true;
        }

        public ref float Time => ref Projectile.ai[0];
        public ref float Mode => ref Projectile.ai[1];

        public ref Player Owner => ref Main.player[Projectile.owner];

        public override void AI()
        {
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (Time == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();

            Owner.ChangeDir(Projectile.velocity.X > 0 ? 1 : -1);
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Owner.SetDummyItemTime(3);
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.rotation.AngleTowards(Owner.AngleTo(Main.MouseWorld), 0.4f), 0.5f * Utils.GetLerpValue(5, 10, Time % 10, true));
            Projectile.velocity = Projectile.rotation.ToRotationVector2();
            Projectile.Center = Owner.MountedCenter - new Vector2(0, 6 * Owner.gravDir) + Projectile.velocity.SafeNormalize(Vector2.Zero) * 10;
            bool canKill = false;

            if (Main.rand.NextBool())
            {
                Vector2 sludgeVelocity = (-(MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.direction + Projectile.rotation).ToRotationVector2() * Main.rand.NextFloat(1f, 4f);
                Vector2 sludgeOff = -(new Vector2(22, 18 * Projectile.direction) + Main.rand.NextVector2Circular(15, 5).RotatedBy(-MathHelper.PiOver4 * Projectile.direction)).RotatedBy(Projectile.rotation);
                Dust sludge = Dust.NewDustPerfect(Projectile.Center + sludgeOff * Projectile.scale * gunSquish, DustID.TintableDust, sludgeVelocity, 100, Color.Black, 0.5f + Main.rand.NextFloat(2f));
                sludge.noGravity = !Main.rand.NextBool(25);
            }

            if (Time % 3 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(0.05f) * Main.rand.Next(15, 20), ModContent.ProjectileType<DarkSludge>(), Owner.HeldItem.damage, 1f, Owner.whoAmI);
            }

            //if (Time % 10 == 2)
            //{
            //    SoundStyle sludgeSound = SoundID.DD2_SkeletonSummoned;
            //    sludgeSound.PitchVariance = 0.2f;
            //    sludgeSound.MaxInstances = 0;
            //    SoundEngine.PlaySound(sludgeSound, Projectile.Center);
            //}

            if (!Owner.channel)
                canKill = true;

            if (!canKill)
                Projectile.timeLeft = 10000;

            if (canKill && Projectile.timeLeft > 27)
                Projectile.timeLeft = 27;

            float gunSquishProg = Utils.GetLerpValue(0, 3, Time % 10, true) * Utils.GetLerpValue(10, 3, Time % 10, true);
            gunSquish = new Vector2(1.05f - gunSquishProg * 0.11f, 0.9f + gunSquishProg * 0.15f);

            Time++;

            HandleSound();

        }

        public LoopingSound squartSound;
        public float volume;
        public float pitch;

        public void HandleSound()
        {
            volume = Utils.GetLerpValue(0, 8, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 5, Time, true) * 0.6f;
            pitch = Utils.GetLerpValue(0, 8, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 5, Time, true) - 0.8f;
            if (squartSound == null)
                squartSound = new LoopingSound(AssetDirectory.Sounds.Weapon.SludgeShakerFiringLoop, new ProjectileAudioTracker(Projectile).IsActiveAndInGame);

            squartSound.Update(() => Projectile.Center, () => volume, () => pitch);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public Vector2 gunSquish;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() * new Vector2(0.15f, 0.5f - 0.1f * Projectile.direction);
            SpriteEffects spriteEffects = Projectile.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, Projectile.Center - new Vector2(30, 0).RotatedBy(Projectile.rotation) * Projectile.scale - Main.screenPosition, texture.Frame(), lightColor, Projectile.rotation, origin, Projectile.scale * gunSquish, spriteEffects, 0);

            return false;
        }
    }
}
