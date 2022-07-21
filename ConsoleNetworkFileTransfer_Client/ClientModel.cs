using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNetworkFileTransfer_Client
{
    internal class ClientModel
    {
        TcpClient tcpClient;
        FileStream fileStream;
        NetworkStream networkStream;
        public string serverIP = "192.168.1.111";
        public int serverPort = 815;
        const string PATH = @"C:\iis.png";
        public void ConnectToServer(string serverIP, int serverPort)
        {
            tcpClient = new TcpClient();
            try
            {
                Console.ForegroundColor = ConsoleColor.White;
                tcpClient.Connect(serverIP, serverPort);
                Console.WriteLine("Успешное подключение к серверу!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка - " + ex.Message);
                Console.ReadLine();
            }
        }
        public void SendFile()
        {
            if (tcpClient.Connected != false)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Отправка информации о файле");
                // Получить поток подключенный к серверу
                networkStream = tcpClient.GetStream();
                fileStream = new FileStream(PATH, FileMode.Open, FileAccess.Read);               
                FileInfo info = new FileInfo(PATH);// Получить информацию о файле

                // Отправить серверу имя файла
                string fileName = info.Name;
                byte[] byteFileName = Encoding.ASCII.GetBytes(fileName.ToCharArray());
                networkStream.Write(byteFileName, 0, byteFileName.Length);// Записать имя файла в сетевой поток

                // Отправить серверу размер файла
                long fileSize = info.Length;
                byte[] byteFileSize = Encoding.ASCII.GetBytes(fileSize.ToString().ToCharArray());               
                networkStream.Write(byteFileSize, 0, byteFileSize.Length);// Записать размер файла в сетевой поток

                // Ожидание получения ответа от сервера
                byte[] buffer = new byte[2048];
                int byteSize = networkStream.Read(buffer, 0, 2048);
                string status = Encoding.ASCII.GetString(buffer, 0, byteSize);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(status);

                // Отправка файла на сервер
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Отправка файла {fileName} {fileSize} байт");
                while ((byteSize = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    networkStream.Write(buffer, 0, byteSize);// Запись данных в сетевой поток
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Файл отправлен. Закрытие потоков и соединений!");
                tcpClient.Close();
                networkStream.Close();
                fileStream.Close();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Потоки и соединения закрыты");
            }
            else
            {
                ConnectToServer(serverIP, serverPort);
            }
        }
    }
}
