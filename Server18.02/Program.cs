using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

class MyCurrencyServer 
{
    private static Dictionary<string, double> myExchangeRates = new Dictionary<string, double>
    {
        {"USD_EURO", 0.92}, 
        {"EURO_USD", 1.09}, 
        {"USD_UAH", 40.50}, 
        {"UAH_USD", 0.025}, 
        {"EURO_UAH", 44.26},
        {"UAH_EURO", 0.023} 
    };

    static void Main()
    {
        // Запускаю сервер на порту
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("Сервер валютных курсов запущен! Подключение...");

        // Бесконечный цикл для обработки клиентов
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread clientHandler = new Thread(HandleClientConnection);
            clientHandler.Start(client);
        }
    }

    // Обработка каждого клиента в отдельном потоке
    static void HandleClientConnection(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;
        NetworkStream stream = client.GetStream();
        IPEndPoint clientAddress = (IPEndPoint)client.Client.RemoteEndPoint;
        Console.WriteLine($"Подключился клиент: {clientAddress.Address}:{clientAddress.Port}");

        try
        {
            byte[] buffer = new byte[1024]; // Буфер для получения данных
            int bytesReceived;

            while ((bytesReceived = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string request = Encoding.UTF8.GetString(buffer, 0, bytesReceived).Trim();
                Console.WriteLine($"Получил запрос: {request}");

                string response = "Неверный запрос :"; // Ответ по умолчанию
                if (myExchangeRates.TryGetValue(request.ToUpper(), out double rate))
                {
                    response = rate.ToString(); // Отправляем курс, если нашли
                }

                byte[] responseData = Encoding.UTF8.GetBytes(response);
                stream.Write(responseData, 0, responseData.Length);
                Console.WriteLine($"Отправили ответ: {response}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}"); 
        }
        finally
        {
            client.Close();
            Console.WriteLine("Клиент отключился");
        }
    }
}