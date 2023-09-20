using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgTournaments
{
    public enum TournamentType : byte
    {
        None = 0,
        //DragonWar = 1,
        //FootBall = 2,
        //BattleField = 3,
        //DBShower = 4,
        TeamDeathMatch = 5,
        LastManStand = 6,
        BettingCPs = 7,
        //ExtremePk = 7,
        //KillerOfElite = 8,
        //TreasureThief =9, 
        FreezeWar = 10,
        //SkillTournament = 11,
        KingOfTheHill = 12,
        CouplesPK,
        KillTheCaptain,
        FiveNOut,
        FrozenSky,
        //Count = 13,
        QuizShow = 150
    }
}
