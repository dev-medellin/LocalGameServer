using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace COServer.Game.MsgServer
{
    public static class CheatEx
    {
        public static unsafe void GetCheatPacket(this ServerSockets.Packet stream, out int subtype, out string Conquer,
            out string MagicType, out string MagicEffect, out string C3_WDB, out string DLL_Hash, out string reason, out string Hashes)
        {
            Conquer = MagicEffect = MagicType = C3_WDB = DLL_Hash = reason = Hashes = "";
            subtype = stream.ReadInt32();
            if (subtype == 1)
            {
                Conquer = stream.ReadCString(33);
                MagicType = stream.ReadCString(33);
                MagicEffect = stream.ReadCString(33);
                C3_WDB = stream.ReadCString(33);
                DLL_Hash = stream.ReadCString(33);
                Hashes = stream.ReadCString(100);
            }
            else if (subtype == 2) // Hack detected.
            {
                reason = stream.ReadCString(100);
            }
        }
    }
    public class MsgCheatPacket
    {
        const string Hash1 = "13627237165336698541", Hash2 = "16383272528844131493", Hash3 = "18065140562294159476", Hash4 = "16454834404412545238", Hash5 = "1410152688581965";
        [PacketAttribute(GamePackets.CheatPacket)]
        public async static void CheatPacketHandler(Client.GameClient client, ServerSockets.Packet stream)
        {
            int subtype;
            string Conquer;
            string MagicType;
            string MagicEffect;
            string C3_WDB;
            string DLL_Hash;
            string CheatReason;
            string Hashes;
            stream.GetCheatPacket(out subtype, out Conquer, out MagicType, out MagicEffect, out C3_WDB, out DLL_Hash, out CheatReason, out Hashes);
            switch (subtype)
            {
                case 1:// files packet
                    {
                        if (!MsgCheatPacket.Validated(Conquer, MagicType, MagicEffect, C3_WDB, DLL_Hash) && !client.ProjectManager)
                        {
                            MsgCheatPacket.Report(client.Player.Name, "Files Changed");

                            Console.WriteLine($"Detected files changed on client [{client.Player.Name}].");
                            Task delay = Task.Delay(5000);
                            client.Player.MessageBox("Modified files in client or not updated to latest patch! Please patch! You'll be disconnected.", null, null);
                            await delay;
                            client.Socket.Disconnect();
                            Role.Core.SendGlobalMessage(stream, client.Player.Name + "<<< This ape was cheating by changing his client files and was disconnected.", MsgMessage.ChatMode.TopLeftSystem);

                            return;
                        }
                        if (DLLHash != DLL_Hash)
                        {
                        }
                        string[] HashesSplit = Hashes.Split(' ');
                        if (HashesSplit.Length == 5)
                        {
                            if (!(HashesSplit[0] == Hash1 && HashesSplit[1] == Hash2 && HashesSplit[2] == Hash3 && HashesSplit[3] == Hash4 && HashesSplit[4] == Hash5))
                            {
                                MsgCheatPacket.Report(client.Player.Name, "Memory Edits.");

                                //   Console.WriteLine($"Detected changes on client [{client.Player.Name}].");
                                Task delay = Task.Delay(5000);
                                client.Player.MessageBox("Modified files/cheat in client or not updated to latest patch! Please patch! You'll be disconnected.", null, null);
                                //await delay;
                                client.Socket.Disconnect();
                                Role.Core.SendGlobalMessage(stream, client.Player.Name + "<<< This ape was cheating by changing his client files and was disconnected.", MsgMessage.ChatMode.TopLeftSystem);
                                return;
                            }
                        }
                        client.LastCheatPacket = DateTime.Now;
                        break;
                    }
                case 2:
                    {
                        MsgCheatPacket.Report(client.Player.Name, "CheatReason");

                        Console.WriteLine($"Detected {CheatReason} on player [{client.Player.Name}]");
                        Task delay = Task.Delay(5000);
                        client.Player.MessageBox("Cheat has been detected in your client and was reported! You'll be disconnected.", null, null);
                        await delay;
                        client.Socket.Disconnect();
                        Role.Core.SendGlobalMessage(stream, client.Player.Name + "<<< This ape was cheating and was disconnected.", MsgMessage.ChatMode.TopLeftSystem);

                        break;
                    }
                case 3:
                    {
                        Console.WriteLine($"Suspicious injection: {CheatReason} on player [{client.Player.Name}]");
                        MsgCheatPacket.Report(client.Player.Name, "Injection");

                        //client.Socket.Disconnect();

                        break;
                    }
            }

        }
        public static string ConquerHash, MagicHash, MagicHash2, C3Hash, MagicEffectHash, DLLHash;

        public static void Report(string Name, string reason)
        {
            string logs = $"[CHEAT] {Name} -- REASON: {reason}";
            Database.ServerDatabase.LoginQueue.Enqueue(logs);
        }
        public static void LoadFiles()
        {
            ConquerHash = CalculateMD5(@"Files\Conquer.exe");
            MagicHash = CalculateMD5(@"Files\magictype.dat");
            MagicHash2 = CalculateMD5(@"Files\magictype2.dat");
            C3Hash = CalculateMD5(@"Files\c3.wdb");
            MagicEffectHash = CalculateMD5(@"Files\MagicEffect.ini");
            DLLHash = CalculateMD5(@"Files\CO2Helper.dll");
            Console.WriteLine("Loaded all protection files.", ConsoleColor.Magenta);
        }
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        internal static bool Validated(string conquer, string magicType, string magicEffect, string c3_WDB, string dLL_Hash)
        {
            if (Program.TestServer)
                return true;
            if (conquer == ConquerHash && (magicType == MagicHash || magicType == MagicHash2) && magicEffect == MagicEffectHash && c3_WDB == C3Hash) //&& DLLHash == dLL_Hash)
                return true;
            return false;
        }
    }
}
