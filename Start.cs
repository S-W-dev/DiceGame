using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiceGame {
    public partial class Start : Form {
        public Start() {
            InitializeComponent();
            playerBox1.setName(DatabaseConn.getUsername());
            playerBox1.setMoney(DatabaseConn.getMoney());
            playerBox1.setImage(DatabaseConn.getImage());
            playerBox1.setClickForName(delegate {
                DatabaseConn.setUsername(Prompt.ShowDialog("Enter a new username: ", "Enter a new username"));
                playerBox1.setName(DatabaseConn.getUsername());
            });
            try {
                new DatabaseConn("", "").DownloadAndSet();
            } catch (Exception) { }
        }

        private void button1_Click(object sender, EventArgs e) {
            Hide();
            new GameMain(false).Show();
        }

        private void button2_Click(object sender, EventArgs e) {
            Hide();
            new GameMain(true).Show();
        }

        private void button3_Click(object sender, EventArgs e) {
        }

        private void Start_Load(object sender, EventArgs e) {
            //Console.WriteLine(DatabaseConn.getUsername());
        }

        private void button4_Click(object sender, EventArgs e) {
            Alert.ShowDialog("Sign Up not allowed for school version of game.", "School Policy");
        }

        private async void button3_Click_1(object sender, EventArgs e) {
            LoginData data = await Login.ShowDialog("Login to your ConcreteGames account below.", "Login to your ConcreteGames Account");
            DatabaseConn.setUsername(data.username);
            DatabaseConn.setMoney(data.money);
            DatabaseConn.setImage(data.image);
            Console.WriteLine(DatabaseConn.getImage());
            new Start().Show();
            this.Hide();
        }

        private void playerBox1_SizeChanged(object sender, EventArgs e) {
            playerBox1.FixProfilePicSize();
        }
    }
}
