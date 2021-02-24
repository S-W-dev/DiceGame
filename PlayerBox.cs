using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace DiceGame {
    public partial class PlayerBox : UserControl {

        public string id;

        private bool hasClickEvent = false;
        private Action clickEvent;

        public PlayerBox() {
            InitializeComponent();
            setImage();
        }

        //public PlayerBox(string _id) {
        //    InitializeComponent();
        //    id = _id;
        //    setImage();
        //}

        public void setClickForName(Action a) {
            hasClickEvent = true;
            clickEvent = a;
        }

        public void setName(string name) {
            this.name.Text = name;
        }

        public void setMoney(int money) {
            this.money.Text = "$" + money;
        }

        public void setMoney(string money) {
            this.money.Text = money;
        }

        public void setStatus(string status) {
            this.status.Text = status;
        }

        public void setImage(string imageUrl = "https://concretegames.net/uploads/DefaultUser.png") {
            pictureBox1.Image = new Bitmap(new WebClient().OpenRead(imageUrl));
        }

        private void name_Click(object sender, EventArgs e) {
            if (hasClickEvent) Invoke(clickEvent);
        }
    }
}
