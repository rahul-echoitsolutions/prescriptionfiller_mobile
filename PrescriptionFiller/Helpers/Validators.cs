using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;

using PrescriptionFiller.Model;

namespace PrescriptionFiller.Helpers
{
    [Flags]
    public enum UserInfoValidationErrors 
    {
        None = 0,
        InvalidEmail = 1,
        EmptyPassword = 2,
        InconsistentPassword = 66,
        InvalidDateOfBirth = 4,
        InvalidSex = 8,
        EmptyFirstName = 16,
        EmptyLastName = 32,
        InvalidPhoneNumber = 64,
    }

    [Flags]
    public enum PrescriptionValidationErrors
    {
        None = 0,
        InvalidPrescription = 1,
    }

    public class Validators
    {
    
        public static bool IsValidPhoneNumber(string phoneNumber) {
            return Regex.IsMatch(phoneNumber, "[0-9]{3}-[0-9]{3}-[0-9]{4}");
        }

        public static bool IsValidSex(char sex) {
            return sex == 'M' || sex == 'F';            
        }

        public static bool IsValidDateOfBirth(string dateOfBirth) {
            return Regex.IsMatch(dateOfBirth, "(19|20)[0-9]{2}-(0|1)[0-9]-[0-3][0-9]");
        }

        public static bool IsValidFirstName(string firstName) {
            return !string.IsNullOrEmpty(firstName);
        }

        public static bool IsValidLastName(string lastName) {
            return !string.IsNullOrEmpty(lastName);
        }

        public static bool IsValidPassword(string password) {
            return !string.IsNullOrEmpty(password);
        }

        public static bool IsValidUserInfo(UserInfo userInfo) {
            return IsValidDateOfBirth(userInfo.date_of_birth) 
                && IsValidSex(userInfo.sex)
                && IsValidPhoneNumber(userInfo.phone_number)
                && IsValidFirstName(userInfo.first_name)
                && IsValidLastName(userInfo.last_name)
                && IsValidEmail(userInfo.email);
        }

        public static bool IsValidPasswordConfirmPassword(string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(confirmPassword))
            {
                return false;
            }
            if (!string.Equals(password,confirmPassword))
            {
                return false;
            }
            return true;
        }

        public static UserInfoValidationErrors GetUserInfoValidationErrors(UserInfo userInfo, string confirmPassword) {
            UserInfoValidationErrors errors = UserInfoValidationErrors.None;

            if(!IsValidDateOfBirth(userInfo.date_of_birth)) {
                errors = UserInfoValidationErrors.InvalidDateOfBirth;
            }

            if(!IsValidPassword(userInfo.password)) {
                errors = UserInfoValidationErrors.EmptyPassword;
            }

            if(!IsValidSex(userInfo.sex)) {
                errors = UserInfoValidationErrors.InvalidSex;
            }

            if(!IsValidPhoneNumber(userInfo.phone_number)) {
                errors = UserInfoValidationErrors.InvalidPhoneNumber;
            }

            if(!IsValidFirstName(userInfo.first_name)) {
                errors = UserInfoValidationErrors.EmptyFirstName;
            }

            if(!IsValidLastName(userInfo.last_name)) {
                errors = UserInfoValidationErrors.EmptyLastName;
            }

            if(!IsValidEmail(userInfo.email)) {
                errors = UserInfoValidationErrors.InvalidEmail;
            }
            if(!IsValidPasswordConfirmPassword(userInfo.password,confirmPassword))
            {
                errors = UserInfoValidationErrors.InconsistentPassword;
            }

            return errors;
        }

        public static UserInfoValidationErrors GetUserInfoValidationErrors(string email, string password, string confirmPassword, string date_of_birth, char sex, string first_name, string last_name, string phone_number, string notes) {
            return GetUserInfoValidationErrors(new UserInfo() {
                email = email,
                password = password,
                date_of_birth = date_of_birth,
                sex = sex,
                first_name = first_name,
                last_name = last_name, 
                phone_number = phone_number,
            }, confirmPassword);
        }
            

        public static bool IsValidEmail(string email)
        {
            try
            {
                new MailAddress(email);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

