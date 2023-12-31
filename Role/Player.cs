﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using COServer.Game.MsgServer;
using COServer.Game.MsgNpc;
using Extensions;
using COServer.Game.MsgTournaments;

namespace COServer.Role
{
    public unsafe class Player : IMapObj
    {
        public Extensions.Time32 UseLayTrap = new Extensions.Time32();
        public Extensions.Time32 EarthStamp = new Extensions.Time32();
        public Extensions.Time32 LoginStamp = new Extensions.Time32();
        public ulong Lastthread;
        public ulong thread_time;
        public byte UseChiToken = 0;
        public bool OnRemoveLukyAmulet = false;
        public bool OnBluedBird = false;
        public DateTime BlueBirdPlumeStamp = new DateTime();
        public bool OnFerentPill { get { return ContainFlag(MsgUpdate.Flags.Poisoned); } }
        public DateTime FerventPill = new DateTime();
        public byte ClaimTowerAmulets = 0;//reset
        public byte LuiseQuestions = 0;
        public DateTime RealPortraitStamp = new DateTime();
        public DateTime RevealVialStamp = new DateTime();
        public DateTime LastDragonPill;
        public bool IsBoy()
        {
            return Role.Core.IsBoy(Body);
        }
        public bool IsGirl()
        {
            return Role.Core.IsGirl(Body);
        }
        public int GiveFlowersToPerformer = 0;

