using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitReceiver2;

internal class Program
{
    static void Main(string[] args)
    {
        ConnectionFactory factory = new();

        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        factory.ClientProvidedName = "Rabbit receiver 2 App";

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

        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (sender, args) =>
        {
            Task.Delay(3000).Wait();
            var body = args.Body.ToArray();
            var msg = Encoding.UTF8.GetString(body);
            Console.WriteLine($"msg received : #{msg}");

            channel.BasicAck(args.DeliveryTag, false);
        };

        var consumerTag = channel.BasicConsume(QueueName, false, consumer);

        Console.ReadLine();

        channel.BasicCancel(consumerTag);

        channel.Close();
        cnn.Close();
    }
}
