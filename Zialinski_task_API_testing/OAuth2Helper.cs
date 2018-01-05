using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace Zialinski_task_API_testing
{
    class OAuth2Helper
    {
        public static string GetUriElement(string uriOrUriPart, string elemName)
        {
            string[] elemsFromUriOrUriPart = uriOrUriPart.Split('&', '=', '#', '?');
            string elem = elemsFromUriOrUriPart[Array.IndexOf(elemsFromUriOrUriPart, elemName) + 1];
            return elem;
        }

        public static string GetSpecificValueFromCookies(IRestResponse response, string specificName)
        {
            try
            {
                return response.Cookies.First(item => item.Name == specificName).Value;
            }
            catch (Exception)
            {
                throw new Exception("No such element "+specificName+" in collection");
            }
        }

        private static string GetAccessTokenFromJSONContent(string content)
        {
            JObject result = JObject.Parse(content);
            string accessToken = result["access_token"].ToString();
            return accessToken;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static IRestResponse ImgurAuth(string responseType, string clientId, string username, string password)
        {
            RestClient client = new RestClient($"https://api.imgur.com/oauth2/authorize?response_type={responseType}&client_id={clientId}");
            RestRequest reqForSpecValue = new RestRequest(Method.GET);

            IRestResponse resWithSpecValue = client.Execute(reqForSpecValue);

            string specificValueName = "authorize_token";
            string specialParamValue = GetSpecificValueFromCookies(resWithSpecValue, specificValueName);

            RestRequest request = new RestRequest(Method.POST);
            request.AddCookie(specificValueName, specialParamValue);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded",
                $"username={username}&password={password}&allow={specialParamValue}",
                ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            return response;
        }

        public static string GetImgurTokenImplicit(string clientId, string username, string password)
        {
            IRestResponse resWithToken = ImgurAuth("token", clientId, username, password); 

            string token = GetUriElement(resWithToken.ResponseUri.Fragment, "access_token");
            Assert.True(!string.IsNullOrEmpty(token), "Token was not presented");
            return token;
        }

        public static string GetImgurTokenThrowCode(string clientId, string clientSecret, string username, string password)
        {
            IRestResponse resWithCode = ImgurAuth("code", clientId, username, password);
            string code = GetUriElement(resWithCode.ResponseUri.Query, "code");
            Assert.True(!string.IsNullOrEmpty(code), "Token was not presented");

            RestClient client = new RestClient("https://api.imgur.com");
            RestRequest reqForToken = new RestRequest("oauth2/token", Method.POST);
            reqForToken.AddHeader("content-type", "application/x-www-form-urlencoded");
            reqForToken.AddParameter("application/x-www-form-urlencoded",
                $"client_id={clientId}&client_secret={clientSecret}&grant_type=authorization_code&code={code}",
                ParameterType.RequestBody);

            IRestResponse resWithToken = client.Execute(reqForToken);

            string accessToken = GetAccessTokenFromJSONContent(resWithToken.Content);
            Assert.True(!string.IsNullOrEmpty(accessToken), "Token was not presented");
            return accessToken;
        }

        public static string GetSpotifyTokenThrowClientCredentials(string clientId, string clientSecret)
        {
            string credentialsForEncode = clientId + ":" + clientSecret;
            RestClient client = new RestClient("https://accounts.spotify.com/api/token");
            RestRequest reqForToken = new RestRequest(Method.POST);
            reqForToken.AddHeader("authorization", "Basic "+Base64Encode(credentialsForEncode));
            reqForToken.AddHeader("content-type", "application/x-www-form-urlencoded");
            reqForToken.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials", ParameterType.RequestBody);
            IRestResponse resWithToken = client.Execute(reqForToken);

            string accessToken = GetAccessTokenFromJSONContent(resWithToken.Content);
            Assert.True(!string.IsNullOrEmpty(accessToken), "Token was not presented");
            return accessToken;
        }

        public static string GetBitlyTokenThrowResourceOwnerPassword(string clientId, string clientSecret, string username, string password)
        {
            string credentialsForEncode = clientId + ":" + clientSecret;
            RestClient client = new RestClient("https://api-ssl.bitly.com/oauth/access_token");
            IRestRequest reqForToken = new RestRequest(Method.POST);
            reqForToken.AddHeader("Authorization",
                "Basic " + Base64Encode(credentialsForEncode));
            reqForToken.AddHeader("content-type", "application/x-www-form-urlencoded");
            reqForToken.AddParameter("application/x-www-form-urlencoded", 
                $"grant_type=password&username={username}&password={password}", 
                ParameterType.RequestBody);
            IRestResponse resWithToken = client.Execute(reqForToken);

            string accessToken = GetAccessTokenFromJSONContent(resWithToken.Content);
            Assert.True(!string.IsNullOrEmpty(accessToken), "Token was not presented");
            return accessToken;
        }
    }
}
