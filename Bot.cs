using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DisCatSharp;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Exceptions;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.ApplicationCommands;

using DisCatSharp.Support.Entities;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace DisCatSharp.Support
{
    /// <summary>
    /// The bot.
    /// </summary>
    internal class Bot : IDisposable
    {
        /// <summary>
        /// Gets the config.
        /// </summary>
        public static Config Config { get; internal set; }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        public static LogLevel LogLevel { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bot"/> class.
        /// </summary>
        public Bot(LogLevel logLevel)
        {
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"config.json"));
            LogLevel = logLevel;

        }


        /// <summary>
        /// Disposes the bot.
        /// </summary>
        public void Dispose()
        {/*
            Conduit = null;
            INext = null;
            CNext = null;
            ApplicationCommands = null;
            Client = null;*/
        }
    }
}
