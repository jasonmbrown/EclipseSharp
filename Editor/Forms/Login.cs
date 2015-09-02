using Editor.Networking;
using System;
using System.Windows.Forms;

namespace Editor {
    public partial class Login : Form {
        public Login() {
            InitializeComponent();
        }

        public void UpdateStatus(Boolean connected) {
            if (connected) {
                lblStatus.Text      = "Connected.";
                btnLogin.Enabled    = true;
            } else {
                lblStatus.Text      = "Failed.";
                btnLogin.Enabled    = false;
                MessageBox.Show("Unable to connect to server.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e) {
            if (txtPassword.Text.Length < 1 || txtUsername.Text.Length < 1) {
                MessageBox.Show("Please fill in your username and password.", "Entry Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Send.Login(txtUsername.Text, txtPassword.Text);
        }
    }
}
