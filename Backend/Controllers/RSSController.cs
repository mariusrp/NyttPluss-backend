using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RSSController : ControllerBase
    {
        private readonly HttpClient _httpClient = new HttpClient();

        [HttpGet("fetch-rss/{category}/{type}")]
        public async Task<IActionResult> FetchRss(string type, string category)
        {
            var feedUrl = category == ""
                ? $"https://www.nrk.no/{type}.rss"
                : $"https://www.nrk.no/{category}/{type}.rss";

            using var response = await _httpClient.GetAsync(feedUrl);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to retrieve RSS feed");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var xdoc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
            var rssItems = new List<Backend.Models.RssItem>();

            var media = XNamespace.Get("http://search.yahoo.com/mrss/");
            foreach (var item in xdoc.Descendants("item"))
            {
                var linkElement = item.Element("link");
                if (linkElement == null || string.IsNullOrEmpty(linkElement.Value))
                {
                    continue;  // Skip this item if there is no link
                }

                var mediaContent = item.Element(media + "content");
                string imageUrl = "https://gfx.nrk.no/r6wyGh9VdMMPBPAQs-0HcgvVU3NWt7QDYrWX-efFF2sQ";  // Set default image URL

                if (mediaContent != null && mediaContent.Attribute("medium")?.Value == "image")
                {
                    imageUrl = mediaContent.Attribute("url")?.Value;
                }

                var rssItem = new Backend.Models.RssItem
                {
                    Id = ObjectId.GenerateNewId().ToString(), // Generate a unique ID
                    Title = item.Element("title")?.Value,
                    Link = linkElement.Value,
                    Description = item.Element("description")?.Value,
                    PubDate = item.Element("pubDate")?.Value,
                    ImageUrl = imageUrl  // This will either be the image URL from the XML or the default image URL
                };

                rssItems.Add(rssItem);
            }

            return Ok(rssItems);
        }
    }
}
