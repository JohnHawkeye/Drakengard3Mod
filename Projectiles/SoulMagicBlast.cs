using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Projectiles
{
    public class SoulMagicBlast : ModProjectile
    {
        // 追尾距離
        private const float DetectRadius = 700f;

        // 飛行速度
        private const float Speed = 12f;

        // 追尾性能（0.08くらいが自然）
        private const float HomingStrength = 0.08f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Magic;

            Projectile.penetrate = 1;

            Projectile.timeLeft = 600;

            // ブロック貫通
            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;

            Projectile.light = 0.6f;

            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            NPC target = FindTarget();

            if (target != null)
            {
                Vector2 desiredVelocity =
                    Projectile.DirectionTo(target.Center) * Speed;

                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    desiredVelocity,
                    HomingStrength);
            }

            // 常に進行方向を向く
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 魂っぽいエフェクト
            Dust dust = Dust.NewDustDirect(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.GemDiamond);

            dust.noGravity = true;
            dust.scale = 1.2f;
            dust.velocity *= 0.2f;
        }

        private NPC FindTarget()
        {
            NPC target = null;

            float distance = DetectRadius;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(this))
                    continue;

                float currentDistance =
                    Vector2.Distance(Projectile.Center, npc.Center);

                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    target = npc;
                }
            }

            return target;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow,300);

            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.GemDiamond);

                d.noGravity = true;
                d.scale = 1.4f;
            }
        }
    }
}