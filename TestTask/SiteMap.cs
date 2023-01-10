using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;


namespace TestTask
{
    /// <summary>
    /// Finds all href from url.
    /// </summary>
    public static class SiteMap
    {

        /// <summary>
        /// Gets the urls from sitemap.xml.
        /// </summary>
        /// <param name="url">The URL of site from user with console.</param>
        /// <returns>Retirn url list from sitemap.</returns>
        ///
        public static IEnumerable<Uri> ReadSiteMap(string url)
        {
            var returnListUri = Enumerable.Empty<Uri>();
            if (!url.Contains("sitemap.xml"))
            {
                if ((url.Last() != '\\') || (url.Last() != '/')) { url = string.Concat(url, '/'); }
                url = string.Concat(url, "sitemap.xml");
            }

            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = System.Text.Encoding.UTF8;
                string sitemapString = wc.DownloadString(url);
                wc.Dispose();
                XmlDocument urldoc = new XmlDocument();
                urldoc.LoadXml(sitemapString);
                XmlNodeList xmlSitemapList = urldoc.GetElementsByTagName("url");

                foreach (XmlNode node in xmlSitemapList)
                {
                    if (node["loc"] != null)
                    {
                        returnListUri = returnListUri.Append(new Uri(node["loc"].InnerText));
                    }
                }

                return returnListUri;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<Uri>();
            }
        }
    }
}
