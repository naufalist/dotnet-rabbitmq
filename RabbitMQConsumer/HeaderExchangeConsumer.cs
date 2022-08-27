using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConsumer
{
    public static class HeaderExchangeConsumer
    {
        public static void Consume(IModel channel)
        {
            // declare exchange with name "demo-header-exchange"
            channel.ExchangeDeclare(
                exchange: "demo-header-exchange",
                type: ExchangeType.Headers,
                durable: true,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    {"x-message-ttl", 30000 } // ttl for the message in miliseconds (30 secs)
                }
            );

            // declare queue with name "demo-header-queue"
            channel.QueueDeclare(
                "demo-header-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // create header
            var header = new Dictionary<string, object>
            {
                { "account", "initialize" }
            };

            // bind a queue to an exchange
            channel.QueueBind(
                queue: "demo-header-queue",
                exchange: "demo-header-exchange",
                routingKey: string.Empty,
                arguments: header
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
                queue: "demo-header-queue",
                autoAck: true,
                consumer: consumer
            );

            Console.WriteLine("Consumer started");
            Console.ReadLine();
        }
    }
}
