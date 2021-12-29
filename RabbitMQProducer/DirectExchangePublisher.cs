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
    public static class DirectExchangePublisher
    {
        public static void Publish(IModel channel)
        {
            // ttl for the message in miliseconds
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", 30000 } // 30 secs
            };

            // declare exchange with name "demo-direct-exchange"
            channel.ExchangeDeclare("demo-direct-exchange", ExchangeType.Direct, arguments: ttl);

            // initialize count for multiple messages
            var count = 0;

            while (true)
            {
                // intialize message and serialize
                var message = new { Name = "Producer", Message = $"Hi! I'm a producer. Count: {count}" };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                // publish body to demo-direct-exchange
                // routingKey: account.init
                channel.BasicPublish("demo-direct-exchange", "account.init", null, body);

                // increment
                count++;

                // sleep for a little bit
                // it will produce a message every second
                Thread.Sleep(1000);
            }
        }
    }
}
