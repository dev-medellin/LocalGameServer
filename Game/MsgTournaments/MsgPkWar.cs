using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgTournaments
{
    public class MsgPkWar
    {
        public const int RewardConquerPoints = 10000, FinishMinutes = 15;

        private ProcesType Mode;
        private DateTime FinishTimer = new DateTime();

        public uint WinnerUID = 0;
        public MsgPkWar()
        {
            Mode = ProcesType.Dead;
        }


        public void Open()
        {
            if (Mode == ProcesType.Dead)
            {
                Mode = ProcesType.Idle;
                MsgSchedules.SendInvitation("WeeklyPK", "CPs,PowerExpBall", 452, 294, 1002, 0, 60);
                FinishTimer = DateTime.Now.AddMinutes(FinishMinutes);
            }
        }
        public bool AllowJoin()
        {
            return Mode == ProcesType.Idle;
        }

        public void CheckUp()
        {
            if (Mode == ProcesType.Idle)
            {
                if (DateTime.Now > FinishTimer)
                {
                    Mode = ProcesType.Dead;
                    MsgSchedules.SendSysMesage("PkWar has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);                   
                }
            }
        }
        public bool IsFinished() { return Mode == ProcesType.Dead; }
        public bool TheLastPlayer()
        {
            return Database.Server.GamePoll.Values.Where(p => p.Player.Map == 1508 && p.Player.Alive).Count() == 1;
        }
        public void GiveReward(Client.GameClient client, ServerSockets.Packet stream)
        {
            WinnerUID = client.Player.UID;
            client.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints and 2 PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
            MsgSchedules.SendSysMesage("" + client.Player.Name + " Won  PK War , he received " + RewardConquerPoints.ToString() + " ConquerPoints and 2PowerExpBalls!", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);
            string reward = "[EVENT]" + client.Player.Name + " has received " + RewardConquerPoints + " from WeeklyPK.";
            Database.ServerDatabase.LoginQueue.Enqueue(reward);
            client.Player.ConquerPoints += RewardConquerPoints;
            if (client.Inventory.HaveSpace(1))
                client.Inventory.Add(stream, Database.ItemType.PowerExpBall);
            else
                client.Inventory.AddReturnedItem(stream, Database.ItemType.PowerExpBall);

            AddTop(client);
            client.Teleport(430, 269, 1002, 0);
        }
        public void AddTop(Client.GameClient client)
        {
            if(WinnerUID == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.WeeklyPKChampion, Role.StatusFlagsBigVector32.PermanentFlag, false);
        }
    }
}
