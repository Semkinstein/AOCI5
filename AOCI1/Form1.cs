using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AOCI1
{
    public partial class Form1 : Form
    {
        ImageEditor imgEditor = new ImageEditor();
        Image<Bgr, byte> image, defaultImage;
        PointF[] srcPoints = new PointF[4];
        List<PointF> points = new List<PointF>();
        int mouseX, mouseY;
        VideoCapture capture;
        CascadeClassifier face = new CascadeClassifier("D:\\Tesseract\\haarcascade_frontalface_default.xml");
        Mat frame;
        Mat capImage = new Mat();

        public Form1()
        {
            InitializeComponent();
            //button1.Click += new EventHandler(button1_Click);
            imageBox1.MouseClick += new MouseEventHandler(imageBox2_MouseClick);

        }

        private void imageBox2_MouseClick(object sender, MouseEventArgs e)
        {
            mouseX = (int)(e.Location.X / imageBox1.ZoomScale / ((double)imageBox1.Width / (double)defaultImage.Width));
            mouseY = (int)(e.Location.Y / imageBox1.ZoomScale / ((double)imageBox1.Width / (double)defaultImage.Width));

            points.Add(new PointF(mouseX, mouseY));
            if(points.Count > 4)
            {
                points.RemoveAt(0);
            }
            if(points.Count == 4)
            {
                srcPoints = points.ToArray();
            }

            Point center = new Point(mouseX, mouseY);
            int radius = 2;
            int thickness = 2;
            var color = new Bgr(Color.Red).MCvScalar;
            // функция, рисующая на изображении круг с заданными параметрами
            CvInvoke.Circle(defaultImage, center, radius, color, thickness);
            imgEditor.ShowImage(imageBox1, defaultImage);
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            defaultImage = imgEditor.SetSourceImage(OpenImageFile());
            imgEditor.ShowImage(imageBox1, defaultImage);
        }


        private void buttonTextArea_Click(object sender, EventArgs e)
        {
            image = imgEditor.ReturnTextAreas(defaultImage);
            FillImage2();
        }

        private void buttonViewPart_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < imgEditor.rois.Count; index++)
                listBox1.Items.Add(index);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            image = imgEditor.ReturnROI(defaultImage, listBox1.SelectedIndex);
            FillImage2();
            var str = imgEditor.ReturnText(image, "rus");
            labelText.Text = str;
        }

        private void buttonCamera_Click(object sender, EventArgs e)
        {
            capture = new VideoCapture();
            capture.Start();
            capture.ImageGrabbed += ProcessFrame;
            
        }

      
        public void ProcessFrame(object sender, EventArgs e)
        {
            capture.Retrieve(capImage);

            var input = capImage.ToImage<Bgr, byte>();

            string str = "";
            imgEditor.ReturnTextAreas(input);
            for (int i = 0; i < imgEditor.rois.Count; i++)
            {
                var part = imgEditor.ReturnROI(input, i);
                str += imgEditor.ReturnText(part, "rus");
            }

            BeginInvoke(new MethodInvoker(delegate
            {
                labelText.Text = str;
            }));

            List<Rectangle> faces = new List<Rectangle>();
            Image<Bgra, byte> res = input.Convert<Bgra, byte>();

            using (Mat ugray = new Mat())
            {
                CvInvoke.CvtColor(capImage, ugray, ColorConversion.Bgr2Gray);
                Rectangle[] facesDetected = face.DetectMultiScale(ugray, 1.1, 10, new Size(10, 10));
                faces.AddRange(facesDetected);
            }

            foreach (Rectangle rect in faces)
                input.Draw(rect, new Bgr(Color.Yellow), 2);
            imageBox1.Image = input;

            if(frame != null)
            foreach (Rectangle rect in faces) //для каждого лица
            {
                res.ROI = rect; //для области содержащей лицо
                Image<Bgra, byte> small = frame.ToImage<Bgra, byte>().Resize(rect.Width, rect.Height, Inter.Nearest); //создание
                                                                                                                      //копирование изображения small на изображение res с использованием маски копирования mask
                CvInvoke.cvCopy(small, res, small.Split()[3]);
                res.ROI = Rectangle.Empty;
            }

            imageBox2.Image = res;
        }

        private void buttonMask_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.ShowDialog();

            frame = CvInvoke.Imread(f.FileName, ImreadModes.Unchanged);

            imageBox2.Image = frame.Split()[3];
        }

        private string OpenImageFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(); // открытие диалога выбора файла
            if (result == DialogResult.OK) // открытие выбранного файла
            {
                string fileName = openFileDialog.FileName;
                return fileName;
            }
            return null;
        }

        

        
      

       

       //filllllllllllll

        private void FillImage2(bool mode = true)
        {
            
            imgEditor.ShowImage(imageBox2, image, mode);
        }

        
    }
}
