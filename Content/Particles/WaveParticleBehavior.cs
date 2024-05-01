/*using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class WaveParticleBehavior : ParticleBehavior
    {
        public int time;
        public int maxTime;
        public int variant;
        public override void OnSpawn()
        {
            variant = Main.rand.Next(4);
            scale *= Main.rand.NextFloat(0.9f, 1.1f);
            maxTime = Main.rand.Next(30, 40);
        }

        public override void Update()
        {
            time++;
            if (time > 8)
                velocity *= 0.9f;

            rotation = velocity.ToRotation() + MathHelper.PiOver2;
            if (time > maxTime)
                Active = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);
            Asset<Texture2D> bloom = ModContent.Request<Texture2D>(Texture + "Bloom");
            Rectangle frame = texture.Frame(4, 2, variant, 0);
            Rectangle glowFrame = texture.Frame(4, 2, variant, 1);

            Vector2 squish = new Vector2(0.9f - velocity.Length() * 0.02f, 1f + velocity.Length() * 0.02f);

            Color topColor = Color.Lerp(color, new Color(255, 255, 255, 0), Utils.GetLerpValue(128, 170, color.A)) * Utils.GetLerpValue(maxTime, maxTime * 0.7f, time, true);
            Color baseColor = Color.Lerp(Color.Lerp(Color.Black, color, 0.8f), color, Utils.GetLerpValue(210, 250, color.A)) * Utils.GetLerpValue(maxTime, maxTime * 0.7f, time, true);
            Color bloomColor = color * Utils.GetLerpValue(maxTime, maxTime * 0.7f, time, true);
            bloomColor.A = 0;

            spriteBatch.Draw(texture.Value, position - Main.screenPosition, frame, baseColor, rotation, frame.Size() * new Vector2(0.5f, 0.1f), scale * squish, 0, 0);
            spriteBatch.Draw(texture.Value, position - Main.screenPosition, glowFrame, topColor, rotation, frame.Size() * new Vector2(0.5f, 0.1f), scale * squish, 0, 0);
            
            spriteBatch.Draw(bloom.Value, position - Main.screenPosition, frame, bloomColor, rotation, frame.Size() * new Vector2(0.5f, 0.1f), scale * squish, 0, 0);
            spriteBatch.Draw(bloom.Value, position - Main.screenPosition, glowFrame, bloomColor * 0.4f, rotation, frame.Size() * new Vector2(0.5f, 0.1f), scale * squish, 0, 0);
        }
    }
}
*/
