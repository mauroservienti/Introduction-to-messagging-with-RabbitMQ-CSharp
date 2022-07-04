using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PubSubPublisher
{
    class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ConfirmSelect();

                channel.ExchangeDeclare(exchange: "messages-events",
                    durable: true,
                    type: "topic");

                var props = channel.CreateBasicProperties();

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "messages-events",
                                     routingKey: "messages-events",
                                     basicProperties: props,
                                     body: body);
                channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                
                Console.WriteLine($"Published {message}");
                Console.WriteLine(" PubSubPublisher endpoint running.");
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}