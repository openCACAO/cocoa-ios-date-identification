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
            // StreamWriter sw = new StreamWriter("output.txt");
            WebClient wcList = new WebClient();
            byte [] dataTop = wcList.DownloadData("https://covid19radar-jpn-prod.azureedge.net/c19r/440/list.json");
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
                SHA256 mySHA = SHA256.Create();
                byte[] hashValue = mySHA.ComputeHash(dataOut);
                // sw.WriteLine("Url:");
                // sw.WriteLine(df.url);
                // sw.WriteLine("Hash:");
                sw.Write("<li>");
                for (i = 0; i < hashValue.Length;i++)  
                {
                    sw.Write("{0:X2}", hashValue[i]);
                }
                // sw.WriteLine("");
                ulong startUnixTime = 0;
                for (i = 0; i < 8; i++)
                {
                    startUnixTime |= (ulong)dataOut[i + 17] << (8 * i);
                }
                ulong endUnixTime = 0;
                for (i = 0; i < 8; i++)
                {
                    endUnixTime |= (ulong)dataOut[i + 26] << (8 * i);
                }
                // sw.WriteLine("Date:");
                DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                epochStart = epochStart.AddSeconds((double)startUnixTime);
                // sw.WriteLine(epochStart.ToString() + "(Start)");
                DateTime epochEnd = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                epochEnd = epochEnd.AddSeconds((double)endUnixTime);
                // sw.WriteLine(epochEnd.ToString() + "(End)");
                // sw.WriteLine(" at " + epochStart.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz") + " - " + epochEnd.ToString("yyyy-MM-dd HH:mm:ss \"GMT\"zzz") + "</li>");
                sw.WriteLine(" at " + epochStart.ToString() + " - " + epochEnd.ToString() + "</li>");
                int number_of_keys = 0;
                for (i = 125; i < dataOut.Length;)
                {
                    if ((dataOut[i] == 0x3a) && (dataOut[i + 1] == 0x1c) && (dataOut[i + 2] == 0x0a) && (dataOut[i + 3] == 0x10))
                    {
                       //  Console.WriteLine("3a 1c 0a 10");
                        i += 4;
                    }
                    else
                    {
                        int j;
                        String tek = "";
                        for (j = 0; j < 16; j++)
                        {
                            tek += String.Format("{0:X2}", dataOut[i]);
                            i++;
                        }
                        // Console.WriteLine(tek);
                        // sw.WriteLine(tek);
                        if ((dataOut[i] == 0x10) && (dataOut[i + 1] == 0x00) && (dataOut[i + 2] == 0x18))
                        {
                            // Console.WriteLine("10 00 18 x0");
                            i += 4;
                        }
                        if ((dataOut[i + 2] == 0x01) && (dataOut[i + 3] == 0x20))
                        {
                            // Console.WriteLine("bx a2 01 20");
                            i += 4;
                        }
                        // 有効時間（144 x 10分＝24時間）
                        if (dataOut[i] == 0x90)
                        {
                            // Console.WriteLine("90");
                            i++;
                            number_of_keys++;
                        }
                        // Console.WriteLine("{0:X2}", dataOut[i]);
                        if ((dataOut[i] != 0x01)&&(dataOut[i] !=0x02))
                        {
                            // Console.WriteLine("False?");
                        }
                        i++;
                    }
                }
                // export.sigのパート：本コードでは読み飛ばし
                zipInputStream.GetNextEntry();
                MemoryStream outputStreamSig = new MemoryStream();
                zipInputStream.CopyTo(outputStreamSig);
                byte[] dataOut2 = outputStreamSig.ToArray();
                for (i = 0; i < dataOut2.Length; i++)
                {
                    // Console.Write(String.Format("{0:X2} ", dataOut2[i]));
                }
                // ここまで到達したら1レコードの処理が完了している
                // Console.Write(".");
                Console.WriteLine("Region: " + df.region + " Url: " + df.url + " Created at " + df.created);
                DateTime epochCreated = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                epochCreated = epochCreated.AddSeconds((double)(df.created / 1000));
                // sw.WriteLine("Number of Keys: " + number_of_keys);
                ///sw.WriteLine("Created at:");
                // sw.WriteLine(epochCreated.ToString());
                // sw.WriteLine("------------------------");
            }
            Console.WriteLine("done");
            sw.Close();
        }
    }
}
