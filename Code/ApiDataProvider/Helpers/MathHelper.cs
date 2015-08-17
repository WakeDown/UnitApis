using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Security.Cryptography;

namespace DataProvider.Helpers
{
    public class MathHelper
    {
        public static byte[] GetChecksum(object obj)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, obj);
            var array = mStream.ToArray();
            var hash= MD5.Create().ComputeHash(array);
            return hash;
        }

        public static uint GetChecksum(byte[] data)
        {
            byte sum = 0;
            unchecked
            {
                sum = data.Aggregate(sum, (current, b) => (byte) (current + b));
            }
            return sum;
        }
    }
}