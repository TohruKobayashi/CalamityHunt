using CalamityHunt.Common.Players;
using CalamityHunt.Common.Systems;
using CalamityHunt.Content.Items.Weapons.Magic;
using CalamityHunt.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityHunt.Common.UI
{
    public class StressBar : ModSystem
    {
        private static bool active;

        public static float fillPercent;
        public static float oldPercent;

        private static int stressFrame = 0;
        private static int stressAnimDelay = 0;
        private static int stressTopFrame = -1;
        private static int stressTopTimer = 0;
        public override void UpdateUI(GameTime gameTime)
        {
            fillPercent = Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stress;
            oldPercent = Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().checkStress;
            if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().active)
            {
                stressAnimDelay++;
                if (stressAnimDelay >= 8 - (int)(oldPercent * 4))
                {
                    stressFrame++;
                    stressAnimDelay = 0;
                }
                if (stressFrame >= 5)
                    stressFrame = 0;
                if (fillPercent >= 1f && stressTopFrame <= 10)
                {
                    stressTopTimer++;
                    if (stressTopTimer >= 5)
                    {
                        stressTopFrame++;
                        stressTopTimer = 0;
                    }
                }
                else if (fillPercent < 1f)
                    stressTopFrame = -1;
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().active)
            {
                int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Entity Health Bars"));
                if (mouseTextIndex != -1)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                        "HuntOfTheOldGod: Stress Bar",
                        delegate
                        {
                            Texture2D bar = AssetDirectory.Textures.Bars.Stress.Value;
                            Texture2D barCharge = AssetDirectory.Textures.Bars.StressCharge.Value;
                            Texture2D barTop = AssetDirectory.Textures.Bars.StressTopped.Value;

                            Vector2 vector = new Vector2(Config.Instance.stressX, Config.Instance.stressY);
                            if (vector.X < 0f || vector.X > 100f)
                                vector.X = 35.77406f;
                            if (vector.Y < 0f || vector.Y > 100f)
                                vector.Y = 3.97614312f;
                            Vector2 position = new Vector2((int)(vector.X * 0.01f * (Main.screenWidth - bar.Width)), (int)(vector.Y * 0.01f * (Main.screenHeight - (bar.Height / 5))));
                            Vector2 shake = Main.LocalPlayer.GetModPlayer<SplendorJamPlayer>().stressedOut ? Shake() * oldPercent : Vector2.Zero;
                            Vector2 offset = new Vector2(21, 26);
                            Vector2 pos = position + shake - offset;
                            int fillAmount = (fillPercent > 0.99f) ? 60 + 42 : (int)(60 * fillPercent + 42);

                            Rectangle mouse = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
                            Rectangle value = new Rectangle((int)(pos.X + 21), (int)(pos.Y + 26), 92, 24);
                            if (mouse.Intersects(value))
                                Main.instance.MouseText("Stress: " + StressString(fillPercent * 100, 100), 0, 0);

                            Rectangle barFrame = new Rectangle(0, stressFrame * (bar.Height / 5), bar.Width, bar.Height / 5);
                            Rectangle fillFrame = new Rectangle(0, (int)(oldPercent / 0.25f)*(barCharge.Height / 5), fillAmount, barCharge.Height / 5);
                            Rectangle topFrame = new Rectangle(0, stressTopFrame * (barTop.Height / 9), barTop.Width, barTop.Height / 9);

                            Main.spriteBatch.Draw(bar, pos, barFrame, Color.White, 0, Vector2.Zero, 1f, 0, 0);
                            Main.spriteBatch.Draw(barCharge, pos, fillFrame, Color.White, 0, Vector2.Zero, 1f, 0, 0);
                            Main.spriteBatch.Draw(barTop, pos, topFrame, Color.White, 0, Vector2.Zero, 1f, 0, 0);
                            return true;
                        },
                        InterfaceScaleType.UI));
                }
            }
        }
        private static Vector2 Shake()
        {
            float shakeIntensity = Config.Instance.stressShake;
            return new Vector2(Main.rand.NextFloat(0f - shakeIntensity, shakeIntensity), Main.rand.NextFloat(0f - shakeIntensity, shakeIntensity));
        }
        private static string StressString(float val, float max)
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            stringBuilder.Append((100f * val / max).ToString("0.00"));
            stringBuilder.Append("% (");
            stringBuilder.Append(val.ToString("n2"));
            stringBuilder.Append(" / ");
            stringBuilder.Append(max.ToString("n2"));
            stringBuilder.Append(')');
            return stringBuilder.ToString();
        }
    }
}
