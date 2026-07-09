using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Projectiles
{
    public class SoulBurstSlash : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Melee;

            Projectile.penetrate = 3;

            Projectile.timeLeft = 120;

            Projectile.tileCollide = true;

            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // 花びら
            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.PinkTorch);

                dust.noGravity = true;
                dust.scale = 1.3f;
                dust.velocity *= 0.2f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.PinkTorch);

                dust.noGravity = true;
                dust.scale = 1.5f;
            }
        }
    }
}