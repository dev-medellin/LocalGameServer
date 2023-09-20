using COServer.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer.AttackHandler.Calculate
{
    public class Magic
    {
        public static void OnMonster(Role.Player player, MsgMonster.MonsterRole monster, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {


            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);


            if (monster.IsFloor)
            {
                SpellObj.Damage = 1;
                return;
            }

            SpellObj.Damage = player.Owner.AjustMagicAttack();
            if (player.Owner.Status.MagicPercent > 0)
            {
                SpellObj.Damage += (uint)((SpellObj.Damage * player.Owner.Status.MagicPercent / 100));
            }
            if (DBSpell != null)
                SpellObj.Damage += (uint)DBSpell.Damage;//(uint)((SpellObj.Damage * DBSpell.Damage) / 100);
            if (player.Level >= monster.Level)
                SpellObj.Damage = (uint)(SpellObj.Damage * 1.8);

            if (SpellObj.Damage > monster.Family.Defense)
                SpellObj.Damage -= monster.Family.Defense;
            else
                SpellObj.Damage = 1;


            if (Base.GetRefinery())
            {


                if (player.Owner.Status.SkillCStrike > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;

                    SpellObj.Damage += (SpellObj.Damage * (player.Owner.AjustMCriticalStrike() / 100)) / 100;
                }

            }


            SpellObj.Damage = (uint)Base.CalcDamageUser2Monster((int)SpellObj.Damage, monster.Family.Defense, player.Level, monster.Level, false);
            SpellObj.Damage = (uint)Base.AdjustMinDamageUser2Monster((int)SpellObj.Damage, player.Owner);

            SpellObj.Damage += player.Owner.Status.MagicDamageIncrease;

            if (monster.Family.Defense2 == 0)
                SpellObj.Damage = 1;

            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                SpellObj.Damage = 2000;
            //if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
            //    SpellObj.Damage /= 10;
            if (monster.Family.ID == 20211)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 4145)
            {
                player.Owner.OnAutoAttack = false;
                SpellObj.Damage = 100000;
            }
        }
        public static void OnPlayer(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {

            //SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            //PlayerToPlayer(player, target, out SpellObj, DBSpell.ID);
            //return;

            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
            {
                SpellObj.Damage = 1;
                return;
            }
            if (target.ContainFlag(MsgUpdate.Flags.MagicDefender))
            {
                SpellObj.Damage = 1;
                SpellObj.Effect = MsgAttackPacket.AttackEffect.Imunity;
                return;
            }
            uint ExtraDmg = 0;
            if (DBSpell != null)
                if (DBSpell.ID == 10310)
                {
                    switch (DBSpell.Level)
                    {
                        case 0: ExtraDmg = 2500; break;
                        case 1: ExtraDmg = 3500; break;
                        case 2: ExtraDmg = 5000; break;
                        case 3: ExtraDmg = 7000; break;
                        case 4: ExtraDmg = 10000; break;
                    }
                }
            SpellObj.Damage = player.Owner.Status.MagicAttack;
            if (DBSpell != null)
                SpellObj.Damage += (uint)(DBSpell.Damage + ExtraDmg);

            if (player.Owner.Status.MagicPercent > 0)
            {
                SpellObj.Damage = (uint)((SpellObj.Damage * (player.Owner.Status.MagicPercent) / 100));
            }

            SpellObj.Damage = Calculate.Base.CalculateBless(SpellObj.Damage, target.Owner.Status.ItemBless);

            uint MagicDefemce = target.Owner.Status.MagicDefence;
            uint MagicPercent = target.Owner.Status.MDefence;
            uint Penetration = player.Owner.Status.Penetration / 100;
            if (MagicPercent > Penetration)
                MagicPercent -= Penetration;
            else
                MagicPercent = 1;
            //uint power = MagicDefemce * MagicPercent / 100;
            MagicDefemce += MagicDefemce * MagicPercent / 100;
            // MagicDefemce -= (MagicDefemce * Penetration / 100);
            SpellObj.Damage = Calculate.Base.CalcaulateDeffence(SpellObj.Damage, MagicDefemce);

            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.MagicDamageIncrease, target.Owner.Status.MagicDamageDecrease);
            int reduction = Base.MulDiv((int)target.Owner.GemValues(Role.Flags.Gem.SuperTortoiseGem), 64, 100);

            SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, (int)(100 - Math.Min(67, reduction)), 100);

            //   SpellObj.Damage = (uint)Base.BigMulDiv(SpellObj.Damage, target.Owner.GetDefense2(), Client.GameClient.DefaultDefense2);
            // SpellObj.Damage = (uint)Base.CalculatePotencyDamage((int)SpellObj.Damage, player.BattlePower, target.BattlePower, true);

            uint m_strike = player.Owner.Status.SkillCStrike;
            if (m_strike > 0)
            {
                if (m_strike > target.Owner.Status.Immunity)
                {
                    double Power = (double)(m_strike - target.Owner.Status.Immunity);
                    Power = (double)(Power / 100);
                    if (Base.Success(Power))
                    {
                        SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                        SpellObj.Damage = (uint)Base.MulDiv((int)SpellObj.Damage, 130, 100);
                    }
                }
            }
            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
            {
                if (SpellObj.Damage > target.AzureShieldDefence)
                {
                    Calculate.AzureShield.CreateDmg(player, target, target.AzureShieldDefence);
                    target.RemoveFlag(MsgUpdate.Flags.AzureShield);
                    SpellObj.Damage -= target.AzureShieldDefence;

                }
                else
                {
                    target.AzureShieldDefence -= (ushort)SpellObj.Damage;
                    Calculate.AzureShield.CreateDmg(player, target, SpellObj.Damage);
                    SpellObj.Damage = 1;
                }
            }
            MsgSpellAnimation.SpellObj InRedirect;
            if (BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out InRedirect))
                SpellObj = InRedirect;
            if (target.Owner.Equipment.ShieldID != 0)
            {
                double Block = (target.Owner.Status.Block / 100);
                if (DateTime.Now < target.ShieldBlockEnd)
                    Block += ((target.ShieldBlockDamage) / 100);
                double Change = Math.Min(70.0, Block);

                if (Base.Success(Change))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.Block;
                    SpellObj.Damage /= 2;
                }
            }
        }
        public static void OnNpcs(Role.Player player, Role.SobNpc target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);

            SpellObj.Damage = player.Owner.Status.MagicAttack;
            if (player.Owner.Status.MagicPercent > 0)
            {
                SpellObj.Damage += (uint)((SpellObj.Damage * player.Owner.Status.MagicPercent / 100));
            }

            if (Base.GetRefinery())
            {

                if (player.Owner.Status.SkillCStrike > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;

                    SpellObj.Damage += (SpellObj.Damage * (player.Owner.Status.SkillCStrike / 100)) / 100;
                }

            }
            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.MagicDamageIncrease, 0);

            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
                SpellObj.Damage = 100;
        }

    }
}
