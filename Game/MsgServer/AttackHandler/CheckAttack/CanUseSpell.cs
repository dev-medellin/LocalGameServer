using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer.Game.MsgServer.AttackHandler.CheckAttack
{
    public class CanUseSpell
    {
        public unsafe static bool Verified(InteractQuery Attack, Client.GameClient client, Dictionary<ushort, Database.MagicType.Magic> DBSpells
            , out MsgSpell ClientSpell, out Database.MagicType.Magic Spell)
        {
            try
            {
                //anti proxy --------------------------

                if (Database.MagicType.RandomSpells.Contains((Role.Flags.SpellID)Attack.SpellID))
                {
                    if (client.Player.RandomSpell != Attack.SpellID)
                    {
                        ClientSpell = default(MsgSpell);
                        Spell = default(Database.MagicType.Magic);
                        return false;
                    }
                    client.Player.RandomSpell = 0;
                }
                //-------------------------------------
                if (client.MySpells.ClientSpells.TryGetValue(Attack.SpellID, out ClientSpell))
                {
                    if (DBSpells.TryGetValue(ClientSpell.Level, out Spell))
                    {

                        if (Spell.Type == Database.MagicType.MagicSort.DirectAttack || Spell.Type == Database.MagicType.MagicSort.Attack)
                        {

                            if (!client.IsInSpellRange(Attack.OpponentUID, Spell.Range))
                            {
                                ClientSpell = default(MsgSpell);
                                Spell = default(Database.MagicType.Magic);
                                return false;
                            }
                        }

                        uint IncreaseSpellStamina = 0;//constant
                        if (client.Player.ContainFlag(MsgUpdate.Flags.ScurvyBomb))
                            IncreaseSpellStamina = (uint)(client.Player.UseStamina + 5);
                        if (client.Player.Map != 1039)
                        {
                            if (Spell.UseStamina + IncreaseSpellStamina > client.Player.Stamina)
                                return false;
                            else
                            {
                                if ((ushort)(Spell.UseStamina + IncreaseSpellStamina) > 0)
                                {
                                    client.Player.Stamina -= (ushort)(Spell.UseStamina + IncreaseSpellStamina);
                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var stream = rec.GetStream();
                                        client.Player.SendUpdate(stream, client.Player.Stamina, MsgUpdate.DataType.Stamina);
                                    }
                                }
                            }
                            if (Spell.UseMana > client.Player.Mana)
                                return false;
                            else
                            {
                                if (Spell.UseMana > 0)
                                {
                                    client.Player.Mana -= Spell.UseMana;
                                }
                            }
                        }
                        if (Spell.UseArrows > 0 && Spell.ID >= 8000 && Spell.ID <= 9875)
                        {
                            if (!client.Equipment.FreeEquip(Role.Flags.ConquerItem.LeftWeapon))
                            {
                                Game.MsgServer.MsgGameItem arrow = null;
                                client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out arrow);
                                if (arrow.Durability <= 0)//< Spell.UseArrows)                                                                  
                                    return false;
                                else
                                {
                                    if (client.Player.Map != 1039)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();

                                            arrow.Durability -= (ushort)Math.Min(arrow.Durability, Spell.UseArrows);
                                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, arrow.UID, arrow.Durability, 0, 0, 0, 0));
                                            if (arrow.Durability <= 0 /*<= Spell.UseArrows*/ || arrow.Durability > 10000)
                                                ReloadArrows(client.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon), client, stream);

                                        }
                                    }

                                }
                            }
                            else if (!client.Equipment.FreeEquip(Role.Flags.ConquerItem.AleternanteLeftWeapon))
                            {
                                Game.MsgServer.MsgGameItem arrow = null;
                                client.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteLeftWeapon, out arrow);
                                if (arrow.Durability <= 0)//< Spell.UseArrows)                                                                  
                                    return false;
                                else
                                {
                                    if (client.Player.Map != 1039)
                                    {
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            arrow.Durability -= (ushort)Math.Min(arrow.Durability, Spell.UseArrows);
                                            client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, arrow.UID, arrow.Durability, 0, 0, 0, 0));
                                            if (arrow.Durability <= 0 /*<= Spell.UseArrows*/ || arrow.Durability > 10000)
                                                ReloadArrows(client.Equipment.TryGetEquip(Role.Flags.ConquerItem.AleternanteLeftWeapon), client, stream);

                                        }
                                    }

                                }
                            }
                            else
                                return false;
                        }
                        if (Spell.IsSpellWithColdTime)
                        {
                            Extensions.Time32 now = Extensions.Time32.Now;
                            if (ClientSpell.ColdTime > now)
                                return false;
                            else
                            {
                                ClientSpell.IsSpellWithColdTime = true;
                                ClientSpell.ColdTime = now.AddMilliseconds(Spell.ColdTime);
                            }

                        }
                        else// if(ClientSpell.ID == 6000 || ClientSpell.ID == 10381)
                        {
                            if (ClientSpell.ID == 10381)
                            {
                                if (client.Player.WhirlWind || DateTime.Now > ClientSpell.LastUse.AddMilliseconds(600))
                                {
                                    ClientSpell.LastUse = DateTime.Now;
                                    client.Player.WhirlWind = false;
                                    return true;
                                }
                            }
                            else if (DateTime.Now > ClientSpell.LastUse.AddMilliseconds(Spell.CustomCoolDown) || Database.Server.RebornInfo.StaticSpells.Contains(Spell.ID))
                            {
                                ClientSpell.LastUse = DateTime.Now;
                                return true;
                            }
                            return false;
                        }
                        return true;
                    }
                }

                ClientSpell = default(MsgSpell);
                Spell = default(Database.MagicType.Magic);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ClientSpell = default(MsgSpell);
                Spell = default(Database.MagicType.Magic);
                return false;
            }
            return false;
        }

        public static unsafe void ReloadArrows(MsgGameItem arrow, Client.GameClient client, ServerSockets.Packet stream)
        {
            if (client.Player.Class < 40 || client.Player.Class > 45)
                return;
            if (client.Equipment.FreeEquip(Role.Flags.ConquerItem.LeftWeapon))
                return;
            if (client.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon).ITEM_ID / 1000 != 500)
                return;
            client.Equipment.DestoyArrow(Role.Flags.ConquerItem.LeftWeapon, stream);
            uint id = 1050002;
            if (arrow != null)
                id = arrow.ITEM_ID;
            if (client.Inventory.Contain(id, 1))
            {
                MsgGameItem newArrow;
                client.Inventory.SearchItemByID(id, out newArrow);
                newArrow.Position = 5;
                client.Inventory.Update(newArrow, Role.Instance.AddMode.REMOVE, stream);
                client.Equipment.Add(newArrow, stream);
                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                client.SendSysMesage("Arrows Reloaded.", MsgMessage.ChatMode.TopLeft);
                //  client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, newArrow.UID, newArrow.Durability, 0, 0, 0, 0));

            }
            else if (!client.Inventory.Contain(id, 1))
            {
                client.SendSysMesage("Can't reload arrows, you are out of " + Database.Server.ItemsBase[arrow.ITEM_ID].Name + "s!", MsgMessage.ChatMode.TopLeft);
                client.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.UpdateArrowCount, 0, 0, 0, 0, 0, 0));
            }

        }
    }
}
