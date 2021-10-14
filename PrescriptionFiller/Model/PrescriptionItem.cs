using System;
//using SQLite;

namespace PrescriptionFiller
{
    public class PrescriptionItem
    {
        public PrescriptionItem()
        {
        }

        //[PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int serverPrescriptionId { get; set; }
        public int userId { get; set; }
        public string path { get; set; }
        public string thumbPath { get; set; }
        public string description { get; set; }
        public string medicalNotes { get; set; }
        public string extendedHealth { get; set; }
        public string status { get; set; }
        public string photoTakenTime { get; set; }
        public string createdDt { get; set; }

        public int pharmacyId { get; set; }
        public string pharmacyName { get; set; }
        public string pharmacyPhoneNumber { get; set; }
        public string pharmacyFaxNumber { get; set; }
        public string pharmacyAddress { get; set; }
        public string pharmacyZipCode { get; set; }
    }
}

