using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgTournaments
{
  public  class MsgSquidwardOctopus
    {

      public int GoldRemain = 0;
      public int SilverRemain = 0;
      public int CopperRemain = 0;

        //12:30-13:29 and 20:30-21:29, Monday - Saturday
      public  ProcesType Process { get; set; }

      public DateTime StartTimer = new DateTime();

      public MsgSquidwardOctopus()
      {
          Process = ProcesType.Dead;
      }
      public void Start()
      {
          if (Process == ProcesType.Dead)
          {
              MsgSchedules.SendInvitation("SquidwardOctopus", "Mounts,Expericence,Refinary", 297, 208, 1002, 0, 60);
              StartTimer = DateTime.Now;
              GoldRemain = 100;
              SilverRemain = 200;
              CopperRemain = 300;

              Process = ProcesType.Alive;
          }
      }
      public void Finish()
      {
          if (Process == ProcesType.Alive)
          {
              MsgSchedules.SendSysMesage("The event SquidwardOctopus has finished.", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.yellow);
              Process = ProcesType.Dead;
          }
      }
      public void CheckUp()
      {
         if (Process == ProcesType.Alive)
          {
              DateTime Now = DateTime.Now;
              if (Now > StartTimer.AddMinutes(60))
                  Finish();
           
          }
      }
    }
}
