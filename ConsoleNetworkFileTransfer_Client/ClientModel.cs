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
        byte[] buffer = new byte[2048];
        int byteSize = 0;
        string status = string.Empty;
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
            try
            {
                if (tcpClient.Connected != false)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Отправка информации о файле");
                    // Получить поток подключенный к серверу
                    networkStream = tcpClient.GetStream();
                    fileStream = new FileStream(PATH, FileMode.Open, FileAccess.Read);
                    FileInfo info = new FileInfo(PATH);/// Получить информацию о файле

                    // Отправить серверу имя файла
                    string fileName = info.Name;
                    byte[] byteFileName = Encoding.UTF8.GetBytes(fileName.ToCharArray());
                    networkStream.Write(byteFileName, 0, byteFileName.Length);/// Записать имя файла в сетевой поток

                    // Отправить серверу размер файла
                    long fileSize = info.Length;
                    byte[] byteFileSize = Encoding.UTF8.GetBytes(fileSize.ToString().ToCharArray());
                    networkStream.Write(byteFileSize, 0, byteFileSize.Length);/// Записать размер файла в сетевой поток

                    WaitResponseFromServer();// Ожидание получения ответа от сервера /// - (Имя и размер файла получены сервером)

                    // Отправка файла на сервер
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"Отправка файла {fileName} {fileSize} байт");
                    while ((byteSize = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        networkStream.Write(buffer, 0, byteSize);/// Запись данных в сетевой поток
                    }
                    Console.WriteLine("Файл отправлен!");

                    WaitResponseFromServer();// Ожидание получения ответа от сервера /// - (файл получен сервером)
                }
                else
                {
                    ConnectToServer(serverIP, serverPort);
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка - " + ex.Message);
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Закрытие потоков и соединений.");
                tcpClient.Close();
                networkStream.Close();
                fileStream.Close();
                Console.WriteLine("Потоки и соединения закрыты.");
            }
        }
        private void WaitResponseFromServer()// Ожидание получения ответа от сервера
        {
            byteSize = networkStream.Read(buffer, 0, buffer.Length);
            status = Encoding.UTF8.GetString(buffer, 0, byteSize);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(status);/// - (файл получен сервером)
        }
    }
}
