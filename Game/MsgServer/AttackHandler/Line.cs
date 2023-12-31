﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COServer.Game.MsgFloorItem;

namespace COServer.Game.MsgServer.AttackHandler
{
    public struct coords
    {
        public int X;
        public int Y;

        public coords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Line
    {


        public static List<coords> LineCoords(ushort userx, ushort usery, ushort shotx, ushort shoty, byte length)
        {
            //var ThisLine = new List<coords>();
            //int dx = shotx - userx, dy = shoty - usery, steps, k;
            //float xincrement, yincrement, x = userx, y = usery;

            //if (Math.Abs(dx) > Math.Abs(dy)) steps = Math.Abs(dx);
            //else steps = Math.Abs(dy);
            //xincrement = dx / (float)steps;
            //yincrement = dy / (float)steps;
            ////List<coords> ThisLine = new List<coords>();
            //ThisLine.Add(new coords() { X = (int)Math.Round(x), Y = Math.Round(y) });
            //for (k = 0; k < length; k++)
            //{
            //    x += xincrement;
            //    y += yincrement;
            //    ThisLine.Add(new coords() { X = (int)Math.Round(x), Y = (int)Math.Round(y) });

            //}
            //return ThisLine;
            double dir = Math.Atan2(shoty - usery, shotx - userx);
            double f_x = (Math.Cos(dir) * length) + userx;
            double f_y = (Math.Sin(dir) * length) + usery;

            return bresenham(userx, usery, (int)Math.Round(f_x), (int)Math.Round(f_y));
        }
        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }
        public static List<coords> bresenham(int x0, int y0, int x1, int y1)
        {
            List<coords> ThisLine = new List<coords>();

            //int dy = y1 - y0;
            //int dx = x1 - x0;
            //int stepx, stepy;

            //if (dy < 0) { dy = -dy; stepy = -1; } else { stepy = 1; }
            //if (dx < 0) { dx = -dx; stepx = -1; } else { stepx = 1; }
            //dy <<= 1;
            //dx <<= 1;

            //ThisLine.Add(x0, y0);
            //if (dx > dy)
            //{
            //    int fraction = dy - (dx >> 1);
            //    while (x0 != x1)
            //    {
            //        if (fraction >= 0)
            //        {
            //            y0 += stepy;
            //            fraction -= dx;
            //        }
            //        x0 += stepx;
            //        fraction += dy;
            //        ThisLine.Add(x0, y0);
            //    }
            //}
            //else
            //{
            //    int fraction = dx - (dy >> 1);
            //    while (y0 != y1)
            //    {
            //        if (fraction >= 0)
            //        {
            //            x0 += stepx;
            //            fraction -= dy;
            //        }
            //        y0 += stepy;
            //        fraction += dx;
            //        ThisLine.Add(x0, y0);
            //    }
            //}
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if (steep)
                    ThisLine.Add(y, x);
                else
                    ThisLine.Add(x, y);

                err = err - dY;
                if (err < 0) { y += ystep; err += dX; }
            }
            return ThisLine;
        }
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.FastBlader:
                    case (ushort)Role.Flags.SpellID.DragonTail:
                    case (ushort)Role.Flags.SpellID.ScrenSword:
                    case (ushort)Role.Flags.SpellID.ViperFang:
                        {
                            bool pass = false;

                            user.Player.TotalHits++;

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            byte Range = DBSpell.Range;
                            user.Player.Angle = Role.Core.GetAngle(user.Player.X, user.Player.Y, Attack.X, Attack.Y);
                            Range += 6;
                            //if (user.Player.Angle == Role.Flags.ConquerAngle.SouthWest ||
                            //    user.Player.Angle == Role.Flags.ConquerAngle.NorthEast ||
                            //    user.Player.Angle == Role.Flags.ConquerAngle.East ||
                            //    user.Player.Angle == Role.Flags.ConquerAngle.West ||
                            //    user.Player.Angle == Role.Flags.ConquerAngle.NorthWest)
                            //    Range += 3;
                            //else if (user.Player.Angle == Role.Flags.ConquerAngle.South)
                            //    Range += 4;
                            //else Range += 1;
                            //Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, (byte)(DBSpell.Range + 1), 0, ClientSpell.ID);
                            var LineRe = LineCoords(user.Player.X, user.Player.Y, Attack.X, Attack.Y, (byte)(Range));
                            byte LineRange = (byte)((ClientSpell.UseSpellSoul > 0) ? 0 : 0);

                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster).Where(e => LineRe.Contains((ushort)(e.X), (ushort)(e.Y))))
                            {

                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if ((attacked.Family.Settings & MsgMonster.MonsterSettings.Guard) == MsgMonster.MonsterSettings.Guard)
                                    continue;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            bool hitSomeone = false;
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player)
                                .Where(e => LineRe.Contains((ushort)(e.X), (ushort)(e.Y))))
                            {
                                var attacked = targer as Role.Player;
                                hitSomeone = true;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < DBSpell.Range)
                                    {
                                        if (user.Player.Class >= 11 && user.Player.Class <= 15)
                                        {
                                            user.Player.Stamina = (ushort)Math.Min((int)(user.Player.Stamina + 30), 150);
                                            user.Player.SendUpdate(stream, user.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                                        }
                                        if (!pass)
                                        {
                                            user.Player.Hits++;
                                            user.Player.Chains++;
                                            if (user.Player.Chains > user.Player.MaxChains)
                                                user.Player.MaxChains = user.Player.Chains;
                                            pass = true;
                                        }
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                                // else if (attacked.Name == "Pacific[PM]") Console.WriteLine($"Loc: ({X},{Y}) -- Attack:({Attack.X},{Attack.Y})");
                            }
                            if (!hitSomeone)
                                user.Player.Chains = 0;
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc).Where(e => LineRe.Contains((ushort)(e.X), (ushort)(e.Y))))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) <= DBSpell.Range)
                                {
                                    if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }

                    case (ushort)Role.Flags.SpellID.SpeedGun:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            byte Range = (byte)(Math.Min(7, (int)DBSpell.Range));
                            Algoritms.InLineAlgorithm Line = new Algoritms.InLineAlgorithm(user.Player.X, Attack.X, user.Player.Y, Attack.Y, user.Map, Range, 0, ClientSpell.ID);

                            byte LineRange = 1;
                            uint Experience = 0;
                            foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);

                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                            {
                                var attacked = targer as Role.Player;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                            {
                                var attacked = targer as Role.SobNpc;
                                if (Role.Core.GetDistance(user.Player.X, user.Player.Y, targer.X, targer.Y) < Range)
                                {
                                    if (Line.InLine(attacked.X, attacked.Y, LineRange))
                                    {
                                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                        {
                                            MsgSpellAnimation.SpellObj AnimationObj;
                                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                            AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);

                                            MsgSpell.Targets.Enqueue(AnimationObj);
                                        }
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}
