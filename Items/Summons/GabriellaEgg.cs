using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Drakengard3Mod.Buffs;

namespace Drakengard3Mod.Items.Summons
{
    public class GabriellaEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Summon;

            Item.width = 32;
            Item.height = 32;

            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.noMelee = true;
            Item.knockBack = 2f;

            Item.value = Item.buyPrice(0, 10);
            Item.rare = ItemRarityID.LightPurple;

            Item.UseSound = SoundID.Item44;

            Item.shoot = ModContent.ProjectileType<Projectiles.Minions.GabriellaMinion>();
            Item.buffType = ModContent.BuffType<GabriellaBuff>();

            Item.mana = 10;
        }

        public override bool Shoot(Player player,
            Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Microsoft.Xna.Framework.Vector2 position,
            Microsoft.Xna.Framework.Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            position = Main.MouseWorld;

            Projectile.NewProjectile(
                source,
                position,
                Microsoft.Xna.Framework.Vector2.Zero,
                type,
                damage,
                knockback,
                player.whoAmI);

            return false;
        }
    }
}