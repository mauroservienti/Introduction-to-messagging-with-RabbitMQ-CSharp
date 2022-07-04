using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace PubSubAnotherSubscriber
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

                channel.QueueDeclare(queue: "pub-sub-another-subscriber",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                channel.QueueBind(queue: "pub-sub-another-subscriber",
                    exchange: "messages-events",
                    routingKey: "messages-events");
                
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var receivedBody = ea.Body.ToArray();
                    var receivedMessage = Encoding.UTF8.GetString(receivedBody);
                    Console.WriteLine($"PubSubAnotherSubscriber - Received {receivedMessage}");
                };

                channel.BasicConsume(queue: "pub-sub-another-subscriber",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" PubSubAnotherSubscriber endpoint running.");
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}