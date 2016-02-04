using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.Payment;
using infrastructure.Utility;

namespace Application.Payment
{
    public class StripeService
    {
        public const string key = "sk_test_wAtHOoX8R1QkEiCr5O9ZbZu1";
        public async Task<PaymentEvent> CreateCharge(string token, decimal amount, string description)
        {
            using (var httpClient = new HttpClient())
            {

                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("amount", ((uint)(amount * 100)).ToString()));
                nvc.Add(new KeyValuePair<string, string>("currency", "aud"));
                nvc.Add(new KeyValuePair<string, string>("description", description));
                nvc.Add(new KeyValuePair<string, string>("source", token));
                HttpContent content = new FormUrlEncodedContent(nvc);
                var response = await httpClient.DoPostFormEncodedAsync<ChargeResponse>("https://api.stripe.com/v1/charges", content, "Basic", key);

                var result = new PaymentEvent()
                {
                    PaymentDateTime = DateTime.UtcNow,
                    ExternalId = response.id,
                    Gateway = "Stripe",
                    Status = response.status,
                    TransactionSuccess = response.status.Equals("succeeded"),
                    Type = "payment",
                };

                return result;
            }


        }

        public async Task<PaymentEvent> Refund(string externalId)
        {
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("charge", externalId));
            HttpContent content = new FormUrlEncodedContent(nvc);
            using (var httpClient = new HttpClient())
            {
                var response =
                    await
                        httpClient.DoPostFormEncodedAsync<RefundResponse>("https://api.stripe.com/v1/refunds", content,
                            "Basic", key);


                var result = new PaymentEvent()
                {
                    PaymentDateTime = DateTime.UtcNow,
                    ExternalId = response.id,
                    Gateway = "Stripe",
                    Status = "",
                    TransactionSuccess = true,
                    Type =response.@object,
                    ExtraInfo = response.charge
                };

                return result;
            }

            
        }
    }


    public class RefundResponse
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int amount { get; set; }
        public object balance_transaction { get; set; }
        public string charge { get; set; }
        public int created { get; set; }
        public string currency { get; set; }
        public Metadata metadata { get; set; }
        public object reason { get; set; }
        public object receipt_number { get; set; }
    }


    public class CreateCharge
    {
        public uint amount { get; set; }
        public string currency { get; set; }
        public string source { get; set; }
        public string description { get; set; }
    }


    public class ChargeResponse
    {
        public string id { get; set; }
        public string _object { get; set; }
        public int created { get; set; }
        public bool livemode { get; set; }
        public bool paid { get; set; }
        public string status { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public bool refunded { get; set; }
        public Source source { get; set; }
        public bool captured { get; set; }
        public string balance_transaction { get; set; }
        public object failure_message { get; set; }
        public object failure_code { get; set; }
        public int amount_refunded { get; set; }
        public object customer { get; set; }
        public object invoice { get; set; }
        public string description { get; set; }
        public object dispute { get; set; }
        public Metadata1 metadata { get; set; }
        public object statement_descriptor { get; set; }
        public Fraud_Details fraud_details { get; set; }
        public object receipt_email { get; set; }
        public object receipt_number { get; set; }
        public object shipping { get; set; }
        public object destination { get; set; }
        public object application_fee { get; set; }
        public Refunds refunds { get; set; }
    }

    public class Source
    {
        public string id { get; set; }
        public string _object { get; set; }
        public string last4 { get; set; }
        public string brand { get; set; }
        public string funding { get; set; }
        public int exp_month { get; set; }
        public int exp_year { get; set; }
        public string country { get; set; }
        public object name { get; set; }
        public object address_line1 { get; set; }
        public object address_line2 { get; set; }
        public object address_city { get; set; }
        public object address_state { get; set; }
        public object address_zip { get; set; }
        public object address_country { get; set; }
        public string cvc_check { get; set; }
        public object address_line1_check { get; set; }
        public object address_zip_check { get; set; }
        public object tokenization_method { get; set; }
        public object dynamic_last4 { get; set; }
        public Metadata metadata { get; set; }
        public object customer { get; set; }
    }

    public class Metadata
    {
    }

    public class Metadata1
    {
    }

    public class Fraud_Details
    {
    }

    public class Refunds
    {
        public string _object { get; set; }
        public int total_count { get; set; }
        public bool has_more { get; set; }
        public string url { get; set; }
        public object[] data { get; set; }
    }

}