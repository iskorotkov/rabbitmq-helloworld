using System;
using System.Text;
using RabbitMQ.Client;

namespace Send
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs", ExchangeType.Fanout);

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine($"[send] Sending message '{message}'.");
            channel.BasicPublish(exchange: "logs",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine($"[send] Message '{message}' sent.");
        }

        private static string GetMessage(string[] args)
        {
            return (args.Length > 0) ? string.Join(" ", args) : "Hello World!";
        }
    }
}
