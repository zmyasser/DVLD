using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsDriver
    {

        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;


        public int DriverID { set; get; }
        public int PersonID { set; get; }
        private clsPerson _Person;
        public clsPerson PersonInfo
        {
            get
            {
                if (_Person is null && PersonID > 0)
                    _Person = clsPerson.Find(PersonID);

                return _Person;
            }

            set
            {
                _Person = value;
            }
        }
        public int CreatedByUserID { set; get; }
        public DateTime CreatedDate {  get; }

        public clsDriver()
        {
            this.DriverID = -1;
            this.PersonID = -1;
            this.CreatedByUserID = -1;
            this.CreatedDate = DateTime.Now;

            _Mode = enMode.AddNew;
        }

        public clsDriver(int DriverID, int PersonID,int CreatedByUserID, DateTime CreatedDate)
        {
            this.DriverID = DriverID;
            this.PersonID = PersonID;
            this.CreatedByUserID = CreatedByUserID;
            this.CreatedDate = CreatedDate;

            _Mode = enMode.Update;
        }

        private bool _AddNewDriver()
        {
            this.DriverID = clsDriverData.AddNewDriver( PersonID,  CreatedByUserID);
             
            return (this.DriverID != -1);
        }

        private bool _UpdateDriver()
        {
            return clsDriverData.UpdateDriver(this.DriverID,this.PersonID,this.CreatedByUserID);
        }

        public static clsDriver FindByDriverID(int DriverID)
        {
            
            int PersonID = -1; int CreatedByUserID = -1;DateTime CreatedDate= DateTime.Now; 

            if (clsDriverData.GetDriverInfoByDriverID(DriverID, ref PersonID,ref CreatedByUserID,ref CreatedDate))
                return new clsDriver(DriverID,  PersonID,  CreatedByUserID,  CreatedDate);
            else
                return null;

        }

        public static clsDriver FindByPersonID(int PersonID)
        {

            int DriverID = -1; int CreatedByUserID = -1; DateTime CreatedDate = DateTime.Now;

            if (clsDriverData.GetDriverInfoByPersonID( PersonID, ref DriverID,  ref CreatedByUserID, ref CreatedDate))
                return new clsDriver(DriverID, PersonID, CreatedByUserID, CreatedDate);
            else
                return null;

        }

        public static DataTable GetAllDrivers()
        {
            return clsDriverData.GetAllDrivers();
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewDriver())
                    {

                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateDriver();

            }

            return false;
        }

        public static DataTable GetLicenses(int DriverID)
        {
            return clsLicense.GetDriverLicenses(DriverID);
        }

        public static DataTable GetInternationalLicenses(int DriverID)
        {
            return clsInternationalLicense.GetDriverInternationalLicenses(DriverID);
        }

    }
}
