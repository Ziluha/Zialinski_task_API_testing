using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Policy;
using HtmlAgilityPack;
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
            string token = OAuth2Helper.GetSpotifyTokenThrowClientCredentials("ffa4d3f9b2b04aa5b3791716ab943218",
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
        public void BitlyResourceOwnerPasswordFlowCheck()
        {
            string token = OAuth2Helper.GetBitlyTokenThrowResourceOwnerPassword("967be650896308c8dea967f02a08253a261abdeb",
                "9de9e38f5ac92985ba91e2fae0f5ba187ce01e49", "TestTaskZel", "Test1234Test");

            RestClient client = new RestClient("https://api-ssl.bitly.com");
            RestRequest getRequest = new RestRequest($"/v3/link/info?access_token={token}&link=http%3A%2F%2Fbit.ly%2FMwSGaQ", Method.GET);
            IRestResponse getResponse = client.Execute(getRequest);

            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'status_code': {'type':'number'},
                'data': {'type':'object'},
                'status_txt': {'type':'string'}
              }
            }");

            JObject result = JObject.Parse(getResponse.Content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is not valid");
        }

        [Test]
        public void ImgurAuthorizationCodeFlowCheck()
        {
            string token = OAuth2Helper.GetImgurTokenThrowCode("0f624d4e6985af4",
                "6ff9c38a4c91b88a68f8cc970a3dc88e7c585839", "TestTaskZel", "Test1234Test");

            RestClient client = new RestClient("https://api.imgur.com/3/account/me/available_avatars")
            {
                Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(token, bearerTokenType)
            };
            RestRequest getRequest = new RestRequest(Method.GET);
            IRestResponse getResponse = client.Execute(getRequest);

            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'data': {'type':'object'},
                'success': {'type':'boolean'},
                'status': {'type':'number'}
              }
            }");

            JObject result = JObject.Parse(getResponse.Content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is not valid");
        }
    }
}
