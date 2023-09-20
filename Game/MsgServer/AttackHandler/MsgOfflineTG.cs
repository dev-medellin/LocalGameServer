using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer.AttackHandler
{
    public struct MsgOfflineTG
    {
        public enum Mode : uint
        {
            OnConfirmation = 0,
            Disconnect = 1,
            UnKnow = 2,//if send that receive -> ReplyToConfirmation
            ReplyToConfirmation = 3,
            ClaimExperience = 4
        }
        public ushort Length;
        public ushort ID;
        public Mode Action;
        public uint Unknow;
        public unsafe fixed byte TQServer[8];
        public unsafe void FinalizePacket()
        {
            fixed (byte* data = TQServer)
                *(ulong*)(data) = ServerSockets.Packet.TQServer;

        }
        public MsgOfflineTG Create()
        {
            MsgOfflineTG retn = new MsgOfflineTG();
            retn.ID = GamePackets.OfflineTG;
            retn.Length = 12;
            return retn;
        }
        [PacketAttribute(Game.GamePackets.OfflineTG)]
        public unsafe static void OfflineTG(Client.GameClient client, ServerSockets.Packet packet)
        {
            MsgOfflineTG* OfflineTG = (MsgOfflineTG*)packet.Pointer;
            switch (OfflineTG->Action)
            {
                case Mode.OnConfirmation:
                    {
                        if (client.Player.Map != 601)
                        {
                            if (client.Player.Map != 1036)
                            {
                                client.Player.PMap = client.Player.Map;
                                client.Player.PMapX = client.Player.X;
                                client.Player.PMapY = client.Player.Y;
                            }
                            client.Player.Map = 601;
                            client.Player.X = 64;
                            client.Player.Y = 56;
                            client.Player.JoinOnflineTG = DateTime.Now;
                        }
                        OfflineTG->FinalizePacket();
                        OfflineTG->Action = Mode.Disconnect;
                        client.Send((byte*)OfflineTG);

                        break;
                    }
                case Mode.ClaimExperience:
                    {
                        var T1 = new TimeSpan(DateTime.Now.Ticks);
                        var T2 = new TimeSpan(client.Player.JoinOnflineTG.Ticks);
                        ushort minutes = (ushort)(T1.TotalMinutes - T2.TotalMinutes);
                        minutes = (ushort)Math.Min((ushort)900, minutes);
                        client.Player.JoinOnflineTG = DateTime.Now;
                        client.GainExpBall(minutes, true, Role.Flags.ExperienceEffect.angelwing);
                        client.Teleport(client.Player.PMapX, client.Player.PMapY, client.Player.PMap);
                        break;
                    }
              
            }
        }
    }
}
