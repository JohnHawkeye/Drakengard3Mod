using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Buffs
{
    public class OneLoveBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 120;
        }
    }
}