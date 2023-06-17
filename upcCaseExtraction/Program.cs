using upcCaseExtraction;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using LiteDB;
using LiteDB.Engine;
using System.Text.Json;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

class Program
{

    private static int pageCount = 0;
    private static readonly HttpClient httpClient = new HttpClient();
    private static readonly AsyncRetryPolicy<HttpResponseMessage> httpRetryPolicy =
        Policy
            .HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(4),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(60)

            });


    static async Task Main(string[] args)
    {
        int start = System.Convert.ToInt32(args[0]);

        await GetCaseFromUPC(start);
        for (int i = start+1; i < pageCount+1; i++)
        {
            await GetCaseFromUPC(i);
        }

    }


    static async Task GetCaseFromUPC(int page)
    {
        
        HttpResponseMessage response = await GetHttpResponseAsync($"https://api-prod.unified-patent-court.org/upc/public/api/v4/cases?pageSize=100&pageNumber={page}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Read page: {DateTime.Now.ToShortTimeString()} {page}");
            string json = await response.Content.ReadAsStringAsync();
            await InsertJsonToLiteDBAsync(json);
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
        }
    }

    static async Task<HttpResponseMessage> GetHttpResponseAsync(string uri)
    {
        return await httpRetryPolicy.ExecuteAsync(async () =>
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            //response.EnsureSuccessStatusCode();
            return response;
        });
    }

    static async Task InsertJsonToLiteDBAsync(string json)
    {
        var root = JsonConvert.DeserializeObject<Root>(json);

        if (pageCount == 0)
        {
            pageCount = root.totalResults / 100;
        }

        using var db = new LiteDatabase("Filename=UpcCases.db;Mode=Shared");
        var col = db.GetCollection<BsonDocument>("Cases");

        foreach (var c in root.content)
        {

            var doc = BsonMapper.Global.ToDocument<Content>(c);
            await Task.Run(() => col.Insert(doc));
        }
    }
}
