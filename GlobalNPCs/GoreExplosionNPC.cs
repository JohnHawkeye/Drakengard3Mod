using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.GlobalNPCs
{
    public class GoreExplosionNPC : GlobalNPC
    {
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            // 街NPCには適用しない
            if (npc.townNPC)
                return;

            // ===== 攻撃時の血しぶき =====

            int bloodAmount = Utils.Clamp(hit.Damage / 8, 3, 25);

            for (int i = 0; i < bloodAmount; i++)
            {
                int dust = Dust.NewDust(
                    npc.position,
                    npc.width,
                    npc.height,
                    DustID.Blood
                );

                Dust d = Main.dust[dust];

                d.velocity *= Main.rand.NextFloat(2f, 5f);
                d.velocity.Y -= Main.rand.NextFloat(1f, 3f);
                d.scale = Main.rand.NextFloat(1.1f, 2.0f);
                d.noGravity = false;
            }

            // 強い攻撃なら追加の血飛沫
            if (hit.Damage > 100)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.BloodWater
                    );

                    Main.dust[dust].velocity *= 4f;
                    Main.dust[dust].scale = 1.5f;
                }
            }

            // ===== 死亡時 =====

            if (npc.life <= 0)
            {
                // 大量の血しぶき
                for (int i = 0; i < 80; i++)
                {
                    int dust = Dust.NewDust(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.Blood
                    );

                    Dust d = Main.dust[dust];

                    d.velocity *= Main.rand.NextFloat(4f, 8f);
                    d.scale = Main.rand.NextFloat(1.5f, 3f);
                }

                // 血の霧
                for (int i = 0; i < 40; i++)
                {
                    int dust = Dust.NewDust(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.BloodWater
                    );

                    Dust d = Main.dust[dust];

                    d.velocity *= Main.rand.NextFloat(3f, 7f);
                    d.scale = Main.rand.NextFloat(1.5f, 2.5f);
                }

                // Terraria標準ゴア
                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = new Vector2(
                        Main.rand.NextFloat(-10f, 10f),
                        Main.rand.NextFloat(-12f, -2f)
                    );

                    string goreName = Main.rand.Next(3) switch
                    {
                        0 => "BloodChunk1",
                        1 => "BloodChunk2",
                        _ => "BloodChunk3"
                    };

                    Gore.NewGore(
                        npc.GetSource_Death(),
                        npc.Center,
                        velocity,
                        Mod.Find<ModGore>(goreName).Type
                    );
                }

                // ボスなら追加演出
                if (npc.boss)
                {
                    for (int i = 0; i < 150; i++)
                    {
                        int dust = Dust.NewDust(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.Blood
                        );

                        Dust d = Main.dust[dust];

                        d.velocity *= Main.rand.NextFloat(6f, 12f);
                        d.scale = Main.rand.NextFloat(2f, 4f);
                    }

                    for (int i = 0; i < 25; i++)
                    {
                        Vector2 velocity = new Vector2(
                            Main.rand.NextFloat(-15f, 15f),
                            Main.rand.NextFloat(-15f, -3f)
                        );

                        Gore.NewGore(
                            npc.GetSource_Death(),
                            npc.Center,
                            velocity,
                            Main.rand.Next(61, 64)
                        );
                    }
                }
            }
        }
    }
}