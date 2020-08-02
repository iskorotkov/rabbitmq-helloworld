﻿using System;
using System.Text;
using RabbitMQ.Client;

namespace Send
{
    internal static class Program
    {
        private static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            const string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($"[x] Sent {message}");

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
