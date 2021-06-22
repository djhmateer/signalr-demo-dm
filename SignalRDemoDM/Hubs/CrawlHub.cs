using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace SignalRDemoDM.Hubs
{
    public class CrawlHub : Hub
    {
        // A wrapper iterator so can catch exceptions here which can't be done in 
        // a block which does yield return
        public async IAsyncEnumerable<string> Crawl(string url, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await using var messageEnumerator = CrawlAndGetMessages(url, cancellationToken).GetAsyncEnumerator(cancellationToken);
            var more = true;
            while (more)
            {
                // Catch exceptions only on executing/resuming the iterator function
                try
                {
                    more = await messageEnumerator.MoveNextAsync();
                }
                catch (TaskCanceledException)
                {
                    // This is normal behaviour for if we cancel the crawl
                    // which essentially calls the cancellation token
                    // Caught in the javascript to update the UI with a message
                    Log.Information("TaskCancelledException - caught in top level exception handler in CrawlHub");
                }
                catch (Exception ex)
                {
                    Log.Fatal("Top level Crawl caught an unhandled exception: " + ex);
                    throw; // The exception will bubble up to the UI through normal channels
                }

                // If all done don't yield the last message
                if (more)
                    yield return messageEnumerator.Current;
            }
        }

        public async IAsyncEnumerable<string> CrawlAndGetMessages(string url, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // handing off to Crawler which returns back messages (UIMessage objects) every now and again on progress
            await foreach (var message in Crawler.Crawl(url, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                Log.Information(message);

                // if message (which was a complex type) was something
                // then send message to the client directly - not on the stream of messages

                yield return message;
            }
        }
    }

    public static class Crawler
    {
        public static async IAsyncEnumerable<string> Crawl(string url, [EnumeratorCancellation] CancellationToken cancellationToken)
        {


            // BaseUrl bad don't crawl
            if (string.IsNullOrWhiteSpace(url))
            {
                var messageb = "Bad baseUrl, finished crawler (cs)";
                Log.Warning(messageb);
                yield return messageb;
                yield break;
            }

            yield return $"base url looks good {url}";

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

            var i = 0;
            while (i < 5)
            {
                yield return DateTime.Now.ToLongTimeString();
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                i++;
            }
            throw new ApplicationException("something bad happened - all stop! This will get sent to the client if detailed errors are on");

            // Start the Crawl and pass messages back to the UI (and log)
            //await foreach (var uiMessageFromCrawl in CrawlWithGoodBaseUrl(baseUrl, startPage!, cancellationToken,
            //    webHostEnvironment))
            //{
            //    yield return uiMessageFromCrawl;
            //}

            yield return "done";
        }
    }
}
