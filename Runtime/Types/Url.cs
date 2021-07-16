using ReactUnity.Converters;
using System;
using System.Text.RegularExpressions;

namespace ReactUnity.Types
{
    public class Url
    {
        private static Regex DataRegex = new Regex(@"^data:(?<mime>[\w/\-\.]+)?(;(?<encoding>\w+))?,?(?<data>.*)", RegexOptions.Compiled);

        public UrlProtocol Protocol { get; }
        public string FullUrl { get; }
        public string NormalizedUrl { get; }

        public bool HasKnownProtocol => Protocol != UrlProtocol.Contextual && Protocol != UrlProtocol.None;

        public Url(string fullUrl)
        {
            fullUrl = AllConverters.StringConverter.Convert(fullUrl) as string;

            FullUrl = fullUrl;

            if (string.IsNullOrWhiteSpace(fullUrl))
            {
                Protocol = UrlProtocol.None;
                NormalizedUrl = "";
                return;
            }

            var splits = fullUrl.Split(new char[] { ':' }, 2);
            if (splits.Length == 1)
            {
                Protocol = UrlProtocol.Contextual;
                NormalizedUrl = fullUrl;
                return;
            }

            var protocol = splits[0];
            NormalizedUrl = splits[1];

            if (protocol == "ctx" || protocol == "context")
            {
                Protocol = UrlProtocol.Contextual;
            }
            else if (protocol == "http" || protocol == "https")
            {
                Protocol = UrlProtocol.Web;
                NormalizedUrl = fullUrl;
            }
            else if (protocol == "res" || protocol == "resource" || protocol == "resources")
            {
                Protocol = UrlProtocol.Resource;
            }
            else if (protocol == "file")
            {
                Protocol = UrlProtocol.File;
            }
            else if (protocol == "data")
            {
                Protocol = UrlProtocol.Data;
            }
            else if (protocol == "global" || protocol == "globals")
            {
                Protocol = UrlProtocol.Global;
            }
            else
            {
                Protocol = UrlProtocol.Contextual;
                NormalizedUrl = fullUrl;
            }
        }

        public byte[] GetData()
        {
            var dataMatch = DataRegex.Match(FullUrl);
            if (dataMatch.Success)
            {
                var mime = dataMatch.Groups["mime"].Value;
                var encoding = dataMatch.Groups["encoding"].Value;
                var data = dataMatch.Groups["data"].Value;
                return Convert.FromBase64String(data);
            }
            return null;
        }
    }

    public enum UrlProtocol
    {
        None = 0,
        Contextual = 1,
        Web = 2,
        Resource = 3,
        File = 4,
        Data = 5,
        Global = 6,
    }
}
