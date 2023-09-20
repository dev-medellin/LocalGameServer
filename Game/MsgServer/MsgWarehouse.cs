using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace COServer.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetWarehouse(this ServerSockets.Packet stream, out uint NpcID, out MsgWarehouse.DepositActionID Action, out uint ItemUID)
        {
            NpcID = stream.ReadUInt32();
            Action = (MsgWarehouse.DepositActionID)stream.ReadUInt32();
            uint file_size = stream.ReadUInt32();
            ItemUID = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet WarehouseCreate(this ServerSockets.Packet stream, uint NpcID, MsgWarehouse.DepositActionID Action, uint ItemUID, int File_Size, int count)
        {
            stream.InitWriter();
            stream.Write(NpcID);
            stream.Write((uint)Action);
            stream.Write(File_Size);
            stream.Write(ItemUID);
            stream.Write(count);
            return stream;
        }
        public static unsafe ServerSockets.Packet AddItemWarehouse(this ServerSockets.Packet stream, Game.MsgServer.MsgGameItem item)
        {
            stream.Write(item.UID);
            stream.Write(item.ITEM_ID);
            stream.ZeroFill(1); //unknown
            stream.Write((byte)(item.SocketOne));
            stream.Write((byte)(item.SocketTwo));
            stream.ZeroFill(1); //unknown
            stream.Write((ushort)(0));
            stream.ZeroFill(3); //unknown
            stream.Write((byte)(item.Plus));
            stream.Write((byte)(item.Bless));
            stream.Write((byte)(item.Bound));
            stream.Write((ushort)(item.Enchant));
            stream.Write((ushort)item.Effect); // locked
            stream.Write(item.Locked); // locked
            stream.Write((ushort)item.Suspicious);
            stream.Write((byte)item.Color);
            stream.Write(item.SocketProgress);
            stream.Write(item.PlusProgress);
            stream.Write(item.Inscribed);
            stream.Write((uint)(item.TimeLeftInMinutes * 60)); // time_remaining
            // stream.ZeroFill(1 * sizeof(int));
            stream.Write((ushort)(item.StackSize));
            stream.Write((ushort)(item.Durability));
            stream.Write((ushort)(item.MaximDurability));
            stream.Write((ushort)item.Purification.PurificationItemID);
            return stream;
        }
        public static unsafe ServerSockets.Packet FinalizeWarehouse(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.Warehause);
            return stream;
        }
    }
    public class MsgWarehouse
    {
        public enum DepositActionID : ushort
        {
            Show = 2560,
            DepositItem = 2561,
            WithdrawItem = 2562,

            Show_WH_House = 5120,
            DepositItem_WH_House = 5121,
            WithdrawItem_WH_House = 5122,

            ShashShow = 7680,
            ShashDepositItem = 7681,
            ShashWithdrawItem = 7682,

            ShowInventorySash = 10240,
            InventorySashDepositItem = 10241,
            InventorySashWithdrawItem = 10242,
        }



        [PacketAttribute(GamePackets.Warehause)]
        public unsafe static void HandlerWarehause(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!client.Player.IsCheckedPass)
                return;//cheater
            uint NpcID;
            MsgWarehouse.DepositActionID Action;
            uint ItemUID;
            if (client.PokerPlayer != null)
                return;
            stream.GetWarehouse(out NpcID, out Action, out ItemUID);
            switch (Action)
            {
                case DepositActionID.ShashDepositItem:
                    {
                        /// if (client.Player.UID == NpcID)
                        {
                            MsgGameItem item;
                            if (client.Inventory.TryGetItem(ItemUID, out item))
                            {
                                if (client.Warehouse.AddItem(item, NpcID))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);


                                    stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                                    stream.AddItemWarehouse(item);

                                    client.Send(stream.FinalizeWarehouse());


                                    item.SendItemExtra(client, stream);
                                    item.SendItemLocked(client, stream);
                                }
                            }
                        }
                        break;
                    }
                case DepositActionID.DepositItem_WH_House:
                case DepositActionID.DepositItem:
                    {

                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID || client.Player.UID == NpcID)
                        {
                            MsgGameItem item;
                            if (client.Inventory.TryGetItem(ItemUID, out item))
                            {
                                if (client.Warehouse.AddItem(item, NpcID))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);


                                    stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                                    stream.AddItemWarehouse(item);

                                    client.Send(stream.FinalizeWarehouse());


                                    item.SendItemExtra(client, stream);
                                    item.SendItemLocked(client, stream);
                                }
                            }
                        }
                        break;
                    }
                case DepositActionID.ShashShow:
                case DepositActionID.ShowInventorySash:
                    {
                        //  if (client.Player.UID == NpcID)
                        {
                            client.Warehouse.Show(NpcID, Action, stream);
                        }
                        break;
                    }
                case DepositActionID.Show_WH_House:
                case DepositActionID.Show:
                    {
                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID)
                        {
                            client.Warehouse.Show(NpcID, Action, stream);
                        }
                        break;
                    }
                case DepositActionID.ShashWithdrawItem:
                case DepositActionID.InventorySashWithdrawItem:
                    {
                        //   if (client.Player.UID == NpcID)
                        {
                            if (client.Warehouse.RemoveItem(ItemUID, NpcID, stream))
                            {
                                stream.WarehouseCreate(NpcID, Action, ItemUID, 0, 0);

                                client.Send(stream.FinalizeWarehouse());
                            }
                        }
                        break;
                    }
                case DepositActionID.WithdrawItem_WH_House:
                case DepositActionID.WithdrawItem:
                    {
                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID)
                        {
                            if (client.Warehouse.RemoveItem(ItemUID, NpcID, stream))
                            {
                                stream.WarehouseCreate(NpcID, Action, ItemUID, 0, 0);

                                client.Send(stream.FinalizeWarehouse());
                            }
                        }
                        break;
                    }
            }
        }


    }
}
