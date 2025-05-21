using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceDescriptor.IoLink.Source
{
    class IoddFinderResponse
    {      
            public string? ProductName { get; set; }
            public string? DownloadUrl { get; set; }
            public string? VendorName { get; set; }
            public string? DeviceId { get; set; }        
    }
}
