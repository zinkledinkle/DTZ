using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Mycology.Systems
{
    public abstract class ShroomyDamage : DamageClass
    {
        protected virtual DamageClass ParentClass { get; }
        public override LocalizedText DisplayName => Language.GetOrRegister("Mods.Mycology.Shroomy" + ParentClass.Name, () => "[c/12bfff:Shroomy]" + ParentClass.DisplayName);
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == ParentClass) return StatInheritanceData.Full;
            return StatInheritanceData.None;            
        }
        public override bool GetEffectInheritance(DamageClass damageClass) => ParentClass == damageClass;
        public override bool GetPrefixInheritance(DamageClass damageClass) => ParentClass == damageClass;
    }
    public class ShroomyMelee : ShroomyDamage { protected override DamageClass ParentClass => Melee; }
    public class ShroomyRanged : ShroomyDamage { protected override DamageClass ParentClass => Ranged; }
    public class ShroomyMagic : ShroomyDamage { protected override DamageClass ParentClass => Magic; }
    public class ShroomySummon : ShroomyDamage { protected override DamageClass ParentClass => Summon; }
    public class PureShroomyDamage : ShroomyDamage { protected override DamageClass ParentClass => Generic; }
}
