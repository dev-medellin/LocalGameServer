using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer
{
    public struct MsgLoginClient
    {
        public ushort Length;
        public ushort PacketID;
        public uint AccountHash;
        public uint Key;
        public static ConcurrentDictionary<string, List<string>> PlayersIP = new ConcurrentDictionary<string, List<string>>();
        [PacketAttribute(Game.GamePackets.LoginGame)]
        public unsafe static void LoginGame(Client.GameClient client, ServerSockets.Packet packet)
        {
            uint[] Decrypt = Program.transferCipher.Decrypt(new uint[] { packet.ReadUInt32(), packet.ReadUInt32() });
            client.OnLogin = new MsgLoginClient()
            {
                Key = Decrypt[0],
                AccountHash = Decrypt[1],

            };
            //string Conquer = packet.ReadCString(33),
            //MagicType = packet.ReadCString(33),
            //MagicEffect = packet.ReadCString(33),
            //C3_WDB = packet.ReadCString(33),
            //DLL_Hash = packet.ReadCString(33);
            //if (!MsgCheatPacket.Validated(Conquer, MagicType, MagicEffect, C3_WDB, DLL_Hash))
            //{
            //    using (var rec = new ServerSockets.RecycledPacket())
            //    {
            //        var stream = rec.GetStream();
            //        string Messaj = "Your client files are edit. Reinstall the client or you'll get banned.";
            //        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
            //        return;
            //    }
            //}
            client.ClientFlag |= Client.ServerFlag.OnLoggion;
            Database.ServerDatabase.LoginQueue.TryEnqueue(client);
        }
        public unsafe static void LoginHandler(Client.GameClient client, MsgLoginClient packet)
        {
            client.ClientFlag &= ~Client.ServerFlag.OnLoggion;

            if (client.Socket != null)
            {
                if (client.Socket.RemoteIp == "NONE")
                {
                    Console.WriteLine("Breack login client.");
                    return;
                }
            }
            try
            {
                if (client.OnLogin.Key > 100000000 || client.OnLogin.Key < 1000000)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        string Messaj = "You can't do this";
                        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                    }
                    return;
                }
                //string HWID;
                //using (MySqlCommand cmd = new MySqlCommand(MySqlCommandType.SELECT))
                //{
                //    cmd.Select("accounts").Where("EntityID", client.OnLogin.Key);
                //    using (MySqlReader r = new MySqlReader(cmd))
                //    {
                //        if (r.Read())
                //        {
                //            HWID = r.ReadString("HWID");
                //        }
                //    }
                //}
                //if (Database.SystemBanned.IsHWIDBanned(HWID))
                //{
                //    using (var rec = new ServerSockets.RecycledPacket())
                //    {
                //        var stream = rec.GetStream();
                //        string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";
                //        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                //    }
                //    return;
                //}

                string BanMessaje;
                if (Database.SystemBanned.IsBanned(client.Socket.RemoteIp, out BanMessaje))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";
                        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                    }
                    return;
                }
                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) == Client.ServerFlag.CreateCharacterSucces)
                {
                    if (Database.ServerDatabase.AllowCreate(client.ConnectionUID))
                    {

                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacterSucces;
                        if (client.Player.MyChi == null)
                        {
                            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);

                        }
                        if (client.Player.SubClass == null)
                            client.Player.SubClass = new Role.Instance.SubClass();
                        if (client.Player.Flowers == null)
                        {
                            client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
                            client.Player.Flowers.FreeFlowers = 1;
                        }
                        if (client.Player.Nobility == null)
                            client.Player.Nobility = new Role.Instance.Nobility(client);
                        if (client.Player.Associate == null)
                        {
                            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                            client.Player.Associate.MyClient = client;
                            client.Player.Associate.Online = true;
                        }
                        Database.Server.GamePoll.TryAdd(client.Player.UID, client);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage("ANSWER_OK", "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));


                            Database.ServerDatabase.CreateCharacte(client);
                            Database.ServerDatabase.SaveClient(client);

                            client.ClientFlag |= Client.ServerFlag.AcceptLogin;

                            Console.WriteLine(client.Player.Name + " has logged in [" + client.Socket.RemoteIp + "]", ConsoleColor.Yellow);

                            client.Send(stream.LoginHandlerCreate(1, client.Player.Map));
                            MsgLoginHandler.LoadMap(client, stream);
                        }
                        return;
                    }
                }
                if ((client.ClientFlag & Client.ServerFlag.AcceptLogin) != Client.ServerFlag.AcceptLogin)
                {

                    var login = client.OnLogin;
                    //MsgLoginClient* login = (MsgLoginClient*)&packet;

                    client.ConnectionUID = login.Key;


                    if (Database.SystemBannedAccount.IsBanned(client.ConnectionUID, out BanMessaje))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            string aMessaj = "Your account is Banned for: " + BanMessaje + " ";
                            client.Send(new MsgServer.MsgMessage(aMessaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                        return;
                    }

                    string Messaj = "NEW_ROLE";

                    if (Database.ServerDatabase.AllowCreate(login.Key) == false)
                    {

                        Client.GameClient InGame = null;
                        if (Database.Server.GamePoll.TryGetValue((uint)login.Key, out InGame))
                        {
                            if (InGame.Player != null)
                            {
                                Console.WriteLine("Account try join but is in use. " + InGame.Player.Name);

                                if (InGame.Player.UID == 0)
                                {
                                    Database.Server.GamePoll.TryRemove((uint)login.Key, out InGame);
                                    if (InGame != null && InGame.Player != null)
                                    {
                                        if (InGame.Map != null)
                                            InGame.Map.Denquer(InGame);
                                    }
                                }
                            }
                            InGame.Socket.Disconnect();
                            Messaj = "Sorry, you Account is online, Try Again";
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                            }
                            if (InGame.TRyDisconnect-- == 0)
                            {
                                if (InGame.Player != null && InGame.FullLoading)
                                {
                                    InGame.ClientFlag |= Client.ServerFlag.Disconnect;
                                    //if ((InGame.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                                    Database.ServerDatabase.SaveClient(InGame);
                                }
                                Database.Server.GamePoll.TryRemove((uint)login.Key, out InGame);
                                if (InGame != null && InGame.Player != null)
                                {
                                    if (InGame.Map != null)
                                        InGame.Map.Denquer(InGame);
                                }
                            }
                        }
                        else
                        {
                            Database.Server.GamePoll.TryAdd((uint)login.Key, client);
                            Messaj = "ANSWER_OK";
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) != Client.ServerFlag.CreateCharacterSucces)
                                {
                                    //  lock (client.Player)
                                    //      client.Player = new Role.Player(client);
                                    Database.ServerDatabase.LoadCharacter(client, (uint)login.Key);
                                }
                                client.ClientFlag |= Client.ServerFlag.AcceptLogin;
                                Console.WriteLine(client.Player.Name + " has login [" + client.Socket.RemoteIp + "]", ConsoleColor.Yellow);
                                client.IP = client.Socket.RemoteIp;
                                try
                                {
                                    List<string> ListP;
                                    if (PlayersIP.TryGetValue(client.IP, out ListP))
                                    {
                                        if (!ListP.Contains(client.Player.Name))
                                            PlayersIP[client.IP].Add(client.Player.Name);
                                    }
                                    else
                                        PlayersIP.TryAdd(client.IP, new List<string>() { client.Player.Name });
                                }
                                catch
                                {

                                }
                                client.Player.LoginStamp = Extensions.Time32.Now;
                                client.Player.Lastthread = 0;
                                client.Player.thread_time = 0;
                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                                client.Send(stream.LoginHandlerCreate(1, client.Player.Map));
                                MsgLoginHandler.LoadMap(client, stream);
                                if (client.Player.ArenaCPS != 0)
                                {
                                    client.Player.ConquerPoints += client.Player.ArenaCPS;
                                    client.Player.MessageBox($"You got {client.Player.ArenaCPS} CPs for ranking in top10 arena.", null, null);
                                    client.Player.ArenaCPS = 0;
                                }
                            }
                        }
                    }
                    else//new client
                    {
                        client.ClientFlag |= Client.ServerFlag.CreateCharacter;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteException(e); }
        }
    }
}
