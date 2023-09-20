using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace COServer.ServerSockets
{
    public class SocketPoll
    {
        const int SOCKET_PROCESS_INTERVAL = 20, FD_SETSIZE = 2048;

        public static Extensions.MyList<SecuritySocket> ConnectionPoll = new Extensions.MyList<SecuritySocket>();

        private static ServerSocket[] Sockets;
        public SocketPoll(string GroupName, params ServerSocket[] _Sockets)
        {
            Sockets = _Sockets;
            var ThreadItem = new Extensions.ThreadGroup.ThreadItem(SOCKET_PROCESS_INTERVAL, GroupName, CheckUp);
            ThreadItem.Open();
        }
        public static void CheckUp()
        {
            try
            {

                List<Socket> RecSockets = new List<Socket>();

                if (ConnectionPoll.Count > 0 || Sockets.Length > 0)
                {

                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Start();

                    foreach (var socket in Sockets)
                    {
                        if (socket == null || socket.IsAlive == false)
                            continue;

                        RecSockets.Add(socket.GetConnection);
                    }

                    foreach (var socket in ConnectionPoll.GetValues())
                    {

                        socket.CheckUp();

                        if (socket.Alive && socket.Connection.Connected)
                        {
                            RecSockets.Add(socket.Connection);
                        }
                        else
                            socket.Disconnect();
                    }



                    //     int ret = WindowsAPI.ws2_32.SocketSelect(FD_SETSIZE, RecSockets, SendSockets, ErrorSockets, 1000);

                    //   if (ret > 0)
                    //  WindowsAPI.ws2_32.SocketSelect(FD_SETSIZE, RecSockets, SendSockets, ErrorSockets, 0);
                    try
                    {
                        //      Socket.Select(RecSockets, null, null, 0);
                    }
                    catch (Exception e)
                    {
                        Console.SaveException(e);
                        foreach (var socket in ConnectionPoll.GetValues())
                        {
                            if (RecSockets.Contains(socket.Connection))
                            {
                                if (socket.Connection.Connected == false)
                                {
                                    socket.Disconnect();
                                }
                            }
                        }

                    }
                    //  if (RecSockets.Count > 0  )
                    {
                        //  int procesed = RecSockets.Count;
                        foreach (var socket in ConnectionPoll.GetValues())
                        {

                            //   if (RecSockets.Contains(socket.Connection))
                            {
                                //   procesed -= 1;
                                try
                                {
                                    socket.ReceiveBuffer();
                                    socket.HandlerBuffer();

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                    continue;
                                }
                            }
                            try
                            {
                                while (SecuritySocket.TrySend(socket)) ;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                        // if (procesed > 0)
                        {
                            foreach (var socket in Sockets)
                            {
                                if (socket == null)
                                    continue;

                                //    if (RecSockets.Contains(socket.GetConnection))
                                {
                                    socket.Accept();
                                }
                            }
                        }
                    }
                    timer.Stop();
                    if (timer.ElapsedMilliseconds > 30)
                    {
                        //Console.WriteLine("stamp -> " + timer.ElapsedMilliseconds);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
