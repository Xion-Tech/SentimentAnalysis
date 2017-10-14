using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net;
using SentimentAnalysis.Entities;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
namespace SentimentAnalysis.BusinessLayer
{
    class ImageOperations
    {
        public byte[] FetchImageFromUrl(string imageUrl)
        {
            byte[] imageData = null;
            using (WebClient webClient = new WebClient())
            {
                imageData = webClient.DownloadData(imageUrl);
            }
            return imageData;
        }

        public EmotionResult[] ImageEmotions(string imageUrl, Tweet tw)
        {
            byte[] imageData = FetchImageFromUrl(imageUrl);

            tw.ImageSentiment.ImageData = imageData;

            var _apiUrl = "https://api.projectoxford.ai/emotion/v1.0/recognize";
            var _apiKey = "fed01eba157842f98916d73761153dfc";
           
            EmotionResult[] imageEmotions = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                /*Setup data object*/
                HttpContent content = new StreamContent(new MemoryStream(imageData));
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
                /*Make Http request*/
                var response = httpClient.PostAsync(_apiUrl, content);
                /*Read response and write to view*/
                var responseContent = response.Result.Content.ReadAsStreamAsync();
                MemoryStream ms = (MemoryStream)responseContent.Result;
                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                imageEmotions = JsonDeserialize<EmotionResult[]>(jsonString);
            }

            return imageEmotions;
        }

        public static T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        public static Image Resize(byte[] imageData, int newWidth, int maxHeight, bool onlyResizeIfWider)
        {

            Image image = Image.FromStream(new MemoryStream(imageData));

            if (onlyResizeIfWider && image.Width <= newWidth) newWidth = image.Width;

            var newHeight = image.Height * newWidth / image.Width;
            if (newHeight > maxHeight)
            {
                // Resize with height instead  
                newWidth = image.Width * maxHeight / image.Height;
                newHeight = maxHeight;
            }

            var res = new Bitmap(newWidth, newHeight);

            using (var graphic = Graphics.FromImage(res))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;
                graphic.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return res;
        }  
    }


}
