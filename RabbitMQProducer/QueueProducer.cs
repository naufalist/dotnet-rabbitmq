using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQProducer
{
    public static class QueueProducer
    {
        public static void Publish(IModel channel)
        {
            // declare queue with name "demo-queue"
            channel.QueueDeclare(
                "demo-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // initialize count for multiple messages
            var count = 0;

            while (true)
            {
                // intialize message and serialize
                var message = new { Name = "Producer", Message = $"Hi! I'm a producer. Count: {count}" };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                // publish body to (empty) default exchange rabbitMQ
                // routingKey: demo-queue
                channel.BasicPublish("", "demo-queue", null, body);

                // increment
                count++;

                // sleep for a little bit
                // it will produce a message every second
                Thread.Sleep(1000);
            }
        }
    }
}
