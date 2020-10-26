using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;                                   
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Request
    {
        public String URL { get; set; }
        public String Type { get; set; }
        public String Host { get; set; }

        private Request(String type, String url, String host)
        {
            Type = type;
            URL = url;
            Host = host;
        }

        public static Request GetRequest(String msg)
        {
            if (String.IsNullOrEmpty(msg))                
                return null;

            String[] tokens = msg.Split(' ');                   // Разбиваем строку ответа по пробелам
            Console.WriteLine("TYPE: {0}, URL: {1}, HOST: {2}", tokens[0], tokens[1], tokens[3]);
            return new Request(tokens[0], tokens[1], tokens[3]); // Возвращаем реквест с тремя параметрами "тип, URL и хост"
        }

    }
}