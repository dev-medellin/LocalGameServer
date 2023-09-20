using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgFloorItem
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetItemPacket(this ServerSockets.Packet stream, out uint uid)
        {
         //   uint stamp = stream.ReadUInt32();
            uid = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet ItemPacketCreate(this ServerSockets.Packet stream, MsgItemPacket Item)
        {
            stream.InitWriter();
            stream.Write(Item.m_UID);//8
            stream.Write(Item.m_ID);//12
            stream.Write(Item.m_X);//16
            stream.Write(Item.m_Y);//18
            stream.Write((ushort)Item.m_Color);//Item.m_Color);
            stream.Write((byte)Item.DropType);//22
            stream.Finalize(GamePackets.FloorMap);
            return stream;
        }
    }

    public unsafe class MsgItemPacket
    {
        public enum EffectMonsters : uint
        {
            None = 0,
            EarthquakeLeftRight = 1,
            EarthquakeUpDown = 2,
            Night = 4,
            EarthquakeAndNight = 5
        }

        public const uint
            DBShowerEffect = 17;


        public uint m_UID;
        public uint m_ID;
        public ushort m_X;
        public ushort m_Y;
        public ushort MaxLife;
        public MsgDropID DropType;
        public uint Life;
        public byte m_Color;
        public byte m_Color2;
        public uint ItemOwnerUID;
        public byte DontShow;
        public uint GuildID;
        public byte FlowerType;
        public ulong Timer;
        public string Name;
        public uint UnKnow;
        public byte Plus;



        public ushort OwnerX;
        public ushort OwnerY;

        public static MsgItemPacket Create()
        {
            MsgItemPacket item = new MsgItemPacket();
            return item;
        }

        [PacketAttribute(GamePackets.FloorMap)]
        public unsafe static void FloorMap(Client.GameClient client, ServerSockets.Packet packet)
        {
            if (client.InTrade)
                return;
            if (!client.Player.OnMyOwnServer)
                return;

            uint m_UID;

            packet.GetItemPacket(out m_UID);

            MsgFloorItem.MsgItem MapItem;
            if (client.Map.View.TryGetObject<MsgFloorItem.MsgItem>(m_UID, Role.MapObjectType.Item, client.Player.X, client.Player.Y, out MapItem))
            {
                if (MapItem.ToMySelf)
                {
                    if (!MapItem.ExpireMySelf)
                    {
                        if (MapItem.ItemOwner != client.Player.UID)
                        {
                            if (client.Team != null)
                            {
                                if (MapItem.Typ != MsgItem.ItemType.Money &&
                                    (!client.Team.IsTeamMember(MapItem.ItemOwner) || !client.Team.PickupItems))
                                {
                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                                    return;
                                }
                                else if (MapItem.Typ == MsgItem.ItemType.Money)
                                {
                                    if (!client.Team.PickupMoney)
                                    {
                                        client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                                        return;
                                    }
                                }
                            }
                            else if (client.Team == null)
                            {
                                if (MapItem.Typ == MsgItem.ItemType.Money)
                                {
                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                                    return;
                                }
                                else
                                {
                                    client.SendSysMesage("You have to wait a little bit before you can pick up any items dropped from monsters killed by other players.");
                                    return;
                                }
                            }
                        }
                    }
                }
                if (Role.Core.GetDistance(client.Player.X,client.Player.Y,MapItem.MsgFloor.m_X,MapItem.MsgFloor.m_Y) <= 5)
                {
                    switch (MapItem.Typ)
                    {

                        case MsgItem.ItemType.Money:
                            {

                                client.Player.Money += MapItem.Gold;
                                client.Player.SendUpdate(packet, client.Player.Money, MsgServer.MsgUpdate.DataType.Money);
                                MapItem.SendAll(packet, MsgDropID.Remove);
                                client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                client.Map.View.LeaveMap<Role.IMapObj>(MapItem);
                                client.SendSysMesage("You have picked up a " + MapItem.Gold + " silvers.");
                                break;
                            }
                        case MsgItem.ItemType.Item:
                            {
                                Database.ItemType.DBItem DBItem;
                                if (client.Inventory.HaveSpace(1))
                                {
                                    if (Database.Server.ItemsBase.TryGetValue(MapItem.MsgFloor.m_ID, out DBItem))
                                    {

                                       
                                        client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                        if (MapItem.ItemBase.StackSize > 1)
                                        {
                                            client.Inventory.Update(MapItem.ItemBase, Role.Instance.AddMode.ADD, packet);
                                        }
                                        else
                                            client.Inventory.Add(MapItem.ItemBase, DBItem, packet);
                                        client.Map.View.LeaveMap<Role.IMapObj>(MapItem);
                                        MapItem.SendAll(packet,MsgDropID.Remove);
                                        client.SendSysMesage("You have picked up a " + DBItem.Name + ".");
                                        if (DBItem.ID == 711352)
                                        {
                                            client.Player.QuestGUI.IncreaseQuestObjectives(packet, 1311, 1);
                                        }
                                    }
                                }
                                break;
                            }
                        case MsgItem.ItemType.Cps:
                            {
                                Database.ItemType.DBItem DBItem;
                                if (Database.Server.ItemsBase.TryGetValue(MapItem.MsgFloor.m_ID, out DBItem))
                                {
                                    if (MapItem.ItemBase.ITEM_ID == 3001133)
                                    {
                                        client.Player.ConquerPoints += 5;
                                        client.SendSysMesage("You have picked up a 5 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720657)
                                    {
                                        client.Player.ConquerPoints += 5;
                                        client.SendSysMesage("You have picked up a 5 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720656)
                                    {
                                        client.Player.ConquerPoints += 10;
                                        client.SendSysMesage("You have picked up a 10 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 3001134)
                                    {
                                        client.Player.ConquerPoints += 10;
                                        client.SendSysMesage("You have picked up a 10 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 3001135)
                                    {
                                        client.Player.ConquerPoints += 20;
                                        client.SendSysMesage("You have picked up a 20 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720655)
                                    {
                                        client.Player.ConquerPoints += 20;
                                        client.SendSysMesage("You have picked up a 20 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720658)
                                    {
                                        client.Player.ConquerPoints += 25;
                                        client.SendSysMesage("You have picked up a 25 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720663)
                                    {
                                        client.Player.ConquerPoints += 50;
                                        client.SendSysMesage("You have picked up a 50 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720659)
                                    {
                                        client.Player.ConquerPoints += 50;
                                        client.SendSysMesage("You have picked up a 50 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720660)
                                    {
                                        client.Player.ConquerPoints += 100;
                                        client.SendSysMesage("You have picked up a 100 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720664)
                                    {
                                        client.Player.ConquerPoints += 100;
                                        client.SendSysMesage("You have picked up a 100 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720665)
                                    {
                                        client.Player.ConquerPoints += 200;
                                        client.SendSysMesage("You have picked up a 200 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720675)
                                    {
                                        client.Player.ConquerPoints += 200;
                                        client.SendSysMesage("You have picked up a 250 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720653)
                                    {
                                        client.Player.ConquerPoints += 270;
                                        client.SendSysMesage("You have picked up a 270 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720681)
                                    {
                                        client.Player.ConquerPoints += 500;
                                        client.SendSysMesage("You have picked up a 500 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720676)
                                    {
                                        client.Player.ConquerPoints += 500;
                                        client.SendSysMesage("You have picked up a 500 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 3001136)
                                    {
                                        client.Player.ConquerPoints += 500;
                                        client.SendSysMesage("You have picked up a 500 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720677)
                                    {
                                        client.Player.ConquerPoints += 1000;
                                        client.SendSysMesage("You have picked up a 1000 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720682)
                                    {
                                        client.Player.ConquerPoints += 1000;
                                        client.SendSysMesage("You have picked up a 1000 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720687)
                                    {
                                        client.Player.ConquerPoints += 1000;
                                        client.SendSysMesage("You have picked up a 1000 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720661)
                                    {
                                        client.Player.ConquerPoints += 1350;
                                        client.SendSysMesage("You have picked up a 1350 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720654)
                                    {
                                        client.Player.ConquerPoints += 1380;
                                        client.SendSysMesage("You have picked up a 1380 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720683)
                                    {
                                        client.Player.ConquerPoints += 2000;
                                        client.SendSysMesage("You have picked up a 2000 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720688)
                                    {
                                        client.Player.ConquerPoints += 2000;
                                        client.SendSysMesage("You have picked up a 2000 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720693)
                                    {
                                        client.Player.ConquerPoints += 2500;
                                        client.SendSysMesage("You have picked up a 2500 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720666)
                                    {
                                        client.Player.ConquerPoints += 2700;
                                        client.SendSysMesage("You have picked up a 2700 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720689)
                                    {
                                        client.Player.ConquerPoints += 4000;
                                        client.SendSysMesage("You have picked up a 4000 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720694)
                                    {
                                        client.Player.ConquerPoints += 5000;
                                        client.SendSysMesage("You have picked up a 5000 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720662)
                                    {
                                        uint val = (uint)Role.Core.Random.Next(1, 15);
                                        client.Player.ConquerPoints += val;
                                        client.SendSysMesage("You have picked up a " + val + " ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720695)
                                    {
                                        client.Player.ConquerPoints += 10000;
                                        client.SendSysMesage("You have picked up a 10000 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720678)
                                    {
                                        client.Player.ConquerPoints += 13500;
                                        client.SendSysMesage("You have picked up a 13500 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720667)
                                    {
                                        client.Player.ConquerPoints += 13800;
                                        client.SendSysMesage("You have picked up a 13800 ConquerPoints.");
                                    }
                                    else if (MapItem.ItemBase.ITEM_ID == 720684)
                                    {
                                        client.Player.ConquerPoints += 27000;
                                        client.SendSysMesage("You have picked up a 27000 ConquerPoints.");
                                    }
                                    else
                                        client.Inventory.Add(MapItem.ItemBase, DBItem, packet);
                                    MapItem.SendAll(packet, MsgDropID.Remove);
                                    client.Map.cells[MapItem.MsgFloor.m_X, MapItem.MsgFloor.m_Y] &= ~Role.MapFlagType.Item;
                                    client.Map.View.LeaveMap<Role.IMapObj>(MapItem);
                                    break;
                                }
                                break;
                            }
                    }
                }
            }
        }
    }
}
