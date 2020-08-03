using System;
using System.Text;
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

            channel.QueueDeclare(queue: "rpc_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.BasicQos(0, 1, false);

            Console.WriteLine("[receive] Waiting for messages.");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var response = "";
                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[receive] Received message '{message}'");

                    var n = int.Parse(message);
                    response = Fib(n).ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "",
                                         routingKey: props.ReplyTo,
                                         basicProperties: replyProps,
                                         body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }

                Console.WriteLine("[receive] Message processed.");
            };

            channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        private static int Fib(int n)
        {
            if (n < 0) throw new ArgumentException("N must be non-negative.", nameof(n));
            return (n <= 1) ? n : Fib(n - 1) + Fib(n - 2);
        }
    }
}
