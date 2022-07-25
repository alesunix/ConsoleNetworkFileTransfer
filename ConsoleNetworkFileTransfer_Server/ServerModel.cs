using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNetworkFileTransfer_Server
{
    internal class ServerModel
    {
        // Поток для записи файла на жесткий диск
        private Stream stream;
        // Сетевой поток который получает файл
        private NetworkStream networkStream;
        // Слушатель TCP, который будет прослушивать подключения 
        private TcpListener tcpListener;
        byte[] status;
        public string Ip { get; set; }
        public int Port { get; set; }
        const string PATH = @"C:\Test\";
        public void StartReceiving()
        {
            while (true)
            {
                try
                {
                    int serverPort = 815;
                    // Получить имя хоста (Сервера)
                    string hostServer = Dns.GetHostName();
                    IPAddress ipAdress = Dns.GetHostEntry(hostServer).AddressList[1];
                    if (tcpListener == null)
                    {
                        // Создать обьект прослушивания TCP
                        tcpListener = new TcpListener(ipAdress, serverPort);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Запуск сервера...");
                    tcpListener.Start();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Сервер запущен. Пожалуйста подключите клиент к {ipAdress}:{serverPort}");
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();/// Принять соединение
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Сервер принял клиента");
                    networkStream = tcpClient.GetStream();/// Получить поток и сохранить его в networkStream
                    Console.WriteLine("Сервер получил поток");

                    // Получаем от клиента имя и размер файла
                    byte[] buffer = new byte[2048];/// Буфер в котором хранятся данные от Клиента
                    int byteSize = networkStream.Read(buffer, 0, buffer.Length);/// Получаем имя файла
                    string fileName = Encoding.UTF8.GetString(buffer, 0, byteSize);/// Преобразовать поток в строку
                    stream = new FileStream(PATH + fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);/// Путь сохранения + имя файла, который отправил клиент
                    byteSize = networkStream.Read(buffer, 0, buffer.Length);/// Получаем размер файла
                    int fileSize = Convert.ToInt32(Encoding.UTF8.GetString(buffer, 0, byteSize));

                    Console.WriteLine(SendResponseToClient());// Отправка ответа клиенту 

                    // Получение файла
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"Получение файла {fileName} ({fileSize} байт)");
                    int size = fileSize > buffer.Length ? buffer.Length : fileSize;
                    while (fileSize > 0)
                    {
                        byteSize = networkStream.Read(buffer, 0, size);
                        fileSize -= byteSize;
                        stream.Write(buffer, 0, byteSize);/// Запись данных в локальный файловый поток
                    }
                    Console.WriteLine("Файл получен!");
                    Console.WriteLine(SendResponseToClient());// Отправка ответа клиенту 
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка - " + ex.Message);
                }
                finally
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Закрытие потоков.");
                    stream.Close();
                    networkStream.Close();
                    Console.WriteLine("Потоки закрыты.");
                }
            }
        }
        private string SendResponseToClient()// Отправка ответа клиенту 
        { 
            status = Encoding.UTF8.GetBytes("Ответ сервера - данные получены.".ToCharArray());
            networkStream.Write(status, 0, status.Length);
            Console.ForegroundColor = ConsoleColor.Cyan;
            return "Отпрака ответа клиенту о том, что данные получены.";
        }
    }
}