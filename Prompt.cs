using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiceGame {

    public static class Login {
        public static LoginData ShowDialog(string text, string caption) {
            bool loginSuccess = false;
            Form login = new Form() {
                Width = 500,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox username = new TextBox() { Left = 50, Top = 50, Width = 400 };
            TextBox password = new TextBox() { Left = 50, Top = 70, Width = 400 };
            Button confirmation = new Button() { Text = "Login", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { login.Close(); };
            login.Controls.Add(username);
            login.Controls.Add(password);
            login.Controls.Add(confirmation);
            login.Controls.Add(textLabel);
            login.AcceptButton = confirmation;

            LoginData data = new LoginData();

            if (login.ShowDialog() == DialogResult.OK) {



                return data;
            } else {
                return data;
            }
        }
    }

    public class LoginData {
        public string username;
        public string name;
        public string money;
    }

    public static class Prompt {
        public static string ShowDialog(string text, string caption) {
            Form prompt = new Form() {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }

    public static class Alert {
        public static string ShowDialog(string text, string caption) {
            Form alert = new Form() {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { alert.Close(); };
            alert.Controls.Add(confirmation);
            alert.Controls.Add(textLabel);
            alert.AcceptButton = confirmation;

            return alert.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
