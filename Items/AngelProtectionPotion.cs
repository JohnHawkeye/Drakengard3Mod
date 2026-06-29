using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Items
{
    public class AngelProtectionPotion : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<Buffs.AngelProtectionCooldown>());
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(
                ModContent.BuffType<Buffs.AngelProtectionBuff>(),
                60 * 60
            );

            player.AddBuff(
                ModContent.BuffType<Buffs.AngelProtectionCooldown>(),
                60 * 180
            );

            return true;
        }
    }
}