using System;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

        }
        private static void SubscribeOrdersFromRemoteClients()
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl(conf.Default.urlHub)
                .Build();

            //cuando pierde conexión se vuelve a conectar
            hubConnection.Closed += async (error) =>
                await hubConnection.StartAsync();
            hubConnection.On("start", () => StartService());
            hubConnection.On("stop", () => StopService());
        }

        private static void StopService()
        {
            Console.WriteLine("start");
        }

        private static void StartService()
        {
            Console.WriteLine("stop");
        }
    }
}
