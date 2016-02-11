using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.Payment;
using infrastructure.Utility;
using Newtonsoft.Json;

namespace Application.Payment
{
    public class PaypalService
    {
        protected string PaypalClientId;
        protected string PaypalSecret;
        protected string PaypalEndpoint;
        protected string PaypalSuccessurl;
        protected string PaypalFailurl;

        public PaypalService()
        {
            PaypalClientId = ConfigurationManager.AppSettings["PaypalClientId"];
            PaypalSecret = ConfigurationManager.AppSettings["PaypalSecret"];
            PaypalEndpoint = ConfigurationManager.AppSettings["PaypalEndpoint"];
            PaypalSuccessurl = ConfigurationManager.AppSettings["PaypalSuccessurl"];
            PaypalFailurl = ConfigurationManager.AppSettings["PaypalFailurl"];

        }

        protected async Task<string> GetAccessToken()
        {
            using (var httpClient = new HttpClient())
            {
                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                HttpContent content = new FormUrlEncodedContent(nvc);

                var token =
                    await
                        httpClient.DoPostFormEncodedAsync<TokenResponse>(PaypalEndpoint + "/v1/oauth2/token", content, "Basic",
                            PaypalClientId + ":" + PaypalSecret);
                return token.access_token;
            }
        }

        public async Task<PaymentEvent> ExecutePayment(string paymentID, string payerId)
        {
            var token = await GetAccessToken();
            using (var httpClient = new HttpClient())
            {
                var response = await
                    httpClient.PostJsonAsync<ExecuteResponse, ExecuteRequest>(
                        PaypalEndpoint + "/v1/payments/payment/" + paymentID + "/execute/", new ExecuteRequest()
                        {
                            payer_id = payerId
                        }, "Bearer", token);


                return new PaymentEvent()
                {
                    ExternalId = response.transactions.First().related_resources.First().sale.id,
                    Gateway = "Paypal",
                    PaymentDateTime = DateTime.UtcNow,
                    Type = "ExecutePayment",
                    Status = response.state,
                    TransactionSuccess = response.state.Equals("approved"),
                    ExtraInfo = JsonConvert.SerializeObject(response)
                };
            }


        }

        public async Task<string> GetRedirectUrl(decimal total, string description, string successUrl, string failUrl)
        {
            var token = await GetAccessToken();
            var createPaymentRequest = new CreatePaymentRequest()
            {
                intent = "sale",
                payer = new CreatePaymentRequest.Payer()
                {
                    payment_method = "paypal"
                },
                redirect_urls = new CreatePaymentRequest.Redirect_Urls()
                {
                    cancel_url = failUrl,
                    return_url = successUrl,
                },
                transactions = new CreatePaymentRequest.Transaction[]
                {
                    new CreatePaymentRequest.Transaction()
                    {
                        description = description,
                        amount = new CreatePaymentRequest.Amount()
                        {
                            currency = "AUD",
                            total = total.ToString()
                        }
                    },
                }
            };

            using (var httpClient = new HttpClient())
            {
                var response = await
                    httpClient.PostJsonAsync<CreatePaymentresponse, CreatePaymentRequest>(
                        PaypalEndpoint + "/v1/payments/payment ", createPaymentRequest, "Bearer", token);

                return response.links[1].href;
            }
        }

        public async Task<PaymentEvent> Refund(string externalId)
        {
            var token = await GetAccessToken();
            using (var httpClient = new HttpClient())
            {
                var response = await
                    httpClient.PostJsonAsync<PaypalRefundResponse, PaypalRefundRequest>(
                        PaypalEndpoint + $"/v1/payments/sale/{externalId}/refund", new PaypalRefundRequest(), "Bearer", token);


                return new PaymentEvent()
                {
                    ExternalId = response.id,
                    Gateway = "Paypal",
                    PaymentDateTime = DateTime.UtcNow,
                    Type = "Refund",
                    Status = response.state,
                    TransactionSuccess = response.state.Equals("completed"),
                    ExtraInfo = JsonConvert.SerializeObject(response)
                };
            }
        }
    }

