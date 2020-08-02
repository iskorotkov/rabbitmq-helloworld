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

            channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.BasicQos(0, 1, false);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine($"[send] Sending message '{message}'.");
            channel.BasicPublish(exchange: "",
                                 routingKey: "task_queue",
                                 basicProperties: properties,
                                 body: body);
            Console.WriteLine($"[send] Message '{message}' sent.");
        }

        private static string GetMessage(string[] args)
        {
            return (args.Length > 0) ? string.Join(" ", args) : "Hello World!";
        }
    }
}
