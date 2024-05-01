using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Humanizer;
using Terraria.Audio;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class CometKunaiGhostProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod("CalamityMod"))
            {
                DamageClass d;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<DamageClass>("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[0] > -1 && CanDamage().Value)
            {
                NPC target = Main.npc[(int)Projectile.ai[0]];
                if (target.CanBeChasedBy(this, true) && target.active)
                {
                    Projectile.extraUpdates = 2;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center).SafeNormalize(Vector2.Zero) * 24, 0.1f);
                }
            }
            else
            {
                if (Projectile.ai[0] < 0)
                    Projectile.ai[0] = Projectile.FindTargetWithinRange(800)?.whoAmI ?? -1;

                else
                {
                    NPC target = Main.npc[(int)Projectile.ai[0]];
                    if (!target.CanBeChasedBy(this, true) && target.active)
                        Projectile.ai[0] = -1;
                }

                Projectile.extraUpdates = 1;
                Projectile.velocity *= 0.99f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle killSound = SoundID.MaxMana with { MaxInstances = 0, Pitch = 0.5f, PitchVariance = 0.4f, Volume = 0.5f };
            SoundEngine.PlaySound(killSound, Projectile.Center);
            for (int i = 0; i < 9; i++)
            {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 10, 10, DustID.RainbowRod, newColor: randomColor);
                d.noGravity = true;
            }
        }

        public override bool? CanDamage() => Projectile.ai[1] > 20;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D sparkle = AssetDirectory.Textures.Sparkle.Value;
            Vector2 direction = Projectile.rotation.ToRotationVector2() * 10;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float p = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Type];
                Color drawColor = Color.Lerp(new Color(0, 10, 190, 0), new Color(60, 180, 255, 0), p);
                Main.EntitySpriteDraw(sparkle, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, sparkle.Frame(), drawColor, Projectile.oldRot[i] + MathHelper.PiOver2, sparkle.Size() * 0.5f, Projectile.scale * new Vector2(0.7f * p, 0.7f), 0, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center + direction - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * new Vector2(1f, 0.5f), Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(sparkle, Projectile.Center - Main.screenPosition, sparkle.Frame(), new Color(0, 10, 60, 0), Projectile.rotation + MathHelper.PiOver2, sparkle.Size() * 0.5f, Projectile.scale * new Vector2(1.5f, 0.6f), 0, 0);

            return false;
        }
    }
}
