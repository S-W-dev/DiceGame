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
using DiceGame;
using System.Windows.Threading;

namespace DiceGame {
    public partial class GameMain : Form {
        public static WebSocket server = null;
        public static bool connected = false;
        public static Player player;
        public static UpdateMessage game;

        int currentBet = 100, currentChoice = 0;

        Thread conn;
        public GameMain(bool join) {
            InitializeComponent();

            TopMost = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            WindowState = FormWindowState.Maximized;
            MaximizeBox = false;

            conn = new Thread(new ServerComponents(this).connection);
            conn.Start();
            while (!connected) {
                Thread.Sleep(1);
            }
            if (join) {
                var message = new ClientMessage.JoinMessage();
                message.room_code = Prompt.ShowDialog("Enter the room code", "Prompt");
                var message1 = JsonConvert.SerializeObject(message);
                Console.WriteLine(message1);
                ServerComponents.SendMessage(server, message1);
            }
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

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            while (game == null) {
                Thread.Sleep(1);
            }
            //UpdatePlayerDisplay();

            tableTable.ColumnStyles[2].Width = 800f / 1588f * MainPanel.Width;
            tableTable.RowStyles[2].Height = 400f / 841f * MainPanel.Height;

            setChoice(1);

            setName(DatabaseConn.getUsername());
            setMoney(DatabaseConn.getMoney());
        }

        //Player[] currentPlayers;
        List<PlayerBox> pBoxes = new List<PlayerBox>();

        private void UpdatePlayers() {
            for (var i = 0; i < pBoxes.Count; i++) {
                pBoxes[i].Location = new Point(i * 100, 0);
                pBoxes[i].setImage();
                pBoxes[i].setMoney(game.players[i].money);
                pBoxes[i].setName(game.players[i].name);
                pBoxes[i].setStatus(game.players[i].status);
            }
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

                base.Controls.Add(playerTable);
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

                base.Controls.Add(playerTable2);
                base.Controls.Add(playerTable1);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            server.Close();
            conn.Abort();
            Application.Exit();
        }

        private void button8_Click(object sender, EventArgs e) {
            if (currentBet > 100) currentBet -= 100;
            bet.Text = "$" + currentBet.ToString();
        }

        private void setChoice(int choice) {
            var i = 1;
            foreach (Label l in new Label[] { choice1, choice2, choice3, choice4, choice5, choice6 }) {
                //Console.WriteLine(l.Name);
                l.Hide();
                if (i == choice) l.Show();
                i++;
            }
            currentChoice = choice;
        }

        private void button1_Click(object sender, EventArgs e) {
            setChoice(1);
        }

        private void button2_Click(object sender, EventArgs e) {
            setChoice(2);
        }

        private void button3_Click(object sender, EventArgs e) {
            setChoice(3);
        }

        private void button4_Click(object sender, EventArgs e) {
            setChoice(4);
        }

        private void button5_Click(object sender, EventArgs e) {
            setChoice(5);
        }

        private void button6_Click(object sender, EventArgs e) {
            setChoice(6);
        }

        int setBet = 0;
        int setC = 0;

        private void submit_Click(object sender, EventArgs e) {
            setBet = currentBet;
            setC = currentChoice;
            var message = new ClientMessage.BetMessage();
            message.bet = setBet >= 100 ? setBet : 100; ;
            message.choice = setC;
            ServerComponents.SendMessage(server, JsonConvert.SerializeObject(message));
        }

        private void setName(string name) {
            var message = new ClientMessage.NameMessage();
            message.name = name;
            ServerComponents.SendMessage(server, JsonConvert.SerializeObject(message));
        }

        private void setMoney(int money) {
            var message = new ClientMessage.MoneyMessage();
            message.money = money;
            ServerComponents.SendMessage(server, JsonConvert.SerializeObject(message));
        }

        private void plus_Click(object sender, EventArgs e) {
            if (currentBet < player.money) currentBet += 100;
            bet.Text = "$" + currentBet.ToString();
        }

        private void GameMain_SizeChanged(object sender, EventArgs e) {
            tableTable.ColumnStyles[1].Width = 800f / 1588f * tableTable.Width;
            tableTable.RowStyles[1].Height = 400f / 841f * tableTable.Height;
        }

        private void tableTable_Paint(object sender, PaintEventArgs e) {

        }

        private void label4_Click(object sender, EventArgs e) {

        }

        class ServerComponents {
            private GameMain gamemain;
            public ServerComponents(GameMain _gamemain) {
                gamemain = _gamemain;
            }

