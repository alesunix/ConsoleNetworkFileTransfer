// See https://aka.ms/new-console-template for more information
using ConsoleNetworkFileTransfer_Server;

ServerModel serverModel = new ServerModel();
serverModel.StartReceiving();
Console.ReadLine();
