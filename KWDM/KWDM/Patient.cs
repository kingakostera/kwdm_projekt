using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KWDM
{
    class Patient
    {
        string collection;

        public string Collection
        {
            get { return collection; }
            set { collection = value; }
        }
        string patientID;

        public string PatientID
        {
            get { return patientID; }
            set { patientID = value; }
        }
        string studyDate;

        public string StudyDate
        {
            get { return studyDate; }
            set { studyDate = value; }
        }
        string studyDescription;

        public string StudyDescription
        {
            get { return studyDescription; }
            set { studyDescription = value; }
        }
        string modality;

        public string Modality
        {
            get { return modality; }
            set { modality = value; }
        }
        string seriesDescription;

        public string SeriesDescription
        {
            get { return seriesDescription; }
            set { seriesDescription = value; }
        }
        string manufacturer;

        public string Manufacturer
        {
            get { return manufacturer; }
            set { manufacturer = value; }
        }
        string manufacturerModel;

        public string ManufacturerModel
        {
            get { return manufacturerModel; }
            set { manufacturerModel = value; }
        }
        string softwareVersion;

        public string SoftwareVersion
        {
            get { return softwareVersion; }
            set { softwareVersion = value; }
        }
        string seriesUID;

        public string SeriesUID
        {
            get { return seriesUID; }
            set { seriesUID = value; }
        }



    }
}
