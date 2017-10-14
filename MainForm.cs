using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

using System.Threading;
using Aylien.TextApi;

using SentimentAnalysis.Entities;
using SentimentAnalysis.BusinessLayer;

namespace SentimentAnalysis
{
    public partial class MainForm : Form
    {
        #region Variable Initilisation
        public static Aylien.TextApi.Client client;
        public bool posLock = false;
        public bool negLock = false;
        public bool neutralLock = false;
        public int totalThreadsInExecution = 0;
        List<Tweet> LstTwitts;
        int posCnt = 0;
        int negCnt = 0;
        int neutCnt = 0;
        int failCnt = 0;
        List<System.Threading.Thread> ThreadList = new List<Thread>();
        List<string> checkForDuplicate = new List<string>();

        private System.Timers.Timer _timer = new System.Timers.Timer();
        System.Diagnostics.Process Pr;

        int ExecThreadCount = 0;
        int tweetCount = 0;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
        }

      

        private void initialiseThreadGroup(List<Tweet> LstTwitts)
        {
            client = new Client("5e942b05", "3935d61712db1b91d90f2bdee7f1c90b");
            int count = Int32.Parse(txtTweetCount.Text.ToString());
            tweetCount = count;
            if (count > LstTwitts.Count) { count = LstTwitts.Count; }
            ExecThreadCount = 0;
            for (int i = 0; i < count; i++)
            {
                Thread th = new Thread(new ParameterizedThreadStart(CallThreads));
                th.Start(LstTwitts[i]);
            }

            Thread thFinalise = new Thread(new ThreadStart(CheckExitThreads));
            thFinalise.Start();
        }

        private void CheckExitThreads()
        {
            while (ExecThreadCount < LstTwitts.Count)
            {

            }

            for (int i = 0; i < LstTwitts.Count; i++)
            {
                computeFinalSentiment(LstTwitts[i]);
            }

            checkIfAllthreadsAreFinished();
        }

        private void CallThreads(object tweet)
        {
            Tweet tw = (Tweet)tweet;
            initialiseTextThread(tw);
            initialiseImageThread(tw);
            ExecThreadCount++;
        }

