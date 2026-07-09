using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Drakengard3Mod.Items.Weapons
{
    public class SoulCalibur : ModItem
    {
        private const int NormalDamage = 40;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = NormalDamage;
            Item.DamageType = DamageClass.Melee;

            Item.width = 64;
            Item.height = 64;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.knockBack = 8f;
            Item.autoReuse = true;

            Item.value = Item.buyPrice(gold: 50);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                // 輪廻の魂を持っていない
                if (!player.HasItem(ModContent.ItemType<ReincarnationSoul>()))
                    return false;

                Item.damage = 60;
            }
            else
            {
                Item.damage = NormalDamage;
            }

            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Vector2 direction = Main.MouseWorld - player.Center;
                direction.Normalize();

                Vector2 velocity = direction * 24f;

                int p = Projectile.NewProjectile(
                    player.GetSource_ItemUse(Item),
                    player.Center,
                    velocity,
                    ModContent.ProjectileType<Projectiles.SoulBurstSlash>(),
                    60,
                    2f,
                    player.whoAmI);

                Main.projectile[p].rotation = velocity.ToRotation();
                    
                player.ConsumeItem(
                    ModContent.ItemType<ReincarnationSoul>()
                );

                SoundEngine.PlaySound(SoundID.Item62, player.Center);

                CombatText.NewText(
                    player.Hitbox,
                    Color.Purple,
                    "SOUL BURST!"
                );
                SpawnSoulBurstEffect(player);
            }

            return true;
        }

        private void SpawnSoulBurstEffect(Player player)
        {
            Vector2 center = player.Center;

            // 外側の紫炎リング
            for (int i = 0; i < 50; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f;

                Vector2 velocity =
                    angle.ToRotationVector2() *
                    Main.rand.NextFloat(2f, 6f);

                int dust = Dust.NewDust(
                    center,
                    1,
                    1,
                    DustID.PurpleTorch,
                    velocity.X,
                    velocity.Y,
                    150,
                    default,
                    2f
                );

                Main.dust[dust].noGravity = true;
            }

            // 剣と体から立ち上る闇の炎
            for (int i = 0; i < 25; i++)
            {
                int dust = Dust.NewDust(
                    player.position,
                    player.width,
                    player.height,
                    DustID.Shadowflame
                );

                Main.dust[dust].velocity *= 0.3f;
                Main.dust[dust].scale = 1.8f;
                Main.dust[dust].noGravity = true;
            }

            // 紫の発光
            Lighting.AddLight(
                player.Center,
                0.7f,
                0.1f,
                0.9f
            );
        }
    }
}