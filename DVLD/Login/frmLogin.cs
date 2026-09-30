using DVLD.Classes;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLD.Classes.clsGlobal;

namespace DVLD.Login
{
    public partial class frmLogin : Form
    {

        private enum enLoginStatus : byte {enFailedToLoginAfterThreeTrials, enLoginSucceeded }
        private byte _logTrials = 0;

        public frmLogin()
        {
            InitializeComponent();
        }

       
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool WriteErrorOrInfoEventToTheEventViewer(enLoginStatus loginInfo)
        {
           
            string sourceName = "DVLD_Program";

            try
            {
                // Create the event source if it does not exist
                if (!EventLog.SourceExists(sourceName))
                {
                    EventLog.CreateEventSource(sourceName, "Application");
                }

                switch (loginInfo)
                {
                    case enLoginStatus.enFailedToLoginAfterThreeTrials:
                        {
                            MessageBox.Show("The system is locked, please try again!", "System Locked", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            EventLog.WriteEntry(sourceName, "Failed to login after three trials!", EventLogEntryType.Warning);
                            break;
                        }

                    case enLoginStatus.enLoginSucceeded:
                        {
                            EventLog.WriteEntry(sourceName, $"User {CurrentUser.UserName} has loged to the system!", EventLogEntryType.Information);
                            break;
                        }
                }

                return true;
            }
            
            catch(Exception ex)
            {
                MessageBox.Show($"Something went wrong, {ex.Message}");
                return false;
            }
        }

        private bool _VerifyPassword(string enteredPassword, string storedHash)
        {
            // 1. Séparer le sel et le hash en utilisant le point '.'
            string[] parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            // 2. Convertir le sel et le hash stockés de Base64 vers byte[]
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            // 3. Recalculer le hash du mot de passe saisi avec le sel extrait (100 000 itérations)
            using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 100000))
            {
                byte[] computedHash = pbkdf2.GetBytes(64);

                // 4. Comparer le hash calculé avec le hash stocké
                return _SlowEquals(storedHashBytes, computedHash);
            }
        }

        // Méthode pour éviter les attaques temporelles (Timing Attacks)
        private bool _SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            clsUser user = clsUser.FindByUsername(txtUserName.Text.Trim());

            if (user != null && _VerifyPassword(txtPassword.Text.Trim(), user.Password)) 
            { 

                if (chkRememberMe.Checked)
                {
                    //store username and password
                    RememberUsernameAndPassword(txtUserName.Text.Trim(), txtPassword.Text.Trim());   
                }
                
                else
                {
                    //store empty username and password
                    RememberUsernameAndPassword("", "");
                }

                //incase the user is not active
                if (!user.IsActive)
                {
                    txtUserName.Focus();
                    MessageBox.Show("Your account is not Active, Contact Admin!", "inActive Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CurrentUser = user;
                CurrentUser.Password = txtPassword.Text.Trim();
                WriteErrorOrInfoEventToTheEventViewer(enLoginStatus.enLoginSucceeded);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            else
            {
                txtUserName.Focus();
                MessageBox.Show("Invalid Username/Password.", "Wrong Credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (++_logTrials >= 3)
                {
                    WriteErrorOrInfoEventToTheEventViewer(enLoginStatus.enFailedToLoginAfterThreeTrials);
                    btnLogin.Enabled = false;
                }
            }    

        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            string UserName = "", Password = "";

            if (GetStoredCredential(ref UserName, ref Password))
            {
                txtUserName.Text = UserName;
                txtPassword.Text = Password;
                chkRememberMe.Checked = true;
            }

            else
                chkRememberMe.Checked = false;

        }
    }
}
