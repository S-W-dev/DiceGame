using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DiceGame {
    public class DatabaseConn {

        static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\14356\Desktop\Coding\DiceGame\DiceGame\DiceGame\Database.mdf;Integrated Security=True";

        string uname, upass;

        public DatabaseConn(string _uname, string _upass) {
            uname = _uname;
            upass = SHA256Hash(_upass);
        }

        public static void setUsername(string _username) {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                con.Open();
                var sql = @"UPDATE player_data SET username = '" + _username + "' WHERE Id = 0";
                using (var cmd = new SqlCommand(sql, con)) {
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string getUsername() {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                con.Open();
                var sql = @"SELECT * FROM player_data";
                using (var cmd = new SqlCommand(sql, con)) {
                    using (var rdr = cmd.ExecuteReader()) {
                        while (rdr.Read()) {
                            try {
                                return Convert.ToString(rdr["username"]);
                            } catch (Exception x) { }
                        }
                        return null;
                    }
                }
            }
        }

        public static void setMoney(int _money) {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                con.Open();
                var sql = @"UPDATE player_data SET money = " + _money + " WHERE Id = 0";
                using (var cmd = new SqlCommand(sql, con)) {
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int getMoney() {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                con.Open();
                var sql = @"SELECT * FROM player_data";
                using (var cmd = new SqlCommand(sql, con)) {
                    using (var rdr = cmd.ExecuteReader()) {
                        while (rdr.Read()) {
                            try {
                                return Convert.ToInt32(rdr["money"]);
                            } catch (Exception x) { }
                        }
                        return 0;
                    }
                }
            }
        }

        public async Task<LoginData> Download() {
            LoginData data = new LoginData();
            data.username = uname;
            var getData = await Data("download", upass);
            data.money = getData.money;
            return data;
        }

        public void Upload(int _money) {
            data myData = new data();
            myData.money = _money;
            Data("upload", upass, JsonConvert.SerializeObject(myData));
        }

        public async Task<data> Data(string operation, string login_string, string data = "null", string server = "https://concretegames.net", string link = "/games/upload_scores.php?", string return_value = "highrollers") {
            var req = server + link + "username=" + uname + "&login_string=" + login_string + "&operation=" + operation + "&data=" + data + "&return=" + return_value;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string output = new StreamReader(response.GetResponseStream()).ReadToEnd();

            string message = "";

            if(output.Contains("error")) {

                if (output.Contains("message")) {
                    message = output.Split('m')[1].Split(':')[1].Replace("'", "").Replace("}", "");
                    Console.WriteLine(message);
                } else {
                    message = output.Split('u')[1].Split(':')[1].Replace("'", "").Replace("}", "");
                    Console.WriteLine(message);
                }

                return new data();

            } else {
                Console.WriteLine(output);
                return JsonConvert.DeserializeObject<data>(output);
            }

        }

        public static string SHA256Hash(string text) {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            sha256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = sha256.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++) {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        [Serializable]
        public class data {
            public int money;
        }

        [Serializable]
        public class Message {
            public string isError;
            public string message;
            public string error;
            //public string error;
        }

        [Serializable]
        public class Error {
            public string isError;
            public string error;
            //public string error;
        }
    }
}
