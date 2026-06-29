using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Items
{
    public class OneEarring : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Summon;

            Item.mana = 10;

            Item.width = 28;
            Item.height = 28;

            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.noMelee = true;
            Item.knockBack = 2f;

            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.LightPurple;

            Item.UseSound = SoundID.Item44;

            Item.buffType = ModContent.BuffType<Buffs.OneBuff>();

            Item.shoot = ModContent.ProjectileType<Projectiles.OneMinion>();

            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            if (player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    source,
                    Main.MouseWorld,
                    Vector2.Zero,
                    type,
                    damage,
                    knockback,
                    player.whoAmI
                );
            }

            return false;
        }
    }
}