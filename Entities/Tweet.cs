using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentimentAnalysis.Entities
{
    class Tweet
    {
        public string Text{get;set;}
        public ImageSentiments ImageSentiment { get; set; }
        public TextualSentiments TextualSentiment { get; set; }
        public string ImgTextual { get; set; }
    }
}
