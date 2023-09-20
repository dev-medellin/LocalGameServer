using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COServer.Database
{
    public class LotteryEffect
    {
        public static List<uint> LotteryItems = new List<uint>();
        public static List<uint> LotteryItemsafter30 = new List<uint>();
        public static List<uint> LotteryItemsafter50 = new List<uint>();
        public void Load()
        {
            string[] text = File.ReadAllLines(Program.ServerConfig.DbLocation + "LotteryEffect.ini");

            for (int x = 0; x < text.Length; x++)
            {
                string line = text[x].Trim().Replace("_", " ");
                if (line == "")
                    continue;
                string[] split = line.Split(new string[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (split[0].Contains("Gift"))
                    if (!LotteryItems.Contains(uint.Parse(split[1])))
                        LotteryItems.Add(uint.Parse(split[1]));

            }
            LotteryItemsafter50 = LotteryItemsafter30 = LotteryItems;
            LotteryItemsafter30.Remove(ItemType.Meteor);
            LotteryItemsafter50.Remove(ItemType.DragonBall);
            Console.WriteLine("LotteryEffect " + LotteryItems.Count + " Items Loaded!.");
        }
        public byte LotteryEntry(byte vipLevel)
        {
            byte chance = 0;
            switch (vipLevel)
            {
                default:
                    chance = 10;
                    break;
                case 6:
                    chance = 50;
                    break;
            }
            return chance;
        }
        public static void Handler(Client.GameClient user, ServerSockets.Packet stream)
        {
            int rand = Program.GetRandom.Next(Database.LotteryEffect.LotteryItems.Count);
            var item = Database.LotteryEffect.LotteryItems[rand];

            if (item % 10 == (byte)Role.Flags.ItemQuality.Elite)
            {
                int Rate = user.Player.MyRandom.Next(1, 1000);
                if (Rate < 2)
                {
                    user.Inventory.Add(stream, item, 1, 8);
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + user.Player.Name + " Won " + Server.ItemsBase[item].Name + " +8 in Lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                    user.SendSysMesage("You won a " + Server.ItemsBase[item].Name + "  +8 from the lottery!", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                }
                else if (Rate < 80)
                {
                    ushort pos = Database.ItemType.ItemPosition(item);
                    if (pos == (ushort)Role.Flags.ConquerItem.Bottle || pos == (ushort)Role.Flags.ConquerItem.Garment || pos == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory || pos == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory || pos == (ushort)Role.Flags.ConquerItem.SteedMount || pos == (ushort)Role.Flags.ConquerItem.RidingCrop)
                        return;
                    user.Inventory.Add(stream, item, 1, 0, 0, 0, Role.Flags.Gem.EmptySocket);
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + user.Player.Name + " Won " + Server.ItemsBase[item].Name + " 1 Socket in Lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                    user.SendSysMesage("You won a " + Server.ItemsBase[item].Name + "  one Socket from the lottery!", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                }
                else if (Rate < 50)
                {
                    ushort pos = Database.ItemType.ItemPosition(item);
                    if (pos == (ushort)Role.Flags.ConquerItem.Bottle || pos == (ushort)Role.Flags.ConquerItem.Garment || pos == (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory || pos == (ushort)Role.Flags.ConquerItem.RightWeaponAccessory || pos == (ushort)Role.Flags.ConquerItem.SteedMount || pos == (ushort)Role.Flags.ConquerItem.RidingCrop)
                        return;
                    user.Inventory.Add(stream, item, 1, 0, 0, 0, Role.Flags.Gem.EmptySocket, Role.Flags.Gem.EmptySocket);
                    Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("Congratulations! " + user.Player.Name + " Won " + Server.ItemsBase[item].Name + " 2 Socket in Lottery.", Game.MsgServer.MsgMessage.MsgColor.white, Game.MsgServer.MsgMessage.ChatMode.System).GetArray(stream));
                    user.SendSysMesage("You won a " + Server.ItemsBase[item].Name + "  two Socket from the lottery!", Game.MsgServer.MsgMessage.ChatMode.System, Game.MsgServer.MsgMessage.MsgColor.red);
                }
                else
                {
                    user.Inventory.Add(stream, item);
                }
            }
            else
            {
                user.Inventory.Add(stream, item);
            }

        }
    }
}
