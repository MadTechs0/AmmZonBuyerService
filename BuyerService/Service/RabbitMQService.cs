using BuyerService.Interface;
using BuyerService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BuyerService.Service
{
    public class RabbitMQService: BackgroundService
    {
        public readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "statusQueue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
        public void StatusListener()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {

                var body = ea.Body.ToArray();
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                    var orderStaus = JsonConvert.DeserializeObject<OrderStatus>(Encoding.UTF8.GetString(body));
                    bool status = await orderService.UpdateStatus(orderStaus);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON deserialization error: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: "statusQueue",
                                 autoAck: true,
                                 consumer: consumer);
        }


        public void PublishOrder(OrderRequest order)
        {

            _channel.QueueDeclare(queue: "orderQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(order));
            _channel.BasicPublish(exchange: "", routingKey: "orderQueue", basicProperties: null, body: body);
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
