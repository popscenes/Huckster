using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.ExceptionHandling;
using NLog;

namespace WebSite
{
    public class NLogExceptionLogger : ExceptionLogger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public override void Log(ExceptionLoggerContext context)
        {
            logger.Log(LogLevel.Error, RequestToString(context.Request) + "\n\n" + context.Exception);
        }

        private static string RequestToString(HttpRequestMessage request)
        {
            var message = new StringBuilder();
            if (request.Method != null)
                message.Append(request.Method);

            if (request.RequestUri != null)
                message.Append(" ").Append(request.RequestUri);

            return message.ToString();
        }
    }
}