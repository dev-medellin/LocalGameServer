using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COServer
{
    public class MapGroupThread
    {
        public const int AI_Buffers = 500,
            AI_Guards = 700,
            JUMP = 800,
            AI_Monsters = 400,
            User_Buffers = 500,
            User_Stamina = 500,
            User_StampXPCount = 3000,
            User_AutoAttack = 100,
            User_CheckSeconds = 1000,
            User_FloorSpell = 300,
            User_CheckItems = 1000;

        public Extensions.ThreadGroup.ThreadItem Thread;
        public Extensions.ThreadGroup.ThreadItem Thread2;
        public MapGroupThread(int interval, string name)
        {
            Thread = new Extensions.ThreadGroup.ThreadItem(interval, name, OnProcess);
            Thread2 = new Extensions.ThreadGroup.ThreadItem(interval, name, OnProcess2);
        }
        public void Start()
        {
            Thread.Open();
            Thread2.Open();
        }
        Role.GameMap _desert;
        public Role.GameMap Desert
        {
            get
            {
                if (_desert == null)
                    _desert = Database.Server.ServerMaps[1000];
                return _desert;
            }
        }
        Role.GameMap _bird;
        public Role.GameMap Bird
        {
            get
            {
                if (_bird == null)
                    _bird = Database.Server.ServerMaps[1000];
                return _bird;
            }
        }

        Role.GameMap _towerofmystery;
        public Role.GameMap TowerofMystery
        {
            get
            {
                if (_towerofmystery == null)
                    _towerofmystery = Database.Server.ServerMaps[3998];
                return _towerofmystery;
            }
        }
        public void OnProcess()
        {
            Extensions.Time32 clock = Extensions.Time32.Now;


            Desert.CheckUpSoldierReamins(clock);
            Bird.CheckUpSoldierReamins(clock);
            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (!user.Fake)
                {
                    user.Player.View.CheckUpMonsters(clock);
                    if (clock > user.BuffersStamp)
                    {
                        Client.PoolProcesses.BuffersCallback(user);
                        user.BuffersStamp.Value = clock.Value + User_Buffers;
                    }
                    if (clock > user.StaminStamp)
                    {
                        Client.PoolProcesses.StaminaCallback(user);
                        user.StaminStamp.Value = clock.Value + User_Stamina;
                    }
                    if (clock > user.XPCountStamp)
                    {
                        Client.PoolProcesses.StampXPCountCallback(user);
                        user.XPCountStamp.Value = clock.Value + User_StampXPCount;
                    }
                    if (clock > user.AttackStamp)
                    {
                        Client.PoolProcesses.AutoAttackCallback(user);
                        user.AttackStamp.Value = clock.Value + User_AutoAttack;
                    }
                    if (clock > user.CheckSecondsStamp)
                    {
                        Client.PoolProcesses.CheckSeconds(user);
                        user.CheckSecondsStamp.Value = clock.Value + User_CheckSeconds;
                    }

                    if (clock > user.CheckItemsView)
                    {
                        Client.PoolProcesses.CheckItems(user);
                        user.CheckItemsView.Value = clock.Value + User_CheckItems;
                    }
                }
            }
        }
        public void OnProcess2()
        {
            Extensions.Time32 clock = Extensions.Time32.Now;
            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (!user.Fake)
                {
                    if (clock > user.JumpStamp)
                    {
                        user.Player.dummyX = user.Player.dummyX2 = user.Player.X;
                        user.Player.dummyY = user.Player.dummyY2 = user.Player.Y;
                        user.JumpStamp.Value = clock.Value + 700;// JUMP;
                    }

                }
            }
        }
    }
}
