using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Items
{
    public class OneChakramItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;

            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.useStyle = ItemUseStyleID.Swing;

            Item.knockBack = 4f;
            Item.value = Item.buyPrice(copper: 10);
            Item.rare = ItemRarityID.Green;

            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.OneChakram>();
            Item.shootSpeed = 12f;

            Item.UseSound = SoundID.Item1;

            Item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}