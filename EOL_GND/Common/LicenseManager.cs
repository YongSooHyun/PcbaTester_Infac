using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Common
{
    internal class LicenseManager
    {
        /// <summary>
        /// 입력한 License key가 올바른 것인지 검사한다.
        /// </summary>
        /// <param name="licenseKey"></param>
        /// <returns></returns>
        internal static bool CheckLicenseKey(string licenseKey)
        {
            if (string.IsNullOrEmpty(licenseKey))
            {
                return false;
            }

            var licenseBytes = Convert.FromBase64String(licenseKey);
            var requestCode = GetRequestCode();
            var requestBytes = Encoding.UTF8.GetBytes(requestCode);

            // Verify using the public key.
            var publicKey = Properties.Resources.PublicKey;
            var rsaCsp = new RSACryptoServiceProvider();
            rsaCsp.ImportCspBlob(publicKey);
            return rsaCsp.VerifyData(requestBytes, SHA256.Create(), licenseBytes);
        }

        /// <summary>
        /// 하드웨어 유일 ID의 해시값을 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal static string GetRequestCode()
        {
            // Motherboard S/N 이 없으면 processor ID를 unique hardware ID로 이용한다.
            var uniqueHwId = GetBaseBoardSerialNumber();
            if (string.IsNullOrEmpty(uniqueHwId))
            {
                uniqueHwId = GetProcessorId();
            }

            if (string.IsNullOrEmpty(uniqueHwId))
            {
                return null;
            }

            // SHA1 hash.
            var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(uniqueHwId));
            return string.Join("", hash.Select(x => $"{x:X2}"));
        }

        private static string GetBaseBoardSerialNumber()
        {
            var baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT SerialNumber FROM Win32_BaseBoard");
            var resultObjects = baseboardSearcher.Get();
            foreach (ManagementObject managementObj in resultObjects)
            {
                return managementObj["SerialNumber"]?.ToString();
            }

            return null;
        }

        private static string GetProcessorId()
        {
            var baseboardSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT ProcessorId FROM Win32_Processor");
            var resultObjects = baseboardSearcher.Get();
            foreach (ManagementObject managementObj in resultObjects)
            {
                return managementObj["ProcessorId"]?.ToString();
            }

            return null;
        }
    }
}
