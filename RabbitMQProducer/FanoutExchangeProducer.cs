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
            // ttl for the message in miliseconds
            var ttl = new Dictionary<string, object>
            {
                {"x-message-ttl", 30000 } // 30 secs
            };

            // declare exchange with name "demo-fanout-exchange"
            channel.ExchangeDeclare("demo-fanout-exchange", ExchangeType.Fanout, arguments: ttl);

            // initialize count for multiple messages
            var count = 0;

            while (true)
            {
                // intialize message and serialize
                var message = new { Name = "Producer", Message = $"Hi! I'm a producer. Count: {count}" };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                // set header props
                // basic props not needed, just to show that it really doesn't matter
                // what you pass in header
                var properties = channel.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object> { { "account", "new" } };

                // publish body to demo-fanout-exchange
                // routingKey: account.init
                // property: properties
                channel.BasicPublish("demo-fanout-exchange", "account.init", properties, body);

                // increment
                count++;

                // sleep for a little bit
                // it will produce a message every second
                Thread.Sleep(1000);
            }
        }
    }
}
