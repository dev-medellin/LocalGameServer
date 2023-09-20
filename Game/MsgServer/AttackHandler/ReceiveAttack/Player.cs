using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Player
    {
        public unsafe static void Execute(MsgSpellAnimation.SpellObj obj, Client.GameClient client, Role.Player attacked)
        {
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FreezeWar)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        if (client.Player.FreezeTeamType != attacked.FreezeTeamType)
                        {
                            if (attacked.ContainFlag(MsgUpdate.Flags.Freeze) == false)
                            {
                                attacked.AddFlag(MsgUpdate.Flags.Freeze, Role.StatusFlagsBigVector32.PermanentFlag, true);
                                client.Player.FreezePts++;
                            }
                        }
                        else
                        {
                            if (client.Player.FreezeTeamType == attacked.FreezeTeamType)
                                if (attacked.ContainFlag(MsgUpdate.Flags.Freeze))
                                    attacked.RemoveFlag(MsgUpdate.Flags.Freeze);
                        }
                        return;
                    }
                }
            }
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FiveNOut)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        if (attacked.FiveNOut > 0)
                        {
                            attacked.FiveNOut--;
                            if (attacked.FiveNOut == 0)
                                attacked.Owner.SendSysMesage("You`ve just lost your final point, next hit you`re out.");
                            else
                                attacked.Owner.SendSysMesage($"You`ve just lost 1 point. Current points left {attacked.FiveNOut}");
                        }
                        else
                            attacked.Owner.Teleport(1002, 480, 320);
                        return;
                    }
                }
            }
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FrozenSky)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        if (!attacked.ContainFlag(MsgUpdate.Flags.Freeze))
                        {
                            attacked.AddFlag(MsgUpdate.Flags.Fly, 5, true);
                            attacked.Protect = Extensions.Time32.Now.AddSeconds(5);
                            attacked.BlockMovement = DateTime.Now.AddSeconds(5);
                            attacked.BlockMovementCo = true;
                            client.Player.SkyFight++;
                        }
                        return;
                    }
                }
            }
            if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.KillTheCaptain)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
                {
                    if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                    {
                        if (client.TeamKillTheCaptain == attacked.Owner.TeamKillTheCaptain)
                            return;
                    }
                }
            }
            if (Calculate.Base.Success(10))
            {
                CheckAttack.CheckItems.RespouseDurability(client);
            }
            ushort X = attacked.X;
            ushort Y = attacked.Y;
            using (var rec = new ServerSockets.RecycledPacket())
            {

                var stream = rec.GetStream();
                ActionQuery Gui = new ActionQuery()
                {
                    Type = (ActionType)158,
                    ObjId = client.Player.UID,
                    wParam1 = client.Player.X,
                    wParam2 = client.Player.Y
                };
                client.Send(stream.ActionCreate(&Gui));
                ActionQuery action = new ActionQuery()
                {
                    Type = (ActionType)158,
                    ObjId = attacked.UID,
                    wParam1 = attacked.X,
                    wParam2 = attacked.Y
                };
                attacked.Owner.Send(stream.ActionCreate(&action));
            }
            if (attacked.HitPoints <= obj.Damage)
            {
                attacked.DeadState = true;
                if (client.Player.OnTransform)
                {
                    if (client.Player.TransformInfo != null)
                    {
                        client.Player.TransformInfo.FinishTransform();
                    }
                }
                attacked.Dead(client.Player, X, Y, 0);

            }
            else
            {
                CheckAttack.CheckGemEffects.CheckRespouseDamage(attacked.Owner);
                client.UpdateQualifier(client, attacked.Owner, obj.Damage);
                attacked.HitPoints -= (int)obj.Damage;
            }


        }
    }
}