        private void InvokeTextAndImageSentiment(object tweet)
        {

            try
            {
                Tweet tw = (Tweet)tweet;

                var keyword = tw.TextualSentiment.Keyword;

                Sentiment sentiment = client.Sentiment(text: keyword.ToString());


                //dgvSentiments.Rows.Add(new string[] { keyword.ToString(), sentiment.PolarityConfidence.ToString(), sentiment.Polarity.ToUpper() });
                //dgvSentiments.FirstDisplayedScrollingRowIndex = dgvSentiments.Rows[dgvSentiments.Rows.Count - 1].Index;
                dgvSentiments.Invoke(new MethodInvoker(delegate
                {
                    dgvSentiments.Rows.Add(new string[] { keyword.ToString(), sentiment.PolarityConfidence.ToString(), sentiment.Polarity.ToUpper() });
                    dgvSentiments.FirstDisplayedScrollingRowIndex = dgvSentiments.Rows[dgvSentiments.Rows.Count - 1].Index;
                }));

                tw.TextualSentiment.PolarityConfidence = sentiment.PolarityConfidence;
                tw.TextualSentiment.Polarity = sentiment.Polarity;

                totalThreadsInExecution++;

                switch (sentiment.Polarity)
                {
                    case "positive":
                        try
                        {
                            posCnt++;
                        }
                        catch { }
                        break;

                    case "negative":
                        try
                        {
                            negCnt++;
                        }
                        catch { }
                        break;

                    case "neutral":
                        try
                        {
                            neutCnt++;
                        }
                        catch { }
                        break;

                    default:
                        //Do Nothing
                        break;
                }

                //Image Sentiment..

                var imageUrl = tw.ImageSentiment.ImageUrl;
                EmotionResult[] faces = new ImageOperations().ImageEmotions(imageUrl, tw);

                string firstExpression = "";
                double maxWeight = 0;
                //int faceCnt = 1;

                foreach (EmotionResult em in faces)
                {
                    if (em.scores.anger > maxWeight)
                    {
                        firstExpression = "Anger";
                        maxWeight = em.scores.anger;
                    }

                    if (em.scores.contempt > maxWeight)
                    {
                        firstExpression = "Contempt";
                        maxWeight = em.scores.contempt;
                    }

                    if (em.scores.disgust > maxWeight)
                    {
                        firstExpression = "Disgust";
                        maxWeight = em.scores.disgust;
                    }

                    if (em.scores.fear > maxWeight)
                    {
                        firstExpression = "Fear";
                        maxWeight = em.scores.fear;
                    }

                    if (em.scores.happiness > maxWeight)
                    {
                        firstExpression = "Happiness";
                        maxWeight = em.scores.happiness;
                    }

                    if (em.scores.neutral > maxWeight)
                    {
                        firstExpression = "Neutral";
                        maxWeight = em.scores.neutral;
                    }

                    if (em.scores.sadness > maxWeight)
                    {
                        firstExpression = "Sadness";
                        maxWeight = em.scores.sadness;
                    }

                    if (em.scores.surprise > maxWeight)
                    {
                        firstExpression = "Surprise";
                        maxWeight = em.scores.surprise;
                    }

                    em.ExactEmotion = firstExpression;
                }

                tw.ImageSentiment.ImageEmotions = faces;
                tw.ImageSentiment.FaceCount = faces.Length;

                // Thread.CurrentThread.Abort();

                /*Bind Values to Datagrid*/

                Image img = ImageOperations.Resize(tw.ImageSentiment.ImageData, 100, 100, false);

                if (tw.ImageSentiment.ImageEmotions.Length > 0)
                {
                    //dgvImageSentiment.Invoke(new MethodInvoker(delegate
                    //{
                    //    dgvImageSentiment.Rows.Add(new object[] { img, tw.Text, tw.ImageSentiment.ImageEmotions[0].ExactEmotion.ToUpper() });
                    //    dgvImageSentiment.FirstDisplayedScrollingRowIndex = dgvImageSentiment.Rows[dgvImageSentiment.Rows.Count - 1].Index;
                    //    dgvImageSentiment.RowTemplate.Height = 110;
                    //}));
                    dgvImageSentiment.Rows.Add(new object[] { img, tw.Text, tw.ImageSentiment.ImageEmotions[0].ExactEmotion.ToUpper()});
                    dgvImageSentiment.FirstDisplayedScrollingRowIndex = dgvImageSentiment.Rows[dgvImageSentiment.Rows.Count - 1].Index;
                    dgvImageSentiment.RowTemplate.Height = 110;
                }
                else
                {
                    //dgvImageSentiment.Invoke(new MethodInvoker(delegate
                    //{
                    //    dgvImageSentiment.Rows.Add(new object[] { img, tw.Text, "No face has been detected!" });
                    //    dgvImageSentiment.FirstDisplayedScrollingRowIndex = dgvImageSentiment.Rows[dgvImageSentiment.Rows.Count - 1].Index;
                    //    dgvImageSentiment.RowTemplate.Height = 110;
                    //}));

                    dgvImageSentiment.Rows.Add(new object[] { img, tw.Text, "No face has been detected!" });
                    dgvImageSentiment.FirstDisplayedScrollingRowIndex = dgvImageSentiment.Rows[dgvImageSentiment.Rows.Count - 1].Index;
                    dgvImageSentiment.RowTemplate.Height = 110;

                }


                string textSent = tw.TextualSentiment.Polarity;

                if (tw.ImageSentiment.ImageEmotions.Length > 0)
                {

                    string imgSent = tw.ImageSentiment.ImageEmotions[0].ExactEmotion;

                    if (imgSent.ToLower() == "happiness" || imgSent.ToLower() == "surprise")
                        imgSent = "Positive";

                    else if (imgSent.ToLower() == "anger" || imgSent.ToLower() == "contempt" || imgSent.ToLower() == "disgust" || imgSent.ToLower() == "fear" || imgSent.ToLower() == "sadness")
                        imgSent = "Negative";


                    if (textSent.ToLower() == "positive" && imgSent.ToLower() == "positive")
                        tw.ImgTextual = "positive";

                    else if (textSent.ToLower() == "positive" && imgSent.ToLower() == "negative")
                        tw.ImgTextual = "negative";


                    else if (textSent.ToLower() == "positive" && imgSent.ToLower() == "neutral")
                        tw.ImgTextual = "positive";

                    else if (textSent.ToLower() == "positive" && imgSent.ToLower() == "neutral")
                        tw.ImgTextual = "positive";

                    else if (textSent.ToLower() == "negative" && imgSent.ToLower() == "positive")
                        tw.ImgTextual = "negative";

                    else if (textSent.ToLower() == "negative" && imgSent.ToLower() == "negative")
                        tw.ImgTextual = "negative";

                    else if (textSent.ToLower() == "negative" && imgSent.ToLower() == "neutral")
                        tw.ImgTextual = "negative";

                    else if (textSent.ToLower() == "neutral" && imgSent.ToLower() == "positive")
                        tw.ImgTextual = "positive";

                    else if (textSent.ToLower() == "neutral" && imgSent.ToLower() == "negative")
                        tw.ImgTextual = "negative";

                    else if (textSent.ToLower() == "neutral" && imgSent.ToLower() == "neutral")
                        tw.ImgTextual = "neutral";

                    //grdFinalSentiment.Invoke(new MethodInvoker(delegate
                    //{
                    //    grdFinalSentiment.Rows.Add(new object[] { tw.Text, tw.TextualSentiment.Polarity.ToUpper(), tw.ImageSentiment.ImageEmotions[0].ExactEmotion.ToUpper(), tw.ImgTextual.ToUpper() });
                    //    grdFinalSentiment.FirstDisplayedScrollingRowIndex = grdFinalSentiment.Rows[grdFinalSentiment.Rows.Count - 1].Index;

                    //}));

                    grdFinalSentiment.Rows.Add(new object[] { tw.Text, tw.TextualSentiment.Polarity.ToUpper(), tw.ImageSentiment.ImageEmotions[0].ExactEmotion.ToUpper(), tw.ImgTextual.ToUpper() });
                    grdFinalSentiment.FirstDisplayedScrollingRowIndex = grdFinalSentiment.Rows[grdFinalSentiment.Rows.Count - 1].Index;
                }


            }
            catch { failCnt++; }
            //finally { Thread.CurrentThread.Name = "CL"; }
        }

