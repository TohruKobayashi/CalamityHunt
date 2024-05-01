using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace CalamityHunt.Content.Projectiles.Weapons.Summoner
{
    public class PinkyLight : ModProjectile
    {
        public override string Texture => $"{Mod.Name}/Assets/Textures/SharpSpark";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 80;
            Projectile.hide = true;
        }

        public override void AI()
        {
            if (Projectile.ai[2] < 0)
            {
                Projectile.Kill();
                return;
            }

            Projectile host = Main.projectile[(int)Projectile.ai[2]];

            if (!host.active || host.type != ModContent.ProjectileType<DivinePinky>() || host.owner != Projectile.owner)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = host.Center;

            if (Projectile.timeLeft < 75 && Projectile.ai[0] < 1)
            {
                SoundStyle ray = SoundID.Item29 with { MaxInstances = 0, Pitch = -0.4f, PitchVariance = 0.2f, Volume = 0.2f };
                SoundEngine.PlaySound(ray, Projectile.Center);

                Projectile.ai[0]++;
            }

            if (Projectile.timeLeft < 75 && Projectile.timeLeft > 47 && Projectile.timeLeft % Projectile.localNPCHitCooldown == 0)
            {
                float scale = 1f + Projectile.ai[1] / 80f;
                Color color = Color.Lerp(Color.Orchid, Color.CadetBlue, Utils.GetLerpValue(60, 70, Projectile.timeLeft, true));
                color.A = 0;
                int count = Main.rand.Next(5, 10);
                for (int i = 0; i < count; i++)
                    sparkles.Add(new Sparkle(MathHelper.TwoPi / count * i + Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-1f, 1f) * 0.05f, Main.rand.NextFloat(0.9f, 1.2f) * scale, Main.rand.Next(15, 25), color));
            }

            if (sparkles == null)
                sparkles = new List<Sparkle>();

            foreach (Sparkle sparkle in sparkles.ToList())
            {
                sparkle.Update();
                if (!sparkle.active)
                    sparkles.Remove(sparkle);
            }

            Lighting.AddLight(Projectile.Center, Color.Orchid.ToVector3() * 0.5f);
        }

        public override bool? CanDamage() => Projectile.timeLeft < 75 && Projectile.timeLeft > 47 && Projectile.timeLeft % Projectile.localNPCHitCooldown == 0;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float radius = MathHelper.Clamp(Projectile.Center.Distance(targetHitbox.Center.ToVector2()), 0, Projectile.ai[1]);
            Vector2 checkPoint = Projectile.Center + (targetHitbox.Center.ToVector2() - Projectile.Center).SafeNormalize(Vector2.Zero) * radius;
            if (targetHitbox.Contains(checkPoint.ToPoint()))
                return true;

            return false;
        }

        private List<Sparkle> sparkles;

        private class Sparkle
        {
            public float rotation;
            public float angularVelocity;
            public float scale;
            public Color color;
            public int time;
            public int maxTime;
            public bool active;

            public Sparkle(float rotation, float angularVelocity, float scale, int maxTime, Color color)
            {
                this.rotation = rotation;
                this.angularVelocity = angularVelocity;
                this.scale = scale;
                this.maxTime = maxTime;
                this.color = color;
                active = true;
            }

            public void Update()
            {
                angularVelocity *= 0.95f;
                rotation += angularVelocity;
                time++;

                if (time > maxTime)
                    active = false;
            }

            public void Draw(Vector2 position)
            {
                Texture2D sparkle = AssetDirectory.Textures.Sparkle.Value;
                Texture2D glow = AssetDirectory.Textures.Glow.Value;
                Rectangle glowFrame = glow.Frame(1, 2, 0, 0);
                Vector2 drawScale = new Vector2(scale * 0.3f, scale * 1.4f) * Utils.GetLerpValue(0, maxTime * 0.1f, time, true);
                Vector2 thinScale = new Vector2(scale * 0.06f, scale * 0.5f) * Utils.GetLerpValue(0, maxTime * 0.7f, time, true);
                Vector2 offset = new Vector2(0, -15 - 30 * time / maxTime).RotatedBy(rotation);
                float fadeOut = Utils.GetLerpValue(maxTime, 0, time, true);
                Main.EntitySpriteDraw(sparkle, position + offset, sparkle.Frame(), new Color(160, 160, 160, 40) * fadeOut, rotation, sparkle.Size() * 0.5f, thinScale, 0, 0);
                Main.EntitySpriteDraw(sparkle, position + offset, sparkle.Frame(), color * fadeOut, rotation, sparkle.Size() * 0.5f, thinScale, 0, 0);
                Main.EntitySpriteDraw(glow, position, glowFrame, color * 0.3f * fadeOut, rotation, glowFrame.Size() * new Vector2(0.5f, 1f), drawScale * 0.6f, 0, 0);
                Main.EntitySpriteDraw(glow, position, glowFrame, color * 0.1f * fadeOut, rotation, glowFrame.Size() * new Vector2(0.5f, 1f), drawScale, 0, 0);
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindProjectiles.Add(index);

        public override bool PreDraw(ref Color lightColor)
        {
            if (sparkles != null) foreach (Sparkle sparkle in sparkles) sparkle.Draw(Projectile.Center - Main.screenPosition);

            return false;
        }
    }
}
