//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Canti.Data;

namespace Canti.Blockchain.RPC
{
    public class MethodBuilder
    {
        private string Method { get; set; }
        private string Password { get; set; }
        private Dictionary<string, object> Params { get; set; }
        private Dictionary<string, object> ExternalParams { get; set; }
        public MethodBuilder(string Method, string Password = "")
        {
            this.Method = Method;
            this.Password = Password;
            Params = new Dictionary<string, object>();
        }
        public void AddParam(string Name, object Value)
        {
            Params.Add(Name, Value);
        }
        public void AddExternalParam(string Name, object Value)
        {
            ExternalParams.Add(Name, Value);
        }
        public override string ToString()
        {
            string Output = "{\"jsonrpc\":\"2.0\",\"id\":1,\"password\":\"" + Password + "\",\"method\":\"" + Method + "\",\"params\":{";
            int ParamCount = 0;
            foreach (KeyValuePair<string, object> Param in Params)
            {
                ParamCount++;
                Output += "\"" + Param.Key + "\":" + Serialization.SerializeObjectToJson(Param.Value);
                if (ParamCount < Params.Count) Output += ",";
            }
            Output += "}";
            foreach (KeyValuePair<string, object> Param in ExternalParams)
                Output += ",\"" + Param.Key + "\":" + Serialization.SerializeObjectToJson(Param.Value);
            Output += "}";
            return Output;
        }
    }

    public partial class Methods
    {
        private static string SendRequest(string Destination, string Data)
        {
            string Output = "{}";
            try
            {
                HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(Destination);
                HttpWebRequest.ContentType = "application/json-rpc";
                HttpWebRequest.Method = "POST";
                byte[] ByteArray = System.Text.Encoding.UTF8.GetBytes(Data);
                HttpWebRequest.ContentLength = ByteArray.Length;
                Stream Stream = HttpWebRequest.GetRequestStream();
                Stream.Write(ByteArray, 0, ByteArray.Length);
                Stream.Close();
                using (WebResponse WebResponse = HttpWebRequest.GetResponse())
                using (StreamReader reader = new StreamReader(WebResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                    Output = reader.ReadToEnd();
            }
            catch (Exception Error)
            {
                Output = "{\"error\":{\"message\":\"" + Error.Message + "\"}}";
            }
            return Output;
        }

        private static string ReadEndpoint(string Destination, string Endpoint)
        {
            try
            {
                using (WebClient WebClient = new WebClient()) return WebClient.DownloadString(Destination + Endpoint);
            }
            catch (Exception Error)
            {
                return "{\"error\":{\"message\":\"" + Error.Message + "\"}}";
            }
        }
    }
}
