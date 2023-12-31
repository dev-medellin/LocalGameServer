﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer.AttackHandler.Calculate
{
    public class Physical
    {
        public static void OnMonster(Role.Player player, MsgMonster.MonsterRole monster, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, byte MultipleDamage = 0)
        {

            SpellObj = new MsgSpellAnimation.SpellObj(monster.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (monster.IsFloor)
            {
                SpellObj.Damage = 2;
                return;
            }
            if (DBSpell == null)
            {
                if (Base.Dodged(player.Owner, monster))
                {
                    SpellObj.Damage = 0;
                    return;
                }

            }
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            Damage = (int)player.Owner.AjustMaxAttack((uint)Damage);

            var rawDefense = monster.Family.Defense;

            Damage = Math.Max(0, Damage - rawDefense);

            if (DBSpell != null && DBSpell.Damage < 10 && DBSpell.ID != 10490)
                DBSpell.Damage = 10;

            if (player.ContainFlag(MsgUpdate.Flags.FatalStrike))
            {
                if (monster.Family.ID == 4145)
                    Damage = 10000;
                else if (monster.Boss != 1)
                    Damage = Base.MulDiv((int)Damage, 500, 100);
                else
                    Damage = Base.MulDiv((int)Damage, 125, 100);

            }
            else if (MultipleDamage != 0)
            {
                Damage = Damage * MultipleDamage;
            }
            else
            {
                if (DBSpell != null && DBSpell.ID == 12770)
                {
                    if (monster.Boss == 1)
                        Damage = Base.MulDiv((int)Damage, 350, 100);
                    else
                        Damage = Base.MulDiv((int)Damage, 500, 100);
                }
                else
                {
                    if (DBSpell == null || (DBSpell.ID != 10490))
                        Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);
                }
            }
            if (player.ContainFlag(MsgUpdate.Flags.Oblivion))
            {
                player.OblivionMobs++;
                if (monster.Boss == 0)
                    Damage = Base.MulDiv((int)Damage, 200, 100);
                else
                    Damage = Base.MulDiv((int)Damage, 125, 100);

            }
            if (player.OblivionMobs > 32 && player.ContainFlag(MsgUpdate.Flags.Oblivion))
                player.RemoveFlag(MsgUpdate.Flags.Oblivion);
            Damage = Base.AdjustMinDamageUser2Monster(Damage, player.Owner);
            Damage = Base.CalcDamageUser2Monster(Damage, monster.Family.Defense, player.Level, monster.Level, false);
            Damage = (int)Base.BigMulDiv(Damage, monster.Family.Defense2, Client.GameClient.DefaultDefense2);
            if (monster.Family.Defense2 > 0)
                Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.AjustPhysicalDamageIncrease(), 0);
            SpellObj.Damage = (uint)Math.Max(1, Damage);
            if (monster.Boss == 0)
            {
                if (player.ContainFlag(MsgUpdate.Flags.Superman))
                    SpellObj.Damage *= 10;
            }
            else if (player.ContainFlag(MsgUpdate.Flags.Superman))
                SpellObj.Damage = (uint)(SpellObj.Damage * 1.5);
            if (Base.GetRefinery())
            {
                if (player.Owner.Status.CriticalStrike > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage += (SpellObj.Damage * (player.Owner.AjustCriticalStrike() / 100)) / 100;
                }
            }

            if ((monster.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                SpellObj.Damage /= 10;
            if (monster.Family.ID == 20211)
                SpellObj.Damage = 1;
            if (monster.Family.ID == 4145)
            {
                player.Owner.OnAutoAttack = false;
                SpellObj.Damage = 100000;
            }
            if (player.Owner.ProjectManager)
                SpellObj.Damage = 10000000;
        }
        public static void OnPlayer(Role.Player player, Role.Player target, Database.MagicType.Magic DBSpell, out MsgSpellAnimation.SpellObj SpellObj, bool StackOver = false, int IncreaseAttack = 0)
        {
            SpellObj = new MsgSpellAnimation.SpellObj(target.UID, 0, MsgAttackPacket.AttackEffect.None);
            if (target.ContainFlag(MsgUpdate.Flags.ShurikenVortex))
            {
                SpellObj.Damage = 1;
                return;
            }
            bool update = false;
            if (DBSpell == null)
            {
                if (Base.Dodged(player.Owner, target.Owner))
                {
                    SpellObj.Damage = 0;
                    return;
                }
            }
            if (DBSpell != null && DBSpell.ID == 10490)
                DBSpell = null;
            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);
            Damage = (int)player.Owner.AjustAttack((uint)Damage);
            var rawDefense = target.Owner.AjustDefense;
            if (Damage > rawDefense)
                Damage -= (int)rawDefense;
            else
                Damage = 1;
            if (DBSpell != null)
            {
                if (DBSpell.ID == 12080)
                {
                    if (Role.Core.GetDistance(player.X, player.Y, target.X, target.Y) <= 3)
                    {
                        Damage = Base.MulDiv((int)Damage, 135, 100);
                        update = true;
                    }
                }
                else if (DBSpell.ID == 12290)
                {
                    Damage = Base.MulDiv((int)Damage, 60, 100);
                    update = true;
                }
                else if (DBSpell.ID == 11050)
                {
                    Damage = Base.MulDiv((int)Damage, 50, 100);
                    update = true;
                }
            }
            if (!update)
            {
                if (DBSpell != null)
                    Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);

            }
            bool onbreak = false;
            update = false;
            if (player.Owner.Status.CriticalStrike > 0)
            {
                if (!update && Base.GetRefinery(player.Owner.Status.CriticalStrike / 100, target.Owner.Status.Immunity / 100))
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    Damage = Base.MulDiv((int)Damage, 120, 100);
                    update = true;
                }
            }
            if (!update && player.Owner.Status.Breakthrough > 0)
            {
                if (target.BattlePower > player.BattlePower)
                {
                    if (player.Owner.Status.Breakthrough > target.Owner.Status.Counteraction)
                    {
                        double Power = (double)(player.Owner.Status.Breakthrough - target.Owner.Status.Counteraction);
                        Power = (double)(Power / 10);
                        if (Base.Success(Power))
                        {
                            onbreak = true;
                            SpellObj.Effect |= MsgAttackPacket.AttackEffect.Break;

                        }
                    }
                }
            }


            double tortise_success = 0.60;
            if ((target.Class >= 21 && target.Class <= 25))
                tortise_success = 1;
            if ((target.Class >= 131 && target.Class <= 135))
                tortise_success = 1;
            var TortoisePercent = target.Owner.GemValues(Role.Flags.Gem.SuperTortoiseGem);
            if (TortoisePercent > 0)
                Damage -= (int)(Damage * Math.Min((int)TortoisePercent, 50) * tortise_success / 100);

            if (target.Reborn > 0)
                Damage = (int)Base.BigMulDiv((int)Damage, 7000, Client.GameClient.DefaultDefense2);
            byte Bless = 0;
            if (target.Class >= 141 && target.Class <= 145)
                Bless = 10;
            Damage -= (int)((Damage * (target.Owner.Status.ItemBless + Bless)) / 100);
            if (onbreak == false)
            {
                var olddamage = Damage;
                Damage = Database.Disdain.UserAttackUser(player, target, ref Damage);
                //if (Damage < olddamage)
                //{
                //    Damage += olddamage / 3;
                //}
            }
            Damage = (int)Calculate.Base.CalculateExtraAttack((uint)Damage, player.Owner.Status.PhysicalDamageIncrease, target.Owner.Status.PhysicalDamageDecrease);

            SpellObj.Damage = (uint)Math.Max(1, Damage);

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
            if (CheckAttack.BlockRefect.CanUseReflect(player.Owner))
            {
                if (!StackOver)
                {
                    MsgSpellAnimation.SpellObj InRedirect;
                    if (BackDmg.Calculate(player, target, DBSpell, SpellObj.Damage, out InRedirect))
                        SpellObj = InRedirect;
                }
            }
            if (target.Owner.Equipment.ShieldID != 0)
            {
                double Block = (target.Owner.Status.Block / 100);
                if (DateTime.Now < target.ShieldBlockEnd)
                    Block += target.ShieldBlockDamage;
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

            int Damage = (int)Base.GetDamage(player.Owner.Status.MaxAttack, player.Owner.Status.MinAttack);

            Damage = (int)player.Owner.AjustAttack((uint)Damage);

            Damage = Base.MulDiv((int)Damage, (int)((DBSpell != null) ? DBSpell.Damage : Program.ServerConfig.PhysicalDamage), 100);


            SpellObj.Damage = (uint)Math.Max(1, Damage);

            if (Base.GetRefinery())
            {
                if (player.Owner.Status.CriticalStrike > 0)
                {
                    SpellObj.Effect |= MsgAttackPacket.AttackEffect.CriticalStrike;
                    SpellObj.Damage = Base.CalculateArtefactsDmg(SpellObj.Damage, player.Owner.Status.CriticalStrike, 0);
                }
            }
            SpellObj.Damage = Calculate.Base.CalculateExtraAttack(SpellObj.Damage, player.Owner.Status.PhysicalDamageIncrease, 0);
            if (target.ContainFlag(MsgUpdate.Flags.AzureShield))
                SpellObj.Damage = 100;
        }
    }
}
