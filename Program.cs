using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

public class UdpFileClient
{
    // Детали файла
    [Serializable]
    public class FileDetails
    {
        public string FILETYPE = "";
        public long FILESIZE = 0;
    }

    [STAThread]
    static void Main(string[] args)
    {
        FileStream fs;
        FileDetails fileDet;
        IPAddress remoteIPAddress;


        string s = null;
        Console.WriteLine("---- PLS Write host port catalog ----");
        s = Console.ReadLine();
        string[] s_arr = s.Split();
        remoteIPAddress = IPAddress.Parse(s_arr[0]);
        int tcpPort = int.Parse(s_arr[1]);
        string temp = s_arr[2];

        UdpClient UdpClient = new UdpClient(tcpPort);
        IPEndPoint RemoteIpEndPoint = null;
        
        Byte[] receiveBytes = new Byte[0];
        try
        {
            
            Console.WriteLine("---- Waiting for file information ----");

            // Получаем информацию о файле
            receiveBytes = UdpClient.Receive(ref RemoteIpEndPoint);
            Console.WriteLine("---- File information received ----");

            XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
            MemoryStream stream1 = new MemoryStream();

            // Считываем информацию о файле
            stream1.Write(receiveBytes, 0, receiveBytes.Length);
            stream1.Position = 0;

            // Вызываем метод Deserialize
            fileDet = (FileDetails)fileSerializer.Deserialize(stream1);
            Console.WriteLine("---- Received a file of type ." + fileDet.FILETYPE +
                " sized " + fileDet.FILESIZE.ToString() + " byte ----");

            Console.WriteLine("---- Wait for the file ----");

            // Получаем файл
            receiveBytes = UdpClient.Receive(ref RemoteIpEndPoint);

            // Преобразуем и отображаем данные
            Console.WriteLine("---- File received...Saving... ----");
            // Создаем временный файл с полученным расширением
            fs = new FileStream("temp." + fileDet.FILETYPE, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(receiveBytes, 0, receiveBytes.Length);

            Console.WriteLine("---- File saved ----");

            fs.Close();
        }
        catch (Exception eR)
        {
            Console.WriteLine(eR.ToString());
        }
        finally
        {
            
            UdpClient.Close();
            Console.Read();
        }
    }
    
}