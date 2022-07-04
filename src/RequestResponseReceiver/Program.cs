﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RequestResponseReceiver
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
                channel.QueueDeclare(queue: "request-response-receiver",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var receivedBody = ea.Body.ToArray();
                    var receivedMessage = Encoding.UTF8.GetString(receivedBody);
                    var receivedProps = ea.BasicProperties;
                    Console.WriteLine($"Received {receivedMessage} with correlation ID {receivedProps.CorrelationId}");

                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = receivedProps.CorrelationId;

                    string replyMessage = $"Hey, {receivedProps.CorrelationId} nice to meet you...";
                    var replyBody = Encoding.UTF8.GetBytes(replyMessage);

                    channel.BasicPublish(exchange: "",
                                     routingKey: receivedProps.ReplyTo,
                                     basicProperties: replyProps,
                                     body: replyBody);
                    channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                
                    Console.WriteLine($"Sent {replyMessage}");
                };
                channel.BasicConsume(queue: "request-response-receiver",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" RequestResponseReceiver endpoint running.");
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
