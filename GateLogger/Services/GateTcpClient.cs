﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GateLogger.Services.StartEvents;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using TcpClient = NetCoreServer.TcpClient;

namespace GateLogger.Services
{
    public class GateTcpClient : TcpClient
    {
        private const int _port = 1917;

        public GateTcpClient(string address) : base(address, _port)
        {
        }
        public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs args);

        public static event AsyncEventHandler<EventResponse> NewEventAsync = delegate { return Task.CompletedTask; };

        public static event EventHandler<EventResponse> NewEvent = delegate {};
        public GateTcpClient(string address, int port) : base(address, port) { }

        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"client connected a new session with Id {Id} {Address} {Port}");
            //this.SendAsync(JsonSerializer.Serialize(new GetConfigCommand()));

            //this.Send(new byte[] { 0x0 });
            
            this.SendAsync(JsonSerializer.Serialize(new StartEventsCommand()));

        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Client disconnected a session with Id {Id} {Address} {Port}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }


        private readonly List<byte> _receiveBuffer = new();
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            var encodingBytes = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.UTF8, buffer, (int)offset, (int)size);
            _receiveBuffer.AddRange(encodingBytes);
            // если получен не полный ответ, то прерываемся
            if (encodingBytes[^1] != '\n') return;


            var response = Encoding.UTF8.GetString(_receiveBuffer.ToArray());
            _receiveBuffer.Clear();

            var list = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var str in list)
            {
                try
                {
                    Console.WriteLine(str);
                    var result = JsonSerializer.Deserialize<StartEventsResponse>(str,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var gateEvent = result?._event;
                    if (gateEvent == null) continue;


                    lock (gateEvent)
                    {
                        NewEvent.Invoke(this, gateEvent);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


                
            }

        }


        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }

        private bool _stop;
    }
}
