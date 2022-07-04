using System;
using RabbitMQ.Client;
using System.Text;

namespace BasicSender
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

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "basic-receiver",
                                     basicProperties: null,
                                     body: body);
                channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
