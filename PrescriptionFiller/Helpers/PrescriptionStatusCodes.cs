using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionFiller.Helpers
{
    public enum PrescriptionStatusCodes
    {
        FAX_QUEUED = 14, // fax successfully queued
        FAX_SENT = 15, // fax successfully sent
        FAX_FAILED = 16 // fax failed
    }
}
