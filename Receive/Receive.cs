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

            channel.QueueDeclare(queue: "task_queue",
                 durable: true,
                 exclusive: false,
                 autoDelete: false,
                 arguments: null);
            channel.BasicQos(0, 1, false);

            Console.WriteLine("[receive] Waiting for messages.");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[receive] Received message '{message}'");

                int dots = message.Count(c => c == '.');
                Thread.Sleep(dots * 1000);
                Console.WriteLine("[receive] Message processed.");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "task_queue",
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
