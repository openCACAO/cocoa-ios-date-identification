using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
// NuGet:
using System.Text.Json;
using ICSharpCode.SharpZipLib.Zip;
using System.Security.Cryptography;

namespace covidTest
{
    public class JsonDataFormat
    {
        public String region { get; set; }
        public String url { get; set; }
        public long created { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            StreamWriter sw = new StreamWriter("output-html.txt");
            WebClient wcList = new WebClient();
            byte[] dataTop = wcList.DownloadData("https://covid19radar-jpn-prod.azureedge.net/c19r/440/list.json");
            String jsonText = Encoding.ASCII.GetString(dataTop);
            List<JsonDataFormat> listR = JsonSerializer.Deserialize<List<JsonDataFormat>>(jsonText);
            int k;
            for (k = 0; k < listR.Count; k++)
            {
                JsonDataFormat df = listR[k];
                WebClient wc = new WebClient();
                byte[] dataBin = wc.DownloadData(df.url);
                MemoryStream inputStream = new MemoryStream(dataBin);
                MemoryStream outputStream = new MemoryStream();
                ZipInputStream zipInputStream = new ZipInputStream(inputStream);
                zipInputStream.GetNextEntry();
                zipInputStream.CopyTo(outputStream);
                byte[] dataOut = outputStream.ToArray();
                TemporaryExposureKeyExport teke;
                // 先頭16バイトのヘッダを読み捨てる
                byte[] dataOut3 = new byte[dataOut.Length - 16];
                for (i=0;i<dataOut3.Length;i++)
                {
                    dataOut3[i] = dataOut[i + 16];
                }
                // Parserに入力する
                teke = TemporaryExposureKeyExport.Parser.ParseFrom(dataOut3);
                SHA256 mySHA = SHA256.Create();
                byte[] hashValue = mySHA.ComputeHash(dataOut);
                sw.Write("<li>");
                for (i = 0; i < hashValue.Length; i++)
                {
                    sw.Write("{0:X2}", hashValue[i]);
                }
                DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                epochStart = epochStart.AddSeconds((double)teke.StartTimestamp);
                DateTime epochEnd = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                epochEnd = epochEnd.AddSeconds((double)teke.EndTimestamp);
                sw.WriteLine(" at " + epochStart.ToString() + " - " + epochEnd.ToString() + "</li>");
            }
            sw.Close();
        }
    }
}
