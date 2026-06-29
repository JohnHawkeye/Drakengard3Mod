using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Drakengard3Mod.Items;
using Drakengard3Mod.Items.Armor;
//test
namespace Drakengard3Mod.NPCs
{
    [AutoloadHead]
    public class One : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;

            NPCID.Sets.ExtraFramesCount[Type] = 0;
            NPCID.Sets.AttackFrameCount[Type] = 1;
            NPCID.Sets.DangerDetectRange[Type] = 500;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 30;
            NPCID.Sets.AttackAverageChance[Type] = 10;
            NPCID.Sets.HatOffsetY[Type] = 4;
            
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;

            NPC.width = 20;
            NPC.height = 20;

            NPC.aiStyle = NPCAIStyleID.Passive;

            NPC.defense = 35;
            NPC.lifeMax = 300;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;

        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            int soulType = ModContent.ItemType<ReincarnationSoul>();
            for(int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player !=null && player.active && player.HasItem(soulType))
                {
                    return true;
                }
            }
            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "ワン姉さん"
            };
        }

        public override string GetChat()
        {
            return Main.rand.Next(3) switch
            {
                0 => "ゼロを倒すために君にはもっと強くなってもらわなくてはならない。",
                1 => "この世界のボスを倒して強くなるんだ。",
                2 => "夜になったら帰ってくるんだ。深い意味は…ない。",
                _ => "あまり…遠くに行かないでくれ…。"
            };
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "お買い物";
            button2 = "交換";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = "Shop";
            }
            else
            {
                Player player = Main.LocalPlayer;

                int soulType = ModContent.ItemType<ReincarnationSoul>();

                if (player.CountItem(soulType) >= 10)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        player.ConsumeItem(soulType);
                    }

                    player.QuickSpawnItem(
                        player.GetSource_GiftOrReward(),
                        ModContent.ItemType<AngelProtectionPotion>()
                    );

                    Main.npcChatText =
                        "輪廻の魂10個と引き換えに天使の保護ポーションを渡そう。";
                }
                else
                {
                    Main.npcChatText =
                        "輪廻の魂が10個必要だ。";
                }
            }
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type);
            npcShop.Add(ItemID.WoodenArrow);
            npcShop.Add(ItemID.WoodenBow);
            npcShop.Add(ItemID.TinBow);
            npcShop.Add(ModContent.ItemType<AtonementRag>());
            npcShop.Add(ModContent.ItemType<OneChakramItem>());
            npcShop.Add(ModContent.ItemType<OneCosAWig>());
            npcShop.Add(ModContent.ItemType<OneCosACoat>());
            npcShop.Add(ModContent.ItemType<OneCosABoots>());
            npcShop.Add(ModContent.ItemType<OneCosBWig>());
            npcShop.Add(ModContent.ItemType<OneCosBCoat>());
            npcShop.Add(ModContent.ItemType<OneCosBBoots>());
            npcShop.Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 5;
            randExtraCooldown = 10;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<Projectiles.OneChakram>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(
            ref float multiplier,
            ref float gravityCorrection,
            ref float randomOffset)
        {
            multiplier = 7f;
        }
    }
}