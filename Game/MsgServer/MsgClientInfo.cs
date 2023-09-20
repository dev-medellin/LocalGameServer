using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {

        public static void GetHeroInfo(this ServerSockets.Packet stream, Client.GameClient Owner, out Role.Player user)
        {
            user = new Role.Player(Owner);
            user.InitTransfer = stream.ReadUInt32();

            user.RealUID = stream.ReadUInt32();
            user.AparenceType = stream.ReadUInt16();
            uint mesh = stream.ReadUInt32();
            user.Body = (ushort)(mesh % 10000);
            user.Face = (ushort)((mesh - user.Body) / 10000);
            user.Hair = stream.ReadUInt16();
            user.Money = stream.ReadUInt32();
            user.ConquerPoints = stream.ReadUInt32();
            user.Experience = stream.ReadUInt64();

            user.ServerID = (ushort)stream.ReadUInt16();

            user.SetLocationType = (ushort)stream.ReadUInt16();


            user.VirtutePoints = stream.ReadUInt32();
            user.HeavenBlessing = stream.ReadInt32();
            user.Strength = stream.ReadUInt16();
            user.Agility = stream.ReadUInt16();
            user.Vitality = stream.ReadUInt16();
            user.Spirit = stream.ReadUInt16();
            user.Atributes = stream.ReadUInt16();
            user.HitPoints = stream.ReadInt32();
            user.Mana = stream.ReadUInt16();
            user.PKPoints = stream.ReadUInt16();
            user.Level = stream.ReadUInt8();
            user.Class = stream.ReadUInt8();
            user.FirstClass = stream.ReadUInt8();
            user.SecondClass = stream.ReadUInt8();
            user.NobilityRank = (Role.Instance.Nobility.NobilityRank)stream.ReadUInt8();
            user.Reborn = stream.ReadUInt8();
            stream.SeekForward(sizeof(byte));
            user.QuizPoints = stream.ReadUInt32();
            stream.SeekForward(sizeof(uint));
            user.Enilghten = stream.ReadUInt16();
            user.EnlightenReceive = (ushort)(stream.ReadUInt16() / 100);
            stream.SeekForward(sizeof(uint));
            user.VipLevel = (byte)stream.ReadUInt32();
            user.MyTitle = (byte)stream.ReadUInt16();
            user.BoundConquerPoints = stream.ReadInt32();
            stream.SeekForward(sizeof(byte));
            stream.SeekForward(sizeof(uint));
            stream.SeekForward(2 * sizeof(uint));
            stream.SeekForward(sizeof(ushort));
            stream.SeekForward(sizeof(uint));
            stream.SeekForward(sizeof(uint));
            stream.SeekForward(sizeof(uint));
            string[] strs = stream.ReadStringList();
            user.Name = strs[0];
            user.Spouse = strs[1];


        }


        public static unsafe ServerSockets.Packet HeroInfo(this ServerSockets.Packet stream, Role.Player client, int inittransfer = 0)
        {
            stream.InitWriter();
            stream.Write(client.UID);
            stream.Write((ushort)client.AparenceType);
            stream.Write(client.Mesh);
            stream.Write(client.Hair);
            stream.Write((uint)client.Money);
            stream.Write(client.ConquerPoints);
            stream.Write(client.Experience);//22


            stream.Write(client.SetLocationType);//30
            stream.Write((uint)0);//32
            stream.Write((uint)0);//36
            stream.Write((ushort)0);//40
            stream.Write(client.VirtutePoints);//42
            stream.Write(client.HeavenBlessing);//forinterserver//46
            stream.Write(client.Strength);//50
            stream.Write(client.Agility);
            stream.Write(client.Vitality);
            stream.Write(client.Spirit);
            stream.Write(client.Atributes);
            stream.Write((ushort)client.HitPoints);
            stream.Write(client.Mana);
            stream.Write(client.PKPoints);
            stream.Write((byte)client.Level);
            stream.Write(client.Class);
            stream.Write(client.FirstClass);
            stream.Write(client.SecondClass);
            stream.Write((byte)client.NobilityRank);
            stream.Write(client.Reborn);//71
            stream.Write((byte)0);//72
            stream.Write(client.QuizPoints);//73
            stream.Write(client.Enilghten);//77
            stream.Write((ushort)(client.EnlightenReceive * 100));//79
            stream.Write((uint)0);//unknow81
            stream.Write((uint)client.VipLevel);//85
            stream.Write((ushort)client.MyTitle);//89
            stream.Write(client.BoundConquerPoints);//91

            if (client.SubClass != null)
            {
                stream.Write((byte)client.ActiveSublass);
                stream.Write(client.SubClass.GetHashPoint());
            }
            else
                stream.ZeroFill(5);//95
            stream.Write((uint)0);//100
            stream.Write((ushort)client.DonationPoints);//104
            stream.ZeroFill(2);
            stream.Write((byte)3);
            //stream.Write((byte)1);
            //stream.Write((byte)2);
            stream.WriteStringWithLength(client.Name);
            stream.ZeroFill(1);
            stream.WriteStringWithLength(client.Spouse);
            stream.Finalize(GamePackets.HeroInfo);
            return stream;
        }

    }

}
