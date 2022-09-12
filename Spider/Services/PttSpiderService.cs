using System.Text.Json;
using System.Text.Json.Serialization;
using AngleSharp;
using AngleSharp.Dom;
using Spider.Model;

namespace Spider.Services;

public class PttSpiderService
{
    public PttSpiderService()
    {
        
    }

    public async Task<object> Getpost()
    {
        var config = AngleSharp.Configuration.Default
            .WithDefaultLoader()
            .WithDefaultCookies();
		
        var browser = BrowsingContext.New(config);

        var baseUrl = "https://www.ptt.cc";
        var indexUrl = "/bbs/Beauty/index.html";

        var pages = 10;
        var posts = await GetPosts(browser, baseUrl, indexUrl, pages);
        var fileName = "pttBeautyPost.json";
        await using (FileStream createStream = File.Create(fileName))
        {
            await JsonSerializer.SerializeAsync(createStream, posts.Where(post => post.Push > 90).ToList()).ConfigureAwait(false);
            await createStream.DisposeAsync();   
        }
        
        return posts.Where(post => post.Push > 90);
    }
    
    private async Task<IEnumerable<Post>> GetPosts(
        IBrowsingContext browser, 
        string baseUrl, 
        string pageUrl,
        int remainingPages)
    {
        // 組裝 Url 並設定 Cookie
        var url = new Url(baseUrl + pageUrl);
        browser.SetCookie(url, "over18=1'");
        var document = await browser.OpenAsync(url);

        // 取出所有文章標題
        var postSource = document.QuerySelectorAll("div.r-ent");

        var posts = postSource.Select(post =>
            {
                var titleElement = post.QuerySelector("div.title > a");
                var title = titleElement?.InnerHtml;
                var link = titleElement?.GetAttribute("href");

                var pushString = post.QuerySelector("div.nrec > span")?.InnerHtml;
                var pushCount =
                    pushString == "爆" ? 100 :
                    Int16.TryParse(pushString, out var push) ? push : 0;

                return new Post
                {
                    Title = title,
                    Link = link,
                    Push = pushCount
                };
            })
            .Where(post => post.Title != null);

        // 取得下一頁的連結
        var nextPageUrl = document
            .QuerySelector("div.btn-group.btn-group-paging > a:nth-child(2)")
            .GetAttribute("href");
		
        document.Close();
	
        // 檢查剩餘頁數
        remainingPages--;
        if (remainingPages == 0)
        {
            return posts;
        }

        // 組裝遞迴取得的文章列表
        var nextPagePosts = await GetPosts(browser, baseUrl, nextPageUrl, remainingPages);
        return posts.Concat(nextPagePosts);
    }
}