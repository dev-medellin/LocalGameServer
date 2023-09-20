﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COServer.Game.MsgServer;

namespace COServer.Game.MsgNpc
{
    using ActionInvoker = CachedAttributeInvocation<ProcessAction, NpcAttribute, NpcID>;
    using Extensions;
    public unsafe delegate void ProcessAction(Client.GameClient user, ServerSockets.Packet stream, byte Option, string Input, uint id);

    

    public class Procesor
    {
        public static ExecuteNpcInvoker ExecuteNpc = new ExecuteNpcInvoker();
        public unsafe class InvokerClient
        {
            public Client.GameClient client;
            public byte InteractType;
            public byte option;
            public string input;
            public uint npcid;
            public InvokerClient(Client.GameClient Client, ServerSockets.Packet Server_Replay, uint _npcid, byte _InteractType, byte _option, string _input)
            {
                client = Client; 

                option = _option;
                InteractType = _InteractType;
                input = _input;
                npcid = _npcid;
            }
        }

        public static ActionInvoker invoker = new ActionInvoker(NpcAttribute.Translator);

        [PacketAttribute(GamePackets.NpcServerReplay)]
        private unsafe static void NpcServerReplay(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.InTrade == true || user.IsVendor || !user.Socket.Alive)
                return;
            if (!user.Player.Alive)
                return;
            if (user.PokerPlayer != null)
                return;
            uint npcid;
            ushort Mesh;
            byte option;
            byte type;
            NpcServerReplay.Mode Action;
            string input;
            //action ==6 place!
            stream.NpcDialog(out npcid, out Mesh, out option, out type, out Action, out input);
            if (Action == MsgNpc.NpcServerReplay.Mode.PlaceFurniture)
            {
                if (!user.Inventory.HaveSpace(1))
                {
                    user.CreateBoxDialog("Please make 1 more space your inventory.");
                    return;
                }
                Npc furniture;
                if (user.MyHouse.Furnitures.TryGetValue(npcid, out furniture))
                {
                    var npc = Database.NpcServer.GetNpcFromMesh(furniture.Mesh);
                    if (npc != null)
                    {
                        user.Inventory.Add(stream, npc.ItemID);
                        user.MyHouse.Furnitures.TryRemove(npcid, out furniture);
                        Database.ItemType.DBItem item;
                        if (Database.Server.ItemsBase.TryGetValue(npc.ItemID, out item))
                            user.SendSysMesage("You got a " + item.Name + "!", MsgMessage.ChatMode.System);
                        var action = new ActionQuery()
                        {
                            ObjId = npcid,
                            Type = ActionType.RemoveEntity
                        };
                        user.Send(stream.ActionCreate(&action));
                    }
                }
                return;
            }
            if (Action == MsgNpc.NpcServerReplay.Mode.Statue)
            {
                Npc furniture;
                if (user.MyHouse.Furnitures.TryGetValue(npcid, out furniture))
                {
                    user.MyHouse.Furnitures.TryRemove(npcid, out furniture);
                    Database.ItemType.DBItem item;

                    var action = new ActionQuery()
                    {
                        ObjId = npcid,
                        Type = ActionType.RemoveEntity
                    };
                    user.Send(stream.ActionCreate(&action));
                }
                return;
            }
            if (option == 255)
                return;
            user.ActiveNpc = (uint)npcid;
            ExecuteNpc.Enqueue(new InvokerClient(user, stream, (uint)npcid, type, option, input));
        }
        [PacketAttribute(GamePackets.NpcServerRequest)]
        private unsafe static void NpcServerRequest(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.InTrade == true || user.IsVendor || !user.Socket.Alive)
                return;
            if (!user.Player.Alive)
                return;
            uint npcid;
            ushort Mesh;
            byte option;
            byte type;
            NpcServerReplay.Mode Action;
            string input;

