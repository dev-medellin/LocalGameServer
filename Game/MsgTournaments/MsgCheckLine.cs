using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgTournaments
{
    public class MsgCheckLine
    {
        public static bool CheckLineSpells(Client.GameClient user)
        {
            if (user.MySpells.ClientSpells.ContainsKey(1045))
                return true;
            if (user.MySpells.ClientSpells.ContainsKey(1046))
                return true;


                if (user.MySpells.ClientSpells.ContainsKey(12350))
                return true;


            return false;
        }

        public static bool CheckItems(Client.GameClient user)
        {

            Game.MsgServer.MsgGameItem RightWeapon;
            if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon))
            {
                if (CheckItem(RightWeapon.ITEM_ID))
                    return true;
            }

            Game.MsgServer.MsgGameItem LeftWeapon;
            if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out LeftWeapon))
            {
                if (CheckItem(LeftWeapon.ITEM_ID))
                    return true;
            }

            Game.MsgServer.MsgGameItem AltLeftWeapon;
            if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteLeftWeapon, out AltLeftWeapon))
            {
                if (CheckItem(AltLeftWeapon.ITEM_ID))
                    return true;
            }

            Game.MsgServer.MsgGameItem AltRightWeapon;
            if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteRightWeapon, out AltRightWeapon))
            {
                if (CheckItem(AltRightWeapon.ITEM_ID))
                    return true;
            }
            return false;
        }

        public static bool CheckItem(uint ID)
        {
            return ID >= 410003 && ID <= 410439 || ID >= 420003 && ID <= 420439 || ID >= 421003 && ID <= 421439
                || ID == 410501 || ID == 410601 || ID == 410701 || ID == 410801 || ID == 410901;
        }
    }
}
