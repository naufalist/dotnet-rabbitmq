using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQProducer
{
    static class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };

            // create connection
            using var connection = factory.CreateConnection();

            // create channel
            using var channel = connection.CreateModel();

            // publish
            TopicExchangeProducer.Publish(channel);
        }
    }
}
