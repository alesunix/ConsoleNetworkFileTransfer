// See https://aka.ms/new-console-template for more information
using ConsoleNetworkFileTransfer_Server;

Console.WriteLine("Запуск клиента...");
ServerModel serverModel = new ServerModel();
serverModel.StartReceiving();
