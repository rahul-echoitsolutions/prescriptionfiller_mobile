using System;
using PrescriptionFiller;
using Xamarin.Forms;
using PrescriptionFiller.Droid.Services;
using System.IO;

//.... square bracket assembly: Dependency (typeof (DroidSQLite)) square bracket

namespace PrescriptionFiller.Droid.Services
{
    public class DroidSQLiteDeleteMe// : ISQLite
    {
        public DroidSQLiteDeleteMe()
        {
        }

//        #region ISQLite implementation
        //public SQLite.SQLiteConnection GetConnection ()
        //{
        //    var sqliteFilename = "PrescriptionFillerSQLite.db3";
        //    string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal); // Documents folder
        //    var path = Path.Combine(documentsPath, sqliteFilename);

        //    var conn = new SQLite.SQLiteConnection(path);

        //    // Return the database connection 
        //    return conn;
        //}
        //#endregion

        /// <summary>
        /// helper method to get the database out of /raw/ and into the user filesystem
        /// </summary>
        void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}