using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {

            string currentDir = Directory.GetCurrentDirectory();
            string site = currentDir + @"/site";
            DumpHttpServer server = new DumpHttpServer(site,7777);
            
            
        }
    }
}