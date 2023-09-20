using COServer.Client;
using COServer.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Database
{
    public class ArenaTable
    {

        internal void Save()
        {
            using (Database.DBActions.Write writer = new DBActions.Write("Arena.ini"))
            {
                foreach (var user in Game.MsgTournaments.MsgArena.ArenaPoll.Values)
                {
                    writer.Add(user.ToString());
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }
        internal void Load()
        {
            using (Database.DBActions.Read reader = new DBActions.Read("Arena.ini"))
            {
                if (reader.Reader())
                {
                    for (int i = 0; i < reader.Count; i++)
                    {
                        Game.MsgTournaments.MsgArena.User user = new Game.MsgTournaments.MsgArena.User();
                        user.Load(reader.ReadString(""));
                        Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(user.UID, user);
                    }
                }
            }

            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop10();
            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop1000();
            Game.MsgTournaments.MsgArena.UpdateRank();
        }
        void GiveCpsOffline(uint UID, uint val)
        {
            var ini = new IniFile($"\\Users\\{UID}.ini");
            uint CPS = ini.ReadUInt32("Character", "ArenaCPS", 0);
            int DragonPills = ini.ReadInt32("Character", "DragonPills", 0);
            ini.Write<uint>("Character", "ArenaCPS", CPS + val);
            //ini.Write<int>("Character", "DragonPills", DragonPills + 1);
        }
        uint CPsForRank(uint Rank)
        {
            switch (Rank)
            {
                case 1: return 40000;
                case 2: return 30000;
                case 3: return 20000;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    return 15000;
                default: return 0;
            }
        }
        internal void ResetArena()
        {

            foreach (var user in Game.MsgTournaments.MsgArena.ArenaPoll.Values)
            {
                user.LastSeasonArenaPoints = user.Info.ArenaPoints;
                user.LastSeasonWin = user.Info.TodayWin;
                user.LastSeasonLose = user.Info.TotalLose;
                user.LastSeasonRank = user.Info.TodayRank;
                //user.Info.CurrentHonor = user.Info.HistoryHonor = 0;

                if (user.Info.TodayRank >= 1 && user.Info.TodayRank <= 10 && user.Info.ArenaPoints != 4000)
                {
                    GameClient client;
                    if (Database.Server.GamePoll.TryGetValue(user.UID, out client))
                    {
                        client.Player.ConquerPoints += CPsForRank(user.Info.TodayRank);
                        //  client.Player.DragonPills++;
                        client.Player.MessageBox($"You got {CPsForRank(user.Info.TodayRank)} CPs for ranking in top10 arena.", null, null);
                    }
                    else
                    {
                        GiveCpsOffline(user.UID, CPsForRank(user.Info.TodayRank));
                    }
                }


                ////if (user.Info.TodayWin >= 10)
                //{
                if (user.Info.TodayRank >= 1 && user.Info.TodayRank <= 15)
                    if (user.Info.ArenaPoints != 4000)
                    {
                        int honner = 16000 - ((int)user.Info.TodayRank * 1000);
                        if (honner < 0)
                            honner = 0;
                        user.Info.CurrentHonor += (uint)honner;
                    }
                if (user.Info.HistoryHonor < user.Info.CurrentHonor)
                    user.Info.HistoryHonor = user.Info.CurrentHonor;
                //}

                user.Info.TodayWin = 0;
                user.Info.TodayBattles = 0;
                user.Info.TotalLose = 0;

                user.Info.TodayRank = 0;
                user.Info.ArenaPoints = 4000;

            }

            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop10();
            Game.MsgTournaments.MsgSchedules.Arena.CreateRankTop1000();
            Game.MsgTournaments.MsgArena.UpdateRank();
        }
    }
}
