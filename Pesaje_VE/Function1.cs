using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using Pesaje_VE.models;
using Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pesaje_VE
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Inicio")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)

        //[Function("Inicio")]
        //public async HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            mEntrada entrada = new mEntrada();
            mRespuesta Respuesta = new mRespuesta();

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            string CadenaRespuesta = string.Empty;

            Respuesta.Estado = "OK";
            Respuesta.Mensaje_Error = string.Empty;
            Respuesta.Peso = 0;

            try
            {

                Procesar procesar = new Procesar();

                string Cuerpo = await HttpRequestDataExtensions.ReadAsStringAsync(req);
                CadenaRespuesta = procesar.Ejecutar(ref Respuesta, Cuerpo);

            }
            catch (Exception ex) 
            {
                Respuesta.Estado = "NOK";
                Respuesta.Mensaje_Error = ex.ToString();
            }

            try
            {
                response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                response.WriteStringAsync(CadenaRespuesta);
            }
            catch (Exception ex) 
            { }

            return response;

        }
    }
}
