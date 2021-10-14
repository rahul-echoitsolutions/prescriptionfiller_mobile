using System;

using Newtonsoft.Json;

namespace PrescriptionFiller.Model
{
	public class Account{
		public string token{ get; set;}
		public UserInfo user_info{get;set;}
        public Account(string token) {
            this.token = token;
        }
        public Account(string token, UserInfo userInfo){
			this.token = token;
            this.user_info = userInfo;
		}
	}

	public class UserInfo{
		public int id { get; set; }
		public string email { get; set; }
		public string password { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string medical_insurance_provider { get; set; }
        public string carrier_number { get; set; }
        public string plan_number { get; set; }
        public string member_id { get; set; }
        public string issue_number { get; set; }
        public string personal_health_number { get; set; }
        public string shots { get; set; }
        public string drugs { get; set; }
        public string vaccinations { get; set; }
        public DateTime dateObj_DOB {
            get {
                if (date_of_birth != null && !date_of_birth.Trim().Equals(""))
                {
                    DateTime dob;
                    dob = DateTime.Parse(date_of_birth);
                    return dob;
                }
                else
                {
                    DateTime dob;
                    dob = DateTime.Now;
                    return dob;
                }
            }
            set { 
                if (value != null) {
                    string year = value.Year.ToString();
                    string month = value.Month.ToString().PadLeft(2, '0');
                    string day = value.Day.ToString().PadLeft(2, '0');
                    this.date_of_birth = 
                        year + "-" + month + "-" + day;
                }
            }
        }
		public string date_of_birth { get; set; }
		public char sex { get; set; }
		public string first_name{ get; set; }
		public string last_name { get; set; }
		public string phone_number { get; set; }
        // todo remove me after API field is removed
//		public string notes {get;set;}
        public string allergies { get; set; }
        public string medicalInsuranceProvider { get; set; }
        public string carrierNumber { get; set; }
        public string planNumber { get; set; }
        public string memberId { get; set; }
        public string issueNumber { get; set; }
        public string personalHealthNumber { get; set; }
        public bool isAccountInfoEmpty()
        {
            bool result = false;
            if (allergies == null && medicalInsuranceProvider == null &&
                carrierNumber == null && planNumber == null && memberId == null && issueNumber == null &&
                personalHealthNumber == null)
            {
                result = true;
            }
            return result;
        }
        public void copyTo(UserInfo _userInfo) {
            _userInfo.id = this.id;
            _userInfo.email = this.email;
            _userInfo.password = this.password;
            _userInfo.created_at = this.created_at;
            _userInfo.updated_at = this.updated_at;
            _userInfo.medical_insurance_provider = this.medical_insurance_provider;
            _userInfo.carrier_number = this.carrier_number;
            _userInfo.plan_number = this.plan_number;
            _userInfo.member_id = this.member_id;
            _userInfo.issue_number = this.issue_number;
            _userInfo.personal_health_number = this.personal_health_number;
            _userInfo.shots = this.shots;
            _userInfo.drugs = this.drugs;
            _userInfo.vaccinations = this.vaccinations;
            _userInfo.date_of_birth = this.date_of_birth;
            _userInfo.sex = this.sex;
            _userInfo.first_name = this.first_name;
            _userInfo.last_name = this.last_name;
            _userInfo.phone_number = this.phone_number;
            _userInfo.allergies = this.allergies;
            _userInfo.medicalInsuranceProvider = this.medicalInsuranceProvider;
            _userInfo.carrierNumber = this.carrierNumber;
            _userInfo.planNumber = this.planNumber;
            _userInfo.memberId = this.memberId;
            _userInfo.issueNumber = this.issueNumber;
            _userInfo.personalHealthNumber = this.personalHealthNumber;
        }
    }


}

