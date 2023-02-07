using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace VideoSearch
{

    class Api
    {
        public static String url = "https://code-in-life.netlify.app";
    }

    class Seach
    {
        public static HttpClient httpClient = new();

        public static String GetUserInput(String message) {

            Console.WriteLine(message);
            String? wd = Console.ReadLine();

            if (wd != null && wd != "") {
                return wd;
            }
            else
            {
                Console.WriteLine("输入不合法");
                return GetUserInput(message);
            }
        }

        public static async Task StartSearch(String wd) {
            Console.WriteLine("正在搜索...");
            SearchResult[]? searchResult = await GetSearchSearch(wd);
            if (searchResult != null)
            {
                Console.WriteLine("请求成功, 获取到" + searchResult.Length.ToString() + "条数据");
                DisplaySearchResult(searchResult);
            }
            else
            {
                Console.WriteLine("请求数据失败, 请检查网络设置");
            }
        }

        public static async Task<SearchResult[]?> GetSearchSearch(String wd) {
            String url = Api.url + "/video/search/api?s=" + System.Web.HttpUtility.UrlEncode(wd);
            SearchResult[]? searchResult = null;
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                String textContent = await response.Content.ReadAsStringAsync();
                string pattern = @"[a-zA-Z\d\/\+\=]{100,}";
                Match match = Regex.Match(textContent, pattern);
                String json = DecodeBase64("utf-8", match.Value);
                searchResult = JsonSerializer.Deserialize<SearchResult[]>(json);
            }
            return searchResult;
        }

        public static string DecodeBase64(string input, string code)
        {
            byte[] bytes = Convert.FromBase64String(code);
            string decode;
            try
            {
                decode = Encoding.GetEncoding(input).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }

        public static void DisplaySearchResult(SearchResult[] result) {
            
            foreach (SearchResult resultItem in result.Reverse())
            {
                Console.WriteLine($"\n [ {resultItem.name}  Star {resultItem.rating} ]");

                foreach (SearchResultData dataItem in resultItem.data!)
                {
                    Console.WriteLine("\n-------------------------------------");
                    Console.WriteLine($"[编号] {resultItem.key}_{dataItem.id}");
                    Console.WriteLine($"[名称] {dataItem.name} {dataItem.note}");
                    Console.WriteLine($"[分类] {dataItem.type}");
                    Console.WriteLine($"[更新] {dataItem.last}");
                    Console.WriteLine("-------------------------------------\n");
                }
                Console.WriteLine("\n ********************************* \n");
            }
        }

        public static void PickVideo() {
            String id = GetUserInput("请输入要查看的编号");
            List<String> args = id.Split('_').ToList();
            if (args.Count == 2) {
                Process.Start(new ProcessStartInfo($"https://cif.gatsbyjs.io/video/?api={args[0]}&id={args[1]}") { UseShellExecute = true });
            }
            else
            {
                Console.WriteLine("非法的编号, 请重新输入");
            }
            PickVideo();
        }

        public static void Main()
        {
            String keyword = GetUserInput("请输入要搜索的关键词");
            StartSearch(keyword).GetAwaiter().GetResult();
            PickVideo();
        }
    }

    class SearchResult
    {
        public String? key { get; set; }
        public String? name { get; set; }
        public double? rating { get; set; }
        public SearchResultPage? page { get; set; }
        public SearchResultData[]? data { get; set; }
    }

    class SearchResultPage
    {
        public int? page { get; set; }
        public int? pagecount { get; set; }
        public int? pagesize { get; set; }
        public int? recordcount { get; set; }
    }

    class SearchResultData
    {
        public int? id { get; set; }
        public String? name { get; set; }
        public int? tid { get; set; }
        public String? type { get; set; }
        public String? note { get; set; }
        public String? dt { get; set; }
        public String? last { get; set; }
    }
}
