using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConsumer
{
    public static class FanoutExchangeConsumer
    {
        public static void Consume(IModel channel)
        {
            // declare exchange
            channel.ExchangeDeclare("demo-fanout-exchange", ExchangeType.Fanout);

            // declare queue with name "demo-fanout-queue"
            channel.QueueDeclare(
                "demo-fanout-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // bind a queue to an exchange
            channel.QueueBind("demo-fanout-queue", "demo-fanout-exchange", string.Empty);

            // this will make consumer to fetch 10 messages at the time
            channel.BasicQos(0, 10, false);

            // declare consumer
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };

            // started consuming
            channel.BasicConsume("demo-fanout-queue", true, consumer);
            Console.WriteLine("Consumer started");
            Console.ReadLine();
        }
    }
}
