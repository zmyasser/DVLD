using DVLD.Applications;
using DVLD_Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using DVLD_Business;
using DVLD.DriverLicense;
using System.Security.Cryptography;
using DVLD.Drivers;
using DVLD.Licenses.International_License;

namespace DVLD.Tests
{
    public partial class frmListLocalDrivingLicesnseApplications : Form
    {
        private DataTable _dtAllLocalDrivingLicenseApplications;
        public frmListLocalDrivingLicesnseApplications()
        {
            InitializeComponent();
        }

        private void frmListLocalDrivingLicesnseApplications_Load(object sender, EventArgs e)
        {
            _dtAllLocalDrivingLicenseApplications = clsLocalDrivingLicenseApplication.GetAllLocalDrivingLicenseApplications();
            dgvLocalDrivingLicenseApplications.DataSource = _dtAllLocalDrivingLicenseApplications;
            
            lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();

            if (dgvLocalDrivingLicenseApplications.Rows.Count>0)
            {

                dgvLocalDrivingLicenseApplications.Columns[0].HeaderText = "L.D.L.AppID";
                dgvLocalDrivingLicenseApplications.Columns[0].Width = 120;

                dgvLocalDrivingLicenseApplications.Columns[1].HeaderText = "Driving Class";
                dgvLocalDrivingLicenseApplications.Columns[1].Width = 300;

                dgvLocalDrivingLicenseApplications.Columns[2].HeaderText = "National No.";
                dgvLocalDrivingLicenseApplications.Columns[2].Width = 150;

                dgvLocalDrivingLicenseApplications.Columns[3].HeaderText = "Full Name";
                dgvLocalDrivingLicenseApplications.Columns[3].Width = 350;

                dgvLocalDrivingLicenseApplications.Columns[4].HeaderText = "Application Date";
                dgvLocalDrivingLicenseApplications.Columns[4].Width = 170;

                dgvLocalDrivingLicenseApplications.Columns[5].HeaderText = "Passed Tests";
                dgvLocalDrivingLicenseApplications.Columns[5].Width = 150;
            }

            cbFilterBy.SelectedIndex = 0;

        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow is null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0) return;

            int LDLAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            frmLocalDrivingLicenseApplicationInfo frm = new frmLocalDrivingLicenseApplicationInfo(LDLAppID);
            frm.ShowDialog();

            //refresh
            frmListLocalDrivingLicesnseApplications_Load(null, null);

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

            _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = "";
            lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {

            string FilterColumn = "";

            //Map Selected Filter to real Column name 
            switch (cbFilterBy.Text)
            {

                case "L.D.L.AppID":
                    FilterColumn = "LocalDrivingLicenseApplicationID";
                    break;

                case "National No.":
                    FilterColumn = "NationalNo";
                    break;


                case "Full Name":
                    FilterColumn = "FullName";
                    break;

                case "Status":
                    FilterColumn = "Status";
                    break;


                default:
                    FilterColumn = "None";
                    break;

            }

            //Reset the filters in case nothing selected or filter value contains nothing.
            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = "";
                lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
                return;
            }


            if (FilterColumn == "LocalDrivingLicenseApplicationID")
                _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
            else
                _dtAllLocalDrivingLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim());

            lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow is null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0) return;

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            frmAddUpdateLocalDrivingLicesnseApplication frm =
                         new frmAddUpdateLocalDrivingLicesnseApplication(LocalDrivingLicenseApplicationID);
            frm.ShowDialog();

            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.Text == "L.D.L.AppID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void vistionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            clsTestAppointment Appointment =
                clsTestAppointment.GetLastTestAppointment(LocalDrivingLicenseApplicationID, clsTestType.enTestType.VisionTest);

            if (Appointment is null)
            {
                MessageBox.Show("No Vision Test Appointment Found!", "Set Appointment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            frmTakeTest frm = new frmTakeTest(Appointment.TestAppointmentID, clsTestType.enTestType.VisionTest);
            frm.ShowDialog();
        }

        private void writtenTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;


            if (!clsLocalDrivingLicenseApplication.DoesPassTestType(LocalDrivingLicenseApplicationID, clsTestType.enTestType.VisionTest))
            {
                MessageBox.Show("Person Should Pass the Vision Test First!", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clsTestAppointment Appointment =
               clsTestAppointment.GetLastTestAppointment(LocalDrivingLicenseApplicationID, clsTestType.enTestType.WrittenTest);


            if (Appointment is null)
            {
                MessageBox.Show("No Written Test Appointment Found!", "Set Appointment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            frmTakeTest frm = new frmTakeTest(Appointment.TestAppointmentID, clsTestType.enTestType.WrittenTest);
            frm.ShowDialog();

        }

        private void streetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            if (!clsLocalDrivingLicenseApplication.DoesPassTestType(LocalDrivingLicenseApplicationID, clsTestType.enTestType.WrittenTest))
            {
                MessageBox.Show("Person Should Pass the Written Test First!", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clsTestAppointment Appointment =
             clsTestAppointment.GetLastTestAppointment(LocalDrivingLicenseApplicationID, clsTestType.enTestType.StreetTest);


            if (Appointment is null)
            {
                MessageBox.Show("No Street Test Appointment Found!", "Set Appointment", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            frmTakeTest frm = new frmTakeTest(Appointment.TestAppointmentID, clsTestType.enTestType.StreetTest);
            frm.ShowDialog();


        }

        private void _ScheduleTest(clsTestType.enTestType TestType)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow is null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0) return;

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;
            frmListTestAppointments frm = new frmListTestAppointments(LocalDrivingLicenseApplicationID, TestType);
            frm.ShowDialog();

            //refresh
            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }
      
        private void scheduleVisionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ScheduleTest(clsTestType.enTestType.VisionTest);
        }

        private void scheduleWrittenTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            _ScheduleTest(clsTestType.enTestType.WrittenTest);
        }

        private void scheduleStreetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ScheduleTest(clsTestType.enTestType.StreetTest);
        }

        private void btnAddNewApplication_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicesnseApplication frm = new frmAddUpdateLocalDrivingLicesnseApplication();
            frm.ShowDialog();

            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }

        private void issueDrivingLicenseFirstTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow is null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0) return;

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;
            frmIssueDriverLicenseFirstTime frm = new frmIssueDriverLicenseFirstTime(LocalDrivingLicenseApplicationID);
            frm.ShowDialog();

            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }

        private void cmsApplications_Opening(object sender, CancelEventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            int TotalPassedTests = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[5].Value;

            bool LicenseExists = LocalDrivingLicenseApplication.IsLicenseIssued();

            //Enabled only if person passed all tests and Does not have license. 
            issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = (TotalPassedTests == 3) && !LicenseExists;
            
            showLicenseToolStripMenuItem.Enabled = LicenseExists;
            editToolStripMenuItem.Enabled = !LicenseExists && (LocalDrivingLicenseApplication.ApplicationStatus == clsApplication.enApplicationStatus.New);
            ScheduleTestsMenue.Enabled = !LicenseExists;

            //Enable/Disable Cancel Menue Item
            //We only canel the applications with status = new.
            CancelApplicaitonToolStripMenuItem.Enabled = (LocalDrivingLicenseApplication.ApplicationStatus == clsApplication.enApplicationStatus.New);

            //Enable/Disable Delete Menue Item
            //We only allow delete incase the application status is new not complete or Cancelled.
            DeleteApplicationToolStripMenuItem.Enabled = (LocalDrivingLicenseApplication.ApplicationStatus == clsApplication.enApplicationStatus.New );



            //Enable Disable Schedule menue and it's sub menue
            bool PassedVisionTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestType.VisionTest); ;
            bool PassedWrittenTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestType.WrittenTest);
            bool PassedStreetTest = LocalDrivingLicenseApplication.DoesPassTestType(clsTestType.enTestType.StreetTest);

            ScheduleTestsMenue.Enabled = (!PassedVisionTest || !PassedWrittenTest || !PassedStreetTest) && 
                (LocalDrivingLicenseApplication.ApplicationStatus == clsApplication.enApplicationStatus.New);

            if (ScheduleTestsMenue.Enabled)
            {
                //To Allow Schdule vision test, Person must not pass the same test before.
                scheduleVisionTestToolStripMenuItem.Enabled = !PassedVisionTest;

                //To Allow Schdule written test, Person must pass the vision test and must not pass the same test before.
                scheduleWrittenTestToolStripMenuItem.Enabled = PassedVisionTest && !PassedWrittenTest;

                //To Allow Schdule street test, Person must pass the vision & written tests, and must not pass the same test before.
                scheduleStreetTestToolStripMenuItem.Enabled = PassedVisionTest && PassedWrittenTest && !PassedStreetTest;
            }
        }

        private void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dgvLocalDrivingLicenseApplications.CurrentRow is null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0) return;

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            int LicenseID = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(
               LocalDrivingLicenseApplicationID).GetActiveLicenseID();

            if (LicenseID != -1)
            {
                frmShowLicenseInfo frm = new frmShowLicenseInfo(LicenseID);
                frm.ShowDialog();
            }

            else
            {
                MessageBox.Show("No license was found!", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void CancelApplicaitonToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dgvLocalDrivingLicenseApplications.CurrentRow is null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0) return;


            if (MessageBox.Show("Do you realy want to cancel this application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            if (LocalDrivingLicenseApplication != null)
            {
                if (LocalDrivingLicenseApplication.Cancel())
                {
                    MessageBox.Show("Application has been Cancelled Successfully!", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmListLocalDrivingLicesnseApplications_Load(null, null);
                }
                else
                {
                    MessageBox.Show("Could not cancel application!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else
            {
                MessageBox.Show("Selected Application was not found!", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }

        private void DeleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dgvLocalDrivingLicenseApplications.CurrentRow is null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0) return;

            if (MessageBox.Show("Do you realy want to delete this application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;

            clsLocalDrivingLicenseApplication LocalDrivingLicenseApplication =
                clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            if (LocalDrivingLicenseApplication != null)
            {
                if (LocalDrivingLicenseApplication.Delete())
                {
                    MessageBox.Show("Application has been Deleted Successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmListLocalDrivingLicesnseApplications_Load(null, null);
                }
                else
                {
                    MessageBox.Show("Could not delete that application, other data depends on it!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else
            {
                MessageBox.Show("Selected Application was not found!", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow is null || dgvLocalDrivingLicenseApplications.CurrentRow.Index < 0) return;


            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;
            clsLocalDrivingLicenseApplication localDrivingLicenseApplication = clsLocalDrivingLicenseApplication.FindByLocalDrivingAppLicenseID(LocalDrivingLicenseApplicationID);

            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(localDrivingLicenseApplication.ApplicantPersonID);
            frm.ShowDialog();
        }
    }
}
