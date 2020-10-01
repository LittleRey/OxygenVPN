﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OxygenVPN.Models
{
    public interface IServerUtil
    {
        /// <summary>
        ///     Collection order basis
        /// </summary>
        ushort Priority { get; }

        /// <summary>
        ///     Server.Type
        /// </summary>
        string TypeName { get; }

        /// <summary>
        ///     Protocol Name
        /// </summary>
        string FullName { get; }

        /// <summary>
        ///     Support URI
        /// </summary>
        string[] UriScheme { get; }

        Server ParseJObject(JObject j);

        public void Edit(Server s);

        public void Create();

        string GetShareLink(Server server);

        public abstract ServerController GetController();

        public abstract IEnumerable<Server> ParseUri(string text);

        bool CheckServer(Server s);
    }
}