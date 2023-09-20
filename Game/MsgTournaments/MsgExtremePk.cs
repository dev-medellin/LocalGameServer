using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgTournaments
{
    public class MsgExtremePk : ITournament
    {
       public const uint RewardConquerPoints = 40000;

       public ProcesType Process { get; set; }
       public DateTime StartTimer = new DateTime();
       public DateTime ScoreStamp = new DateTime();
       public DateTime InfoTimer = new DateTime();
       public Role.GameMap Map;
       public uint DinamicID, Secounds = 0;
       public KillerSystem KillSystem;
       public TournamentType Type { get; set; }
       public MsgExtremePk(TournamentType _type)
       {
           Type = _type;
           Process = ProcesType.Dead;
       }
       public void Open()
       {
           if (Process == ProcesType.Dead)
           {
               KillSystem = new KillerSystem();
               Map = Database.Server.ServerMaps[1505];
               DinamicID = Map.GenerateDynamicID();
#if Arabic
                 MsgSchedules.SendInvitation("ExtremePk", "ConquerPoints", 315, 150, 1002, 0, 60);
#else
               MsgSchedules.SendInvitation("ExtremePk", "ConquerPoints", 315, 150, 1002, 0, 60);
#endif
             
               StartTimer = DateTime.Now;
               InfoTimer = DateTime.Now;
               Secounds = 60;
               Process = ProcesType.Idle;
           }
       }
       public void Revive(Extensions.Time32 Timer, Client.GameClient user)
       {
           if (user.Player.Alive == false && Process != ProcesType.Dead)
           {
               if (InTournament(user))
               {
                   if (user.Player.DeadStamp.AddSeconds(4) < Timer)
                   {
                       ushort x = 0;
                       ushort y = 0;
                       Map.GetRandCoord(ref x, ref y);
                       user.Teleport(x, y, Map.ID, DinamicID);
                   }
               }
           }
       }
       public bool Join(Client.GameClient user,ServerSockets.Packet stream)
       {
           if (Process == ProcesType.Idle)
           {
               user.Player.XtremePkPoints = 1000;
               ushort x = 0;
               ushort y = 0;

               Map.GetRandCoord(ref x, ref y);
               user.Teleport(x, y, Map.ID, DinamicID);
               return true;
           }
           return false;
       }

       public void CheckUp()
       {
           if (Process == ProcesType.Idle)
           {
               if (DateTime.Now > StartTimer.AddMinutes(1))
               {
#if Arabic
                    MsgSchedules.SendSysMesage("ExtremePk has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                   MsgSchedules.SendSysMesage("ExtremePk has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#endif
                   Process = ProcesType.Alive;
                   StartTimer = DateTime.Now;
               }
               else if (DateTime.Now > InfoTimer.AddSeconds(10))
               {
                   Secounds -= 10;
#if Arabic
                    MsgSchedules.SendSysMesage("[ExtremePk] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                   MsgSchedules.SendSysMesage("[ExtremePk] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#endif
                   InfoTimer = DateTime.Now;
               }
           }
           if (Process == ProcesType.Alive)
           {
               DateTime Now = DateTime.Now;

               if (Now > StartTimer.AddMinutes(10))
               {
#if Arabic
                     MsgSchedules.SendSysMesage("ExtremePk has ended. All Players of ExtremePk has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                   MsgSchedules.SendSysMesage("ExtremePk has ended. All Players of ExtremePk has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#endif
                  var array = MapPlayers().OrderByDescending(p => p.Player.XtremePkPoints).ToArray();
                   if (array.Length > 0)
                   {
                       var Winner = array.First();
                       MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  ExtremePk. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
                       Winner.Player.ConquerPoints += RewardConquerPoints;
                       using (var rec = new ServerSockets.RecycledPacket())
                       {
                           var stream = rec.GetStream();
                           if (Winner.Inventory.HaveSpace(2))
                               Winner.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2);
                           else
                               Winner.Inventory.AddReturnedItem(stream, Database.ItemType.PowerExpBall, 2);
                           Winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints and 2PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                    
                       }
                       foreach (var user in array)
                       {
                           user.Teleport(315, 150, 1002);
                       }
                   }
                   Process = ProcesType.Dead;
               }

               if (Now > ScoreStamp)
               {
                     using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var array = MapPlayers().OrderByDescending(p => p.Player.XtremePkPoints).ToArray();

                        foreach (var user in MapPlayers())
                        {
                            Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("---Your Score:" + user.Player.XtremePkPoints + "---", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                            user.Send(msg.GetArray(stream));
                            msg = new MsgServer.MsgMessage("My tournament Kills: " + user.Player.TournamentKills.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
                        }

                        int x = 0;
                        foreach (var obj in array)
                        {
                            if (x == 4)
                                break;
                            Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.XtremePkPoints.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            SendMapPacket(amsg.GetArray(stream));
                            x++;
                        }
                      
                    }
                   ScoreStamp = Now.AddSeconds(4);
               }

             
           }
       }

       public void SharePoints(Client.GameClient user, Client.GameClient Killer)
       {
           uint xtremepoints = (uint)(user.Player.XtremePkPoints / 5);
           Killer.Player.XtremePkPoints += xtremepoints;
           user.Player.XtremePkPoints -= xtremepoints;

           Killer.SendSysMesage("You gained " + xtremepoints.ToString() + " points for killing " + user.Player.Name + "", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
           user.SendSysMesage("You lost " + xtremepoints.ToString() + " points when " + Killer.Player.Name.ToString() + " has killed you.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);

       }

       public void SendMapPacket(ServerSockets.Packet stream)
       {
           foreach (var user in MapPlayers())
               user.Send(stream);
       }

       public bool InTournament(Client.GameClient user)
       {
           if (Map == null)
               return false;
           return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
       }

       public Client.GameClient[] MapPlayers()
       {
           return Map.Values.Where(p => InTournament(p)).ToArray();
       }
    }
}
