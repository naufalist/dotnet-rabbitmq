using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQConsumer
{
    public static class DirectExchangeConsumer
    {
        public static void Consume(IModel channel)
        {
            // declare exchange with name "demo-direct-exchange"
            channel.ExchangeDeclare(
                exchange: "demo-direct-exchange",
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    {"x-message-ttl", 30000 } // ttl for the message in miliseconds (30 secs)
                }
            );

            // declare queue with name "demo-direct-queue"
            channel.QueueDeclare(
                "demo-direct-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // bind a queue to an exchange
            channel.QueueBind(
                queue: "demo-direct-queue",
                exchange: "demo-direct-exchange",
                routingKey: "account.balance",
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
                queue: "demo-direct-queue",
                autoAck: true,
                consumer: consumer
            );

            Console.WriteLine("Consumer started");
            Console.ReadLine();
        }
    }
}
