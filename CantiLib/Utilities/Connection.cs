//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

namespace Canti.Utilities
{
    public class Connection
    {
        public string Host { get; private set; }
        public string Endpoint{ get; private set; }
        public string Password { get; private set; }
        public int Port { get; private set; }
        public Connection(string Host, int Port, string Endpoint = "", string Password = "")
        {
            this.Host = Host;
            this.Port = Port;
            this.Endpoint = Endpoint;
            this.Password = Password;
        }
        public string ToString(bool IgnoreEndpoint = false)
        {
            string Output = "http://" + Host + ":" + Port + "/";
            if (!IgnoreEndpoint) Output += Endpoint;
            return Output;
        }
    }
}
