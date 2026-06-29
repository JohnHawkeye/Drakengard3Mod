using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.Projectiles
{
    public class OneChakram : ModProjectile
    {
        private const float ReturnSpeed = 14f;
        private const float ReturnLerp = 0.12f;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 3;

            Projectile.timeLeft = 180;

            Projectile.tileCollide = true;

            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            // 回転
            Projectile.rotation += 0.4f;

            // 一定時間経過で帰還開始
            if (Projectile.timeLeft < 150)
            {
                Projectile.ai[1] = 1f;
            }

            // 帰還モード
            if (Projectile.ai[1] == 1f)
            {
                Player owner = Main.player[Projectile.owner];

                if (!owner.active || owner.dead)
                {
                    Projectile.Kill();
                    return;
                }

                Vector2 direction = owner.Center - Projectile.Center;
                float distance = direction.Length();

                // プレイヤーに戻った
                if (distance < 20f)
                {
                    Projectile.Kill();
                    return;
                }

                direction.Normalize();

                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    direction * ReturnSpeed,
                    ReturnLerp
                );
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 壁に当たったら帰還モードへ
            Projectile.ai[1] = 1f;

            // 少し減速
            Projectile.velocity *= 0.8f;

            // 消滅させない
            return false;
        }
    }
}