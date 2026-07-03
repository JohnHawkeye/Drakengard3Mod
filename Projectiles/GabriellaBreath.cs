using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Drakengard3Mod.Projectiles
{
    public class GabriellaBreath : ModProjectile
    {
        // 残留炎のダメージ間隔
        private int damageTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.scale = 1f;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Summon;

            // ブロックを貫通
            Projectile.tileCollide = false;

            // 消えない
            Projectile.penetrate = -1;

            Projectile.ignoreWater = true;

            // 飛翔時間
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            // ============================
            // 飛翔中
            // ============================
            if (Projectile.ai[0] == 0)
            {
                Lighting.AddLight(
                    Projectile.Center,
                    0.6f,
                    0.2f,
                    0.8f);

                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustDirect(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        DustID.Shadowflame);

                    dust.noGravity = true;
                    dust.scale = 1.4f;
                    dust.velocity *= 0.2f;
                }

                Projectile.rotation += 0.25f;
                Lighting.AddLight(Projectile.Center, 0.5f, 0.15f, 0.8f);
            }

            // ============================
            // 残留炎
            // ============================
            else
            {
                float breath =
                    (float)System.Math.Sin(Main.GameUpdateCount * 0.10f) * 0.08f;

                float pulse =
                    0.85f + 0.15f *
                    (float)System.Math.Sin(Main.GameUpdateCount * 0.10f);

                Projectile.scale = 1.5f + breath;

                Projectile.velocity = Vector2.Zero;

                Projectile.rotation = 0f;

                Lighting.AddLight(
                    Projectile.Center,
                    0.8f * pulse,
                    0.2f * pulse,
                    1.1f * pulse);

                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustDirect(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        DustID.Shadowflame);

                    dust.noGravity = true;
                    dust.scale = 1.8f;
                    dust.velocity *= 0.05f;
                }

                Lighting.AddLight(Projectile.Center, 0.8f, 0.2f, 1.1f);

                if (Projectile.timeLeft < 30)
                {
                    float progress = 1f - Projectile.timeLeft / 30f;

                    Projectile.scale = MathHelper.Lerp(
                        1.5f,
                        2.0f,
                        progress);

                    Projectile.alpha = (int)MathHelper.Lerp(
                        0,
                        255,
                        progress);
                }
                damageTimer++;
            }
        }

        // 敵に命中
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 飛翔中のみ停止する
            if (Projectile.ai[0] == 0)
            {
                // 命中時に紫炎が弾ける
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        DustID.Shadowflame);

                    dust.noGravity = true;
                    dust.velocity *= 2f;
                    dust.scale = 1.8f;
                }

                Projectile.ai[0] = 1;
                Projectile.scale = 1.5f;
                Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft = 300;
                Projectile.netUpdate = true;

                Lighting.AddLight(Projectile.Center, 1.5f, 0.3f, 2.0f);
            }
        }

        // 残留炎でもダメージを与える
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 0)
                return true;

            // 残留炎は15フレーム毎だけ当たる
            return damageTimer >= 15;
        }

        public override void ModifyHitNPC(
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 1)
            {
                damageTimer = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Shadowflame);

                dust.noGravity = true;
                dust.scale = 2.3f;
                dust.velocity *= 3f;
            }

            SoundEngine.PlaySound(
                SoundID.Item74,
                Projectile.Center);
        }
    }
}