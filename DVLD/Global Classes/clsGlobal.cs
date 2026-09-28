using DVLD_Business;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DVLD.Classes
{
    internal static  class clsGlobal
    {
        public static clsUser CurrentUser;

        public static bool RememberUsernameAndPassword(string UsernameVal, string PasswordVal)
        {

            string keyPath = @"HKEY_CURRENT_USER\SOFTWARE\DVLD_Program";

            string UserName = "Username";
            string UsernameValue = UsernameVal;

            string Password = "Password";
            string PasswordValue = PasswordVal;


            try
            {
                // Write the value to the Registry
                Registry.SetValue(keyPath, UserName, UsernameValue, RegistryValueKind.String);
                Registry.SetValue(keyPath, Password, PasswordValue, RegistryValueKind.String);
                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }


        }

        public static bool GetStoredCredential(ref string Username, ref string Password)
        {
            string keyPath = @"HKEY_CURRENT_USER\SOFTWARE\DVLD_Program";

            try
            {
                // Read the value from the Registry
                Username = Registry.GetValue(keyPath, "Username", null) as string;
                Password = Registry.GetValue(keyPath, "Password", null) as string;
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }

        }
    }
}
