using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsQuery;

namespace StackoverflowAnalizer
{
    public class StackoverflowWalker
    {
        private readonly IPageProcessor _pageProcessor;
        private const string UrlForTag = "http://stackoverflow.com/questions/tagged/c%23?page={0}&sort=newest&pagesize=50";
        readonly Downloader _downloader = new Downloader(10);
        private readonly ConcurrentQueue<string> _urls = new ConcurrentQueue<string>();
        private int _currentPage = 1;
        private const int PageSize = 5;


        public StackoverflowWalker(IPageProcessor pageProcessor)
        {
            _pageProcessor = pageProcessor;
        }

        public async Task Walk()
        {
            await DownloadLinks();
            while (!_urls.IsEmpty)
            {
                string url;
                while (_urls.TryDequeue(out url))
                {
                    var url1 = url;
                    await _downloader.Enqueue(url).ContinueWith(n => _pageProcessor.ProcessPage(url1, _downloader.DequeueResult(url1)));
                }
                await DownloadLinks();
            }
        }

        private async Task DownloadLinks()
        {
            var walkByLinks = WalkByLinks();
            await walkByLinks;
            walkByLinks.Wait();
        }

        private async Task WalkByLinks()
        {
            if (_urls.IsEmpty)
            {
                var tasks = new List<Task>();
                for (var i = _currentPage; i <= _currentPage + PageSize; i++)
                {
                    var page = string.Format(UrlForTag, i);
                    var linkDownloading =
                        _downloader.Enqueue(page).ContinueWith(n => LinkProcess(_downloader.DequeueResult(page)));
                    tasks.Add(linkDownloading);
                    await linkDownloading;
                }
                _currentPage += PageSize;
                Task.WaitAny(tasks.ToArray());
            }
        }


        private void LinkProcess(string linkPage)
        {
            var dom = CQ.Create(linkPage);
            foreach (var links in dom.Select(".question-hyperlink"))
            {
                _urls.Enqueue("http://stackoverflow.com" + links.GetAttribute("href"));
            }
        }
    }

    public interface IPageProcessor
    {
        void ProcessPage(string url, string html);
    }
}
