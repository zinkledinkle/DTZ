using Mycology.Content.Projectiles;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Mycology.Content.Buffs
{

    // i could NOT be bothered to make 4 seperate .cs files when its lowkey like 8 lines of code per buff :broken heart:
    public abstract class MushionBuff : ModBuff
    {
        protected virtual int ProjectileType { get; }
        public override bool RightClick(int buffIndex)
        {
            Main.LocalPlayer.GetModPlayer<MycologyPlayer>().KillFirstMushion();
            return base.RightClick(buffIndex);
        }
    }
    public class GlowingBuff : MushionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.lifeRegen += 5 * player.ownedProjectileCounts[ProjectileType];
        }
        protected override int ProjectileType => ModContent.ProjectileType<GlowingMushion>();
    }

    public class HellcapBuff : MushionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.GetCritChance(DamageClass.Generic) += 2 * player.ownedProjectileCounts[ProjectileType];
        }
        protected override int ProjectileType => ModContent.ProjectileType<HellcapMushion>();
    }
    public class IceliumBuff : MushionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.DefenseEffectiveness *= 1f - (.075f *player.ownedProjectileCounts[ProjectileType]); 
        }
        protected override int ProjectileType => ModContent.ProjectileType<IceliumMushion>();
    }
    public class ToadBuff : MushionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.moveSpeed += .3f * player.ownedProjectileCounts[ProjectileType];
            player.jumpSpeedBoost += 1 * player.ownedProjectileCounts[ProjectileType];
        }
        protected override int ProjectileType => ModContent.ProjectileType<ToadMushion>();
    }
    public class NormieBuff : MushionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.statLifeMax2 += 10 * player.ownedProjectileCounts[ProjectileType];
        }
        protected override int ProjectileType => ModContent.ProjectileType<NormieMushion>();
    }
    public class OppsgothimBuff : MushionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.statLifeMax2 = 1;
        }
        protected override int ProjectileType => ModContent.ProjectileType<Oppsgothim>();
    }
    public class CryingToadBuff : MushionBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.statLifeMax2 = 1;
        }
        protected override int ProjectileType => ModContent.ProjectileType<CryingToadMushion>();
    }

}
