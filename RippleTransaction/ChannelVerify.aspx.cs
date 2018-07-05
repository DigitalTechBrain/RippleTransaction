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
    public partial class ChannelVerify : System.Web.UI.Page
    {
        public JObject InvokeMethod()
        {
            // HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://s.altnet.rippletest.net:51234");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://s1.ripple.com:51234/");

            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";


            string json = @"
            {

             'method': 'channel_verify',
        'params': [{
        'channel_id': '6E4BC9B9A4B37054EE577AC84837402AE9DA40D5559A84AF3A609FAB22E98B93',
        'signature': '304402207C3BB0856EEE58122FA19BC3673904DB6A69EC488B6132A0526D1A1FD34D9A2E022015F5781F7DAAD5F7A158E107F8B6147F22782FF70F68186AC412F2DEA260A4A1',
        'public_key': 'aB44YfzW24VDEJQ2UuLPV2PvqcPCSoLnL7y5M1EzhdW4LnK5xMS3',
        'amount': '10000000'
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