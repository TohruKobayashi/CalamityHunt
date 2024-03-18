using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Ranged
{
    public class CometKunaiCritProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.manualDirectionChange = true;
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                DamageClass d;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
        }

        private Vector2 oldVelocity;

        public override void AI()
        {
            for (int i = 0; i < 5; i++) {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity / 5f * i, DustID.SparkForLightDisc, Projectile.velocity * 0.1f, 0, randomColor, 1.1f);
                d.noGravity = true;
            }

            if (Projectile.ai[0] == 0 && Main.myPlayer == Projectile.owner) {
                Projectile.ai[0]++;
                oldVelocity = Projectile.velocity;
                Projectile.rotation = Main.rand.NextFloat();
                Projectile.netUpdate = true;
            }

            Projectile.direction = oldVelocity.X > 0 ? 1 : -1;

            Projectile.rotation += Projectile.direction * 0.2f;

            Projectile.velocity = oldVelocity.RotatedBy(MathF.Cos(Projectile.ai[1] * 0.19f) * 0.3f);

            Projectile.ai[1]++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++) {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(10), 20, 20, DustID.SparkForLightDisc, 0, 0, 0, randomColor);
                d.noGravity = true;
                d.velocity += Main.rand.NextVector2Circular(4, 4);
            }

            if (Main.myPlayer == Projectile.owner) {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -oldVelocity * 0.6f, ModContent.ProjectileType<CometKunaiGhostProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -1);
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow[0].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(0, 70, 200, 0), Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.6f), Projectile.scale * 1.3f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.6f), Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(0, 10, 90, 0), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 0.7f, 0, 0);

            return false;
        }
    }
}
