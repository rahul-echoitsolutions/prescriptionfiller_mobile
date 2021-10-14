using System;
using System.Collections.Generic;
using System.Text;

namespace PrescriptionFiller.Interfaces
{
    public interface IResizing
    {
        byte[] ResizeImage(byte[] imageData, float width, float height);
        byte[] ResizeImage(byte[] imageData, float sizeDesired);
    }
}
