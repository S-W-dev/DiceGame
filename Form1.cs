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

namespace DiceGame {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            using (var ws = new WebSocket("ws://concretegames.net:667/socket/?EIO=2&transport=websocket")) {
                Console.WriteLine(ws);
                ws.OnMessage += (sender, e) =>
                    Console.WriteLine("Message: " + e.Data);
                ws.OnError += (sender, e) =>
                    Console.WriteLine("Error: " + e.Message);
                ws.Connect();
                Console.ReadKey(true);
            }

        }

    }
}
