using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsInternationalLicense
    {

        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        public int InternationalLicenseID { set; get; }
        public int ApplicationID { get; set; }

        private clsApplication _ApplicationInfo;
        public clsApplication ApplicationInfo
        {
            get
            {
                if (_ApplicationInfo is null && ApplicationID > 0)
                    _ApplicationInfo = clsApplication.FindBaseApplication(ApplicationID);

                return _ApplicationInfo;
            }

            set { _ApplicationInfo = value; }
        }


        public int DriverID { set; get; }
        private clsDriver _DriverInfo;
        public clsDriver DriverInfo
        {
            get
            {
                if (_DriverInfo is null && DriverID > 0)
                    _DriverInfo = clsDriver.FindByDriverID(DriverID);

                return _DriverInfo;
            }

            set { _DriverInfo = value; }
        }

        public int IssuedUsingLocalLicenseID { set; get; }   
        public DateTime IssueDate { set; get; }
        public DateTime ExpirationDate { set; get; }    
        public bool IsActive { set; get; }

        public int CreatedByUserID { get; set; }
       
        private clsInternationalLicense(int InternationalLicenseID,  int ApplicationID, int DriverID, int IssuedUsingLocalLicenseID,
            DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {
            
            this.InternationalLicenseID = InternationalLicenseID;
            this.ApplicationID = ApplicationID;
            this.DriverID = DriverID;
            this.IssuedUsingLocalLicenseID = IssuedUsingLocalLicenseID;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
            this.IsActive = IsActive;
            this.CreatedByUserID = CreatedByUserID;

            _Mode = enMode.Update;
        }

        public clsInternationalLicense()
        {
            
            this.InternationalLicenseID = -1;
            this.ApplicationID = -1;
            this.DriverID = -1;
            this.IssuedUsingLocalLicenseID = -1;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.Now;
            this.IsActive = true;

            _Mode = enMode.AddNew;
        }


        private bool _AddNewInternationalLicense()
        {
            this.InternationalLicenseID = 
                clsInternationalLicenseData.AddNewInternationalLicense(this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
               this.IssueDate, this.ExpirationDate, 
               this.IsActive, this.CreatedByUserID);

            return (this.InternationalLicenseID != -1);
        }

        private bool _UpdateInternationalLicense()
        {
            return clsInternationalLicenseData.UpdateInternationalLicense(
                this.InternationalLicenseID,this.ApplicationID, this.DriverID, this.IssuedUsingLocalLicenseID,
               this.IssueDate, this.ExpirationDate, 
               this.IsActive, this.CreatedByUserID);
        }

        public static clsInternationalLicense Find(int InternationalLicenseID)
        {
            int ApplicationID = -1;
            int DriverID = -1; int IssuedUsingLocalLicenseID = -1;
            DateTime IssueDate = DateTime.Now; DateTime ExpirationDate = DateTime.Now;
             bool IsActive = true; int CreatedByUserID = 1;

            if (clsInternationalLicenseData.GetInternationalLicenseInfoByID(InternationalLicenseID,ref ApplicationID, ref DriverID, 
                ref IssuedUsingLocalLicenseID, ref IssueDate, ref ExpirationDate, ref IsActive, ref CreatedByUserID))
            {

                return new clsInternationalLicense(InternationalLicenseID, ApplicationID, DriverID, IssuedUsingLocalLicenseID,
                                         IssueDate, ExpirationDate, IsActive, CreatedByUserID);
            }
             
            else
                return null;

        }

        public static DataTable GetAllInternationalLicenses()
        {
            return clsInternationalLicenseData.GetAllInternationalLicenses();
        }

        public bool Save()
        {

            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewInternationalLicense())
                    {

                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateInternationalLicense();

            }

            return false;
        }

        public bool IssueAnInternationalLicense(int PersonID, int CreatedByUserID, int IssuedUsingLocalLicenseID)
        {
            clsApplication Application = new clsApplication();

            Application.ApplicantPersonID = PersonID;
            Application.ApplicationDate = DateTime.Now;
            Application.ApplicationTypeID = (int)clsApplication.enApplicationType.NewInternationalLicense;
            Application.ApplicationStatus = clsApplication.enApplicationStatus.Completed;
            Application.LastStatusDate = DateTime.Now;
            Application.PaidFees = clsApplicationType.Find((int)clsApplication.enApplicationType.NewInternationalLicense).Fees;
            Application.CreatedByUserID = CreatedByUserID;

            if (!Application.Save())
                return false;


            this.ApplicationID = Application.ApplicationID;
            this.DriverID = clsDriver.FindByPersonID(PersonID).DriverID;
            this.IssuedUsingLocalLicenseID = IssuedUsingLocalLicenseID;
            this.IssueDate = DateTime.Now;
            this.ExpirationDate = DateTime.Now.AddYears(1); // Adding one year!
            this.IsActive = true;
            this.CreatedByUserID = CreatedByUserID;

            if (!this.Save())
                return false;


            return true;

        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            return clsInternationalLicenseData.GetActiveInternationalLicenseIDByDriverID(DriverID);
        }

        public static DataTable GetDriverInternationalLicenses(int DriverID)
        {
            return clsInternationalLicenseData.GetDriverInternationalLicenses(DriverID);
        }
    }
}
