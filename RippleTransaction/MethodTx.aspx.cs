using System;
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
    public partial class MethodTx : System.Web.UI.Page
    {
        public JObject InvokeMethod()
        {
            // HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://s.altnet.rippletest.net:51234");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://s1.ripple.com:51234/");

            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";


            string json = @"
            {

              'method': 'tx',
        'params': [{
        'transaction': '3F93C482C0BC2A1387D9E67DF60BECBB76CC2160AE98522C77AF0074D548F67D'
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