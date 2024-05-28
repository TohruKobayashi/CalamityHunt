using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using CalamityHunt.Content.Items.Placeable;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using Terraria.GameContent.ObjectInteractions;
using CalamityHunt.Common.Systems;
using CalamityHunt.Common.Graphics.SlimeMonsoon;
using Terraria.Enums;
using Terraria.Audio;
using Terraria.Chat;
using CalamityHunt.Content.Items.Misc;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace CalamityHunt.Content.Tiles
{
    public class SlimeNinjaStatueTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true; 
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.Origin = new Point16(2, 5);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            DustType = DustID.Stone;
            AddMapEntry(new Color(128, 128, 128));
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<GelatinousCatalyst>();

            if (player.HasItem(ModContent.ItemType<SludgeFocus>()))
                player.cursorItemIconID = ModContent.ItemType<SludgeFocus>();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public static LocalizedText failText;

        public override void Load()
        {
            failText = Language.GetOrRegister(Mod.GetLocalizationKey("Chat.NinjaShrineFailure"));
        }

        public override bool RightClick(int i, int j)
        {
            //SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));

            int top = j;
            int center = i;

            for (int y = 0; y <= 6; y++)
            {
                Tile checkTile = Framing.GetTileSafely(i, j + y);
                if (checkTile.HasTile && checkTile.TileType == Type)
                    top++;
                else
                    break;
            }

            top -= 5;

            for (int x = 0; x <= 4; x++)
            {
                Tile checkTile = Framing.GetTileSafely(i + x, top);
                if (checkTile.TileType != Type)
                {
                    center += x;
                    center -= 2;
                    break;
                }
                    
            }

            Player player = Main.LocalPlayer;

            if (!GoozmaSystem.GoozmaActive)
            {
                if (GoozmaSystem.FindSlimeStatues(center, top, 20, 20))
                {
                    if (player.HasItem(ModContent.ItemType<SludgeFocus>()))
                        GoozmaSystem.GoozmaEgg(new Vector2(center * 16, (top - 1) * 16));

                    else if (!Main.slimeRain)
                    {
                        if (player.ConsumeItem(ModContent.ItemType<GelatinousCatalyst>()))
                        {
                            foreach (Vector2 position in GoozmaSystem.slimeStatuePoints)
                            {
                                for (int r = 0; r < 5; r++)
                                {
                                    Dust d = Dust.NewDustDirect(position - new Vector2(16, 0), 32, 38, DustID.TintableDust, 0, -Main.rand.NextFloat(2f, 5f), 160, new Color(200, 200, 255), 1f + Main.rand.NextFloat());
                                    d.noGravity = true;
                                }
                            }

                            for (int r = 0; r < 15; r++)
                            {
                                Dust d = Dust.NewDustDirect(GoozmaSystem.ninjaStatuePoint - new Vector2(32, 4), 64, 92, DustID.TintableDust, 0, -Main.rand.NextFloat(2f, 5f), 160, new Color(200, 200, 255), 1f + Main.rand.NextFloat());
                                d.noGravity = true;
                            }

                            Main.StartSlimeRain(true);
                            NetMessage.SendData(MessageID.SetMiscEventValues);

                            SoundStyle spawnSound = AssetDirectory.Sounds.SlimeRainActivate;
                            SoundEngine.PlaySound(spawnSound, new Vector2(center * 16, (top - 1) * 16));
                        }
                    }
                }
                else
                {
                    Color color = new Color(255, 255, 0);
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(failText.Value), color, player.whoAmI);
                    else
                        Main.NewText(failText.Value, color);
                }
            }

            return true;
        }
    }
}
