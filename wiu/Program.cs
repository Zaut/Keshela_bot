using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TeleBot
{
    class Program
    {
        static TelegramBotClient client;
        static DateTime time;
        static List<long> users = new List<long>();
        static void Main(string[] args)
        {
            time = DateTime.Now;
            client = new TelegramBotClient("1265245906:AAEfDlf9jBqwoNn-C9XhzgcmT6GDgCI7uuk");
            client.OnMessage += getMsg;
            client.StartReceiving();
            Timer keshelatime = new Timer(tick_func, null, 0, 5000);
            Console.Read();
        }
        //https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5
        private static string GetMoney()
        {
            string message = "";
            var json = new WebClient().DownloadString("https://api.privatbank.ua/p24api/pubinfo?json&exchange&coursid=5");
            JArray keshela = JArray.Parse(json);
            for (int i = 0; i < keshela.Count; i++)
            {
                message += "Exchange Rates:" + keshela[i]["buy"] + keshela[i]["ccy"] + "\n";
            }

            return message;
        }
        public static void tick_func(object state)
        {
            if (time.Minute == DateTime.Now.Minute)
            {
                time = time.AddMinutes(1);
                foreach (var user in users)
                {
                    client.SendTextMessageAsync(user, GetMoney());
                }
            }
        }

        private static void getMsg(object sender, MessageEventArgs e)
        {
            if (e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return;

            Console.WriteLine($"Msg from {e.Message.Chat.Id}" + ":" + e.Message.Text);

            switch (e.Message.Text.ToLower())
            {
                case "/start":
                    Console.WriteLine(GetMoney());
                    users.Add(e.Message.Chat.Id);
                    break;
            }
        }
    }
}