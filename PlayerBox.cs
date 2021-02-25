using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;

namespace DiceGame {
    public partial class PlayerBox : UserControl {

        public string id;

        private bool hasClickEvent = false;
        private Action clickEvent;

        public PlayerBox() {
            InitializeComponent();
            setImage();
            Show();
        }

        //public PlayerBox(string _id) {
        //    InitializeComponent();
        //    id = _id;
        //    setImage();
        //}

        public void setPosition(int x, int y) {
            Location = new Point(x - Width / 2, y - Height / 2);
        }

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
