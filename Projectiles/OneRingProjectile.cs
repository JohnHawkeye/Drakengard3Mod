using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Projectiles
{
    public class OneRingProjectile : ModProjectile
    {
        private const float Speed = 10f;
        private const float ReturnSpeed = 12f;

        private bool returning = false;

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
            Player player = Main.player[Projectile.owner];

            Vector2 ownerCenter = player.Center;

            Vector2 toOwner = ownerCenter - Projectile.Center;
            float distance = toOwner.Length();

            // 一定距離で帰還開始
            if (distance > 220f)
                returning = true;

            if (returning)
            {
                toOwner.Normalize();
                Projectile.velocity = toOwner * ReturnSpeed;

                // プレイヤーに戻ったら消える
                if (distance < 20f)
                    Projectile.Kill();

                return;
            }

            Projectile.rotation += 0.4f;

            Projectile.velocity *= 0.98f;
        }

        public override void OnKill(int timeLeft)
        {
            // ミニオン側に通知するため
            foreach (Projectile proj in Main.projectile)
            {
                if(!proj.active)
                    continue;
                
                if(proj.owner != Projectile.owner)
                    continue;

                if(proj.type != ModContent.ProjectileType<OneMinion>())
                    continue;
                
                OneMinion minion = proj.ModProjectile as OneMinion;

                if (minion != null)
                {
                    minion.RingReturned();
                }
            }
        }
    }
}