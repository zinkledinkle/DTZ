using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DTZ.Content.Vanity.DevSets
{
    public class DevSetSystem : ModSystem
    {
        public override void Load()
        {
            IL_Player.TryGettingDevArmor += IL_Player_TryGettingDevArmor;
        }
        public override void Unload()
        {
            IL_Player.TryGettingDevArmor -= IL_Player_TryGettingDevArmor;
        }
        private static readonly int[] ZinklePieces = new int[]
        {
            ModContent.ItemType<Zinkle.ZinkleHead>(),
            ModContent.ItemType<Zinkle.ZinkleBody>(),
            ModContent.ItemType<Zinkle.ZinkleLegs>(),
            ModContent.ItemType<Zinkle.ZinkleHat>(),
        };
        private static readonly int[] LucidPieces = new int[]
        {
            ModContent.ItemType<Lucid.CodeSoda>(),
        };
        private void IL_Player_TryGettingDevArmor(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);

                if (!c.TryGotoNext(i => i.MatchLdcI4(16))) return;
                c.Next.Operand = 1; //REMOVE THIS LATER

                for (int i = 0; i < 2; i++) 
                    if (!c.TryGotoNext(i => i.MatchCallvirt(typeof(Terraria.Utilities.UnifiedRandom).GetMethod("Next", BindingFlags.Instance | BindingFlags.Public, [typeof(int)])))) return;
                c.Index--;
                var countOperand = c.Next.Operand;
                int count = 0;
                if (c.Next.Operand is System.SByte b) count = ((byte)b); //very messy way of doing it probably but everything else crashed

                c.Next.Operand = count + 2;
                if (!c.TryGotoNext(i => i.MatchSwitch(out _))) return;

                ILLabel zinkleLabel = c.DefineLabel();
                ILLabel lucidLabel = c.DefineLabel();

                ILLabel[] labels = c.Next.Operand as ILLabel[];
                c.Next.Operand = labels.Append(zinkleLabel).Append(lucidLabel).ToArray();

                if (!c.TryGotoNext(i => i.MatchRet())) return;

                c.MarkLabel(zinkleLabel);

                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate((object self, IEntitySource source) =>
                {
                    Player plr = self as Player;
                    foreach (int itemType in ZinklePieces)
                    {
                        plr.QuickSpawnItem(source, itemType, 1);
                    }
                });
                c.Emit(OpCodes.Ret);

                c.MarkLabel(lucidLabel);

                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate((object self, IEntitySource source) =>
                {
                    Player plr = self as Player;
                    foreach (int itemType in LucidPieces) //was gonna do an array based system that added multiple labels based on the amount of sets in an int[][], but it was being really buggy and since theres only 2 this works fine for now
                    {
                        plr.QuickSpawnItem(source, itemType, 1);
                    }
                });
                c.Emit(OpCodes.Ret);
            }
            catch (Exception e)
            {
                ModContent.GetInstance<DTZ>().Logger.Error($"Error in IL_Player.TryGettingDevArmor: {e}");
                MonoModHooks.DumpIL(Mod, il);
            }
        }
    }
}
