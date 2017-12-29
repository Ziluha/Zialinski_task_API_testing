using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;

namespace Zialinski_task_API_testing
{
    class OAuthAPITest
    {
       

        private string GetToken()
        {
            var client = new RestClient("https://api.imgur.com/oauth2/authorize?response_type=token&client_id=270ef1233bb6141");
            var getRequest = new RestRequest(Method.GET);
            
            IRestResponse getResponse = client.Execute(getRequest);

            string allow = getResponse.Cookies[0].Value;
            
            var postRequest = new RestRequest(Method.POST);
            postRequest.AddCookie("authorize_token", allow);
            postRequest.AddHeader("cache-control", "no-cache");
            postRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            postRequest.AddParameter("application/x-www-form-urlencoded", 
                $"username=TestTaskZel&password=Test1234Test&allow={allow}", 
                ParameterType.RequestBody);
            IRestResponse response = client.Execute(postRequest);

            string absPath = response.ResponseUri.AbsoluteUri;
            string[] elemsOfPath = absPath.Split('&', '=', '#');
            string token = elemsOfPath[Array.IndexOf(elemsOfPath, "access_token")+1];
            
            return token;
        }

        private string GetTokenThrowCode()
        {
            var client = new RestClient("https://api.imgur.com");
            var getRequest = new RestRequest("oauth2/authorize", Method.GET);
            getRequest.AddQueryParameter("response_type", "code");
            getRequest.AddQueryParameter("client_id", "270ef1233bb6141");

            IRestResponse getResponse = client.Execute(getRequest);

            string allow = getResponse.Cookies[0].Value;

            var postRequest = new RestRequest("oauth2/authorize", Method.POST);
            postRequest.AddQueryParameter("response_type", "code");
            postRequest.AddQueryParameter("client_id", "270ef1233bb6141");
            postRequest.AddCookie("authorize_token", allow);
            postRequest.AddHeader("cache-control", "no-cache");
            postRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            postRequest.AddParameter("application/x-www-form-urlencoded",
                $"username=TestTaskZel&password=Test1234Test&allow={allow}",
                ParameterType.RequestBody);

            IRestResponse response = client.Execute(postRequest);

            string absPath = response.ResponseUri.AbsoluteUri;
            string[] elemsOfPath = absPath.Split('&', '=', '#');
            string code = elemsOfPath[Array.IndexOf(elemsOfPath, "code") + 1];

            // here request to /oauth2/token

            return code;
        }

        [Test]
        public void GetMyImages()
        {
            string token = GetToken();
            var client = new RestClient("https://api.imgur.com/3/account/me/images");
            var getRequest = new RestRequest(Method.GET);
            getRequest.AddHeader("Authorization", "Bearer " + token);
            IRestResponse getResponse = client.Execute(getRequest);

            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'data': {'type':'array'},
                'success': {'type':'boolean'},
                'status': {'type':'number'}
              }
            }");
            var content = getResponse.Content;
            Console.Write(getResponse.Content);

            JObject result = JObject.Parse(content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is not valid");
        }
    }
}
