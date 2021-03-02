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

            new DatabaseConn("", "").DownloadAndSet();

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

            Console.WriteLine("ORIGINAL SIZE: " + Width);
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

            tableTable.ColumnStyles[2].Width = 700f / 1592f * MainPanel.Width;
            tableTable.RowStyles[2].Height = 350f / 850f * MainPanel.Height;

            Console.WriteLine(Table.Location);

            Console.WriteLine("LOG 1: " + MainPanel.Width);

            setChoice(1);

            setName(DatabaseConn.getUsername());
            setMoney(DatabaseConn.getMoney());
            setImage(DatabaseConn.getImage());

            UpdatePlayers();
        }

        //Player[] currentPlayers;
        List<PlayerBox> pBoxes = new List<PlayerBox>();

        int[][][] DefaultPositions = { new int[][] { new int[] { 597, 185 } }, 
                                       new int[][] { new int[] { 0, 0 } } 
                                     };

        private void UpdatePlayers() {
            int tableCenterX = MainPanel.Width/2;
            int tableCenterY = Table.Location.Y + Table.Height/2;
            DefaultPositions = new int[][][] {
                                //new int[][] { new int[] { MainPanel.Width/2, (int)(MainPanel.Height / 2 - (400f / 850f * MainPanel.Height) / 2) } }, 
                                new int[][] { new int[] { 0, 225 } },
                                new int[][] { new int[] { 0, 225 }, new int[] { 0, -225 } },
                                new int[][] { new int[] { 0, 225 }, new int[] { 0, -225 }, new int[] { -450, 0 } },
                                new int[][] { new int[] { 0, 225 }, new int[] { 0, -225 }, new int[] { -450, 0 }, new int[] { 450, 0 } },
                                new int[][] { new int[] { -120, 225 }, new int[] { 0, -225 }, new int[] { -450, 0 }, new int[] { 450, 0 }, new int[] { 120, 225 } },
                                new int[][] { new int[] { -120, 225 }, new int[] { 120, -225 }, new int[] { -450, 0 }, new int[] { 450, 0 }, new int[] { 120, 225 }, new int[] { -120, -225 } },
                                new int[][] { new int[] { -120, 225 }, new int[] { 0, -225 }, new int[] { -450, 0 }, new int[] { 450, 0 }, new int[] { 120, 225 }, new int[] { -380, -170 }, new int[] { 380, -170 } },
                                new int[][] { new int[] { 0, 225 }, new int[] { 0, -225 }, new int[] { -450, 0 }, new int[] { 450, 0 }, new int[] { 380, 170 }, new int[] { -380, -170 }, new int[] { 380, -170 }, new int[] { -380, 170 } }
                               };
            Console.WriteLine("LOG 2: " + MainPanel.Width);
            for (var i = 0; i < pBoxes.Count; i++) {
                pBoxes[i].UpdateScale(MainPanel.Width, MainPanel.Height);
                pBoxes[i].setPosition((int)(DefaultPositions[pBoxes.Count - 1][i][0] / 1592f * MainPanel.Width) + tableCenterX, tableCenterY - (int)(DefaultPositions[pBoxes.Count - 1][i][1] / 850f * MainPanel.Height));
                //pBoxes[i].Location = new Point(i * 100, 0);
                pBoxes[i].setImage(game.players[i].image);
                Console.WriteLine(game.players[i].image);
                pBoxes[i].setMoney(game.players[i].money);
                pBoxes[i].setName(game.players[i].name);
                pBoxes[i].setStatus(game.players[i].status);
                pBoxes[i].Show();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            
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

        private void setImage(string image) {
            var message = new ClientMessage.ImageMessage();
            message.image = image;
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

        private void roll_Click(object sender, EventArgs e) {

        }

        private void GameMain_FormClosing(object sender, FormClosingEventArgs e) {
            server.Close();
            conn.Abort();
            Application.Exit();
        }

        private void quit_Click(object sender, EventArgs e) {
            Close();
        }

        class ServerComponents {
            private GameMain gamemain;
            public ServerComponents(GameMain _gamemain) {
                gamemain = _gamemain;
            }

            string[] StatusArray = new string[] { "Won the Bet!", "Lost the Bet!" };
            string[] GetStatusArray = new string[] { "Won the Bet!", "Lost the Bet!" };


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
                                player = message.player;
                                game = message;
                                Image[] DiceImages = new Image[] { null, Properties.Resources.dice_one, Properties.Resources.dice_two, Properties.Resources.dice_three, Properties.Resources.dice_four, Properties.Resources.dice_five, Properties.Resources.dice_six };

                                gamemain.Invoke((Action)delegate {
                                    List<string> pbIDs = new List<string>();
                                    List<string> gpIDs = new List<string>();

                                    foreach (var p in gamemain.pBoxes) {
                                        pbIDs.Add(p.id);
                                    }

                                    foreach (var gp in game.players) {
                                        gpIDs.Add(gp.socketId);
                                    }

                                    foreach (var pid in pbIDs) {
                                        if (!gpIDs.Contains(pid)) { //remove player
                                            foreach (var pbox in gamemain.pBoxes) {
                                                if (pbox.id == pid) {
                                                    gamemain.pBoxes.Remove(pbox);
                                                    gamemain.Controls.Remove(pbox);
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    foreach (var gpid in gpIDs) {
                                        if (!pbIDs.Contains(gpid)) { //add player
                                            var pb = new PlayerBox();

                                            pb.id = gpid;
                                            gamemain.pBoxes.Add(pb);
                                            gamemain.Controls.Add(pb);
                                            pb.Hide();
                                            pb.Parent = gamemain.MainPanel;
                                            pb.BringToFront();
                                        }
                                    }

                                    try {
                                        var rollpb = new PictureBox();
                                        rollpb.Size = new Size(200, 200);
                                        rollpb.Location = new Point((gamemain.Table.Location.X + gamemain.Table.Width / 2) - rollpb.Width / 2, (gamemain.Table.Location.Y + gamemain.Table.Height / 2) - rollpb.Height / 2);
                                        rollpb.Name = "roll";
                                        rollpb.Parent = gamemain.MainPanel;
                                        gamemain.Controls.Add(rollpb);
                                        rollpb.Show();
                                        rollpb.BringToFront();
                                        rollpb.Image = DiceImages[1];
                                        rollpb.SizeMode = PictureBoxSizeMode.CenterImage;
                                    } catch (Exception) { }

                                    try {
                                        ((PictureBox)gamemain.Controls.Find("roll", false)[0]).Image = DiceImages[game.roll];
                                    } catch (Exception x) {
                                        //Console.WriteLine(x);
                                    }

                                    try {
                                        switch (player.status) {
                                            case "Lost Game":
                                                gamemain.oldstatus.Text = "Lost the Game!";
                                                gamemain.oldstatus.BackColor = Color.Yellow;
                                                Alert.ShowDialog("You lost the game!", "You Lost");
                                                break;
                                            case "Lost Bet":
                                                gamemain.oldstatus.Text = "Lost the Bet!";
                                                break;
                                            case "Won Game!":
                                                gamemain.oldstatus.Text = "Won the Game!";
                                                Alert.ShowDialog("You WON the game!", "You Won!");
                                                break;
                                            case "Won Bet!":
                                                gamemain.oldstatus.Text = "Won the Bet!";
                                                break;
                                            default:
                                                break;
                                        }
                                        gamemain.money.Text = "You have: $" + player.money;
                                        gamemain.name.Text = player.name;
                                        gamemain.status.Text = player.status;
                                        gamemain.betval.Text = "Bet $" + gamemain.setBet + " on:";
                                        gamemain.choice.Image = DiceImages[gamemain.setC];
                                        gamemain.timeout.Text = (30 - player.timeout).ToString();
                                        gamemain.room_code.Text = player.room_code;

                                        gamemain.UpdatePlayers();

                                        DatabaseConn.setMoney(player.money);
                                        new DatabaseConn("", "").Upload();

                                        if (player.timeouts >= 3) {
                                            Alert.ShowDialog("You have been kicked from the game for being idle.", "You have been kicked.");
                                            Application.Exit();
                                        }

                                    } catch (Exception x) {
                                        Console.WriteLine(x);
                                    }
                                });
                            } catch (Exception x) {
                                Console.WriteLine(x);
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
        public static string image = "image";

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

        public class ImageMessage : ClientMessageObject {
            public string image;

            public ImageMessage() {
                type = ClientMessage.image;
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
        public string image;
    }
}

