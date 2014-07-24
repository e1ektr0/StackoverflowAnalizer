using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StackoverflowAnalizer
{
    public class Downloader
    {
        private readonly ConcurrentQueue<string> _urls = new ConcurrentQueue<string>();
        private readonly Dictionary<string, string> _results = new Dictionary<string, string>();
        private readonly Dictionary<string, Exception> _errors = new Dictionary<string, Exception>();
        private int _treadCount = 2;

        public async Task AddToQueue(string url)
        {
            var startingThread = 0;
            _urls.Enqueue(url);
            var neededThread = _urls.Count;
            while (_treadCount > 0)
            {
                _treadCount--;
                await StartDownloadTread().ContinueWith(n => _treadCount++);
                startingThread++;
                if (startingThread == neededThread)
                    break;
            }
        }

        private async Task StartDownloadTread()
        {
            string url;
            if (_urls.TryDequeue(out url))
            {
                await await Download(url).ContinueWith(task => StartDownloadTread());
            }
        }

        private async Task Download(string url)
        {
            try
            {
                var result = await new WebClient().DownloadStringTaskAsync(url);
                _results.Add(url, result);
            }
            catch (Exception ex)
            {
                _errors.Add(url, ex);
            }
        }

        public Dictionary<string, string> GetResult()
        {
            return _results.ToDictionary(n => n.Key, n => n.Value);
        }
    }
}
