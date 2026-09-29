using DVLD.Classes;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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

        private bool WriteErrorEventToTheEventViewer(enLoginStatus loginInfo)
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

        private void btnLogin_Click(object sender, EventArgs e)
        {

            clsUser user = clsUser.FindByUsernameAndPassword(txtUserName.Text.Trim(),txtPassword.Text.Trim());

            if (user != null) 
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
                if (!user.IsActive )
                {
                    txtUserName.Focus();
                    MessageBox.Show("Your account is not Active, Contact Admin!", "inActive Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CurrentUser = user;
                WriteErrorEventToTheEventViewer(enLoginStatus.enLoginSucceeded);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            else
            {
                txtUserName.Focus();
                MessageBox.Show("Invalid Username/Password.", "Wrong Credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (++_logTrials >= 3)
                {
                    WriteErrorEventToTheEventViewer(enLoginStatus.enFailedToLoginAfterThreeTrials);
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
