using System.Net.NetworkInformation;
using System.Text;

var pingSender = new Ping();
string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaa";
byte[] buffer = Encoding.ASCII.GetBytes(data);
const int timeout = 120;  // em milisegundos

while (true)
{
    Console.WriteLine("Ping para o google com timeout de 120ms");
    var options = new PingOptions();
    options.DontFragment = true;

    var completedTask = pingSender.SendPingAsync("google.com", timeout, buffer, options);
    completedTask.Wait();

    if (completedTask.Result.Status == IPStatus.Success)
    {
        Console.WriteLine("Internet voltou :( ");
        while (true)
        {
            Console.Beep();
            Thread.Sleep(1000);
        }
    }
    else
    {
        Console.WriteLine("Sem internet :) ");
    }

    Thread.Sleep(10000);
}