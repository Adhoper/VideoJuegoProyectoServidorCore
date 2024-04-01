using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using VideoJuegoProyectoServidorCore.Data;
using VideoJuegoProyectoServidorCore.Models;

namespace VideoJuegoProyectoServidorCore
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            var httpListener = new HttpListener();

            httpListener.Prefixes.Add("http://localhost:1600/");

            httpListener.Start();

            Console.WriteLine("Se ha iniciado el servidor");

            try
            {
                while (true)
                {
                    // Espera una solicitud HTTP entrante
                    var context = await httpListener.GetContextAsync();

                    // Verifica si la solicitud es una solicitud WebSocket
                    if (context.Request.IsWebSocketRequest)
                    {
                        // Acepta la conexión WebSocket
                        var webSocketContext = await context.AcceptWebSocketAsync(null);

                        // Maneja la conexión WebSocket de forma asincrónica
                        // Se aplica la recursividad ya que este se llama asi misma luego de enviar la respuesta al cliente.
                        await HandleWebSocketAsync(webSocketContext.WebSocket);
                    }
                    else
                    {
                        // Rechaza las solicitudes que no sean WebSocket
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Se ha producido una excepción con la solicitud HTTP entrante: {ex.Message}");
            }
        }

        static async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            // Buffer para recibir mensajes WebSocket
            byte[] buffer = new byte[1024];

            // Recibe el primer mensaje del cliente
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Bucle principal que maneja la comunicación WebSocket
            while (!result.CloseStatus.HasValue)
            {

                // Decodifica el mensaje recibido como una cadena UTF-8
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Mensaje recibido: {message}");

                //Se instancia el contexto de la base de datos
                using (var db = new InfoClienteContext())
                {

                    //Asegura la creacion de la base de datos
                    await db.Database.EnsureCreatedAsync();


                // Valida que el mensaje sea iniciar juego
                if (message.ToLower() == "iniciar juego")
                {
                    //Instancia un objeto y le asigna el mensaje recibido a su propiedad
                    var infocliente = new ClienteInfo()
                    {
                        Mensaje = message,
                    };

                    // Envía una actualización de estado al cliente
                    string response = $"\u001b[36mSe inicio el juego.\u001b[0m";
                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Envía una actualización de estado al cliente
                    response = $"\u001b[36mMundos Disponibles: Jimwestt y Kethan. Escoja uno:\u001b[0m";
                    responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    //Se agrega el objeto a una coleccion
                    db.ClienteInfo.Add(infocliente);
                }
                else if (message.ToLower() == "jimwestt")
                {
                    var infocliente = new ClienteInfo()
                    {
                        Mensaje = message,
                    };

                    string response = $"\u001b[36mSe ha elegido Jimwestt.\u001b[0m";
                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    db.ClienteInfo.Add(infocliente);
                }
                else if (message.ToLower() == "kethan")
                {
                    var infocliente = new ClienteInfo()
                    {
                        Mensaje = message,
                    };

                    string response = $"\u001b[36mSe ha elegido Kethan.\u001b[0m";
                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    db.ClienteInfo.Add(infocliente);
                }
                else
                {
                    // Si lo escrito no es válido, envía un mensaje de error al cliente
                    string errorMessage = "\u001b[31mLo que escribiste no es valido.\u001b[0m";
                    byte[] errorBuffer = Encoding.UTF8.GetBytes(errorMessage);
                    await webSocket.SendAsync(new ArraySegment<byte>(errorBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                    //Se guarda en la base de datos
                    await db.SaveChangesAsync();

               }

                // Recibe el siguiente mensaje del cliente
                buffer = new byte[1024];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            // Cierra la conexión WebSocket
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }

}

