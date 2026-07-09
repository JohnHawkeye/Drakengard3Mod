using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Drakengard3Mod.Projectiles;

namespace Drakengard3Mod.Items.Weapons
{
    public class SoulStaff : ModItem
    {

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;

            Item.width = 40;
            Item.height = 40;

            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.useStyle = ItemUseStyleID.Shoot;

            Item.noMelee = true;
            Item.knockBack = 3f;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.LightRed;

            Item.UseSound = SoundID.Item43;

            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<SoulMagic>();
            Item.shootSpeed = 12f;

            Item.mana = 8;
            Item.staff[Item.type]= true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.mana = 0;
                Item.shoot = ModContent.ProjectileType<SoulMagicBlast>();
            }
            else
            {
                Item.mana = 8;
                Item.shoot = ModContent.ProjectileType<SoulMagic>();
            }

            return true;
        }

        public override bool Shoot(Player player,
            Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Microsoft.Xna.Framework.Vector2 position,
            Microsoft.Xna.Framework.Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                int soulItem = ModContent.ItemType<ReincarnationSoul>();

                if (!player.HasItem(soulItem))
                    return false;

                player.ConsumeItem(soulItem);

                Projectile.NewProjectile(
                    source,
                    position,
                    velocity,
                    ModContent.ProjectileType<SoulMagicBlast>(),
                    damage,
                    knockback,
                    player.whoAmI
                );

                return false;
            }

            return true;
        }
    }
}