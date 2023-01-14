using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using API.Controllers;
using API.model;

namespace API.Helpers;

public class MessageHandler
{
    private readonly ILogger<MessageHandler> _logger;
    private readonly HttpClient _httpClient;

    private readonly IConfiguration _config;
    public MessageHandler(ILogger<MessageHandler> logger, IConfiguration config, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _config = config;

        _logger = logger;

    }

    public void HandleMessage(MessageObject message)
    {
        var messageType = message.entry[0].changes[0].value.messages[0].type;
        _logger.LogInformation(messageType);
        switch (messageType)
        {
            case "image":
                var imageId = message.entry[0].changes[0].value.messages[0].image.id;
                downloadImage(imageId);
                break;
            case "video":
                var videoId = message.entry[0].changes[0].value.messages[0].video.id;
                downloadVideo(videoId);
                break;

            
        };
    }

    private async Task<bool> downloadVideo(string videoId)
    {
        _logger.LogInformation("downloading video ...");
        var mediaUrl = await GetMediaUrl(videoId);
        if (mediaUrl != null)
        {
            byte[] fileBytes = await _httpClient.GetByteArrayAsync(mediaUrl);
            await File.WriteAllBytesAsync("files/test.mp4", fileBytes);
            _logger.LogInformation("saved video successfully");
            return true;
        }
        _logger.LogError("cannot video  locally");
        return false;
    }

    private async Task<bool> downloadImage(string imageId)
    {


        _logger.LogInformation("downloading image ...");
        var mediaUrl = await GetMediaUrl(imageId);
        if (mediaUrl != null)
        {
            byte[] fileBytes = await _httpClient.GetByteArrayAsync(mediaUrl);
            await File.WriteAllBytesAsync("files/test.jpg", fileBytes);
            _logger.LogInformation("saved image successfully");
            return true;
        }
        _logger.LogError("cannot save image locally");
        return false;
    }

    private async Task<string> GetMediaUrl(string mediaId)
    {
        var res = await _httpClient.GetFromJsonAsync<MediaUrlResponse>(mediaId);
        if (res != null)
        {
            var resString = JsonSerializer.Serialize<MediaUrlResponse>(res);
            _logger.LogInformation(res.url);
            return res.url;
        }
        else
        {
            _logger.LogError("Cannot get Media Url");
        }
        return null;
    }
}
