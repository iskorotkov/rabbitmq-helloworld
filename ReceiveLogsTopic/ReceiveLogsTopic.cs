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
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("topic_logs", ExchangeType.Topic);

            var queueName = channel.QueueDeclare().QueueName;

            if (args.Length < 1)
            {
                Console.Error.WriteLine($"Usage: {Environment.GetCommandLineArgs()[0]}, [info] [warning] [error]");

                Environment.ExitCode = 1;
                return;
            }

            foreach (var routingKey in args)
            {
                channel.QueueBind(queue: queueName,
                                  exchange: "topic_logs",
                                  routingKey: routingKey);
            }

            Console.WriteLine("[receive] Waiting for messages.");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine($"[receive] Received message '{message}' with key '{routingKey}'");
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
