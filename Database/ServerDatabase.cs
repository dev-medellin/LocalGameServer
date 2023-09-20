using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using COServer.Game.MsgServer;
using System.Windows.Forms;
using COServer.Role.Instance;

namespace COServer.Database
{
    public class ServerDatabase
    {
        public static void ResetingEveryDay(Client.GameClient client)
        {
            try
            {
                if (DateTime.Now.DayOfYear != client.Player.Day)
                {
                    client.Player.UseChiToken = 0;
                    client.Player.TodayChampionPoints = 0;

                    if (client.Player.DailyMonth == 0)
                        client.Player.DailyMonth = (byte)DateTime.Now.Month;
                    if (client.Player.DailyMonth != DateTime.Now.Month)
                    {
                        client.Player.DailySignUpRewards = 0;
                        client.Player.DailySignUpDays = 0;
                        client.Player.DailyMonth = (byte)DateTime.Now.Month;
                    }
                    client.Player.QuestGUI.RemoveQuest(6126);
                    client.Player.OpenHousePack = 0;
                    client.Player.DbTry = false;
                    client.Player.LotteryEntries = 0;
                    client.Player.BDExp = 0;
                    client.Player.ExpBallUsed = 0;
                    client.Player.MysteryFruit = 0;
                    client.Player.TCCaptainTimes = 0;
                    client.DemonExterminator.FinishToday = 0;
                    if (client.Player.MyChi != null && DateTime.Now.DayOfYear > client.Player.Day)
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + Math.Min(((DateTime.Now.DayOfYear - client.Player.Day) * 300), 4000);
                    else
                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 300;

                    client.Player.Flowers.FreeFlowers = 1;
                    foreach (var flower in client.Player.Flowers)
                        flower.Amount2day = 0;

                    if (client.Player.Level >= 90)
                    {
                        client.Player.Enilghten = CalculateEnlighten(client.Player);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Player.SendUpdate(stream, client.Player.Enilghten, Game.MsgServer.MsgUpdate.DataType.EnlightPoints);
                        }
                    }
                    client.Player.QuestGUI.RemoveQuest(35024);
                    client.Player.QuestGUI.RemoveQuest(35007);
                    client.Player.QuestGUI.RemoveQuest(35025);
                    client.Player.QuestGUI.RemoveQuest(35028);
                    client.Player.QuestGUI.RemoveQuest(35034);

                    //---- reset Quests
                    client.Player.QuestGUI.RemoveQuest(6390);
                    client.Player.QuestGUI.RemoveQuest(6329);
                    client.Player.QuestGUI.RemoveQuest(6245);
                    client.Player.QuestGUI.RemoveQuest(6049);
                    client.Player.QuestGUI.RemoveQuest(6366);
                    client.Player.QuestGUI.RemoveQuest(6014);
                    client.Player.QuestGUI.RemoveQuest(2375);
                    client.Player.QuestGUI.RemoveQuest(6126);
                    client.Player.DailyHeavenChance = client.Player.DailyMagnoliaChance
                        = client.Player.DailyMagnoliaItemId
                        = client.Player.DailyHeavenChance = client.Player.DailySpiritBeadCount = client.Player.DailyRareChance = 0;
                    //
                    client.Player.Day = DateTime.Now.DayOfYear;
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        public static ushort CalculateEnlighten(Role.Player player)
        {
            if (player.Level < 90)
                return 0;
            ushort val = 100;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Knight || player.NobilityRank == Role.Instance.Nobility.NobilityRank.Baron)
                val += 100;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Earl || player.NobilityRank == Role.Instance.Nobility.NobilityRank.Duke)
                val += 200;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.Prince)
                val += 300;
            if (player.NobilityRank == Role.Instance.Nobility.NobilityRank.King)
                val += 400;
            if (player.VipLevel <= 3)
                val += 100;
            if (player.VipLevel > 3 && player.VipLevel <= 5)
                val += 200;
            if (player.VipLevel > 5)
                val += 300;

            return val;
        }
        public static void SaveClient(Client.GameClient client)
        {

            try
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.UID + ".ini");

                if ((client.ClientFlag & Client.ServerFlag.LoginFull) != Client.ServerFlag.LoginFull)
                {

                    if (client.Map != null)
                        client.Map.Denquer(client);
                }

                if (HouseTable.InHouse(client.Player.Map) && client.Player.DynamicID != 0 || client.Player.DynamicID != 0)
                {
                    if (client.Socket != null && client.Socket.Alive == false)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 428;
                        client.Player.Y = 378;
                    }
                }
                if ((client.ClientFlag & Client.ServerFlag.Disconnect) == Client.ServerFlag.Disconnect)
                {
                    if (client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 9972
                        || client.Player.Map == 1080 || client.Player.Map == 3820 || client.Player.Map == 3954
                    || client.Player.Map == 1806
                        || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(client.Player.Map) || client.Player.Map == 1508
                        || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map)
                || client.Player.Map == 1768
                || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 1509 || client.Player.Map == 1508 || client.Player.Map == 1507
                 || client.Player.Map == 1801 || client.Player.Map == 1780 || client.Player.Map == 1779 || client.Player.Map == 3071 || client.Player.Map == 1068

                        || client.Player.Map == 3830 || client.Player.Map == 3831 || client.Player.Map == 3832 || client.Player.Map == 3834
                        || client.Player.Map == 3826 || client.Player.Map == 3827 || client.Player.Map == 3828 || client.Player.Map == 3829
                        || client.Player.Map == 3833 || client.Player.Map == 3825
                        || client.Player.Map == 10088 || client.Player.Map == 10089 || client.Player.Map == 10090
                        || client.Player.Map == 44455 || client.Player.Map == 44456 || client.Player.Map == 44457
                         || client.Player.Map == 44460 || client.Player.Map == 44461 || client.Player.Map == 44462 || client.Player.Map == 44463
#if !Poker
                        || client.Player.Map == 1860 || client.Player.Map == 1858
                    
#endif
)
                    {

                        client.Player.Map = 1002;
                        client.Player.X = 428;
                        client.Player.Y = 378;
                    }
#if !Roullete
                    if (client.Player.Map == 3852)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 428;
                        client.Player.Y = 378;
                    }
