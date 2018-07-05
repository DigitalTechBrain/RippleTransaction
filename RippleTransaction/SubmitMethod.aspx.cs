using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RippleTransaction
{
    public partial class SubmitMethod : System.Web.UI.Page
    {
        public JObject InvokeMethod()
        {
            // HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://s.altnet.rippletest.net:51234");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://s1.ripple.com:51234/");

            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";


            string json = @"
            {

              'method': 'submit',
             'params': [{
            'secret': '',
            'tx_json': {
                'Account': 'rf1BiGeXwwQoi8Z2ueFYTEXSwuJYfV2Jpn',
                'TransactionType': 'PaymentChannelClaim',
                'Amount': '1000000',
                'Balance': '1000000',
                'Channel': '5DB01B7FFED6B67E6B0414DED11E051D2EE2B7619CE0EAA6286D67A3A4D5BDB3',
                'PublicKey': '023693F15967AE357D0327974AD46FE3C127113B1110D6044FD41E723689F81CC6',
                'Signature': '304402204EF0AFB78AC23ED1C472E74F4299C0C21F1B21D07EFC0A3838A420F76D783A400220154FB11B6F54320666E4C36CA7F686C16A3A0456800BBC43746F34AF50290064'
            },
            'fee_mult_max': 1000
        }]

           }";


            JObject joe = JObject.Parse(json);
            string s = JsonConvert.SerializeObject(joe);


            // serialize json for the requestf
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;

            try
            {
                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (WebException we)
            {
                //inner exception is socket
                //{"A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond 23.23.246.5:8332"}
                throw we;
            }

            WebResponse webResponse = null;
            try
            {
                using (webResponse = webRequest.GetResponse())
                {
                    using (Stream str = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException webex)
            {

                using (Stream str = webex.Response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str))
                    {
                        var tempRet = JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                        return tempRet;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var ret = InvokeMethod();
            Response.Write(ret);
        }
    }
}