            stream.NpcDialog(out npcid, out Mesh, out option, out type, out Action, out input);

           
            if (type == (byte)NpcReply.InteractTypes.MessageBox)
            {
                if (Program.BlockTeleportMap.Contains(user.Player.Map))
                    return;
                if (user.Player.StartMessageBox > Extensions.Time32.Now)
                {
                    if (option == 0 && user.Player.MessageOK != null)
                        user.Player.MessageOK.Invoke(user);
                    else if (user.Player.MessageCancel != null)
                        user.Player.MessageCancel.Invoke(user);
                }
                user.Player.MessageOK = null;
                user.Player.MessageCancel = null;
                return;
            }
            if (type == 102)
            {
                if (user.Player.GuildRank == Role.Flags.GuildMemberRank.GuildLeader || user.Player.GuildRank == Role.Flags.GuildMemberRank.DeputyLeader)
                {
                    if (user.Player.MyGuild != null)
                    {
                        user.Player.MyGuild.Quit(input, true, stream);
                        return;
                    }
                }
            }
            if (option == 255 || option == 0 || user.InTrade)
                return;
            if (user.ActiveNpc == 9999997 && user.Player.WaitingKillCaptcha)
            {
                if (option == 255) return;
                if (input == user.Player.KillCountCaptcha)
                {
                    user.Player.SolveCaptcha();
                }
                else
                {
                    Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(user, stream);
                    dialog.Text("Input the current text: " + user.Player.KillCountCaptcha + " to verify your humanity.");
                    dialog.AddInput("Captcha message:", (byte)user.Player.KillCountCaptcha.Length);
                    dialog.Option("No thank you.", 255);
                    dialog.AddAvatar(39);
                    dialog.FinalizeDialog();
                }
                return;
            }
            npcid = (uint)user.ActiveNpc;

