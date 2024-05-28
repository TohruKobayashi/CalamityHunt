using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;


namespace CalamityHunt.Common.Systems
{
    public class DragonlensCamouflage : ModSystem
    {

        public static ILHook HideFromNPCSpawner;
        public static ILHook HideFromItemSpawner;
        public static ILHook HideFromProjectileSpawner;

        public static ILHook HideCalamityHuntFilterNPC;
        public static ILHook HideCalamityHuntFilterItem;
        public static ILHook HideCalamityHuntFilterProjectile;

        //Cant use load because dragonlens loads after
        public override void PostSetupContent()
        {
            if (!ModContent.GetInstance<GatekeepSystem>().Undercover || !ModLoader.HasMod("Dragonlens"))
                return;

            Assembly dragonlensCode = ModLoader.GetMod("DragonLens").Code;

            Type npcBrowser = dragonlensCode.GetType("DragonLens.Content.Tools.Spawners.NPCBrowser");
            if (npcBrowser != null)
            {
                HideFromNPCSpawner = new ILHook(npcBrowser.GetMethod("PopulateGrid"), HideCalHuntNPCsFromDragonLens);
                HideCalamityHuntFilterNPC = new ILHook(npcBrowser.GetMethod("SetupFilters"), HideCalamityHuntFilter);
            }

            Type itemBrowser = dragonlensCode.GetType("DragonLens.Content.Tools.Spawners.ItemBrowser");
            if (itemBrowser != null)
            {
                HideFromItemSpawner = new ILHook(itemBrowser.GetMethod("PopulateGrid"), HideCalHuntItemsFromDragonLens);
                HideCalamityHuntFilterItem = new ILHook(itemBrowser.GetMethod("SetupFilters"), HideCalamityHuntFilter);
            }

            Type projectileBrowser = dragonlensCode.GetType("DragonLens.Content.Tools.Spawners.ProjectileBrowser");
            if (projectileBrowser != null)
            {
                HideFromProjectileSpawner = new ILHook(projectileBrowser.GetMethod("PopulateGrid"), HideCalHuntProjectilesFromDragonLens);
                HideCalamityHuntFilterProjectile = new ILHook(projectileBrowser.GetMethod("SetupFilters"), HideCalamityHuntFilter);
            }
        }

        public delegate bool IsTypeModContent(int i);

        public void HideCalHuntNPCsFromDragonLens(ILContext il)
        {
            var npcs = Mod.GetContent<ModNPC>();

            modNPCStart = npcs.First().Type;
            modNPCEnd = modNPCStart + npcs.Count();
            HideCalHuntContentFromModLoader(il, IndexIsModNPC);
        }

        public void HideCalHuntItemsFromDragonLens(ILContext il)
        {
            var items = Mod.GetContent<ModItem>();

            modItemStart = items.First().Type;
            modItemEnd = modItemStart + items.Count();
            HideCalHuntContentFromModLoader(il, IndexIsModItem);
        }
        public void HideCalHuntProjectilesFromDragonLens(ILContext il)
        {
            var projs = Mod.GetContent<ModProjectile>();

            modProjectileStart = projs.First().Type;
            modProjectileEnd = modProjectileStart + projs.Count();
            HideCalHuntContentFromModLoader(il, IndexIsModProjectile);
        }


        public static int modNPCStart;
        public static int modNPCEnd;
        public bool IndexIsModNPC(int type)
        {
            if (type < modNPCStart)
                return false;
            if (type >= modNPCEnd)
                return false;

            return true;
        }

        public static int modItemStart;
        public static int modItemEnd;
        public bool IndexIsModItem(int type)
        {
            if (type < modItemStart)
                return false;
            if (type >= modItemEnd)
                return false;

            return true;
        }


        public static int modProjectileStart;
        public static int modProjectileEnd;
        public bool IndexIsModProjectile(int type)
        {
            if (type < modProjectileStart)
                return false;
            if (type >= modProjectileEnd)
                return false;

            return true;
        }

        public void HideCalHuntContentFromModLoader(ILContext il, IsTypeModContent check)
        {
            ILCursor cursor = new ILCursor(il);

            int buttonListIndex = 0;
            //Go get the list of buttons
            if (!cursor.TryGotoNext(MoveType.After,
                i => i.MatchNewobj(out _),
                i => i.MatchStloc(out buttonListIndex)
                ))
            {
                return;
            }

            int loopIteratorIndex = 1;
            if (!cursor.TryGotoNext(MoveType.After,
               i => i.MatchLdcI4(1),
               i => i.MatchStloc(out loopIteratorIndex)
               ))
            {
                return;
            }

            int modContentIndex = 2;

            //Go right before the content is added to the button list
            if (!cursor.TryGotoNext(MoveType.AfterLabel,
                i => i.MatchLdloc(buttonListIndex),
                i => i.MatchLdloc(out modContentIndex),
                i => i.MatchLdarg(0),
                i => i.MatchNewobj(out _),
                i => i.MatchCallvirt(out _)
                ))
            {
                return;
            }

            ILLabel endLoopLabel = cursor.DefineLabel();

            cursor.Emit(OpCodes.Ldloc, loopIteratorIndex);
            cursor.EmitDelegate(check);
            cursor.Emit(OpCodes.Brtrue, endLoopLabel);

            //This is the check that continues the loop
            if (!cursor.TryGotoNext(MoveType.Before,
                i => i.MatchLdloc(loopIteratorIndex),
                i => i.MatchLdcI4(1),
                i => i.MatchAdd(),
                i => i.MatchStloc(loopIteratorIndex)
                ))
            {
                return;
            }

            endLoopLabel.Target = cursor.Next;
        }



        public void HideCalamityHuntFilter(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            //Go to the loop for the mods
            if (!cursor.TryGotoNext(MoveType.After,
                i => i.MatchCall(typeof(ModLoader).GetMethod("get_Mods", BindingFlags.Static | BindingFlags.Public))
                ))
            {
                return;
            }

            ILLabel loopStartLabel = null;
            //Retrieve the part of the mod loop where it skips, so we can go there ourselves
            if (!cursor.TryGotoNext(MoveType.Before,
                i => i.MatchLdloc(0),
                i => i.MatchCallvirt(out _),
                i => i.MatchBrtrue(out loopStartLabel)
                ))
            {
                return;
            }

            ILLabel nextLoopStepLabel = cursor.DefineLabel();
            nextLoopStepLabel.Target = cursor.Next;

            //Go back to the start of the loop
            cursor.GotoLabel(loopStartLabel);
            if (!cursor.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(0),
                i => i.MatchCallvirt(out _),
                i => i.MatchStloc(1)
                ))
            {
                return;
            }

            //Load the current mod were enumerating through, check if its calamity hunt, skip to the next loop if it is
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.EmitDelegate(IsThatCalamityHunt);
            cursor.Emit(OpCodes.Brtrue, nextLoopStepLabel);
        }

        public bool IsThatCalamityHunt(Mod mod) => mod == Mod;

        public override void Unload()
        {        
            HideFromNPCSpawner?.Undo();
            HideFromItemSpawner?.Undo();
            HideFromProjectileSpawner?.Undo();

            HideCalamityHuntFilterNPC?.Undo();
            HideCalamityHuntFilterItem?.Undo();
            HideCalamityHuntFilterProjectile?.Undo();

        }
    }
}
