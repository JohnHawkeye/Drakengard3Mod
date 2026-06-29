using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Drakengard3Mod.NPCs.Bosses
{
    public abstract class HumanoidBossBase : ModNPC
    {
        protected Player Target => Main.player[NPC.target];

        protected float ArmRotation;
        protected float SwordRotation;

        protected int AttackTimer;
        protected int State;

        public override void AI()
        {
            if (!Target.active || Target.dead)
            {
                NPC.TargetClosest(false);
                return;
            }

            switch (State)
            {
                case 0:
                    Idle();
                    break;

                case 1:
                    Chase();
                    break;

                case 2:
                    PrepareAttack();
                    break;

                case 3:
                    SlashAttack();
                    break;

                case 4:
                    Recover();
                    break;
            }
        }

        protected virtual void Idle() { }

        protected virtual void Chase() { }

        protected virtual void PrepareAttack() { }

        protected virtual void SlashAttack() { }

        protected virtual void Recover() { }
    }
}