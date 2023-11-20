using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Transactions;

var tcpListener = new TcpListener(IPAddress.Any, 8888);

try
{
    tcpListener.Start();    // запускаем сервер
    Console.WriteLine("Сервер запущен. Ожидание подключений... ");

    while (true)
    {
        // получаем подключение в виде TcpClient
        using var tcpClient = await tcpListener.AcceptTcpClientAsync();
        // получаем объект NetworkStream для взаимодействия с клиентом
        var stream = tcpClient.GetStream();

        //while (true)
        //{
            
        //}


        var comm = new Rootobject();
        comm.command = "newEvent";
        var evt = new Event
        {
            eventCode = 2,
            readerId = 79,
            userId = 5,
            dateTime = DateTime.Now.ToString("G"),
            alarm = false,
            unit = "Хронос выход",
            fio = "Каськов Владимир Васильевич"
        };
        comm._event = evt;
        var json = JsonSerializer.Serialize(comm);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 100; i++)
        {
            sb.Append(json + '\n');
        }



        await stream.WriteAsync(Encoding.UTF8.GetBytes(sb.ToString()));
        Thread.Sleep(5000);



    }
}
finally
{
    tcpListener.Stop(); // останавливаем сервер
}




public class Rootobject
{
    public string command { get; set; }
    public Event _event { get; set; }
}

public class Event
{
    public int eventCode { get; set; }
    public int readerId { get; set; }
    public int userId { get; set; }
    public string dateTime { get; set; }
    public bool alarm { get; set; }
    public string unit { get; set; }
    public string message { get; set; }
    public string name { get; set; }
    public string fio { get; set; }
    public string group { get; set; }
    public string expiry { get; set; }
    public string det1 { get; set; }
    public string det2 { get; set; }
    public string det3 { get; set; }
    public string det4 { get; set; }
    public string det5 { get; set; }
    public string det6 { get; set; }
    public string det7 { get; set; }
    public string det8 { get; set; }
}
