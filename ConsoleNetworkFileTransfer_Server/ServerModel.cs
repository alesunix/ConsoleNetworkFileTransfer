﻿using System;
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
        const string PATH = @"C:\Test\";
        public void StartReceiving()
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
                TcpClient tcpClient = tcpListener.AcceptTcpClient();// Принять соединение
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Сервер принял клиента");
                networkStream = tcpClient.GetStream();// Получить поток и сохранить его в networkStream
                Console.WriteLine("Сервер получил поток");

                int byteSize = 0;
                byte[] buffer = new byte[2048];// Буфер в котором хранятся данные от Клиента
                byteSize = networkStream.Read(buffer, 0, 2048);// Представляет имя файла
                string fileName = Encoding.ASCII.GetString(buffer, 0, byteSize);// Преобразовать поток в строку и сохранить
                stream = new FileStream(PATH + fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);// Путь сохранения + имя файла, который отправил клиент

                buffer = new byte[2048];
                byteSize = networkStream.Read(buffer, 0, 2048);// Представляет размер файла
                long fileSize = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 0, byteSize));
                Console.WriteLine($"Получение файла {fileName} ({fileSize} байт)");

                // Размер буфера для приема файла
                buffer = new byte[2048];
                while ((byteSize = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Запись данных в локальный файловый поток
                    stream.Write(buffer, 0, byteSize);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка - " + ex.Message);
                StartReceiving();// Перезапуск сервера - прослушиванеие TCP
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Файл получен, закрытие потоков.");
                stream.Close();
                networkStream.Close();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Потоки закрыты.");
                StartReceiving();// Перезапуск сервера - прослушиванеие TCP
            }
        }
    }
}