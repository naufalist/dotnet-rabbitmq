using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConsumer
{
    public static class QueueConsumer
    {
        public static void Consume(IModel channel)
        {
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
            Console.WriteLine("Consumer started");
            Console.ReadLine();
        }
    }
}
