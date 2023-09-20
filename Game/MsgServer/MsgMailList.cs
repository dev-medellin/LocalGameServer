//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace COServer.Game.MsgServer
//{
//    public static class MsgMailList
//    {
//        public static unsafe void GetMailinfo(this ServerSockets.Packet stream, out byte type, out uint ShowCount, out uint Page, out uint TotalCount)
//        {
//            type = stream.ReadUInt8();
//            ShowCount = stream.ReadUInt32();
//            Page = stream.ReadUInt32();
//            TotalCount = stream.ReadUInt32();
//        }

//        public static unsafe ServerSockets.Packet CreateMailList(this ServerSockets.Packet stream, uint ShowCount, uint TotalCount, uint Index)
//        {
//            stream.InitWriter();
//            stream.Write((byte)0);
//            stream.Write(ShowCount);//5
//            stream.Write(Index);//9
//            stream.Write(TotalCount);//13
//            return stream;
//        }
//        public static unsafe ServerSockets.Packet AddItemInboxMail(this ServerSockets.Packet stream, Role.Instance.MailPrizes.MailInfo prize)
//        {
//            stream.Write(prize.ID);//uint
//            stream.Write(prize.Sender, 32);
//            stream.Write(prize.Subject, 64);
//            stream.Write(prize.Money);//ulong
//            stream.Write(prize.ConquerPoints);//uint
//            stream.Write(prize.DominoPoints);//ulong
//            stream.Write(prize.GetExpireTime);//uint
//            stream.Write((uint)(prize.Item != null ? prize.Item.UID : 0));

//            stream.Write((byte)(prize.AttachmentType != 0 ? 1 : 0));

//            stream.Write((byte)(prize.HaveAttachment ? 1 : 0));

//            return stream;
//        }
//        public static unsafe ServerSockets.Packet MsgMailListFinalize(this ServerSockets.Packet stream)
//        {
//            stream.Finalize(GamePackets.MsgMailList);
//            return stream;
//        }
//        [PacketAttribute(GamePackets.MsgMailList)]
//        public static unsafe void Handler(Client.GameClient user, ServerSockets.Packet stream)
//        {
//#if TEST
//            MyConsole.PrintPacketAdvanced(stream.Memory, stream.Size);
//#endif

//            const int max = 7;

//            byte type;
//            uint ShowCount;

//            uint index;
//            uint TotalCount;

//            stream.GetMailinfo(out type, out ShowCount, out index, out TotalCount);

//            var array = user.Mail.Mails.GetValues().ToArray();

//            uint count = (uint)Math.Min(max, array.Length - index);

//            stream.CreateMailList(count, (uint)array.Length, index);


//            for (byte x = 0; x < count; x++)
//            {
//                if (x + index >= array.Length)
//                    break;
//                var entity = array[x + index];
//                stream.AddItemInboxMail(entity);


//            }

//            stream.MsgMailListFinalize();
//            user.Send(stream);



//        }
//    }
//}