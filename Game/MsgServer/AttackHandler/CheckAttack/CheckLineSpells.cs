using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CheckLineSpells
    {
        public static bool CheckUp(Client.GameClient user, ushort spellid)
        {

            if ((MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FreezeWar
                || MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FiveNOut
                || MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FrozenSky)
             && MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
            {
                if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(user) || UnlimitedArenaRooms.Maps.ContainsValue(user.Player.DynamicID))
                {
                    if (spellid != 1045 && spellid != 1046 && spellid != 11005 && spellid != 11000)
                    {
                        user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword/ViperFang/DragonTrail)");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
