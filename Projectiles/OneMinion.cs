using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.Projectiles
{
    public class OneMinion : ModProjectile
    {
        private const float WalkSpeed = 2.5f;
        private const float JumpSpeed = -6f;
        private int attackCooldown = 0;
        private const int AttackDelay = 60;
        private bool hitWall = false;

        private enum ActionState
        {
            Idle,
            Walk,
            Attack
        }
        private ActionState state = ActionState.Idle;
        private int stateTimer = 0;
        private bool ringActive = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 25;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 56;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;

            Projectile.DamageType = DamageClass.Summon;

            Projectile.penetrate = -1;

            Projectile.timeLeft = 2;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.netImportant = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            var modPlayer = player.GetModPlayer<Players.OnePlayer>();

            NPC targetNPC = FindClosestEnemy(500f);
            bool hasTarget = targetNPC != null && targetNPC.active && !targetNPC.friendly;

            if (attackCooldown > 0)
                attackCooldown--;

            // 状態決定
            if (hasTarget)
            {
                state = ActionState.Attack;
            }
            else if (
                Math.Abs(Projectile.velocity.X) > 0.2f &&
                Projectile.velocity.Y == 0f)
            {
                state = ActionState.Walk;
            }
            else
            {
                state = ActionState.Idle;
            }

            // プレイヤー死亡時
            if (!player.active || player.dead)
            {
                modPlayer.oneMinion = false;
                return;
            }

            // 召喚維持
            if (modPlayer.oneMinion)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 targetPos = player.Center + new Vector2(-60 * player.direction, 0);

            float distanceToPlayer = Vector2.Distance(Projectile.Center, targetPos);

            //------------------------------------
            // 追従停止（敵がいれば止まる準備）
            //------------------------------------
            if (state == ActionState.Attack)
            {
                Projectile.velocity.X *= 0.8f;

                Projectile.direction =
                    Projectile.spriteDirection =
                    (targetNPC.Center.X > Projectile.Center.X) ? 1 : -1;

                if (!ringActive && attackCooldown == 0)
                {
                    TryAttack(targetNPC);
                }

            }
            else
            {
                MoveToPlayer(player, targetPos);
            }

            //------------------------------------
            // 重力
            //------------------------------------
            Projectile.velocity.Y += 0.4f;
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;

            //------------------------------------
            // 向き
            //------------------------------------
            if (Projectile.velocity.X > 0.1f)
                Projectile.direction = 1;
            else if (Projectile.velocity.X < -0.1f)
                Projectile.direction = -1;

            Projectile.spriteDirection = Projectile.direction;

            Animate();
        }

        //====================================
        // プレイヤー追従移動
        //====================================
        private void MoveToPlayer(Player player, Vector2 targetPos)
        {
            float distance = Vector2.Distance(Projectile.Center, targetPos);

            // ワープ
            if (distance > 900f)
            {
                Projectile.Center = targetPos;
                Projectile.velocity = Vector2.Zero;
                return;
            }

            //----------------------------------
            // 左右移動
            //----------------------------------

            if (targetPos.X > Projectile.Center.X + 20)
            {
                Projectile.velocity.X += 0.12f;
            }
            else if (targetPos.X < Projectile.Center.X - 20)
            {
                Projectile.velocity.X -= 0.12f;
            }
            else
            {
                Projectile.velocity.X *= 0.9f;
            }

            Projectile.velocity.X = MathHelper.Clamp(
                Projectile.velocity.X,
                -WalkSpeed,
                WalkSpeed);

            if (Projectile.velocity.X > 0.1f)
                Projectile.direction = 1;
            else if (Projectile.velocity.X < -0.1f)
                Projectile.direction = -1;

            Projectile.spriteDirection = Projectile.direction;
            //----------------------------------
            // 段差を登る
            //----------------------------------

            if (Math.Abs(Projectile.velocity.Y) < 0.01f &&
                CanStepUp())
            {
                Projectile.position.Y -= 16f;
            }
            //----------------------------------
            // 壁ジャンプ
            //----------------------------------

            if (hitWall)
            {
                hitWall = false;
                if (!CanStepUp())
                {
                    Projectile.velocity.Y = JumpSpeed;
                }

            }
        }
        //====================================
        // 1マス段差を登れるか
        //====================================
        private bool CanStepUp()
        {
            int dir = Projectile.direction;

            // 足元のタイル
            Point foot = new Point(
                (int)((Projectile.Center.X + dir * 18) / 16),
                (int)((Projectile.Bottom.Y) / 16));

            // 足元の一つ上
            Point upper = new Point(
                foot.X,
                foot.Y - 1);

            Tile footTile = Main.tile[foot.X, foot.Y];
            Tile upperTile = Main.tile[upper.X, upper.Y];

            bool lowerSolid =
                footTile != null &&
                footTile.HasTile &&
                Main.tileSolid[footTile.TileType];

            bool upperEmpty =
                upperTile == null ||
                !upperTile.HasTile ||
                !Main.tileSolid[upperTile.TileType];

            return lowerSolid && upperEmpty;
        }
        //====================================
        // 敵検索（簡易）
        //====================================
        private NPC FindClosestEnemy(float range)
        {
            NPC closest = null;
            float dist = range;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.CanBeChasedBy(this))
                    continue;

                float d = Vector2.Distance(Projectile.Center, npc.Center);

                if (d < dist)
                {
                    dist = d;
                    closest = npc;
                }
            }

            return closest;
        }
        private void TryAttack(NPC target)
        {
            attackCooldown = AttackDelay;
            ringActive = true;

            Vector2 dir = target.Center - Projectile.Center;
            dir.Normalize();

            float speed = 10f;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                dir * speed,
                ModContent.ProjectileType<Projectiles.OneRingProjectile>(),
                Projectile.damage,
                2f,
                Projectile.owner
            );

            //軽い演出

            string text = Main.rand.Next(3) switch
            {
                0 => "てぃ！",
                1 => "逃さない！",
                _ => "ころす！殺す殺す殺す！"
            };

            CombatText.NewText(Projectile.getRect(), Color.Yellow, text);
        }

        public void RingReturned()
        {
            ringActive = false;
        }

        //====================================
        // アニメーション
        //====================================
        private void Animate()
        {
            Projectile.frameCounter++;

            int frameSpeed = 8;

            if (state == ActionState.Idle)
            {
                if (Projectile.frameCounter >= frameSpeed)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame < 0 || Projectile.frame > 3)
                        Projectile.frame = 0;
                }
            }
            else if (state == ActionState.Walk)
            {
                if (Projectile.frameCounter >= frameSpeed)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame < 4 || Projectile.frame > 11)
                        Projectile.frame = 4;
                }
            }
            else if (state == ActionState.Attack)
            {
                if (Projectile.frameCounter >= frameSpeed)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame < 12 || Projectile.frame > 24)
                        Projectile.frame = 12;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.X != Projectile.velocity.X)
            {
                hitWall = true;
            }

            // 壁に当たっても消えない
            return false;
        }
    }
}