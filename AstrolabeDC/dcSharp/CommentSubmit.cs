using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace AstrolabeDC.dcSharp
{
    internal class CommentSubmit
    {
        private string? CSRF_TOKEN;

        public string? name { get; set; }
        public string? password { get; set; }
        public string? memo { get; set; }

        private async Task<string> ConKey()
        {
            string blockKey = "";
            HttpClient HttpClient = new HttpClient();

            string url = "https://m.dcinside.com/ajax/access";

            var postData = new Dictionary<string, string>
            {
                { "token_verify", "com_submit" }
            };
            HttpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            HttpClient.DefaultRequestHeaders.Add("User-Accept", "Mozilla/5.0 (Linux; Android 7.0; SM-G892A Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/67.0.3396.87 Mobile Safari/537.36");
            HttpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            HttpClient.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.9");
            HttpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            HttpClient.DefaultRequestHeaders.Referrer = new Uri("https://m.dcinside.com/");
            var content = new FormUrlEncodedContent(postData);
            var response = await HttpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject(responseContent)!;
                blockKey = jsonObject!.Block_key;
            }
            else
            {
                Console.WriteLine("요청이 실패하였습니다. 상태 코드: " + response.StatusCode);
            }

            return blockKey;
        }


        public async Task<Dictionary<string, string>> CommentSubmitHTMLAsync(string target_url)
        {
            HttpClient httpClient = new HttpClient();
            Dictionary<string, string> Dictionary_Comment = new Dictionary<string, string>();

            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 7.0; SM-G892A Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/67.0.3396.87 Mobile Safari/537.36");
            httpClient.DefaultRequestHeaders.Referrer = new Uri(target_url);

            var response = await httpClient.GetAsync(target_url);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            //HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//div[@class='brick-wid']");
            var title = doc.DocumentNode.SelectSingleNode("//div/span[@class='tit']").InnerText;
            var hide_robot = doc.DocumentNode.SelectSingleNode("//input[@class='hide-robot']").Attributes["name"].Value;
            var board_id_hidden = doc.DocumentNode.SelectSingleNode("//ul/input[@name='cpage']");
            CSRF_TOKEN = doc.DocumentNode.SelectSingleNode("//meta[@name='csrf-token']").Attributes["content"].Value;

            string? board_idStr;
            if (board_id_hidden == null)
            {
                board_idStr = "";
            }
            else
            {
                board_idStr = board_id_hidden.Attributes["value"].Value;
            }

            string pattern = @"\/board\/(?<boardName>[^\/]+)\/(?<postId>[^\/]+)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(target_url);
            string boardName = match.Groups["boardName"].Value;
            string postId = match.Groups["postId"].Value;

            Dictionary_Comment.Add("comment_memo", memo!); //디코딩
            Dictionary_Comment.Add("comment_nick", name!); //디코딩
            Dictionary_Comment.Add("comment_pw", password!);
            Dictionary_Comment.Add("mode", "com_write");
            Dictionary_Comment.Add("comment_no", "");
            Dictionary_Comment.Add("id", boardName);
            Dictionary_Comment.Add("no", postId);
            Dictionary_Comment.Add("best_chk", "");
            Dictionary_Comment.Add("subject", title); //디코딩
            Dictionary_Comment.Add("board_id", board_idStr);
            Dictionary_Comment.Add("reple_id", "");
            Dictionary_Comment.Add("cpage", "1");
            Dictionary_Comment.Add("con_key", await ConKey());
            Dictionary_Comment.Add(hide_robot, "1");

            return Dictionary_Comment;
        }

        private static string Quote(string decoded)
        {
            List<string> arr = new List<string>();
            foreach (char c in decoded)
            {
                string t = ((int)c).ToString("X");
                if (t.Length >= 4)
                {
                    arr.Add("%u" + t);
                }
                else
                {
                    arr.Add("%" + t);
                }
            }
            return string.Join("", arr);
        }

        public string CookiesStr(string URL)
        {

            string pattern = @"\/board\/(?<boardName>[^\/]+)\/(?<postId>[^\/]+)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(URL);
            string board_id = match.Groups["boardName"].Value;
            string board_name = match.Groups["postId"].Value;

            Dictionary<string, string> cookies = new Dictionary<string, string>
            {
                { "m_dcinside_" + board_id, board_id },
                { "m_dcinside_lately", Quote(board_id + "|" + board_name + ",") },
                { "_ga", "GA1.2.693521455.1588839880" }
            };

            string Cookies = "";
            foreach (var cookie in cookies)
            {

                Cookies += $"{cookie.Key}: {cookie.Value}, ";
            }

            return Cookies;

        }

        public async Task<string> SubmitAsync(string URL)
        {

            HttpClient client = new HttpClient();

            string submit_url = "https://m.dcinside.com/ajax/comment-write";
            var postDataTxt = await CommentSubmitHTMLAsync(URL);
            //Mozilla/5.0 (Linux; Android 11; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.91 Mobile Safari/537.36
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("User-Accept", "Mozilla/5.0 (Linux; Android 7.0; SM-G892A Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/67.0.3396.87 Mobile Safari/537.36");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            client.DefaultRequestHeaders.Add("Host", "m.dcinside.com");
            client.DefaultRequestHeaders.Add("Origin", "https://m.dcinside.com");
            client.DefaultRequestHeaders.Add("Cookie", CookiesStr(URL));
            client.DefaultRequestHeaders.Add("X-Csrf-Token", CSRF_TOKEN);
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Referrer = new Uri(URL);
            var content = new FormUrlEncodedContent(postDataTxt);
            var response = await client.PostAsync(submit_url, content);
            //string requestData = "comment_memo=%E3%85%8B%E3%85%8B%E3%85%8B&comment_nick=%E3%85%87%E3%85%87&comment_pw=1234Q&mode=com_write&comment_no=&id=dcbest&no=153593&best_chk=&subject=%EA%B3%A0%EB%9D%BC%EB%8B%88+%EB%8B%A8+%ED%95%9C%EB%A7%88%EB%A6%AC%EB%9E%91+%EC%8B%B8%EC%9A%B0%EB%8B%A4&board_id=&reple_id=&cpage=1&con_key=2eb2dd19f6c73aab67abcaa947877c64772408fb99c6&7e8771=1";
            //var response = await client.PostAsync(submit_url, new StringContent(requestData));


            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("응답 내용:");
                Console.WriteLine(responseContent);
            }
            else
            {
                Console.WriteLine("HTTP Error: " + response.StatusCode);
            }

            return "";
        }
    }
}
