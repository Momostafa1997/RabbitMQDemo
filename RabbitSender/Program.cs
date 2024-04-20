using RabbitMQ.Client;
using System.Text;

namespace RabbitSender;

internal class Program
{
    static void Main(string[] args)
    {
        ConnectionFactory factory = new();

        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        factory.ClientProvidedName = "Rabbit Sender App";

        var cnn = factory.CreateConnection();

        var channel = cnn.CreateModel();

        var exchangeName = "DemoExchangee";
        var routeKey = "demo-route-key";
        var QueueName = "DemoQueue";

        // create exchange
        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

        // create Queue
        channel.QueueDeclare(QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        // bind
        channel.QueueBind(QueueName, exchangeName, routeKey, null);

        //string message = "Hello, RabbitMQ!";
        //var body = Encoding.UTF8.GetBytes(message);

        for (int i = 0; i < 60; i++)
        {
            Thread.Sleep(1000);
            // push
            channel.BasicPublish(exchangeName, routeKey, basicProperties: null, Encoding.UTF8.GetBytes(i.ToString()));
            Console.WriteLine(i);
        }

        channel.Close();
        cnn.Close();
    }
}
