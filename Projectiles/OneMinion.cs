using System;
using System.Diagnostics;
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

        private int repositionTimer = 0;
        private int desiredSide = 1;
        private int retreatTimer = 0;

        private int talkCooldown = 0;

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

            if (repositionTimer > 0)
                repositionTimer--;

            if (attackCooldown > 0)
                attackCooldown--;

            if (repositionTimer == 0)
            {
                desiredSide = Main.rand.NextBool() ? 1 : -1;
                repositionTimer = Main.rand.Next(90, 180);
            }

            //トーク用のクールダウン
            if (talkCooldown > 0)
                talkCooldown--;

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


            Vector2 targetPos = player.Center + new Vector2(-player.direction * 50f, -4f);

            float distanceToPlayer = Vector2.Distance(Projectile.Center, targetPos);

            if (state == ActionState.Attack)
            {
                float distance = Vector2.Distance(
                    Projectile.Center,
                    targetNPC.Center);

                Projectile.direction =
                    Projectile.spriteDirection =
                    targetNPC.Center.X > Projectile.Center.X ? 1 : -1;

                if (distance > 64f)
                {
                    // 敵へ歩く
                    MoveToEnemy(targetNPC);
                }
                else
                {
                    // 停止
                    Projectile.velocity.X *= 0.8f;

                    if (!ringActive && attackCooldown == 0)
                    {
                        TryAttack(targetNPC);
                    }
                }
            }
            else
            {
                MoveToPlayer(player, targetPos);

                if (talkCooldown == 0)
                {
                    Talk("モンスターがいるぞ。気を付けろ。", Color.Yellow, 600);
                }
            }

            HandleJump(hasTarget ? targetNPC : null);

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
            const float TeleportDistance = 400f;

            // ワープ
            if (distance > TeleportDistance)
            {
                TeleportToPlayer(player);
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

        }
        private void TeleportToPlayer(Player player)
        {
            Vector2 pos = new Vector2(
                player.Center.X - player.direction * 50f,
                player.Bottom.Y - Projectile.height);

            while (Collision.SolidCollision(pos, Projectile.width, Projectile.height))
            {
                pos.Y -= 16f;
            }

            Projectile.position = pos;
            Projectile.velocity = Vector2.Zero;
        }
        private bool CanStepUp()
        {
            int dir = Projectile.direction;

            int x = (int)((Projectile.Center.X + dir * 18f) / 16);
            int y = (int)((Projectile.Bottom.Y - 2) / 16);

            Tile front = Main.tile[x, y];
            Tile upper = Main.tile[x, y - 1];

            if (front == null || upper == null)
                return false;

            bool frontSolid =
                front.HasTile &&
                Main.tileSolid[front.TileType];

            bool upperEmpty =
                !upper.HasTile ||
                !Main.tileSolid[upper.TileType];

            return frontSolid && upperEmpty;
        }

        //====================================
        // 前方に地面があるか
        //====================================
        private bool HasGroundAhead()
        {
            int dir = Projectile.direction;

            int x = (int)((Projectile.Center.X + dir * (Projectile.width / 2 + 8)) / 16);
            int y = (int)((Projectile.Bottom.Y + 16f) / 16);

            Tile tile = Main.tile[x, y];

            return tile != null &&
                   tile.HasTile &&
                   Main.tileSolid[tile.TileType];
        }

        private bool HasWallAhead()
        {
            int dir = Projectile.direction;

            // 前方の胴体付近を調べる
            int x = (int)((Projectile.Center.X + dir * 18f) / 16);
            int y = (int)((Projectile.Center.Y) / 16);

            Tile tile = Main.tile[x, y];

            return tile != null &&
                   tile.HasTile &&
                   Main.tileSolid[tile.TileType];

        }

        //====================================
        // 敵へ接近
        //====================================
        private void MoveToEnemy(NPC target)
        {
            if (retreatTimer > 0)
            {
                retreatTimer--;

                Projectile.velocity.X =
                    -Projectile.direction * 1.2f;

                return;
            }

            Vector2 targetPos = target.Center + new Vector2(desiredSide * 64f, 0);

            // 左右移動
            if (targetPos.X > Projectile.Center.X + 64f)
            {
                Projectile.velocity.X += 0.12f;
            }
            else if (targetPos.X < Projectile.Center.X - 64f)
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
        }

        private void HandleJump(NPC target)
        {
            if (Projectile.velocity.Y != 0)
                return;
            // 空中なら何もしない
            if (!IsGrounded())
                return;

            // if (CanStepUp())
            // {
            //     Projectile.position.Y = -4f;
            //     return;
            // }

            // 2マス以上の壁
            if (HasWallAhead())
            {
                Projectile.velocity.Y = JumpSpeed;

            }

            // 穴
            if (!HasGroundAhead())
            {
                Projectile.velocity.Y = JumpSpeed;
                return;
            }

            // 敵がかなり高い位置
            if (target != null &&
                target.Center.Y < Projectile.Center.Y - 48f)
            {
                Projectile.velocity.Y = JumpSpeed;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // タイルに当たっても消えない
            return false;
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

                if (d < dist &&
                    Collision.CanHit(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        npc.position,
                        npc.width,
                        npc.height
                    ))
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

            retreatTimer = 20;

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

        private bool IsGrounded()
        {
            return Projectile.velocity.Y == 0;
        }

        private void Talk(string text, Color color, int cooldown = 180)
        {
            if (talkCooldown > 0)
                return;

            Rectangle rect = new Rectangle(
                (int)Projectile.Center.X,
                (int)Projectile.Top.Y - 24,
                1,
                1);

            CombatText.NewText(rect, color, text);

            talkCooldown = cooldown;
        }
    }
}