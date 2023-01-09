using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using HtmlAgilityPack;

namespace TestTask
{
    /// <summary>
    /// Finds all href from url.
    /// </summary>
    public static class SiteScan
    {

        /// <summary>
        /// Gets the urls from website.
        /// </summary>
        /// <param name="url">The URL of site from user with console.</param>
        /// <returns></returns>
        /// 
        public static IEnumerable<Uri> GetUrlsFromHtml(string url)
        {
            try
            {
                return GetUrlsFromHtml(new Uri(url));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<Uri>();
            }
        }

        /// <summary>
        /// Gets the urls from HTML.
        /// </summary>
        /// <param name="url">The URL of site from user with console.</param>
        /// <returns></returns>
        public static IEnumerable<Uri> GetUrlsFromHtml(Uri url)
        {
            var client = new HttpClient();
            var tresponse = client.GetAsync(url);      //request to url
            tresponse.Wait();
            var tcontent = tresponse.Result.Content.ReadAsStringAsync();
            tcontent.Wait();

            HtmlDocument doc = new HtmlDocument();      //create html doc by url
            doc.LoadHtml(tcontent.Result);

            var xpath = new StringBuilder("//a[@href")
                .Append(" and not(@href='')")
                .Append(" and not(contains(@href,'#'))")
                .Append("]").ToString();

            try
            {
                var urls = doc.DocumentNode.SelectNodes(xpath)      //get all urls
                          .Select(p => p.Attributes["href"].Value)
                          .Distinct()
                          .Select(p =>
                          {
                              return (p[0] == '/' || p[0] == '?' || p[0] == '\\') ?
                                         new Uri(url, p) :   //if relative url
                                         new Uri(p);     //if absolute url
                          });

                return urls;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<Uri>();
            }
        }

        /// <summary>
        /// Finding sublinks in urls.
        /// </summary>
        /// <param name="url">The URL of site from user with console.</param>
        /// <param name="queueUri">Queue with all urls on website</param>
        /// <returns>All urls from website</returns>
        public static IEnumerable<Uri> CheckAllPages(List<Uri> queueUri, string url)
        {
            var returnListUri = Enumerable.Empty<Uri>();

            while (queueUri.Count != 0)
            {
                if ((queueUri.FirstOrDefault().ToString() == url) || (!queueUri.FirstOrDefault().ToString().Contains(url)))
                {
                    if (!returnListUri.Contains(queueUri.FirstOrDefault()))
                    {
                        returnListUri = returnListUri.Append(queueUri.FirstOrDefault());
                    }
                    queueUri.Remove(queueUri.FirstOrDefault());
                }
                else if (!returnListUri.Contains(queueUri.FirstOrDefault()))
                {
                    queueUri.AddRange(GetUrlsFromHtml(queueUri.FirstOrDefault().ToString()).ToList());
                    queueUri = queueUri.Distinct().ToList();
                    returnListUri = returnListUri.Append(queueUri.FirstOrDefault());
                    queueUri.Remove(queueUri.FirstOrDefault());
                }
                else
                {
                    queueUri.Remove(queueUri.FirstOrDefault());
                }
            }

            return returnListUri.Distinct().ToList();
        }
    }

}