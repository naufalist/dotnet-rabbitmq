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
            // declare exchange
            channel.ExchangeDeclare("demo-header-exchange", ExchangeType.Headers);

            // declare queue with name "demo-topic-queue"
            channel.QueueDeclare(
                "demo-header-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // create header
            var header = new Dictionary<string, object> { { "account", "new" } };

            // bind a queue to an exchange
            channel.QueueBind("demo-header-queue", "demo-header-exchange", string.Empty, header);

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
            channel.BasicConsume("demo-header-queue", true, consumer);
            Console.WriteLine("Consumer started");
            Console.ReadLine();
        }
    }
}
