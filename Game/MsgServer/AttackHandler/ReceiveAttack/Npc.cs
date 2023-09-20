using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer.AttackHandler.ReceiveAttack
{
    public class Npc
    {
        public static uint Execute(ServerSockets.Packet stream, MsgSpellAnimation.SpellObj obj, Client.GameClient client, Role.SobNpc attacked)
        {

            if (obj.Damage >= attacked.HitPoints)
            {
                uint exp = (uint)attacked.HitPoints;
                attacked.Die(stream, client);
                if (attacked.Map == 1039)
                    return exp / 10;
            }
            else
            {
                attacked.HitPoints -= (int)obj.Damage;

                if (attacked.UID == Game.MsgTournaments.MsgSchedules.GuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.GuildWar.UpdateScore(client.Player, obj.Damage);
                if (attacked.UID == Game.MsgTournaments.MsgSchedules.EliteGuildWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.EliteGuildWar.UpdateScore(client.Player, obj.Damage);
                if (attacked.UID == Game.MsgTournaments.MsgSchedules.PoleDomination.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.PoleDomination.UpdateScore(client.Player, obj.Damage);
                if (attacked.UID == Game.MsgTournaments.MsgSchedules.ClassicClanWar.Furnitures[Role.SobNpc.StaticMesh.Pole].UID)
                    Game.MsgTournaments.MsgSchedules.ClassicClanWar.UpdateScore(client.Player, obj.Damage);
                if (Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.ContainsKey(attacked.UID))
                {
                    Game.MsgTournaments.MsgSchedules.CaptureTheFlag.UpdateFlagScore(client.Player, attacked, obj.Damage, stream);
                }
                //else if (Game.MsgTournaments.MsgSchedules.ClanWar.Process == MsgTournaments.ProcesType.Alive)
                //{
                //    if (Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar != null && Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.InWar(client))
                //    {
                //        if (Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.Proces == MsgTournaments.ProcesType.Alive)
                //        {
                //            Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.UpdateScore(client.Player, obj.Damage);
                //        }
                //    }
                //}
                if (attacked.Map == 1039 || attacked.Map == 1038)
                    return obj.Damage / 1000;

            }
            return 0;
        }
    }
}
