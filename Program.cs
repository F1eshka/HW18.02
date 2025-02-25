using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class MyCurrencyClient 
{
    static void Main()
    {
        Socket clientSocket = null;
        try
        {
            // Подключаюсь к моему серверу на локальном хосте
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Loopback, 5000));

            Console.Write("Введи пару валют (например, USD_EURO): ");
            string message = Console.ReadLine();
            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine("Пустое сообщение. Завершаю работу.");
                return; // Завершаю, если ничего не ввели
            }

            // Отправляю запрос серверу
            byte[] messageData = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(messageData);

            // Получаю ответ от сервера
            byte[] buffer = new byte[1024];
            int receivedBytes = clientSocket.Receive(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            Console.WriteLine($"Сервер ответил: {response}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Ошибка сети: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Что-то пошло не так: {ex.Message}");
        }
        finally
        {
            if (clientSocket != null)
            {
                try
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    Console.WriteLine("Сокет закрыт. До встречи!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при закрытии: {ex.Message}");
                }
            }
        }
    }
}