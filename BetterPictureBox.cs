﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiceGame
{
    public partial class BetterPictureBox : UserControl
    {

        public Image Image = null;

        public BetterPictureBox()
        {
            InitializeComponent();
            Image = Properties.Resources.CGLogo;
        }

        private void BetterPictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image, new Point(Location.X, Location.Y+100));
        }
    }
}
