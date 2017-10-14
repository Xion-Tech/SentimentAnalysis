 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentimentAnalysis.Entities
{
    class ImageSentiments
    {
        public string ImageUrl { get; set; }
        public byte[] ImageData { get; set; }
        public EmotionResult[] ImageEmotions { get; set; }
        public int FaceCount { get; set; }
    }

    public class Scores
    {
        public double anger { get; set; }
        public double contempt { get; set; }
        public double disgust { get; set; }
        public double fear { get; set; }
        public double happiness { get; set; }
        public double neutral { get; set; }
        public double sadness { get; set; }
        public double surprise { get; set; }
    }

    public class EmotionResult
    {
        public Scores scores { get; set; }
        public string ExactEmotion { get; set; }
    }
}
