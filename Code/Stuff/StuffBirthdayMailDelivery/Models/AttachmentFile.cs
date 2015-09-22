using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StuffDelivery.Models
{
    public class AttachmentFile
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string DataMimeType { get; set; }
    }
}
