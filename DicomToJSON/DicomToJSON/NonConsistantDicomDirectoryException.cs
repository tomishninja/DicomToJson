using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomToJSON
{
    class NonConsistantDicomDirectoryException : Exception
    {
        public NonConsistantDicomDirectoryException() : base ("A dicom in this directory has some information that does not conform the rest of the dicoms in this directory")
        {

        }

        public NonConsistantDicomDirectoryException(string regardingInformation) : base("A dicom in this directory has some information that does not conform the rest of the dicoms in this directory, due to " + regardingInformation)
        {

        }
    }
}
