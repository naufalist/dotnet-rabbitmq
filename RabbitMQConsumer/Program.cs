using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQConsumer
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

            // create consumer
            var consumer = new EventingBasicConsumer(channel);

            // receive message
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };

            // consume
            channel.BasicConsume("demo-queue", true, consumer);
            Console.ReadLine();
        }
    }
}
