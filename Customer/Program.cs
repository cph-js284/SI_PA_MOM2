using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace producer
{
    class Program
    {
        static readonly int timeOut = 2500;
        static readonly string HostName = "SomeRabbit";
        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory(){HostName=HostName};
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "loanreq", type:ExchangeType.Fanout);
                    
                    channel.QueueDeclare(queue: "loanresp", durable:false, exclusive:false, autoDelete: false, arguments: null);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received +=  (mode, ea) => {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body).Split(":");
                        //run storing in seperate thread
                        Task.Run(()=> AddOfferToStore(message[4], double.Parse(message[2]), message[1]));
                        System.Console.WriteLine("**********************************************");
                        System.Console.WriteLine($"RESPONSE : from bank  [{message[1]}] - Offer : [{message[2]}%] - Amount : {message[3]} - (messageId:{message[4]})" );
                        System.Console.WriteLine("**********************************************");
                    };

                    channel.BasicConsume(queue: "loanresp", autoAck: true, consumer:consumer);
                    System.Console.WriteLine("Enter amount of money you want to loan");
                    var amount = Console.ReadLine();
                    var requestId = Guid.NewGuid().ToString();

                    try
                    {
                        var message = "Request:" + double.Parse(amount).ToString() + ":" + requestId;
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "loanreq", routingKey:"", body:body);
                        System.Console.WriteLine("Loan-request sent to banks : " + message);

                    }
                    catch (System.Exception)
                    {
                        System.Console.WriteLine("just bleh...");                            
                    }
                    //timeout for banks to reply
                    Thread.Sleep(timeOut);
                    await Task.Run(()=>FindBestOffer(requestId, 1000));
                    Console.ReadLine();

                }
            }
        }

        //helpers
        //loan-interest-offers from bank-responses are saved here
        static Dictionary<string, List<BankOffer>> OfferStore = new Dictionary<string, List<BankOffer>>();

        //Adding incoming offer to dictionary
        static void AddOfferToStore(string key, double offer, string name){
            if(!OfferStore.ContainsKey(key)){
                OfferStore.Add(key, new List<BankOffer>());

                OfferStore[key].Add(new BankOffer(){Offer=offer, Name=name});
            }else{
                OfferStore[key].Add(new BankOffer(){Offer=offer, Name=name});
            }
        }

        //Searching the dictionary for best offer
        static void FindBestOffer(string key, int replyTime){
            //allow for replytime travel though queue (1sec since localhost)
            Thread.Sleep(replyTime);
            OfferStore[key].Sort(); 

            System.Console.WriteLine('\n' +"---------------------------------------------------");
            System.Console.WriteLine("---------------------------------------------------");
            System.Console.WriteLine($"BEST OFFER : {OfferStore[key].FirstOrDefault().Name}" + '\n' + "Interest : " + $"{OfferStore[key].FirstOrDefault().Offer}%" +'\n' + $"For loanrequestID: {key}");
            System.Console.WriteLine("---------------------------------------------------");
            System.Console.WriteLine("---------------------------------------------------");
            System.Console.WriteLine("Done... press enter to terminate");

        }

    
    }
}
