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
    public static class HeaderExchangeProducer
    {
        public static void Publish(IModel channel)
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

            // initialize count for multiple messages
            var count = 0;

            while (true)
            {
                // intialize message and serialize
                var message = $"Balance: Rp 100000. Count: {count}";
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                // set header props
                var properties = channel.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object>
                {
                    { "account", "initialize" }
                };

                // publish body to demo-header-exchange
                // routingKey: string.Empty
                // property: properties
                channel.BasicPublish(
                    exchange: "demo-header-exchange",
                    routingKey: string.Empty,
                    basicProperties: properties,
                    body: body
                );
                Console.WriteLine($"[{DateTime.Now}] Published: {message}");

                // increment
                count++;

                // sleep for a little bit
                // it will produce a message every second
                Thread.Sleep(1000);
            }
        }
    }
}
