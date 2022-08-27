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
    public static class FanoutExchangeProducer
    {
        public static void Publish(IModel channel)
        {
            // declare exchange with name "demo-fanout-exchange"
            channel.ExchangeDeclare(
                exchange: "demo-fanout-exchange",
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    {"x-message-ttl", 30000 } // ttl for the message in miliseconds (30 secs)
                }
            );

            // declare queue with name "demo-fanout-queue"
            channel.QueueDeclare(
                queue: "demo-fanout-queue",
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
                // basic props not needed, just to show that it really doesn't matter
                // what you pass in header
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Headers = new Dictionary<string, object>
                {
                    { "account", "initialize" }
                };

                // publish body to demo-fanout-exchange
                // routingKey: account.balance (routingKey not needed, just to show that it really doesn't matter
                channel.BasicPublish(
                    exchange: "demo-fanout-exchange",
                    routingKey: "account.balance",
                    basicProperties: basicProperties,
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
