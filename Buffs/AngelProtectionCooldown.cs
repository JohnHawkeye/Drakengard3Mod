using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Buffs
{
    public class AngelProtectionCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}