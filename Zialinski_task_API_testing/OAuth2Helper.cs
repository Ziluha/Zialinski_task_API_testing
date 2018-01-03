﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Zialinski_task_API_testing
{
    class OAuth2Helper
    {
        private static string GetUriElement(string uriOrUriPart, string elemName)
        {
            string[] elemsFromUriOrUriPart = uriOrUriPart.Split('&', '=', '#', '?');
            string elem = elemsFromUriOrUriPart[Array.IndexOf(elemsFromUriOrUriPart, elemName) + 1];
            return elem;
        }

        public static string GetToken(string url, string username, string password, string specialCookie, string specialParam)
        {
            var client = new RestClient(url);
            var getRequest = new RestRequest(Method.GET);
            getRequest.AddQueryParameter("response_type", "token");

            IRestResponse getResponse = client.Execute(getRequest);

            string specialParamValue = getResponse.Cookies[0].Value;

            var postRequest = new RestRequest(Method.POST);
            postRequest.AddQueryParameter("response_type", "token");
            postRequest.AddCookie(specialCookie, specialParamValue);
            postRequest.AddHeader("cache-control", "no-cache");
            postRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            postRequest.AddParameter("application/x-www-form-urlencoded",
                $"username={username}&password={password}&{specialParam}={specialParamValue}",
                ParameterType.RequestBody);

            IRestResponse response = client.Execute(postRequest);
            
            string token = GetUriElement(response.ResponseUri.Fragment, "access_token");

            return token;
        }

    }
}
