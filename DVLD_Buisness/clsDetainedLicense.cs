using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsDetainedLicense
    {

        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;


        public int DetainID { set; get; }
        public int LicenseID { set; get; }
        public DateTime DetainDate { set; get; }

        public float  FineFees { set; get; }
        public int CreatedByUserID { set; get; }

        private clsUser _CreatedByUserInfo;
        public clsUser CreatedByUserInfo
        {
            get
            {
                if (_CreatedByUserInfo is null && CreatedByUserID > 0)
                    _CreatedByUserInfo = clsUser.FindByUserID(CreatedByUserID);

                return _CreatedByUserInfo;
            }

            set { _CreatedByUserInfo = value; }

        }


        public bool IsReleased { set; get; }
        public DateTime ReleaseDate { set; get; }
        public int ReleasedByUserID { set; get; }

        private clsUser _ReleasedByUserInfo;
        public clsUser ReleasedByUserInfo
        {
            get
            {
                if (_ReleasedByUserInfo is null && ReleasedByUserID > 0)
                    _ReleasedByUserInfo = clsUser.FindByUserID(ReleasedByUserID);

                return _ReleasedByUserInfo;
            }

            set { _ReleasedByUserInfo = value; }
        }

        public int ReleaseApplicationID { set; get; }

        private bool _AddNewDetainedLicense()
        {
            this.DetainID = clsDetainedLicenseData.AddNewDetainedLicense(this.LicenseID,this.DetainDate,this.FineFees,this.CreatedByUserID);
            
            return (this.DetainID != -1);
        }

        private bool _UpdateDetainedLicense()
        {
            return clsDetainedLicenseData.UpdateDetainedLicense(this.DetainID,this.LicenseID,this.DetainDate,this.FineFees,this.CreatedByUserID);
        }


        public clsDetainedLicense(int DetainID,
            int LicenseID,  DateTime DetainDate,
            float FineFees,  int CreatedByUserID,
            bool IsReleased,  DateTime ReleaseDate,
            int ReleasedByUserID,  int ReleaseApplicationID)

        {
            this.DetainID = DetainID;
            this.LicenseID = LicenseID;
            this.DetainDate = DetainDate;
            this.FineFees = FineFees;
            this.CreatedByUserID = CreatedByUserID;
            this.IsReleased = IsReleased;
            this.ReleaseDate = ReleaseDate;
            this.ReleasedByUserID = ReleasedByUserID;
            this.ReleaseApplicationID = ReleaseApplicationID;

            _Mode = enMode.Update;
        }

        public clsDetainedLicense()
        {
            this.DetainID = -1;
            this.LicenseID = -1;
            this.DetainDate = DateTime.Now;
            this.FineFees = 0;
            this.CreatedByUserID = -1;
            this.IsReleased = false;
            this.ReleaseDate = DateTime.MaxValue;
            this.ReleasedByUserID = 0;
            this.ReleaseApplicationID = -1;

            _Mode = enMode.AddNew;
        }


        public static clsDetainedLicense Find(int DetainID)
        {
            int LicenseID = -1; DateTime DetainDate = DateTime.Now;
            float FineFees= 0; int CreatedByUserID = -1;
            bool IsReleased = false; DateTime ReleaseDate = DateTime.MaxValue;
            int ReleasedByUserID = -1; int ReleaseApplicationID = -1;

            if (clsDetainedLicenseData.GetDetainedLicenseInfoByID(DetainID,
            ref LicenseID, ref DetainDate,
            ref FineFees, ref CreatedByUserID,
            ref IsReleased, ref ReleaseDate,
            ref ReleasedByUserID, ref ReleaseApplicationID))
                return new clsDetainedLicense(DetainID,
                     LicenseID,  DetainDate,
                     FineFees,  CreatedByUserID,
                     IsReleased,  ReleaseDate,
                     ReleasedByUserID,  ReleaseApplicationID);

            else
                return null;

        }

        public static DataTable GetAllDetainedLicenses()
        {
            return clsDetainedLicenseData.GetAllDetainedLicenses();
        }

        public static clsDetainedLicense FindByLicenseID(int LicenseID)
        {
            int DetainID = -1; DateTime DetainDate = DateTime.Now;
            float FineFees = 0; int CreatedByUserID = -1;
            bool IsReleased = false; DateTime ReleaseDate = DateTime.MaxValue;
            int ReleasedByUserID = -1; int ReleaseApplicationID = -1;

            if (clsDetainedLicenseData.GetDetainedLicenseInfoByLicenseID(LicenseID,
            ref DetainID, ref DetainDate,
            ref FineFees, ref CreatedByUserID,
            ref IsReleased, ref ReleaseDate,
            ref ReleasedByUserID, ref ReleaseApplicationID))
                return new clsDetainedLicense(DetainID,
                     LicenseID, DetainDate,
                     FineFees, CreatedByUserID,
                     IsReleased, ReleaseDate,
                     ReleasedByUserID, ReleaseApplicationID);

            else
                return null;

        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewDetainedLicense())
                    {

                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateDetainedLicense();

            }

            return false;
        }

        public static bool IsLicenseDetained(int LicenseID)
        {
            return clsDetainedLicenseData.IsLicenseDetained(LicenseID);
        }

        public bool ReleaseDetainedLicense(int ReleasedByUserID, int ReleaseApplicationID)
        {
            return clsDetainedLicenseData.ReleaseDetainedLicense(this.DetainID, ReleasedByUserID, ReleaseApplicationID);
        }
    }
}
