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

        System.IO.Directory.CreateDirectory("files/images");
        System.IO.Directory.CreateDirectory("files/videos");
        System.IO.Directory.CreateDirectory("files/documents");
        System.IO.Directory.CreateDirectory("files/audios");
    }

    public void HandleMessage(MessageObject message)
    {
        var messageType = message.entry[0].changes[0].value.messages[0].type;
        _logger.LogInformation(messageType);
        string mimeType;
        switch (messageType)
        {
            case "image":
                var imageId = message.entry[0].changes[0].value.messages[0].image.id;
                mimeType = message.entry[0].changes[0].value.messages[0].image.mime_type;
                downloadImage(imageId, mimeType);
                break;
            case "video":
                var videoId = message.entry[0].changes[0].value.messages[0].video.id;
                mimeType = message.entry[0].changes[0].value.messages[0].video.mime_type;
                downloadVideo(videoId, mimeType);
                break;
            case "document":
                var documentId = message.entry[0].changes[0].value.messages[0].document.id;
                mimeType = message.entry[0].changes[0].value.messages[0].document.filename;
                downloadDocument(documentId, mimeType);
                break;
            case "audio":
                var audioId = message.entry[0].changes[0].value.messages[0].audio.id;
                mimeType = message.entry[0].changes[0].value.messages[0].audio.mime_type;
                downloadAudio(audioId, mimeType);
                break;


        };
    }

    private async Task<bool> downloadVideo(string videoId, string mimeType)
    {
        var extension = mimeType.Split('/')[1];
        _logger.LogInformation(extension);
        var mediaUrl = await GetMediaUrl(videoId);
        if (mediaUrl != null)
        {
            byte[] fileBytes = await _httpClient.GetByteArrayAsync(mediaUrl);
            await File.WriteAllBytesAsync($"files/videos/{videoId}.{extension}", fileBytes);
            return true;
        }
        _logger.LogError("cannot video  locally");
        return false;
    }

    private async Task<bool> downloadImage(string imageId, string mimeType)
    {
        var extension = mimeType.Split('/')[1];
        _logger.LogInformation(extension);
        var mediaUrl = await GetMediaUrl(imageId);
        if (mediaUrl != null)
        {
            byte[] fileBytes = await _httpClient.GetByteArrayAsync(mediaUrl);
            await File.WriteAllBytesAsync($"files/images/{imageId}.{extension}", fileBytes);
            return true;
        }
        _logger.LogError("cannot save image locally");
        return false;
    }

    private async Task<bool> downloadAudio(string audioId, string mimeType)
    {
        var extension = mimeType.Split('/')[1].Split(';')[0];
        _logger.LogInformation(extension);
        var mediaUrl = await GetMediaUrl(audioId);
        if (mediaUrl != null)
        {
            byte[] fileBytes = await _httpClient.GetByteArrayAsync(mediaUrl);
            await File.WriteAllBytesAsync($"files/audios/{audioId}.{extension}", fileBytes);
            return true;
        }
        _logger.LogError("cannot save audio locally");
        return false;
    }

    private async Task<bool> downloadDocument(string documentId, string mimeType)
    {
      
        _logger.LogInformation(mimeType);
        var mediaUrl = await GetMediaUrl(documentId);
        if (mediaUrl != null)
        {
            byte[] fileBytes = await _httpClient.GetByteArrayAsync(mediaUrl);
            await File.WriteAllBytesAsync($"files/documents/{documentId}.{mimeType}", fileBytes);
            return true;
        }
        _logger.LogError("cannot save document locally");
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
