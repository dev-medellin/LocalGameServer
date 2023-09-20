using COServer.Client;
using COServer.ServerSockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COServer.Database
{
    public class Lottery
    {
        public static List<uint> RateH = new List<uint>()
        {
            723859,723855,723712,720128,723727,723713,723714,723856,181425,181325,181315,181305,181415,181405,181715,181705,752099,1200000,723724,721258,723700,723834,722700,723017,182355,1088001,723711,753999, 752999,751999
            ,200301 ,200300
        };
        public static List<uint> RateH2 = new List<uint>()
        {
            1200001,720611,723862,181565,181925,723715,720609,723860,721259,181905,720610,
            723861,181625,723716,181525,723584,181805,725018,723725,725019,753099,752099,751099,
            200211, 723094
        };
        public static List<uint> RateH3 = new List<uint>()
        {
           181395,725016,723341,725020,723717,725021,1088000,2100045,723725,1200005,725022,723718,725023,181335,181435,181535,181635,181735,181835,181935,753009,752009,751009,
           182445,191405,183325 // 1 soc all - 2 soc weaponsm
           , 200300, 723094, 200003,200004,200005,200008,183305,183325,183375,184325,181955,191305,754999,751999,753999,754999,754099,753099,751099,751009
        };
        public static List<uint> RateH4 = new List<uint>()
        {
           1200002,723865,181605,723719,723720,723863,723864,181985,183315,723695,753003,752003,751003,
           181975,1060039,182905,
           181945,181725,182325,182365,181525,181515,181505,723584,181825,182335,182315,183375,182375,723718 // 2 soc items.
           , 200003,200004,200005,200008,183305,183325,183375,184325,181955,191305,754999,751999,753999,754999,754099,753099,751099,751009
        };
        public static List<uint> RateH5 = new List<uint>()
        {
            720049,725024,181355,723701,723721,723722,723694,1100009,723723,2100025,723342,752999,191905,753001,752001,751001 // +8 items.
            , 200003,200004,200005,200008,183305,183325,183375,184325,181955,191305,754999,751999,753999,754999,754099,753099,751099,751009
        };
        public static List<uint> Super1SocItems = new List<uint>()
        {
           121129,133049,152129,131059,151119,130059,123059,141059,112059,136059,118059,117069,201009,150119,113039,160099,601199,111059,120129,134059,143059,142059,114069,202009,135059
        };

        public static List<uint> Super2SocItems = new List<uint>()
        {
            142049,610199,121099,151099,131049,421199,114049,561199,160119,430199,130049,601099,450199,152089,120099,530199,143029,201009,150099,113029,118049,560199,440199,601199,420199,135029,141049,540199,112029,134049,410199,490199,500189,123029,460199,580199,480199,136029,202009,111049,117049,900089,481199,510199,133029
        };

        public static List<uint> SuperNoSocItems = new List<uint>()
        {
            113069,131089,123069,112069,152189,134099,150199,120189,136089,121189,143069,201009,117089,151179,160199,141069,118089,135089,133079,114099,202009,142069,111089,130089

        };
        public static List<uint> ElitePlus8Items = new List<uint>()
        {
            150078,121088,113018,120088,410078,123038,490088,160078,142018,421078,540088,130038,580088,143038,201008,500078,151078,133038,481088,112038,117038,560088,135048,420088,430088,610088,136048,510088,141038,450088,440088,111038,900008,134038,202008,131038,118038,114038,530088,601088,561088,480088,152108,460088
        };

        public static void GetRandomPrize(GameClient Client, Packet stream)
        {
            if (Role.Core.Rate(0.0003))
            {
                // Lucky +8 elite item.
                uint Id = ElitePlus8Items[Role.Core.Random.Next(0, ElitePlus8Items.Count)];
                Client.Inventory.Add(stream, Id, 1, 8);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + Client.Player.Name + " won a +8" + Server.ItemsBase[Id].Name + " in Lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.Plus8++;
            }
            else if (Role.Core.Rate(0.0009))
            {
                // Lucky Super2Soc
                uint Id = Super2SocItems[Role.Core.Random.Next(0, Super2SocItems.Count)];
                Client.Inventory.Add(stream, Id, 1, 0, 0, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + Client.Player.Name + " won a Super-2soc." + Server.ItemsBase[Id].Name + " in lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.Super2Soc++;

            }
            else if (Role.Core.Rate(0.001))
            {
                // Lucky Super1Soc
                uint Id = Super1SocItems[Role.Core.Random.Next(0, Super1SocItems.Count)];
                Client.Inventory.Add(stream, Id, 1, 0, 0, 0, Role.Flags.Gem.EmptySocket);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + Client.Player.Name + " won a Super-1soc." + Server.ItemsBase[Id].Name + " in lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.Super1Soc++;

            }
            else if (Role.Core.Rate(0.005))
            {
                // Lucky Super
                uint Id = SuperNoSocItems[Role.Core.Random.Next(0, SuperNoSocItems.Count)];
                Client.Inventory.Add(stream, Id, 1);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + Client.Player.Name + " won a Super" + Server.ItemsBase[Id].Name + " in lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                Program.SuperNoSoc++;

            }
            else if (Role.Core.Rate(0.004))
            {
                if (Client.ProjectManager)
                    Client.SendSysMesage("Group5", Game.MsgServer.MsgMessage.ChatMode.TopLeft);
                uint Id = RateH5[Role.Core.Random.Next(0, RateH5.Count)];
                Client.Inventory.Add(stream, Id, 1);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + Client.Player.Name + " won " + Server.ItemsBase[Id].Name + " in lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
            }
            else if (Role.Core.Rate(0.008))
            {
                if (Client.ProjectManager)
                    Client.SendSysMesage("Group4", Game.MsgServer.MsgMessage.ChatMode.TopLeft);
                uint Id = RateH4[Role.Core.Random.Next(0, RateH4.Count)];
                Client.Inventory.Add(stream, Id, 1);
                Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + Client.Player.Name + " won " + Server.ItemsBase[Id].Name + " in lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
            }
            else if (Role.Core.Rate(0.009))
            {
                if (Client.ProjectManager)
                    Client.SendSysMesage("Group3", Game.MsgServer.MsgMessage.ChatMode.TopLeft);
                uint Id = RateH3[Role.Core.Random.Next(0, RateH3.Count)];
                Client.Inventory.Add(stream, Id, 1);
            }
            else if (Role.Core.Rate(10))
            {
                if (Client.ProjectManager)
                    Client.SendSysMesage("Group2", Game.MsgServer.MsgMessage.ChatMode.TopLeft);
                uint Id = RateH2[Role.Core.Random.Next(0, RateH2.Count)];
                Client.Inventory.Add(stream, Id, 1);
            }
            else
            {
                if (Client.ProjectManager)
                    Client.SendSysMesage("Group1", Game.MsgServer.MsgMessage.ChatMode.TopLeft);
                uint Id = RateH[Role.Core.Random.Next(0, RateH.Count)];
                Client.Inventory.Add(stream, Id, 1);
            }
        }


        //static string ItemsSuper = "";
        //static string ItemsElite = "";
        //static string ItemsSuper1Soc = "";
        //static string ItemsSuper2Soc = "";
        //public static void TestLoad()
        //{
        //    using (var reader = new StreamReader("Lott.txt"))
        //    {
        //        string[] lines = reader.ReadToEnd().Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        //        foreach (var item in lines)
        //        {
        //            string name = "";
        //            if (item.StartsWith("Elite+8"))
        //            {
        //                name = item.Replace("Elite+8", "");
        //                var Id = ItemType.GetItemIdByName(name).Split('-');
        //                if (Id[0] != "0")
        //                {
        //                    foreach (var ip in Id)
        //                        if (ip.EndsWith("8"))
        //                        {
        //                            ItemsElite += ip + ",";
        //                            break;
        //                        }
        //                }
        //            }
        //            else if (item.StartsWith("Super2-Soc."))
        //            {
        //                name = item.Replace("Super2-Soc.", "");
        //                var Id = ItemType.GetItemIdByName(name).Split('-');
        //                if (Id[0] != "0")
        //                {
        //                    foreach (var ip in Id)
        //                        if (ip.EndsWith("9"))
        //                        {
        //                            ItemsSuper2Soc += ip + ",";
        //                            break;
        //                        }
        //                }
        //            }
        //            else if (item.StartsWith("Super1-Soc."))
        //            {
        //                name = item.Replace("Super1-Soc.", "");
        //                var Id = ItemType.GetItemIdByName(name).Split('-');
        //                if (Id[0] != "0")
        //                {
        //                    foreach (var ip in Id)
        //                        if (ip.EndsWith("9"))
        //                        {
        //                            ItemsSuper1Soc += ip + ",";
        //                            break;
        //                        }
        //                }
        //            }
        //            else if (item.StartsWith("Super"))
        //            {
        //                name = item.Replace("Super", "");
        //                var Id = ItemType.GetItemIdByName(name).Split('-');
        //                if (Id[0] != "0")
        //                {
        //                    foreach (var ip in Id)
        //                        if (ip.EndsWith("9"))
        //                        {
        //                            ItemsSuper += ip + ",";
        //                            break;
        //                        }
        //                }
        //            }
        //        }
        //        //using (var writer = new StreamWriter("lott2.txt"))
        //        //{
        //        //    foreach (var item in Items)
        //        //        writer.WriteLine(item);
        //        //    writer.Close();
        //        //}
        //    }
        //}
        public static void TestLoad()
        {
            List<string> e = new List<string>();
            using (var reader = new StreamReader("garments.txt"))
            {
                string[] lines = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in lines)
                {
                    var data = item.Split(' ');
                    uint uid = uint.Parse(ItemType.GetItemIdByName(data[0]));
                    e.Add($"{uid} {data[1]} item other new");

                }
                using (var writer = new StreamWriter("lott2.txt"))
                {
                    foreach (var item in e)
                        writer.WriteLine(item);
                    writer.Close();
                }
            }
        }

    }
}
