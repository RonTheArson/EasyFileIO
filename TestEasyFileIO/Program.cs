// See https://aka.ms/new-console-template for more information
using EasyFileIO;

Console.WriteLine("Hello, World!");
Console.WriteLine(FileHandler.SearchFiles("C:\\", "error"));
Console.WriteLine(FileHandler.ReadFile("C:\\Erorr log.txt"));
Console.ReadLine();