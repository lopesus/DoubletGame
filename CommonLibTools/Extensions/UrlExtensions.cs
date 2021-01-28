using System;
using System.Text;

namespace CommonLibTools.Extensions
{
    //    var uri = UriExtensions.CreateUriWithQuery(new Uri("http://example.com"), 
    //    new UrlParams { { "key1", "value1" }, { "key2", "value2" }});

    //Results:

    //http://localhost/?key1=value1&key2=value2


    public static class UrlExtensions
    {
        public static Uri CreateUriWithQuery(Uri uri, UrlParams values)
        {
            var queryStr = new StringBuilder();
            // presumes that if there's a Query set, it starts with a ?
            var str = string.IsNullOrWhiteSpace(uri.Query) ?
                "" : uri.Query.Substring(1) + "&";

            foreach (var value in values)
            {
                queryStr.Append(str + value.Key + "=" + value.Value);
                str = "&";
            }
            // query string will be encoded by building a new Uri instance
            // clobbers the existing Query if it exists
            return new UriBuilder(uri)
            {
                Query = queryStr.ToString()
            }.Uri;
        }
    }
}