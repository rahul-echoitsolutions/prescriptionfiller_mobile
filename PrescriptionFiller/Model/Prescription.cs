using System;
using System.IO;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using PrescriptionFiller.Helpers;
using PrescriptionFiller.Database;

namespace PrescriptionFiller
{
	public class Prescription : Image
	{
        public class PrescriptionSaveTimer : System.Timers.Timer {
            public PrescriptionSaveTimer(double interval) : base(interval) {

            }
            public Prescription prescription { get; set; }
        }

        private PrescriptionItem _prescriptionItem;
        public bool dataChanged; 
        PrescriptionSaveTimer timer;

        public PrescriptionItem prescriptionItem {
            get { return _prescriptionItem; }
            set { _prescriptionItem = value; }
        }

        public string PrescriptionStatus {
            get {
                if (_prescriptionItem == null)
                {
                    return null;
                }
                else
                {
                    string result = null;
                    if (_prescriptionItem.status == null) {
                        result = null;
                    } else { 
                        if (_prescriptionItem.status.Equals(((int)(PrescriptionStatusCodes.FAX_SENT)).ToString())) {
                            result = "FAX SENT";
                        }
                        if (_prescriptionItem.status.Equals(((int)(PrescriptionStatusCodes.FAX_QUEUED)).ToString()))
                        {
                            result = "FAX QUEUED";
                        }
                        if (_prescriptionItem.status.Equals(((int)(PrescriptionStatusCodes.FAX_FAILED)).ToString()))
                        {
                            result = "FAX FAILED";
                        }
                    }
                    //                    !itemStatus.Equals(PrescriptionStatus.FAX_SENT.ToString()) &&
                    //                    !itemStatus.Equals(PrescriptionStatus.FAX_FAILED.ToString()) &&
                    return result;
                }
            }
//            set {
//                _prescriptionItem.status = value;
//                //OnPropertyChanged("PrescriptionDescription");
//            }
        }

        public bool isNotSent
        {
            get {
                if (_prescriptionItem.status != null && _prescriptionItem.status.Equals(((int)PrescriptionStatusCodes.FAX_SENT).ToString()))
                {
                    return false;
                } else
                {
                    return true;
                }
                    
            }
        }

        public string PrescriptionDescription {
            get {
                if (_prescriptionItem == null)
                {
                    return null;
                }
                else
                {
                    return _prescriptionItem.description;
                }
            }
            set {
                _prescriptionItem.description = value;
                timedSave();
                //OnPropertyChanged("PrescriptionDescription");
            }
        }
        
        public string MedicalNotes
        {
            get
            {
                if (_prescriptionItem == null)
                {
                    return null;
                }
                else
                {
                    return _prescriptionItem.medicalNotes;
                }
            }
            set
            {
                _prescriptionItem.medicalNotes = value;
                timedSave();
                //OnPropertyChanged("PrescriptionDescription");
            }
        }

        public string ExtendedHealth {
            get {
                if (_prescriptionItem == null)
                {
                    return null;
                }
                else
                {
                    return _prescriptionItem.extendedHealth;
                }
            }
            set {
                _prescriptionItem.extendedHealth = value;
                timedSave();
                //OnPropertyChanged("PrescriptionDescription");
            }
        }

        private void timedSave() {
            //if (timer != null)
            //{
            //    timer.Dispose();
            //    timer = null;
            //}
            //timer = new PrescriptionSaveTimer(500);
            //timer.prescription = this;
            //timer.Elapsed += savePrescriptionToLocalDb;
            //timer.Start();
            //HomePage.Instance.DisplayAlert("triggered save to local db savePrescriptionToLocalDb: ", "invoked", "cancel");
            LocalPrescriptionDatabase.Instance.SaveItem(this.prescriptionItem);
            //HomePage.Instance.DisplayAlert("triggered save to local db savePrescriptionToLocalDb: ", "done", "cancel");
        }

        private void savePrescriptionToLocalDb(Object sender, System.Timers.ElapsedEventArgs e) {
            //HomePage.Instance.DisplayAlert("triggered save to local db savePrescriptionToLocalDb: ", "invoked","cancel");
            PrescriptionSaveTimer prescriptionSaveTimer = (PrescriptionSaveTimer)sender;
            //Prescription prescription = prescriptionSaveTimer.prescription;
            LocalPrescriptionDatabase.Instance.SaveItem(this.prescriptionItem);
            //Console.WriteLine("saved prescription to Local DB " + this.PrescriptionDescription);
            //HomePage.Instance.DisplayAlert("triggered save to local db savePrescriptionToLocalDb: ", "done", "cancel");
            timer.Dispose();
        }

        private int _borderRadius;
        public int BorderRadius 
        {
            get 
            {
                return _borderRadius;
            }
            set 
            {
                _borderRadius = value;
                OnPropertyChanged("BorderRadius");
            }
        }

		public string PrescriptionPath
		{
            get { 
                if (_prescriptionItem == null)
                {
                    return null;
                }
                else
                {
                    return _prescriptionItem.path;
                }
            }
		}

		public ImageSource PrescriptionImageSource
		{
			get { 
                if (_prescriptionItem == null || _prescriptionItem.path == null)
                {
                    return null;
                }
                else
                {
                    return ImageSource.FromFile(_prescriptionItem.path);
                }
            }
		}

		public ImageSource PrescriptionThumbImageSource
		{
            get { 
                if (_prescriptionItem == null || _prescriptionItem.thumbPath == null)
                {
                    return null;
                }
                else
                {
                    return ImageSource.FromFile(_prescriptionItem.thumbPath);
                }
            }
		}

        public Prescription (PrescriptionItem prescriptionItem)
		{
            this._prescriptionItem = prescriptionItem;
		}

	}
}

