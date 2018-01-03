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
        public void ImplicitFrow()
        {
            string token = OAuth2Helper.GetToken("https://api.imgur.com/oauth2/authorize?client_id=0f624d4e6985af4",
                "TestTaskZel", "Test1234Test", "authorize_token", "allow");
            var client = new RestClient("https://api.imgur.com/3/account/me/images");
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");
            var getRequest = new RestRequest(Method.GET);
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

        [Test]
        public void ExplicitFlow()
        {
           /* var client2 = new RestClient("https://www.eventbrite.com/oauth/authorize?response_type=code&client_id=ZAM34PJ47M66YCYJ6J");

            var gettttt = new RestRequest(Method.GET);
            IRestResponse getttRes = client2.Execute(gettttt);*/

            var client = new RestClient("https://www.wunderlist.com/oauth/authorize?client_id=47751cb0c02675e3621a&redirect_uri=https://www.getpostman.com/oauth2/callback&state=RANDOM");

            /*var postCredentials = new RestRequest("ajax/login/", Method.POST);
            postCredentials.AddHeader("content-type", "application/x-www-form-urlencoded");
            postCredentials.AddParameter("application/x-www-form-urlencoded",
                "email=test.task.zel@gmail.com&password=Test1234Test&referrer=/oauth/authorize?response_type=code&client_id=ZAM34PJ47M66YCYJ6J",
                ParameterType.RequestBody);

            IRestResponse credsResponse = client.Execute(postCredentials);
            string specValue = OAuth2Helper.GetSpecificValueFromCookies(credsResponse, "csrftoken");

            var postAuth = new RestRequest("ngapi/oauth/authorizing", Method.POST);
            postAuth.AddHeader("Cookie", credsResponse.Headers[12].Value.ToString());
            postAuth.AddCookie("csrftoken", specValue);
            postAuth.AddHeader("content-type", "application/x-www-form-urlencoded");
            postAuth.AddParameter("application/x-www-form-urlencoded",
                $"csrfmiddlewaretoken={specValue}&client_id=ZAM34PJ47M66YCYJ6J&response_type=code&access_choices=allow",
                ParameterType.RequestBody);
            IRestResponse authResponse = client.Execute(postAuth);
            
            Assert.True(true);*/

            var postCredentials = new RestRequest(Method.POST);
            postCredentials.AddHeader("content-type", "application/x-www-form-urlencoded");
            postCredentials.AddParameter("application/x-www-form-urlencoded",
                "email=test.task.zel@gmail.com&password=Test1234Test",
                ParameterType.RequestBody);

            IRestResponse credsResponse = client.Execute(postCredentials);


            //string specialParamValue = getResponse.Cookies[0].Value;

            /* var postRequest = new RestRequest(Method.POST);
             postRequest.AddQueryParameter("response_type", "code");
             postRequest.AddCookie(specialCookie, specialParamValue);
             postRequest.AddHeader("cache-control", "no-cache");
             postRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
             postRequest.AddParameter("application/x-www-form-urlencoded",
                 $"username={username}&password={password}&{specialParam}={specialParamValue}",
                 ParameterType.RequestBody);
 
             IRestResponse postResponse = client.Execute(postRequest);
 
             string code = GetUriElement(postResponse.ResponseUri.Query, "code");
 
             string tokenUri = postResponse.ResponseUri.AbsoluteUri.Replace(postResponse.ResponseUri.Query, tokenPath);
 
             var clientForToken = new RestClient(tokenUri);
             var postForToken = new RestRequest(Method.POST);
             postForToken.AddQueryParameter("code", code);
 
             IRestResponse postResponseForToken = clientForToken.Execute(postForToken);*/

        }
    }
}
