using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PrescriptionFiller.Model;
using PrescriptionFiller.Helpers;
using RestSharp;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Xamarin.Forms;
using Newtonsoft.Json.Converters;
using PrescriptionFiller.Views.Popups;

namespace PrescriptionFiller.Services
{
	public class AccountAPIServer
	{
        private static AccountAPIServer _instance;

        public static AccountAPIServer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AccountAPIServer();

                return _instance;
            }
        }

		public delegate Task SuccessCallback<T>(T response);
        public delegate Task ErrorCallback(ServerResponseErrorCode error);

		public class ServerResponse<T>
		{
			public T data { get; set; }
			public bool error { get; set; }
			public ServerResponseErrorCode error_code { get; set; }
		}

		public enum ServerResponseErrorCode
		{
			NONE, // No error
			ERROR_NO_RECORD_FOUND = 1, // If you load a record and no record can be found
			ERROR_MISSING_REQUIRED_DATA, // If you try to make a request and you are missing a required field(s)
			ERROR_SAVING_DATA, // Server side error if the saving of a record fails for a server related issue
			ERROR_INVALID_EMAIL_ADDRESS, // Error if a invalid email address is supplied. For example, test@blah
			ERROR_INVALID_PHONE_NUMBER, // Error if a invalid phone number is provided. Has to be of the format 604-555-5555
			ERROR_INVALID_POSTAL_CODE, // Error if a invalid postal code is provided. Has to be a Canadian postal code like so V3R2L1
			ERROR_INVALID_SEX, // Invalid sex, has to be either M or F
			ERROR_EMAIL_ADDRESS_EXISTS, // The email address of an account has be unique. Error if you try to save a new account using an email that already exists in the database
			ERROR_INCORRECT_USERNAME_OR_PASSWORD, // Error if the login credentials provided are incorrect
			ERROR_NO_TOKEN_SENT, // Error if no token is provided
			ERROR_TOKEN_IS_INVALID, // Error if a token provided is invalid
            ERROR_CONNECTING_TO_FAX_SERVER, // Could not connect to SRFax to fax out the prescription
			ERROR_CONNECTION_TIMEOUT,
		}

		//const string URL_SERVER = "http://dev1.marpoletech.com/";
        const string URL_SERVER = "https://api.prescriptionfiller.com/";


        public Account Account { get; set; }

        public List<PharmacyInfo> pharmacyInfos { get; set; }

        public int Timeout { get; set; }

		public RestClient Client { get; set; }

        public string password; // A hack... this is so that password can be populated in the edit user info

		private AccountAPIServer()
		{
            InitServerConnection();
		}

		public void InitServerConnection()
		{
			Client = new RestClient (URL_SERVER);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            Account = null;
		}

        public void SaveToken(string token)
        {
            Account = new Account(token);
        }

        public void SaveAccount(string token, UserInfo userInfo)
        {
            Account = new Account(token, userInfo);
            UserInfoListenersNotifier.notifyListeners();
        }

		#region Login
        public SuccessCallback<LoginResponse> loginSuccessCallback;
        public ErrorCallback loginErrorCallback;
        public LoginResponse instanceLoginReponse;
        [Serializable]
		public class LoginResponse
		{
			public string token { get; set; }
            //public string token_type { get; set; }
            //public string expires_in { get; set; }
            //public string access_token { get; set; }
            public UserInfo user { get; set; }
            //public string refresh_token { get; set; }

        }

        public async Task Login(INavigation navigation, string email, string password, SuccessCallback<LoginResponse> successCallback, ErrorCallback errorCallback)
		{
            Account = null;
            // Pre-conditions
            if (/*email.IsValidEmail() == false*/!Validators.IsValidEmail(email) || string.IsNullOrEmpty(password))
            {
                if (errorCallback != null)
                    errorCallback(ServerResponseErrorCode.ERROR_INCORRECT_USERNAME_OR_PASSWORD);    
                return;
            }

            _instance.password = password;

//			RestRequest request = new RestRequest("user.php/login ", Method.POST);
//			request.AddParameter("email_address", email); // adds to POST or URL querystring based on Method
//            Console.WriteLine("AccountAPIService: hash " + password.Hash());
//            request.AddParameter("password", password.Hash()); // replaces matching token in request.Resource
			RestRequest request = new RestRequest("oauth/token", Method.POST);
            
            request.AddParameter("grant_type", "password");
            request.AddParameter("client_id", "2");
            request.AddParameter("client_secret", "lWxODhkqibWRX2knZqBlTLk5YrDqGTuBtgNKnyWU");

			request.AddParameter("username", email); // adds to POST or URL querystring based on Method
            request.AddParameter("password", password); // replaces matching token in request.Resource
            request.AddParameter("scope", "");
            //ExecuteRequestAsync<LoginResponse>(request, successCallback, errorCallback);
            Instance.loginSuccessCallback = successCallback;
            Instance.loginErrorCallback = errorCallback;
//            await navigation.ModalStack.ElementAt(navigation.ModalStack.Count).DisplayAlert("Alert", "test2", "OK");
            ExecuteRequestAsync<OAuthResponse>(request, authSuccessful, errorCallback);

            // await LoadingPopup.Show(navigation);
		}
        [Serializable]
        public class OAuthResponse
        {
            //public string token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string access_token { get; set; }
            //public UserInfo user { get; set; }
            public string refresh_token { get; set; }

        }
        private OAuthResponse oAuthResponse = null;
        private async Task<int> authSuccessful(OAuthResponse response)
        {
            _instance.oAuthResponse = response;
            _instance.instanceLoginReponse = new LoginResponse();
            _instance.instanceLoginReponse.token = _instance.oAuthResponse.access_token;
            if (_instance.oAuthResponse.access_token == null)
            {
                await _instance.loginErrorCallback(ServerResponseErrorCode.ERROR_INCORRECT_USERNAME_OR_PASSWORD);
                return 0;
            }
            await Task.Run(() => {
                //{
                //  "data": {
                //    "id": 3,
                //    "name": "a@a.com",
                //    "email": "a@a.com",
                //    "created_at": "2017-02-15 04:22:52",
                //    "updated_at": "2017-02-19 01:48:31",
                //    "date_of_birth": "1970-01-01",
                //    "sex": "M",
                //    "first_name": "chrometest",
                //    "last_name": "ctest2",
                //    "phone_number": "1234567890",
                //    "allergies": "1",
                //    "medical_insurance_provider": "2",
                //    "carrier_number": "3",
                //    "plan_number": "4",
                //    "member_id": "5",
                //    "issue_number": "6",
                //    "personal_health_number": "7",
                //    "shots": "8",
                //    "drugs": "9",
                //    "vaccinations": "10"
                //  },
                //  "error": false,
                //  "error_code": 0
                //}
                RestRequest request = new RestRequest("public/api/user", Method.GET);
                request.AddHeader("Authorization", "Bearer " + _instance.instanceLoginReponse.token);
                ExecuteRequestAsync<ServerResponse<UserInfoResponseData>>(
                    request, getUserInfoSuccess, _instance.loginErrorCallback);
            });
            return 0;
        }
        [Serializable]
        public class UserInfoResponseData
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string date_of_birth { get; set; }
            public char sex { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string phone_number { get; set; }
            public string allergies { get; set; }
            public string medical_insurance_provider { get; set; }
            public string carrier_number { get; set; }
            public string plan_number { get; set; }
            public string member_id { get; set; }
            public string issue_number { get; set; }
            public string personal_health_number { get; set; }
            public string shots { get; set; }
            public string drugs { get; set; }
            public string vaccinations { get; set; }
        }
        private async Task getUserInfoSuccess(ServerResponse<UserInfoResponseData> userInfoResponse)
        {   
            _instance.instanceLoginReponse.user = new UserInfo();
            _instance.instanceLoginReponse.user.id = userInfoResponse.data.id;
            _instance.instanceLoginReponse.user.email = userInfoResponse.data.email;
            _instance.instanceLoginReponse.user.created_at = userInfoResponse.data.created_at;
            _instance.instanceLoginReponse.user.updated_at = userInfoResponse.data.updated_at;
            _instance.instanceLoginReponse.user.date_of_birth = userInfoResponse.data.date_of_birth;
            _instance.instanceLoginReponse.user.sex = userInfoResponse.data.sex;
            _instance.instanceLoginReponse.user.first_name = userInfoResponse.data.first_name;
            _instance.instanceLoginReponse.user.last_name = userInfoResponse.data.last_name;
            _instance.instanceLoginReponse.user.phone_number = userInfoResponse.data.phone_number;
            // delete _instance.instanceLoginReponse.user.notes = userInfoResponse.notes;
            _instance.instanceLoginReponse.user.allergies = userInfoResponse.data.allergies;
            _instance.instanceLoginReponse.user.medical_insurance_provider = userInfoResponse.data.medical_insurance_provider;
            _instance.instanceLoginReponse.user.carrier_number = userInfoResponse.data.carrier_number;
            _instance.instanceLoginReponse.user.plan_number = userInfoResponse.data.plan_number;
            _instance.instanceLoginReponse.user.member_id = userInfoResponse.data.member_id;
            _instance.instanceLoginReponse.user.issue_number = userInfoResponse.data.issue_number;
            _instance.instanceLoginReponse.user.personal_health_number = userInfoResponse.data.personal_health_number;
            _instance.instanceLoginReponse.user.shots = userInfoResponse.data.shots;
            _instance.instanceLoginReponse.user.drugs = userInfoResponse.data.drugs;
            _instance.instanceLoginReponse.user.vaccinations = userInfoResponse.data.vaccinations;

            await _instance.loginSuccessCallback(_instance.instanceLoginReponse);
        }
        #endregion Login

        #region Logout
        public void Logout()
        {
            Account = null;
        }
        #endregion

        #region Update User info
        [Serializable]
        public class UpdateInfoResponse
        {
            public int id { get; set; }
        }

        public async Task<UserInfoValidationErrors> UpdateInfo(INavigation navigation, UserInfo newUserInfo, SuccessCallback<ServerResponse<UpdateInfoResponse>> successCallback, ErrorCallback errorCallback) {
            //{
            //  "data": {
            //    "id":3,
            //    "name":"a@a.com",
            //    "email":"a@a.com",
            //    "created_at":"2017-02-15 04:22:52",
            //    "updated_at":"2017-02-19 01:48:31",
            //    "date_of_birth":"1970-01-01",
            //    "sex":"M",
            //    "first_name":"chrometest",
            //    "last_name":"ctest2",
            //    "phone_number":"1234567890",
            //    "allergies":"1",
            //    "medical_insurance_provider":"2",
            //    "carrier_number":"3",
            //    "plan_number":"4",
            //    "member_id":"5",
            //    "issue_number":"6",
            //    "personal_health_number":"7",
            //    "shots":"8",
            //    "drugs":"9",
            //    "vaccinations":"10"
            //  },
            //  "error":false,
            //  "error_code":0
            //}
            UserInfoValidationErrors errors = Validators.GetUserInfoValidationErrors(newUserInfo, newUserInfo.password);

            //if (errors != UserInfoValidationErrors.None)
            //    return errors;

            //RestRequest request = new RestRequest("user.php/user ", Method.POST);
            RestRequest request = new RestRequest("public/api/user/" + newUserInfo.id, Method.PUT);

//            request.AddParameter("name", newUserInfo.name);
            request.AddParameter("email", newUserInfo.email);
            request.AddParameter("password", newUserInfo.password);
            //          request.AddParameter("remember_token", newUserInfo.remember_token);
            //            request.AddParameter("created_at", newUserInfo.created_at);
            //            request.AddParameter("updated_at", newUserInfo.updated_at);
            request.AddParameter("date_of_birth", newUserInfo.date_of_birth);
            request.AddParameter("sex", newUserInfo.sex);
            request.AddParameter("first_name", newUserInfo.first_name);
            request.AddParameter("last_name", newUserInfo.last_name);
            request.AddParameter("phone_number", newUserInfo.phone_number);
            request.AddParameter("allergies", newUserInfo.allergies);
            request.AddParameter("medical_insurance_provider", newUserInfo.medical_insurance_provider);
            request.AddParameter("carrier_number", newUserInfo.carrier_number);
            request.AddParameter("plan_number", newUserInfo.plan_number);
            request.AddParameter("member_id", newUserInfo.member_id);
            request.AddParameter("issue_number", newUserInfo.issue_number);
            request.AddParameter("personal_health_number", newUserInfo.personal_health_number);
            request.AddParameter("shots", newUserInfo.shots);
            request.AddParameter("drugs", newUserInfo.drugs);
            request.AddParameter("vaccinations", newUserInfo.vaccinations);

            ExecuteRequestAsync<ServerResponse<UpdateInfoResponse>>(request, successCallback, errorCallback);

            await LoadingPopup.Show(navigation);

            return errors;
            
        }
        #endregion

        #region Reset Password
        [Serializable]
        public class ResetPasswordResponse
        {
            public string token { get; set; }
            //public string token_type { get; set; }
            //public string expires_in { get; set; }
            //public string access_token { get; set; }
            public UserInfo user { get; set; }
            //public string refresh_token { get; set; }

        }
        public async Task ResetPassword(INavigation navigation, string email, SuccessCallback<ResetPasswordResponse> successCallback, ErrorCallback errorCallback)
        {
            // {email: 'myemail@email.com'}
            if (/*email.IsValidEmail() == false*/!Validators.IsValidEmail(email))
            {
                if (errorCallback != null)
                    errorCallback(ServerResponseErrorCode.ERROR_INCORRECT_USERNAME_OR_PASSWORD);
                return;
            }
            RestRequest request = new RestRequest("public/api/reset_password", Method.POST);
            request.AddParameter("email", email);

            ExecuteRequestAsync<ResetPasswordResponse>(request, successCallback, errorCallback);

            await LoadingPopup.Show(navigation);
        }
        #endregion Reset Password

        [Serializable]
        public class RetrieveUserInfoResponse
        {
        }

		#region Sign up 
  //      [Serializable]
		//public class SignUpResponse
		//{
		//	public int id { get; set; }
		//	public string token { get; set; }
		//}
			
        public async Task<UserInfoValidationErrors> SignUp(INavigation navigation, string email, string password, string confirmPassword, string date_of_birth, char sex, string first_name, string last_name, string phone_number, string notes, SuccessCallback<ServerResponse<UserInfoResponseData>> successCallback, ErrorCallback errorCallback)
		{
            //{
            //  "data":{
            //    "email":"1@1.com",
            //    "date_of_birth":"19700101",
            //    "sex":"F",
            //    "first_name":"first",
            //    "last_name":"last",
            //    "phone_number":"11111111",
            //    "allergies":"1",
            //    "medical_insurance_provider":"2",
            //    "carrier_number":"3",
            //    "plan_number":"4",
            //    "member_id":"5",
            //    "issue_number":"6",
            //    "personal_health_number":"7",
            //    "shots":"8",
            //    "drugs":"9",
            //    "vaccinations":"10",
            //    "name":"1@1.com",
            //    "updated_at":"2017-02-19 07:43:32",
            //    "created_at":"2017-02-19 07:43:32",
            //    "id":11
            //  },
            //  "error":false,
            //  "error_code":0
            //}
            UserInfoValidationErrors errors = Validators.GetUserInfoValidationErrors(email, password, confirmPassword, date_of_birth, sex, first_name, last_name, phone_number, notes);

            if (errors != UserInfoValidationErrors.None)
				return errors;

            // Request
            //RestRequest request = new RestRequest("user.php/user ", Method.POST);
            RestRequest request = new RestRequest("user", Method.POST);
            request.AddParameter("email", email); 
			request.AddParameter("password", password); 
			request.AddParameter("date_of_birth", date_of_birth); 
			request.AddParameter("sex", sex); 
			request.AddParameter("first_name", first_name); 
			request.AddParameter("last_name", last_name); 
			request.AddParameter("phone_number", phone_number); 
			request.AddParameter("notes", notes); 

			ExecuteRequestAsync<ServerResponse<UserInfoResponseData>>(request, successCallback, errorCallback);

            await LoadingPopup.Show(navigation);
				
			return errors;
		}
		#endregion

        public async Task<PrescriptionValidationErrors> CreatePrescription(INavigation navigation, 
            int user_id, int pharmacy_id, string description, string medicalNotes, string extendedHealth, string thumbnail_image_path, string image_path, 
            SuccessCallback<ServerResponse<PrescriptionRecord>> successCallback, 
            ErrorCallback errorCallback) 
        {
            PrescriptionValidationErrors errors = PrescriptionValidationErrors.None;

            //RestRequest request = new RestRequest("prescription.php/prescription", Method.POST);
            RestRequest request = new RestRequest("public/api/prescription", Method.POST);
            //request.AddHeader("contentType", "application/x-www-form-urlencoded; charset=utf-8");
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddParameter("user_id", user_id);
            request.AddParameter("pharmacy_id", pharmacy_id);
            request.AddParameter("description", description);
            request.AddParameter("extended_health", extendedHealth);
            request.AddParameter("fax_id", ""); // ?
            request.AddParameter("medical_notes", medicalNotes);
            request.AddParameter("image_path", image_path);
            //request.AddParameter("image_data", getBase64String(image_path));
            request.AddFile("image", thumbnail_image_path);
//            request.AddParameter("contentType", "application/x-www-form-urlencoded; charset=utf-8");

            ExecuteRequestAsync<ServerResponse<PrescriptionRecord>>(request, successCallback, errorCallback, 50000);

            await LoadingPopup.Dismiss(navigation);
            await LoadingPopup.Show(navigation);

            return errors;
        }   

        private String getBase64String(String image_path)
        {
            string base64String;
            using (var fs = new FileStream(image_path, FileMode.Open, FileAccess.Read))
            {
                var imageData = new byte[fs.Length];
                fs.Read(imageData, 0, (int)fs.Length);
                base64String = Convert.ToBase64String(imageData);
            }
            return base64String;
            ;
        }

        [Serializable]
        public class PrescriptionRecord
        {
            public int id { get; set; }
            public int user_id { get; set; }
            public int pharmacy_id { get; set; }
            public string description { get; set; }
            public string image_path { get; set; }
            public string fax_status { get; set; }
            public string fax_id { get; set; }
            public string extended_health { get; set; }
            public string medical_notes { get; set; }
            public string created_at { get; set; }
            //"id": 15,
            //"user_id": "3",
            //"pharmacy_id": "2",
            //"description": null,
            //"extended_health": null,  
            //"image_path": "/storage/emulated/0/Android/data/com.pixelmatic.prescriptionfiller/files/Pictures/Prescriptions/prescription_2017_2_18_9_49_31_617_1.jpg",
            //"fax_id": null,
            //"updated_at": "2017-02-18 22:22:49",
            //"created_at": "2017-02-18 22:22:49",
            //"medical_notes": null,
            //"fax_status": null
        }

        [Serializable]
        public class PharmaciesInfoResponse
        {
            public int id { get; set; }
            public string name { get; set; }
            public string phone_number { get; set; }
            public string fax_number { get; set; }
            public string address { get; set; }
            public string zip_code { get; set; }
        }

        public async Task RetrievePharmaciesById(INavigation navigation, int pharmacyId, SuccessCallback<PharmaciesInfoResponse> successCallback, ErrorCallback errorCallback)
        {
            // Pre-conditions
            if (Account == null)
            {
                if (errorCallback != null)
                    await errorCallback(ServerResponseErrorCode.ERROR_TOKEN_IS_INVALID);
                return;
            }
            RestRequest request = new RestRequest("public/api/pharmacy/" + pharmacyId, Method.GET);
            ExecuteRequestAsync<PharmaciesInfoResponse>(request, successCallback, errorCallback);
        }

        public async Task RetrievePharmaciesByCityAndName(INavigation navigation, string city, string pharmacyName, SuccessCallback<ServerResponse<PharmaciesInfoResponse[]>> successCallback, ErrorCallback errorCallback)
        {
            // Pre-conditions
            if (Account == null)
            {
                if (errorCallback != null)
                    await errorCallback(ServerResponseErrorCode.ERROR_TOKEN_IS_INVALID);    
                return;
            }

            //RestRequest request = new RestRequest("pharmacy.php/name/" + (pharmacyName != null ? pharmacyName : ""), Method.GET);
            RestRequest request = new RestRequest("public/api/pharmacy/name/" + ((pharmacyName != null && !pharmacyName.Equals(""))? pharmacyName : "null") + 
                                                  "/city/" + 
                                                  ((city != null && !city.Equals("")) ? city : "null")
                                                  , Method.GET);
            //{
            //  "data":[
            //    {
            //	  "id":2,
            //	  "phone_number":"111-1111",
            //	  "fax_number":"222-2222",
            //	  "address":"test address",
            //	  "zip_code":"v3f5g5",
            //	  "name":"test pharmacy1"
            //	},
            //	{
            //	  "id":3,
            //	  "phone_number":"121-1111",
            //	  "fax_number":"232-2222",
            //	  "address":"test address",
            //	  "zip_code":"v3f5g5",
            //	  "name":"test pharmacy2"
            //	}
            //  ],
            //  "error":false,
            //  "error_code":0
            //}
            ExecuteRequestAsync<ServerResponse<PharmaciesInfoResponse[]>>(request, successCallback, errorCallback);
//            await LoadingPopup.Show(navigation);
        }

        public async Task RetrievePharmaciesByLocation(INavigation navigation, string longitude, string latitude, SuccessCallback<ServerResponse<PharmaciesInfoResponse[]>> successCallback, ErrorCallback errorCallback)
        {
            // Pre-conditions
            if (Account == null)
            {
                if (errorCallback != null)
                    await errorCallback(ServerResponseErrorCode.ERROR_TOKEN_IS_INVALID);
                return;
            }
            // public/api/pharmacy/latitude/49.2765/longitude/-123.2177
            RestRequest request = new RestRequest("public/api/pharmacy/latitude/" + latitude +
                                                  "/longitude/" +
                                                  longitude
                                                  , Method.GET);
            //{
            //  "data":[
            //    {
            //    "id":2,
            //    "phone_number":"111-1111",
            //    "fax_number":"222-2222",
            //    "address":"test address",
            //    "zip_code":"v3f5g5",
            //    "name":"test pharmacy1"
            //  },
            //  {
            //    "id":3,
            //    "phone_number":"121-1111",
            //    "fax_number":"232-2222",
            //    "address":"test address",
            //    "zip_code":"v3f5g5",
            //    "name":"test pharmacy2"
            //  }
            //  ],
            //  "error":false,
            //  "error_code":0
            //}
            ExecuteRequestAsync<ServerResponse<PharmaciesInfoResponse[]>>(request, successCallback, errorCallback);
            //            await LoadingPopup.Show(navigation);
        }

        public async Task RetrievePrescriptionById(INavigation navigation, int prescription_id, SuccessCallback<PrescriptionRecord> successCallback, ErrorCallback errorCallback)
        {
            // Pre-conditions
            if (Account == null)
            {
                if (errorCallback != null)
                    await errorCallback(ServerResponseErrorCode.ERROR_TOKEN_IS_INVALID);    
                return;
            }

            //RestRequest request = new RestRequest("prescription.php/prescription_id/" + (prescription_id > 0 ? prescription_id.ToString() : "0"), Method.GET);
            RestRequest request = new RestRequest("public/api/prescription/" + (prescription_id > 0 ? prescription_id.ToString() : "0"), Method.GET);

            ExecuteRequestAsync<PrescriptionRecord>(request, successCallback, errorCallback);

            await LoadingPopup.Show(navigation);
        }

        public async Task RetrievePrescriptionsByUserId(INavigation navigation, int user_id, SuccessCallback<ServerResponse<PrescriptionRecord[]>> successCallback, ErrorCallback errorCallback)
        {
            // Pre-conditions
            if (Account == null)
            {
                if (errorCallback != null)
                    await errorCallback(ServerResponseErrorCode.ERROR_TOKEN_IS_INVALID);    
                return;
            }

            //RestRequest request = new RestRequest("prescription.php/user_id/" + (user_id > 0 ? user_id.ToString() : "0"), Method.GET);
            RestRequest request = new RestRequest("public/api/prescription/user_id/" + (user_id > 0 ? user_id.ToString() : "0"), Method.GET);

            ExecuteRequestAsync<ServerResponse<PrescriptionRecord[]>>(request, successCallback, errorCallback);

            await LoadingPopup.Show(navigation);
        }

        private void logRequest(RestRequest request)
        {
            Console.WriteLine("sending to " + request.Resource);
            foreach (Parameter param in request.Parameters)
            {
                Console.WriteLine("param type: " + param.GetType());
                Console.WriteLine("(name,value)" + "(" + param.Name + "," + param.Value + ")");
            }
        }

        private void ExecuteRequestAsync<T>(RestRequest request, SuccessCallback<T> successCallback, ErrorCallback errorCallback)
        {
            ExecuteRequestAsync(request, successCallback, errorCallback, 5000); // 5 seconds timeout
        }

        private void ExecuteRequestAsync<T>(RestRequest request, SuccessCallback<T> successCallback, ErrorCallback errorCallback, int timeout)
        {
            Client.Timeout = Client.ReadWriteTimeout = timeout;
            request.Timeout = request.ReadWriteTimeout = timeout;
            if (Account != null && !request.Resource.Contains("oauth/token"))
                request.AddHeader("Authorization", "Bearer " + Account.token);

            logRequest(request);

            // async with deserialization
            Client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            Client.ExecuteAsync(request, response => {

                // The internal deserializer of RestSharp doesn't work with ServerResponse<T>
                // So we use the deserializer from Newtonsoft
                //if (request.Resource.Equals("api/prescription")) {
                //    HomePage.Instance.DisplayAlert("response for create prescription","invoked","cancel");
                //    HomePage.Instance.DisplayAlert("response for create prescription", "response.ResponseUri:" + response.ResponseUri, "cancel");
                //    HomePage.Instance.DisplayAlert("response for create prescription", "response.Content: " + response.Content, "cancel");
                //}
                Console.WriteLine("request.Resource: " + request.Resource);
                Console.WriteLine("response.ResponseUri:" + response.ResponseUri);
                Console.WriteLine("response.Content: " + response.Content);
                Console.WriteLine("response.ErrorMessage" + response.ErrorMessage);
                Console.WriteLine("response.ErrorException" + response.ErrorException);
                Console.WriteLine("response.StatusCode" + response.StatusCode);
                Console.WriteLine("response.ResponseStatus" + response.ResponseStatus);

                //ServerResponse<T> serverReponse = JsonConvert.DeserializeObject<ServerResponse<T>>(response.Content);
                T serverReponse = JsonConvert.DeserializeObject<T>(response.Content,new JsonSerializerSettings {
                    DateFormatString = "yyyy-MM-dd\nHH:mm:ss"
                });

                // Timeout
                if (response.StatusCode == HttpStatusCode.RequestTimeout || response.ResponseStatus == ResponseStatus.TimedOut)
                {
                    if (errorCallback != null)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await errorCallback(ServerResponseErrorCode.ERROR_CONNECTION_TIMEOUT);
                        });
                    }

                    return;
                }

                if (serverReponse == null)
                {
                    Console.WriteLine("!!! serverResponse is null !!!");
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await errorCallback(ServerResponseErrorCode.ERROR_CONNECTION_TIMEOUT);
                    });
                    return;
                }

                // No errors
                //if (serverReponse.error == false)
                //{
                    if (successCallback != null)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            //await successCallback(serverReponse.data);
                            await successCallback(serverReponse);
                        });
                    }
                //}

                // Erros 
                //else
                //{
                //    if (errorCallback != null)
                //    {
                //        Device.BeginInvokeOnMainThread(async () =>
                //        {
                //            await errorCallback(serverReponse.error_code);
                //        });
                //    }
                //}
            });
        }
    }

    public static class APIExtenstions
	{
        public static bool Contains(this UserInfoValidationErrors errors, UserInfoValidationErrors error)
		{
			return (errors & error) == error;
		}

        // SHA-1 Hash
		public static string Hash(this string password)
		{
            // Should not be modified once production has started
			string salt = "gt&ug09";

			byte[] data = Encoding.ASCII.GetBytes(salt + password);

			SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
			byte[] sha1data = sha1.ComputeHash(data);

            return new ASCIIEncoding().GetString(sha1data);
		}

	}
}

