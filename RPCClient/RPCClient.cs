using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Send
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var n = "30";
            if (args.Length > 0 && int.TryParse(args[0], out _))
            {
                n = args[0];
            }

            using var rpcClient = new RpcClient();
            Console.WriteLine($"[send] Requesting '{n}'.");

            var response = rpcClient.Call(n);
            Console.WriteLine($"[send] Result is '{response}'.");
        }
    }

    public class RpcClient : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly BlockingCollection<string> _responseQueue = new BlockingCollection<string>();
        private readonly IBasicProperties _props;

        public RpcClient()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _replyQueueName = _channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_channel);
            _props = _channel.CreateBasicProperties();

            var correlationId = Guid.NewGuid().ToString();
            _props.CorrelationId = correlationId;
            _props.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    _responseQueue.Add(response);
                }
            };
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish("", "rpc_queue", _props, messageBytes);
            _channel.BasicConsume(_consumer, _replyQueueName, true);
            return _responseQueue.Take();
        }

        public void Close()
        {
            _connection.Close();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
