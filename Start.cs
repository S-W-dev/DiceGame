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
        }

        private void button4_Click(object sender, EventArgs e) {
            Alert.ShowDialog("Sign Up not allowed for school version of game.", "School Policy");
        }

        private void button3_Click_1(object sender, EventArgs e) {
            
        }
    }
}
