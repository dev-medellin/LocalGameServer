using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace COServer
{
    public class Discord
    {
        string API = "https://discordapp.com/api/webhooks/594950689407631403/pJ7pZBzSXtsd4oLQ8bUkdL8NFGcpiAFApehFd7n5hUBi8mNq6k-ny8AEI4bPKdLrn_2A";
        Queue<string> Msgs;
        Uri webhook;

        public Discord(string API)
        {
            this.API = API;
            Msgs = new Queue<string>();
            webhook = new Uri(API);
            Console.WriteLine("Discord Server Ready.");
            var thread = new Thread(Dequeue);
            thread.Start();
        }
        private void Dequeue()
        {
            postToDiscord("Discord Thread started");
            while (true)
            {
                try
                {
                    while (Msgs.Count != 0)
                    {
                        var msg = Msgs.Dequeue();
                        postToDiscord(msg);
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        string lastmsg = "";
        public void Enqueue(string str)
        {
            Msgs.Enqueue(/*$"[{DateTime.Now.ToString()}]: */$"{str}");
        }
        private void postToDiscord(string Text)
        {
            if (Program.TestServer)
                return;
            if (lastmsg == Text)
                return;
            lastmsg = Text;
            Text = Text.Replace("@everyone", "");
            HttpClient client = new HttpClient();

            Dictionary<string, string> discordToPost = new Dictionary<string, string>();
            discordToPost.Add("content", Text);

            var content = new FormUrlEncodedContent(discordToPost);

            var res = client.PostAsync(webhook, content).Result;
            //If you want to check result value
            if (res.IsSuccessStatusCode)
            {
                //Console.WriteLine($"ent {Text}!");
            }
        }
    }
}
