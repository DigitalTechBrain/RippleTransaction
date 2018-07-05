﻿using Newtonsoft.Json;
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
    public partial class Default : System.Web.UI.Page
    {
        public JObject InvokeMethod(string a_sMethod)
        {
            // HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://s.altnet.rippletest.net:51234");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://s1.ripple.com:51234/");

            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";

           
            string json = @"
            {

             'method': 'account_channels',
        'params': [{
        'account': 'rfginNsDt73fZtzp858dT6Y6U67Y1QB7GV',
        'destination_account': 'rPspuKM5rCw5EkRDD9vGL816V15DwtSa3L',
        'ledger_index': 'validated'
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
            var ret = InvokeMethod("ping");
            Response.Write(ret);
        }
    }
}