            ExecuteNpc.Enqueue(new InvokerClient(user, stream, (uint)npcid, type, option, input));
        }
        public class ExecuteNpcInvoker : ConcurrentSmartThreadQueue<InvokerClient>
        {
            public ExecuteNpcInvoker()
                : base(3)
            {
                Start(5);
            }
            public void TryEnqueue(InvokerClient action)
            {
                Enqueue(action);
            }

            protected unsafe override void OnDequeue(InvokerClient action, int time)
            {
                try
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        if (!action.client.Player.OnMyOwnServer)
                        {
                            if (action.npcid == (ushort)NpcID.KingdomMissionEnvoy
                                || action.npcid == (ushort)NpcID.RealmEnvoy)
                            {
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            }
                            else if (action.client.Player.Map == 3935 || action.client.Player.Map == Game.MsgTournaments.MsgEliteGroup.WaitingAreaID)
                            {
                                 Game.MsgNpc.Npc _obj;
                                 if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                                 {
                                     if (action.client.ProjectManager)
                                         action.client.SendSysMesage("Active Npc [" + action.npcid + "] X[" + _obj.X + "] Y[" + _obj.Y + "]", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                                     if (action.npcid >= (ushort)NpcID.Crystal1 && action.npcid <= (ushort)NpcID.Crystal5)
                                     {
                                         NpcHandler.KingDoomCrystals(action.client, stream, action.option, action.input, action.npcid);
                                         return;
                                     }

                                     Tuple<NpcAttribute, ProcessAction> processFolded;
                                     if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                         processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                                 }
                                 else
                                 {
                                     Role.IMapObj inpc;
                                     if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                                     {
                                         var npc = inpc as Role.SobNpc;
                                         Tuple<NpcAttribute, ProcessAction> processFolded;
                                         if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                             processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);

                                     }
                                 }
                            }
                            return;
                        }
                        if (action.InteractType == (byte)NpcReply.InteractTypes.MessageBox)
                        {
                            if (action.client.Player.StartMessageBox > Extensions.Time32.Now)
                            {
                                if (action.option == 255 && action.client.Player.MessageOK != null)
                                    action.client.Player.MessageOK.Invoke(action.client);
                                else if (action.client.Player.MessageCancel != null)
                                    action.client.Player.MessageCancel.Invoke(action.client);
                            }
                            action.client.Player.MessageOK = null;
                            action.client.Player.MessageCancel = null;
                            return;
                        }
                        if ((uint)action.npcid == 3124)//house WH
                        {
                            if (action.client.MyHouse != null && action.client.Player.DynamicID == action.client.Player.UID)
                            {
                                ActionQuery query = new ActionQuery()
                          {
                              Type = ActionType.OpenDialog,
                              ObjId = action.client.Player.UID,
                              dwParam = MsgServer.DialogCommands.Warehouse,
                              wParam1 = action.client.Player.X,
                              wParam2 = action.client.Player.Y
                          };
                                action.client.Send(stream.ActionCreate(&query));

                                return;
                            }
                            else
                            {
                                action.client.SendSysMesage("I'm sorry but you dont own this house !");                               
                            }
                        }
                        if (action.client.Player.Map == 1038)//Guild War
                        {
                            Role.IMapObj inpc;
                            if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                            {
                                var npc = inpc as Role.SobNpc;
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }

                        }
                        if (action.npcid >= 7832 && action.npcid <= 7840)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.CheckDesertGuardian(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid >= 8546 && action.npcid <= 8550)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.SoldierBird(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid >= 8551 && action.npcid <= 8555)
                        {
                            Game.MsgNpc.Npc _obj;
                            if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out _obj))
                            {
                                action.client.OnRemoveNpc = _obj;
                                NpcHandler.BandittiFlowers(action.client, stream, action.option, action.input, action.npcid);

                            }
                            return;

                        }
                        if (action.npcid == (uint)NpcID.SelectSacredRefineryPack || action.npcid == (uint)NpcID.SelectP7WeaponSoulPack
                            || action.npcid == (uint)NpcID.SelectP7EquipmentSoulPack
                            || action.npcid == (uint)NpcID.Steed1
                            || action.npcid == (uint)NpcID.Steed3
                            || action.npcid == (uint)NpcID.Steed6 || action.npcid == (uint)NpcID.DailyItem1
                            || action.npcid == (uint)NpcID.DailyEliteSpiritBead
                            || action.npcid == (uint)NpcID.DailyNormalSpiritBead
                            || action.npcid == (uint)NpcID.DailyRefinedSpiritBead
                            || action.npcid == (uint)NpcID.DailyUniqueSpiritBead
                            || action.npcid == (uint)NpcID.DailySuperSpiritBead
                            || action.npcid == (uint)NpcID.Level43UniqueRingPack
                            || action.npcid == (uint)NpcID.NobleSteedPack
                            || action.npcid == (uint)NpcID.DazzlingDiamondBox
                            || action.npcid == (uint)NpcID.RareSteedPack6
                            || action.npcid == (uint)NpcID.TempestSecretLetter
                            || action.npcid == (uint)NpcID.SashFragment_Realm
                            || action.npcid == (uint)NpcID.GarmentPacket
                            || action.npcid == (uint)NpcID.GarmentPacket2
                            || action.npcid == (uint)NpcID.MountPacket
                            || action.npcid == (uint)NpcID.MountPacket2
                                     || action.npcid == (uint)NpcID.AccesoryPacket
                              || action.npcid == (uint)NpcID.AccesoryPacket2
                               || action.npcid == (uint)NpcID.MountPacket3
                            || action.npcid == (uint)NpcID.GoldPrizeToken
                            || action.npcid == (uint)NpcID.BlackFridayGarmentPack
                            || action.npcid == (uint)NpcID.BlackFridayMountPack || action.npcid == (uint)NpcID.BlackFridayAccesory
                            || action.npcid == (uint)NpcID.Steed1Pack
                            || action.npcid == (uint)NpcID.Steed3Pack
                              || action.npcid == (uint)NpcID.HeavenDemonBox
                              || action.npcid == (uint)NpcID.ChaosDemonBox
                              || action.npcid == (uint)NpcID.SacredDemonBox
                              || action.npcid == (uint)NpcID.AuroraDemonBox
                              || action.npcid == (uint)NpcID.DemonBox
                              || action.npcid == (uint)NpcID.AncientDemonBox
                            || action.npcid == (uint)NpcID.FloodDemonBox
                           || action.npcid == (uint)NpcID.MrMirror2
                            || action.npcid == (uint)NpcID.SuperHeadgearPack || action.npcid == (uint)NpcID.RingPack
                            || action.npcid == (uint)NpcID.ClothingPack || action.npcid == (uint)NpcID.PowerBook
                            || action.npcid == (uint)NpcID.Level50UniqueWeaponPack
                            || action.npcid == (uint)NpcID.Level52UniqueHeadgearPack
                            || action.npcid == (uint)NpcID.Level55EliteWeaponPack
                            || action.npcid == (uint)NpcID.Level67EliteHeadgearPack
                            || action.npcid == (uint)NpcID.VIPBook
                            || action.npcid == (uint)NpcID.L60UniqueGearPack)
                        {
                            Tuple<NpcAttribute, ProcessAction> processFolded;
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            return;
                        }
                        Game.MsgNpc.Npc obj;
                        if (action.client.Map.SearchNpcInScreen((uint)action.npcid, action.client.Player.X, action.client.Player.Y, out obj))
                        {
                            if (action.client.ProjectManager)
                                action.client.SendSysMesage("Active Npc [" + action.npcid + "] X[" + obj.X + "] Y[" + obj.Y + "]", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red); if (action.client.Player.Map == 1511)
                            {
                                NpcHandler.Furnitures(action.client, stream, action.option, action.input, action.npcid);
                                return;
                            }
                            Tuple<NpcAttribute, ProcessAction> processFolded;
                            if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                            else
                            {

                                if (action.client.Player.Map == 1038)
                                {
                                    if (action.npcid == (uint)NpcID.GuildConductor1 || action.npcid == (uint)NpcID.GuildConductor2
                                        || action.npcid == (uint)NpcID.GuildConductor3 || action.npcid == (uint)NpcID.GuildConductor4)
                                        NpcHandler.GuildConductorsProces(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (((int)action.npcid >= 10031 && (int)action.npcid <= 10041 || (int)action.npcid == 10043) && action.client.Player.DynamicID == 0)
                                {
                                    NpcHandler.SpaceMarks(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (action.npcid == (uint)NpcID.TeleGuild1 || action.npcid == (uint)NpcID.TeleGuild2
                                   || action.npcid == (uint)NpcID.TeleGuild3 || action.npcid == (uint)NpcID.TeleGuild4)
                                {
                                    NpcHandler.GuildCondTeleBack(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else if (action.npcid == (uint)NpcID.WHTwin || action.npcid == (uint)NpcID.wHPheonix
                                   || action.npcid == (uint)NpcID.WHMarket || action.npcid == (uint)NpcID.WHBird
                                   || action.npcid == (uint)NpcID.WHDesert || action.npcid == (uint)NpcID.WHApe
                                    || action.npcid == (uint)NpcID.WHPoker)
                                {
                                    NpcHandler.Warehause(action.client, stream, action.option, action.input, action.npcid);
                                }

                                else if ((int)action.npcid >= 925 && (int)action.npcid <= 930 && action.client.Player.Map == 700 && action.client.Player.DynamicID == 0)
                                {
                                    NpcHandler.LotteryBoxes(action.client, stream, action.option, action.input, action.npcid);
                                }
                                else
                                {
                                    if (action.client.ProjectManager)
                                        Console.WriteLine("Not find Npc -> " + action.npcid + " ");
                                }
                            }
                        }
                        else if (action.npcid == 12)
                        {
                            if (action.client.Player.VipLevel > 0)
                            {

                                ActionQuery query = new ActionQuery()
                                {
                                    Type = ActionType.OpenDialog,
                                    ObjId = action.client.Player.UID,
                                    dwParam = MsgServer.DialogCommands.VIPWarehouse,
                                    wParam1 = action.client.Player.X,
                                    wParam2 = action.client.Player.Y
                                };
                                action.client.Send(stream.ActionCreate(&query));



                            }
                        }
                        else
                        {
                            Role.IMapObj inpc;
                            if (action.client.Player.View.TryGetValue((uint)action.npcid, out inpc, Role.MapObjectType.SobNpc))
                            {
                                var npc = inpc as Role.SobNpc;
                                Tuple<NpcAttribute, ProcessAction> processFolded;
                                if (invoker.TryGetInvoker((NpcID)action.npcid, out processFolded))
                                    processFolded.Item2(action.client, stream, action.option, action.input, action.npcid);
                          
                            }
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }
        }
    }
}
