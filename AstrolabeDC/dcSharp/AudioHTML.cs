using HtmlAgilityPack;


namespace AstrolabeDC.dcSharp
{
    internal class AudioHTML
    {
        private HtmlNodeCollection? htmlNodes;

        public async Task HTML(string URL)
        {
            if (URL.Contains("&vr_open=1&type=W") == false) { URL += "&vr_open=1&type=W"; }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:108.0) Gecko/20100101 Firefox/108.0");
            httpClient.DefaultRequestHeaders.Referrer = new Uri("https://gall.dcinside.com/");
            try
            {
                var response = await httpClient.GetAsync(URL);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                htmlNodes = doc.DocumentNode.SelectNodes("//body");

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}");
            }

        }

        public string AudioURL()
        {
            string reHTML = "";
            foreach (var Nodes in htmlNodes!)
            {
                foreach (HtmlNode videolink in Nodes.SelectNodes(".//input[@value]"))
                {
                    HtmlAttribute attvideo = videolink.Attributes["value"];
                    reHTML = attvideo.Value;
                }
            }
            return reHTML;
        }
    }
}
