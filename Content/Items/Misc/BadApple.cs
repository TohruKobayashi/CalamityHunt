using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.Items.Rarities;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityHunt.Content.Items.Misc
{
    public class BadApple : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<VioletRarity>();
            if (ModLoader.HasMod(HUtils.CalamityMod)) {
                ModRarity r;
                Mod calamity = ModLoader.GetMod(HUtils.CalamityMod);
                calamity.TryFind<ModRarity>("Violet", out r);
                Item.rare = r.Type;
            }
        }

        int time = 0;

        public override void HoldItem(Player player)
        {
            
            if (time == 0) {
                for (int i = 0; i < 2500; i++) {
                    CalamityHunt.particles.Add(Particle.Create<ChromaticEnergyDust>(particle => {
                        particle.position = player.position;
                        particle.velocity = new Vector2(0, 0);
                        particle.scale = 5f;
                        particle.color = Color.White;
                        particle.colorData = new ColorOffsetData(true, 20f);
                    }));
                }
            }
            time++;
            if (time >= 240) {
                time = 0;
            }

            //for (int i = 0; i < 1; i++) {
            //    CosmosMetaball.particles.Add(Particle.Create<SmokeSplatterParticle>(particle => {
            //        particle.position = player.Center;
            //        particle.velocity = Main.rand.NextVector2Circular(90, 90) * Utils.GetLerpValue(0, 50, 1, true);
            //        particle.scale = Main.rand.NextFloat(20f, 30f) * Utils.GetLerpValue(-30, 50, 1, true);
            //        particle.maxTime = Main.rand.Next(70, 100);
            //        particle.color = Color.White;
            //        particle.fadeColor = Color.White;
            //        particle.anchor = () => player.velocity * 0.66f;
            //    }));
            //}
        }
    }
}
