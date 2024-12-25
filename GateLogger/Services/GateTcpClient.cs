using GateLogger.Services.StartEvents;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TcpClient = NetCoreServer.TcpClient;

using System.Threading.Channels;


namespace GateLogger.Services
{
    public class GateTcpClient : TcpClient
    {
        //private const int _port = 1917;
        private static readonly Channel<EventResponse> _channel = Channel.CreateUnbounded<EventResponse>();

        private bool _stop;
        private readonly object _lock = new object();
        //public static event Func<EventResponse, Task>? NewEvent;
        public static event Action<EventResponse>? NewEvent;
        private readonly ILogger<Worker> _logger;
        public GateTcpClient(string address, int port, ILogger<Worker> logger) : base(address, port) => _logger = logger;


        public void DisconnectAndStop()
        {
            _stop = true;
            DisconnectAsync();
            while (IsConnected)
                Thread.Yield();
        }

        protected override void OnConnected()
        {
            _logger.LogInformation($"Установлено соединение с сервером {Address} {Port}");
            //this.SendAsync(JsonSerializer.Serialize(new GetConfigCommand()));

            //this.Send(new byte[] { 0x0 });
            // запрос событий с сервера
            this.SendAsync(JsonSerializer.Serialize(new StartEventsCommand()));

        }

        protected override void OnDisconnected()
        {
            //Console.WriteLine($"Client disconnected a session with Id {Id} {Address} {Port}");
            _logger.LogInformation($"Client disconnected a session with Id {Id} {Address} {Port}");

            // Wait for a while...
            Thread.Sleep(1000);
            
            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }


        private readonly MemoryStream _receiveBuffer = new();

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            try
            {
                // Конвертируем байты из Windows-1251 в UTF-8
                var encodingBytes = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.UTF8, buffer, (int)offset, (int)size);
                _receiveBuffer.Write(encodingBytes, 0, encodingBytes.Length);

                // Если последний байт не '\n', выходим
                if (encodingBytes[^1] != '\n')
                    return;

                // Преобразуем данные из буфера в строку
                var response = Encoding.UTF8.GetString(_receiveBuffer.ToArray());
                _receiveBuffer.SetLength(0); // Очищаем буфер
                _receiveBuffer.Position = 0;

                // Разделяем строку по символу новой строки
                var messages = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var msg in messages)
                {
                    try
                    {
                        var result = JsonSerializer.Deserialize<StartEventsResponse>(msg,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        var gateEvent = result?.Event;
                        if (gateEvent == null) continue;

                        // Пишем в канал
                        if (!_channel.Writer.TryWrite(gateEvent))
                            _logger.LogWarning("Не удалось записать событие в канал: канал переполнен.");



                        //NewEvent?.Invoke(gateEvent);
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError($"Ошибка десериализации JSON: {jsonEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке данных в OnReceived: {ex.Message}");
            }
        }




        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP client caught an error with code {error}");
        }


        // Асинхронный метод для чтения сообщений
        public static async Task<EventResponse> ReadMessageAsync(CancellationToken cancellationToken = default)
        {
            // Чтение данных из канала
            return await _channel.Reader.ReadAsync(cancellationToken);
        }

        //public static async Task<EventResponse> ReadMessage()
        //{
        //    var tcs = new TaskCompletionSource<EventResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
        //    void Handler(EventResponse e) => tcs.TrySetResult(e);

        //    try
        //    {
        //        NewEvent += Handler;
        //        return await tcs.Task;
        //        //tcs.Task<>
        //    }
        //    finally
        //    {
        //        NewEvent -= Handler;
        //    }
        //}

    }
}
