using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Drakengard3Mod.Players
{
    public class AngelProtectionPlayer : ModPlayer
    {
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.HasBuff(ModContent.BuffType<Buffs.AngelProtectionBuff>()))
            {
                modifiers.FinalDamage *= 0f;
            }
        }
    }
}