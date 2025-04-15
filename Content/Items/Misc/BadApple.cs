using CalamityHunt.Common.Graphics.RenderTargets;
using CalamityHunt.Common.Systems;
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
            if (Config.Instance.debugMode) {
                if (time == 0) {
                    for (int i = 0; i < 15; i++) {
                        CalamityHunt.Particles.Add(Particle.Create<FusionFlameParticle>(particle => {
                            particle.position = player.Center;
                            particle.velocity = player.velocity * Main.rand.NextFloat();
                            particle.rotation = player.velocity.ToRotation();
                            particle.scale = 4.25f + Main.rand.NextFloat(1f, 2f);
                            particle.maxTime = Main.rand.Next(25, 40);
                            //particle.color = (glowColor * 0.8f) with { A = 50 };
                            //particle.fadeColor = (glowColor * 0.1f) with { A = 20 };
                            particle.emitLight = true;
                        }));
                    }
                }
                time++;
                if (time >= 120) {
                    time = 0;
                }
            }
        }
    }
}
