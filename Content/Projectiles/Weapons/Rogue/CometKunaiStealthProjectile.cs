using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Projectiles.Weapons.Rogue
{
    public class CometKunaiStealthProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Throwing;
            if (ModLoader.HasMod("CalamityMod"))
            {
                DamageClass d;
                Mod calamity = ModLoader.GetMod("CalamityMod");
                calamity.TryFind<DamageClass>("RogueDamageClass", out d);
                Projectile.DamageType = d;
            }
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            for (int i = 0; i < 4; i++)
            {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(18), 36, 36, DustID.SparkForLightDisc, 0, 0, 0, randomColor);
                d.noGravity = true;
                if (Projectile.ai[0] != 0)
                    d.velocity += Main.rand.NextVector2Circular(12, 12);
            }

            if (Projectile.ai[1] > -1)
            {
                NPC target = Main.npc[(int)Projectile.ai[1]];
                if (target.active)
                    Projectile.Center += (target.position - target.oldPosition) / (Projectile.extraUpdates + 1);
                else
                    Projectile.Kill();
            }

            if (Projectile.ai[0] == 0)
                Projectile.rotation += Projectile.direction * 0.3f;
            else
            {
                if (Projectile.ai[2]++ % 4 == 0)
                {
                    Vector2 position = Projectile.Center - new Vector2(0, 400) + Main.rand.NextVector2Circular(500, 300);
                    Vector2 velocity = position.DirectionTo(Projectile.Center).RotatedByRandom(0.12f).SafeNormalize(Vector2.Zero) * position.Distance(Projectile.Center) * 0.024f;
                    velocity.X *= 0.9f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, velocity, ModContent.ProjectileType<CometKunaiStarfall>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }
            }

            if (Projectile.frameCounter++ > 1)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 3;
            }

            Projectile.localAI[0]++;
        }

        private void SetCollided(bool stick)
        {
            Projectile.extraUpdates = 1;
            Projectile.ai[0] = stick ? -2 : -1;
            Projectile.localAI[1] = 1f;
            Projectile.timeLeft = stick ? 180 : 30;
            if (stick)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
                SoundStyle attachSound = AssetDirectory.Sounds.Slime.StellarConstellationForm with { MaxInstances = 0, Pitch = 0.8f, PitchVariance = 0.1f, Volume = 0.4f };
                SoundEngine.PlaySound(attachSound, Projectile.Center);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 0)
                SetCollided(true);

            Projectile.Center += oldVelocity;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SetCollided(true);
            Projectile.Center += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Projectile.ai[1] = target.whoAmI;
        }

        public override bool? CanDamage() => Projectile.ai[0] == 0;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.ai[1] < 0)
                behindNPCsAndTiles.Add(index);
            else
                Projectile.hide = false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 18; i++)
            {
                Color randomColor = Color.Lerp(Color.Blue, Color.RoyalBlue, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 10, 10, DustID.RainbowRod, newColor: randomColor);
                d.noGravity = true;
                d.velocity += Main.rand.NextVector2Circular(14, 14);
            }            
            
            for (int i = 0; i < 6; i++)
            {
                Color randomColor = Color.Lerp(Color.Goldenrod, Color.Gold, Main.rand.NextFloat());
                randomColor.A = 0;
                Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(5), 10, 10, DustID.RainbowRod, newColor: randomColor);
                d.noGravity = true;
                d.velocity += Main.rand.NextVector2Circular(6, 6);
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = new Vector2(0, -10).RotatedBy(Projectile.rotation + MathHelper.TwoPi / 5f * i);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<CometKunaiGhostProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[1]);
            }

            SoundStyle killSound = SoundID.DD2_ExplosiveTrapExplode with { MaxInstances = 0, Pitch = -1 };
            SoundEngine.PlaySound(killSound, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D flame = AssetDirectory.Textures.Extras.CometKunaiFlame.Value;
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            Rectangle fireFrame = flame.Frame(1, 3, 0, Projectile.frame);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), new Color(0, 70, 100, 0), Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale * 1.3f, 0, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(), Color.White, Projectile.rotation, texture.Size() * new Vector2(0.5f, 0.55f), Projectile.scale, 0, 0);


            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(5, 10, 60, 0), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 1.2f, 0, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, glow.Frame(), new Color(0, 5, 30, 0), Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 2f, 0, 0);
            
            if (Projectile.ai[0] == 0)
            {
                float p = Utils.GetLerpValue(5, 20, Projectile.localAI[0], true);
                Main.EntitySpriteDraw(flame, Projectile.Center - Main.screenPosition, fireFrame, Color.Black * p, Projectile.velocity.ToRotation() - MathHelper.PiOver2, fireFrame.Size() * new Vector2(0.5f, 0.75f), Projectile.scale * 1.2f, 0, 0);
                Main.EntitySpriteDraw(flame, Projectile.Center - Main.screenPosition, fireFrame, new Color(0, 20, 200, 20) * p, Projectile.velocity.ToRotation() - MathHelper.PiOver2, fireFrame.Size() * new Vector2(0.5f, 0.75f), Projectile.scale * 1.3f, 0, 0);
            }

            return false;
        }
    }
}
