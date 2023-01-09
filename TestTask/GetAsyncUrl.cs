using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestTask
{
    public class GetAsyncUrl
    {
        private readonly HttpClient _client;

        public GetAsyncUrl()
        {
            _client = new HttpClient();
        }

        /// <summary>
        /// Show response time.
        /// </summary>
        /// <param name="url">The URL for check responce.</param>
        /// <param name="indexNumber">Index for table.</param>
        /// <returns></returns>
        /// 
        public async Task<long> showAsyncTime(string url, string indexNumber)
        {
            var sw = new Stopwatch();
            sw.Restart();
            await _client.GetStringAsync(url);
            sw.Stop();

            //Console.Write(indexNumber + "\t" + url.ToString() + "\t"+ sw.ElapsedMilliseconds + "mc\n");
            return sw.ElapsedMilliseconds;
        }
    }
}