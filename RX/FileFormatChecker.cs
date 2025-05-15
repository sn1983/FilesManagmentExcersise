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

    /// <summary>
    /// Provides methods to check the format of files for specific patterns.
    /// </summary>
    public static class FileFormatChecker
    {
        /// <summary>
        /// Determines whether the specified file matches the expected flag file format.
        /// The file must exist and its first two lines must match the required patterns:
        /// - Line 1: "Time Stamp: yyyyMMdd_HHmmss"
        /// - Line 2: "Service Name: T(x)_Service"
        /// </summary>
        /// <param name="filePath">The path to the file to check.</param>
        /// <returns>
        /// True if the file exists and matches the flag file format; otherwise, false.
        /// </returns>
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
