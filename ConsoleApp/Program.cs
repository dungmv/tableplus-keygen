using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;

namespace RNCryptor
{
    class Program
    {
        public static string HomePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\com.tinyapp.TablePlus";
        public static string GetLicenseCredentialPass() => "JfatmQbk2rqpWjjsH8XgG7eNa9BzhyNRLdZmChV9KeJ53ZHmk9pdnsJt7NsQrvkpkyQfGVyqU9LASqQgWUSYAKHG3kZpRgUBjRLPmVwfAjVXHkWhtBp9vQ7vs7akn3Dvj2ZBXcdKfQM7y4yeYJ2tHa7nzYKXv2KaBE5XacgZGPNzJb7mbLtTZepg9LjwTv4RYCvL3ZEqBNY8tr4xsatv6y9xhGddvUqsEsJz8mDh9d4tgVjjqRq98g7V2hLb82ju";
        private static readonly string oldLicensePath = HomePath + "\\.licensewin";
        private static readonly string newLicensePath = HomePath + "\\.licensewindows";
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\com.tinyapp.TablePlus");
            Debug.WriteLine(GetDeviceIDs().FirstOrDefault<string>());
            var deviceID = GetDeviceIDs().FirstOrDefault<string>();
            var updatesAvailableUntil = new DateTime();
            updatesAvailableUntil.AddYears(10);
            Write(new Dictionary<string, string>()
            {
                {
                    "deviceID",
                    deviceID
                },
                {
                    "updatesAvailableUntil",
                    updatesAvailableUntil.ToString()
                },
                {
                    "email",
                    "maivandung91@gmail.com"
                },
                {
                    "sign",
                    "xxx"
                }
            });
        }

        private static void Write(Dictionary<string, string> dict)
        {
            string licenseCredentialPass = GetLicenseCredentialPass();
            WriteFile(new Encryptor().Encrypt(Encoding.UTF8.GetBytes(GetLine(dict)), licenseCredentialPass), newLicensePath);
            try
            {
                File.SetAttributes(newLicensePath, File.GetAttributes(newLicensePath) | FileAttributes.Hidden);
            }
            catch
            {
            }
        }

        public static bool WriteFile(byte[] content, string path)
        {
            try
            {
                new FileInfo(path).Directory.Create();
                if (File.Exists(path))
                    File.Delete(path);
                File.WriteAllBytes(path, content);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Write file byte error: " + ex.Message);
                return false;
            }
        }

        public static string GetLine(Dictionary<string, string> d)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in d)
                stringBuilder.Append(keyValuePair.Key).Append(":").Append(keyValuePair.Value).Append(',');
            return stringBuilder.ToString().TrimEnd(',');
        }

        //device ids
        public static List<string> GetDeviceIDs()
        {
            List<string> stringList1 = new List<string>();
            string input1 = GetMacAdress() + BaseId() + BiosId();
            stringList1.Add(GetMD5(input1));
            string str = BaseId() + BiosId();
            List<string> stringList2 = new List<string>();
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                stringList2.Add(networkInterface.GetPhysicalAddress().ToString());
            foreach (string input2 in stringList2)
            {
                if (!string.IsNullOrEmpty(input2))
                {
                    stringList1.Add(GetMD5(input2 + str));
                    stringList1.Add(GetMD5(input2));
                }
            }
            return stringList1;
        }

        private static string GetMacAdress()
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    return networkInterface.GetPhysicalAddress().ToString();
            }
            return (string)null;
        }

        private static string BaseId() => Identifier("Win32_BaseBoard", "SerialNumber");

        private static string BiosId() => Identifier("Win32_BIOS", "SerialNumber");

        private static string Identifier(string wmiClass, string wmiProperty)
        {
            string content = "";
            try
            {
                foreach (ManagementObject instance in new ManagementClass(wmiClass).GetInstances())
                {
                    if (content == "")
                    {
                        content = instance[wmiProperty].ToString();
                        break;
                    }
                }
            }
            catch (Exception ex1)
            {
                Debug.WriteLine("Error: WMI Class: " + wmiClass + " - WMI Property: " + wmiProperty + "\n");
                string path = HomePath + "/.tbid";
                if (File.Exists(path))
                {
                    content = ReadFile(path);
                }
                else
                {
                    content = GetMD5(Guid.NewGuid().ToString().ToUpper());
                    WriteFile(content, path);
                    try
                    {
                        File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
                    }
                    catch (Exception ex2)
                    {
                        Debug.WriteLine("Write ID error: " + ex2.Message + "\n");
                    }
                }
            }
            return content;
        }

        public static string GetMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string ReadFile(string path)
        {
            if (!File.Exists(path))
                return "";
            try
            {
                return File.ReadAllText(path);
            }
            catch
            {
                return "";
            }
        }

        public static bool WriteFile(string content, string path)
        {
            try
            {
                new FileInfo(path).Directory.Create();
                if (File.Exists(path))
                    File.Delete(path);
                File.WriteAllText(path, content);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Write file content error: " + ex.Message);
                Debug.WriteLine(ex.Message, "Error");
                return false;
            }
        }
    }
}
