using System.Dynamic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using RestSharp;

namespace Zialinski_task_API_testing
{
    public class APITest
    {
        [Test]
        public void ValidSchemaAndObject()
        {
            var client = new RestClient("https://itunes.apple.com");
            var request = new RestRequest("lookup?id=909253", Method.GET);
            
            IRestResponse res = client.Execute(request);
            var content = res.Content; 
            
            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'resultCount': {'type':'number'},
                'results': {'type': 'array'}
              }
            }");
            
            JObject result = JObject.Parse(content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is not valid");
        }

        [Test]
        public void ValidSchemaAndObjectWithError()
        {
            var client = new RestClient("https://www.googleapis.com");
            var request = new RestRequest("customsearch/v1", Method.GET);

            IRestResponse res = client.Execute(request);
            var content = res.Content;
            
            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'error': {'type':'object'},
                'code': {'type':'number'},
                'message': {'type':'string'}
              }
            }");

            JObject result = JObject.Parse(content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is not valid");
        }

        [Test]
        public void ValidSchemaButNotObject()
        {
            var client = new RestClient("https://www.googleapis.com");
            var request = new RestRequest("customsearch/v1", Method.GET);

            IRestResponse res = client.Execute(request);
            var content = res.Content;
            
            JSchema schema = JSchema.Parse(@"{
              'type': 'array',
              'properties': {
                'error': {'type':'object'},
                'code': {'type':'number'},
                'message': {'type':'string'}
              }
            }");

            JObject result = JObject.Parse(content);

            bool valid = result.IsValid(schema);
            Assert.False(valid, "Result is valid");
        }

        [Test]
        public void ValidSchemaForPost()
        {
            var client = new RestClient("https://jsonplaceholder.typicode.com");
            var request = new RestRequest("posts", Method.POST);

            dynamic obj = new ExpandoObject();
            obj.title = "title";
            obj.body = "body";
            obj.userId = 2;

            request.AddObject(obj);

            IRestResponse res = client.Execute(request);
            var content = res.Content;
           
            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'id': {'type':'number'}
              }
            }");

            JObject result = JObject.Parse(content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is valid");
        }

        [Test]
        public void ErrorAPICheck()
        {
            var client = new RestClient("https://jsonplaceholder.typicode.com");

            var request = new RestRequest("posts", Method.POST);

            dynamic obj = new ExpandoObject();
            obj.title = "title";
            obj.body = "body";
            obj.userId = 2;

            request.AddObject(obj);

            IRestResponse res = client.Execute(request);
            var content = res.Content;

            JSchema schema = JSchema.Parse(@"{
              'type': 'object',
              'properties': {
                'id': {'type':'array'}
              }
            }");

            JObject result = JObject.Parse(content);

            bool valid = result.IsValid(schema);
            Assert.True(valid, "Result is valid");
        }
    }
}