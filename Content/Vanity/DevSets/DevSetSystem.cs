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
            ModContent.ItemType<Zinkle.ZinkleLegs>()
        };
        private void IL_Player_TryGettingDevArmor(MonoMod.Cil.ILContext il)
        {
            try
            {
                var c = new ILCursor(il);

                if (!c.TryGotoNext(i => i.MatchCallvirt(typeof(Terraria.Utilities.UnifiedRandom).GetMethod("Next", BindingFlags.Instance | BindingFlags.Public)))) return;
                c.Index--;
                int count = c.Next.Operand is int operand ? operand : 0;
                c.Next.Operand = count + 1;
                if (!c.TryGotoNext(i => i.MatchSwitch(out _))) return;

                ILLabel zinkleLabel = c.DefineLabel(); //will add more later

                ILLabel[] labels = c.Next.Operand as ILLabel[];
                labels.Append(zinkleLabel);
                c.Next.Operand = labels;

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
            }
            catch (Exception e)
            {
                ModContent.GetInstance<DTZ>().Logger.Error($"Error in IL_Player.TryGettingDevArmor: {e}");
            }
        }
    }
}
