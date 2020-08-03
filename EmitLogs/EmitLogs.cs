using System;
using System.Linq;
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

            channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);

            var severity = (args.Length > 0) ? args[0] : "info";
            var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine($"[send] Sending message '{message}' with severity '{severity}'.");
            channel.BasicPublish(exchange: "direct_logs",
                                 routingKey: severity,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine($"[send] Message '{message}' sent with severity '{severity}'.");
        }
    }
}
