using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Text.RegularExpressions;

namespace SpecialEducationPlanning
.Business.Model.DataMigrationModel
{
    /// <summary>
    /// Zip file helper
    /// </summary>
    public class CadFileUnzip
    {
        /// <summary>
        /// Given a bytearray valid zip, finds and extracts the first file that ends with ".rom"
        /// </summary>
        /// <param name="zip">zip bytearray</param>
        /// <returns>Unziped Rom or empty array</returns>
        public byte[] UnzipPlan(byte[] zip)
        {
            return Unzip(zip, ".*[.](rom|rm2|ROM|RM2)");
        }

        /// <summary>
        /// Given a bytearra valid zip, finds and extracts the first files that ends with ".jpg" or ".jpeg"
        /// </summary>
        /// <param name="zip">zip bytearray</param>
        /// <returns>Unziped preview or empty array</returns>
        public byte[] UnzipPreview(byte[] zip)
        {
            return Unzip(zip, ".*[.](jpg|jpeg)");
        }

        /// <summary>
        /// Finds and extracts the first file that matches the regex 
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public byte[] Unzip(byte[] zip, string regex)
        {
            if (zip.Length <= 0) return zip;
            var stream = new MemoryStream(zip);
            return UnzipFromStream(stream, regex);
        }

        /// <summary>
        /// Given an stream, unzips the first file that matches a regular expressions
        /// </summary>
        /// <param name="zip">zip stream to read from</param>
        /// <param name="desiredRegex">Desired patter to match</param>
        /// <returns>Unziped content or byte[0]</returns>
        protected byte[] UnzipFromStream(Stream zip, string desiredRegex)
        {
            ZipInputStream zipInputStream = new ZipInputStream(zip);
            ZipEntry zipEntry = zipInputStream.GetNextEntry();
            while (zipEntry != null)
            {
                if (!zipEntry.IsDirectory && Regex.IsMatch(zipEntry.Name, desiredRegex))
                {
                    return this.Unzip(zipInputStream, zipEntry);
                }

                zipEntry = zipInputStream.GetNextEntry();
            }

            return new byte[0];
        }

        /// <summary>
        /// Given a zipEntry, tries to unzip the content
        /// </summary>
        /// <param name="zipStream">Source stream</param>
        /// <param name="zipEntry">Zip entry</param>
        /// <returns></returns>
        protected byte[] Unzip(Stream zipStream, ZipEntry zipEntry)
        {
            if (zipEntry.CanDecompress)
            {
                var reader = new BinaryReader(zipStream);
                var buffer = new byte[zipEntry.Size];
                reader.Read(buffer, 0, (int)zipEntry.Size);
                return buffer;
            }
            else
            {
                return new byte[0];
            }
        }
    }
}
