using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using DVLD_DataAccess;

namespace DVLD_Business
{
    public class clsTest
    {

        private enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode = enMode.AddNew;

        public int TestID { set; get; }
        public int TestAppointmentID { set; get; }

        private clsTestAppointment _TestAppointmentInfo;
        public clsTestAppointment TestAppointmentInfo
        {
            get
            {
                if (_TestAppointmentInfo is null && TestAppointmentID > 0)
                    _TestAppointmentInfo = clsTestAppointment.Find(TestAppointmentID);

                return _TestAppointmentInfo;
            }

            set
            {
                _TestAppointmentInfo = value;
            }
        }
        public bool TestResult { set; get; }
        public string Notes { set; get; }
        public int CreatedByUserID { set; get; } 

       
        private bool _AddNewTest()
        {

            this.TestID = clsTestData.AddNewTest(this.TestAppointmentID,
                this.TestResult,this.Notes,this.CreatedByUserID);
              
            return (this.TestID != -1);
        }

        private bool _UpdateTest()
        {
            return clsTestData.UpdateTest(this.TestID, this.TestAppointmentID,
                this.TestResult, this.Notes, this.CreatedByUserID);
        }


        private clsTest(int TestID,int TestAppointmentID,
            bool TestResult, string Notes, int CreatedByUserID)
        {
            this.TestID = TestID;
            this.TestAppointmentID = TestAppointmentID;
            this.TestResult = TestResult;
            this.Notes = Notes;
            this.CreatedByUserID = CreatedByUserID;

            _Mode = enMode.Update;
        }

        public clsTest()
        {
            this.TestID = -1;
            this.TestAppointmentID = -1;
            this.TestResult = false;
            this.Notes = "";
            this.CreatedByUserID = -1;

            _Mode = enMode.AddNew;
        }

        public static clsTest Find(int TestID)
        {
            int TestAppointmentID = -1;
            bool TestResult = false; string Notes = "";int CreatedByUserID = -1;

            if (clsTestData.GetTestInfoByID( TestID,
            ref  TestAppointmentID, ref  TestResult,
            ref  Notes, ref  CreatedByUserID))
            {
                return new clsTest(TestID,
                        TestAppointmentID, TestResult,
                        Notes, CreatedByUserID);
            }
            
            else
                return null;

        }

        public static clsTest FindLastTestPerPersonAndLicenseClass
            (int PersonID, int LicenseClassID, clsTestType.enTestType TestTypeID)
        {
            int TestID = -1;
            int TestAppointmentID = -1;
            bool TestResult = false; string Notes = ""; int CreatedByUserID = -1;

            if (clsTestData.GetLastTestByPersonAndTestTypeAndLicenseClass
                (PersonID,LicenseClassID,(int) TestTypeID, ref TestID,
            ref TestAppointmentID, ref TestResult,
            ref Notes, ref CreatedByUserID))
            {
                return new clsTest(TestID,
                       TestAppointmentID, TestResult,
                       Notes, CreatedByUserID);
            }

            else
                return null;

        }

        public static DataTable GetAllTests()
        {
            return clsTestData.GetAllTests();
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTest())
                    {

                        _Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateTest();

            }

            return false;
        }

        public static byte GetPassedTestCount(int LocalDrivingLicenseApplicationID)
        {
            return clsTestData.GetPassedTestCount(LocalDrivingLicenseApplicationID);
        }

        public static bool PassedAllTests(int LocalDrivingLicenseApplicationID)
        {
            // if total passed test is less than 3 it will return false, otherwise it will return true
            return GetPassedTestCount(LocalDrivingLicenseApplicationID) == 3;
        }

    }
}
