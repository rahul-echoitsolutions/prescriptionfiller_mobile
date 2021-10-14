using System;

namespace PrescriptionFiller
{
    public class PharmacyInfo
    {
        public int pharmacyId { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string faxNumber { get; set; }
        public string address { get; set; }
        public string zipCode { get; set; }
        public PrescriptionItem prescriptionItem { get; set; }
    }
}

