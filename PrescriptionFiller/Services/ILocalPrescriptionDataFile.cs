using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionFiller.Services
{
    interface ILocalPrescriptionDataFile
    {
        IEnumerable<PrescriptionItem> getPrescriptionItems();
        int savePrescriptionItem(PrescriptionItem prescriptionItem);
    }
}
