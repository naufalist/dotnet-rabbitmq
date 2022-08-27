using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQProducer
{
    public static class TopicExchangeProducer
    {
        public static void Publish(IModel channel)
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

            // initialize count for multiple messages
            var count = 0;

            while (true)
            {
                // intialize message and serialize
                var message = $"Balance: Rp 100000. Count: {count}";
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                // publish body to demo-topic-exchange
                // routingKey: account.balance
                channel.BasicPublish(
                    exchange: "demo-topic-exchange",
                    routingKey: "account.balance",
                    basicProperties: null,
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
