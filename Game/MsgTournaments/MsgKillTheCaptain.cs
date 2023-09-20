using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgTournaments
{
    public enum KillTheCaptainTeams
    {
        Red = 0,
        Blue = 1
    }
    public class MsgKillTheCaptain : ITournament
    {
        public ProcesType Process { get; set; }
        public TournamentType Type { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public DateTime ScoreStamp = new DateTime();
        public KillerSystem KillSystem;
        public uint Secounds = 60;
        public uint DinamicID = 0;
        public Role.GameMap Map;

        public MsgKillTheCaptain(TournamentType _type)
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
                MsgSchedules.SendInvitation("KillTheCaptain", "ConquerPoints, 2-PowerExpBalls", 445, 351, 1002, 0, 60);
                InfoTimer = DateTime.Now.AddSeconds(10);
                Secounds = 60;
                RedScore = 0;
                BlueScore = 0;
            }
        }
        public byte NextTeam()
        {
            int nextid = ++TeamNow;
            if (nextid % 2 == 0)
                return 0;
            else return 1;
        }
        int TeamNow = 0;
        public bool Join(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                client.Player.KillTheCaptain = 0;
                client.TeamKillTheCaptain = (KillTheCaptainTeams)NextTeam();
                client.SendSysMesage("You're in the " + client.TeamKillTheCaptain.ToString() + " team.");
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, Map.ID, DinamicID);
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 60, true);
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
        public void Revive(Client.GameClient user)
        {
            if (user.Player.Alive == false && Process != ProcesType.Dead)
            {
                if (InTournament(user))
                {
                    ushort x = 0;
                    ushort y = 0;
                    Map.GetRandCoord(ref x, ref y);
                    user.Teleport(x, y, Map.ID, DinamicID);
                }
            }
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => InTournament(p)).ToArray();
        }
        public int RedScore = 0, BlueScore = 0;

        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in MapPlayers())
                user.Send(stream);
        }
        public void ChooseRandomLeader(KillTheCaptainTeams killTheCaptainTeams)
        {
            var players = MapPlayers().Where(e => e.Player.Alive && e.TeamKillTheCaptain == killTheCaptainTeams).ToList();
            if (players.Count == 0)
            {
                if (killTheCaptainTeams == KillTheCaptainTeams.Blue)
                {
                    EndEvent(KillTheCaptainTeams.Red);
                    return;
                }
                else
                {
                    EndEvent(KillTheCaptainTeams.Blue);
                    return;
                }
            }
            foreach (var player in players)
                if (player.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Flashy))
                    player.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Flashy);
            var rndLeader = players[Program.GetRandom.Next(0, players.Count)];
            rndLeader.Player.AddFlag(MsgServer.MsgUpdate.Flags.Flashy, 600, true);
            MsgSchedules.SendSysMesage("[KillTheCap] Team " + killTheCaptainTeams.ToString() + " your new target is " + rndLeader.Player.Name, MsgServer.MsgMessage.ChatMode.TopLeft, MsgServer.MsgMessage.MsgColor.red);
        }
        private void EndEvent(KillTheCaptainTeams winnerTeam)
        {
            var array = MapPlayers()
                .Where(e => e.TeamKillTheCaptain == winnerTeam)
                .OrderByDescending(p => p.Player.KillTheCaptain)
                .ToArray();

            if (array.Length > 0)
            {
                #region Rewards
                var Winner = array.FirstOrDefault();
                if (Winner != null)
                {

                    MsgSchedules.SendSysMesage("KillTheCaptain has ended. All Players of KillTheCaptain has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has won the KillTheCaptain. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
                    Winner.Player.ConquerPoints += 12000;
                    string reward = "[EVENT]" + Winner.Player.Name + " has won and received 12,000 CPs from KillTheCaptain.";
                    //                Program.//                DiscordAPI.Enqueue($"``{reward}``");

                    Database.ServerDatabase.LoginQueue.Enqueue(reward);
                    Winner.Teleport(440, 368, 1002);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        Winner.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Flashy);
                        var stream = rec.GetStream();
                        if (Winner.Inventory.HaveSpace(2))
                            Winner.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2);
                        else
                            Winner.Inventory.AddReturnedItem(stream, Database.ItemType.PowerExpBall, 2);
                        Winner.SendSysMesage("You received 12,000 ConquerPoints and 2PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                    }
                    foreach (var user in MapPlayers())
                    {
                        user.Teleport(440, 368, 1002);//to do
                        user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Flashy);

                    }
                    Process = ProcesType.Dead;
                }
                #endregion
            }
            else
                Process = ProcesType.Dead;
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddMinutes(1))
                {
                    MsgSchedules.SendSysMesage("KillTheCaptain has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    StartTimer = DateTime.Now;
                    foreach (var user in MapPlayers())
                        user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                    Process = ProcesType.Alive;
                }
                else if (DateTime.Now > InfoTimer)
                {
                    Secounds -= 10;
                    MsgSchedules.SendSysMesage("Fight starts in " + Secounds.ToString() + " seconds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    InfoTimer = DateTime.Now.AddSeconds(10);
                }
            }
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > ScoreStamp)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var array = MapPlayers();

                        if (array.Count() == 0) return;
                        foreach (var user in array)
                        {
                            //if (DateTime.Now > user.Player.DeathStamp.AddSeconds(1))
                            Revive(user);
                        }

                        bool full_red = true, full_blue = true;

                        full_red = array.Where(e => e.TeamKillTheCaptain == KillTheCaptainTeams.Red && e.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Flashy)).Count() == 0;
                        if (full_red)
                            ChooseRandomLeader(KillTheCaptainTeams.Red);

                        full_blue = array.Where(e => e.TeamKillTheCaptain == KillTheCaptainTeams.Blue && e.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Flashy)).Count() == 0;

                        if (full_blue)
                            ChooseRandomLeader(KillTheCaptainTeams.Blue);
                        RedScore = BlueScore = 0;
                        foreach (var player in array)
                            if (player.TeamKillTheCaptain == KillTheCaptainTeams.Red)
                                RedScore += player.Player.KillTheCaptain;
                            else BlueScore += player.Player.KillTheCaptain;
                        if (RedScore >= 200)
                        {
                            EndEvent(KillTheCaptainTeams.Red);
                            return;
                        }
                        if (BlueScore >= 200)
                        {
                            EndEvent(KillTheCaptainTeams.Blue);
                            return;
                        }
                        if (DateTime.Now > StartTimer.AddMinutes(10))
                        {
                            if (RedScore >= BlueScore)
                                EndEvent(KillTheCaptainTeams.Red);
                            else
                                EndEvent(KillTheCaptainTeams.Blue);
                            return;
                        }

                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("ScoreBoard: (1st to 200 wins)", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                        SendMapPacket(msg.GetArray(stream));

                        Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("Red Team Score:" + RedScore, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        Game.MsgServer.MsgMessage amsg2 = new MsgServer.MsgMessage("Blue Team Score:" + BlueScore, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                        SendMapPacket(amsg.GetArray(stream));
                        SendMapPacket(amsg2.GetArray(stream));

                        var top_red = array.Where(e => e.TeamKillTheCaptain == KillTheCaptainTeams.Red).OrderByDescending(e => e.Player.KillTheCaptain).FirstOrDefault().Player.Name;
                        var top_blue = array.Where(e => e.TeamKillTheCaptain == KillTheCaptainTeams.Blue).OrderByDescending(e => e.Player.KillTheCaptain).FirstOrDefault().Player.Name;

                        foreach (var user in array)
                        {
                            msg = new MsgServer.MsgMessage("My tournament Kills: " + user.Player.KillTheCaptain.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            string name = "";
                            if (user.TeamKillTheCaptain == KillTheCaptainTeams.Red)
                                name = top_red;
                            else name = top_blue;
                            var msg2 = new MsgServer.MsgMessage("Current Team prize goes to: " + name, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
                            user.Send(msg2.GetArray(stream));

                            Game.MsgServer.MsgMessage msg3 = new MsgServer.MsgMessage("My Team: " + user.TeamKillTheCaptain, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg3.GetArray(stream));
                        }
                    }
                    ScoreStamp = DateTime.Now.AddSeconds(3);
                }
            }
        }
    }
}
