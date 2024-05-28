/*using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Content.Bosses.Goozma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class GoozGelBitOld : ParticleBehavior
    {
        private bool colorful;
        private int variant;
        private int direction;
        private float colOffset;
        private int time;
        private Vector2 homePos;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(1f, 1.1f);
            variant = Main.rand.Next(3);
            direction = Main.rand.NextBool() ? 1 : -1;
            colOffset = Main.rand.NextFloat();
            homePos = position;
            colorful = Main.rand.NextBool(3);
            behindEntities = true;
        }

        public override void Update()
        {
            time++;
            rotation += velocity.X * 0.02f;

            if (data is int)
            {
                if (time > 20 && time < (int)data)
                {
                    velocity *= 0.99f;
                    velocity = Vector2.Lerp(velocity, Main.rand.NextVector2Circular(78, 67), 0.02f);
                }
                else if (time > (int)data + 20)
                {
                    velocity = Vector2.Lerp(velocity, position.DirectionTo(homePos).SafeNormalize(Vector2.Zero) * (position.Distance(homePos) + 10) * 0.02f, 0.1f * Utils.GetLerpValue((int)data + 20, (int)data + 40, time, true));
                    velocity = velocity.RotatedBy(0.03f * direction);
                    if (position.Distance(homePos) < 30)
                        scale *= 0.9f;
                }
            }
            else
                velocity *= 0.98f;

            if (scale < 0.5f)
                Active = false;

            if (Main.rand.NextBool(50))
            {
                ParticleBehavior hue = NewParticle(ModContent.GetInstance<HueLightDust>(), position + Main.rand.NextVector2Circular(30, 30), Main.rand.NextVector2Circular(2, 2) - Vector2.UnitY * 2f, Color.White, 1f);
                hue.data = time * 2f + colOffset; 
            }


            if (Main.rand.NextBool(120))
            {
                Vector2 gooVelocity = -velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.2f);
                ParticleBehavior goo = NewParticle(ModContent.GetInstance<GooBurst>(), position + Main.rand.NextVector2Circular(30, 30), gooVelocity, Color.White, 0.1f + Main.rand.NextFloat(1.5f));
                goo.data = time * 2f + colOffset;

            }

            if (Main.rand.NextBool(70))
                Dust.NewDustPerfect(position + Main.rand.NextVector2Circular(10, 10), DustID.TintableDust, Main.rand.NextVector2CircularEdge(3, 3), 100, Color.Black, Main.rand.NextFloat(2, 4)).noGravity = true;
        }

        public static Asset<Texture2D> texture;

        public override void Load()
        {
            texture = AssetUtilities.RequestImmediate<Texture2D>(Texture);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D glow = AssetDirectory.Textures.Glow.Value;
            Rectangle frame = texture.Value.Frame(3, 2, variant, 0);
            Rectangle glowFrame = texture.Value.Frame(3, 2, variant, 1);

            Color glowColor = new GradientColor(SlimeUtils.GoozColors, 0.2f, 0.2f).ValueAt(time * 2f + colOffset);
            glowColor.A = 0;
            if (colorful)
                spriteBatch.Draw(glow, position - Main.screenPosition, null, glowColor * 0.1f, rotation, glow.Size() * 0.5f, scale * 2f, 0, 0);

            for (int i = 0; i < 4; i++)
            {
                Vector2 off = new Vector2(2).RotatedBy(MathHelper.TwoPi / 4f * i + rotation);
                spriteBatch.Draw(texture.Value, position + off - Main.screenPosition, frame, glowColor, rotation, frame.Size() * 0.5f, scale, 0, 0);
            }
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, Color.Lerp(color, Color.Black, 0.6f), rotation, frame.Size() * 0.5f, scale, 0, 0);

            if (colorful)
                spriteBatch.Draw(texture.Value, position - Main.screenPosition, glowFrame, glowColor * 0.75f * scale, rotation, frame.Size() * 0.5f, scale, 0, 0);

        }
    }
}
*/
