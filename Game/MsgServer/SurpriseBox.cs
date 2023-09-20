using COServer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer
{
    public class SurpriseBox
    {
        static List<uint> High = new List<uint>()
        {
            720611,723863,723860,723864,723865,720028,720027,722057,711083,723094,1200000,723727,
            181955, 182635, 191305,200005,754999,753999,753003,753001,751999,751001,751003,730006
        };
        static List<uint> Mid = new List<uint>()
        {
            720049,723342,723342,753999,753099,754999,780001,720027,722057,711083,723094,1200000,1200001,1200002,1100009,1100006,200312,200311,200310,
            181955, 182635, 191305,200005,754999,753999,753003,753001,751999,751001,751003,730006
        };
        static uint GenerateSoulsItems()
        {
            var arr1 = new[] { 2, 3, 4, 5 };
            var rndMember = arr1[Role.Core.Random.Next(arr1.Length)];
            var level = (ushort)rndMember;
            if (Database.ItemType.PurificationItems.ContainsKey(level))
            {
                var array = Database.ItemType.PurificationItems[level].Values.ToArray();
                int position = Program.GetRandom.Next(0, array.Length);
                return array[position].ID;
            }


            return 0;
        }

        static uint GenerateSoulsItemsPhase6()
        {
            var arr1 = new[] { 2, 3, 4, 5 };
            var rndMember = arr1[Role.Core.Random.Next(arr1.Length)];
            var level = (ushort)rndMember;
            if (Database.ItemType.PurificationItems.ContainsKey(level))
            {
                var array = Database.ItemType.PurificationItems[level].Values.ToArray();
                int position = Program.GetRandom.Next(0, array.Length);
                return array[position].ID;
            }


            return 0;
        }
        public static void GetReward(GameClient client, ServerSockets.Packet stream)
        {
            if (Role.MyMath.Success(100))//0.5
            {
                var reward = GenerateSoulsItems();
                client.Inventory.Add(stream, reward);
                client.SendSysMesage("You got a nice reward check your inventory");
            }
            else
            {
                var reward = GenerateSoulsItems();
                client.Inventory.Add(stream, reward);
                client.SendSysMesage("You got a nice reward check your inventory");
            }
            // else if (Role.MyMath.Success(10))
            // {
            //     var reward = Mid[Role.Core.Random.Next(0, Mid.Count)];
            //     client.Inventory.Add(stream, reward, 1);
            //     client.SendSysMesage("You got a nice reward check your inventory");
            // }
        }

        public static void GetSoulPackReward(GameClient client, ServerSockets.Packet stream)
        {
            if (Role.MyMath.Success(10))//0.5
            {
                var reward = GenerateSoulsItemsPhase6();
                client.Inventory.Add(stream, reward);
                client.SendSysMesage("You got a nice reward check your inventory");
            }
            else
            {
                var reward = GenerateSoulsItems();
                client.Inventory.Add(stream, reward);
                client.SendSysMesage("You got a nice reward check your inventory");
            }
            // else if (Role.MyMath.Success(10))
            // {
            //     var reward = Mid[Role.Core.Random.Next(0, Mid.Count)];
            //     client.Inventory.Add(stream, reward, 1);
            //     client.SendSysMesage("You got a nice reward check your inventory");
            // }
        }
    }
}
