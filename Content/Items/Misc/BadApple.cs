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
                        CosmosMetaball.particles.Add(Particle.Create<SmokeSplatterMetaball>(particle => {
                            particle.position = player.Center;
                            particle.velocity = Main.rand.NextVector2Circular(90, 90) * Utils.GetLerpValue(0, 50, 1, true);
                            particle.scale = Main.rand.NextFloat(20f, 30f) * Utils.GetLerpValue(-30, 50, 1, true);
                            particle.maxTime = Main.rand.Next(70, 100);
                            particle.color = Color.White;
                            particle.fadeColor = Color.White;
                            particle.anchor = () => player.velocity * 0.66f;
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
