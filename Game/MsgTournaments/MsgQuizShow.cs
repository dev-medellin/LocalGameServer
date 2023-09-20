using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COServer.Game.MsgServer;

namespace COServer.Game.MsgTournaments
{
    public class MsgQuizShow : ITournament
    {
        public const int MAX_QUESTIONS = 20;
        
        public Extensions.Time32 StartQuestion = new Extensions.Time32();
    

        public List<int> UseQuestion = new List<int>();
        public Database.QuizShow.Question CurrentQuestion = null;

        public Extensions.SafeDictionary<uint, Client.GameClient> Participants = new Extensions.SafeDictionary<uint, Client.GameClient>();
        public ProcesType Process { get; set; }
        public TournamentType Type { get; set; }


        public MsgQuizShow(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }
        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                Process = ProcesType.Idle;
                UseQuestion = new List<int>();
                Participants = new Extensions.SafeDictionary<uint, Client.GameClient>();
               
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    foreach (var user in Database.Server.GamePoll.Values)
                    {
                        Join(user, stream);
                       
                    }
                }
                StartQuestion = Extensions.Time32.Now;
         

            }
        }
        public uint TimeLeft
        {
            get
            {
                int val = (int)((StartQuestion.AddSeconds(30).AllMilliseconds - Extensions.Time32.Now.AllMilliseconds) / 1000);
                if (val < 0) val = 0;
                return (uint)val;
            }
        }
        public bool Join(Client.GameClient user,ServerSockets.Packet stream)
        {
            if (!Participants.ContainsKey(user.Player.UID) && Process != ProcesType.Dead)
            {
                user.QuizShowPoints = 0;
                user.StartQuizTimer = DateTime.Now;
                user.RightAnswer = 2;
                Participants.Add(user.Player.UID, user);
                user.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.AcotionID.Open, (ushort)TimeLeft, (ushort)(20 - UseQuestion.Count), 30, 0, 900, 600, 300));
                return true;
            }
            return false;
        }
        public void RemovePlayer(Client.GameClient user)
        {
            if (Participants.ContainsKey(user.Player.UID))
            {
                Participants.Remove(user.Player.UID);
            }
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (Extensions.Time32.Now > StartQuestion.AddSeconds(30))
                {
                    CurrentQuestion = GenerateQuestion();
                    StartQuestion = Extensions.Time32.Now;
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        foreach (var user in Participants.GetValues())
                        {
                            user.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.AcotionID.SendQuestion, (ushort)UseQuestion.Count, user.RightAnswer, 20, 0, (ushort)TimeLeft, user.QuizShowPoints, 0, CurrentQuestion.GetStr()));
                        }
                    }
                    Process = ProcesType.Alive;
                }
            }
            else if (Process == ProcesType.Alive)
            {
                if (TimeLeft == 0)
                {
                    var ranks = CalculateRank();
                    if (UseQuestion.Count == 20)
                    {
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            foreach (var user in Participants.GetValues())
                            {
                                user.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.AcotionID.SendToping, user.QuizShowPoints, (ushort)user.GetQuizTimer(), (byte)user.QuizRank, user.RightAnswer, 0, 0, 0, ranks));
                                user.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.AcotionID.HistoryBestsRank,(ushort)user.QuizRank, (ushort)Math.Floor(user.QuizShowPoints * 3.30), (byte)user.GetQuizTimer(),0, user.QuizShowPoints,0,0,ranks));
                            }
                        }
                        Process = ProcesType.Dead;
                        return;
                    }
                    CurrentQuestion = GenerateQuestion();
                    StartQuestion = Extensions.Time32.Now;
                   
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        foreach (var user in Participants.GetValues())
                        {
                            user.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.AcotionID.SendQuestion, (ushort)UseQuestion.Count, user.RightAnswer, 20, 0, (ushort)TimeLeft, user.QuizShowPoints, 0, CurrentQuestion.GetStr()));
                            user.Send(stream.QuizShowCreate(MsgServer.MsgQuizShow.AcotionID.SendToping, user.QuizShowPoints, (ushort)user.GetQuizTimer(), (byte)user.QuizRank, user.RightAnswer, 0, 0, 0, ranks));
                        }
                    }

                }

            }
        }


        public Client.GameClient[] CalculateRank()
        {

            var data = Participants.GetValues().ToArray();
            Array.Sort(data, (c1, c2) =>
            {
                int val = c2.QuizShowPoints.CompareTo(c1.QuizShowPoints);
                if (c1.QuizShowPoints == c2.QuizShowPoints)
                    val = c1.GetQuizTimer().CompareTo(c2.GetQuizTimer());
                return val;
            });
            for (int x = 0; x < data.Length; x++)
                data[x].QuizRank = Math.Min(255, x + 1);
            return data;
        }

        public Database.QuizShow.Question GenerateQuestion()
        {
            var array_question = Database.QuizShow.Questions.Where(p => UseQuestion.Contains(p.Index) == false).ToArray();
            var question = array_question[Program.GetRandom.Next(0, array_question.Length)];
            UseQuestion.Add(question.Index);
            return question;

        }
        public bool InTournament(Client.GameClient user)
        {
            return Participants.ContainsKey(user.Player.UID);
        }
    }
}
