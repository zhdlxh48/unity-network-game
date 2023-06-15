// See https://aka.ms/new-console-template for more information

using ServerLib;

var server = new Server(19948, 50);
server.Start();

Console.ReadKey();