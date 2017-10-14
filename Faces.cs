using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SentimentAnalysis.Entities;
using SentimentAnalysis.BusinessLayer;
namespace SentimentAnalysis
{
    public partial class Faces : Form
    {
        private byte[] ImageData;
        private EmotionResult[] ImageEmotions;
        
        public Faces(byte[] _imageData, EmotionResult[] _imageEmotions)
        {
            InitializeComponent();
            ImageData = _imageData;
            ImageEmotions = _imageEmotions;
        }

        private void Faces_Load(object sender, EventArgs e)
        {
            Image img = ImageOperations.Resize(ImageData, 290, 290, false);
            picBoxImage.Image = img;
            if (ImageEmotions.Length > 0)
            {
                int faceCnt = 1;
                lblErrorText.Text = "";
                dgvFacesSentiment.Visible = true;
                foreach (EmotionResult faces in ImageEmotions)
                {
                    dgvFacesSentiment.Rows.Add(new string[] { "Face : "+faceCnt.ToString(), faces.scores.anger.ToString(),
                faces.scores.contempt.ToString(),
                faces.scores.disgust.ToString(),
                faces.scores.fear.ToString(),
                faces.scores.happiness.ToString(),
                faces.scores.neutral.ToString(),
                faces.scores.sadness.ToString(),
                faces.scores.surprise.ToString(),
                faces.ExactEmotion
                });
                    faceCnt++;
                }
                dgvFacesSentiment.Rows[0].Selected = false;
            }

            else
            {
                lblErrorText.Text = "Sorry. No face has been detected!";
                lblErrorText.ForeColor = Color.Red;
                dgvFacesSentiment.Visible = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
