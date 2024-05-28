using CalamityHunt.Common.Systems.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Particles
{
    public class DarkSludgeChunk : Particle
    {
        public int variant;
        public int time;
        public bool stuck;

        public override void OnSpawn()
        {
            scale *= Main.rand.NextFloat(0.7f, 0.9f);
            variant = Main.rand.Next(2);
        }

        public override void Update()
        {
            time++;

            if (!stuck)
            {
                if (velocity.Y < 30)
                    velocity.Y += 0.4f;

                rotation = velocity.ToRotation() - MathHelper.PiOver2;

                if (Collision.IsWorldPointSolid(position + velocity) && time > 2)
                {
                    time = 0;
                    stuck = true;
                    position.Y = (int)(position.Y / 16f) * 16 + 16;
                    for (int i = 0; i < 8; i++)
                    {
                        if (Collision.IsWorldPointSolid(position + velocity - new Vector2(0, 8 * i)))
                            position.Y -= 8;                        
                    }
                    position.Y -= 3;
                }
            }
            else
            {
                rotation = 0;
                velocity = Vector2.Zero;
                if (time > 10)
                    scale *= 0.95f;

                if (scale < 0.1f)
                    ShouldRemove = true; 
            }

        }
    }
}
