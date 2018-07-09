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
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://s.altnet.rippletest.net:51234");
           // HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://s1.ripple.com:51234/");

            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";


            string json = @"
            {

             'method': 'channel_verify',
        'params': [{
        'channel_id': '351C0E602430D6E8499017C10338EE9ED998D2F26B49D422F305AA060747C1C9',
        'signature': '30450221008BFA28FBCA4B7D93E185277763AD0ED9659F2EE8820F5EFF330EE8BB0B07489F0220276FD5599780055C89C890234219513725EE3FA79DACCCD9C3AA6FCC97D30FD4',
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