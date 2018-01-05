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
            string token = OAuth2Helper.GetImgurTokenThrowCode("6882d17e7f88b68",
                "06b9984082706750a1182b1f16e78e650381b416", "TestTaskZel", "Test1234Test");

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

        [Test]
        public void AuthorizationCodeFlowCheck()
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
                 $"remember=true&username=ziluha&password=Test1234Test&csrf_token={elem}", 
                 ParameterType.RequestBody);
             IRestResponse postResponse = client.Execute(postRequest);

            /* RestClient client = new RestClient("https://www.instagram.com");
             RestRequest getRequest = new RestRequest("accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9be620147bce430fb13c5c9421206f46%26redirect_uri%3Dhttps%3A//www.getpostman.com/oauth2/callback%26response_type%3Dcode", Method.GET);
             IRestResponse getResponse = client.Execute(getRequest);

             string csrftoken = OAuth2Helper.GetSpecificValueFromCookies(getResponse, "csrftoken");
             Console.WriteLine(csrftoken);

             RestRequest postRequest = new RestRequest("accounts/login/?force_classic_login=&next=/oauth/authorize/%3Fclient_id%3D9be620147bce430fb13c5c9421206f46%26redirect_uri%3Dhttps%3A//www.getpostman.com/oauth2/callback%26response_type%3Dcode", Method.POST);
             postRequest.AddCookie("csrftoken", csrftoken);
             postRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
             postRequest.AddParameter("application/x-www-form-urlencoded",
                 $"csrfmiddlewaretoken={csrftoken}&username=&password=", 
                 ParameterType.RequestBody);
             IRestResponse postResponse = client.Execute(getRequest);

             RestRequest getRequest2 = new RestRequest("oauth/authorize/%3Fclient_id%3D9be620147bce430fb13c5c9421206f46%26redirect_uri%3Dhttps%3A//www.getpostman.com/oauth2/callback%26response_type%3Dcode", Method.GET);
             getRequest2.AddCookie("csrftoken", csrftoken);
             IRestResponse getResponse2 = client.Execute(getRequest);*/

            /* RestClient client = new RestClient("https://bitly.com");
             RestRequest getRequest = new RestRequest("oauth/authorize?client_id=967be650896308c8dea967f02a08253a261abdeb&redirect_uri=https://www.getpostman.com/oauth2/callback", Method.GET);
             IRestResponse getResponse = client.Execute(getRequest);

             string _xsrf = OAuth2Helper.GetSpecificValueFromCookies(getResponse, "_xsrf");
             string oauth = OAuth2Helper.GetSpecificValueFromCookies(getResponse, "oauth");
             Console.WriteLine(_xsrf);

             RestRequest postRequest = new RestRequest("a/sign_in?rd=%2Foauth%2Fauthorize%3Fstate%3D%26redirect_uri%3Dhttps%253A%252F%252Fwww.getpostman.com%252Foauth2%252Fcallback%26client_id%3D967be650896308c8dea967f02a08253a261abdeb", Method.POST);
             postRequest.AddCookie("_xsrf", _xsrf);

             postRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
             postRequest.AddParameter("application/x-www-form-urlencoded",
                 $"username=TestTaskZel&password=Test1234Test&rd=/oauth/authorize?state=&redirect_uri=https%3A%2F%2Fwww.getpostman.com%2Foauth2%2Fcallback&client_id=967be650896308c8dea967f02a08253a261abdeb&_xsrf={_xsrf}",
                 ParameterType.RequestBody);
             IRestResponse postResponse = client.Execute(getRequest);


             RestRequest postRequest2 = new RestRequest("oauth/authorize", Method.POST);
             postRequest2.AddCookie("_xsrf", _xsrf);
             postRequest2.AddHeader("cache-control", "no-cache");
             postRequest2.AddHeader("content-type", "application/x-www-form-urlencoded");
             postRequest2.AddParameter("application/x-www-form-urlencoded", $"_xsrf={_xsrf}&umdb_brand_guid=Bi14cSkip9y&redirect_uri=https%3A%2F%2Fwww.getpostman.com%2Foauth2%2Fcallback&client_id=967be650896308c8dea967f02a08253a261abdeb&state=&action=Allow", ParameterType.RequestBody);
             IRestResponse postResponse2 = client.Execute(postRequest2);*/

            /*RestClient client = new RestClient("https://oauth.vk.com/authorize?client_id=6313844&redirect_uri=https://www.getpostman.com/oauth2/callback");
            
            RestRequest getRequest = new RestRequest(Method.GET);
            IRestResponse getResponse = client.Execute(getRequest);
            var doc = new HtmlDocument();
            doc.LoadHtml(getResponse.Content);
            var inputs = doc.DocumentNode.Descendants("input");
            Dictionary<string, string> hiddenInputValues = new Dictionary<string, string>();
            foreach (var node in inputs)
            {
                string name = node.GetAttributeValue("name", string.Empty);
                string value = node.GetAttributeValue("value", string.Empty);
                hiddenInputValues.Add(name, value);
            }

            RestRequest postRequest = new RestRequest(Method.POST);
            postRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            postRequest.AddParameter("application/x-www-form-urlencoded",
                $"ip_h={hiddenInputValues["ip_h"]}&lg_h={hiddenInputValues["lg_h"]}&_origin={hiddenInputValues["_origin"]}&to={hiddenInputValues["to"]}&expire=0&email=&pass=",
                ParameterType.RequestBody);

            IRestResponse postResponse = client.Execute(postRequest);*/

            /*RestClient client = new RestClient("https://api.imgur.com");
            RestRequest reqForSpecValue = new RestRequest("oauth2/authorize?response_type=code&client_id=0f624d4e6985af4", Method.GET);

            IRestResponse resWithSpecValue = client.Execute(reqForSpecValue);

            string specificValueName = "authorize_token";
            string specialParamValue = OAuth2Helper.GetSpecificValueFromCookies(resWithSpecValue, specificValueName);

            RestRequest reqForCode = new RestRequest("oauth2/authorize?response_type=code&client_id=0f624d4e6985af4", Method.POST);
            reqForCode.AddCookie(specificValueName, specialParamValue);
            reqForCode.AddHeader("content-type", "application/x-www-form-urlencoded");
            reqForCode.AddParameter("application/x-www-form-urlencoded",
                $"username=TestTaskZel&password=Test1234Test&allow={specialParamValue}",
                ParameterType.RequestBody);

            IRestResponse resWithCode = client.Execute(reqForCode);

            string code = OAuth2Helper.GetUriElement(resWithCode.ResponseUri.Query, "code");

            RestRequest reqForToken = new RestRequest("oauth2/token", Method.POST);
            reqForToken.AddCookie(specificValueName, specialParamValue);
            reqForToken.AddHeader("content-type", "application/x-www-form-urlencoded");
            reqForToken.AddParameter("application/x-www-form-urlencoded",
                $"client_id=0f624d4e6985af4&client_secret=6ff9c38a4c91b88a68f8cc970a3dc88e7c585839&grant_type=authorization_code&code={code}",
                ParameterType.RequestBody);

            IRestResponse resWithToken = client.Execute(reqForToken);*/
        }
    }
}
