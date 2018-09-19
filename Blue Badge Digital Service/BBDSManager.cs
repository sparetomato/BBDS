using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Blue_Badge_Digital_Service
{
    class BBDSManager
    {
        private readonly HttpClient client = new HttpClient();
        private string bearerToken;

        public void GetNewApplications()
        {
            var baseUri = new Uri(ConfigurationManager.AppSettings["BBDSURI"]);
            string startOfTheMonth = "2018-09-01T00:00Z";
            var requestApplicationsData = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUri, "applications?from=" + startOfTheMonth)
            };
            requestApplicationsData.Headers.TryAddWithoutValidation("Authorization", String.Format("Bearer {0}", bearerToken));
            HttpResponseMessage results = client.SendAsync(requestApplicationsData).Result;
            JObject resultData = JObject.Parse(results.Content.ReadAsStringAsync().Result);// resultData is the JSON object, use Nortonsoft to parse this into individual calls
            Console.WriteLine(resultData["data"].Count() + " Applications received");
            //for(int i = 0;i<resultData["data"].Count(); i++)
            for(int i=0;i<1;i++)
            {
                Console.WriteLine("Getting Application Data for ID:" + resultData["data"][i]["applicationId"] + " : " + resultData["data"][i]["name"]);
                var requestFullApplicationData = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(baseUri, "applications/" + resultData["data"][i]["applicationId"])
                };
                requestFullApplicationData.Headers.TryAddWithoutValidation("Authorization", String.Format("Bearer {0}", bearerToken));
                
                HttpResponseMessage applicationResult = client.SendAsync(requestFullApplicationData).Result;
                JObject applicationResultData = JObject.Parse(applicationResult.Content.ReadAsStringAsync().Result);
                Console.WriteLine(applicationResult.Content.ReadAsStringAsync().Result);
                BBDSApplication application = new BBDSApplication((JObject)applicationResultData["data"]);
                //CreateFirmstepCaseFromApplication(application);
            }
        }

        private void CreateFirmstepCaseFromApplication(BBDSApplication application)
        {
            String sampleJSON =
                "{\"process_id\": \"AF-Process-b035fb86-7458-4554-a9d7-5681fa2c3c81\",\"data\" :{\"test\":\"test\"},\"submissionType\" : \"new\",\"published\" : \"true\",\"ucrn\": \"768255944\"}";

            JObject startThreadJSON = new JObject();
            startThreadJSON.Add("process_id", "AF-Process-b035fb86-7458-4554-a9d7-5681fa2c3c81");
            
            JObject applicationData = new JObject();
            applicationData.Add("first_name", application.party.person.badgeHolderName);
            applicationData.Add("Date_of_Birth", application.party.person.dob);
            applicationData.Add("FullName",application.party.person.badgeHolderName);
            applicationData.Add("nino",application.party.person.nino);
            applicationData.Add("house", application.party.contact.buildingStreet);
            applicationData.Add("AddressLine2", application.party.contact.line2);
            applicationData.Add("town", application.party.contact.townCity);
            applicationData.Add("postcode", application.party.contact.postCode);
            
            startThreadJSON.Add("data",applicationData);
            startThreadJSON.Add("submissionType", "new");
            startThreadJSON.Add("published", "true");
            startThreadJSON.Add("ucrn", "768255944");

            var startThreadAPIMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(ConfigurationManager.AppSettings["FS-StartthreadAPIUrl"] + "?apiKey=" + ConfigurationManager.AppSettings["FS-StartthreadAPIKey"]),
                Method = HttpMethod.Post,
            };
            startThreadAPIMessage.Content = new StringContent(startThreadJSON.ToString(Formatting.None));
            startThreadAPIMessage.Headers.TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            startThreadAPIMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
            //startThreadAPIMessage.Headers.TryAddWithoutValidation("Authorization", String.Format("Bearer {0}", bearerToken));
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            HttpClient firmstepClient = new HttpClient(handler);
            
           // HttpResponseMessage applicationResult = firmstepClient.SendAsync(startThreadAPIMessage).Result;
            //Console.WriteLine(applicationResult);
            //using (CookieAwareWebClient client = new CookieAwareWebClient())
            //{
            //    NameValueCollection values = new NameValueCollection();
            //    byte[] response =
            //        client.UploadValues(
            //            ConfigurationManager.AppSettings["FS-StartthreadAPIUrl"] + "?apiKey=" +
            //            ConfigurationManager.AppSettings["FS-StartthreadAPIKey"], values);
            //    Console.WriteLine(Encoding.Default.GetString(response));
            //}


            
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        internal void getoAuthToken()
        {
            var baseUri = new Uri(ConfigurationManager.AppSettings["BBDSURI"]);
            var encodedConsumerKey = ConfigurationManager.AppSettings["BBDSUser"];
            var encodedConsumerKeySecret = ConfigurationManager.AppSettings["BBDSSecret"];
            var encodedPair = Base64Encode(String.Format("{0}:{1}", encodedConsumerKey, encodedConsumerKeySecret));

            var requestToken = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUri, "oauth/token"),
                Content = new StringContent("grant_type=client_credentials")
            };
            requestToken.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded") {CharSet = "UTF-8"};
            requestToken.Headers.TryAddWithoutValidation("Authorization", String.Format("Basic {0}", encodedPair));
            
            

            var bearerResult = client.SendAsync(requestToken).Result;
            var bearerData = bearerResult.Content;
            //bearerToken = bearerData.ReadAsStringAsync().Result;
            bearerToken = JObject.Parse(bearerData.ReadAsStringAsync().Result)["access_token"].ToString();

            

            Console.WriteLine(bearerToken);

        }


    }
}
