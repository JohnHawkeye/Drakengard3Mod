using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Buffs
{
    public class OneBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<Players.OnePlayer>().oneMinion = true;
            // ワン姉さんが存在している間はバフを維持
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.OneMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                // いなくなったらバフ解除
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}