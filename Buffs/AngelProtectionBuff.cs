using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Buffs
{
    public class AngelProtectionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}