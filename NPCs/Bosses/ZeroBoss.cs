using System;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Drakengard3Mod.Projectiles;

namespace Drakengard3Mod.NPCs.Bosses
{
    public class ZeroBoss : ModNPC
    {

        private int aiState = 0;
        private int LeaveState = 5;
        private int leaveDirection = 1;
        private int attackTimer = 0;
        private const float MoveSpeed = 3f;
        private const float AttackDistance = 64f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 56;

            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 5000;

            NPC.aiStyle = -1;

            NPC.knockBackResist = 0f;

            NPC.noGravity = true;
            NPC.noTileCollide = false;

            NPC.boss = true;

            NPC.value = Item.buyPrice(0, 5);

            NPC.lavaImmune = true;
        }

        public override void AI()
        {
            NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
            {
                if (aiState != LeaveState)
                {
                    leaveDirection = (NPC.Center.X < player.Center.X) ? -1 : 1;
                    aiState = LeaveState;
                    attackTimer = 0;
                }
                Leave();
                return;
            }

            // プレイヤーの方向を向く
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            switch (aiState)
            {
                case 0://start
                    SpawnSequence(player);
                    break;

                case 1://chase
                    ChasePlayer(player);
                    break;
                case 2://attack
                    PrepareAttack();
                    break;
                case 3://slash
                    SlashAttack();
                    break;
                case 4://
                    Recover();
                    break;
                case 5:
                    Leave();
                    break;

            }

        }

        //出現演出
        private void SpawnSequence(Player player)
        {
            attackTimer++;

            // 漂う
            NPC.velocity = Vector2.Zero;

            // 光の演出
            Lighting.AddLight(NPC.Center, 1.5f, 1.3f, 1.3f);

            if (attackTimer < 180)
            {
                NPC.velocity = Vector2.Zero;
                return;
            }

            // 降下
            NPC.velocity = new Vector2(0, 2.13f);

            if (NPC.collideY)
            {
                NPC.velocity = Vector2.Zero;
                NPC.noGravity = false;
                //NPC.noTileCollide = false;

                attackTimer = 0;
                aiState = 1;
            }
        }

        //おっかけ
        private void ChasePlayer(Player player)
        {
            float distance = Math.Abs(player.Center.X - NPC.Center.X);

            if (distance > AttackDistance)
            {
                NPC.velocity.X =
                    player.Center.X > NPC.Center.X ?
                    MoveSpeed :
                    -MoveSpeed;
            }
            else
            {
                NPC.velocity.X = 0;
                attackTimer = 0;
                aiState = 2;
            }

            // 壁ならジャンプ
            if (NPC.collideX && NPC.velocity.Y == 0)
            {
                NPC.velocity.Y = -12f;
            }
        }

        //攻撃開始
        private void PrepareAttack()
        {
            // 空中なら着地するまで待つ
            if (NPC.velocity.Y != 0f)
            {
                NPC.velocity.X = 0;
                return;
            }

            NPC.velocity.X = 0;

            attackTimer++;

            if (attackTimer >= 20)
            {
                attackTimer = 0;
                aiState = 3;
            }
        }

        //斬撃
        private void SlashAttack()
        {
            NPC.velocity.X = 0;

            attackTimer++;

            if (attackTimer == 6)
            {
                string text = Main.rand.Next(3) switch
                {
                    0 => "死ねい！",
                    1 => "逃すか！",
                    _ => "覚悟しろ！"
                };

                CombatText.NewText(
                    NPC.Hitbox,
                    Color.Red,
                    text
                    );
                
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<Projectiles.ZeroSlashProjectile>(),
                    NPC.damage,
                    2f,
                    Main.myPlayer,
                    NPC.whoAmI
                );
            }

            if (attackTimer >= 12)
            {
                attackTimer = 0;
                aiState = 4;
            }
        }

        //攻撃後硬直
        private void Recover()
        {
            NPC.velocity.X = 0;

            attackTimer++;

            if (attackTimer >= 20)
            {
                attackTimer = 0;
                aiState = 1;
            }
        }

        private void Leave()
        {
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.direction = leaveDirection;
            NPC.spriteDirection = leaveDirection;

            // 
            NPC.position.X += leaveDirection * 2f;
            NPC.velocity = Vector2.Zero;

            float distance = Vector2.Distance(NPC.Center, Main.LocalPlayer.Center);

            if (distance > 400f)
            {
                NPC.alpha += 4;

                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }

        }
        public override void FindFrame(int frameHeight)
        {
            bool walking = Math.Abs(NPC.velocity.X)> 0.1f || aiState == LeaveState;

            // 止まっている
            if (!walking)
            {
                NPC.frame.Y = 0;
                NPC.frameCounter = 0;
                return;
            }

            // 歩きアニメ
            NPC.frameCounter++;

            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;

                int frame = NPC.frame.Y / frameHeight;

                frame++;

                if (frame > 3)
                    frame = 1;

                NPC.frame.Y = frame * frameHeight;
            }
        }
    }
}