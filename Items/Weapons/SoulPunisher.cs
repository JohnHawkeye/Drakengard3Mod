using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Drakengard3Mod.Projectiles;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;

namespace Drakengard3Mod.Items.Weapons
{
    public class SoulPunisher : ModItem
    {
        private const int Damage = 40;

        public override void SetDefaults()
        {
            Item.damage = Damage;
            Item.DamageType = DamageClass.Ranged;

            Item.width = 32;
            Item.height = 80;

            Item.useStyle = ItemUseStyleID.Shoot;

            Item.useTime = 8;
            Item.useAnimation = 8;

            Item.autoReuse = true;

            Item.useAmmo = AmmoID.Arrow;

            Item.shootSpeed = 18f;

            Item.shoot = ProjectileID.WoodenArrowFriendly;

            Item.noMelee = true;

            Item.UseSound = SoundID.Item5;
        }


        public override bool AltFunctionUse(Player player)
        {
            return true;
        }


        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                //右クリック

                if (player.CountItem(ModContent.ItemType<ReincarnationSoul>()) <= 0)
                {
                    return false;
                }

                Item.useTime = 16;
                Item.useAnimation = 16;
                Item.shootSpeed = 12f;
                Item.autoReuse = true;
                Item.useAmmo = AmmoID.None;
                Item.shoot =
                    ModContent.ProjectileType<SoulMissile>();


            }
            else
            {
                //左クリック
                Item.useAmmo = AmmoID.Arrow;
                Item.shoot = ProjectileID.WoodenArrowFriendly;

                Item.useTime = 16;
                Item.useAnimation = 16;

            }

            return true;
        }

        public override bool Shoot(
        Player player,
        Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
        Vector2 position,
        Vector2 velocity,
        int type,
        int damage,
        float knockback)
        {

            if (player.altFunctionUse == 2)
            {


                player.ConsumeItem(
                ModContent.ItemType<ReincarnationSoul>()
                );

                int numProj = 5;
                float rotation = MathHelper.ToRadians(10);

                SoundEngine.PlaySound(SoundID.Item92);
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        position,
                        8,
                        8,
                        DustID.BlueTorch
                    );

                    dust.velocity *= 2f;
                    dust.scale = 1.5f;
                    dust.noGravity = true;
                }

                for (int i = 0; i < numProj; i++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedBy(
                        MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1))
                    );

                    Projectile.NewProjectile(
                    source,
                    position,
                    velocity,
                    ModContent.ProjectileType<SoulMissile>(),
                    damage,
                    knockback,
                    player.whoAmI
                );
                }



                return false;
            }

            return true;
        }
    }
}