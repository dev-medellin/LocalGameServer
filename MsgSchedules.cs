using COServer.Database;
using COServer.Game.MsgNpc;
using COServer.Game.MsgServer;
using COServer.Game.MsgServer.AttackHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgTournaments
{
    public class MsgSchedules
    {
        public static List<coords> HideNSeek = new List<coords>() { };
        public static List<coords> LavaBeast = new List<coords>() { 
        new coords(313,250) { },
        new coords(262,241) { },
        new coords(205,231) { },
        new coords(241,201) { },
        new coords(209,143) { },
        new coords(280,176) { },
        new coords(328,166) { },
        new coords(328,120) { },
        new coords(329,067) { },
        new coords(297,087) { },
        new coords(401,185) { },
        new coords(441,225) { },
        new coords(487,249) { },
        new coords(451,289) { },
        new coords(442,348) { },
        new coords(518,296) { },
        new coords(566,334) { },
        new coords(605,322) { },
        new coords(656,359) { },
        new coords(652,407) { },
        new coords(706,414) { },
        new coords(721,471) { },
        new coords(720,518) { },
        new coords(653,512) { },
        new coords(620,549) { },
        new coords(644,599) { },
        new coords(541,510) { },
        new coords(526,466) { },
        new coords(488,431) { },
        new coords(440,408) { },
        new coords(438,484) { },
        new coords(389,483) { },
        new coords(326,475) { },
        new coords(279,441) { },
        new coords(259,478) { },
        new coords(201,435) { },
        new coords(195,375) { },
        new coords(178,324) { },
        new coords(198,285) { },
        new coords(149,435) { },
        new coords(105,407) { },
        new coords(135,367) { },
        new coords(092,348) { },
        new coords(030,321) { },
        new coords(120,314) { },
        new coords(096,256) { },
        new coords(128,251) { },
        new coords(363,525) { },
        new coords(393,558) { },
        new coords(387,592) { },
        new coords(374,643) { },
        new coords(308,625) { },
        new coords(309,572) { },
        new coords(287,540) { },
        new coords(428,643) { },
        new coords(474,642) { },
        new coords(461,683) { },
        new coords(467,735) { },
        new coords(534,670) { },
        new coords(550,630) { },
        new coords(524,588) { },
        new coords(578,601) { }
        };
        public static Extensions.Time32 Stamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.TournamentsStamp);
        public static Dictionary<TournamentType, ITournament> Tournaments = new Dictionary<TournamentType, ITournament>();
        public static ITournament CurrentTournament;
        internal static MsgGuildWar GuildWar;
        internal static MsgEliteGuildWar EliteGuildWar;
        internal static MsgPoleDomination PoleDomination;
        internal static MsgClassicClanWar ClassicClanWar;
        internal static MsgArena Arena;
        internal static MsgTeamArena TeamArena;
        internal static MsgClassPKWar ClassPkWar;
        internal static MsgCouples CouplesPKWar;
        internal static MsgEliteTournament ElitePkTournament;
        internal static MsgTeamPkTournament TeamPkTournament;
        internal static MsgSkillTeamPkTournament SkillTeamPkTournament;
        internal static MsgCaptureTheFlag CaptureTheFlag;
        internal static MsgClanWar ClanWar;
        internal static MsgPkWar PkWar;
        internal static MsgDisCity DisCity;
        internal static MsgSteedRace SteedRace;
        private static int NextBoss = 0;
        public static void LoadCoords()
        {
            HideNSeek.Add(297, 300);
            HideNSeek.Add(266, 327);
            HideNSeek.Add(248, 360);
            HideNSeek.Add(209, 321);
            HideNSeek.Add(157, 253);
            HideNSeek.Add(130, 242);
            HideNSeek.Add(095, 207);
            HideNSeek.Add(050, 161);
            HideNSeek.Add(020, 130);
            HideNSeek.Add(038, 119);
            HideNSeek.Add(074, 113);
            HideNSeek.Add(133, 143);
            HideNSeek.Add(136, 102);
            HideNSeek.Add(169, 093);
            HideNSeek.Add(169, 061);
            HideNSeek.Add(152, 043);
            HideNSeek.Add(135, 022);
            HideNSeek.Add(193, 075);
            HideNSeek.Add(222, 099);
            HideNSeek.Add(238, 122);
            HideNSeek.Add(271, 148);
            HideNSeek.Add(295, 183);
            HideNSeek.Add(347, 223);
            HideNSeek.Add(363, 259);
            HideNSeek.Add(315, 202);
            HideNSeek.Add(289, 165);
            HideNSeek.Add(186, 099);
            HideNSeek.Add(081, 189);

        }
        internal static void Create()
        {
            LoadCoords();
            //Tournaments.Add(TournamentType.QuizShow, new MsgQuizShow(TournamentType.QuizShow));
            Tournaments.Add(TournamentType.FreezeWar, new MsgFreezeWar(TournamentType.FreezeWar));
            Tournaments.Add(TournamentType.TeamDeathMatch, new MsgTeamDeathMatch(TournamentType.TeamDeathMatch));
            Tournaments.Add(TournamentType.LastManStand, new MsgLastManStand(TournamentType.LastManStand));
            Tournaments.Add(TournamentType.BettingCPs, new MsgBettingCompetition(TournamentType.BettingCPs));
            Tournaments.Add(TournamentType.KingOfTheHill, new MsgKingOfTheHill(TournamentType.KingOfTheHill));
            Tournaments.Add(TournamentType.KillTheCaptain, new MsgKillTheCaptain(TournamentType.KillTheCaptain));
            Tournaments.Add(TournamentType.FiveNOut, new Fivenout(TournamentType.FiveNOut));
            //Tournaments.Add(TournamentType.FrozenSky, new FrozenSky(TournamentType.FrozenSky));
            CurrentTournament = Tournaments[TournamentType.LastManStand];
            GuildWar = new MsgGuildWar();
            EliteGuildWar = new MsgEliteGuildWar();
            PoleDomination = new MsgPoleDomination();
            ClassicClanWar = new MsgClassicClanWar();
            Arena = new MsgArena();
            TeamArena = new MsgTeamArena();
            ClassPkWar = new MsgClassPKWar(ProcesType.Dead);
            ElitePkTournament = new MsgEliteTournament();
            CaptureTheFlag = new MsgCaptureTheFlag();
            PkWar = new MsgPkWar();
            CouplesPKWar = new MsgCouples();
            DisCity = new MsgDisCity();
            SteedRace = new MsgSteedRace();
            TeamPkTournament = new MsgTeamPkTournament();
            SkillTeamPkTournament = new MsgSkillTeamPkTournament();
            MsgBroadcast.Create();

        }
        public static void SpawnLavaBeast()
        {
            var Map = Database.Server.ServerMaps[2056];
            int Loc = Program.GetRandom.Next(0, LavaBeast.Count);
            var spawnLoc = LavaBeast[Loc];
            LavaBeast.RemoveAt(Loc);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                string msg = "LavaBeast has spawned in FrozenGrotto6! Hurry find it and kill it.";
                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.TopLeft).GetArray(stream));
                Database.Server.AddMapMonster(stream, Map, 20055, (ushort)spawnLoc.X, (ushort)spawnLoc.Y, 1, 1, 1);
                //                Program.//                DiscordAPI.Enqueue($"``{msg}``");
                Console.WriteLine($"Spawned Lava Beast at {spawnLoc.X},{spawnLoc.Y}");
            }
        }
        internal static void SendInvitation(string Name, string Prize, ushort X, ushort Y, ushort map, ushort DinamicID, int Seconds, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            string Message = " " + Name + " is about to begin! Will you join it? Prize[" + Prize + "]";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                var packet = new Game.MsgServer.MsgMessage(Message, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer())
                        continue;
                    client.Send(packet);
                    client.Player.MessageBox(Message, new Action<Client.GameClient>(user => user.Teleport(X, Y, map, DinamicID)), null, Seconds, messaj);
                }
            }
        }
        internal unsafe static void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red, bool SendScren = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var packet = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                    client.Send(packet);
            }
        }
        static bool hideNSeek = false;
        static List<string> SystemMsgs = new List<string>() {
            "Selling/trading cps outside the game will lead to your accounts banned forever.",
            "Join our discord group to be in touch with the community and suggest/report stuff.",
            "TeratoDragon can only be spawned once a day(24hours) in Twin city and Altar.",
            "Administrators have [GM/PM] in their names,do not trust anyone else claiming to be a [GM].",
            "Refer our server and gain rewards! (contact GM/PM).",
            "Thanks for supporting us! we will keep on working to provide the best for you!",
            "Check out Guide in TwinCity for information about the game.",
            "Like our Facebook page to support us!"
        };
        public static bool TCBossInv = false, TCBossLaunched = false, TeratoINV = false, TeratoLaunched = false, HourlyBossInv = false, HourlyBossLaunched = false;
        internal static DateTime NextLavaBeast;
        internal static int LavaBeastsCount = 0;

        internal static void CheckUp(Extensions.Time32 clock)
        {

            if (clock > Stamp)
            {
                DateTime Now64 = DateTime.Now;

                //if (Now64.Minute == 55 && Now64.Hour == 19 && Now64.DayOfWeek == DayOfWeek.Friday)
                //{
                //    ElitePkTournament.Start();

                //}
                if (!Database.Server.FullLoading)
                    return;

                if (Arena.Proces == ProcesType.Dead)
                {
                    Arena.Proces = ProcesType.Alive;
                }
                if (TeamArena.Proces == ProcesType.Dead)
                {
                    TeamArena.Proces = ProcesType.Alive;
                }
                if (Now64.Minute == 0)
                    TCBossInv = TCBossLaunched = TeratoINV = TeratoLaunched = HourlyBossInv = HourlyBossLaunched = false;
                try
                {
                    //SteedRace.work(0);

                    if (CaptureTheFlag.Proces == ProcesType.Alive)
                    {
                        CaptureTheFlag.UpdateMapScore();
                        CaptureTheFlag.CheckUpX2();
                        CaptureTheFlag.SpawnFlags();
                    }
                    if (Now64.Minute == 0 && Now64.Second <= 5)
                    {
                        //r reset all variables.
                        hideNSeek = false;
                    }
                    //if ((Now64.Hour == 7 && Now64.Minute == 30 || Now64.Hour == 17 && Now64.Minute == 30))
                    //    DisCity.Open();
                    CurrentTournament.CheckUp();
                    //if (DateTime.Now.Hour == 21 && DateTime.Now.Minute == 1)
                    //    ClanWar.Open();
                    //ClanWar.CheckUp(Now64);
                    DisCity.CheckUp();
                    PkWar.CheckUp();
                    CouplesPKWar.CheckUp();
                    //if (Now64.Hour % 1 == 0)
                    if (Now64.Minute == 46 && Now64.Second <= 3)
                        NextBoss = Role.Core.Random.Next(0, 2);
                    if (Now64.Minute % 10 == 0 && Now64.Second > 58)
                    {
                        var rndMsg = SystemMsgs[Program.GetRandom.Next(0, SystemMsgs.Count)];
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(rndMsg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        }
                        //                Program.//                DiscordAPI.Enqueue($"Total Online: {Database.Server.GamePoll.Count} - Max Online: {KernelThread.MaxOnline}");
                    }

                    #region LavaBeasts
                    if (DateTime.Now > NextLavaBeast && LavaBeastsCount > 0)
                    {
                        LavaBeastsCount--;
                        if (LavaBeastsCount > 0)
                            NextLavaBeast = DateTime.Now.AddMinutes(3);
                        SpawnLavaBeast();
                    }
                    #endregion

                    #region DarkmoonDemon
                    if (Now64.Minute == 2 && Now64.Second < 2 && !TCBossInv)
                    {
                        TCBossInv = true;

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            string msg = "DarkmoonDemon will be spawned next to the TwinCity's Bridge (Drops Souls).";
                            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            //                Program.//                DiscordAPI.Enqueue($"``{msg}``");

                        }
                        SendInvitation("NewPlayerBoss", "DragonSouls", 663, 670, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                    }
                    if (Now64.Minute == 5 && Now64.Second <= 2 && !TCBossLaunched)
                    {
                        TCBossLaunched = true;
                        var Map = Database.Server.ServerMaps[1002];

                        if (!Map.ContainMobID(4145))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                string msg = "DarkmoonDemon has spawned next to the TwinCity's Bridge (656,677) ! Hurry and kill it.";
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                Database.Server.AddMapMonster(stream, Map, 4145, 656, 677, 1, 1, 1);
                                //                Program.//                DiscordAPI.Enqueue($"``{msg}``");


                            }
                        }
                        else
                        {
                            var loc = Map.GetMobLoc(20070);
                            if (loc != "")
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    string msg = "DarkmoonDemon is still alive in TwinCity at " + loc;
                                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                    //                Program.//                DiscordAPI.Enqueue($"``{msg}``");

                                }
                        }
                    }
                    #endregion

                    #region SnowBanshee/Spook
                    if (Now64.Minute == 47 && Now64.Second <= 3 && !HourlyBossInv)
                    {
                        HourlyBossInv = true;
                        if (NextBoss == 0)
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                string msg = "SnowBashee will be spawned in 3 mins in FrozenGrotto2.";
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                //                Program.//                DiscordAPI.Enqueue($"``{msg}``");
                            }
                        }
                        else
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                string msg = "Thrilling Spook will be spawned in 3 mins in the Spook's land (Join from market).";
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                //                Program.//                DiscordAPI.Enqueue($"``{msg}``");


                            }
                        }
                    }
                    if (Now64.Minute == 50 && Now64.Second <= 3 && !HourlyBossLaunched)
                    {
                        HourlyBossLaunched = true;
                        if (NextBoss == 0) // Banshee
                        {
                            var Map = Database.Server.ServerMaps[2054];

                            if (!Map.ContainMobID(20070))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    string msg = "SnowBashee has spawned in FrozenGrotto2 (407,433) ! Hurry and kill it.";
                                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                    Database.Server.AddMapMonster(stream, Map, 20070, 407, 433, 1, 1, 1);
                                    //                Program.//                DiscordAPI.Enqueue($"``{msg}``");


                                }
                            }
                            else
                            {
                                var loc = Map.GetMobLoc(20070);
                                if (loc != "")
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        string msg = "SnowBashee is still alive in FrozenGrotto2 at " + loc;
                                        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                        //                Program.//                DiscordAPI.Enqueue($"``{msg}``");

                                    }

                            }
                        }
                        else
                        {
                            var Map = Database.Server.ServerMaps[2090];

                            if (!Map.ContainMobID(20160))
                            {
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Thrilling Spook has spawned! Hurry and kill it.", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

                                    Database.Server.AddMapMonster(stream, Map, 20160, 41, 40, 1, 1, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);

                                }
                            }
                            else
                            {
                                var loc = Map.GetMobLoc(20160);
                                if (loc != "")
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage("Spook is still alive  at " + loc + " can be reached from market npc.", "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                    }
                            }
                        }
                    }
                    #endregion

                    #region Terato
                    if (Now64.Hour % 6 == 0 && Now64.Minute == 17 && Now64.Second <= 3 && !TeratoINV)
                    {
                        TeratoINV = true;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            string msg = "TeratoDragon will be spawned in 3 mins in FrozenGrotto6.";
                            Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                            //                Program.//                DiscordAPI.Enqueue($"``{msg}``");
                        }
                    }
                    if (Now64.Hour % 6 == 0 && Now64.Minute == 20 && Now64.Second <= 3 && !TeratoLaunched)
                    {
                        TeratoLaunched = true;
                        var Map = Database.Server.ServerMaps[2056];

                        if (!Map.ContainMobID(20070))
                        {
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                string msg = "TeratoDragon has spawned in FrozenGrotto6 (328,354) ! Hurry and kill it.";
                                Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                Database.Server.AddMapMonster(stream, Map, 20060, 328, 354, 1, 1, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.EarthquakeAndNight);

                                //                Program.//                DiscordAPI.Enqueue($"``{msg}``");
                            }
                        }
                        else
                        {
                            var loc = Map.GetMobLoc(20060);
                            if (loc != "")
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    string msg = "SnowBashee is still alive in FrozenGrotto6 at " + loc;
                                    Program.SendGlobalPackets.Enqueue(new MsgServer.MsgMessage(msg, "ALLUSERS", "Server", MsgServer.MsgMessage.MsgColor.red, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                                    //                Program.//                DiscordAPI.Enqueue($"``{msg}``");

                                }
                        }

                    }
                    #endregion
                }
                catch (Exception e)
                {
                    Console.SaveException(e);
                }
                #region Hourly
                Random Rand = new Random();
                if (CurrentTournament.Process == ProcesType.Dead)
                {
                    if (Now64.Minute == 10 && Now64.Second < 4)
                    {
                        switch (Rand.Next(0, 4))
                        {
                            case 0:
                                CurrentTournament = Tournaments[TournamentType.KingOfTheHill];
                                break;
                            case 1:
                                CurrentTournament = Tournaments[TournamentType.KillTheCaptain];
                                break;
                            case 2:
                                CurrentTournament = Tournaments[TournamentType.LastManStand];
                                break;
                            case 3:
                            default:
                                CurrentTournament = Tournaments[TournamentType.BettingCPs];
                                break;
                        }
                        CurrentTournament.Open();
                        Console.WriteLine("Started Tournament " + CurrentTournament.Type.ToString());
                        //                Program.//                DiscordAPI.Enqueue("``Tournament " + CurrentTournament.Type.ToString() + " has started!``");
                    }
                }
                if (CurrentTournament.Process == ProcesType.Dead)
                {
                    if (Now64.Minute == 30 && Now64.Second < 4)
                    {
                        switch (Rand.Next(0, 2))
                        {
                            case 0:
                                CurrentTournament = Tournaments[TournamentType.FreezeWar];
                                break;
                            case 1:
                                CurrentTournament = Tournaments[TournamentType.FiveNOut];
                                break;
                        }
                        CurrentTournament.Open();
                        Console.WriteLine("Started Tournament " + CurrentTournament.Type.ToString());
                        //                Program.//                DiscordAPI.Enqueue("``Tournament " + CurrentTournament.Type.ToString() + " has started!``");
                    }
                }
                if (Now64.Minute == 00 && Now64.Second >= 40 && !hideNSeek)
                {
                    hideNSeek = true;
                    var map = Server.ServerMaps[1036];
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Role.Core.SendGlobalMessage(stream, $"Hide(n)Seek event has started! Find the [GM] Npc in market to claim a prize.", MsgMessage.ChatMode.TopLeftSystem);
                        Role.Core.SendGlobalMessage(stream, $"Hide(n)Seek event has started! Find the [GM] Npc in market to claim a prize.", MsgMessage.ChatMode.Center);
                        //                Program.//                DiscordAPI.Enqueue("``Hide(n)Seek event has started! Find the [GM] Npc in market to claim a prize.``");
                    }
                    ushort x = 0, y = 0;
                    map.GetRandCoord(ref x, ref y);
                    var npc = Game.MsgNpc.Npc.Create();
                    npc.UID = (uint)NpcID.HideNSeek;
                    var rndCoords = HideNSeek[Rand.Next(0, HideNSeek.Count)];
                    npc.X = (ushort)rndCoords.X;
                    npc.Y = (ushort)rndCoords.Y;
                    npc.Mesh = 29681;
                    npc.NpcType = Role.Flags.NpcType.Talker;
                    npc.Map = 1036;
                    map.AddNpc(npc);
                    Console.WriteLine($"Hide(N)Seek location: {x}, {y}");
                }
                #endregion
                #region Days
                if (Now64.DayOfWeek == DayOfWeek.Saturday)
                {
                    #region GuildWar
                    if (Now64.Hour < 21)
                    {
                        if (GuildWar.Proces == ProcesType.Dead)
                            GuildWar.Start();
                        if (GuildWar.Proces == ProcesType.Idle)
                        {
                            if (Now64 > GuildWar.StampRound)
                                GuildWar.Began();
                        }
                        if (GuildWar.Proces != ProcesType.Dead)
                        {
                            if (DateTime.Now > GuildWar.StampShuffleScore)
                            {
                                GuildWar.ShuffleGuildScores();
                            }
                        }
                        if (Now64.Hour == 20)
                        {
                            if (GuildWar.FlamesQuest.ActiveFlame10 == false)
                            {
                                SendSysMesage("The Flame Stone 9 is Active now. Light up the Flame Stone (62,59) near the Stone Pole in the Guild City.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                                GuildWar.FlamesQuest.ActiveFlame10 = true;
                            }
                        }
                        else if (GuildWar.SendInvitation == false && Now64.Hour == 18)
                        {
                            SendInvitation("GuildWar", "ConquerPoints", 200, 254, 1038, 0, 60, MsgServer.MsgStaticMessage.Messages.GuildWar);
                            GuildWar.SendInvitation = true;
                        }

                    }
                    else
                    {
                        if (GuildWar.Proces == ProcesType.Alive || GuildWar.Proces == ProcesType.Idle)
                            GuildWar.CompleteEndGuildWar();
                    }
                    #endregion
                }

                #region EliteGuildWar
                if (Now64.Hour >= 15 && Now64.Hour < 16)
                {
                    if (EliteGuildWar.Proces == ProcesType.Dead)
                        EliteGuildWar.Start();
                    if (EliteGuildWar.Proces == ProcesType.Idle)
                    {
                        if (Now64 > EliteGuildWar.StampRound)
                            EliteGuildWar.Began();
                    }
                    if (EliteGuildWar.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > EliteGuildWar.StampShuffleScore)
                        {
                            EliteGuildWar.ShuffleGuildScores();
                        }
                    }

                    if (EliteGuildWar.SendInvitation == false && (Now64.Hour == 15 || Now64.Hour == 4) && Now64.Minute == 30 && Now64.Second >= 30)
                    {
                        SendInvitation("EliteGuildWar", "ConquerPoints", 437, 248, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                        EliteGuildWar.SendInvitation = true;
                    }
                }
                else
                {
                    if (EliteGuildWar.Proces == ProcesType.Alive || EliteGuildWar.Proces == ProcesType.Idle)
                        EliteGuildWar.CompleteEndGuildWar();
                }
                #endregion

                #region PoleDomination
                if (Now64.Minute >= 35 && Now64.Minute < 45)
                {
                    if (PoleDomination.Proces == ProcesType.Dead)
                        PoleDomination.Start();
                    if (PoleDomination.Proces == ProcesType.Idle)
                    {
                        if (Now64 > PoleDomination.StampRound)
                            PoleDomination.Began();
                    }
                    if (PoleDomination.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > PoleDomination.StampShuffleScore)
                        {
                            PoleDomination.ShuffleGuildScores();
                        }
                    }

                    if (PoleDomination.SendInvitation == false && Now64.Minute == 35)
                    {
                        SendInvitation("ApeCity PoleDomination", "ConquerPoints", 576, 623, 1020, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                        PoleDomination.SendInvitation = true;
                    }
                }
                else
                {
                    if (PoleDomination.Proces == ProcesType.Alive || PoleDomination.Proces == ProcesType.Idle)
                        PoleDomination.CompleteEndGuildWar();
                }
                #endregion

                #region ClanWar
                if (Now64.Hour == 21)
                {
                    if (Now64.Minute >= 00 && Now64.Minute < 30)
                    {
                        if (ClassicClanWar.Proces == ProcesType.Dead)
                            ClassicClanWar.Start();
                        if (ClassicClanWar.Proces == ProcesType.Idle)
                        {
                            if (Now64 > ClassicClanWar.StampRound)
                                ClassicClanWar.Began();
                        }
                        if (PoleDomination.Proces != ProcesType.Dead)
                        {
                            if (DateTime.Now > ClassicClanWar.StampShuffleScore)
                            {
                                ClassicClanWar.ShuffleGuildScores();
                            }
                        }

                        if (ClassicClanWar.SendInvitation == false && Now64.Minute == 00)
                        {
                            SendInvitation("ClanWar", "ConquerPoints", 424, 251, 1002, 0, 60, MsgServer.MsgStaticMessage.Messages.None);
                            ClassicClanWar.SendInvitation = true;
                        }
                    }
                    else
                    {
                        if (ClassicClanWar.Proces == ProcesType.Alive || ClassicClanWar.Proces == ProcesType.Idle)
                            ClassicClanWar.CompleteEndGuildWar();
                    }
                }
                #endregion
                //else
                {
                    #region ElitePkTournament
                    if ((Now64.DayOfWeek == DayOfWeek.Friday) && Now64.Hour != 19 && Now64.Hour != 20 && Now64.Hour != 21)
                    {
                        if (ElitePkTournament.Proces != ProcesType.Dead)
                        {
                            ElitePkTournament.Proces = ProcesType.Dead;
                        }
                    }
                    if ((Now64.DayOfWeek == DayOfWeek.Friday) && Now64.Hour == 19 && Now64.Minute == 55)
                    {
                        ElitePkTournament.Start();
                    }
                    #endregion
                    #region TeamPkTournament
                    if ((Now64.DayOfWeek == DayOfWeek.Saturday) && Now64.Hour == 16 && Now64.Minute == 45)
                    {
                        TeamPkTournament.Start();
                    }
                    #endregion
                    #region SkillTeamTournament
                    if ((Now64.DayOfWeek == DayOfWeek.Wednesday) && Now64.Hour == 19 && Now64.Minute == 45)
                    {
                        SkillTeamPkTournament.Start();
                    }
                    #endregion
                    #region ClassPK
                    if (Now64.DayOfWeek == DayOfWeek.Monday)
                    {
                        if (Now64.Hour == 18 && Now64.Minute == 0)
                        {
                            ClassPkWar.Start();
                        }
                        if (Now64.Hour == 18 && Now64.Minute >= 10)
                        {
                            foreach (var war in ClassPkWar.PkWars)
                                foreach (var map in war)
                                {
                                    var players_in_map = Database.Server.GamePoll.Values.Where(e => e.Player.DynamicID == map.DinamicID && e.Player.Alive);
                                    if (players_in_map.Count() == 1)
                                    {
                                        var winner = players_in_map.SingleOrDefault();
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            map.GetMyReward(winner, stream);
                                        }
                                    }
                                }
                        }
                    }
                    #endregion
                }
                #endregion



                if (Now64.DayOfWeek == DayOfWeek.Saturday && Now64.Hour == 14 && Now64.Minute == 45)
                {
                    CouplesPKWar.Open();
                }

                Stamp.Value = clock.Value + KernelThread.TournamentsStamp;
            }
        }
    }
}
