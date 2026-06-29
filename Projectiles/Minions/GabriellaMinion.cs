using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Drakengard3Mod.Buffs;
using Drakengard3Mod.Projectiles;
using Terraria.Audio;
using Terraria.ID;

namespace Drakengard3Mod.Projectiles.Minions
{
    public class GabriellaMinion : ModProjectile
    {
        private int attackTimer = 0;
        private const float DetectRange = 800f;

        private int attackAnimationTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 2;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage()
        {
            // ガブリエラ本体では攻撃しない
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // バフがある限り存在
            if (player.HasBuff(ModContent.BuffType<GabriellaBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            // プレイヤー追従
            Vector2 idlePosition = player.Center + 
                new Vector2(80f*player.direction, -80f);

            Vector2 toIdle = idlePosition - Projectile.Center;
            float distance = toIdle.Length();

            if (distance > 800f)
            {
                Projectile.Center = idlePosition;
                Projectile.velocity = Vector2.Zero;
            }

            if (distance > 20f)
            {
                toIdle.Normalize();

                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity,
                    toIdle * 10f,
                    0.08f);
            }
            else
            {
                Projectile.velocity *= 0.92f;
            }

            // 羽ばたき
            UpdateAnimation();

            // 攻撃
            AttackEnemy();
        }

        private void UpdateAnimation()
        {
            if (attackAnimationTimer > 0)
            {
                attackAnimationTimer--;
                Projectile.frame = 3;
                return;
            }

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
        }

        private void AttackEnemy()
        {
            NPC target = FindTarget();

            if (target == null)
            {
                attackTimer = 0;
                return;
            }

            attackTimer++;

            // 敵の方向を向く
            Projectile.spriteDirection =
                target.Center.X > Projectile.Center.X ? -1 : 1;

            if (attackTimer >= 45)
            {
                attackTimer = 0;

                Vector2 velocity = target.Center - Projectile.Center;
                velocity.Normalize();
                velocity *= 18f;

                attackAnimationTimer = 10;

                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot,Projectile.Center);

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<GabriellaBreath>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner);
            }
        }

        private NPC FindTarget()
        {
            NPC target = null;
            float distance = DetectRange;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(this))
                    continue;

                float d = Vector2.Distance(Projectile.Center, npc.Center);

                if (d < distance)
                {
                    distance = d;
                    target = npc;
                }
            }

            return target;
        }
    }
}