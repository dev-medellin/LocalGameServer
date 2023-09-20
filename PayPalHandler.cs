using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COServer
{
    public class PayPalHandler
    {
        public const string ConnectionString = "Server=localhost;username=root;password=callalista;database=auth;";
        public static Dictionary<int, int> getItems(string username)
        {
            Dictionary<int, int> items = new Dictionary<int, int>();// key : item_number and value : count 
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    using (var cmd = new MySqlCommand("select item_number from payments where username=@u and claimed=0"
                        , conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@u", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int r = int.Parse(reader.GetString("item_number"));
                                if (items.ContainsKey(r))
                                    items[r]++;
                                else items.Add(r, 1);
                            }
                        }
                    }

                    using (var cmd = new MySqlCommand("update payments set claimed=1 where username=@u"
                        , conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return items;
        }

        public static void logDonation(string user, string name, string log)
        {
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    using (var cmd = new MySqlCommand("insert into log_payments (username,name,log) values (@user,@name,@log)"
                        , conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@log", log);
                        cmd.ExecuteNonQuery();

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
