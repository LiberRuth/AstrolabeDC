using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace AstrolabeDC.dcSharp
{
    internal class VideoHTML
    {
        private HtmlNodeCollection? htmlNodes;
        private HtmlNodeCollection? htmlNodes_WidthHeight;

        public async Task HTML(string URL)
        {

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://gall.dcinside.com/");
            try
            {
                var response = await httpClient.GetAsync(URL);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                htmlNodes = doc.DocumentNode.SelectNodes("//div[@class='video_wrap']/div[@class='video_inbox'] | //div[@class='video_inbox']");
                htmlNodes_WidthHeight = doc.DocumentNode.SelectNodes("//body[@class='tx-content-container']");

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}");
            }

        }

        public string VideoURL()
        {
            string reHTML = "";
            foreach (var Nodes in htmlNodes!)
            {
                foreach (HtmlNode videolink in Nodes.SelectNodes(".//source[@src]"))
                {
                    HtmlAttribute attvideo = videolink.Attributes["src"];
                    reHTML = attvideo.Value;
                }
            }
            return reHTML;
        }

        public int Width()
        {
            int width = 0;
            foreach (var WidthHTML in htmlNodes_WidthHeight!)
            {
                foreach (HtmlNode item in WidthHTML.SelectNodes(".//div[@class='v-container']"))
                {
                    HtmlAttribute attvideo = item.Attributes["style"];
                    Match match = Regex.Match(attvideo.Value, @"width:(\d+)px");
                    string widthString = match.Groups[1].Value;
                    width = int.Parse(widthString);
                }
            }
            return width;
        }

        public int Height()
        {
            int height = 0;
            foreach (var WidthHTML in htmlNodes_WidthHeight!)
            {
                foreach (HtmlNode item in WidthHTML.SelectNodes(".//div[@class='video_inbox']"))
                {
                    HtmlAttribute attvideo = item.Attributes["style"];
                    Match match = Regex.Match(attvideo.Value, @"height:(\d+)");
                    string heightString = match.Groups[1].Value;
                    height = int.Parse(heightString);
                }
            }
            return height;
        }
    }
}
