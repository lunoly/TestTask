using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTask
{
    public static class Program
    {
        public static async Task Main()
        {
            IEnumerable<Uri> uri;
            IEnumerable<Uri> siteMapUri;
            IEnumerable<Uri> generalListUri = Enumerable.Empty<Uri>();
            IEnumerable<Uri> unitedListUri;
            IEnumerable<Uri> inSiteMapNotInGeneralListUri;
            IEnumerable<Uri> inGeneralListNotInSiteMap;
            IEnumerable<(Uri, long)> timingList = Enumerable.Empty<(Uri, long)>();

            string userUrl;
            int i;

            do
            {
                Console.WriteLine("Enter URL");
                userUrl = Console.ReadLine();
                uri = SiteScan.GetUrlsFromHtml(userUrl);
            } while (uri.Count() == 0);

            generalListUri = SiteScan.CheckAllPages(uri.ToList(), userUrl);
            siteMapUri = SiteMap.ReadSiteMap(userUrl);
            unitedListUri = generalListUri.Concat(siteMapUri).Distinct();
            inSiteMapNotInGeneralListUri = siteMapUri.Except(generalListUri);
            Console.WriteLine();
            Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site");

            if (inSiteMapNotInGeneralListUri.Any())
            {
                i = 0;
                foreach (var s in inSiteMapNotInGeneralListUri)
                {
                    i++;
                    Console.WriteLine(i + "\t" + s);
                }
            }
            else
            {
                Console.WriteLine("Sitemap.xml is not found.");
            }

            inGeneralListNotInSiteMap = generalListUri.Except(siteMapUri);
            Console.WriteLine();
            Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
            i = 0;
            foreach (var s in inGeneralListNotInSiteMap)
            {
                i++;
                Console.WriteLine(i + "\t" + s);
            }

            var getUrlClass = new GetAsyncUrl();
            Console.WriteLine();
            Console.WriteLine("Timing");
            i = 0;
            foreach (var s in unitedListUri)
            {
                i++;
                try
                {
                    var someString = await getUrlClass.showAsyncTime(s.ToString(), i.ToString());
                    timingList = timingList.Append((s, someString));
                }
                catch (Exception)
                {
                    // Not necessary do something
                }
            }

            if (timingList.Any())
            {
                timingList = timingList.OrderBy(p => p.Item2);
                i = 0;
                foreach (var t in timingList)
                {
                    i++;
                    Console.Write(i + "\t" + t.Item1.ToString() + "\t" + t.Item2 + "mc\n");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Urls(html documents) found after crawling a website: " + generalListUri.Count());
            Console.WriteLine("Urls found in sitemap: " + siteMapUri.Count());
        }
    }
}
