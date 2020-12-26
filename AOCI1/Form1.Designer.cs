namespace AOCI1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.button1 = new System.Windows.Forms.Button();
            this.imageBox2 = new Emgu.CV.UI.ImageBox();
            this.label = new System.Windows.Forms.Label();
            this.buttonCamera = new System.Windows.Forms.Button();
            this.buttonTextArea = new System.Windows.Forms.Button();
            this.buttonViewPart = new System.Windows.Forms.Button();
            this.buttonMask = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.labelText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox1
            // 
            this.imageBox1.Location = new System.Drawing.Point(0, 0);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(500, 500);
            this.imageBox1.TabIndex = 2;
            this.imageBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(506, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Open image";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // imageBox2
            // 
            this.imageBox2.Location = new System.Drawing.Point(627, 0);
            this.imageBox2.Name = "imageBox2";
            this.imageBox2.Size = new System.Drawing.Size(500, 500);
            this.imageBox2.TabIndex = 2;
            this.imageBox2.TabStop = false;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(624, 516);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(0, 13);
            this.label.TabIndex = 7;
            // 
            // buttonCamera
            // 
            this.buttonCamera.Location = new System.Drawing.Point(507, 42);
            this.buttonCamera.Name = "buttonCamera";
            this.buttonCamera.Size = new System.Drawing.Size(114, 23);
            this.buttonCamera.TabIndex = 8;
            this.buttonCamera.Text = "Camera";
            this.buttonCamera.UseVisualStyleBackColor = true;
            this.buttonCamera.Click += new System.EventHandler(this.buttonCamera_Click);
            // 
            // buttonTextArea
            // 
            this.buttonTextArea.Location = new System.Drawing.Point(506, 71);
            this.buttonTextArea.Name = "buttonTextArea";
            this.buttonTextArea.Size = new System.Drawing.Size(114, 23);
            this.buttonTextArea.TabIndex = 9;
            this.buttonTextArea.Text = "Text Area";
            this.buttonTextArea.UseVisualStyleBackColor = true;
            this.buttonTextArea.Click += new System.EventHandler(this.buttonTextArea_Click);
            // 
            // buttonViewPart
            // 
            this.buttonViewPart.Location = new System.Drawing.Point(506, 100);
            this.buttonViewPart.Name = "buttonViewPart";
            this.buttonViewPart.Size = new System.Drawing.Size(114, 23);
            this.buttonViewPart.TabIndex = 10;
            this.buttonViewPart.Text = "View part";
            this.buttonViewPart.UseVisualStyleBackColor = true;
            this.buttonViewPart.Click += new System.EventHandler(this.buttonViewPart_Click);
            // 
            // buttonMask
            // 
            this.buttonMask.Location = new System.Drawing.Point(506, 149);
            this.buttonMask.Name = "buttonMask";
            this.buttonMask.Size = new System.Drawing.Size(114, 23);
            this.buttonMask.TabIndex = 11;
            this.buttonMask.Text = "Mask";
            this.buttonMask.UseVisualStyleBackColor = true;
            this.buttonMask.Click += new System.EventHandler(this.buttonMask_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(507, 126);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(117, 17);
            this.listBox1.TabIndex = 12;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // labelText
            // 
            this.labelText.AutoSize = true;
            this.labelText.Location = new System.Drawing.Point(630, 503);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(35, 13);
            this.labelText.TabIndex = 13;
            this.labelText.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 589);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.buttonMask);
            this.Controls.Add(this.buttonViewPart);
            this.Controls.Add(this.buttonTextArea);
            this.Controls.Add(this.buttonCamera);
            this.Controls.Add(this.label);
            this.Controls.Add(this.imageBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.imageBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox1;
        private System.Windows.Forms.Button button1;
        private Emgu.CV.UI.ImageBox imageBox2;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button buttonCamera;
        private System.Windows.Forms.Button buttonTextArea;
        private System.Windows.Forms.Button buttonViewPart;
        private System.Windows.Forms.Button buttonMask;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label labelText;
    }
}

