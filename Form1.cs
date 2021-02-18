using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using System.Threading;
using Newtonsoft.Json;

namespace DiceGame {
    public partial class Form1 : Form {
        public static WebSocket server = null;
        public static bool connected = false;
        Thread conn;
        public Form1() {
            InitializeComponent();

            TopMost = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Maximized;
            MaximizeBox = false;

            conn = new Thread(ServerComponents.connection);
            conn.Start();
            while(!connected) {
                Thread.Sleep(1);
            }
            ServerComponents.SendMessage(server, "Hello");
        }

        protected override void WndProc(ref Message message) {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (message.Msg) {
                case WM_SYSCOMMAND:
                    int command = message.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            base.WndProc(ref message);
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            server.Close();
            conn.Abort();
            Application.Exit();
        }
    }

    class ServerComponents {
        public static void connection() {
            using (var ws = new WebSocket("ws://concretegames.net:667/socket/?EIO=2&transport=websocket")) {
                Form1.server = ws;
                Console.WriteLine(ws);
                ws.OnMessage += (sender, e) =>
                    Console.WriteLine("Message: " + e.Data);
                ws.OnError += (sender, e) =>
                    Console.WriteLine("Error: " + e.Message);
                ws.OnOpen += (sender, e) =>
                    Form1.connected = true;
                ws.Connect();
                Console.ReadKey(true);
            }
        }

        public static void SendMessage(WebSocket ws, string message) {
            //ws.Send(JsonConvert.SerializeObject(new { type = "getframe" }));
            ws.Send(message);
        }
    }
}
