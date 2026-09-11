using DVLD.Classes;
using DVLD.DriverLicense;
using DVLD.Licenses.International_License;
using DVLD.Licenses.International_Licenses;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLD_Business.clsApplication;
using static DVLD.Classes.clsGlobal;

namespace DVLD.Applications.International_License
{
    public partial class frmNewInternationalLicenseApplication : Form
    {
        
        private int _InternationalLicenseID = -1;
      
        public frmNewInternationalLicenseApplication()
        {
            InitializeComponent();
        }

        private void ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected(int ID)
        {
            int SelectedLicenseID = ID;

            lblLocalLicenseID.Text = SelectedLicenseID.ToString();

            llShowLicenseHistory.Enabled = (SelectedLicenseID != -1);

            if (SelectedLicenseID == -1) 
            {
                return;
            }

            

            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.LicenseClass != 3)
            {
                MessageBox.Show("Selected license should be of Class 3!", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int ActiveInternaionalLicenseID = clsInternationalLicense.GetActiveInternationalLicenseIDByDriverID(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverID);

            if (ActiveInternaionalLicenseID != -1)
            {
                MessageBox.Show("Person already has an active international license with ID = " + ActiveInternaionalLicenseID.ToString(),
                    "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                llShowLicenseInfo.Enabled = true;
                _InternationalLicenseID = ActiveInternaionalLicenseID;
                btnIssueLicense.Enabled = false;
                return;
            }

            btnIssueLicense.Enabled = true;
        }

        private void frmNewInternationalLicenseApplication_Load(object sender, EventArgs e)
        {
            lblApplicationDate.Text = clsFormat.DateToShort(DateTime.Now);
            lblIssueDate.Text = lblApplicationDate.Text;
            lblExpirationDate.Text = clsFormat.DateToShort(DateTime.Now.AddYears(1)); //adding one year.
            lblFees.Text = clsApplicationType.Find((int)clsApplication.enApplicationType.NewInternationalLicense).Fees.ToString();
            lblCreatedByUser.Text = CurrentUser.UserName;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIssueLicense_Click(object sender, EventArgs e)
        {
           
            if ( MessageBox.Show("Do you realy want to issue that license?","Confirm",MessageBoxButtons.YesNo ,MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            clsInternationalLicense IntLicense = new clsInternationalLicense();
            int PersonID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID;
            int IssuedUsingLocalLicenseID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.LicenseID;

            if (!IntLicense.IssueAnInternationalLicense(PersonID, CurrentUser.UserID, IssuedUsingLocalLicenseID))
            {
                MessageBox.Show("Something went wrong, license has not been issued" , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblApplicationID.Text = IntLicense.ApplicationID.ToString();
            _InternationalLicenseID = IntLicense.InternationalLicenseID;
            lblInternationalLicenseID.Text = IntLicense.InternationalLicenseID.ToString();
            MessageBox.Show("International license has been issued successfully in wich License ID = " + IntLicense.InternationalLicenseID.ToString()
               , "License Issued", MessageBoxButtons.OK, MessageBoxIcon.Information);

            btnIssueLicense.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            llShowLicenseInfo.Enabled = true;
        }

        private void llShowDriverLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowPersonLicenseHistory  frm = new frmShowPersonLicenseHistory(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverInfo.PersonID);
            frm.ShowDialog();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowInternationalLicenseInfo frm = new frmShowInternationalLicenseInfo(_InternationalLicenseID);
            frm.ShowDialog();
        }

        private void frmNewInternationalLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();
        }
    }
}
