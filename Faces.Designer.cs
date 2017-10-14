namespace SentimentAnalysis
{
    partial class Faces
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.picBoxImage = new System.Windows.Forms.PictureBox();
            this.lblErrorText = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.emotion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.surprise = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sadness = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.neutral = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.happiness = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.disgust = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contempt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Anger = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.face = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFacesSentiment = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxImage)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacesSentiment)).BeginInit();
            this.SuspendLayout();
            // 
            // picBoxImage
            // 
            this.picBoxImage.BackColor = System.Drawing.Color.White;
            this.picBoxImage.Location = new System.Drawing.Point(12, 12);
            this.picBoxImage.Name = "picBoxImage";
            this.picBoxImage.Size = new System.Drawing.Size(290, 290);
            this.picBoxImage.TabIndex = 0;
            this.picBoxImage.TabStop = false;
            // 
            // lblErrorText
            // 
            this.lblErrorText.AutoSize = true;
            this.lblErrorText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorText.Location = new System.Drawing.Point(309, 44);
            this.lblErrorText.Name = "lblErrorText";
            this.lblErrorText.Size = new System.Drawing.Size(0, 15);
            this.lblErrorText.TabIndex = 65;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Location = new System.Drawing.Point(4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1322, 307);
            this.panel1.TabIndex = 66;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.SteelBlue;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1172, 265);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(134, 32);
            this.btnClose.TabIndex = 67;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // emotion
            // 
            this.emotion.HeaderText = "Emotion";
            this.emotion.Name = "emotion";
            // 
            // surprise
            // 
            this.surprise.HeaderText = "Surprise";
            this.surprise.Name = "surprise";
            // 
            // Sadness
            // 
            this.Sadness.HeaderText = "Sadness";
            this.Sadness.Name = "Sadness";
            // 
            // neutral
            // 
            this.neutral.HeaderText = "Neutral";
            this.neutral.Name = "neutral";
            // 
            // happiness
            // 
            this.happiness.HeaderText = "Happiness";
            this.happiness.Name = "happiness";
            // 
            // fear
            // 
            this.fear.HeaderText = "Fear";
            this.fear.Name = "fear";
            // 
            // disgust
            // 
            this.disgust.HeaderText = "Disgust";
            this.disgust.Name = "disgust";
            // 
            // contempt
            // 
            this.contempt.HeaderText = "Contempt";
            this.contempt.Name = "contempt";
            // 
            // Anger
            // 
            this.Anger.HeaderText = "Anger";
            this.Anger.Name = "Anger";
            // 
            // face
            // 
            this.face.HeaderText = "Face";
            this.face.Name = "face";
            // 
            // dgvFacesSentiment
            // 
            this.dgvFacesSentiment.AllowUserToAddRows = false;
            this.dgvFacesSentiment.AllowUserToResizeColumns = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.dgvFacesSentiment.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFacesSentiment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFacesSentiment.BackgroundColor = System.Drawing.Color.White;
            this.dgvFacesSentiment.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFacesSentiment.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvFacesSentiment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFacesSentiment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.face,
            this.Anger,
            this.contempt,
            this.disgust,
            this.fear,
            this.happiness,
            this.neutral,
            this.Sadness,
            this.surprise,
            this.emotion});
            this.dgvFacesSentiment.Location = new System.Drawing.Point(308, 12);
            this.dgvFacesSentiment.MultiSelect = false;
            this.dgvFacesSentiment.Name = "dgvFacesSentiment";
            this.dgvFacesSentiment.RowHeadersVisible = false;
            this.dgvFacesSentiment.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFacesSentiment.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFacesSentiment.Size = new System.Drawing.Size(1000, 254);
            this.dgvFacesSentiment.TabIndex = 64;
            // 
            // Faces
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1329, 316);
            this.Controls.Add(this.lblErrorText);
            this.Controls.Add(this.dgvFacesSentiment);
            this.Controls.Add(this.picBoxImage);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Faces";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Faces";
            this.Load += new System.EventHandler(this.Faces_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxImage)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacesSentiment)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBoxImage;
        private System.Windows.Forms.Label lblErrorText;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn emotion;
        private System.Windows.Forms.DataGridViewTextBoxColumn surprise;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sadness;
        private System.Windows.Forms.DataGridViewTextBoxColumn neutral;
        private System.Windows.Forms.DataGridViewTextBoxColumn happiness;
        private System.Windows.Forms.DataGridViewTextBoxColumn fear;
        private System.Windows.Forms.DataGridViewTextBoxColumn disgust;
        private System.Windows.Forms.DataGridViewTextBoxColumn contempt;
        private System.Windows.Forms.DataGridViewTextBoxColumn Anger;
        private System.Windows.Forms.DataGridViewTextBoxColumn face;
        private System.Windows.Forms.DataGridView dgvFacesSentiment;
    }
}