using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var Bank = new Bank();
            var bankId = Guid.NewGuid().ToString();
            string Host = "SomeRabbit";

            //Randomizing new bank instance
            System.Console.WriteLine("Setting up new bank instance");
            if (args.Length == 0){
                Bank.Name = "Bank_" + bankId;
            }else{
                Bank.Name = args[0];
            }
            Bank.GenerateLoanInterest();
            var factory = new ConnectionFactory(){HostName=Host};
            System.Console.WriteLine($"New bank ({Bank.Name}) started on {Host}");
            
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    System.Console.WriteLine("Waiting for request - press enter to exit");
                    channel.ExchangeDeclare(exchange: "loanreq", type: ExchangeType.Fanout);
                    var ReqQueueName = channel.QueueDeclare().QueueName;
                    
                    channel.QueueBind(queue: ReqQueueName, exchange: "loanreq", routingKey: "", arguments: null);
                    channel.QueueDeclare(queue: "loanresp", durable:false, exclusive:false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    

                    consumer.Received += (model, ea) => {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body).Split(":");
                        var resptmp = Bank.TheDecider(double.Parse(message[1]));
                        var respmsg = "RESPONSE:"+Bank.Name+":"+ resptmp +":"+ message[1]+":"+message[2];
                        System.Console.WriteLine($"DATA RECEIVED : loanrequest for {message[1]} - messageId: {message[2]}");
                        var respbody = Encoding.UTF8.GetBytes(respmsg);

                        channel.BasicPublish(exchange:"", routingKey: "loanresp", body: respbody);
                    };

                    channel.BasicConsume(queue: ReqQueueName, autoAck: true, consumer: consumer);
                    

                    Console.ReadLine();
                }
            }

        }
    }
}
