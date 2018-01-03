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
        [Test]
        public void GetMyImages()
        {
            string token = OAuth2Helper.GetToken("https://api.imgur.com/oauth2/authorize?client_id=2058b576d9029e1",
                "TestTaskZel", "Test1234Test", "authorize_token", "allow");
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