        private void initialiseTextThread(object tweet)
        {
            try
            {
                Tweet tw = (Tweet)tweet;

                var keyword = tw.TextualSentiment.Keyword;

                Sentiment sentiment = client.Sentiment(text: keyword.ToString());

                dgvSentiments.Invoke(new MethodInvoker(delegate
                {
                    dgvSentiments.Rows.Add(new string[] { keyword.ToString(), sentiment.PolarityConfidence.ToString(), sentiment.Polarity.ToUpper() });
                    dgvSentiments.FirstDisplayedScrollingRowIndex = dgvSentiments.Rows[dgvSentiments.Rows.Count - 1].Index;
                }));

                tw.TextualSentiment.PolarityConfidence = sentiment.PolarityConfidence;
                tw.TextualSentiment.Polarity = sentiment.Polarity;

                totalThreadsInExecution++;

                switch (sentiment.Polarity)
                {
                    case "positive":
                        try
                        {
                            posCnt++;
                        }
                        catch { }
                        break;

                    case "negative":
                        try
                        {
                            negCnt++;
                        }
                        catch { }
                        break;

                    case "neutral":
                        try
                        {
                            neutCnt++;
                        }
                        catch { }
                        break;

                    default:
                        //Do Nothing
                        break;
                }

                //Thread.CurrentThread.Abort();
            }
            catch { failCnt++; }
        }

