using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
        private string bearerTokenType = "Bearer";

        [Test]
        public void ImgurImplicitFlow()
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
        public void SpotifyClientCredentialsFlow()
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
                                'external_urls':{'type':'object'},
                                'genres':{'type':'array'}
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
    }
}
