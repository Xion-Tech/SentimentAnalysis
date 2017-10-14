using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SentimentAnalysis.Entities
{
    class TextualSentiments
    {
        public string Keyword { get; set;}
        public double PolarityConfidence { get; set; }
        public string Polarity { get; set; }
    }
}