        public DateTime GallbladerrStamp = new DateTime();
        public int DefeatedArenaGuardians = 0;
        public DateTime JoinPowerArenaStamp = new DateTime();
        public DateTime JoinPrizeNpcOctopus = new DateTime();
        public byte OpenHousePack = 0;
        public void InitializeTransfer(uint ServerID)
        {
            if (ConquerPoints > 1000000 || Owner.Intrn == true)
            {
                var server = Database.GroupServerList.GetServer(ServerID);
                TransferToServer = server.Name;
                CheckTransfer = true;
                MsgInterServer.PipeClient.Connect(Owner, server.IPAddress, server.Port);
                Owner.CreateBoxDialog("We're preparing your transfer , please stand by ...");
            }
            else
                Owner.CreateBoxDialog("You need to have 1kk CPs for transfer ! ");
        }
        public string TransferToServer = "";
        public uint InitTransfer = 0;
        public bool CheckTransfer = false;
        public bool OnTransfer = false;
        public ushort ExtraAtributes = 0;
        public void AddExtraAtributes(ServerSockets.Packet stream, ushort value)
        {
            if (ExtraAtributes + value <= 300)
            {
                ExtraAtributes += value;
                Atributes += value;
                SendUpdate(stream, Atributes, MsgUpdate.DataType.Atributes);
            }
            else
            {
                value = (ushort)(ExtraAtributes - 300);
                ExtraAtributes += value;
                Atributes += value;
                SendUpdate(stream, Atributes, MsgUpdate.DataType.Atributes);
            }
        }
        public uint BuyItemS = 0;
        public bool OnMyOwnServer
        {
            get { return ServerID == Database.GroupServerList.MyServerInfo.ID; }
        }
        public ushort ServerID = 0;
        public ushort SetLocationType = 0;
        public DateTime SickleStamp2 = new DateTime();
        public DateTime StampJump = new DateTime();
        public int StampJumpMiliSeconds = 0;
        public DateTime KingOfTheHillStamp = new DateTime();
        public uint KingOfTheHillScore = 0;
        public uint SkillTournamentLifes = 0;
        public const ushort MaxInventorySashCount = 1000;
        public ushort InventorySashCount = 0;
        public bool Invisible = false;
        public DateTime StampSecorSpells = new DateTime();
        public DateTime StampBloodyScytle = new DateTime();
        public DateTime MedicineStamp = new DateTime();
        //public Game.MsgTournaments.MsgFreezeWar.Team.TeamType FreezeTeamType;
        public uint ReceiveTest = 0;
        public DateTime ReceivePing = DateTime.Now;
        public DateTime LastSuspect = DateTime.Now;
        public uint AtiveQuestApe = 0;
        public uint QuestCaptureType = 0;
        public Random MyRandom = new Random(Program.GetRandom.Next());
        public bool Rate(int value)
        {
            return value > MyRandom.Next() % 100;
        }
        public DateTime SickleStamp = new DateTime();
        public int FootballTeamID = 0;
        public uint FootBallMatchPoints = 0;
        public uint MyFootBallPoints = 0;
        public ulong DailySignUpDays = 0;
        public byte DailySignUpRewards = 0;
        public byte DailyMonth = 0;
        public uint DailyDays
        {
            get
            {
                uint days = 0;
                for (byte x = 0; x < 31; x++)
                    if ((DailySignUpDays & (1ul << x)) == (1ul << x))
                        days += 1;
                return days;
            }
        }
        public uint TaskReward = 0;
        public uint TaskRewardIndex = 0;
        public uint CountSpeedHack = 0;
        public ushort CountryID = 0;
        public enum MainFlagType : uint
        {
            None = 0,
            CanClaim = 1 << 0,
            ShowSpecialItems = 1 << 1,
            ClaimGift = 1 << 2,
            OnMeleeAttack = 1 << 3,
        }
        public MainFlagType MainFlag = 0;
        public ushort NameEditCount = 0;
        public uint CurrentTreasureBoxes = 0;
        public DateTime TaskQuestTimer = new DateTime();
        public uint QuestMultiple = 0;
        public DateTime AzurePillStamp = new DateTime();
        public bool StartVote = false;
        public Extensions.Time32 StartVoteStamp = new Extensions.Time32();
        public bool OnAttackPotion = false;
        public Extensions.Time32 OnAttackPotionStamp = new Extensions.Time32();
        public void ActiveAttackPotion(int Timer)
        {
            OnAttackPotion = true;
            OnAttackPotionStamp = Extensions.Time32.Now.AddMinutes(Timer);
            AddFlag(MsgUpdate.Flags.Stigma, 60, true);
            Owner.SendSysMesage("Your attack will increase during the next 30 minutes.", MsgMessage.ChatMode.System);
        }
        public bool OnDefensePotion = false;
        public Extensions.Time32 OnDefensePotionStamp = new Extensions.Time32();
        public void ActiveDefensePotion(int Timer)
        {
            OnDefensePotion = true;
            OnDefensePotionStamp = Extensions.Time32.Now.AddMinutes(Timer);
            AddFlag(MsgUpdate.Flags.Shield, 60, true);
            Owner.SendSysMesage("Your defense will increase during the next 30 minutes.", MsgMessage.ChatMode.System);
        }
        public bool AllowDynamic { get; set; }
        public uint DailyMagnoliaChance = 0;
        public uint DailyMagnoliaItemId = 0;
        public uint DailySpiritBeadCount = 0;
        public uint DailySpiritBeadItem = 0;
        public uint DailyRareChance = 0;
        public uint DailyHeavenChance = 0;
        public uint TodayChampionPoints = 0;
        public uint HistoryChampionPoints = 0;
        uint _ChampionPoints;
        public uint ChampionPoints
        {

            get
            {
                return _ChampionPoints;
            }
            set
            {

                _ChampionPoints = value;
                if (_ChampionPoints > HistoryChampionPoints)
                    HistoryChampionPoints = value;
            }
        }
        public void AddChampionPoints(uint value, bool settodayvalue = true)
        {
            if (settodayvalue)
            {
                if (TodayChampionPoints > 650)
                {
                    Owner.SendSysMesage("Your Champion Points have reached the maximum amount of 650 points. You can collect more points tomorrow.");
                    return;
                }
            }
            ChampionPoints += value;
            TodayChampionPoints += value;
        }
        public Extensions.Time32 PickStamp = Extensions.Time32.Now;
        public bool ActivePick = false;
        public void AddPick(ServerSockets.Packet stream, string Name, ushort timer)
        {
            PickStamp = Extensions.Time32.Now.AddSeconds(timer);
            Owner.Send(stream.ActionPick(UID, 1, timer, Name));
            ActivePick = true;
            ActionQuery action = new ActionQuery()
            {
                ObjId = UID,
                Type = (ActionType)1165,
                wParam1 = 277,
                wParam2 = 2050
            };
            Owner.Send(stream.ActionCreate(&action));
        }
        public void RemovePick(ServerSockets.Packet stream)
        {
            ActivePick = false;
            Owner.Send(stream.ActionPick(UID, 3, 0, Name));
        }
        public uint IndexInScreen { get; set; }
        public bool IsTrap() { return false; }
        public byte Away = 0;
        public uint OnlineMinutes = 0;
        public Extensions.Time32 OnlineStamp = Extensions.Time32.Now;
        public uint OnlineHours
        {
            get { return OnlineMinutes / 60; }
        }
        public Extensions.Time32 LastAttack;
        public Extensions.Time32 KillCountCaptchaStamp;
        public bool WaitingKillCaptcha;
        public string KillCountCaptcha;
        public Game.MsgTournaments.MsgSteedRace.UsableRacePotion[] RacePotions = null;
        public uint KillerPkPoints = 0;
        public uint XtremePkPoints = 0;
        public uint DragonWarHits = 0;
        public uint DragonWarScore = 0;
        public uint TeamDeathMacthKills = 0;
        public uint TournamentKills = 0;
        public uint KillersDisCity = 0;
        public uint AparenceType = 0;
        public unsafe void memcpy(void* dest, void* src, Int32 size)
        {
            Int32 count = size / sizeof(long);
            for (Int32 i = 0; i < count; i++)
                *(((long*)dest) + i) = *(((long*)src) + i);

            Int32 pos = size - (size % sizeof(long));
            for (Int32 i = 0; i < size % sizeof(long); i++)
                *(((Byte*)dest) + pos + i) = *(((Byte*)src) + pos + i);
        }
        public unsafe byte[] GetBytes(byte* packet)
        {
            int size = *(ushort*)(packet);
            size += 8;
            byte[] buff = new byte[size];
            fixed (byte* ptr = buff)
                memcpy(ptr, packet, size);
            return buff;
        }
        public uint TCCaptainTimes = 0;
        public bool IsCheckedPass = false;
        public uint SecurityPassword = 0;
        public uint OnReset = 0;
        public DateTime ResetSecurityPassowrd = new DateTime();
        public Extensions.Time32 LoginTimer = Extensions.Time32.Now;
        public bool InElitePk = false;
        public bool InTeamPk = false;
        public uint DonationPoints = 0;
        public uint BattleFieldPoints = 0;
        public List<byte> Titles = new List<byte>();
        public unsafe void AddTitle(byte _title, bool aSwitch)
        {
            if (!Titles.Contains(_title))
            {
                Titles.Add(_title);

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Owner.Send(stream.TitleCreate(UID, _title, MsgTitle.QueueTitle.Enqueue));

                }

                if (aSwitch)
                    SwitchTitle(_title);
            }
        }
        public unsafe void SwitchTitle(byte ntitle)
        {
            if (Titles.Contains(ntitle) || ntitle == 0)
            {
                MyTitle = ntitle;

                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    Owner.Send(stream.TitleCreate(UID, ntitle, MsgTitle.QueueTitle.Change));

                }
            }
        }
        public unsafe byte MyTitle;
        public Flags.PKMode PreviousPkMode = Flags.PKMode.Capture;
        public unsafe void SetPkMode(Flags.PKMode pkmode)
        {
            PreviousPkMode = PkMode;
            PkMode = pkmode;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery action = new ActionQuery()
                {
                    ObjId = UID,
                    dwParam = (uint)PkMode,
                    Type = ActionType.SetPkMode
                };
                Owner.Send(stream.ActionCreate(&action));
            }

        }
        public unsafe void RestorePkMode()
        {
            PkMode = PreviousPkMode;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                ActionQuery action = new ActionQuery()
                {
                    ObjId = UID,
                    dwParam = (uint)PkMode,
                    Type = ActionType.SetPkMode
                };
                Owner.Send(stream.ActionCreate(&action));
            }
        }
        public DateTime EnlightenTime = new DateTime();
        public int CursedTimer = 0;
        public void AddCursed(int time)
        {
            if (time != 0)
            {
                if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cursed))
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Cursed);

                CursedTimer += time;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, CursedTimer, Game.MsgServer.MsgUpdate.DataType.CursedTimer);
                }
                AddFlag(Game.MsgServer.MsgUpdate.Flags.Cursed, CursedTimer, false, 1);
            }
        }
        public uint MyKillerUID;
        public string MyKillerName;
        public bool Delete = false;
        public uint testtttttttttt = 0;
        public ConcurrentDictionary<ushort, FloorSpell.ClientFloorSpells> FloorSpells = new ConcurrentDictionary<ushort, FloorSpell.ClientFloorSpells>();
        public ushort RandomSpell = 0;
        public bool DbTry = false;
        public byte AddJade = 0;
        public byte LotteryEntries;
        public bool Reincarnation = false;
        Instance.Clan.Member clanmemb;
        public Instance.Clan.Member MyClanMember
        {
            get
            {
                if (clanmemb == null)
                {
                    if (MyClan != null)
                    {
                        MyClan.Members.TryGetValue(UID, out clanmemb);
                    }
                }
                return clanmemb;
            }
            set
            {
                clanmemb = value;
            }
        }
        public Instance.Clan MyClan;
        public unsafe uint ClanUID;
        public unsafe ushort ClanRank;
        public Role.Instance.Guild.Member MyGuildMember;
        public Role.Instance.Guild MyGuild;
        public uint TargetGuild = 0;
        uint _extbattle;
        public unsafe uint ExtraBattlePower
        {
            get { return _extbattle; }
            set
            {
                _extbattle = value;
            }
        }
        public unsafe Flags.GuildMemberRank GuildRank = Flags.GuildMemberRank.None;
        public unsafe uint GuildID;
        uint guildBP;
        public uint GuildBattlePower
        {
            get
            {
                return guildBP;
            }
            set
            {
                ExtraBattlePower -= guildBP;
                guildBP = value;
                ExtraBattlePower += guildBP;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, guildBP, Game.MsgServer.MsgUpdate.DataType.GuildBattlePower);
                }
            }
        }
        uint _clanbp;
        public uint ClanBp
        {
            get { return _clanbp; }
            set
            {
                ExtraBattlePower -= _clanbp;
                _clanbp = value;
                ExtraBattlePower += _clanbp;
            }
        }
        public int MaxBP()
        {
            if (Nobility == null) return 0;
            switch (Nobility.Rank)
            {
                case Instance.Nobility.NobilityRank.King: return 385;
                case Instance.Nobility.NobilityRank.Prince: return 382;
                case Instance.Nobility.NobilityRank.Duke: return 380;
                default: return 379;
            }
        }
        uint _mentorBp;
        private uint MentorBp
        {
            get { return _mentorBp; }
            set
            {
                ExtraBattlePower -= _mentorBp;
                ExtraBattlePower += value;
                _mentorBp = value;
            }
        }
        public unsafe void SetMentorBattlePowers(uint val, uint mentorPotency)
        {

            MentorBp = val;
            //using (var rec = new ServerSockets.RecycledPacket())
            //{
            //    var stream = rec.GetStream();

            //    Game.MsgServer.MsgUpdate upd = new Game.MsgServer.MsgUpdate(stream, UID, 2);
            //    stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.ExtraBattlePower, val);
            //    stream = upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.ExtraBattlePower, mentorPotency);
            //    stream = upd.GetArray(stream);
            //    Owner.Send(stream);
            //}
        }
        public uint targetTrade = 0;
        public Role.Instance.Associate.MyAsociats MyMentor = null;
        public Role.Instance.Associate.MyAsociats Associate;
        public uint TradePartner = 0;
        public uint TargetFriend = 0;
        public Role.Instance.Nobility Nobility;
        Role.Instance.Nobility.NobilityRank _NobilityRank;
        public Role.Instance.Nobility.NobilityRank NobilityRank
        {
            get { return _NobilityRank; }
            set
            {
                _NobilityRank = value;
                if (MyGuild != null && MyGuildMember != null)
                    MyGuildMember.NobilityRank = (uint)_NobilityRank;

            }
        }
        public Instance.Flowers Flowers;
        public unsafe uint FlowerRank;
        public bool OnFairy = false;
        public unsafe Game.MsgServer.MsgTransformFairy FairySpawn;
        public Instance.Chi MyChi;
        public ushort ActiveDance = 0;
        public Client.GameClient ObjInteraction;
        public unsafe Game.MsgServer.InteractQuery InteractionEffect = default(Game.MsgServer.InteractQuery);
        public bool OnInteractionEffect = false;
        public Instance.SubClass SubClass;
        public unsafe uint SubClassHasPoints;
        public unsafe Database.DBLevExp.Sort ActiveSublass;
        public bool ContainReflect
        {
            get
            {
                if (MsgSchedules.CurrentTournament.Type == TournamentType.FiveNOut || MsgSchedules.CurrentTournament.Type == TournamentType.FrozenSky)
                    if (MsgSchedules.CurrentTournament.Process != ProcesType.Dead && MsgSchedules.CurrentTournament.InTournament(this.Owner))
                        return false;
                return Database.AtributesStatus.IsWarrior(SecondClass);
            }
        }
        public bool BlackSpot = false;
        public Extensions.Time32 Stamp_BlackSpot = new Extensions.Time32();
        public byte UseStamina = 0;
        public Extensions.Time32 Protect = new Extensions.Time32();
        private Extensions.Time32 ProtectedJumpAttack = new Extensions.Time32();
        internal void ProtectAttack(int StampMiliSeconds)
        {
            Protect = Extensions.Time32.Now.AddMilliseconds(StampMiliSeconds);
        }
        internal void ProtectJumpAttack(int Seconds)
        {
            ProtectedJumpAttack = Extensions.Time32.Now.AddSeconds(Seconds);
        }
        internal bool AllowAttack()
        {
            return Extensions.Time32.Now > Protect && Extensions.Time32.Now > ProtectedJumpAttack;
        }
        public uint ShieldBlockDamage = 0;
        internal void CheckAura()
        {
            if (UseAura != Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                IncreaseStatusAura(UseAura, Aura);
            }
        }
        public Game.MsgServer.MsgUpdate.Flags UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
        public Database.MagicType.Magic Aura;
        private int AuraTimer = 0;
        internal unsafe bool AddAura(Game.MsgServer.MsgUpdate.Flags flag, Database.MagicType.Magic new_aura, int Timer)
        {
            if (flag == UseAura)
            {
                RemoveFlag(UseAura);
                DecreaseStatusAura(UseAura);
                UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
                return false;
            }
            AuraTimer = Timer;
            if (UseAura != Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                RemoveFlag(UseAura);
                DecreaseStatusAura(UseAura);
                UseAura = Game.MsgServer.MsgUpdate.Flags.Normal;
            }
            UseAura = flag;
            Aura = new_aura;
            IncreaseStatusAura(flag, new_aura);
            AddFlag(flag, Timer, true, 0);

            Game.MsgServer.MsgFlagIcon.ShowIcon icon = MsgFlagIcon.ShowIcon.EarthAura;

            if (flag == Game.MsgServer.MsgUpdate.Flags.FeandAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.FeandAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.TyrantAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.MetalAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.WoodAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.WaterAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.FireAura;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura) icon = Game.MsgServer.MsgFlagIcon.ShowIcon.EarthAura;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Owner.Send(stream.FlagIconCreate(UID, icon, new_aura.Level, (uint)new_aura.Damage));
            }

            return true;
        }
        private void DecreaseStatusAura(Game.MsgServer.MsgUpdate.Flags flag)
        {
            if (flag == Game.MsgServer.MsgUpdate.Flags.FeandAura)
                Owner.Status.Immunity -= (uint)(Aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura)
                Owner.Status.CriticalStrike -= (uint)(Aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura)
                Owner.Status.MetalResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura)
                Owner.Status.WoodResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura)
                Owner.Status.WaterResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura)
                Owner.Status.FireResistance -= (uint)Aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura)
                Owner.Status.EarthResistance -= (uint)Aura.Damage;
        }
        private void IncreaseStatusAura(Game.MsgServer.MsgUpdate.Flags flag, Database.MagicType.Magic new_aura)
        {
            if (flag == Game.MsgServer.MsgUpdate.Flags.FeandAura)
                Owner.Status.Immunity += (uint)(new_aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.TyrantAura)
                Owner.Status.CriticalStrike += (uint)(new_aura.Damage * 100);
            else if (flag == Game.MsgServer.MsgUpdate.Flags.MetalAura)
                Owner.Status.MetalResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WoodAura)
                Owner.Status.WoodResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.WaterAura)
                Owner.Status.WaterResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.FireAura)
                Owner.Status.FireResistance += (uint)new_aura.Damage;
            else if (flag == Game.MsgServer.MsgUpdate.Flags.EartAura)
                Owner.Status.EarthResistance += (uint)new_aura.Damage;
        }
        public uint SpouseUID = 0;
        public Extensions.Time32 AttackStamp = new Extensions.Time32();
        public Extensions.Time32 SpellAttackStamp = new Extensions.Time32();
        public bool OnTransform { get { return TransformationID != 0; } }
        public ClientTransform TransformInfo = null;
        public double PoisonLevel = 0;
        public byte PoisonLevehHu = 0;
        public bool ActivateCounterKill = false;
        public Action<Client.GameClient> MessageOK;
        public Action<Client.GameClient> MessageCancel;
        public Extensions.Time32 StartMessageBox = new Extensions.Time32();
        public unsafe void MessageBox(string text, Action<Client.GameClient> msg_ok, Action<Client.GameClient> msg_cancel, int Seconds = 0, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
            if (!OnMyOwnServer)
                return;
            if (Owner.PokerPlayer != null)
                return;
            if (Program.BlockTeleportMap.Contains(Owner.Player.Map))
            {
                if (Owner != null && Owner.Map != null)
                    Owner.SendSysMesage("you can't use it in " + Owner.Map.Name + " ");
                return;
            }
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                MessageOK = msg_ok;
                MessageCancel = msg_cancel;
                Game.MsgNpc.Dialog dialog = new Game.MsgNpc.Dialog(Owner, stream);
                dialog.CreateMessageBox(text).FinalizeDialog(true);
                StartMessageBox = Extensions.Time32.Now.AddHours(24);
                if (Seconds != 0)
                {
                    StartMessageBox = Extensions.Time32.Now.AddSeconds(Seconds);
                    if (messaj != Game.MsgServer.MsgStaticMessage.Messages.None)
                    {
                        Owner.Send(stream.StaticMessageCreate(messaj, MsgStaticMessage.Action.Append, (uint)Seconds));
                    }
                }
            }
        }
        public void RemoveBuffersMovements(ServerSockets.Packet stream)
        {
            InUseIntensify = false;
            //Intensify = false;

            RemoveFlag(Game.MsgServer.MsgUpdate.Flags.Praying);
            RemoveFlag(Game.MsgServer.MsgUpdate.Flags.CastPray);
            if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.MagicDefender))
            {
                RemoveFlag(Game.MsgServer.MsgUpdate.Flags.MagicDefender);
                SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.MagicDefender, 0
   , 0, 0, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
            }
        }
        public bool InUseIntensify = false;
        public Extensions.Time32 IntensifyStamp = new Extensions.Time32();
        public bool Intensify = false;
        public int IntensifyDamage = 0;
        public int BattlePower
        {
            get
            {

                int val = (int)(Level + Reborn * 5 + Owner.Equipment.BattlePower + (byte)NobilityRank + ExtraBattlePower);
                if (val > MaxBP())
                    return MaxBP();

                return Math.Min(385, val);
            }
        }
        public int RealBattlePower
        {
            get
            {
                int val = (int)(Level + Reborn * 5 + Owner.Equipment.BattlePower + (byte)NobilityRank);
                if (val > MaxBP())
                    return MaxBP();

                return val;
            }
        }
        ushort azuredef;
        public byte AzureShieldLevel;
        public ushort AzureShieldDefence
        {
            get { return azuredef; }
            set
            {
                azuredef = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, Game.MsgServer.MsgUpdate.Flags.AzureShield, 60
                        , value, AzureShieldLevel, Game.MsgServer.MsgUpdate.DataType.AzureShield, true);
                }
            }
        }
        public Extensions.Time32 XPListStamp = new Extensions.Time32();
        public ushort Stamina
        {
            get
            {
                if (UnlimitedArenaRooms.Maps.ContainsValue(DynamicID))
                    return 150;
                return _stamina;
            }
            set { _stamina = value; }
        }
        public ushort GetAddStamina()
        {
            //Role.Flags.ConquerAction act = Action;
            //if (VipLevel == 6)
            //    switch (act)
            //    {
            //        case Role.Flags.ConquerAction.Sit:
            //            return 15;
            //        default: return 5;
            //    }
            //else
            switch (Action)
            {
                case Role.Flags.ConquerAction.Sit:
                    return 12;
                default: return 5;
            }
            //return 100;
        }
        public StatusFlagsBigVector32 BitVector;
        public void AddSpellFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Seconds, bool RemoveOnDead, int StampSeconds = 0)
        {
            if (BitVector.ContainFlag((int)Flag))
                BitVector.TryRemove((int)Flag);
            AddFlag(Flag, Seconds, RemoveOnDead, StampSeconds);
        }
        public uint StatusFlag = 0;
        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Seconds, bool RemoveOnDead, int StampSeconds = 0, uint showamount = 0, uint amount = 0)
        {
            if (Flag == MsgUpdate.Flags.Freeze)
            {

            }
            if (!BitVector.ContainFlag((int)Flag))
            {
                StatusFlag |= (uint)Flag;
                BitVector.TryAdd((int)Flag, Seconds, RemoveOnDead, StampSeconds);

                UpdateFlagOffset();
                if ((int)Flag >= 52 && (int)Flag <= 60)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        View.SendView(stream.GameUpdateCreate(UID, (Game.MsgServer.MsgGameUpdate.DataType)Flag, true, showamount, (uint)Seconds, amount), true);

                    }
                }
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                StatusFlag &= (uint)~Flag;
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                if (Flag == MsgUpdate.Flags.Oblivion)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Owner.IncreaseExperience(stream, Owner.ExpOblivion);
                    }
                    Owner.ExpOblivion = 0;
                }
                if ((int)Flag >= 52 && (int)Flag <= 60)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        Owner.Send(stream.GameUpdateCreate(UID, (Game.MsgServer.MsgGameUpdate.DataType)Flag, false, 0, 0, 0));

                    }
                }
                return true;
            }
            return false;
        }
        public bool UpdateFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Seconds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Seconds, SetNewTimer, MaxTime);
        }
        public void ClearFlags()
        {
            BitVector.GetClear();
            UpdateFlagOffset();
        }
        public bool ContainFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }
        public bool CheckInvokeFlag(Game.MsgServer.MsgUpdate.Flags Flag, Extensions.Time32 timer32)
        {
            return BitVector.CheckInvoke((int)Flag, timer32);
        }
        public unsafe void UpdateFlagOffset()
        {
            SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag, true);
        }
        public unsafe void SendUpdateHP()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var MyStream = rec.GetStream();
                Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(MyStream, UID, 2);
                MyStream = Upd.Append(MyStream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Owner.Status.MaxHitpoints);
                MyStream = Upd.Append(MyStream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, HitPoints);
                MyStream = Upd.GetArray(MyStream);
                Owner.Send(MyStream);
            }
        }
        public ushort Dead_X;
        public ushort Dead_Y;
        public bool GetPkPkPoints = false;
        public bool CompleteLogin = false;
        public DateTime GhostStamp = new DateTime();
        public unsafe void Dead(Role.Player killer, ushort DeadX, ushort DeadY, uint KillerUID)
        {
            if (UnlimitedArenaRooms.Maps.ContainsValue(DynamicID))
                return;
            if (OnTransform && TransformInfo != null)
            {
                TransformInfo.FinishTransform();
            }
            else if (OnTransform)
                TransformationID = 0;

            GhostStamp = DateTime.Now.AddMilliseconds(1000);
            Owner.OnAutoAttack = false;

            Owner.SendSysMesage("You are dead.", MsgMessage.ChatMode.System);
            if (ContainFlag(MsgUpdate.Flags.FatalStrike))
                RemoveFlag(MsgUpdate.Flags.FatalStrike);
            GetPkPkPoints = true;
            if (this.ContainFlag(MsgUpdate.Flags.RedName)
                || this.ContainFlag(MsgUpdate.Flags.BlackName)
                || this.ContainFlag(MsgUpdate.Flags.FlashingName))
                GetPkPkPoints = false;

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                if (!Program.FreePkMap.Contains(Map))
                {
                    if (Associate != null && killer != null)
                    {
                        killer.Associate.AddPKExplorer(killer.Owner, this);
                        Associate.AddEnemy(Owner, killer);
                    }
                }
                if (killer != null)
                {
                    if (killer.InElitePk || killer.InTeamPk)
                    {
                        killer.TournamentKills += 1;
                    }
                }

                if (BlackSpot)
                {
                    BlackSpot = false;

                    View.SendView(stream.BlackspotCreate(false, UID), true);

                }
                Dead_X = DeadX;
                Dead_Y = DeadY;
                DeadStamp = Extensions.Time32.Now;
                DeathStamp = DateTime.Now;
                HitPoints = 0;
                if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.KillTheCaptain)
                {
                    var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgKillTheCaptain;
                    if (Owner != null && killer != null && killer.Owner != null)
                        if (tournament.InTournament(Owner) && tournament.InTournament(killer.Owner))
                        {
                            if (Owner.TeamKillTheCaptain != killer.Owner.TeamKillTheCaptain)
                            {
                                if (ContainFlag(MsgUpdate.Flags.Flashy))
                                {
                                    RemoveFlag(MsgUpdate.Flags.Flashy);
                                    killer.KillTheCaptain += 10;
                                    MsgSchedules.SendSysMesage("[KillTheCaptain] " + killer.Name + " has killed the leader of the other team and gained 10 points.", MsgMessage.ChatMode.TopLeft, MsgMessage.MsgColor.red);
                                }
                                else
                                {
                                    killer.KillTheCaptain++;
                                    killer.Owner.SendSysMesage("You gained a point for killing an enemy");
                                }
                            }
                        }
                }
                ClearFlags();
                AddFlag(Game.MsgServer.MsgUpdate.Flags.Dead, StatusFlagsBigVector32.PermanentFlag, true);



                if (Map == 700)
                {
                    Owner.EndQualifier();
                }

                if (killer != null)
                {
                    killer.XPCount++;

                    if (Map == Game.MsgTournaments.MsgTeamDeathMatch.MapID ||
                       (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.LastManStand
                       && Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(Owner)))
                    {
                        killer.TournamentKills += 1;
                    }



                    if (Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(Owner))
                    {
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.LastManStand)
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgLastManStand;
                            tournament.KillSystem.Update(killer.Owner);
                            tournament.KillSystem.CheckDead(this.UID);
                        }
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.BettingCPs)
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgBettingCompetition;
                            tournament.KillSystem.Update(killer.Owner);
                            tournament.KillSystem.CheckDead(this.UID);
                        }
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.TeamDeathMatch)
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgTeamDeathMatch;
                            tournament.KillSystem.Update(killer.Owner);
                            tournament.KillSystem.CheckDead(this.UID);
                            killer.TeamDeathMacthKills += 1;
                        }
                        if (Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == Game.MsgTournaments.TournamentType.KingOfTheHill)
                        {
                            var tournament = Game.MsgTournaments.MsgSchedules.CurrentTournament as Game.MsgTournaments.MsgKillTheCaptain;
                            killer.KingOfTheHillScore += 5;
                            killer.Owner.SendSysMesage("You gained 5 points for killing an enemy");
                        }

                    }

                    InteractQuery action = new InteractQuery()
                    {
                        UID = killer.UID,
                        X = DeadX,
                        Y = DeadY,
                        AtkType = MsgAttackPacket.AttackID.Death,
                        KillCounter = killer.KillCounter,
                        SpellID = (ushort)(Database.ItemType.IsBow(killer.Owner.Equipment.RightWeapon) ? 5 : 1),
                        OpponentUID = UID,
                    };
                    View.SendView(stream.InteractionCreate(&action), true);


                    if (!Program.NoDropItems.Contains(Map) && !Program.FreePkMap.Contains(Map))
                    {
                        if (!(Map == 1020 && MsgSchedules.PoleDomination.Proces == ProcesType.Alive))
                        {
                            if (!(Map == 1011 && DynamicID != 0))
                            {
                                if (killer.PkMode != Flags.PKMode.Jiang)
                                    CheckDropItems(killer, stream);
                                if (PKPoints >= 300)
                                {
                                    Owner.Teleport(35, 72, 6000, 0, false);
                                    JailerUID = killer.UID;

                                }
                                CheckPkPoints(killer);
                            }
                        }
                    }

                }
                else
                {


                    InteractQuery action = new InteractQuery()
                    {
                        UID = KillerUID,
                        X = DeadX,
                        Y = DeadY,
                        AtkType = MsgAttackPacket.AttackID.Death,
                        OpponentUID = UID
                    };
                    View.SendView(stream.InteractionCreate(&action), true);


                    if (!Program.NoDropItems.Contains(Map) && !Program.FreePkMap.Contains(Map))
                    {
                        CheckDropItems(killer, stream);
                    }

                }


            }
        }
        public void CheckDropItems(Role.Player killer, ServerSockets.Packet stream)
        {
            // Removed the Drop Items.
            //if (OnMyOwnServer == false)
            //    return;
            //if (Map == 3935)
            //    return;
            //try
            //{
            //    ushort x = X;
            //    ushort y = Y;
            //    if (x > 5 && y > 5)
            //    {
            //        var inventoryItems = Owner.Inventory.ClientItems.Values.ToArray();
            //        if (inventoryItems.Length / 4 > 1)
            //        {
            //            uint count = (uint)Program.GetRandom.Next(1, (int)(inventoryItems.Length / 4));

            //            for (int index = 0; index < count; index++)
            //            {
            //                try
            //                {
            //                    if (inventoryItems.Length > index && inventoryItems[index] != null)
            //                    {
            //                        var item = inventoryItems[index];
            //                        if (item.Position == (ushort)Role.Flags.ConquerItem.AleternanteBottle || item.Position == (ushort)Role.Flags.ConquerItem.Bottle)
            //                            continue;
            //                        if (item.Locked == 0 && item.Inscribed == 0 && item.Bound == 0
            //                            && !Database.ItemType.unabletradeitem.Contains(item.ITEM_ID) && !Database.ItemType.IsSash(item.ITEM_ID))
            //                        {

            //                            ushort New_X = (ushort)Program.GetRandom.Next((ushort)(x - 5), (ushort)(x + 5));
            //                            ushort New_Y = (ushort)Program.GetRandom.Next((ushort)(y - 5), (ushort)(y + 5));
            //                            if (Owner.Map.AddGroundItem(ref New_X, ref New_Y))
            //                            {
            //                                DropItem(item, New_X, New_Y, stream);
            //                            }
            //                        }
            //                    }
            //                }
            //                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            //            }

            //        }
            //    }
            //    if (PKPoints >= 30 && killer != null && !Program.FreePkMap.Contains(Map))
            //    {
            //        int Count_DropItem = (PKPoints >= 30 && PKPoints <= 99) ? 1 : 2;
            //        var EquipmentArray = Owner.Equipment.CurentEquip.Where(p => p != null &&
            //             p.Position != (ushort)Role.Flags.ConquerItem.Bottle && p.Position != (ushort)Role.Flags.ConquerItem.AleternanteBottle
            //             && p.Position != (ushort)Role.Flags.ConquerItem.Garment && p.Position != (ushort)Role.Flags.ConquerItem.AleternanteGarment
            //             && p.Position != (ushort)Role.Flags.ConquerItem.Steed && p.Position != (ushort)Role.Flags.ConquerItem.SteedMount
            //             && p.Position != (ushort)Role.Flags.ConquerItem.RightWeaponAccessory && p.Position != (ushort)Role.Flags.ConquerItem.LeftWeaponAccessory).ToArray();

            //        if (EquipmentArray.Length > 0)
            //        {
            //            int trying = 0;
            //            int Dropable = 0;
            //            Dictionary<uint, Game.MsgServer.MsgGameItem> ItemsDrop = new Dictionary<uint, Game.MsgServer.MsgGameItem>();
            //            do
            //            {
            //                if (trying == 14)
            //                    break;
            //                byte ArrayPosition = (byte)Program.GetRandom.Next(0, EquipmentArray.Length);
            //                var Element = EquipmentArray[ArrayPosition];
            //                if (!ItemsDrop.ContainsKey(Element.UID))
            //                {
            //                    ItemsDrop.Add(Element.UID, Element);
            //                    Dropable++;
            //                }
            //                trying++;
            //            }
            //            while (Dropable < Count_DropItem);

            //            //remove equip item--------------


            //            foreach (var item in ItemsDrop.Values)
            //            {

            //                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.RemoveEquipment, item.UID, item.Position, 0, 0, 0, 0));

            //                Game.MsgServer.MsgGameItem Remover;
            //                Owner.Equipment.ClientItems.TryRemove(item.UID, out Remover);
            //                if (item.Inscribed == 1)
            //                {
            //                    if (MyGuild != null && MyGuild.MyArsenal != null)
            //                    {
            //                        MyGuild.MyArsenal.Remove(Role.Instance.Guild.Arsenal.GetArsenalPosition(item.ITEM_ID), item.UID);
            //                    }
            //                    item.Inscribed = 0;
            //                }
            //            }
            //            //checkGuildBattlePower;
            //            if (MyGuild != null)
            //                GuildBattlePower = MyGuild.ShareMemberPotency(GuildRank);

            //            //compute status
            //            Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);

            //            //--------------------------------

            //            //add container Item
            //            foreach (var item in ItemsDrop.Values)
            //                Owner.Confiscator.AddItem(Owner, killer.Owner, item, stream);
            //            //-----------
            //        }
            //    }
            //}
            //catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
        public void DropItem(Game.MsgServer.MsgGameItem item, ushort x, ushort y, ServerSockets.Packet stream)
        {
            Game.MsgFloorItem.MsgItem DropItem = new Game.MsgFloorItem.MsgItem(item, x, y, Game.MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, UID, false, Owner.Map);

            if (Owner.Map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, Game.MsgFloorItem.MsgDropID.Visible);
                Owner.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);
            }
        }
        private void CheckPkPoints(Role.Player killer)
        {
            if (killer.OnMyOwnServer == true && OnMyOwnServer == false)
                return;
            if (Map == 3935)
                return;
            if (Map == 1011 && DynamicID != 0)
                return;
            if (killer.PkMode != Flags.PKMode.Jiang)
            {
                if (!Program.FreePkMap.Contains(Map))
                {
                    if (!this.ContainFlag(Game.MsgServer.MsgUpdate.Flags.RedName) && !this.ContainFlag(Game.MsgServer.MsgUpdate.Flags.BlackName))
                    {
                        if (HeavenBlessing > 0)
                        {
                            if (killer.HeavenBlessing > 0)
                            {
                                Owner.LoseDeadExperience(killer.Owner);
                            }
                            else
                            {
                                Owner.SendSysMesage("Your Heaven Blessing takes effect! You lose no EXP!", MsgMessage.ChatMode.System);
                                killer.AddCursed(5 * 60);
                            }
                        }
                        else
                            Owner.LoseDeadExperience(killer.Owner);

                        if (GetPkPkPoints)
                        {
                            if (killer.MyGuild != null)
                            {
                                if (killer.MyGuild.Enemy.ContainsKey(GuildID))
                                {
                                    killer.PKPoints += 3;
                                    if (killer.MyGuild != null && killer.MyGuildMember != null && MyGuild != null && MyGuild.GuildName != killer.MyGuild.GuildName)
                                    {
                                        killer.MyGuildMember.PkDonation += 5;
                                    }
                                    if (Database.Server.MapName.ContainsKey(Map))
                                    {
                                        if (GuildRank >= Flags.GuildMemberRank.Manager)
                                            killer.MyGuild.SendMessajGuild("The (" + killer.GuildRank.ToString() + ")" + killer.Name + " killed (" + GuildRank.ToString() + ")" + Name + " from guild " + MyGuild.GuildName + " in " + Database.Server.MapName[Map] + "", Game.MsgServer.MsgMessage.ChatMode.Guild, Game.MsgServer.MsgMessage.MsgColor.yellow);
                                    }
                                    return;
                                }
                            }
                            if (killer.MyClan != null)
                            {
                                if (killer.MyClan.Enemy.ContainsKey(ClanUID))
                                {
                                    killer.PKPoints += 3;
                                    if (killer.MyGuild != null && killer.MyGuildMember != null && MyGuild != null && MyGuild.GuildName != killer.MyGuild.GuildName)
                                    {
                                        killer.MyGuildMember.PkDonation += 5;
                                    }
                                    return;
                                }
                            }
                            if (killer.Associate.Contain(Role.Instance.Associate.Enemy, UID))
                            {
                                killer.PKPoints += 5;
                                if (killer.MyGuild != null && killer.MyGuildMember != null && MyGuild != null && MyGuild.GuildName != killer.MyGuild.GuildName)
                                {
                                    killer.MyGuildMember.PkDonation += 10;
                                }
                                return;
                            }
                            killer.PKPoints += 5;
                            if (killer.MyGuild != null && killer.MyGuildMember != null && MyGuild != null && MyGuild.GuildName != killer.MyGuild.GuildName)
                            {
                                killer.MyGuildMember.PkDonation += 10;
                            }
                        }
                    }
                }

            }
        }
        public unsafe void Revive(ServerSockets.Packet stream)
        {
            ProtectAttack(5 * 1000);//5 Seconds

            HitPoints = (int)Owner.Status.MaxHitpoints;
            ClearFlags();
            TransformationID = 0;
            XPCount = 0;
            SendUpdate(stream, XPCount, MsgUpdate.DataType.XPCircle);
            Stamina = 150;
            SendUpdate(stream, Stamina, MsgUpdate.DataType.Stamina);
            Send(stream.MapStatusCreate(Map, Map, Owner.Map.TypeStatus));
            View.SendView(GetArray(stream, false), false);
        }
        public Extensions.Time32 PkPointsStamp = new Extensions.Time32();
        public uint BlessTime = 0;
        public Extensions.Time32 CastPrayStamp = new Extensions.Time32();
        public Extensions.Time32 CastPrayActionsStamp = new Extensions.Time32();
        public Game.MsgServer.MsgUpdate.Flags UseXPSpell;
        public void OpenXpSkill(Game.MsgServer.MsgUpdate.Flags flag, int Timer, int StampExec = 0)
        {
            XPCount = 0;
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                SendUpdate(stream, XPCount, Game.MsgServer.MsgUpdate.DataType.XPCircle);
            }
            Game.MsgServer.MsgUpdate.Flags UseSpell = OnXPSkill();
            if (UseSpell == Game.MsgServer.MsgUpdate.Flags.Normal)
            {
                KillCounter = 0;
                UseXPSpell = flag;
                AddFlag(flag, Timer, true, StampExec);
            }
            else
            {
                if (UseSpell != flag)
                {
                    RemoveFlag(UseSpell);
                    UseXPSpell = flag;
                    AddFlag(flag, Timer, true, StampExec);
                }
                else
                {
                    if (flag == MsgUpdate.Flags.Cyclone || flag == MsgUpdate.Flags.Superman)
                        UpdateFlag(flag, Timer, true, 20);
                    else
                        UpdateFlag(flag, Timer, true, 60);
                }
            }
        }
        public Game.MsgServer.MsgUpdate.Flags OnXPSkill()
        {
            if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Cyclone))
                return Game.MsgServer.MsgUpdate.Flags.Cyclone;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Superman))
                return Game.MsgServer.MsgUpdate.Flags.Superman;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.Oblivion))
                return Game.MsgServer.MsgUpdate.Flags.Oblivion;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.FatalStrike))
                return Game.MsgServer.MsgUpdate.Flags.FatalStrike;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.ShurikenVortex))
                return Game.MsgServer.MsgUpdate.Flags.ShurikenVortex;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.ChaintBolt))
                return Game.MsgServer.MsgUpdate.Flags.ChaintBolt;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.BlackbeardsRage))
                return Game.MsgServer.MsgUpdate.Flags.BlackbeardsRage;
            else if (ContainFlag(Game.MsgServer.MsgUpdate.Flags.CannonBarrage))
                return Game.MsgServer.MsgUpdate.Flags.CannonBarrage;
            else
                return Game.MsgServer.MsgUpdate.Flags.Normal;
        }
        public void UpdateXpSkill()
        {
            if (UseXPSpell == Game.MsgServer.MsgUpdate.Flags.Cyclone
                || UseXPSpell == Game.MsgServer.MsgUpdate.Flags.Superman)
            {
                if (ContainFlag(UseXPSpell))
                    UpdateFlag(UseXPSpell, 1, false, 20);
            }
        }
        public unsafe void SendScrennXPSkill(IMapObj obj)
        {
            if (OnXPSkill() != Game.MsgServer.MsgUpdate.Flags.Normal)
            {


                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();

                    InteractQuery action = new InteractQuery()
                    {
                        UID = UID,
                        KilledMonster = true,
                        X = X,
                        Y = Y,
                        AtkType = MsgAttackPacket.AttackID.Death,
                        KillCounter = KillCounter
                    };
                    obj.Send(stream.InteractionCreate(&action));

                }
            }
        }
        uint _KO;
        public uint KillCounter
        {
            get { return _KO; }
            set { _KO = value; }
        }
        ushort _xpc;
        public ushort XPCount
        {
            get { return _xpc; }
            set
            {
                _xpc = value;
            }
        }
        public DateTime DeathStamp;
        public Extensions.Time32 DeadStamp = new Extensions.Time32();
        public ushort Avatar;
        public long WHMoney;
        public bool IsAsasin = false;
        public Game.MsgServer.ClientAchievement Achievement;
        public Extensions.Time32 LastWorldMessaj = new Extensions.Time32();
        public Flags.PKMode PkMode = Flags.PKMode.Capture;
        public Client.GameClient Owner;
        public MapObjectType ObjType { get; set; }
        public RoleView View;
        public Instance.Quests QuestGUI;
        public unsafe void Send(ServerSockets.Packet msg)
        {
            Owner.Send(msg);
        }
        public Player(Client.GameClient _own)
        {
            AllowDynamic = false;
            this.Owner = _own;
            ObjType = MapObjectType.Player;
            View = new RoleView(Owner);
            BitVector = new StatusFlagsBigVector32(32 * 4);//6
            QuestGUI = new Instance.Quests(this);
        }
        public int Day = 0;
        public unsafe uint UID { get; set; }
        public unsafe string Name = "";
        public unsafe string ClanName = "";
        string _spouse = "None";
        public string Spouse
        {
            get
            {
                return _spouse;
            }
            set { _spouse = value; }
        }
        public ushort Agility;
        public ushort Vitality;
        public ushort Spirit;
        public ushort Strength;
        public ushort Atributes;
        byte _class;
        public unsafe byte Class
        {
            get { return _class; }
            set
            {
                _class = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, value, Game.MsgServer.MsgUpdate.DataType.Class);
                    }
                    if (MyGuildMember != null)
                        MyGuildMember.Class = value;
                }
            }
        }
        public byte FirstRebornLevel;
        public byte SecoundeRebornLevel;
        public unsafe byte FirstClass;
        public unsafe byte SecondClass;
        ushort _level;
        public unsafe ushort Level
        {
            get { return _level; }
            set
            {
                _level = value;
                if (_level >= 140)
                {
                    _level = 140;
                    Experience = 0;
                }
            }
        }
        public unsafe byte Reborn;
        uint _Money;
        public uint Money
        {
            get
            {
                return _Money;
            }
            set
            {
                if (value > 2000000000)
                {
                    uint dif = value - 2000000000;
                    WHMoney += dif;
                    value -= dif;
                    if (Owner.PokerPlayer != null)
                        Owner.PokerPlayer.CurrentMoney = value;
                    this.Owner.SendSysMesage(dif + " Gold has transfare to your warehouse you can't hold more than 2,000,000,000 into your inventory.", MsgMessage.ChatMode.Monster, MsgMessage.MsgColor.yellow);
                }
                _Money = value;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, value, MsgUpdate.DataType.Money);
                }
            }
        }
        uint _cps;
        public uint ConquerPoints
        {
            get { return _cps; }
            set
            {
                if (Owner.FullLoading)
                {
                    if (value > _cps)
                    {
                        uint get_cps = value - _cps;
                        if (get_cps > 59)
                        {
                            string logs = "[CallStack]" + Name + " get " + get_cps + " he have " + _cps + "";
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                    }
                    else
                    {
                        uint lost_cps = _cps - value;
                        if (lost_cps > 59)
                        {
                            string logs = "[CallStack]" + Name + " lost " + lost_cps + " he have " + _cps + "";
                            Database.ServerDatabase.LoginQueue.Enqueue(logs);
                        }
                    }
                }
                _cps = value;
                if (Owner.FullLoading)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                        stream = packet.Append(stream, MsgUpdate.DataType.ConquerPoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }
        int _bountCps;
        public int BoundConquerPoints
        {
            get { return _bountCps; }
            set
            {
                _bountCps = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                        stream = packet.Append(stream, MsgUpdate.DataType.BoundConquerPoints, value);
                        stream = packet.GetArray(stream);
                        Owner.Send(stream);
                    }
                }
            }
        }
        public ulong Experience;
        public uint VirtutePoints;
        int _minhitpoints;
        public unsafe int HitPoints
        {
            get
            {
                if (UnlimitedArenaRooms.Maps.ContainsValue(DynamicID))
                    return (int)Owner.Status.MaxHitpoints;
                return _minhitpoints;
            }
            set
            {
                if (value > 0) DeadState = false;
                else DeadState = true;
                _minhitpoints = value;
                if (Owner.Team != null)
                {
                    var TeamMember = Owner.Team.GetMember(UID);
                    if (TeamMember != null)
                    {
                        TeamMember.Info.MaxHitpoints = (ushort)Owner.Status.MaxHitpoints;
                        TeamMember.Info.MinMHitpoints = (ushort)value;
                        Owner.Team.SendTeamInfo(TeamMember);
                    }
                }
                if (Owner.FullLoading)
                {
                    SendUpdateHP();
                }
            }
        }
        ushort _mana;
        public unsafe ushort Mana
        {
            get { return _mana; }
            set
            {
                _mana = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, value, Game.MsgServer.MsgUpdate.DataType.Mana);
                    }
                }
            }
        }
        ushort _pkpoints;
        public ushort PKPoints
        {
            get { return _pkpoints; }
            set
            {
                _pkpoints = value;
                if (PKPoints > 299)
                {
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.RedName);
                    AddFlag(Game.MsgServer.MsgUpdate.Flags.BlackName, StatusFlagsBigVector32.PermanentFlag, false, 6 * 60);
                }
                else if (PKPoints > 29)
                {
                    AddFlag(Game.MsgServer.MsgUpdate.Flags.RedName, StatusFlagsBigVector32.PermanentFlag, false, 6 * 60);
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.BlackName);
                }
                else if (PKPoints < 30)
                {
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.RedName);
                    RemoveFlag(Game.MsgServer.MsgUpdate.Flags.BlackName);
                }
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SendUpdate(stream, PKPoints, Game.MsgServer.MsgUpdate.DataType.PKPoints);
                }
            }
        }
        public unsafe uint QuizPoints;
        public unsafe ushort Enilghten;
        public DateTime FreeVIP = new DateTime();
        DateTime _ExpireVip;
        public DateTime ExpireVip
        {
            get
            {
                var List = Database.ShareVIP.SharedPoll.GetValues().Where(p => p.ShareUID == UID).ToList();
                if (List.Count > 0)
                {
                    var client = List.FirstOrDefault();
                    return client.ShareEnds;
                }
                return _ExpireVip;
            }
            set
            {
                _ExpireVip = value;
            }
        }
        byte _viplevel;
        public byte VipLevel
        {
            get
            {
                //var List = Database.ShareVIP.SharedPoll.GetValues().Where(p => p.ShareUID == UID).ToList();
                //if (List.Count > 0)
                //{
                //    var client = List.FirstOrDefault();
                //    return client.ShareLevel;
                //}
                return _viplevel;
            }
            set
            {
                if (value == 6 && !Titles.Contains(11))
                {
                    AddTitle(11, true);
                    if (Owner != null)
                        Owner.SendSysMesage("You got your VIP title!");
                }
                _viplevel = value;
            }
        }
        public ushort EnlightenReceive;
        ushort face;
        public unsafe ushort Face
        {
            get { return face; }
            set
            {
                face = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh);
                    }
                }
            }
        }
        public byte HairColor
        {
            get
            {
                return (byte)(Hair / 100);
            }
            set
            {
                Hair = (ushort)((value * 100) + (Hair % 100));
            }
        }
        public unsafe ushort Hair;
        public uint PDinamycID { get; set; }
        public uint DynamicID { get; set; }
        uint _mmmap;
        public uint Map
        {
            get { return _mmmap; }
            set { _mmmap = value; }
        }
        ushort xx, yy;
        public ushort dummyX = 0, dummyY = 0;
        public ushort dummyX2 = 0, dummyY2 = 0;
        public byte dummies = 0;
        public unsafe ushort X
        {
            get { return xx; }
            set { Px = X; xx = value; }
        }
        public unsafe ushort Y
        {
            get { return yy; }
            set { Py = Y; yy = value; }
        }
        public void ClearPreviouseCoord()
        {
            Px = 0;
            Py = 0;
        }
        public ushort Px;
        public ushort Py;
        public ushort PMapX;
        public ushort PMapY;
        public uint PMap;
        public short GetMyDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(X, Y, X2, Y2);
        }
        public short OldGetDistance(ushort X2, ushort Y2)
        {
            return Core.GetDistance(Px, Py, X2, Y2);
        }
        public bool InView(ushort X2, ushort Y2, byte distance)
        {
            //      Console.WriteLine(Name + " " + OldGetDistance(X2, Y2) + " " + GetMyDistance(X2, Y2));
            return ((OldGetDistance(X2, Y2) > distance) && GetMyDistance(X2, Y2) <= distance);
        }
        public unsafe Flags.ConquerAngle Angle = Flags.ConquerAngle.East;
        public unsafe Flags.ConquerAction Action = Flags.ConquerAction.None;
        public byte ExpBallUsed = 0;
        public byte BDExp = 0;
        public DateTime JoinOnflineTG = new DateTime();
        public Extensions.Time32 OnlineTrainingTime = new Extensions.Time32();
        public Extensions.Time32 ReceivePointsOnlineTraining = new Extensions.Time32();
        public Extensions.Time32 HeavenBlessTime = new Extensions.Time32();
        public int HeavenBlessing = 0;
        public uint OnlineTrainingPoints = 0;
        public uint HuntingBlessing = 0;
        public uint DExpTime = 0;
        public uint RateExp = 1;
        public uint ExpProtection = 0;
        public unsafe void CreateExtraExpPacket(ServerSockets.Packet stream)
        {
            Game.MsgServer.MsgUpdate update = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = update.Append(stream, Game.MsgServer.MsgUpdate.DataType.DoubleExpTimer, new uint[4] { DExpTime, 0, (uint)(RateExp * 100), 0 });
            stream = update.GetArray(stream);
            Owner.Send(stream);
        }
        public void AddHeavenBlessing(ServerSockets.Packet stream, int Time)
        {
            if (!ContainFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing))
                HeavenBlessTime = Extensions.Time32.Now;
            if (Time > 60 * 60 * 24)
                Owner.SendSysMesage("You`ve received " + Time / (60 * 60 * 24) + " days` blessing time.", Game.MsgServer.MsgMessage.ChatMode.System);
            else
            {
                Owner.SendSysMesage("You`ve received " + (Time / 60) / 60 + " hours` blessing time.", Game.MsgServer.MsgMessage.ChatMode.System);
            }
            bool None = HeavenBlessing == 0;
            HeavenBlessTime = HeavenBlessTime.AddSeconds(Time);

            HeavenBlessing += Time;
            CreateHeavenBlessPacket(stream, None);

            if (MyMentor != null)
            {
                MyMentor.Mentor_Blessing += (uint)(Time / 10000);
                Role.Instance.Associate.Member mee;
                if (MyMentor.Associat.ContainsKey(Role.Instance.Associate.Apprentice))
                {
                    if (MyMentor.Associat[Role.Instance.Associate.Apprentice].TryGetValue(UID, out mee))
                    {
                        mee.Blessing += (uint)(Time / 10000);
                    }
                }
            }

        }
        public void CreateHeavenBlessPacket(ServerSockets.Packet stream, bool ResetOnlineTraining)
        {
            if (HeavenBlessing > 0)
            {
                if (ResetOnlineTraining)
                {
                    ReceivePointsOnlineTraining = Extensions.Time32.Now.AddMinutes(1);
                    OnlineTrainingTime = Extensions.Time32.Now.AddMinutes(10);
                }
                AddFlag(Game.MsgServer.MsgUpdate.Flags.HeavenBlessing, Role.StatusFlagsBigVector32.PermanentFlag, false);
                SendUpdate(stream, HeavenBlessing, Game.MsgServer.MsgUpdate.DataType.HeavensBlessing, false);

                SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.Show, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                if (Map == 601 || Map == 1039)
                    SendUpdate(stream, Game.MsgServer.MsgUpdate.OnlineTraining.InTraining, Game.MsgServer.MsgUpdate.DataType.OnlineTraining);
                SendString(stream, Game.MsgServer.MsgStringPacket.StringID.Effect, true, new string[1] { "bless" });
            }
        }
        public byte GetGender
        {
            get
            {
                if (Body % 10 >= 3)
                    return 0;
                else
                    return 1;
            }
        }
        ushort body;
        public unsafe ushort Body
        {
            get { return body; }
            set
            {
                body = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh, true);
                    }
                }
            }
        }
        private ushort _transformationid;
        public unsafe ushort TransformationID
        {
            get
            {
                return _transformationid;
            }
            set
            {
                _transformationid = value;
                if (Owner.FullLoading)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        SendUpdate(stream, Mesh, Game.MsgServer.MsgUpdate.DataType.Mesh, true);
                    }
                }
            }
        }
        public bool Alive { get { return HitPoints > 0; } }
        public unsafe uint Mesh
        {
            get
            {
                //  if (_mesh != 0)
                //    return _mesh;
                //2471671003                     10000000
                return (uint)(TransformationID * 10000000 + Face * 10000 + Body);
            }
            //set { _mesh = value; }
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, long Value, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, Value);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
            {
                View.SendView(stream, false);
            }
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.Flags Flag, uint Time, uint Dmg, uint Level, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, (byte)Flag, Time, Dmg, Level);
            stream = packet.GetArray(stream);
            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(ServerSockets.Packet stream, Game.MsgServer.MsgUpdate.DataType datatype, uint Time, uint Dmg, uint Level, bool scren = false)
        {
            Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
            stream = packet.Append(stream, datatype, (byte)0, Time, Dmg, Level);
            stream = packet.GetArray(stream);

            Owner.Send(stream);
            if (scren)
                View.SendView(stream, false);
        }
        public unsafe void SendUpdate(uint[] Value, Game.MsgServer.MsgUpdate.DataType datatype, bool scren = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Owner.Send(stream);
                if (scren)
                    View.SendView(stream, false);
            }
        }
        public bool ShowGemEffects = true;
        public unsafe void UpdateVip(ServerSockets.Packet stream)
        {
            SendUpdate(stream, VipLevel, MsgUpdate.DataType.VIPLevel, false);
            if (VipLevel > 0)
                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.FullVip));
            else
                Owner.Send(stream.VipStatusCreate(MsgVipStatus.VipFlags.None));
        }
        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, bool SendScreen, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;
            packet.Strings = args;

            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true);
            else
                Owner.Send(stream.StringPacketCreate(packet));
        }
        public unsafe void SendGemString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, bool SendScreen, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;
            packet.Strings = args;

            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true, true, ShowGemEffects);
            else
                if (ShowGemEffects)
                Owner.Send(stream.StringPacketCreate(packet));
        }
        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, uint _uid, bool SendScreen, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = _uid;
            packet.Strings = args;

            if (SendScreen)
                View.SendView(stream.StringPacketCreate(packet), true);
            else
                Owner.Send(stream.StringPacketCreate(packet));
        }
        public uint HeadId = 0;
        public uint GarmentId = 0;
        public uint ArmorId = 0;
        public uint LeftWeaponId = 0;
        public uint RightWeaponId = 0;
        public uint LeftWeaponAccessoryId = 0;
        public uint RightWeaponAccessoryId = 0;
        public uint SteedId = 0;
        public uint MountArmorId = 0;
        public ushort ColorArmor = 0;
        public ushort ColorShield = 0;
        public ushort ColorHelment = 0;
        public uint SteedPlus = 0;
        public uint SteedColor = 0;
        public uint HeadSoul = 0;
        public uint ArmorSoul = 0;
        public uint LeftWeapsonSoul = 0;
        public uint RightWeapsonSoul = 0;
        public uint RealUID = 0;
        public void AddMapEffect(ServerSockets.Packet stream, ushort x, ushort y, params string[] effect)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = MsgStringPacket.StringID.LocationEffect;
            packet.X = x;
            packet.Y = y;
            packet.Strings = effect;
            View.SendView(stream.StringPacketCreate(packet), true);
        }
        public void ClearItemsSpawn()
        {
            HeadId = GarmentId = ArmorId = LeftWeaponId = RightWeaponId = LeftWeaponAccessoryId = RightWeaponAccessoryId = SteedId = MountArmorId = 0;
            ColorArmor = ColorShield = ColorHelment = 0;
            SteedPlus = SteedColor = 0;
            HeadSoul = ArmorSoul = LeftWeapsonSoul = RightWeapsonSoul = 0;
        }
        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool WindowsView)
        {
            stream.InitWriter();
            stream.Write(Mesh);
            stream.Write(UID);
            stream.Write(GuildID);
            if (Program.ServerConfig.IsInterServer == false && Owner.OnInterServer == false)
                stream.Write((ushort)GuildRank);
            else
                stream.ZeroFill(2);
            stream.ZeroFill(4);
            for (int x = 0; x < BitVector.bits.Length; x++)
                stream.Write((uint)BitVector.bits[x]);//38
            stream.Write((ushort)AparenceType);//apparence type
            stream.Write(HeadId);
            if ((Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == COServer.Game.MsgTournaments.TournamentType.FreezeWar
                || Game.MsgTournaments.MsgSchedules.CurrentTournament.Type == COServer.Game.MsgTournaments.TournamentType.KillTheCaptain)
                && Game.MsgTournaments.MsgSchedules.CurrentTournament.InTournament(this.Owner))
            {
                if (FreezeTeamType == Game.MsgTournaments.MsgFreezeWar.Team.TeamType.Blue || Owner.TeamKillTheCaptain == KillTheCaptainTeams.Blue)
                    stream.Write(181825);
                else
                    stream.Write(181625);
            }
            else
                stream.Write(GarmentId);
            stream.Write(ArmorId);
            stream.Write(LeftWeaponId);
            stream.Write(RightWeaponId);
            stream.Write(LeftWeaponAccessoryId);
            stream.Write(RightWeaponAccessoryId);
            stream.Write(SteedId);
            stream.Write(MountArmorId);
            stream.Write((uint)0);
            stream.Write(HitPoints);//80
            stream.Write(Hair);//84
            stream.Write(X);//86
            stream.Write(Y);
            stream.Write((byte)Angle);
            stream.Write((ushort)Action);
            stream.Write((byte)0);//padding?
            stream.Write(0);
            stream.Write(Reborn);
            stream.Write(Level);
            stream.Write((byte)(WindowsView ? 1 : 0));
            stream.Write(Away);//away
            stream.Write(ExtraBattlePower);
            stream.Write((ushort)0);
            stream.Write(0);
            stream.Write((ushort)0);
            stream.Write((FlowerRank + 10000));
            stream.Write((uint)NobilityRank);
            stream.Write(ColorArmor);
            stream.Write(ColorShield);
            stream.Write(ColorHelment);
            stream.Write(QuizPoints);//quiz points
            stream.Write(SteedPlus);
            stream.Write((ushort)0);
            stream.Write(SteedColor);
            stream.Write((byte)Enilghten);
            stream.Write((ushort)0);
            stream.Write(0);
            stream.Write(0);
            stream.Write((byte)0);
            stream.Write(ClanUID);
            stream.Write((uint)ClanRank);
            stream.Write((uint)0);//unknow
            stream.Write((ushort)MyTitle);
            stream.Write((uint)0);
            stream.Write((uint)0);
            stream.Write((uint)0);
            stream.Write((byte)0);
            stream.Write(HeadSoul);
            stream.Write(ArmorSoul);
            stream.Write(LeftWeapsonSoul);
            stream.Write(RightWeapsonSoul);
            stream.Write(0);
            stream.Write(0);
            stream.ZeroFill(1);
            stream.Write((ushort)FirstClass);
            stream.Write((ushort)SecondClass);
            stream.Write((ushort)Class);

            if (Owner.Team != null)
            {
                stream.Write((uint)Owner.Team.UID);
            }
            else
                stream.Write(0);
            stream.ZeroFill(1);
            //  stream.Write((byte)ServerID);
            if (OnMyOwnServer == false)
            {
                stream.Write(Name, string.Empty, ClanName, string.Empty, string.Empty, MyGuild != null ? MyGuild.GuildName : string.Empty, string.Empty);
            }
            else
            {
                stream.Write(Name, string.Empty, ClanName, string.Empty);
            }
            stream.Finalize(Game.GamePackets.SpawnPlayer);

            return stream;

        }
        public uint GetShareBattlePowers(uint target_battlepower)
        {
            return (uint)Database.TutorInfo.ShareBattle(this.Owner, (int)target_battlepower);
        }
        public uint PokerTableID { get; set; }
        public byte PokerSeat { get; set; }
        public string Mac = "";
        public byte MysteryFruit = 0;
        public string NewUser = "";
        public Game.MsgTournaments.MsgFreezeWar.Team.TeamType FreezeTeamType;
        private int SpecialGarment;
        public DateTime LastMove;
        public DateTime LastSuccessCaptcha = DateTime.Now;
        public bool LootExpBall = true, LootDragonBall = true;
        private ushort _stamina;
        public int TotalHits, Hits, Chains, MaxChains;
        public int NextCaptcha = 5;
        public int KillTheCaptain;
        public uint OnlinePoints;
        public uint PVPPoints;
        public uint PVEPoints;
        public DateTime BossTeleCooldown;
        public DateTime SummonGuild;

        public void RemoveSpecialGarment(ServerSockets.Packet stream)
        {
            SpecialGarment = 0;
            GarmentId = 0;
            //   Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Unequip, uint.MaxValue - 1, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));

            MsgGameItem item;
            if (Owner.Equipment.TryGetEquip(Flags.ConquerItem.Garment, out item))
            {
                Owner.Send(stream.ItemUsageCreate(MsgItemUsuagePacket.ItemUsuageID.Equip, item.UID, (ushort)Flags.ConquerItem.Garment, 0, 0, 0, 0));
                item.Mode = Flags.ItemMode.AddItem;
                item.Send(Owner, stream);
            }
            Owner.Equipment.QueryEquipment(Owner.Equipment.Alternante);
        }

        internal string AgatesString()
        {
            string Agates = "";
            foreach (var item in Owner.Inventory.ClientItems.Values.Where(e => e.ITEM_ID == 720828))
            {
                Agates += item.UID + "#";
                foreach (string coord in item.Agate_map.Values)
                    Agates += coord + "#";
                Agates += "$";

            }
            foreach (var wh in Owner.Warehouse.ClientItems.Values)
            {
                foreach (var item in wh.Values.Where(e => e.ITEM_ID == 720828))
                {
                    Agates += item.UID + "#";
                    foreach (string coord in item.Agate_map.Values)
                        Agates += coord + "#";
                    Agates += "$";
                }
            }
            return Agates;
        }

        internal void LoadAgates(string str)
        {
            if (str != "")
            {
                string[] allAgates = str.Split(new string[] { "$" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in allAgates)
                {
                    var agate = item.Split('#');
                    uint Uid = uint.Parse(agate[0]);
                    string[] agate_data = item.Replace(Uid + "#", "").Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    MsgGameItem itemc;
                    if (Owner.Inventory.ClientItems.TryGetValue(Uid, out itemc))
                    {
                        int key = 0;
                        foreach (var agate_item in agate_data)
                        {
                            itemc.Agate_map.Add((uint)key++, agate_item);
                        }
                    }
                    foreach (var wh in Owner.Warehouse.ClientItems.Values)
                    {
                        if (wh.TryGetValue(Uid, out itemc))
                        {
                            int key = 0;
                            foreach (var agate_item in agate_data)
                            {
                                itemc.Agate_map.Add((uint)key++, agate_item);
                            }
                        }
                    }
                }
            }
        }
        public int BossPoints = 0;
        internal bool SendAllies = true;
        internal int OblivionMobs;
        internal bool WhirlWind;
        internal bool DeadState = true;
        internal DateTime ShieldBlockEnd = DateTime.Now;
        internal DateTime JumpingStamp, PreviousJump;
        internal int FiveNOut = 0;
        internal int SkyFight = 0;
        internal bool BlockMovementCo = false;
        internal DateTime BlockMovement;
        internal uint ArenaCPS;
        internal int DragonPills = 0;
        public uint JailerUID = 0;
        internal DateTime KickedOffSteed = DateTime.Now;
        internal DateTime RemovedShackle;
        internal int FreezePts;

        internal void SolveCaptcha()
        {
            Owner.MobsKilled = 0;
            WaitingKillCaptcha = false;
            KillCountCaptcha = "";
            LastSuccessCaptcha = DateTime.Now;
            NextCaptcha = Role.Core.Random.Next(15, 30);
        }
    }
}
