using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQProducer
{
    static class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };

            // create connection
            using var connection = factory.CreateConnection();

            // create channel
            using var channel = connection.CreateModel();

            // declare queue with name "demo-queue"
            channel.QueueDeclare(
                "demo-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // intialize message and serialize
            var message = new { Name = "Producer", Message = "Hi! I'm a producer" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            // publish body to (empty) default exchange rabbitMQ
            // routingKey: demo-queue
            channel.BasicPublish("", "demo-queue", null, body);
        }
    }
}
