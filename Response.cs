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
    class Response
    {
        Byte[] data;
        String status;
        String mime;

        private Response(String status, String mime, Byte[] data)
        {
            this.status = status;
            this.mime = mime;
            this.data = data;
        }

        public static Response From(Request request)
        {
            if (request == null)
            {
                return NotWork("400.html", "400 Bad request");
            }
            if (request.Type == "GET")
            {
                if (request.URL.Contains("/"))                   // Заменяю "/" в запросе клиента на "\"
                {
                    String[] spl = request.URL.Split('/');
                    request.URL = spl[1] + ".html";
                }
                String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + request.URL;
                FileInfo fiInfo = new FileInfo(file);
                if (fiInfo.Exists && fiInfo.Extension.Contains("."))
                {
                    return MakeFromFile(fiInfo);
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(fiInfo + "/");
                    if (!di.Exists)
                    {
                        return NotWork("404.html", "404 Page not found");
                    }
                    FileInfo[] files = di.GetFiles();
                    foreach (FileInfo ff in files)
                    {
                        if (ff.Name.Contains("defoult.html") || ff.Name.Contains("index.html"))
                        {
                            return MakeFromFile(ff);
                        }
                    }
                }

            }
            else
            {
                return NotWork("405.html", "Method not allowed");
            }
            return NotWork("404.html", "Page not found");


        }

        private static Response MakeFromFile(FileInfo fi)
        {
            FileStream fs = fi.OpenRead();
            Byte[] d = new Byte[fs.Length];
            BinaryReader reader = new BinaryReader(fs);
            reader.Read(d, 0, d.Length);

            return new Response("200 OK", "text/html", d);
        }

        private static Response NotWork(String filename, String status)
        {
            String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + filename;
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);

            return new Response(status, "text/html", d);
        }

        public void Post(NetworkStream Stream)
        {
            StreamWriter writer = new StreamWriter(Stream);

            Console.WriteLine(String.Format("{0} {1} \r\nServer: {2}\r\nContent-Language: ru\r\nContent-type: {3}\r\nContent-Ranges: bytes\r\nContentLenght: {4}\r\nConnection: close\r\n",
                HttpServer.VERSION, status, HttpServer.SERVERNAME, mime, data.Length));
            writer.WriteLine(String.Format("{0} {1} \r\nServer: {2}\r\nContent-Language: ru\r\nContent-type: {3}\r\nContent-Ranges: bytes\r\nContentLenght: {4}\r\nConnection: close\r\n",
                HttpServer.VERSION, status, HttpServer.SERVERNAME, mime, data.Length));
            writer.Flush(); // Очищаем поток

            try                                                   // Из-за того что первым ответом клиента приходит пустая строка 
            {                                                     // вылетает исключение при использовании метода Write
                Stream.Write(data, 0, data.Length);
            }
            catch
            {

            }
        }

        public string httpServer { get; set; }
    }
}
