using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConsumer
{
    public static class TopicExchangeConsumer
    {
        public static void Consume(IModel channel)
        {
            // declare exchange with name "demo-topic-exchange"
            channel.ExchangeDeclare(
                exchange: "demo-topic-exchange",
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    {"x-message-ttl", 30000 } // ttl for the message in miliseconds (30 secs)
                }
            );

            // declare queue with name "demo-topic-queue"
            channel.QueueDeclare(
                queue: "demo-topic-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // bind a queue to an exchange
            channel.QueueBind(
                queue: "demo-topic-queue",
                exchange: "demo-topic-exchange",
                routingKey: "account.*",
                arguments: null
            );

            // this will make consumer to fetch 10 messages at the time
            channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 10,
                global: false
            );

            // declare consumer
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[{DateTime.Now}] Consumed: {message}");
            };

            // started consuming
            channel.BasicConsume(
                queue: "demo-topic-queue",
                autoAck: true,
                consumer: consumer
            );

            Console.WriteLine("Consumer started");
            Console.ReadLine();
        }
    }
}
