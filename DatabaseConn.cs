using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace DiceGame {
    public class DatabaseConn {

        static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + System.Windows.Forms.Application.StartupPath + "\\Database.mdf;Integrated Security=True";

        string uname, upass;

        public DatabaseConn(string _uname, string _upass) {
            uname = _uname;
            upass = SHA256Hash(_upass);
            if (_upass != "") {
                setLoginString(upass);
            }
        }

        public static void Init() {
            SqlConnection myConn = new SqlConnection(connectionString);
            string str = "CREATE TABLE [dbo].[player_data] (" +
                        "[Id]           INT          NOT NULL, " +
                        "[username]     VARCHAR (12) DEFAULT ('Player') NOT NULL, " +
                        "[money]        NCHAR (10)   DEFAULT ((10000)) NOT NULL, " +
                        "[image]        TEXT         DEFAULT ('https://concretegames.net/uploads/DefaultUser.png') NOT NULL, " +
                        "[login_string] TEXT         NULL, " +
                        "PRIMARY KEY CLUSTERED ([Id] ASC) ";

            SqlCommand myCommand = new SqlCommand(str, myConn);
            try {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                System.Windows.Forms.MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (System.Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } finally {
                if (myConn.State == ConnectionState.Open) {
                    myConn.Close();
                }
            }

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

        public static void setImage(string _image) {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                con.Open();
                var sql = @"UPDATE player_data SET image = '" + _image + "' WHERE Id = 0";
                using (var cmd = new SqlCommand(sql, con)) {
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string getImage() {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                con.Open();
                var sql = @"SELECT * FROM player_data";
                using (var cmd = new SqlCommand(sql, con)) {
                    using (var rdr = cmd.ExecuteReader()) {
                        while (rdr.Read()) {
                            try {
                                var x = Convert.ToString(rdr["image"]);
                                if (x == "null") {
                                    x = null;
                                }
                                return x;
                            } catch (Exception x) { }
                        }
                        return null;
                    }
                }
            }
        }

        public async void DownloadAndSet() {
            LoginData data = new LoginData();
            uname = getUsername();
            data.username = uname;
            var getData = await Data("download", getLoginString());
            data.money = getData.money;
            getData = await Data("getImage", getLoginString());
            data.image = getData.image;
            setMoney(data.money);
            setImage(data.image);
        }

        public async Task<LoginData> Download() {
            LoginData data = new LoginData();
            data.username = uname;
            //Console.WriteLine("before old get data");
            var getData = await Data("download", upass);
            //Console.WriteLine("after old get data");
            data.money = getData.money;
            //Console.WriteLine("before new get data");
            getData = await Data("getImage", upass);
            //Console.WriteLine("after new get data");
            data.image = getData.image;
            Console.Write(data.image);
            return data;
        }

        public async void Upload() {
            try {
                uname = getUsername();
                var d = new data();
                d.money = getMoney();
                d.image = "";
                await Data("upload", getLoginString(), JsonConvert.SerializeObject(d));
            } catch (Exception) { }
        }

        private string getLoginString() {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                con.Open();
                var sql = @"SELECT * FROM player_data";
                using (var cmd = new SqlCommand(sql, con)) {
                    using (var rdr = cmd.ExecuteReader()) {
                        while (rdr.Read()) {
                            try {
                                var x = Convert.ToString(rdr["login_string"]);
                                return x;
                            } catch (Exception x) { }
                        }
                        return null;
                    }
                }
            }
        }

        private void setLoginString(string _login_string) {
            using (SqlConnection con = new SqlConnection(connectionString)) {
                con.Open();
                var sql = @"UPDATE player_data SET login_string = '" + _login_string + "' WHERE Id = 0";
                using (var cmd = new SqlCommand(sql, con)) {
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Login string: " + getLoginString());
        }

        //public void Upload(int _money) {
        //    data myData = new data();
        //    myData.money = _money;
        //    Data("upload", upass, JsonConvert.SerializeObject(myData));
        //}

        public async Task<data> Data(string operation, string login_string, string data = "null", string server = "https://concretegames.net", string link = "/games/highrollers.php?", string return_value = "highrollers") {
            var req = server + link + "username=" + uname + "&login_string=" + login_string + "&operation=" + operation + "&data=" + data + "&return=" + return_value;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string output = new StreamReader(response.GetResponseStream()).ReadToEnd();

            string message = "";

            if (output.Contains("error")) {

                if (output.Contains("message")) {
                    message = output.Split('m')[1].Split(':')[1].Replace("'", "").Replace("}", "");
                    Console.WriteLine(message);
                } else {
                    message = output.Split('u')[1].Split(':')[1].Replace("'", "").Replace("}", "");
                    Console.WriteLine(message);
                }

                return new data();

            } else if (output == "null" || output == "" || output == null) {
                return new data();
            } else if (output[0] == 'h') {
                var d = new data();
                d.image = output;
                return d;
            } else {
                //Console.WriteLine(output);
                return JsonConvert.DeserializeObject<data>(output);
                //return null;
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
            public int money = 10000;
            public string image = "https://concretegames.net/uploads/DefaultUser.png";
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
