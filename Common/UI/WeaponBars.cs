using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Melee;
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
using Terraria.UI;

namespace CalamityHunt.Common.UI
{
    public class WeaponBars : ModSystem
    {
        private static int showTime;
        private static bool anyBar;

        private static Color baseColor;
        private static Color fillColor;
        public static int style;
        public static float fillPercent;

        public static void DisplayBar()
        {
            if (anyBar)
                showTime = 120;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            style = 0;
            anyBar = false;
            baseColor = Color.DimGray;
            fillColor = Color.White;
            //showTime set elsewhere
            if (showTime > 0)
                showTime--;

            //decide which bar
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<Parasanguine>())
            {
                anyBar = true;
                baseColor = Color.DarkRed;
                fillColor = Color.Lerp(Color.DarkRed * 0.5f, Color.Red, Utils.GetLerpValue(0.5f, 1f, Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent, true));
                fillPercent = Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().ParasolBloodPercent;
            }            
            
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<CrystalGauntlets>())
            {
                anyBar = true;
                baseColor = Color.SlateBlue;
                fillColor = Color.Lerp(Color.MediumOrchid, Color.Turquoise, Utils.GetLerpValue(0.3f, 0.8f, Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge, true));
                fillColor.A = 128;
                fillPercent = Main.LocalPlayer.GetModPlayer<GoozmaWeaponsPlayer>().CrystalGauntletsCharge;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (showTime > 0)
            {
                int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Entity Health Bars"));
                if (mouseTextIndex != -1)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                        "HuntOfTheOldGod: Weapon Charge Bars",
                        delegate
                        {
                            if (anyBar)
                            {
                                float fade = Utils.GetLerpValue(0, 30, showTime, true);

                                Texture2D bar = AssetDirectory.Textures.Bars.Bar.Value;
                                Texture2D barCharge = AssetDirectory.Textures.Bars.BarCharge.Value;

                                int fillAmount = (fillPercent > 0.99f) ? barCharge.Width : (int)(barCharge.Width * fillPercent);
                                Rectangle fillFrame = new Rectangle(0, 0, fillAmount, barCharge.Height);
                                Vector2 position = (Main.LocalPlayer.Center - Main.screenPosition) / Main.UIScale - new Vector2(barCharge.Width / 2f, 48f / Main.UIScale);

                                float fadeOut = Utils.GetLerpValue(0, 30, showTime, true);
                                Main.spriteBatch.Draw(bar, position, bar.Frame(), baseColor * fadeOut, 0, Vector2.Zero, 1f, 0, 0);
                                Main.spriteBatch.Draw(barCharge, position, fillFrame, fillColor * fadeOut, 0, Vector2.Zero, 1f, 0, 0);
                            }

                            return true;
                        },
                        InterfaceScaleType.UI));
                }
            }
        }
    }
}
