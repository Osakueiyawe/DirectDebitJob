using DirectDebitJob.Interfaces;
using DirectDebitJob.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Connections
{
    class ApiConnection:IApiConnection
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiConnection> _logger;
        public ApiConnection(IConfiguration configuration, ILogger<ApiConnection> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> SendSuccessNotificationToEnterpriseLife(EnterpriseLifeRequest enterpriselifepayload)
        {
            bool result = false;
            string responsecode = "";
            string responsemessage = "";
            try
            {
                string url = _configuration.GetSection("EnterpriseLifeEndpoint").Value;
                string requestData = JsonConvert.SerializeObject(enterpriselifepayload);
                _logger.LogInformation($"Payload sent to Enterprise Life: {requestData}");
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);                
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", requestData, ParameterType.RequestBody);
                request.RequestFormat = DataFormat.Json;
                IRestResponse response = client.Execute(request);
                var responsedata = response.Content;
                _logger.LogInformation($"Response from Enterprise Life: {responsedata}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    EnterpriseLifeResponse paymentnotificationresponse = JsonConvert.DeserializeObject<EnterpriseLifeResponse>(responsedata);
                    if (paymentnotificationresponse != null)
                    {
                        responsecode = paymentnotificationresponse.paymentupdateresponse.ResponseCode;
                        responsemessage = paymentnotificationresponse.paymentupdateresponse.ResponseDesc;
                    }
                }
                if (responsecode == "00")
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calling Enterprise API for {enterpriselifepayload.debitAccountNumber}",ex);
            }
            return result;
        }
    }
}
