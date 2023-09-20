using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace COServer.Database
{
    public class RoleQuests
    {
        public unsafe static void Save(Client.GameClient user)
        {
               WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
               if (binary.Open(Program.ServerConfig.DbLocation + "\\Quests\\" + user.Player.UID + ".bin", FileMode.Create))
               {
                   int AmountQuests =user.Player.QuestGUI.src.Count;
                   binary.Write(&AmountQuests, sizeof(int));
                   foreach (var quest in user.Player.QuestGUI.src.Values)
                   {
                       uint UID = quest.UID;
                       uint Status = (uint)quest.Status;
                       uint Time = quest.Time;
                       

                       int size = quest.Intentions.Length;
                       binary.Write(&UID, sizeof(uint));
                       binary.Write(&Status, sizeof(uint));
                       binary.Write(&Time, sizeof(uint));
                       binary.Write(&size, sizeof(int));
                       for (int x = 0; x < size; x++)
                       {
                           uint Intention = quest.Intentions[x];
                           binary.Write(&Intention, sizeof(uint));
                       }
                     
                   }
                   binary.Close();
               }
        }
        public unsafe static void Load(Client.GameClient user)
        {
            WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
            if (binary.Open(Program.ServerConfig.DbLocation + "\\Quests\\" + user.Player.UID + ".bin", FileMode.Open))
            {
                int AmountQuests = user.Player.QuestGUI.src.Count;
                binary.Read(&AmountQuests, sizeof(int));
                for (int x = 0; x < AmountQuests; x++)
                {
                    
                    uint _UID;
                    uint _Status;
                    uint _Time;
                    int Intentions;
                    uint[] QIntentions;
                    binary.Read(&_UID, sizeof(uint));
                    binary.Read(&_Status, sizeof(uint));
                    binary.Read(&_Time, sizeof(uint));
                    binary.Read(&Intentions, sizeof(int));
                    QIntentions = new uint[Intentions];
                    for (int i = 0; i < Intentions; i++)
                    {
                        uint QIntention;
                        binary.Read(&QIntention, sizeof(uint));
                        QIntentions[i] = QIntention;
                    }
                  
                    var quest = new Game.MsgServer.MsgQuestList.QuestListItem()
                    {
                        UID = _UID,
                        Status = (Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus)_Status,
                        Time = _Time,
                        Intentions = QIntentions
                    };
                   
                    if (!user.Player.QuestGUI.src.ContainsKey(quest.UID))
                        user.Player.QuestGUI.src.Add(quest.UID, quest);
                    if (quest.Status == Game.MsgServer.MsgQuestList.QuestListItem.QuestStatus.Accepted)
                        user.Player.QuestGUI.AcceptedQuests.Add(quest.UID, quest);
                }
            }
        }
    }
}