#endif
                    if (client.Player.Map == 4000 || client.Player.Map == 4003 || client.Player.Map == 4006 || client.Player.Map == 4008 || client.Player.Map == 4009)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 428;
                        client.Player.Y = 378;
                    }
                    if (client.Player.Map == 1038 && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Alive)
                    {
                        client.Player.Map = 1002;
                        client.Player.X = 428;
                        client.Player.Y = 378;
                    }
                }

                if (!client.FullLoading)
                    return;
                write.Write<uint>("Character", "UID", client.Player.UID);
                write.Write<ushort>("Character", "Body", client.Player.Body);
                write.Write<ushort>("Character", "Face", client.Player.Face);
                write.WriteString("Character", "Name", client.Player.Name);
                write.WriteString("Character", "Spouse", client.Player.Spouse);
                write.Write<byte>("Character", "Class", client.Player.Class);
                write.Write<byte>("Character", "FirstClass", client.Player.FirstClass);
                write.Write<byte>("Character", "SecoundeClass", client.Player.SecondClass);
                write.Write<ushort>("Character", "Avatar", client.Player.Avatar);
                write.Write<uint>("Character", "Map", client.Player.Map);
                write.Write<ushort>("Character", "X", client.Player.X);
                write.Write<ushort>("Character", "Y", client.Player.Y);
                write.Write<uint>("Character", "PMap", client.Player.PMap);
                write.Write<ushort>("Character", "PMapX", client.Player.PMapX);
                write.Write<ushort>("Character", "PMapY", client.Player.PMapY);
                write.Write<ushort>("Character", "Agility", client.Player.Agility);
                write.Write<ushort>("Character", "Strength", client.Player.Strength);
                write.Write<ushort>("Character", "Vitaliti", client.Player.Vitality);
                write.Write<ushort>("Character", "Spirit", client.Player.Spirit);
                write.Write<ushort>("Character", "Atributes", client.Player.Atributes);
                write.Write<byte>("Character", "Reborn", client.Player.Reborn);
                write.Write<ushort>("Character", "Level", client.Player.Level);
                write.Write<ushort>("Character", "Haire", client.Player.Hair);
                write.Write<ulong>("Character", "Experience", client.Player.Experience);
                write.Write<int>("Character", "MinHitPoints", client.Player.HitPoints);
                write.Write<ushort>("Character", "MinMana", client.Player.Mana);
                write.Write<uint>("Character", "ConquerPoints", client.Player.ConquerPoints);
                write.Write<int>("Character", "BoundConquerPoints", client.Player.BoundConquerPoints);
                write.Write<uint>("Character", "ArenaCPS", client.Player.ArenaCPS);
                write.Write<long>("Character", "Money", client.Player.Money);
                write.Write<uint>("Character", "VirtutePoints", client.Player.VirtutePoints);
                write.Write<ushort>("Character", "PkPoints", client.Player.PKPoints);
                write.Write<uint>("Character", "JailerUID", client.Player.JailerUID);
                write.Write<uint>("Character", "QuizPoints", client.Player.QuizPoints);
                write.Write<bool>("Character", "HelpDesk", client.HelpDesk);
                write.Write<ushort>("Character", "Enilghten", client.Player.Enilghten);
                write.Write<ushort>("Character", "EnlightenReceive", client.Player.EnlightenReceive);
                write.Write<ulong>("Character", "DailySignUpDays", client.Player.DailySignUpDays);
                write.Write<byte>("Character", "DailyMonth", client.Player.DailyMonth);
                write.Write<byte>("Character", "DailySignUpRewards", client.Player.DailySignUpRewards);
                write.Write<byte>("Character", "VipLevel", client.Player.VipLevel);
                write.Write<long>("Character", "VipTime", client.Player.ExpireVip.Ticks);
                write.Write<long>("Character", "LastDragonPill", client.Player.LastDragonPill.Ticks);
                client.Player.Achievement.Save(client.Achievement);
                write.WriteString("Character", "Achivement", client.Achievement.ToString());
                write.Write<long>("Character", "WHMoney", client.Player.WHMoney);
                write.Write<uint>("Character", "BlessTime", client.Player.BlessTime);
                write.Write<uint>("Character", "SpouseUID", client.Player.SpouseUID);
                write.Write<int>("Character", "HeavenBlessing", client.Player.HeavenBlessing);
                write.Write<uint>("Character", "LostTimeBlessing", client.Player.HeavenBlessTime.Value);
                write.Write<uint>("Character", "HuntingBlessing", client.Player.HuntingBlessing);
                write.Write<uint>("Character", "OnlineTrainingPoints", client.Player.OnlineTrainingPoints);
                write.Write<long>("Character", "JoinOnflineTG", client.Player.JoinOnflineTG.Ticks);
                write.Write<int>("Character", "Day", client.Player.Day);
                write.Write<byte>("Character", "BDExp", client.Player.BDExp);
                write.Write<uint>("Character", "RateExp", client.Player.RateExp);
                write.Write<uint>("Character", "DExpTime", client.Player.DExpTime);
                write.Write<byte>("Character", "ExpBallUsed", client.Player.ExpBallUsed);
                write.Write<byte>("Character", "MysteryFruit", client.Player.MysteryFruit);
                write.WriteString("Character", "SubProfInfo", client.Player.SubClass.ToString());
                write.WriteString("Character", "Dragon", client.Player.MyChi.Dragon.ToString());
                write.WriteString("Character", "Pheonix", client.Player.MyChi.Phoenix.ToString());
                write.WriteString("Character", "Turtle", client.Player.MyChi.Turtle.ToString());
                write.WriteString("Character", "Tiger", client.Player.MyChi.Tiger.ToString());
                write.Write<int>("Character", "ChiPoints", client.Player.MyChi.ChiPoints);

                write.WriteString("Character", "Flowers", client.Player.Flowers.ToString());
                write.Write<ulong>("Character", "DonationNobility", client.Player.Nobility.Donation);
                write.Write<long>("Character", "FreeVIP", client.Player.FreeVIP.Ticks);
                write.Write<uint>("Character", "GuildID", client.Player.GuildID);
                write.Write<ushort>("Character", "GuildRank", (ushort)client.Player.GuildRank);
                if (client.Player.MyGuildMember != null)
                {
                    client.Player.MyGuildMember.LastLogin = DateTime.Now.Ticks;
                    write.Write<uint>("Character", "CpsDonate", client.Player.MyGuildMember.CpsDonate);
                    write.Write<long>("Character", "MoneyDonate", client.Player.MyGuildMember.MoneyDonate);
                    write.Write<uint>("Character", "PkDonation", client.Player.MyGuildMember.PkDonation);
                    write.Write<long>("Character", "LastLogin", client.Player.MyGuildMember.LastLogin);

                    write.Write<uint>("Character", "CTF_Exploits", client.Player.MyGuildMember.CTF_Exploits);
                    write.Write<uint>("Character", "CTF_RCPS", client.Player.MyGuildMember.RewardConquerPoints);
                    write.Write<uint>("Character", "CTF_RM", client.Player.MyGuildMember.RewardMoney);
                    write.Write<byte>("Character", "CTF_R", client.Player.MyGuildMember.CTF_Claimed);
                }
                if (client.Player.MyClan != null)
                {
                    write.Write<uint>("Character", "ClanID", client.Player.MyClan.ID);
                    write.Write<ushort>("Character", "ClanRank", client.Player.ClanRank);
                    if (client.Player.MyClanMember != null)
                        write.Write<uint>("Character", "ClanDonation", client.Player.MyClanMember.Donation);
                }
                write.Write<byte>("Character", "FRL", client.Player.FirstRebornLevel);
                write.Write<byte>("Character", "SRL", client.Player.SecoundeRebornLevel);
                write.Write<bool>("Character", "Reincanation", client.Player.Reincarnation);
                write.Write<byte>("Character", "LotteryEntries", client.Player.LotteryEntries);
                write.Write<bool>("Character", "DbTry", client.Player.DbTry);
                write.WriteString("Character", "DemonEx", client.DemonExterminator.ToString());
                write.WriteString("Character", "PkName", client.Player.MyKillerName);
                write.Write<uint>("Character", "PkUID", client.Player.MyKillerUID);
                write.Write<int>("Character", "Cursed", client.Player.CursedTimer);
                write.Write<uint>("Character", "AparenceType", client.Player.AparenceType);
                write.Write<uint>("Character", "TKills", client.Player.TournamentKills);
                write.Write<uint>("Character", "OnlineMinutes", client.Player.OnlineMinutes);
                write.Write<uint>("Character", "HistoryChampionPoints", client.Player.HistoryChampionPoints);
                write.Write<uint>("Character", "TodayChampionPoints", client.Player.TodayChampionPoints);
                write.Write<uint>("Character", "ChampionPoints", client.Player.ChampionPoints);
                write.Write<uint>("Character", "DailySpiritBeadItem", client.Player.DailySpiritBeadItem);
                write.WriteString("Character", "SecurityPass", GetSecurityPassword(client));
                write.Write<byte>("Character", "TCT", (byte)client.Player.TCCaptainTimes);
                write.Write<uint>("Character", "RacePoints", client.Player.DonationPoints);
                write.Write<ushort>("Character", "NameEditCount", client.Player.NameEditCount);
                write.Write<uint>("Character", "ClaimStateGift", (uint)client.Player.MainFlag);
                write.Write<uint>("Character", "enervant", client.Player.AtiveQuestApe);
                write.Write<ushort>("Character", "InventorySashCount", client.Player.InventorySashCount);
                write.Write<ushort>("Character", "CountryID", client.Player.CountryID);
                write.Write<uint>("Character", "MyFootBallPoints", client.Player.MyFootBallPoints);
                write.Write<uint>("Character", "ExpProtection", client.Player.ExpProtection);
                write.Write<uint>("Character", "BanCount", client.BanCount);
                write.Write<ushort>("Character", "ExtraAtributes", client.Player.ExtraAtributes);
                write.Write<byte>("Character", "OpenHousePack", client.Player.OpenHousePack);
                write.Write<long>("Character", "JPAStamp", client.Player.JoinPowerArenaStamp.Ticks);
                write.Write<int>("Character", "GiveFlowersToPerformer", client.Player.GiveFlowersToPerformer);
                write.Write<byte>("Character", "UseChiToken", client.Player.UseChiToken);
                write.Write<string>("Character", "Agates", client.Player.AgatesString());
                write.Write<uint>("Character", "OnlinePoints", client.Player.OnlinePoints);
                write.Write<uint>("Character", "PVPPoints", client.Player.PVPPoints);
                write.Write<uint>("Character", "PVEPoints", client.Player.PVEPoints);
                write.Write<uint>("Character", "TotalMobsKilled", client.TotalMobsKilled);
                write.Write<uint>("Character", "TotalSouls", client.TotalSouls);
                write.Write<int>("Character", "DragonPills", client.Player.DragonPills);
                write.Write<int>("Character", "BossPoints", client.Player.BossPoints);


                SaveClientItems(client);
                SaveClientSpells(client);
                SaveClientProfs(client);
                RoleQuests.Save(client);
                Role.Instance.House.Save(client);
                Save(new Action(NobilityTable.Save));
                if ((client.ClientFlag & Client.ServerFlag.Disconnect) == Client.ServerFlag.Disconnect)
                {
                    Client.GameClient user;
                    Database.Server.GamePoll.TryRemove(client.Player.UID, out user);
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

        }
        public static void Save(Action obj)
        {
            try
            {
                obj.Invoke();
            }
            catch (Exception e) { Console.WriteLine(e); }
        }
        public static string GetSecurityPassword(Client.GameClient user)
        {
            Database.DBActions.WriteLine writer = new DBActions.WriteLine(',');
            writer.Add(user.Player.SecurityPassword);
            writer.Add(user.Player.OnReset);
            writer.Add(user.Player.ResetSecurityPassowrd.Ticks);
            return writer.Close();
        }
        public static void LoadSecurityPassword(string line, Client.GameClient user)
        {
            Database.DBActions.ReadLine reader = new DBActions.ReadLine(line, ',');
            user.Player.SecurityPassword = reader.Read((uint)0);
            user.Player.OnReset = reader.Read((uint)0);
            if (user.Player.OnReset == 1)
            {
                user.Player.ResetSecurityPassowrd = DateTime.FromBinary(reader.Read((long)0));
                if (DateTime.Now > user.Player.ResetSecurityPassowrd)
                {
                    user.Player.OnReset = 0;
                    user.Player.SecurityPassword = 0;
                }
            }

        }


        public static void LoadCharacter(Client.GameClient client, uint UID)
        {
            client.Player.UID = UID;
            WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Users\\" + UID + ".ini");
            client.Player.Body = reader.ReadUInt16("Character", "Body", 1002);
            client.Player.Face = reader.ReadUInt16("Character", "Face", 0);
            client.Player.Name = reader.ReadString("Character", "Name", "None");
            client.Player.Spouse = reader.ReadString("Character", "Spouse", "None");
            client.Player.Class = reader.ReadByte("Character", "Class", 0);
            client.Player.FirstClass = reader.ReadByte("Character", "FirstClass", 0);
            client.Player.SecondClass = reader.ReadByte("Character", "SecoundeClass", 0);
            client.Player.Avatar = reader.ReadUInt16("Character", "Avatar", 0);
            client.Player.Map = reader.ReadUInt32("Character", "Map", 1002);
            client.Player.X = reader.ReadUInt16("Character", "X", 248);
            client.Player.Y = reader.ReadUInt16("Character", "Y", 238);

            client.Player.PMap = reader.ReadUInt32("Character", "PMap", 1002);
            client.Player.PMapX = reader.ReadUInt16("Character", "PMapX", 300);
            client.Player.PMapY = reader.ReadUInt16("Character", "PMapY", 300);

            client.Player.Agility = reader.ReadUInt16("Character", "Agility", 0);
            client.Player.Strength = reader.ReadUInt16("Character", "Strength", 0);
            client.Player.Spirit = reader.ReadUInt16("Character", "Spirit", 0);
            client.Player.Vitality = reader.ReadUInt16("Character", "Vitaliti", 0);
            client.Player.Atributes = reader.ReadUInt16("Character", "Atributes", 0);
            client.Player.Reborn = reader.ReadByte("Character", "Reborn", 0);
            client.Player.Level = reader.ReadUInt16("Character", "Level", 0);
            client.Player.Hair = reader.ReadUInt16("Character", "Haire", 0);
            client.Player.Experience = (ulong)reader.ReadInt64("Character", "Experience", 0);
            client.Player.HitPoints = reader.ReadInt32("Character", "MinHitPoints", 0);
            client.Player.Mana = reader.ReadUInt16("Character", "MinMana", 0);
            client.Player.ConquerPoints = reader.ReadUInt32("Character", "ConquerPoints", 0);
            client.Player.ArenaCPS = reader.ReadUInt32("Character", "ArenaCPS", 0);
            
            client.HelpDesk = reader.ReadBool("Character", "HelpDesk", false);
            client.Player.BoundConquerPoints = reader.ReadInt32("Character", "BoundConquerPoints", 0);
            client.Player.Money = reader.ReadUInt32("Character", "Money", 0);
            client.Player.VirtutePoints = reader.ReadUInt32("Character", "VirtutePoints", 0);
            client.Player.PKPoints = reader.ReadUInt16("Character", "PkPoints", 0);
            client.Player.JailerUID = reader.ReadUInt32("Character", "JailerUID", 0);
            client.Player.QuizPoints = reader.ReadUInt32("Character", "QuizPoints", 0);
            client.Player.Enilghten = reader.ReadUInt16("Character", "Enilghten", 0);
            client.Player.EnlightenReceive = reader.ReadUInt16("Character", "EnlightenReceive", 0);
            client.Player.DailySignUpDays = reader.ReadUInt64("Character", "DailySignUpDays", 0);
            client.Player.DailyMonth = reader.ReadByte("Character", "DailyMonth", 0);
            client.Player.DailySignUpRewards = reader.ReadByte("Character", "DailySignUpRewards", 0);
            client.Player.VipLevel = reader.ReadByte("Character", "VipLevel", 0);
            client.Player.ExpireVip = DateTime.FromBinary(reader.ReadInt64("Character", "VipTime", 0));
            client.Player.LastDragonPill = DateTime.FromBinary(reader.ReadInt64("Character", "LastDragonPill", 0));
            if (DateTime.Now > client.Player.ExpireVip)
            {
                if (client.Player.VipLevel > 1)
                    client.Player.VipLevel = 0;
            }
            client.Achievement = new AchievementCollection();
            client.Achievement.Load(reader.ReadString("Character", "Achivement", ""));

            client.Player.WHMoney = reader.ReadInt64("Character", "WHMoney", 0);
            client.Player.BlessTime = reader.ReadUInt32("Character", "BlessTime", 0);
            client.Player.SpouseUID = reader.ReadUInt32("Character", "SpouseUID", 0);
            client.Player.HeavenBlessing = reader.ReadInt32("Character", "HeavenBlessing", 0);
            client.Player.HeavenBlessTime = new Extensions.Time32(reader.ReadUInt32("Character", "LostTimeBlessing", 0));
            client.Player.HuntingBlessing = reader.ReadUInt32("Character", "HuntingBlessing", 0);
            client.Player.OnlineTrainingPoints = reader.ReadUInt32("Character", "OnlineTrainingPoints", 0);
            client.Player.JoinOnflineTG = DateTime.FromBinary(reader.ReadInt64("Character", "JoinOnflineTG", 0));
            client.Player.RateExp = reader.ReadUInt32("Character", "RateExp", 0);
            client.Player.DExpTime = reader.ReadUInt32("Character", "DExpTime", 0);
            client.Player.Day = reader.ReadInt32("Character", "Day", 0);
            client.Player.BDExp = reader.ReadByte("Character", "BDExp", 0);
            client.Player.ExpBallUsed = reader.ReadByte("Character", "ExpBallUsed", 0);
            client.Player.MysteryFruit = reader.ReadByte("Character", "MysteryFruit", 0);
            client.Player.FreeVIP = DateTime.FromBinary(reader.ReadInt64("Character", "FreeVIP", 0));
            DataCore.LoadClient(client.Player);
            client.Player.GuildID = reader.ReadUInt32("Character", "GuildID", 0);
            client.Player.GuildRank = (Role.Flags.GuildMemberRank)reader.ReadUInt32("Character", "GuildRank", 200);
            if (client.Player.GuildID != 0)
            {
                Role.Instance.Guild myguild;
                if (Role.Instance.Guild.GuildPoll.TryGetValue(client.Player.GuildID, out myguild))
                {
                    client.Player.MyGuild = myguild;
                    Role.Instance.Guild.Member member;
                    if (myguild.Members.TryGetValue(client.Player.UID, out member))
                    {
                        member.IsOnline = true;
                        client.Player.GuildID = (ushort)myguild.Info.GuildID;
                        client.Player.MyGuildMember = member;
                        client.Player.GuildRank = member.Rank;
                        client.Player.GuildBattlePower = myguild.ShareMemberPotency(member.Rank);


                    }
                    else
                    {
                        client.Player.MyGuild = null;
                        client.Player.GuildID = 0;
                        client.Player.GuildRank = (Role.Flags.GuildMemberRank)0;
                    }
                }
                else
                {
                    client.Player.MyGuild = null;
                    client.Player.GuildID = 0;
                    client.Player.GuildRank = (Role.Flags.GuildMemberRank)0;
                }
            }



            client.Player.SubClass = new Role.Instance.SubClass();
            client.Player.SubClass.Load(reader.ReadString("Character", "SubProfInfo", ""));
            client.Player.SubClass.CreateSpawn(client);

            if (Role.Instance.Chi.ChiPool.ContainsKey(UID))
            {
                client.Player.MyChi = Role.Instance.Chi.ChiPool[UID];
                Role.Instance.Chi.ComputeStatus(client.Player.MyChi);
            }
            else
                client.Player.MyChi = new Role.Instance.Chi(UID);

            if (Role.Instance.Flowers.ClientPoll.ContainsKey(UID))
                client.Player.Flowers = Role.Instance.Flowers.ClientPoll[UID];
            else
                client.Player.Flowers = new Role.Instance.Flowers(UID, client.Player.Name);
            string flowerStr = reader.ReadString("Character", "Flowers", "");
            Database.DBActions.ReadLine Linereader = new DBActions.ReadLine(flowerStr, '/');
            client.Player.Flowers.FreeFlowers = Linereader.Read((uint)0);



            //Role.Instance.Nobility nobility;
            //if (Program.NobilityRanking.TryGetValue(UID, out nobility))
            //{
            //    client.Player.Nobility = nobility;
            //    client.Player.NobilityRank = client.Player.Nobility.Rank;
            //}
            //else
            //{
            //    client.Player.Nobility = new Role.Instance.Nobility(client);
            //    client.Player.Nobility.Donation = reader.ReadUInt64("Character", "DonationNobility", 0);
            //    client.Player.NobilityRank = client.Player.Nobility.Rank;
            //}
            NobilityTable.LoadClient(client);
            Role.Instance.Associate.MyAsociats Associate;
            if (Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out Associate))
            {
                client.Player.Associate = Associate;
                client.Player.Associate.MyClient = client;
                client.Player.Associate.Online = true;
                if (client.Player.Associate.Associat.ContainsKey(Role.Instance.Associate.Mentor))
                {
                    foreach (var member in client.Player.Associate.Associat[Role.Instance.Associate.Mentor].Values)
                    {
                        if (member.UID != 0)
                        {
                            Role.Instance.Associate.MyAsociats mentor;
                            if (Role.Instance.Associate.Associates.TryGetValue(member.UID, out mentor))
                            {
                                client.Player.MyMentor = mentor;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                client.Player.Associate.MyClient = client;
                client.Player.Associate.Online = true;
            }
            client.Player.ClanUID = reader.ReadUInt32("Character", "ClanID", 0);
            if (client.Player.ClanUID != 0)
            {
                Role.Instance.Clan myclan;
                if (Role.Instance.Clan.Clans.TryGetValue(client.Player.ClanUID, out myclan))
                {
                    client.Player.MyClan = myclan;
                    Role.Instance.Clan.Member member;
                    if (myclan.Members.TryGetValue(client.Player.UID, out member))
                    {
                        member.Online = true;
                        client.Player.ClanName = myclan.Name;
                        client.Player.MyClanMember = member;
                        client.Player.ClanRank = (ushort)member.Rank;
                    }
                    else
                    {
                        client.Player.MyClan = null;
                        client.Player.ClanUID = 0;
                        client.Player.ClanRank = 0;
                    }
                }
                else
                    client.Player.ClanUID = 0;
            }
            client.Player.FirstRebornLevel = reader.ReadByte("Character", "FRL", 0);
            client.Player.SecoundeRebornLevel = reader.ReadByte("Character", "SRL", 0);
            client.Player.Reincarnation = reader.ReadBool("Character", "Reincanation", false);
            client.Player.LotteryEntries = reader.ReadByte("Character", "LotteryEntries", 0);
            client.Player.DbTry = reader.ReadBool("Character", "DbTry", false);
            client.DemonExterminator.ReadLine(reader.ReadString("Character", "DemonEx", "0/0/"));
            client.Player.MyKillerUID = reader.ReadUInt32("Character", "PkName", 0);
            client.Player.MyKillerName = reader.ReadString("Character", "PkName", "None");
            client.Player.CursedTimer = reader.ReadInt32("Character", "Cursed", 0);
            client.Player.AtiveQuestApe = reader.ReadUInt32("Character", "enervant", 0);
            client.Player.AparenceType = reader.ReadUInt32("Character", "AparenceType", 0);
            client.Player.TournamentKills = reader.ReadUInt32("Character", "TKills", 0);
            client.Player.OnlineMinutes = reader.ReadUInt32("Character", "OnlineMinutes", 0);
            client.Player.HistoryChampionPoints = reader.ReadUInt32("Character", "HistoryChampionPoints", 0);
            client.Player.AddChampionPoints(reader.ReadUInt32("Character", "ChampionPoints", 0), false);
            client.Player.TodayChampionPoints = reader.ReadUInt32("Character", "TodayChampionPoints", 0);
            client.Player.DailySpiritBeadItem = reader.ReadUInt32("Character", "DailySpiritBeadItem", 0);
            LoadSecurityPassword(reader.ReadString("Character", "SecurityPass", "0,0,0"), client);
            client.Player.TCCaptainTimes = reader.ReadByte("Character", "TCT", 0);
            client.Player.DonationPoints = reader.ReadUInt32("Character", "RacePoints", 0);
            client.Player.NameEditCount = reader.ReadUInt16("Character", "NameEditCount", 0);
            client.Player.MainFlag = (Role.Player.MainFlagType)reader.ReadUInt32("Character", "ClaimStateGift", 0);
            client.Player.CountryID = reader.ReadUInt16("Character", "CountryID", 0);
            client.Player.InventorySashCount = reader.ReadUInt16("Character", "InventorySashCount", 0);
            client.Player.MyFootBallPoints = reader.ReadUInt32("Character", "MyFootBallPoints", 0);
            client.Player.ExpProtection = reader.ReadUInt32("Character", "ExpProtection", 0);
            client.BanCount = reader.ReadByte("Character", "BanCount", 0);
            client.Player.ExtraAtributes = reader.ReadUInt16("Character", "ExtraAtributes", 0);
            client.Player.OpenHousePack = reader.ReadByte("Character", "OpenHousePack", 0);
            client.Player.JoinPowerArenaStamp = DateTime.FromBinary(reader.ReadInt64("Character", "JPAStamp", 0));
            client.Player.GiveFlowersToPerformer = reader.ReadInt32("Character", "GiveFlowersToPerformer", 0);
            client.Player.UseChiToken = reader.ReadByte("Character", "UseChiToken", 0);
            client.Player.OnlinePoints = reader.ReadUInt32("Character", "OnlinePoints", 0);
            client.TotalMobsKilled = reader.ReadUInt32("Character", "TotalMobsKilled", 0);
            client.TotalSouls = reader.ReadUInt32("Character", "TotalSouls", 0);
            client.Player.DragonPills = reader.ReadInt32("Character", "DragonPills", 0);
            client.Player.BossPoints = reader.ReadInt32("Character", "BossPoints", 0);
            client.Player.PVEPoints = reader.ReadUInt32("Character", "PVEPoints", 0);
            LoadClientItems(client);
            client.Player.LoadAgates(reader.ReadString("Character", "Agates", ""));
            LoadClientSpells(client);
            LoadClientProfs(client);
            RoleQuests.Load(client);
            Role.Instance.House.Load(client);
            client.MaxDBMobs = Role.Core.Random.Next(400, 700);
            ResetingEveryDay(client);


            Role.Instance.Confiscator Container;
            if (Server.QueueContainer.PollContainers.TryGetValue(client.Player.UID, out Container))
                client.Confiscator = Container;
            try
            {
                client.Player.Associate.OnLoading(client);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            if (Game.MsgTournaments.MsgArena.ArenaPoll.TryGetValue(client.Player.UID, out client.ArenaStatistic))
            {
                client.ArenaStatistic.ApplayInfo(client.Player);
            }
            else
            {
                client.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
                client.ArenaStatistic.ApplayInfo(client.Player);
                client.ArenaStatistic.Info.ArenaPoints = 4000;
                Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);
            }

            if (Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryGetValue(client.Player.UID, out client.TeamArenaStatistic))
            {
                client.TeamArenaStatistic.ApplayInfo(client.Player);
            }
            else
            {
                client.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
                client.TeamArenaStatistic.ApplayInfo(client.Player);
                client.TeamArenaStatistic.Info.ArenaPoints = 4000;
                Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);
            }
            client.FullLoading = true;
        }
        public unsafe static void LoadDBPackets()
        {
            Program.LoadPackets.Clear();
            var arraybuffer = File.ReadAllBytes(Program.ServerConfig.DbLocation + "\\array0.bin");
            int count = BitConverter.ToInt32(arraybuffer, 0);

            int dd = 0;
            int offset = 4;
            for (int x = 0; x < count; x++)
            {
                try
                {

                    ushort size = BitConverter.ToUInt16(arraybuffer, offset);
                    byte[] packet = new byte[size];
                    Buffer.BlockCopy(arraybuffer, offset, packet, 0, size);
                    offset += size;
                    dd++;
                    //   if (dd >= 2)
                    {
                        Program.LoadPackets.Add(packet);
                    }
                }
                catch
                {
                    //    break;
                }
            }

            /* WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
             if (binary.Open(Program.ServerConfig.DbLocation + "\\array320.bin", System.IO.FileMode.Open))
             {
                 int count;
                 binary.Read(&count, 4);
                 for (int x = 0; x < count; x++)
                 {
                     int size;
                     binary.Read(&size, sizeof(ushort));

                     byte[] buff = new byte[size];
                     int size2 = size;
                     byte* ptr = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
                     binary.Read(&ptr, size2);

                     fixed (byte* tt = buff)
                     {
                         WindowsAPI.Kernel32.memcpy(tt, ptr, size2);
                     }
                     Program.LoadPackets.Add(buff);
                 }
                 binary.Close();
             }*/

        }
        public unsafe static void Testtt()
        {
            Dictionary<string, Dictionary<uint, Game.MsgServer.MsgGameItem>> Clients = new Dictionary<string, Dictionary<uint, MsgGameItem>>();

            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");

            Dictionary<uint, string> Users = new Dictionary<uint, string>();
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
            {
                ini.FileName = fname;

                uint UID = ini.ReadUInt32("Character", "UID", 0);
                string Name = ini.ReadString("Character", "Name", "None");
                uint Cps = ini.ReadUInt32("Character", "ConquerPoints", 0);
                uint money = ini.ReadUInt32("Character", "WHMoney", 0);
                if (Users.ContainsKey(UID))
                    Users.Add(UID, Name);
                if (Cps > 500000)
                //  if (Cps > 1000000 * 100)
                {
                    Console.WriteLine("" + UID + " " + Name + " " + Cps);
                }
            }
            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\PlayersItems\\"))
            {
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (binary.Open(fname, FileMode.Open))
                {
                    ClientItems.DBItem Item;
                    int ItemCount;
                    binary.Read(&ItemCount, sizeof(int));


                    Clients.Add(fname, new Dictionary<uint, MsgGameItem>());


                    for (int x = 0; x < ItemCount; x++)
                    {
                        binary.Read(&Item, sizeof(ClientItems.DBItem));

                        Game.MsgServer.MsgGameItem ClienItem = Item.GetDataItem();
                        if (Clients[fname].ContainsKey(ClienItem.UID) == false)
                            Clients[fname].Add(ClienItem.UID, ClienItem);

                    }
                    binary.Close();
                }
            }
            foreach (var user in Clients)
            {
                uint ItemId = 3004247;
                uint Amount = 0;
                uint Amountp7 = 0;
                uint amounta = 0;
                foreach (var item in user.Value.Values)
                {
                    if (ItemId == item.ITEM_ID)
                    {
                        Amount += 1;
                    }
                    if (Database.ItemType.PurificationItems[7].ContainsKey(item.ITEM_ID))
                    {
                        //   Console.WriteLine(user.Key);
                        Amountp7 += 1;
                    }
                    if (item.StackSize >= 10)
                    {
                        Console.WriteLine(user.Key + " " + item.StackSize + " " + item.ITEM_ID);
                    }
                    if (item.Purification.PurificationItemID != 0)
                        amounta += 1;
                }
                if (Amount > 0)
                    Console.WriteLine(user.Key + " amount " + Amount);
                if (Amountp7 > 0)
                    Console.WriteLine(user.Key + " amount souls " + Amountp7);
                if (amounta > 0)
                    Console.WriteLine("wtf " + amounta + "");
            }
        }



        public unsafe static void LoadClientItems(Client.GameClient client)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                ClientItems.DBItem Item;
                int ItemCount;
                binary.Read(&ItemCount, sizeof(int));
                for (int x = 0; x < ItemCount; x++)
                {
                    binary.Read(&Item, sizeof(ClientItems.DBItem));
                    if (Item.ITEM_ID == 750000)//demonExterminator jar
                        client.DemonExterminator.ItemUID = Item.UID;

                    Game.MsgServer.MsgGameItem ClienItem = Item.GetDataItem();
                    if (Database.ItemType.ItemPosition(Item.ITEM_ID) != (ushort)Role.Flags.ConquerItem.RidingCrop)
                    {
                        if (Item.Bless > 1)
                            Item.Bless = 1;
                    }
                    if (Item.ITEM_ID == 720598 || (Item.ITEM_ID >= 2100065 && Item.ITEM_ID <= 2100095))
                        ClienItem.Bound = 1;
                    if (Item.WH_ID != 0)
                    {
                        if (Item.WH_ID == 100)
                        {
                            if (Item.Position > 0 && Item.Position <= (ushort)Role.Flags.ConquerItem.AleternanteGarment)
                            {
                                client.Equipment.ClientItems.TryAdd(Item.UID, ClienItem);
                            }
                        }
                        else
                        {
                            if (!client.Warehouse.ClientItems.ContainsKey(Item.WH_ID))
                                client.Warehouse.ClientItems.TryAdd(Item.WH_ID, new System.Collections.Concurrent.ConcurrentDictionary<uint, Game.MsgServer.MsgGameItem>());
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Warehouse.ClientItems[Item.WH_ID].TryAdd(Item.UID, ClienItem);
                        }
                    }
                    else
                    {
                        if (Item.Position > 0 && Item.Position <= (ushort)Role.Flags.ConquerItem.AleternanteGarment)
                        {
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Equipment.ClientItems.TryAdd(Item.UID, ClienItem);
                        }
                        else if (Item.Position == 0)
                        {
                            if (client.Player.GuildID == 0)
                                ClienItem.Inscribed = 0;
                            client.Inventory.AddDBItem(ClienItem);
                        }
                    }
                }
                binary.Read(&ItemCount, sizeof(int));
                binary.Close();
            }
        }

        public unsafe static void SaveClientItems(Client.GameClient client)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                ClientItems.DBItem DBItem = new ClientItems.DBItem();
                int ItemCount;
                ItemCount = client.GetItemsCount();
                binary.Write(&ItemCount, sizeof(int));
                foreach (var item in client.AllMyItems())
                {
                    if (item.Position == 0 && item.WH_ID == 0)
                    {
                        var pos = Database.ItemType.ItemPosition(item.ITEM_ID);
                        if (pos == (ushort)Role.Flags.ConquerItem.Garment)
                        {

                        }
                    }
                    DBItem.GetDBItem(item);
                    if (!binary.Write(&DBItem, sizeof(ClientItems.DBItem)))
                        Console.WriteLine("test");
                }
                binary.Write(&ItemCount, sizeof(int));
                binary.Close();
            }
        }
        public unsafe static void LoadClientProfs(Client.GameClient client)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                ClientProficiency.DBProf DBProf;
                int CountProf;
                binary.Read(&CountProf, sizeof(int));
                for (int x = 0; x < CountProf; x++)
                {
                    binary.Read(&DBProf, sizeof(ClientProficiency.DBProf));
                    var ClientProf = DBProf.GetClientProf();
                    client.MyProfs.ClientProf.TryAdd(ClientProf.ID, ClientProf);
                }
                binary.Close();
            }
        }
        public unsafe static void SaveClientProfs(Client.GameClient client)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                ClientProficiency.DBProf DBProf = new ClientProficiency.DBProf();
                int CountProf;
                CountProf = client.MyProfs.ClientProf.Count;
                binary.Write(&CountProf, sizeof(int));
                foreach (var prof in client.MyProfs.ClientProf.Values)
                {
                    DBProf.GetDBSpell(prof);
                    binary.Write(&DBProf, sizeof(ClientProficiency.DBProf));
                }
                binary.Close();
            }
        }
        public unsafe static void LoadClientSpells(Client.GameClient client)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin", FileMode.Open))
            {
                ClientSpells.DBSpell DBSpell;
                int CountSpell;
                binary.Read(&CountSpell, sizeof(int));
                for (int x = 0; x < CountSpell; x++)
                {
                    binary.Read(&DBSpell, sizeof(ClientSpells.DBSpell));
                    var clientSpell = DBSpell.GetClientSpell();
                    client.MySpells.ClientSpells.TryAdd(clientSpell.ID, clientSpell);
                }
                binary.Close();
            }
        }
        public unsafe static void SaveClientSpells(Client.GameClient client)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin", FileMode.Create))
            {
                ClientSpells.DBSpell DBSpell = new ClientSpells.DBSpell();
                int SpellCount;
                SpellCount = client.MySpells.ClientSpells.Count;
                binary.Write(&SpellCount, sizeof(int));
                foreach (var spell in client.MySpells.ClientSpells.Values)
                {
                    DBSpell.GetDBSpell(spell);
                    binary.Write(&DBSpell, sizeof(ClientSpells.DBSpell));
                }
                binary.Close();
            }
        }
        public static void CreateCharacte(Client.GameClient client)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.UID + ".ini");
            write.Write<uint>("Character", "UID", client.Player.UID);
            write.Write<ushort>("Character", "Body", client.Player.Body);
            write.Write<ushort>("Character", "Face", client.Player.Face);
            write.WriteString("Character", "Name", client.Player.Name);
            write.Write<byte>("Character", "Class", client.Player.Class);
            write.Write<uint>("Character", "Map", client.Player.Map);
            write.Write<ushort>("Character", "X", client.Player.X);
            write.Write<ushort>("Character", "Y", client.Player.Y);

            client.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
            client.ArenaStatistic.ApplayInfo(client.Player);
            client.ArenaStatistic.Info.ArenaPoints = 4000;
            Game.MsgTournaments.MsgArena.ArenaPoll.TryAdd(client.Player.UID, client.ArenaStatistic);

            client.Player.Nobility = new Role.Instance.Nobility(client);

            client.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
            client.TeamArenaStatistic.ApplayInfo(client.Player);
            client.TeamArenaStatistic.Info.ArenaPoints = 4000;

            Game.MsgTournaments.MsgTeamArena.ArenaPoll.TryAdd(client.Player.UID, client.TeamArenaStatistic);

            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
            client.Player.Associate.MyClient = client;
            client.Player.Associate.Online = true;


            client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
            client.Player.SubClass = new Role.Instance.SubClass();
            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);
            client.Achievement = new AchievementCollection();

            client.FullLoading = true;

        }

        public static bool AllowCreate(uint UID)
        {
            return !File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + UID + ".ini");
            // WindowsAPI.IniFile reader = new WindowsAPI.IniFile("\\Accounts\\" + Account + ".ini");
            // return reader.ReadUInt32("Account", "UID", 1000000) == 1000000;
        }
        public static void UpdateGuildMember(Role.Instance.Guild.Member Member)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
            //  write.Write<uint>("Character", "GuildID", 0);
            write.Write<ushort>("Character", "GuildRank", 0);
        }
        public static void UpdateGuildMember(Role.Instance.Guild.UpdateDB Member)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
            write.Write<ushort>("Character", "GuildRank", 0);
            write.Write<ushort>("Character", "GuildID", 0);
        }

        public static void UpdateMapRace(Role.GameMap map)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\maps\\" + map.ID + ".ini");
            write.Write<uint>("info", "race_record", map.RecordSteedRace);
        }
        public static void UpdateClanMember(Role.Instance.Clan.Member Member)
        {
            WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + Member.UID + ".ini");
            write.Write<uint>("Character", "ClanID", 0);
            write.Write<ushort>("Character", "ClanRank", 0);
            write.Write<uint>("Character", "ClanDonation", 0);
        }
        public static void DestroySpouse(Client.GameClient client)
        {
            if (client.Player.SpouseUID != 0)
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.SpouseUID + ".ini");
                write.Write<uint>("Character", "SpouseUID", 0);
                write.WriteString("Character", "Spouse", "None");

                client.Player.SpouseUID = 0;
            }

            client.Player.Spouse = "None";
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                client.Player.SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Spouse, false, new string[1] { "None" });
            }
        }
        public static string GenerateDate()
        {
            DateTime now = DateTime.Now;
            return now.Year.ToString() + "and" + now.Month.ToString() + "and" + now.Day.ToString() + "and" + now.Hour.ToString() + "and" + now.Minute.ToString() + "and" + now.Second.ToString();
        }
        public static void UpdateSpouse(Client.GameClient client)
        {
            if (client.Player.SpouseUID != 0)
            {
                WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Users\\" + client.Player.SpouseUID + ".ini");
                write.WriteString("Character", "Spouse", client.Player.Name);
            }
        }
        public static ExecuteLogin LoginQueue = new ExecuteLogin();

        public class ExecuteLogin : ConcurrentSmartThreadQueue<object>
        {
            public object SynRoot = new object();
            public ExecuteLogin()
                : base(5)
            {
                Start(10);
            }
            public void TryEnqueue(object obj)
            {
                //  lock (SynRoot)
                {

                    base.Enqueue(obj);
                }
            }
            protected unsafe override void OnDequeue(object obj, int time)
            {
                try
                {

                    if (obj is string)
                    {
                        string text = obj as string;
                        if (text.StartsWith("[DemonBox]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "DemonBox" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Chat]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Chat" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[CHEAT]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "CHEAT" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Item]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Item" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[CallStack]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "CallStack" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[PVEPoints]"))
                        {

                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "PVEPoints" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        else if (text.StartsWith("[Poker]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Poker" + dt.Month + "-" + dt.Day + "";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        if (text.StartsWith("[Dev]") || text.StartsWith("[Helper]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";

                            var dt = DateTime.Now;
                            string date = "Dev";

                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        if (text.StartsWith("[TradeConquerPoints]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";
                            var dt = DateTime.Now;
                            string date = "TradeConquerPoints";
                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        if (text.StartsWith("[TradeMoney]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";
                            var dt = DateTime.Now;
                            string date = "TradeMoney";
                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        if (text.StartsWith("[TradeItem]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";
                            var dt = DateTime.Now;
                            string date = "TradeItem";
                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }
                        if (text.StartsWith("[FullTrade]"))
                        {
                            const string UnhandledExceptionsPath = "Loggs\\";
                            var dt = DateTime.Now;
                            string date = "FullTrade";
                            if (!Directory.Exists(Application.StartupPath + UnhandledExceptionsPath))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);
                            if (!Directory.Exists(Application.StartupPath + "\\" + UnhandledExceptionsPath + date))
                                Directory.CreateDirectory(Application.StartupPath + "\\" + UnhandledExceptionsPath + date);

                            string fullPath = Application.StartupPath + "\\" + UnhandledExceptionsPath + date + "\\";

                            if (!File.Exists(fullPath + date + ".txt"))
                            {
                                File.WriteAllLines(fullPath + date + ".txt", new string[0]);
                            }


                            using (var SW = File.AppendText(fullPath + date + ".txt"))
                            {
                                SW.WriteLine(text);
                                SW.Close();
                            }
                        }

                    }
                    else if (obj is Role.GameMap)
                    {
                        UpdateMapRace(obj as Role.GameMap);
                    }
                    else if (obj is Role.Instance.Guild.Member)
                    {
                        UpdateGuildMember(obj as Role.Instance.Guild.Member);
                    }
                    else if (obj is Role.Instance.Guild.UpdateDB)
                    {
                        UpdateGuildMember(obj as Role.Instance.Guild.UpdateDB);
                    }

                    else if (obj is Role.Instance.Clan.Member)
                    {
                        UpdateClanMember(obj as Role.Instance.Clan.Member);
                    }
                    else
                    {
                        Client.GameClient client = obj as Client.GameClient;
                        if (client.Player != null && client.Player.Delete)
                        {
                            if (client.Map != null)
                                client.Map.View.LeaveMap<Role.Player>(client.Player);

                            DateTime Now64 = DateTime.Now;

                            Console.WriteLine("Client " + client.Player.Name + " delete he account.");
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini"))
                                File.Copy(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini", Program.ServerConfig.DbLocation + "\\BackUp\\Users\\" + client.Player.UID + "date" + GenerateDate() + ".ini", true);
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin"))
                                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\BackUp\\PlayersSpells\\" + client.Player.UID + "date" + GenerateDate() + ".bin", true);
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin"))
                                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\BackUp\\PlayersProfs\\" + client.Player.UID + "date" + GenerateDate() + ".bin", true);
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin"))
                                File.Copy(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin", Program.ServerConfig.DbLocation + "\\BackUp\\PlayersItems\\" + client.Player.UID + "date" + GenerateDate() + ".bin");


                            if (File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\Users\\" + client.Player.UID + ".ini");
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\PlayersSpells\\" + client.Player.UID + ".bin");
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\PlayersProfs\\" + client.Player.UID + ".bin");
                            if (File.Exists(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\PlayersItems\\" + client.Player.UID + ".bin");
                            try
                            {
                                if (File.Exists(Program.ServerConfig.DbLocation + "\\Quests\\" + client.Player.UID + ".bin"))
                                    File.Delete(Program.ServerConfig.DbLocation + "\\Quests\\" + client.Player.UID + ".bin");
                            }
                            catch
                            {

                            }

                            Role.Instance.House house;
                            if (client.MyHouse != null && Role.Instance.House.HousePoll.ContainsKey(client.Player.UID))
                                Role.Instance.House.HousePoll.TryRemove(client.Player.UID, out house);

                            if (File.Exists(Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin"))
                                File.Delete(Program.ServerConfig.DbLocation + "\\Houses\\" + client.Player.UID + ".bin");

                            //Role.Instance.Chi chi;
                            //if (Role.Instance.Chi.ChiPool.ContainsKey(client.Player.UID))
                            //{
                            //    Role.Instance.Chi.ChiPool.TryRemove(client.Player.UID, out chi);
                            //    WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\BackUp\\ChiInfo.txt");
                            //    write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Dragon", chi.Dragon.ToString());
                            //    write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Phoenix", chi.Phoenix.ToString());
                            //    write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Turtle", chi.Turtle.ToString());
                            //    write.WriteString(client.Player.UID.ToString() + "date" + GenerateDate() + "", "Tiger", chi.Tiger.ToString());

                            //}
                            Nobility nobility;
                            if (NobilityTable.NobilityRanking.ClientPoll.TryGetValue(client.Player.UID, out nobility))
                            {
                                NobilityTable.NobilityRanking.ClientPoll.Remove(nobility.UID);
                                if (File.Exists(Program.ServerConfig.DbLocation + @"\Nobility\" + client.Player.UID + ".ini"))
                                    File.Delete(Program.ServerConfig.DbLocation + @"\Nobility\" + client.Player.UID + ".ini");
                                NobilityTable.NobilityRanking.CreateRank();
                            }
                            Role.Instance.Flowers flow;
                            if (Role.Instance.Flowers.ClientPoll.ContainsKey(client.Player.UID))
                            {
                                Role.Instance.Flowers.ClientPoll.TryRemove(client.Player.UID, out flow);
                            }
                            Role.Instance.Associate.MyAsociats Associate;
                            if (Role.Instance.Associate.Associates.TryGetValue(client.Player.UID, out Associate))
                            {
                                Role.Instance.Associate.Associates.TryRemove(client.Player.UID, out Associate);
                            }
                            Client.GameClient user;
                            Database.Server.GamePoll.TryRemove(client.Player.UID, out user);

                            if (Server.NameUsed.Contains(user.Player.Name.GetHashCode()))
                            {
                                lock (Server.NameUsed)
                                    Server.NameUsed.Remove(user.Player.Name.GetHashCode());
                            }
                            return;

                        }
                        if ((client.ClientFlag & Client.ServerFlag.RemoveSpouse) == Client.ServerFlag.RemoveSpouse)
                        {
                            DestroySpouse(client);
                            client.ClientFlag &= ~Client.ServerFlag.RemoveSpouse;
                            return;
                        }
                        if ((client.ClientFlag & Client.ServerFlag.UpdateSpouse) == Client.ServerFlag.UpdateSpouse)
                        {
                            UpdateSpouse(client);
                            client.ClientFlag &= ~Client.ServerFlag.UpdateSpouse;
                            return;
                        }
                        if ((client.ClientFlag & Client.ServerFlag.SetLocation) != Client.ServerFlag.SetLocation && (client.ClientFlag & Client.ServerFlag.OnLoggion) == Client.ServerFlag.OnLoggion)
                        {
                            Game.MsgServer.MsgLoginClient.LoginHandler(client, client.OnLogin);
                        }
                        else if ((client.ClientFlag & Client.ServerFlag.QueuesSave) == Client.ServerFlag.QueuesSave)
                        {
                            if (client.Player.OnTransform)
                            {
                                client.Player.HitPoints = Math.Min(client.Player.HitPoints, (int)client.Status.MaxHitpoints);

                            }
                            SaveClient(client);

                        }
                    }
                }
                catch (Exception e) { Console.SaveException(e); }
            }
        }

    }
}