        private void initialiseImageThread(object tweet)
        {
            try
            {
                Tweet tw = (Tweet)tweet;
                var imageUrl = tw.ImageSentiment.ImageUrl;
                EmotionResult[] faces = new ImageOperations().ImageEmotions(imageUrl, tw);

                string firstExpression = "";
                double maxWeight = 0;
                //int faceCnt = 1;

                foreach (EmotionResult em in faces)
                {
                    if (em.scores.anger > maxWeight)
                    {
                        firstExpression = "Anger";
                        maxWeight = em.scores.anger;
                    }

                    if (em.scores.contempt > maxWeight)
                    {
                        firstExpression = "Contempt";
                        maxWeight = em.scores.contempt;
                    }

                    if (em.scores.disgust > maxWeight)
                    {
                        firstExpression = "Disgust";
                        maxWeight = em.scores.disgust;
                    }

                    if (em.scores.fear > maxWeight)
                    {
                        firstExpression = "Fear";
                        maxWeight = em.scores.fear;
                    }

                    if (em.scores.happiness > maxWeight)
                    {
                        firstExpression = "Happiness";
                        maxWeight = em.scores.happiness;
                    }

                    if (em.scores.neutral > maxWeight)
                    {
                        firstExpression = "Neutral";
                        maxWeight = em.scores.neutral;
                    }

                    if (em.scores.sadness > maxWeight)
                    {
                        firstExpression = "Sadness";
                        maxWeight = em.scores.sadness;
                    }

                    if (em.scores.surprise > maxWeight)
                    {
                        firstExpression = "Surprise";
                        maxWeight = em.scores.surprise;
                    }

                    em.ExactEmotion = firstExpression;
                }

                tw.ImageSentiment.ImageEmotions = faces;
                tw.ImageSentiment.FaceCount = faces.Length;

               // Thread.CurrentThread.Abort();

                /*Bind Values to Datagrid*/
                
                Image img = ImageOperations.Resize(tw.ImageSentiment.ImageData, 100, 100, false);

                if (tw.ImageSentiment.ImageEmotions.Length > 0)
                {
                    dgvImageSentiment.Invoke(new MethodInvoker(delegate
                    {
                        dgvImageSentiment.Rows.Add(new object[] { img, tw.Text, tw.ImageSentiment.ImageEmotions[0].ExactEmotion.ToUpper() });
                        dgvImageSentiment.FirstDisplayedScrollingRowIndex = dgvImageSentiment.Rows[dgvImageSentiment.Rows.Count - 1].Index;
                        dgvImageSentiment.RowTemplate.Height = 110;
                    }));
                }
                else
                {
                    dgvImageSentiment.Invoke(new MethodInvoker(delegate
                    {
                        dgvImageSentiment.Rows.Add(new object[] { img, tw.Text, "No face has been detected!" });
                        dgvImageSentiment.FirstDisplayedScrollingRowIndex = dgvImageSentiment.Rows[dgvImageSentiment.Rows.Count - 1].Index;
                        dgvImageSentiment.RowTemplate.Height = 110;
                    }));

                }
            }
            catch {  }
        }

        private void computeFinalSentiment(object tweet)
        {
            Tweet tw = (Tweet)tweet;

            string textSent = tw.TextualSentiment.Polarity;

            if (tw.ImageSentiment.ImageEmotions != null && tw.ImageSentiment.ImageEmotions.Length > 0)
            {

                string imgSent = tw.ImageSentiment.ImageEmotions[0].ExactEmotion;

                if (imgSent.ToLower() == "happiness" || imgSent.ToLower() == "surprise")
                    imgSent = "Positive";

                else if (imgSent.ToLower() == "anger" || imgSent.ToLower() == "contempt" || imgSent.ToLower() == "disgust" || imgSent.ToLower() == "fear" || imgSent.ToLower() == "sadness")
                    imgSent = "Negative";


                if (textSent.ToLower() == "positive" && imgSent.ToLower() == "positive")
                    tw.ImgTextual = "positive";

                else if (textSent.ToLower() == "positive" && imgSent.ToLower() == "negative")
                    tw.ImgTextual = "negative";


                else if (textSent.ToLower() == "positive" && imgSent.ToLower() == "neutral")
                    tw.ImgTextual = "positive";

                else if (textSent.ToLower() == "positive" && imgSent.ToLower() == "neutral")
                    tw.ImgTextual = "positive";

                else if (textSent.ToLower() == "negative" && imgSent.ToLower() == "positive")
                    tw.ImgTextual = "negative";

                else if (textSent.ToLower() == "negative" && imgSent.ToLower() == "negative")
                    tw.ImgTextual = "negative";

                else if (textSent.ToLower() == "negative" && imgSent.ToLower() == "neutral")
                    tw.ImgTextual = "negative";

                else if (textSent.ToLower() == "neutral" && imgSent.ToLower() == "positive")
                    tw.ImgTextual = "positive";

                else if (textSent.ToLower() == "neutral" && imgSent.ToLower() == "negative")
                    tw.ImgTextual = "negative";

                else if (textSent.ToLower() == "neutral" && imgSent.ToLower() == "neutral")
                    tw.ImgTextual = "neutral";

                grdFinalSentiment.Invoke(new MethodInvoker(delegate
                        {
                            grdFinalSentiment.Rows.Add(new object[] { tw.Text, tw.TextualSentiment.Polarity.ToUpper(), tw.ImageSentiment.ImageEmotions[0].ExactEmotion.ToUpper(), tw.ImgTextual.ToUpper() });
                            grdFinalSentiment.FirstDisplayedScrollingRowIndex = grdFinalSentiment.Rows[grdFinalSentiment.Rows.Count - 1].Index;

                        }));
            }
        }
      
