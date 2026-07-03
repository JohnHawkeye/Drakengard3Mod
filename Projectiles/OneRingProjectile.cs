using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Projectiles
{
    public class OneRingProjectile : ModProjectile
    {
        private const float ReturnSpeed = 14f;

        private bool returning = false;
        private int flyTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;

            Projectile.penetrate = -1;

            Projectile.tileCollide = true;

            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.rotation += 0.7f;

            flyTimer++;

            // 約0.4秒飛んだら帰還開始
            if (flyTimer >= 25)
                returning = true;

            int ownerIndex = (int)Projectile.ai[0];

            if (ownerIndex < 0 || ownerIndex >= Main.maxProjectiles)
            {
                Projectile.Kill();
                return;
            }

            Projectile one = Main.projectile[ownerIndex];

            if (!one.active)
            {
                Projectile.Kill();
                return;
            }

            if (returning)
            {
                Vector2 targetPos = one.Center;

                Vector2 dir = targetPos - Projectile.Center;

                float distance = dir.Length();

                if (distance < 20f)
                {
                    Projectile.Kill();
                    return;
                }

                dir.Normalize();

                Projectile.velocity = dir * ReturnSpeed;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            returning = true;
            Projectile.tileCollide = false;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            int ownerIndex = (int)Projectile.ai[0];

            if (ownerIndex >= 0 &&
                ownerIndex < Main.maxProjectiles)
            {
                Projectile proj = Main.projectile[ownerIndex];

                if (proj.active &&
                    proj.ModProjectile is OneMinion minion)
                {
                    minion.RingReturned();
                }
            }
        }
    }
}