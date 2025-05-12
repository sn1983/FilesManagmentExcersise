using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RX
{
    using System.IO;
    using System.Text.RegularExpressions;

    public static class FileFormatChecker
    {
        public static bool IsFileFlagFormat(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line1 = reader.ReadLine();
                    string line2 = reader.ReadLine();

                    if (string.IsNullOrEmpty(line1) || string.IsNullOrEmpty(line2))
                        return false;

                    string timeStampPattern = @"^Time Stamp:\s*\d{8}_\d{6}$";
                    string serviceNamePattern = @"^Service Name:\s*T\(x\)_Service$";

                    return Regex.IsMatch(line1, timeStampPattern) &&
                           Regex.IsMatch(line2, serviceNamePattern);
                }
            }
            catch
            {
                return false;
            }
        }
    }

}
