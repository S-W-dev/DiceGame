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
    public partial class GameMain : Form {
        public static WebSocket server = null;
        public static bool connected = false;
        public static Player player;
        public static UpdateMessage game;

        int currentBet = 100, currentChoice = 0;

        Thread conn;
        public GameMain() {
            InitializeComponent();

            TopMost = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Maximized;
            MaximizeBox = false;

            conn = new Thread(new ServerComponents(this).connection);
            conn.Start();
            while(!connected) {
                Thread.Sleep(1);
            }
            //ServerComponents.SendMessage(server, "Hello");
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

            while (game == null) {
                Thread.Sleep(1);
            }
            //UpdatePlayerDisplay();

            tableTable.ColumnStyles[1].Width = 800f / 1588f * tableTable.Width;
            tableTable.RowStyles[1].Height = 400f / 841f * tableTable.Height;
        }

        private void UpdatePlayerDisplay() {
            var numOfPlayers = game.players.Length;
            //numOfPlayers = 3;
            if (numOfPlayers < 5) {
                TableLayoutPanel playerTable = new TableLayoutPanel();
                playerTable.Height = 125;
                playerTable.Dock = DockStyle.Top;

                playerTable.BackColor = Color.Transparent;

                playerTable.RowCount = 1;
                playerTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                playerTable.ColumnCount = numOfPlayers;
                for (var i = 0; i < numOfPlayers; i++) {
                    playerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / (numOfPlayers)));
                }

                for (var i = 0; i < numOfPlayers; i++) {
                    var label = new Label();
                    label.Dock = DockStyle.Fill;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.Text = game.players[i].name;
                    label.Font = new Font("Arial", 25);
                    playerTable.Controls.Add(label);
                }

                Controls.Add(playerTable);
            } else {
                TableLayoutPanel playerTable1 = new TableLayoutPanel();
                playerTable1.Height = 125;
                playerTable1.Dock = DockStyle.Top;

                playerTable1.BackColor = Color.Transparent;

                playerTable1.RowCount = 1;
                playerTable1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                playerTable1.ColumnCount = 4;
                for (var i = 0; i < 4; i++) {
                    playerTable1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
                }

                TableLayoutPanel playerTable2 = new TableLayoutPanel();
                playerTable2.Height = 125;
                playerTable2.Dock = DockStyle.Top;

                playerTable2.BackColor = Color.Transparent;

                playerTable2.RowCount = 1;
                playerTable2.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                playerTable2.ColumnCount = numOfPlayers - 4;
                for (var i = 0; i < numOfPlayers - 4; i++) {
                    playerTable2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / (numOfPlayers - 4)));
                }

                for (var i = 0; i < numOfPlayers; i++) {
                    if (i < 4) {
                        var label = new Label();
                        label.Dock = DockStyle.Fill;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Text = game.players[i].name;
                        label.Font = new Font("Arial", 25);
                        playerTable1.Controls.Add(label);
                    } else {
                        var label = new Label();
                        label.Dock = DockStyle.Fill;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Text = game.players[i].name;
                        label.Font = new Font("Arial", 25);
                        playerTable2.Controls.Add(label);
                    }
                }

                Controls.Add(playerTable2);
                Controls.Add(playerTable1);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            server.Close();
            conn.Abort();
            Application.Exit();
        }

        private void button8_Click(object sender, EventArgs e) {
            currentBet -= 100;
            bet.Text = "$" + currentBet.ToString();
        }

        private void button1_Click(object sender, EventArgs e) {
            currentChoice = 1;
        }

        private void button2_Click(object sender, EventArgs e) {
            currentChoice = 2;
        }

        private void button3_Click(object sender, EventArgs e) {
            currentChoice = 3;
        }

        private void button4_Click(object sender, EventArgs e) {
            currentChoice = 4;
        }

        private void button5_Click(object sender, EventArgs e) {
            currentChoice = 5;
        }

        private void button6_Click(object sender, EventArgs e) {
            currentChoice = 6;
        }

        private void submit_Click(object sender, EventArgs e) {
            ServerComponents.SendMessage(server, JsonConvert.SerializeObject(ClientMessage.GenerateClientMessage(ClientMessage.bet, currentChoice, currentBet)));
        }

        private void plus_Click(object sender, EventArgs e) {
            currentBet += 100;
            bet.Text = "$" + currentBet.ToString();
        }

        private void GameMain_SizeChanged(object sender, EventArgs e) {
            tableTable.ColumnStyles[1].Width = 800f / 1588f * tableTable.Width;
            tableTable.RowStyles[1].Height = 400f / 841f * tableTable.Height;
        }

        class ServerComponents {
            private GameMain gamemain;
            public ServerComponents(GameMain _gamemain) {
                gamemain = _gamemain;
            }
            public void connection() {
                using (var ws = new WebSocket("ws://concretegames.net:667/socket/?EIO=2&transport=websocket")) {
                    GameMain.server = ws;
                    Console.WriteLine(ws);
                    ws.OnMessage += (sender, e) => {
                        //Console.WriteLine("Message: " + e.Data);
                        try {
                            UpdateMessage message = JsonConvert.DeserializeObject<UpdateMessage>(e.Data);

                            //player update
                            //var player = message.player;
                            player = message.player;

                            //Console.WriteLine(GameMain.player.id);
                            //Console.WriteLine(GameMain.player.name);
                            //Console.WriteLine(GameMain.player.status);
                            //Console.WriteLine(GameMain.player.money);

                            //game update
                            //var game = message;
                            game = message;

                            gamemain.Invoke((Action)delegate {
                                gamemain.money.Text = "You have: $" + player.money;
                            });

                            Console.WriteLine("Roll: " + game.roll);
                            //Console.WriteLine(GameMain.game.players[0].status);

                        } catch (Exception x) {
                            if (e.Data == "connected") SendMessage(ws, "connected");
                            else Console.WriteLine(x);
                        }
                    };
                    ws.OnError += (sender, e) =>
                        Console.WriteLine("Error: " + e.Message);
                    ws.OnOpen += (sender, e) =>
                        connected = true;
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

    class ClientMessage {
        public static string bet = "bet";

        public static ClientMessageObject GenerateClientMessage(string type, int choice, int bet) {
            switch (type) {
                case "bet":
                    ClientMessageObject obj = new ClientMessageObject();
                    obj.type = type;
                    obj.choice = choice;
                    obj.bet = bet >= 100 ? bet : 100;
                    return obj;
                default:
                    return null;
            }
        }

        public class ClientMessageObject {
            public string type;
            public int choice;
            public int bet;
        }
    }

    public class UpdateMessage {
        public string type;
        public Player player;

        public int roll;
        public Player[] players;
    }

    public class Player {
        public string name;
        public string status;
        public int id;
        public int money;
    }
}
