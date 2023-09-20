using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgTournaments
{
    public class MsgDragonWar : ITournament
    {
        public const uint RewardConquerPoints = 30000;

        public ProcesType Process { get; set; }

        public DateTime StartTimer = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public DateTime ScoreStamp = new DateTime();
        public KillerSystem KillSystem;
        public uint Secounds = 60;
        public uint DinamicID = 0;
        public Role.GameMap Map;
        public TournamentType Type { get; set; }
        public MsgDragonWar(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
           
        }
        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                KillSystem = new KillerSystem();
                if (Map == null)
                {
                    Map = Database.Server.ServerMaps[700];
                    DinamicID = Map.GenerateDynamicID();
                }
               
                StartTimer = DateTime.Now;
                Process = ProcesType.Idle;
                MsgSchedules.SendInvitation("DragonWar[SS/FB]", "ConquerPoints, 4-PowerExpBalls", 298, 232, 1002, 0, 60);
                InfoTimer = DateTime.Now.AddSeconds(10);
                Secounds = 60;
            }
        }

        public bool Join( Client.GameClient client, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                client.Player.DragonWarScore = 0;
                client.Player.DragonWarHits = 0;
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, Map.ID, DinamicID);
                return true;
            }
            return false;
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
        }
        public void DragonWarRevive(Extensions.Time32 Timer, Client.GameClient user)
        {
            if (user.Player.Alive == false && Process != ProcesType.Dead)
            {
                if (InTournament(user))
                {
                    if (user.Player.DeadStamp.AddSeconds(4) < Timer)
                    {
                        ushort x = 0;
                        ushort y = 0;
                        Map.GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, Map.ID, DinamicID);
                    }
                }
            }
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddMinutes(1))
                {
                    MsgSchedules.SendSysMesage("DragonWar has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    StartTimer = DateTime.Now;
                    Process = ProcesType.Alive;
                    if (MapPlayers().Length > 0)
                    {
                        MapPlayers().First().Player.DragonWarHits = 5;
                        //MapPlayers().First().Player.AddFlag(MsgServer.MsgUpdate.Flags.DragonSwing, Role.StatusFlagsBigVector32.PermanentFlag, true);
               
                    }
                }
                else if (DateTime.Now > InfoTimer)
                {
                    Secounds -= 10;
                    MsgSchedules.SendSysMesage("Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    InfoTimer = DateTime.Now.AddSeconds(10);
                }
            }
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > StartTimer.AddMinutes(5))
                {
                    MsgSchedules.SendSysMesage("DragonWar has ended. All Players of DragonWar has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    var array = MapPlayers().OrderByDescending(p => p.Player.DragonWarScore).ToArray();
                    if (array.Length > 0)
                    {
                        var Winner = array.First();
                        MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  DragonWar. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
                        Winner.Player.ConquerPoints += RewardConquerPoints;
                        Winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                    }
                    int x = 1;
                    foreach (var user in array)
                    {
                        if (x > 1)
                        {
                            user.Player.ConquerPoints += (uint)(RewardConquerPoints / x);
                            user.SendSysMesage("You received " + (RewardConquerPoints / x).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                        }
                        x++;
                        user.Teleport(298, 232, 1002);
                        //if (user.Player.ContainFlag(MsgServer.MsgUpdate.Flags.DragonSwing))
                        //    user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.DragonSwing);
                    }
                    Process = ProcesType.Dead;
                }
                if (!ExistDragonWarEffect())
                {
                    if (MapPlayers().Length > 0)
                    {
                        MapPlayers().First().Player.DragonWarHits = 5;
                        //MapPlayers().First().Player.AddFlag(MsgServer.MsgUpdate.Flags.DragonSwing, Role.StatusFlagsBigVector32.PermanentFlag, true);
                    }
                }
                if (DateTime.Now > ScoreStamp)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var array = MapPlayers().OrderByDescending(p => p.Player.DragonWarScore).ToArray();
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("DragonWar Score: ", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                        SendMapPacket(msg.GetArray(stream));

                        int x = 0;
                        foreach (var obj in array)
                        {
                            if (x == 4)
                                break;
                            Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.DragonWarScore.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            SendMapPacket(amsg.GetArray(stream));
                            x++;
                        }
                        foreach (var user in MapPlayers())
                        {
                            msg = new MsgServer.MsgMessage("My tournament Kills: " + user.Player.TournamentKills.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
                        }
                    }
                    ScoreStamp = DateTime.Now.AddSeconds(3);
                }


            }
        }

        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in MapPlayers())
                user.Send(stream);
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => InTournament(p)).ToArray();
        }
        public bool ExistDragonWarEffect()
        {
            return MapPlayers().Where(p => InTournament(p.Player.Owner)).Count() != 0;
        }

    
    }
}
