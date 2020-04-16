using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace HttpServer
{
    public class DumpHttpServer
    {
        private Thread _thread;
        private string _siteDir; //Дополнительный каталог
        private HttpListener _listener;
        private int _port;
        
        public DumpHttpServer(string path, int port)
        {
            Initialize(path, port);
            
        }
        
        private void Initialize(string path, int port)
        {
            _siteDir = path;
            _port = port;
            _thread = new Thread(Listen);
            _thread.Start();
            Console.WriteLine($"Сервер запущни на порту {_port}");
            Console.WriteLine($"Файлы сайта лежат в каталоге {_siteDir}");
        }
        
        private void Listen()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{_port}/");
            _listener.Start();
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
            Stop();
        }
        private void Process(HttpListenerContext context)
        {
            string fileName = context.Request.Url.AbsolutePath;
            fileName = fileName.Substring(1);
            fileName = Path.Combine(_siteDir, fileName);
            
            if (File.Exists(fileName))
            {
                try
                {
                    Stream fileStream = new FileStream(fileName,FileMode.Open);
                    context.Response.ContentType = GetContentType(fileName);
                    context.Response.ContentLength64 = fileStream.Length;
                    
                    byte[] buffer = new byte[16 * 1024];
                    int dataLength;
                    do
                    {
                        dataLength = fileStream.Read(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Write(buffer, 0,buffer.Length);
                    } while (dataLength > 0);
                    fileStream.Close();
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            context.Response.OutputStream.Close();
        }
        
        
        private string GetContentType(string fileName)
        {
            var dictionary = new Dictionary<string,string> {
                    {".css",  "text/css"},
                    {".html",  "text/html"},
                    {".ico",  "image/x-icon"},
                    {".js",  "application/x-javascript"},
                    {".json",  "applocation/json"},
                    {".png",  "image/png"}
                };
            
            string contentType = "";
            string fileExtension = Path.GetExtension(fileName);
            dictionary.TryGetValue(fileExtension, out contentType);
            Console.WriteLine($"=> {contentType}");
            return contentType;
        }

        public void Stop()
        {
            _thread.Abort();
            _listener.Stop();
        }

       
    }
}