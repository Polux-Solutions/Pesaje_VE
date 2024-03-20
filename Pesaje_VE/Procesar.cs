using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Pesaje_VE.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pesaje_VE
{
    internal class Procesar
    {
        public string Ejecutar(ref mRespuesta Respuesta ,string Cuerpo)
        {
            mEntrada Entrada = new mEntrada();

            JsonSerializerSettings jsettings = new JsonSerializerSettings
            {
                Culture = new CultureInfo("es-ES")
            };


            try
            {
                Entrada = JsonConvert.DeserializeObject<mEntrada>(Cuerpo, jsettings);
            }
            catch (Exception ex)
            {
                Respuesta.Mensaje_Error = $"Errror Deserialize Entrada {ex.Message}";
                Respuesta.Estado = "NOK";
            }

            if (Respuesta.Estado == "OK")
            {
                Respuesta.ID = Entrada.ID;



                switch (Entrada.Comando.ToUpper())
                {
                    case "HOLA":
                        Hola(ref Respuesta);
                        break;
                    case "PESO":
                        ReadBascula(ref Respuesta);
                        break;
                }
            }


            string jsonRespuesta = JsonConvert.SerializeObject(Respuesta);

            return jsonRespuesta;
        }


        internal void Hola(ref mRespuesta Respuesta)
        {
            Respuesta.Estado = "OK";
        }

   

        public static void ReadBascula(ref mRespuesta Respuesta)
        {
            byte[] bytes = new byte[1024];
            string Ip = "88.30.78.72";
            int Port = 14000;
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Ip);
                IPAddress ipAddress = host.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);

                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);


                    int bytesRec = sender.Receive(bytes);

                    string[] Pesadas = Encoding.ASCII.GetString(bytes, 0, bytesRec).Split(Convert.ToChar(2));
                    int PesoMax = 0;

                    foreach (string Peso in Pesadas)
                    {
                        int n = Peso.IndexOf("KG");

                        if (n > -1)
                        {
                            int PesoRow = 0;

                            if (int.TryParse(Peso.Substring(0, Peso.IndexOf("KG")).TrimStart(), out PesoRow))
                            {
                                if (PesoRow > PesoMax) PesoMax = PesoRow;
                            }
                        }
                    }

                    Respuesta.Peso = PesoMax;

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Respuesta.Estado = "NOK";
                    Respuesta.Mensaje_Error = $"ArgumentNullException : {ane.ToString()}";
                }
                catch (SocketException se)
                {
                    Respuesta.Estado = "NOK";
                    Respuesta.Mensaje_Error = $"SocketException : {se.ToString()}";
                }
                catch (Exception e)
                {
                    Respuesta.Estado = "NOK";
                    Respuesta.Mensaje_Error = $"Unexpected exception : {e.ToString()}";
                }

            }
            catch (Exception e)
            {
                Respuesta.Estado = "NOK";
                Respuesta.Mensaje_Error = $"Exception: {e.ToString()}";
            }
        }
    }
}
