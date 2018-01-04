using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;

namespace Zialinski_task_API_testing
{
    class OAuthAPITest
    {
        private string bearerTokenType = "Bearer";

        [Test]
        public void ImgurImplicitFlowCheck()
        {
            string token = OAuth2Helper.GetImgurTokenImplicit("0f624d4e6985af4",
                "TestTaskZel", "Test1234Test");

            RestClient client = new RestClient("https://api.imgur.com/3/account/me/images")
            {
                Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, bearerTokenType)
            };
            RestRequest getRequest = new RestRequest(Method.GET);
            IRestResponse getResponse = client.Execute(getRequest);

            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'data': {'type':'array'},
                'success': {'type':'boolean'},
                'status': {'type':'number'}
              }
            }");

            JObject result = JObject.Parse(getResponse.Content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is not valid");
        }

        [Test]
        public void SpotifyClientCredentialsFlowCheck()
        {
            string token = OAuth2Helper.GetSpotifyTokenClientCredentials("ffa4d3f9b2b04aa5b3791716ab943218",
                "7487562733a049f4b38dc407cc4156ee");

            RestClient client = new RestClient("https://api.spotify.com/v1/search?type=artist&q=Eminem")
            {
                Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, bearerTokenType)
            };

            RestRequest getRequest = new RestRequest(Method.GET);
            IRestResponse getResponse = client.Execute(getRequest);

            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'artists': {
                    'type':'object',
                    'properties':{
                        'href': {'type':'string'},
                        'items': {
                            'type':'array',
                            'items':{
                                'type':'object'
                            }
                        },
                        'limit': {'type':'number'},
                        'offset': {'type':'number'},
                        'total': {'type':'number'}
                    }
                }
              }
            }");
            
            JObject result = JObject.Parse(getResponse.Content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is not valid");
        }

        [Test]
        public void SpotifyExplicitFlowCheck()
        {
            RestClient client = new RestClient("https://accounts.spotify.com");
            RestRequest getRequest = new RestRequest("authorize/?client_id=ffa4d3f9b2b04aa5b3791716ab943218&response_type=code&redirect_uri=https://www.getpostman.com/oauth2/callback", Method.GET);
            IRestResponse getResponse = client.Execute(getRequest);
            string[] cookies = getResponse.Headers[15].Value.ToString().Split('=', ';');
            Console.WriteLine(getResponse.Headers[15].Value);

            string elem = cookies[Array.IndexOf(cookies, "csrf_token") + 1];


            RestRequest postRequest = new RestRequest("api/login", Method.POST);
            postRequest.AddCookie("csrf_token", elem);
            postRequest.AddCookie("fb_continue", "https%3A%2F%2Faccounts.spotify.com%2Fen%2Fauthorize%3Fclient_id%3Dffa4d3f9b2b04aa5b3791716ab943218%26response_type%3Dcode%26redirect_uri%3Dhttps%3A%252F%252Fwww.getpostman.com%252Foauth2%252Fcallback");
            postRequest.AddCookie("remember", "ziluha");
            postRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            postRequest.AddParameter("application/x-www-form-urlencoded", 
                $"remember=false&username=ziluha&password=Test1234Test&csrf_token={elem}", 
                ParameterType.RequestBody);
            IRestResponse postResponse = client.Execute(postRequest);


        }
    }
}
