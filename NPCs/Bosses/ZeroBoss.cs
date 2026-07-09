using System;
using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Drakengard3Mod.Projectiles;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace Drakengard3Mod.NPCs.Bosses
{
    public class ZeroBoss : ModNPC
    {
        //phases
        private int aiState = 0;
        private const int SpawnState = 0;
        private const int ChaseState = 1;
        private const int DashPrepareState = 2;
        private const int DashState = 3;
        private const int SlashState = 4;
        private const int RecoverState = 5;
        private const int LeaveState = 6;
        private const int TeleportJumpState = 7;
        private const int TeleportMoveState = 8;
        private const int TeleportSlashState = 9;


        private int leaveDirection = 1;
        private int attackTimer = 0;

        private const float MoveSpeed = 3f;
        private const float DashSpeed = 14f;
        private const float AttackDistance = 64f;
        private const float AttackHeight = 48f;
        private bool IsEnraged => NPC.life <= NPC.lifeMax * 0.3;

        //cool timers
        private int dashCooldown = 0;
        private int teleportCooldown = 300;
        private const int TeleportCooldownMax = 60 * 5;
        private bool introDialogueShown = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;

            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
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

            //cooldown
            if (dashCooldown > 0)
                dashCooldown--;

            if (teleportCooldown > 0)
                teleportCooldown--;

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
            if (aiState != DashState)
            {
                NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
            }

            switch (aiState)
            {
                case SpawnState:
                    SpawnSequence(player);
                    break;

                case ChaseState:
                    ChasePlayer(player);
                    break;

                case DashPrepareState:
                    PrepareDash(player);
                    break;
                case DashState:
                    Dash(player);
                    break;

                case TeleportJumpState:
                    TeleportJump(player);
                    break;
                case TeleportMoveState:
                    TeleportMove(player);
                    break;
                case TeleportSlashState:
                    TeleportSlash();
                    break;


                case SlashState:
                    SlashAttack();
                    break;

                case RecoverState:
                    Recover();
                    break;
                case LeaveState:
                    Leave();
                    break;

            }
            Main.NewText($"State:{aiState}  Cool:{dashCooldown}");
        }

        //出現演出
        private void SpawnSequence(Player player)
        {
            Lighting.AddLight(NPC.Center, 1.5f, 1.3f, 1.3f);

            // まだ空中
            if (!NPC.collideY)
            {
                NPC.velocity = new Vector2(0, 2.13f);
                return;
            }



            // 着地後
            NPC.velocity = Vector2.Zero;
            NPC.noGravity = false;

            if (!introDialogueShown)
            {
                introDialogueShown = true;
                CombatText.NewText(
                    NPC.Hitbox,
                    Color.HotPink,
                    "君を殺しに来た。覚悟しろ！");

                for (int i = 0; i < 40; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        DustID.PinkTorch);

                    dust.velocity = new Vector2(
                        Main.rand.NextFloat(-3f, 3f),
                        Main.rand.NextFloat(-4f, -2f));

                    dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                    dust.noGravity = true;
                    dust.fadeIn = 1.2f;
                }
            }

            attackTimer++;

            if (attackTimer >= 180)
            {
                attackTimer = 0;
                aiState = ChaseState;
            }
        }


        //おっかけ
        private void ChasePlayer(Player player)
        {
            float xDistance = Math.Abs(player.Center.X - NPC.Center.X);
            float yDistance = Math.Abs(player.Center.Y - NPC.Center.Y);
            float distance = Vector2.Distance(NPC.Center, player.Center);

            bool blocked = !Collision.CanHitLine(
                NPC.position,
                NPC.width,
                NPC.height,
                player.position,
                player.width,
                player.height);

            bool tooFar = distance > 500f;

            // プレイヤーが下にいて、足元がプラットフォームなら降りる
            Point tilePos = NPC.Bottom.ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(tilePos.X, tilePos.Y);

            // テレポートスラッシュ
            if ((blocked || tooFar) && teleportCooldown <= 0)
            {
                NPC.velocity = Vector2.Zero;
                attackTimer = 0;
                aiState = TeleportJumpState;
                return;
            }

            if (tile.HasTile &&
                TileID.Sets.Platforms[tile.TileType] &&
                player.Center.Y > NPC.Center.Y + 32f)
            {
                NPC.noTileCollide = true;
            }
            else
            {
                NPC.noTileCollide = false;
            }

            if (blocked && teleportCooldown <= 0)
            {
                attackTimer = 0;
                aiState = TeleportJumpState;
                return;
            }

            if (xDistance <= AttackDistance && yDistance <= AttackHeight)
            {
                NPC.velocity.X = 0;
                attackTimer = 0;

                if (IsEnraged && dashCooldown <= 0)
                {
                    aiState = DashPrepareState;
                }
                else
                {
                    aiState = SlashState;
                }

                return;
            }

            NPC.velocity.X =
                    player.Center.X > NPC.Center.X ?
                    MoveSpeed : -MoveSpeed;

            // 高い場所にいるプレイヤーを追う
            if (player.Center.Y < NPC.Center.Y - 40f &&
                NPC.velocity.Y == 0)
            {
                NPC.velocity.Y = -10f;
            }
            // 壁ならジャンプ
            if (NPC.collideX && NPC.velocity.Y == 0)
            {
                NPC.velocity.Y = -6f;
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
                    0 => "死なす！",
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
                aiState = RecoverState;
            }
        }

        //dash slash
        private void PrepareDash(Player player)
        {
            NPC.velocity = Vector2.Zero;

            attackTimer++;

            if (attackTimer == 1)
            {
                // ヘルメスブーツ風SE
                SoundEngine.PlaySound(SoundID.Run, NPC.Center);
            }
            // 花びらを少し散らす
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    DustID.PinkTorch);

                dust.noGravity = true;
                dust.scale = 1.2f;
            }

            if (attackTimer >= 60)
            {
                attackTimer = 0;
                NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
                dashCooldown = IsEnraged ? 60 * 10 : 60 * 20;
                aiState = DashState;
            }
        }
        private void Dash(Player player)
        {

            NPC.velocity.X = NPC.direction * DashSpeed;

            NPC.velocity.Y = 0;

            float xDistance = Math.Abs(player.Center.X - NPC.Center.X);
            float yDistance = Math.Abs(player.Center.Y - NPC.Center.Y);

            bool playerInFront =
                (NPC.direction == 1 && player.Center.X > NPC.Center.X) ||
                (NPC.direction == -1 && player.Center.X < NPC.Center.X);
            if (playerInFront &&
                xDistance <= AttackDistance &&
                yDistance <= AttackHeight)
            {
                NPC.velocity = Vector2.Zero;
                attackTimer = 0;
                aiState = SlashState;
                return;
            }

            attackTimer++;

            // 外したら終了
            if (attackTimer >= 90)
            {
                attackTimer = 0;
                aiState = RecoverState;
            }
        }

        //teleport jump
        private void TeleportJump(Player player)
        {
            if (attackTimer == 0)
            {
                NPC.velocity.Y = -8f;
                NPC.velocity.X = 0;

                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        DustID.PinkTorch);

                    dust.velocity = new Vector2(
                        Main.rand.NextFloat(-3f, 3f),
                        Main.rand.NextFloat(-4f, -1f));

                    dust.noGravity = true;
                    dust.scale = 1.5f;
                }
            }

            attackTimer++;

            if (attackTimer >= 12)
            {
                attackTimer = 0;
                aiState = TeleportMoveState;
            }
        }
        private void TeleportMove(Player player)
        {
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    DustID.PinkTorch);

                dust.velocity *= 2f;
                dust.noGravity = true;
            }

            NPC.noTileCollide = true;
            NPC.alpha = 255;

            float side = player.direction == 1 ? -64f : 64f;

            Vector2 target =
                player.Center +
                new Vector2(side, -64f);

            NPC.Center = target;

            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    NPC.position,
                    NPC.width,
                    NPC.height,
                    DustID.PinkTorch);

                dust.velocity =
                    Main.rand.NextVector2Circular(3f, 3f);

                dust.noGravity = true;
            }


            NPC.noTileCollide = false;
            NPC.alpha = 0;

            NPC.velocity.Y = 8f;

            NPC.direction =
                player.Center.X > NPC.Center.X ? 1 : -1;

            NPC.spriteDirection = NPC.direction;

            teleportCooldown = TeleportCooldownMax;

            attackTimer = 0;
            aiState = TeleportSlashState;
        }

        private void TeleportSlash()
        {
            attackTimer++;

            if (attackTimer == 4)
            {
                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<ZeroSlashProjectile>(),
                    NPC.damage,
                    2f,
                    Main.myPlayer,
                    NPC.whoAmI);
            }

            if (NPC.collideY)
            {

                SoundEngine.PlaySound(
                      SoundID.DD2_MonkStaffGroundImpact,
                      NPC.Center);

                for (int i = 0; i < 35; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        DustID.PinkTorch);

                    dust.velocity = new Vector2(
                        Main.rand.NextFloat(-5f, 5f),
                        Main.rand.NextFloat(-2f, 1f));

                    dust.noGravity = true;
                    dust.scale = 1.6f;
                }

                NPC.velocity = Vector2.Zero;
                attackTimer = 0;
                aiState = RecoverState;
            }
        }

        //攻撃後硬直
        private void Recover()
        {
            NPC.velocity = Vector2.Zero;

            attackTimer++;

            if (attackTimer >= 20)
            {
                attackTimer = 0;
                aiState = ChaseState;
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
            bool walking =
                Math.Abs(NPC.velocity.X) > 0.1f &&
                aiState != TeleportJumpState &&
                aiState != TeleportMoveState &&
                aiState != TeleportSlashState;

            walking |= aiState == LeaveState;

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

        public override bool PreDraw(
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            if (aiState == DashState)
            {
                Texture2D texture = TextureAssets.Npc[NPC.type].Value;

                Rectangle frame = NPC.frame;

                Vector2 origin = frame.Size() / 2f;

                for (int i = NPC.oldPos.Length - 1; i >= 0; i--)
                {
                    Vector2 drawPos =
                        NPC.oldPos[i] + NPC.Size / 2f - screenPos;

                    Color color = Color.HotPink * (0.5f - i * 0.06f);

                    SpriteEffects effects =
                        NPC.spriteDirection == -1 ?
                        SpriteEffects.None :
                        SpriteEffects.FlipHorizontally;

                    spriteBatch.Draw(
                        texture,
                        drawPos,
                        frame,
                        color,
                        NPC.rotation,
                        origin,
                        NPC.scale,
                        effects,
                        0f);
                }
            }

            return true;
        }
    }
}