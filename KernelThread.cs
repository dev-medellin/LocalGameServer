using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace COServer
{
    public class KernelThread
    {
        public const int TournamentsStamp = 1000,
            ChatItemsStamp = 180000,
            TeamArena_CreateMatches = 900,
            TeamArena_VerifyMatches = 980,
            TeamArena_CheckGroups = 960,
            Arena_CreateMatches = 1100,
            Arena_VerifyMatches = 1200,
            Arena_CheckGroups = 1150,
            TeamPkStamp = 1000,
            ElitePkStamp = 1000,
            AccServerStamp = 3300,
            BroadCastStamp = 1000,
            ResetDayStamp = 6000,
            SaveDatabaseStamp = 180000,
            RespawnMapMobs = 500;

        //The Snow Banshee appeared in Frozen Grotto 2(540,430)! Defeat it!

        public Extensions.Time32 UpdateServerStatus = Extensions.Time32.Now;
        //public Extensions.Time32 UpdateWebsiteStatus = Extensions.Time32.Now;
        public Extensions.ThreadGroup.ThreadItem Thread, SaveThread, PokerThread;
        public KernelThread(int interval, string name)
        {
            Thread = new Extensions.ThreadGroup.ThreadItem(interval, name, OnProcess);
            SaveThread = new Extensions.ThreadGroup.ThreadItem(interval, "SaveThread", OnSaveThread);
            PokerThread = new Extensions.ThreadGroup.ThreadItem(interval, "PokerThread", OnPokerThread);
        }
        public void Start()
        {
            Thread.Open();
            SaveThread.Open();
            PokerThread.Open();
        }
        static int _last = 0;
        public int Online
        {
            get
            {
                int current = Database.Server.GamePoll.Count;
                if (current > _last)
                    _last = current;
                return current;
            }
        }
        public static int MaxOnline
        {
            get { return _last; }
        }
        public static DateTime LastServerPulse, LastPokerPulse, LastSavePulse, LastGuildPulse;
        public void OnProcess()
        {
            Extensions.Time32 clock = Extensions.Time32.Now;
            try
            {
                if(DateTime.Now > LastGuildPulse.AddHours(24))
                {
                    foreach (var guilds in Role.Instance.Guild.GuildPoll.Values)
                    {
                        guilds.CreateMembersRank();
                        guilds.UpdateGuildInfo();
                    }
                    LastGuildPulse = DateTime.Now;
                }
                if (clock > UpdateServerStatus)
                {

                    if (Program.ServerConfig.IsInterServer)
                        Console.Title = "[" + Database.GroupServerList.MyServerInfo.Name + "] QueuePackets: " + ServerSockets.PacketRecycle.Count + " Online " + Database.Server.GamePoll.Count + " Time: " + DateTime.Now.Hour + "/" + DateTime.Now.Minute + "/" + DateTime.Now.Second + "";
                    else
                        Console.Title = Program.ServerConfig.ServerName + " - Online: " + Online + " - Max " + MaxOnline;
                    UpdateServerStatus = Extensions.Time32.Now.AddSeconds(5);
                    LastServerPulse = DateTime.Now;
                }
                if (clock > Program.ResetRandom)
                {
                    Program.GetRandom.SetSeed(Environment.TickCount);
                    Program.ResetRandom = Extensions.Time32.Now.AddMinutes(30);
                }

                Game.MsgTournaments.MsgSchedules.CheckUp(clock);
                Program.GlobalItems.Work(clock);

                Game.MsgTournaments.MsgSchedules.TeamArena.CheckGroups(clock);
                Game.MsgTournaments.MsgSchedules.TeamArena.CreateMatches(clock);
                Game.MsgTournaments.MsgSchedules.TeamArena.VerifyMatches(clock);

                Game.MsgTournaments.MsgSchedules.Arena.CheckGroups(clock);
                Game.MsgTournaments.MsgSchedules.Arena.CreateMatches(clock);
                Game.MsgTournaments.MsgSchedules.Arena.VerifyMatches(clock);

                foreach (var elitegroup in Game.MsgTournaments.MsgTeamPkTournament.EliteGroups)
                    elitegroup.timerCallback(clock);

                foreach (var elitegroup in Game.MsgTournaments.MsgSkillTeamPkTournament.EliteGroups)
                    elitegroup.timerCallback(clock);

                foreach (var elitegroup in Game.MsgTournaments.MsgEliteTournament.EliteGroups)
                    elitegroup.timerCallback(clock);
                Game.MsgTournaments.MsgBroadcast.Work(clock);


                DateTime DateNow = DateTime.Now;
            }
            catch (Exception e) { Console.WriteException(e); }
        }
        public void OnPokerThread()
        {
            try
            {
                foreach (var t in Poker.Database.Tables.Values)
                    PokerHandler.PokerTablesCallback(t, 0);
                LastPokerPulse = DateTime.Now;

            }
            catch (Exception e) { Console.WriteException(e); }

        }
        public void OnSaveThread()
        {
            Extensions.Time32 clock = Extensions.Time32.Now;
            try
            {
                Database.Server.Reset(clock);
                Program.SaveDBPayers(clock);

                LastSavePulse = DateTime.Now;

            }
            catch (Exception e) { Console.WriteException(e); }
        }
    }
}
