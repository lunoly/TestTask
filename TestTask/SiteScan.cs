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
        /// <returns>List </returns>
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
        /// <returns>Urls list.</returns>
        public static IEnumerable<Uri> GetUrlsFromHtml(Uri url)
        {
            var client = new HttpClient();
            var tresponse = client.GetAsync(url);
            tresponse.Wait();
            var tcontent = tresponse.Result.Content.ReadAsStringAsync();
            tcontent.Wait();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(tcontent.Result);

            var xpath = new StringBuilder("//a[@href")
                .Append(" and not(@href='')")
                .Append(" and not(contains(@href,'#'))")
                .Append(" and not(contains(@href,'}'))")
                .Append(" and not(contains(@href,'java'))")
                .Append("]").ToString();

            try
            {
                var urls = doc.DocumentNode.SelectNodes(xpath)
                          .Select(p => p.Attributes["href"].Value)
                          .Distinct();

                IEnumerable<Uri> newUrls = Enumerable.Empty<Uri>();
                foreach (var u in urls)
                {
                    if (u[0] == '/' || u[0] == '?' || u[0] == '\\')
                    {
                        newUrls = newUrls.Append(new Uri(url, u));
                    }
                    else if (u[0] == 'h')
                    {
                        newUrls = newUrls.Append(new Uri(u));
                    }
                    else
                    {
                        try
                        {
                            newUrls = newUrls.Append(new Uri(url, u));
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(u);
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                return newUrls;
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
                    try
                    {
                        queueUri.AddRange(GetUrlsFromHtml(queueUri.FirstOrDefault().ToString()).ToList());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(queueUri.FirstOrDefault().ToString());
                        Console.WriteLine(ex.Message);
                    }
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