using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using COServer.Game.MsgFloorItem;
using System.IO;
using COServer.ServerSockets;

namespace COServer.Game.MsgServer
{
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct WalkQuery
    {       
        public uint Direction;       
        public uint UID;       
        public uint Running;       
        public uint TimeStamp;
    }
    public static unsafe class MsgMovement
    {
        public const uint Walk = 0, Run = 1, Steed = 9;


        public static sbyte[] DeltaMountX = new sbyte[24] { 0, -2, -2, -2, 0, 2, 2, 2, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1 };
        public static sbyte[] DeltaMountY = new sbyte[24] { 2, 2, 0, -2, -2, -2, 0, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2 };


        public static unsafe void GetWalk(this ServerSockets.Packet stream, WalkQuery* pQuery)
        {
            stream.ReadUnsafe(pQuery, sizeof(WalkQuery));
        }

        public static unsafe ServerSockets.Packet MovementCreate(this ServerSockets.Packet stream, WalkQuery* pQuery)
        {
            stream.InitWriter();
            pQuery->TimeStamp = (uint)Extensions.Time32.Now.Value;
            stream.WriteUnsafe(pQuery, sizeof(WalkQuery));
            stream.Finalize(GamePackets.Movement);

            return stream;
        }

        public static uint Bodyyyy = 0;
        public static uint UIDDDD = 1000000;
        public static int eeffect = 1;
        public static int LastClientStamp = 0;
        [PacketAttribute(GamePackets.Movement)]
        public unsafe static void Movement(Client.GameClient client, ServerSockets.Packet packet)
        {
            client.Player.LastMove = DateTime.Now;
            if (client.Player.BlockMovementCo)
            {
                if (DateTime.Now < client.Player.BlockMovement)
                {
                    client.SendSysMesage($"You can`t move for {(client.Player.BlockMovement - DateTime.Now).TotalSeconds} seconds.");
                    client.Pullback();
                    return;
                }
                else
                    client.Player.BlockMovementCo = false;
            }
            if (client.Player.Away == 1)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var apacket = rec.GetStream();
                    client.Player.Away = 0;
                    client.Player.View.SendView(client.Player.GetArray(apacket, false), false);
                }
            }
            Role.Flags.ConquerAngle dir;

            WalkQuery walkPacket;

            packet.GetWalk(&walkPacket);
            walkPacket.UID = client.Player.UID;


            client.Player.Action = Role.Flags.ConquerAction.None;

            client.OnAutoAttack = false;
            client.Player.RemoveBuffersMovements(packet);

            client.Player.Protect = Extensions.Time32.Now;


            if (walkPacket.Running == MsgMovement.Steed)
            {

                dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 24);

                client.Player.View.SendView(packet.MovementCreate(&walkPacket), true);


                int newX = client.Player.X + DeltaMountX[(byte)dir];
                int newY = client.Player.Y + DeltaMountY[(byte)dir];
#if TEST
                MyConsole.WriteLine("Steed walk direction -> " + dir.ToString() + " " + (byte)dir + ", X " + newX + " Y " + newY + "");
#endif
                if (client.Map == null)
                {
                    client.Teleport(428, 378, 1002);
                    return;
                }
                if (client.Player.Map == 1038)
                {
                    if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, client.Player.X, client.Player.Y))
                    {
                        client.SendSysMesage("Illegal jumping over the gates detected.");
                        client.Pullback();
                        return;
                    }
                }
                if (!client.Map.ValidLocation((ushort)newX, (ushort)newY))
                {
                    client.Pullback();
                    return;
                }
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, newX, newY);
                client.Player.X = (ushort)newX;
                client.Player.Y = (ushort)newY;

                client.Player.Action = Role.Flags.ConquerAction.None;
                client.Player.View.Role(false, packet.MovementCreate(&walkPacket));

                if (client.Vigor >= 2)
                    client.Vigor -= 2;
                else
                    client.Vigor = 0;

                client.Send(packet.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));


            }
            else
            {
                ushort walkX = client.Player.X, walkY = client.Player.Y;
                dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 8);
                Role.Core.IncXY(dir, ref walkX, ref walkY);


#if TEST
                MyConsole.WriteLine("walk direction -> " + dir.ToString() + " " + (byte)dir + ", X " + walkX + " Y " + walkY + "");
#endif
                if (client.Map == null)
                {
                    client.Teleport(428, 378, 1002);
                    return;
                }
                if (client.Player.Map == 1038)
                {
                    if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, walkX, walkY))
                    {
                        client.SendSysMesage("Illegal jumping over the gates detected.");
                        client.Pullback();
                        return;
                    }
                }
                /*if (!client.Map.ValidLocation((ushort)walkX, (ushort)walkY))
                {
                    client.Teleport(client.Player.Px, client.Player.Py, client.Player.Map, client.Player.DynamicID);
                    return;
                }*/

                if (client.Player.ObjInteraction != null)
                {
                    if (client.Player.ObjInteraction.Player.X == client.Player.X && client.Player.ObjInteraction.Player.Y == client.Player.Y)
                    {

                        InterActionWalk query = new InterActionWalk()
                        {
                            Mode = MsgInterAction.Action.Walk,
                            UID = client.Player.UID,
                            OponentUID = client.Player.ObjInteraction.Player.UID,
                            DirectionOne = (byte)dir
                        };

                        client.Player.View.SendView(packet.InterActionWalk(&query), true);

                        client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                        client.Player.X = walkX;
                        client.Player.Y = walkY;
                        client.Player.Angle = dir;

                        client.Player.View.Role(false, packet.InterActionWalk(&query));

                        client.Map.View.MoveTo<Role.IMapObj>(client.Player.ObjInteraction.Player, walkX, walkY);
                        client.Player.ObjInteraction.Player.X = walkX;
                        client.Player.ObjInteraction.Player.Y = walkY;
                        client.Player.ObjInteraction.Player.Angle = dir;

                        client.Player.ObjInteraction.Player.View.Role();
                        return;
                    }
                }
                client.Player.View.SendView(packet.MovementCreate(&walkPacket), true);
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                client.Player.X = walkX;
                client.Player.Y = walkY;
                client.Player.Angle = dir;

                client.Player.View.Role(false, packet.MovementCreate(&walkPacket));
            }
            if (MsgTournaments.MsgSchedules.CaptureTheFlag != null)
                MsgTournaments.MsgSchedules.CaptureTheFlag.ChechMoveFlag(client);
            if (MsgTournaments.MsgSchedules.SteedRace.IsOn)
            {
                if (MsgTournaments.MsgSteedRace.MAPID == client.Player.Map)
                    MsgTournaments.MsgSchedules.SteedRace.CheckForRaceItems(client);
            }
            if (client.Player.ActivePick)
                client.Player.RemovePick(packet);







        }
    }
}
