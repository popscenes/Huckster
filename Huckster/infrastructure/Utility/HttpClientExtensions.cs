using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace infrastructure.Utility
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get { return new UTF8Encoding(false); }
        }
    }

    public static class HttpClientExtension
    {
        public static async Task<R> PostJsonAsync<R, T>(this HttpClient httpClient, string url, T requestObject, string authType = "", string authToken = "")
        {
            if (!authType.IsNullOrWhiteSpace() && !authToken.IsNullOrWhiteSpace())
            {
                var token = authToken;
                if (authType.Equals("basic", StringComparison.CurrentCultureIgnoreCase))
                    token = Convert.ToBase64String(Encoding.ASCII.GetBytes(authToken));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authType, token);
            }
            R responseT;
            try
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8,
                    "application/json");
                
                var httpRewsponse = await httpClient.PostAsync(url, httpContent);
                var jsonResponse = await httpRewsponse.Content.ReadAsStringAsync();
                responseT = JsonConvert.DeserializeObject<R>(jsonResponse);
            }
            catch (Exception exception)
            {

                throw exception;
            }

            return responseT;
        }

        public static async Task<R> PostXmlAsync<R, T>(this HttpClient httpClient, string url,
            T requestObject, string authType = "", string authToken = "", XmlSerializerNamespaces ns = null)
            where T : class
            where R : class
        {
            if (!authType.IsNullOrWhiteSpace() && !authToken.IsNullOrWhiteSpace())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authType,
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(authToken)));
            }
            //R responseT;
            try
            {
                var xmlContent = "";
                using (var writer = new Utf8StringWriter())
                {
                    new XmlSerializer(typeof(T)).Serialize(writer, requestObject, ns);
                    xmlContent = writer.ToString();
                }
                var httpContent = new StringContent(xmlContent, Encoding.UTF8,
                    "text/xml");

                var httpRewsponse = await httpClient.PostAsync(url, httpContent);
                //var xmlResponse = await httpRewsponse.Content.ReadAsStringAsync();
                var response = new XmlSerializer(typeof(R)).Deserialize(await httpRewsponse.Content.ReadAsStreamAsync()) as R;

                return response;
            }
            catch (Exception exception)
            {

                throw exception;
            }


        }

        public static async Task<R> PutJsonAsync<R, T>(this HttpClient httpClient, string url, T requestObject, string authType = "", string authToken = "")
        {
            if (!authType.IsNullOrWhiteSpace() && !authToken.IsNullOrWhiteSpace())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authType,
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(authToken)));
            }
            R responseT;
            try
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8,
                    "application/json");

                var httpRewsponse = await httpClient.PutAsync(url, httpContent);
                var jsonResponse = await httpRewsponse.Content.ReadAsStringAsync();
                responseT = JsonConvert.DeserializeObject<R>(jsonResponse);
            }
            catch (Exception exception)
            {

                throw exception;
            }

            return responseT;
        }
        public static async Task<T> GetJsonAsync<T>(this HttpClient httpClient, string url, string authType = "", string authToken = "")
        {
            if (!authType.IsNullOrWhiteSpace() && !authToken.IsNullOrWhiteSpace())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authType,
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(authToken)));
            }
            T responseT;
            try
            {
                var httpRewsponse = await httpClient.GetAsync(url);
                var jsonResponse = await httpRewsponse.Content.ReadAsStringAsync();
                responseT = JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            catch (Exception exception)
            {

                throw exception;
            }

            return responseT;

        }
        public static async Task<T> DoPostFormEncodedAsync<T>(this HttpClient httpClient, string url, HttpContent content, string authType = "", string authToken = "")
        {

            content.Headers.Clear();
            if (!authType.IsNullOrWhiteSpace() && !authToken.IsNullOrWhiteSpace())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authType,
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(authToken)));
            }
            T responseT;
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            try
            {
                var httpResponse = await httpClient.PostAsync(url, content);
                var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                responseT = JsonConvert.DeserializeObject<T>(jsonResponse);
            }
            catch (Exception exception)
            {

                throw exception;
            }

            return responseT;
        }

        public static async Task<T> PostFileWithJsonRespAsync<T>(this HttpClient httpClient, string url, string fileName, string fileType, byte[] file, string authType = "", string authToken = "")
        {
            if (!authType.IsNullOrWhiteSpace() && !authToken.IsNullOrWhiteSpace())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authType,
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(authToken)));
            }
            T responseT;

            try
            {
                using (var fileContent = new ByteArrayContent(file))
                {

                    fileContent.Headers.Clear();
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(fileType);

                    using (var httpResponse = await httpClient.PostAsync(url, fileContent))
                    {
                        var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
                        responseT = JsonConvert.DeserializeObject<T>(jsonResponse);

                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return responseT;
        }
    }
}
