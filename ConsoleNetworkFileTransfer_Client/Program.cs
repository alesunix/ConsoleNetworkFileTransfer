// See https://aka.ms/new-console-template for more information
using ConsoleNetworkFileTransfer_Client;

Console.WriteLine("Запуск клиента...");
ClientModel clientModel = new ClientModel();
clientModel.ConnectToServer(clientModel.serverIP, clientModel.serverPort);
clientModel.SendFile();