            bool hasPlayerJoined = false, canSetHasPlayerJoined = false;
            public void connection() {
                using (var ws = new WebSocket("ws://concretegames.net:667/socket/?EIO=2&transport=websocket")) {
                    GameMain.server = ws;
                    Console.WriteLine(ws);
                    ws.OnMessage += (sender, e) => {
                        if (e.Data == "connected") {
                            SendMessage(ws, "connected");
                            canSetHasPlayerJoined = true;
                        } else {
                            try {
                                UpdateMessage message = JsonConvert.DeserializeObject<UpdateMessage>(e.Data);

                                /*if (!hasPlayerJoined) {
                                    try {
                                        Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => {
                                            var pb = new PlayerBox();
                                            pb.id = message.socketId;
                                            gamemain.pBoxes.Add(pb);
                                            gamemain.Controls.Add(pb);
                                        }));
                                        Console.WriteLine(gamemain.pBoxes);
                                    } catch (Exception x) {
                                        Console.WriteLine("Error: " + x);
                                    }
                                } else if (canSetHasPlayerJoined) hasPlayerJoined = true;*/

                                player = message.player;
                                game = message;

                                Image[] DiceImages = new Image[] { null, Properties.Resources.dice_one, Properties.Resources.dice_two, Properties.Resources.dice_three, Properties.Resources.dice_four, Properties.Resources.dice_five, Properties.Resources.dice_six };

                                gamemain.Invoke((Action)delegate {

                                    if (message.type == "join" || message.type == "leave") {
                                        try {
                                            if (message.type == "join") {
                                                var pb = new PlayerBox();
                                                pb.id = message.socketId;
                                                gamemain.pBoxes.Add(pb);
                                                gamemain.Controls.Add(pb);
                                                pb.Show();
                                                pb.Parent = gamemain.MainPanel;
                                                pb.BringToFront();
                                                Console.WriteLine(gamemain.pBoxes);
                                                Console.WriteLine(gamemain.pBoxes[0].Location);

                                            } else if (message.type == "leave") {
                                                foreach (var pbox in gamemain.pBoxes) {
                                                    if (pbox.id == message.socketId) {
                                                        gamemain.pBoxes.Remove(pbox);
                                                        gamemain.Controls.Remove(pbox);
                                                        break;
                                                    }
                                                }
                                            }
                                        } catch (Exception x) {
                                            Console.WriteLine(x);
                                        }
                                    } else {
                                        try {
                                            ((PictureBox)gamemain.Controls.Find("roll", false)[0]).Image = DiceImages[game.roll];
                                        } catch (Exception x) {
                                            //Console.WriteLine(x);
                                        }

                                        try {
                                            gamemain.money.Text = "You have: $" + player.money;
                                            gamemain.roll.Image = DiceImages[game.roll];
                                            gamemain.name.Text = player.name;
                                            gamemain.status.Text = player.status;
                                            gamemain.betval.Text = "Bet $" + gamemain.setBet + " on:";
                                            gamemain.choice.Image = DiceImages[gamemain.setC];
                                            gamemain.timeout.Text = (30 - player.timeout).ToString();
                                            gamemain.room_code.Text = player.room_code;

                                            gamemain.UpdatePlayers();

                                            DatabaseConn.setMoney(player.money);
                                            new DatabaseConn("", "").Upload();

                                            if (player.timeouts >= 3) throw new Exception("Player was kicked for 3 timeouts.");

                                        } catch (Exception x) {
                                            //Console.WriteLine(x);
                                        }
                                    }
                                });
                            } catch (Exception x) {
                                //Console.WriteLine(x);
                            }
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
                ws.Send(message);
            }
        }

    }

    static class ClientMessage {
        public static string bet = "bet";
        public static string name = "name";
        public static string join = "join";
        public static string money = "money";

        public class ClientMessageObject {
            public string type;
        }

        public class BetMessage : ClientMessageObject {
            public int choice;
            public int bet;

            public BetMessage() {
                type = ClientMessage.bet;
            }
        }

        public class NameMessage : ClientMessageObject {
            public string name;

            public NameMessage() {
                type = ClientMessage.name;
            }
        }

        public class MoneyMessage : ClientMessageObject {
            public int money;

            public MoneyMessage() {
                type = ClientMessage.money;
            }
        }

        public class JoinMessage : ClientMessageObject {
            public string room_code;

            public JoinMessage() {
                type = "join";
            }
        }
    }

    public class UpdateMessage {
        public string type;
        public Player player;
        public string socketId;
        public int roll;
        public Player[] players;
    }

    public class Player {
        public string name;
        public string status;
        public int id;
        public string socketId;
        public int money;
        public bool hasBet;
        public int timeout;
        public int timeouts;
        public string room_code;
    }
}

