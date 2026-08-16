using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityHunt.Common;
using CalamityHunt.Common.Graphics;
using CalamityHunt.Common.Systems.Particles;
using CalamityHunt.Common.Utilities;
using CalamityHunt.Content.NPCs.Bosses.GoozmaBoss.Projectiles;
using CalamityHunt.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityHunt.Content.NPCs.Bosses.Lethe
{
    public class Lethe : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            /*
            database.FindEntryByNPCID(Type).UIInfoProvider = new HighestOfMultipleUICollectionInfoProvider(new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Goozma>()], true));
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement($"Mods.{nameof(CalamityHunt)}.Bestiary.Goozmite"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.SlimeRain,
                new SlimeMonsoonPortraitBackground()
            });
            */
        }

        public override void SetDefaults()
        {
            NPC.width = 39;
            NPC.height = 76;
            NPC.damage = 0;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCDeath9;
            NPC.DeathSound = SoundID.NPCDeath9;
            NPC.knockBackResist = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0;
            NPC.npcSlots = 255f;
            NPC.aiStyle = -1;
        }

        public enum LetheAttacks
        {
            Spawn = -2,
            Debug = -1,
            None = 0,

            Absolve = 255
        }

        public enum LethePhase
        {
            Debug = -1,
            None = 0,
            AttackTarget_Easy = 2,
            AttackTarget_Hard = 3,
            AttackTarget_Impossible = 4
        }

        public ref float Timer => ref NPC.ai[0];
        public ref float AttackTimer => ref NPC.ai[1];
        public LetheAttacks CurrentAttack
        {
            get => (LetheAttacks)NPC.ai[2];
            set => NPC.ai[2] = (float)value;
        }
        public LethePhase CurrentPhase
        {
            get => (LethePhase)NPC.ai[3];
            set => NPC.ai[3] = (float)value;
        }

        public enum FaceDirection
        {
            Right,
            Left,
            Forward
        }
        public FaceDirection SpriteDirection;

        public override void AI()
        {
            // no no no
            if (Main.netMode == NetmodeID.Server)
                NPC.active = false;

            NPC.target = Main.myPlayer;
            NPC.timeLeft = 9999999;

            CurrentAttack = LetheAttacks.Debug;
            switch (CurrentAttack)
            {
                case LetheAttacks.Spawn:
                    Attack_Spawn();
                    break;
                case LetheAttacks.Debug:
                    Attack_Debug();
                    break;
                case LetheAttacks.None:
                    Attack_None();
                    break;


                case LetheAttacks.Absolve:
                    Attack_Absolve();
                    break;
            }

            Timer++;
            AttackTimer++;
        }

        #region attacks
        public void Attack_Spawn()
        {

        }

        public void Attack_Debug()
        {
            if (Timer > 300)
            {
                Timer = 0;
            }
            else if (Timer > 200)
            {
                SpriteDirection = FaceDirection.Forward;
            }
            else if (Timer > 100)
            {
                SpriteDirection = FaceDirection.Right;
            }
            else
            {
                SpriteDirection = FaceDirection.Left;
            }
        }

        public void Attack_None()
        {
            
        }



        public void Attack_Absolve()
        {

        }
        #endregion

        public override bool CheckDead()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy) 
            {

            }

            if (SpriteDirection == FaceDirection.Forward)
                NPC.frame.Y = NPC.frame.Height;
            else
                NPC.frame.Y = 0;
        }

        public static Texture2D texArmTowardsCamera;
        public static Texture2D texArmAwayFromCamera;

        public override void Load()
        {
            texArmTowardsCamera = ModContent.Request<Texture2D>(Texture + "_ArmTowardsCamera", AssetRequestMode.ImmediateLoad).Value;
            texArmAwayFromCamera = ModContent.Request<Texture2D>(Texture + "_ArmAwayFromCamera", AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texBody = TextureAssets.Npc[Type].Value;

            SpriteEffects flip = SpriteDirection == FaceDirection.Left ? SpriteEffects.None: SpriteEffects.FlipHorizontally;
            int flipInt = SpriteDirection == FaceDirection.Left ? -1 : 1;
            int armOffset = 15;
            if (SpriteDirection == FaceDirection.Forward)
                armOffset += 5;

            float tempRotationShit = -Timer * 2 * 0.01f;

            if (SpriteDirection != FaceDirection.Forward)
                spriteBatch.Draw(texArmAwayFromCamera, NPC.Center + new Vector2(armOffset * flipInt, 10 - texArmAwayFromCamera.Height / 2) - screenPos, texArmAwayFromCamera.Frame(), drawColor, NPC.rotation + tempRotationShit, texArmAwayFromCamera.Size() / 2 - new Vector2(0, texArmAwayFromCamera.Height / 2), NPC.scale, flip, 0);
            spriteBatch.Draw(texBody, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, flip, 0);
            if (SpriteDirection != FaceDirection.Forward)
                spriteBatch.Draw(texArmTowardsCamera, NPC.Center + new Vector2(-armOffset * flipInt, 10 - texArmTowardsCamera.Height / 2) - screenPos, texArmTowardsCamera.Frame(), drawColor, NPC.rotation + tempRotationShit, texArmTowardsCamera.Size() / 2 - new Vector2(0, texArmTowardsCamera.Height / 2), NPC.scale, flip, 0);

            if (SpriteDirection == FaceDirection.Forward)
            {
                spriteBatch.Draw(texArmTowardsCamera, NPC.Center  + new Vector2(armOffset, 10 - texArmTowardsCamera.Height / 2) - screenPos, texArmTowardsCamera.Frame(), drawColor, NPC.rotation + tempRotationShit, texArmTowardsCamera.Size() / 2 - new Vector2(0, texArmTowardsCamera.Height / 2), NPC.scale, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(texArmTowardsCamera, NPC.Center + new Vector2(-armOffset, 10 - texArmTowardsCamera.Height / 2) - screenPos, texArmTowardsCamera.Frame(), drawColor, NPC.rotation + tempRotationShit, texArmTowardsCamera.Size() / 2 - new Vector2(0, texArmTowardsCamera.Height / 2), NPC.scale, 0, 0);
            }

            return false;
        }

    }
}