        private void checkIfAllthreadsAreFinished()
        {
            pnlProcessing.Visible = false;

            string formattedString = string.Empty;

            int textPosCount = 0;
            int textNegCount = 0;
            int textNeutrCount = 0;

            int imgPosCount = 0;
            int imgNegCount = 0;
            int imgNeutrCount = 0;


            int imgTextPosCount = 0;
            int imgTextNegCount = 0;
            int imgTextNeutrCount = 0;

            formattedString += "Keyword   :   " + txtKeyword_Twitter.Text + Environment.NewLine + Environment.NewLine;

            int count = Int32.Parse(txtTweetCount.Text.Trim());

            if (count > LstTwitts.Count) { count = LstTwitts.Count; }

            for (int i = 0; i < count; i++)
            {
                if (LstTwitts[i].ImageSentiment.ImageEmotions!=null && LstTwitts[i].ImageSentiment.ImageEmotions.Length > 0)
                {
                    switch (LstTwitts[i].TextualSentiment.Polarity.ToLower())
                    {

                        case "positive":
                            textPosCount++;
                            break;

                        case "negative":
                            textNegCount++;
                            break;

                        case "neutral":
                            textNeutrCount++;
                            break;
                    }

                    switch (LstTwitts[i].ImageSentiment.ImageEmotions[0].ExactEmotion.ToLower())
                    {

                        case "positive":
                        case "happiness":
                        case "surprise":
                            imgPosCount++;
                            break;

                        case "negative":
                        case "anger":
                        case "contempt":
                        case "disgust":
                        case "fear":
                        case "sadness":
                            imgNegCount++;
                            break;

                        case "neutral":
                            imgNeutrCount++;
                            break;
                    }

                    switch (LstTwitts[i].ImgTextual.ToLower())
                    {

                        case "positive":
                            imgTextPosCount++;
                            break;

                        case "negative":
                            imgTextNegCount++;
                            break;

                        case "neutral":
                            imgTextNeutrCount++;
                            break;
                    }
                }

            }

            string fTextSent = "";
            if (textPosCount > textNegCount /*&& textPosCount > textNeutrCount*/)
                fTextSent = "Positive";
            else if (textNegCount > textPosCount /*&& textNegCount > textNeutrCount*/)
                fTextSent = "Negative";
            else fTextSent = "Neutral";

            string fImgSent = "";
            if (imgPosCount > imgNegCount /*&& imgPosCount > imgNeutrCount*/)
                fImgSent = "Positive";
            else if (imgNegCount > imgPosCount /*&& imgNegCount > imgNeutrCount*/)
                fImgSent = "Negative";
            else fImgSent = "Neutral";

            string fImgTextSent = "";
            if (imgTextPosCount > imgTextNegCount /*&& imgTextPosCount > imgTextNeutrCount*/)
                fImgTextSent = "Positive";
            else if (imgTextNegCount > imgTextPosCount /*&& imgTextNegCount > imgTextNeutrCount*/)
                fImgTextSent = "Negative";
            else fImgTextSent = "Neutral";

            int totalSentiments = textPosCount + imgPosCount + imgTextPosCount;

            double textPerc = (double.Parse(textPosCount.ToString()) / double.Parse(totalSentiments.ToString())) * 100;
            double imagePerc = (double.Parse(imgPosCount.ToString()) / double.Parse(totalSentiments.ToString())) * 100;
            double imgTextPerc = (double.Parse(imgTextPosCount.ToString()) / double.Parse(totalSentiments.ToString())) * 100;

            string strtextPerc = string.Empty;
            string strimagePerc = string.Empty;
            string strimgTextPerc = string.Empty;

            if (textPerc.ToString() == "NaN")
            {
                strtextPerc = "100%";
            }
            else
            {
                if (textPerc.ToString().Length > 4 && textPerc.ToString().IndexOf(".") != -1)
                {
                    strtextPerc = textPerc.ToString().Substring(0, 5) + "%";
                }
                else
                {
                    strtextPerc = textPerc.ToString() + "%";
                }
            }

            if (imagePerc.ToString() == "NaN")
            {
                strimagePerc = "100%";
            }
            else
            {
                if (imagePerc.ToString().Length > 4 && imagePerc.ToString().IndexOf(".") != -1)
                {
                    strimagePerc = imagePerc.ToString().Substring(0, 5) + "%";
                }
                else
                {
                    strimagePerc = imagePerc.ToString() + "%";
                }
            }

            if (imgTextPerc.ToString() == "NaN")
            {
                strimgTextPerc = "100%";
            }
            else
            {
                if (imgTextPerc.ToString().Length > 4 && imgTextPerc.ToString().IndexOf(".") != -1)
                {
                    strimgTextPerc = imgTextPerc.ToString().Substring(0, 5) + "%";
                }
                else
                {
                    strimgTextPerc = imgTextPerc.ToString() + "%";
                }
            }

            strtextPerc = (fTextSent == "Neutral") ? "100%" : strtextPerc;
            strimagePerc = (fImgSent == "Neutral") ? "100%" : strimagePerc;
            strimgTextPerc = (fImgTextSent == "Neutral") ? "100%" : strimgTextPerc;
            
            formattedString += lblTextSent.Text = "Textual Sentiment   :   " + fTextSent + " (" + strtextPerc + ")" + Environment.NewLine + Environment.NewLine;
            formattedString += lblImageSent.Text =  "Image Sentiment   :   " + fImgSent + " (" + strimagePerc + ")" + Environment.NewLine + Environment.NewLine;
            formattedString += lblTextImgSent.Text = "Image + Textual Sentiment   :   " + fImgTextSent + " (" + strimgTextPerc + ")" + Environment.NewLine + Environment.NewLine;

            txtAnalysis.Text = formattedString;

            chartText.Series["Series1"].Points.Clear();
            chartImage.Series["Series1"].Points.Clear();
            chartIText.Series["Series1"].Points.Clear();

            chartText.Series["Series1"].Points.AddXY("Positive", textPosCount.ToString());
            chartText.Series["Series1"].Points.AddXY("Negative", textNegCount.ToString());
            chartText.Series["Series1"].Points.AddXY("Neutral", textNeutrCount.ToString());


            chartImage.Series["Series1"].Points.AddXY("Positive", imgPosCount.ToString());
            chartImage.Series["Series1"].Points.AddXY("Negative", imgNegCount.ToString());
            chartImage.Series["Series1"].Points.AddXY("Neutral", imgNeutrCount.ToString());


            chartIText.Series["Series1"].Points.AddXY("Positive", imgTextPosCount.ToString());
            chartIText.Series["Series1"].Points.AddXY("Negative", imgTextNegCount.ToString());
            chartIText.Series["Series1"].Points.AddXY("Neutral", imgTextNeutrCount.ToString());
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (txtKeyword_Twitter.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Please Provide Keyword", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool MongoError = false;
            string errorMessage = string.Empty;
            try
            {
                string connectionStringTest = "mongodb://" + txtMongoDBServer_Tweeter.Text.Trim();
                MongoClient clientTest = new MongoClient(connectionStringTest);
                MongoServer serverTest = clientTest.GetServer();
                serverTest.Connect();
                serverTest.Disconnect();
            }

            catch (Exception ex)
            {
                MongoError = true;
                errorMessage = ex.Message;
            }

            if (MongoError)
            {
                MessageBox.Show("Error connecting to MongoDB Server. Please check you have started MongoDB server and the Database and Collection names are correct.\n" + errorMessage, "MongoDB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #region Initialization to zero and blank
            
            /*Initialise variables here*/
            tabControl1.SelectedIndex = 0;
            failCnt = 0;  posCnt = 0; negCnt = 0; neutCnt = 0;
            txtAnalysis.Text = "";
            dgvSentiments.Rows.Clear();
            dgvImageSentiment.Rows.Clear();
            grdFinalSentiment.Rows.Clear();
            pnlProcessing.Visible = true;
            totalThreadsInExecution = 0;
            #endregion

            #region MongoDB Server Call
            LstTwitts = new List<Tweet>();
            string connectionString = "mongodb://" + txtMongoDBServer_Tweeter.Text.Trim();
            MongoClient client = new MongoClient(connectionString);
            MongoServer server = client.GetServer();
            var db = server.GetDatabase(txtMongoDBdatabase_Twitter.Text.Trim());
            var col = db.GetCollection(txtMongoDbCollection_Twitter.Text.Trim());
            #endregion

            #region Querying to MongoDB for textual data
            checkForDuplicate.Clear();
            BsonRegularExpression breg = new BsonRegularExpression(txtKeyword_Twitter.Text.Trim());
            var query = Query.Matches("text", breg);
            var items = col.Find(query);
            foreach (var doc in items)
            {
                var imageUrl="";
                try
                {
                    imageUrl = (doc["entities"].AsBsonValue["media"][0].AsBsonValue["media_url"]).AsString;
                }
                catch { }
                if (!String.IsNullOrEmpty(imageUrl))
                {
                    Tweet tw = new Tweet();
                    tw.Text = doc["text"].AsString;

                    ImageSentiments imgSent = new ImageSentiments();
                    imgSent.ImageUrl = imageUrl;
                    tw.ImageSentiment = imgSent;

                    TextualSentiments txtSent = new TextualSentiments();
                    txtSent.Keyword = doc["text"].AsString;
                    tw.TextualSentiment = txtSent;

                    if (!checkForDuplicate.Contains(doc["text"].AsString))
                    {
                        LstTwitts.Add(tw);
                        checkForDuplicate.Add(doc["text"].AsString);

                        if (LstTwitts.Count == Int32.Parse(txtTweetCount.Text))
                            break;
                    }
                }
            }
            
            initialiseThreadGroup(LstTwitts);
            #endregion
        }

        private void dgvImageSentiment_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string tweetText = dgvImageSentiment.Rows[e.RowIndex].Cells[1].Value.ToString();
            Tweet tw = LstTwitts.Find(P => P.Text == tweetText);
            byte[] _imageData = tw.ImageSentiment.ImageData;
            EmotionResult[] _emotionResult = tw.ImageSentiment.ImageEmotions;
            new Faces(_imageData, _emotionResult).ShowDialog();
        }

        private void btnStartTweepy_Click(object sender, EventArgs e)
        {
            if (txtKeyword_Twitter.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Please Provide Keyword", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool MongoError = false;
            string errorMessage = string.Empty;
            try
            {
                string connectionStringTest = "mongodb://" + txtMongoDBServer_Tweeter.Text.Trim();
                MongoClient clientTest = new MongoClient(connectionStringTest);
                MongoServer serverTest = clientTest.GetServer();
                serverTest.Connect();
                serverTest.Disconnect();
            }
            catch (Exception ex)
            {
                MongoError = true;
                errorMessage = ex.Message;
            }
            if (MongoError)
            {
                MessageBox.Show("Error connecting to MongoDB Server. Please check you have started MongoDB server and the Database and Collection names are correct.\n" + errorMessage, "MongoDB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Pr = new System.Diagnostics.Process();
            Pr.StartInfo.WorkingDirectory = txtScriptPath.Text.Trim();
            Pr.StartInfo.FileName = "python.exe";
            Pr.StartInfo.Arguments = "ImportTweets.py" + " " + txtMongoDBdatabase_Twitter.Text.Trim() + " " + txtMongoDbCollection_Twitter.Text.Trim() + " " + txtKeyword_Twitter.Text.Trim() + " " + txtTweetCount.Text.Trim();
            Pr.Start();

            _timer.Interval = 40000;
            _timer.Enabled = true;
            
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            if(!Pr.HasExited)
            Pr.Kill();
        }
        //private void btnGeographicalDistribution_Click(object sender, EventArgs e)
        //{
        //    System.Diagnostics.Process Pr = new System.Diagnostics.Process();
        //    Pr.StartInfo.WorkingDirectory = @"C:\\Program Files\\Google\\Chrome\\Application";
        //    Pr.StartInfo.FileName = "chrome.exe";
        //    Pr.StartInfo.Arguments = "http://localhost:50103/WorldDistribution.aspx";
        //    Pr.Start();
        //}
    }


}

