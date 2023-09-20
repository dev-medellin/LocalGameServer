using COServer.Client;
using COServer.Role.Instance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace COServer.Database
{
    public class NobilityTable
    {
        //public static void Load()
        //{
        //    WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
        //    foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
        //    {
        //        ini.FileName = fname;

        //        ushort Body = ini.ReadUInt16("Character", "Body", 1002);
        //        ushort Face = ini.ReadUInt16("Character", "Face", 0);
        //        uint UID = ini.ReadUInt32("Character", "UID", 0);
        //        string Name = ini.ReadString("Character", "Name", "None");
        //        byte Gender = 0;
        //        if ((byte)(Body % 10) >= 3)
        //            Gender = 0;
        //        else
        //            Gender = 1;
        //        uint Mesh = (uint)(Face * 10000 + Body);
        //        ulong donation = ini.ReadUInt64("Character", "DonationNobility", 0);
        //        Role.Instance.Nobility nobility = new Role.Instance.Nobility(UID, Name, donation, Mesh, Gender);
        //        Program.NobilityRanking.UpdateRank(nobility);
        //    }
        //}
        public static Nobility.NobilityRanking NobilityRanking = new Nobility.NobilityRanking();
        public static void Load()
        {
            foreach (string fname in Directory.GetFiles(Program.ServerConfig.DbLocation + @"\Nobility\"))
            {
                var ini = new IniFile2(fname);
                uint UID = ini.ReadUInt32("Info", "UID");
                string Name = ini.ReadString("Info", "Name");
                ulong Donation = ini.ReadUInt64("Info", "Donation");
                uint Mesh = ini.ReadUInt32("Info", "Mesh");
                Nobility nobility = new Nobility(UID, Name, Donation, Mesh, 0);
                NobilityRanking.UpdateRank(nobility);
            }
            GC.Collect();
        }
        public static void LoadClient(GameClient client)
        {
            Nobility nobility;
            if (NobilityRanking.TryGetValue(client.Player.UID, out nobility))
            {
                client.Player.Nobility = nobility;
                client.Player.Nobility.Gender = client.Player.GetGender;
                client.Player.NobilityRank = client.Player.Nobility.Rank;
            }
            else
            {
                client.Player.Nobility = new Nobility(client);
                client.Player.NobilityRank = client.Player.Nobility.Rank;
            }
        }
        public static void Save()
        {
            foreach (var nob in NobilityRanking.ClientPoll.Values.ToArray())
            {
                IniFile2 write = new IniFile2(Program.ServerConfig.DbLocation + @"\Nobility\" + nob.UID + ".ini");
                write.Write("Info", "UID", nob.UID);
                write.Write("Info", "Name", nob.Name);
                write.Write("Info", "Donation", nob.Donation);
                write.Write("Info", "Mesh", nob.Mesh);
            }
        }
        public static void Reset()
        {
            foreach (var nob in NobilityRanking.ClientPoll.Values.ToArray())
                nob.Donation = 0;
            NobilityRanking.UpdateRank(null);
        }
    }
}
