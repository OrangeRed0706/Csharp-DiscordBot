using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.Api.Client
{
    public sealed class ApiValidationException : Exception
    {
        public string HttpMethod { get; private set; }

        public string TargetUrl { get; private set; }

        public Dictionary<string, List<string>> ValidationErrors { get; set; }

        public ApiValidationException(string httpMethod, string targetUrl, string message, Dictionary<string, List<string>> validationErrors)
            : base(message)
        {
            HttpMethod = httpMethod;
            TargetUrl = targetUrl;
            ValidationErrors = validationErrors;
        }
    }
}