    public class ExecuteRequest
    {
        public string payer_id { get; set; }
    }

    public class PaypalRefundRequest
    {
        
    }

    public class PaypalRefundResponse
    {
        public string id { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public string state { get; set; }
        public Amount amount { get; set; }
        public string sale_id { get; set; }
        public string parent_payment { get; set; }
        public Link[] links { get; set; }
    }

    public class Amount
    {
        public string total { get; set; }
        public string currency { get; set; }
    }

    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }



    public class ExecuteResponse
    {
        public string id { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public string state { get; set; }
        public string intent { get; set; }
        public Payer payer { get; set; }
        public Transaction[] transactions { get; set; }
        public Link1[] links { get; set; }

        public class Payer
        {
            public string payment_method { get; set; }
            public Payer_Info payer_info { get; set; }
        }

        public class Payer_Info
        {
            public string email { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string payer_id { get; set; }
        }

        public class Transaction
        {
            public Amount amount { get; set; }
            public string description { get; set; }
            public Related_Resources[] related_resources { get; set; }
        }

        public class Amount
        {
            public string total { get; set; }
            public string currency { get; set; }
            public Details details { get; set; }
        }

        public class Details
        {
            public string tax { get; set; }
            public string shipping { get; set; }
        }

        public class Related_Resources
        {
            public Sale sale { get; set; }
        }

        public class Sale
        {
            public string id { get; set; }
            public DateTime create_time { get; set; }
            public DateTime update_time { get; set; }
            public string state { get; set; }
            public Amount1 amount { get; set; }
            public Transaction_Fee transaction_fee { get; set; }
            public string parent_payment { get; set; }
            public Link[] links { get; set; }
        }

        public class Amount1
        {
            public string currency { get; set; }
            public string total { get; set; }
        }

        public class Transaction_Fee
        {
            public string value { get; set; }
            public string currency { get; set; }
        }

        public class Link
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }

        public class Link1
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }
    }





    public class CreatePaymentRequest
    {
        public string intent { get; set; }
        public Payer payer { get; set; }
        public Transaction[] transactions { get; set; }
        public Redirect_Urls redirect_urls { get; set; }

        public class Payer
        {
            public string payment_method { get; set; }
            public Funding_Instruments[] funding_instruments { get; set; }
        }

        public class Funding_Instruments
        {
            public Credit_Card credit_card { get; set; }
        }

        public class Credit_Card
        {
            public string number { get; set; }
            public string type { get; set; }
            public int expire_month { get; set; }
            public int expire_year { get; set; }
            public string cvv2 { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public Billing_Address billing_address { get; set; }
        }

        public class Billing_Address
        {
            public string line1 { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string postal_code { get; set; }
            public string country_code { get; set; }
        }

        public class Transaction
        {
            public Amount amount { get; set; }
            public string description { get; set; }
        }

        public class Amount
        {
            public string total { get; set; }
            public string currency { get; set; }
            //public Details details { get; set; }
        }

        public class Details
        {
            public string subtotal { get; set; }
            public string tax { get; set; }
            public string shipping { get; set; }
        }


        public class Redirect_Urls
        {
            public string return_url { get; set; }
            public string cancel_url { get; set; }
        }

    }




    public class CreatePaymentresponse
    {
        public string id { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public string state { get; set; }
        public string intent { get; set; }
        public Payer payer { get; set; }
        public Transaction[] transactions { get; set; }
        public Link[] links { get; set; }

        public class Payer
        {
            public string payment_method { get; set; }
        }

        public class Transaction
        {
            public Amount amount { get; set; }
            public string description { get; set; }
        }

        public class Amount
        {
            public string total { get; set; }
            public string currency { get; set; }
            public Details details { get; set; }
        }

        public class Details
        {
            public string subtotal { get; set; }
        }

        public class Link
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }
    }





    public class TokenResponse
    {
        public string scope { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public int expires_in { get; set; }
    }

}