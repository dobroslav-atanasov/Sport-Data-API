﻿namespace SportHub.Services;

using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

using SportHub.Data.Models.Http;
using SportHub.Services.Interfaces;

public class HttpService : IHttpService
{
    public async Task<byte[]> DownloadBytesAsync(string url)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        var response = await client.GetAsync(url);
        var bytes = await response.Content.ReadAsByteArrayAsync();

        return bytes;
    }

    public async Task<HttpModel> GetAsync(string url, bool isOlympediaUrl = false)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        var response = await client.GetAsync(url);
        if (isOlympediaUrl)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();

            while (responseAsString == "Rate Limit Exceeded")
            {
                await Task.Delay(3000);
                response = await client.GetAsync(url);
                responseAsString = await response.Content.ReadAsStringAsync();
            }
        }

        if (response != null)
        {
            var httpModel = await this.CreateHttpModelAsync(response, url);
            return httpModel;
        }

        return null;
    }

    public async Task<HttpModel> PostAsync(string url, string json)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);

        if (response != null)
        {
            var httpModel = await this.CreateHttpModelAsync(response, url);
            return httpModel;
        }

        return null;
    }

    private async Task<HttpModel> CreateHttpModelAsync(HttpResponseMessage response, string url, string bannedUrl = null)
    {
        var httpModel = new HttpModel
        {
            Url = url,
            StatusCode = response.StatusCode,
            MimeType = response.Content.Headers.ContentType?.MediaType,
            Content = await response.Content.ReadAsStringAsync(),
        };

        if (httpModel.MimeType is not null and "text/html")
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(httpModel.Content);
            httpModel.HtmlDocument = htmlDocument;
        }

        if (!string.IsNullOrEmpty(bannedUrl) && url.StartsWith(bannedUrl))
        {
            httpModel.Content = httpModel
                .HtmlDocument
                .DocumentNode
                .SelectSingleNode("//div[@class='container']")?
                .OuterHtml;

            httpModel.HtmlDocument.LoadHtml(httpModel.Content);
        }

        httpModel.Encoding = Encoding.UTF8;
        var charSet = response.Content.Headers.ContentType?.CharSet;
        if (charSet != null && charSet.ToLower() != "utf-8")
        {
            httpModel.Encoding = Encoding.GetEncoding(charSet);
        }

        return httpModel;
    }
}