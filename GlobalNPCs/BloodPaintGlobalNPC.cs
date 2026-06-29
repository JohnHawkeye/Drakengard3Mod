using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Drakengard3Mod.GlobalNPCs
{
    public class BloodPaintGlobalNPC : GlobalNPC
    {
        // ==============================
        // 汚染処理本体
        // ==============================
        private void PaintArea(int centerX, int centerY, int radius, int intensity)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int i = centerX + x;
                    int j = centerY + y;

                    if (!WorldGen.InWorld(i, j))
                        continue;

                    // 距離で自然に減衰（円形っぽくなる）
                    float dist = x * x + y * y;
                    float maxDist = radius * radius;

                    if (dist > maxDist)
                        continue;

                    // 濃度判定（段階汚染）
                    if (Main.rand.NextFloat() > intensity / 10f)
                        continue;

                    // 地面ペイント
                    WorldGen.paintTile(i, j, PaintID.RedPaint);

                    // 壁ペイント（少し濃い）
                    if (Main.rand.NextBool(2))
                        WorldGen.paintWall(i, j, PaintID.DeepRedPaint);
                }
            }
        }

        // ==============================
        // 攻撃時・被弾時
        // ==============================
        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (npc.townNPC)
                return;

            int centerX = (int)(npc.Center.X / 16);
            int centerY = (int)(npc.Center.Y / 16);

            // --------------------------
            // 軽い攻撃汚染（小範囲）
            // --------------------------
            if (npc.life > 0)
            {
                int intensity = Utils.Clamp(hit.Damage / 20, 1, 3);

                if (Main.rand.NextBool(2))
                {
                    PaintArea(centerX, centerY, 1, intensity);
                }
            }

            // --------------------------
            // 死亡時（中範囲）
            // --------------------------
            if (npc.life <= 0)
            {
                PaintArea(centerX, centerY, 3, 6);
            }

            // --------------------------
            // 強攻撃（追加汚染）
            // --------------------------
            if (hit.Damage > 80)
            {
                PaintArea(centerX, centerY, 2, 4);
            }
        }

        // ==============================
        // 追加：死亡確定処理（保険）
        // ==============================
        public override void OnKill(NPC npc)
        {
            if (npc.townNPC)
                return;

            int centerX = (int)(npc.Center.X / 16);
            int centerY = (int)(npc.Center.Y / 16);

            // 確実に3タイル汚染
            PaintArea(centerX, centerY, 3, 8);
        }
    }
}