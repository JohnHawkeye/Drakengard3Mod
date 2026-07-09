using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Drakengard3Mod.Players;

namespace Drakengard3Mod.Projectiles
{
    public class BloodPickupProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 180; // 約3秒
        }

        public override void AI()
        {
            // 重力
            Projectile.velocity.Y += 0.20f;

            // 少し回転
            Projectile.rotation += Projectile.velocity.X * 0.05f;

            Player player = Main.LocalPlayer;

            float distance = Vector2.Distance(player.Center,Projectile.Center);
            if(distance < 80f)
            {
                Vector2 dir = player.Center -Projectile.Center;
                dir.Normalize();
                Projectile.velocity += dir * 0.4f;
            }

            if (player.active && !player.dead)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    player.GetModPlayer<DODPlayer>().AddBlood(5);

                    Projectile.Kill();
                }
            }
        }
    }
}