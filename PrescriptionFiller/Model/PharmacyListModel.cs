using System;
using System.Collections.Generic;

namespace PrescriptionFiller.Model
{
	public class PharmacyListModel
	{
		public List<PharmacyInfo> list { get; set; }
        public PharmacyListModel () {
        }
        public PharmacyListModel (List<PharmacyInfo> pharmacyInfos)
		{
            list = pharmacyInfos;
//			InitDefault ();
		}

//		public void InitDefault() {
//			list.Add(new Pharmacy{ name= "Ordinary Pharmacy", faxNumber="123456789", });
//			list.Add(new Pharmacy{ name= "Cozy Pharmacy", faxNumber="987654321", });
//			list.Add(new Pharmacy{ name= "Evil Pharmacy", faxNumber="666666666", });
//			list.Add(new Pharmacy{ name= "Random Pharmacy", faxNumber="0312792740", });
//		}
	}

//	public class Pharmacy {
//		public string name { get; set; }
//		public string faxNumber { get; set; }
//	}
}

