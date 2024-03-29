﻿using System;
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
    class HttpServer
    {
        public const String VERSION = "HTTP/1.0";
        public const String SERVERNAME = "LocalServer/1.1";
        public const String MSG_DIR = "\\root\\msg\\";
        public const String WEB_DIR = "\\root\\web\\";

        TcpListener Listener;                                // Прослушиваем подключения от TCP клиентов сети.
        IPAddress localAdr = IPAddress.Parse("127.0.0.1");   
        public HttpServer (int Port)
        {
            Listener = new TcpListener(localAdr, Port);      // Инициализируем новый экземпляр который выполняет ожидание входящих подключений
                                                             //     для порта "Port" и локального адресса 127.0.0.1 
            Listener.Start();                                // Начинаем прослушивать входящие запросы на подключение.
        }

        public void Start ()                                 
        {
            Thread thread;                                   // Создаём новый поток.
            thread = new Thread (new ThreadStart(Run));      // 
            thread.Start();                                  // Запускаем поток с методом Run.
        }

        private void Run()
        {
            Listener.Start();                                    // Ожидание запросов.

            Console.WriteLine("Serwer was runing.");

            while (true)                                
            {
                Console.WriteLine("Wating for connection ...");
                TcpClient Client = Listener.AcceptTcpClient();          // Получаем запрос от браузера.
                Console.WriteLine("Client connected.");
                HandleClient(Client);                                   // Передаём клиента.
                Client.Close();                                         // Закрываем соединение.
            }
        }

        private void HandleClient(TcpClient Client)
        {
            StreamReader Reader = new StreamReader(Client.GetStream());  // Читаем запросы от браузера.
            String msg = "";
            
            while (Reader.Peek() != -1)                                  // Если есть что читать ...
            {
                msg += Reader.ReadLine() + "\n";                            // Читаем из потока и заносим в строку.
            }

            Console.WriteLine("REQUEST: \n {0}", msg);                   // Отображаем запрос клиента.
            Request request = Request.GetRequest(msg);                   // Парсим строку, получаемую от клиента-браузера.
                                                                         // Получаем в ответ объект Request с тремя параметрами, полученными
                                                                         // из ответа клиента-браузера
            
            Response response = Response.From(request);
            Console.WriteLine("Gettyng response\n");
            response.Post(Client.GetStream());
        }


        ~HttpServer()                                            // Остановка сервера.               
        {
            if (Listener != null)                            // Если слушатель был создан, то останавливаем его.
            {
                Listener.Stop();
            }
        }

    }
}
