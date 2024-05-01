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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class FissionFlyerMiniRing : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 1;
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

            int target = Projectile.FindTargetWithLineOfSight();
            if (target > -1)
            {
                Projectile.velocity += Projectile.DirectionTo(Main.npc[target].Center) * 2f;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.oldVelocity.Length();
            }

            Projectile.rotation += Projectile.direction * 0.2f;

            Dust splode = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20) * Projectile.scale, DustID.AncientLight, -Projectile.velocity * Main.rand.NextFloat(3f), 0, glowColor, 1f + Main.rand.NextFloat());
            splode.noGravity = true;

            Dust dust = Dust.NewDustPerfect(Projectile.Center - Main.rand.NextVector2Circular(30, 30) * Projectile.scale, DustID.Sand, Projectile.velocity * Main.rand.NextFloat(), 0, Color.Black, Main.rand.NextFloat());
            dust.noGravity = true;

            if (Projectile.ai[1] == 1)
            {
                Projectile.penetrate = -1;
                Projectile.scale = Utils.GetLerpValue(240, 200, Projectile.timeLeft, true) * Utils.GetLerpValue(0, 120, Projectile.timeLeft, true) * 5;
                Projectile.Resize((int)(60 * Projectile.scale), (int)(60 * Projectile.scale));
            }

            Time++;
            Projectile.localAI[0] = Main.GlobalTimeWrappedHourly * 40f + Time;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (MathF.Abs(oldVelocity.X - Projectile.velocity.X) > 0)
                Projectile.velocity.X = -oldVelocity.X;            
            if (MathF.Abs(oldVelocity.Y - Projectile.velocity.Y) > 0)
                Projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<FusionBurn>(), 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;

            Color backColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            backColor.A = 180;
            Color glowColor = new GradientColor(SlimeUtils.GoozOilColors, 0.2f, 0.2f).ValueAt(Projectile.localAI[0]);
            glowColor.A = 0;
            SpriteEffects spinEffects = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), backColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.5f, spinEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(200, 200, 200, 0), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.5f, spinEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), glowColor, Projectile.rotation * 0.8f, texture.Size() * 0.5f, Projectile.scale * 0.6f, spinEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), glowColor * 0.3f, Projectile.rotation * 0.6f, texture.Size() * 0.5f, Projectile.scale * 0.7f, spinEffects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), glowColor * 0.5f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, spinEffects, 0);

            return false;
        }
    }
}
