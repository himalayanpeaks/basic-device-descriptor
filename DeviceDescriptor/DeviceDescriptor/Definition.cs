using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceDescriptor.Abstract
{
    public class Definition
    {
        public enum AccessType
        {
            R,
            W,
            RW
        }
        public enum DataType
        {
            UINT,
            INT,
            Float32,
            Byte,
            BOOL,
            CHAR,
            Record,
            Array
        }
        public enum  Error
        {
            DescrptorNotFound,
        }
    }
}
