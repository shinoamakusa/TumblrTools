﻿/* 01010011 01101000 01101001 01101110 01101111  01000001 01101101 01100001 01101011 01110101 01110011 01100001
 *
 *  Project: Tumblr Tools - Image parser and downloader from Tumblr blog system
 *
 *  Author: Shino Amakusa
 *
 *  Created: 2013
 *
 *  Last Updated: August, 2015
 *
 * 01010011 01101000 01101001 01101110 01101111  01000001 01101101 01100001 01101011 01110101 01110011 01100001 */

using RestSharp;
using System;
using System.Net;
using System.Net.NetworkInformation;
using Tumblr_Tool.Enums;

namespace Tumblr_Tool.Helpers
{
    public static class WebHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static bool CheckForInternetConnection()
        {
            return new Ping().Send("www.google.com").Status == IPStatus.Success;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDomainName(string url)
        {
            return new Uri(url) != null ? new Uri(url).Host : null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetRemoteDocumentAsString(string url)
        {
            try
            {
                string docStr;
                Uri uri = new Uri(url);
                string domain = uri.Host;
                string protocol = uri.Scheme + Uri.SchemeDelimiter;
                string path = uri.PathAndQuery;

                var client = new RestClient(string.Concat(protocol, domain));
                var request = new RestRequest(path, Method.GET);
                IRestResponse response = client.Execute(request);
                docStr = response.Content; // raw content as string

                //using (var wc = new RestClient(url))
                //{
                //    wc.Encoding = Encoding.UTF8;
                //    docStr = wc.DownloadString(url);
                //}

                return !string.IsNullOrEmpty(docStr) ? docStr : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsValidUrl(this string source)
        {
            Uri uriResult;
            return Uri.TryCreate(source, UriKind.Absolute, out uriResult) && Uri.CheckSchemeName(uriResult.Scheme) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string RemoveTrailingBackslash(string url)
        {
            try
            {
                if (url.EndsWith("/"))
                {
                    url = url.Remove(url.Length - 1);
                }

                return url;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string StripHTMLTags(string HTML)
        {
            // Removes tags from passed HTML
            System.Text.RegularExpressions.Regex objRegEx = new System.Text.RegularExpressions.Regex("<[^>]*>");

            return objRegEx.Replace(HTML, "");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool TumblrExists(this string url, string mode)
        {
            try
            {
                if (mode == ApiModeEnum.v2JSON.ToString())
                {
                    dynamic jsonObject = JsonHelper.GetObject(url);
                    return (jsonObject != null && jsonObject.meta != null && jsonObject.meta.status == ((int)TumblrAPIResponseEnum.OK).ToString());
                }
                else
                    return XmlHelper.GetDocument(url) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool UrlExists(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string domain = uri.Host;
                string protocol = uri.Scheme + Uri.SchemeDelimiter;
                string path = uri.PathAndQuery;

                var client = new RestClient(string.Concat(protocol, domain));
                var request = new RestRequest(path, Method.HEAD);
                IRestResponse response = client.Execute(request);
                HttpStatusCode statusCode = response.StatusCode;

                if (statusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }

                //using (var client = new WebClient())
                //using (var stream = client.OpenRead(url))
                //{
                //    return true;
                //}
            }
            catch
            {
                return false;
            }
        }
    }
}