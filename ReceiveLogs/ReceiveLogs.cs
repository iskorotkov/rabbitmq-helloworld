using System;
using System.Linq;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receive
{
    internal static class Program
    {
        private static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("logs", ExchangeType.Fanout);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: "logs",
                              routingKey: "");

            Console.WriteLine("[receive] Waiting for messages.");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[receive] Received message '{message}'");
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
