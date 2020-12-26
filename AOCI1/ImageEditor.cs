using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.OCR;

namespace AOCI1
{

    class ImageEditor
    {
        private Image<Bgr, byte> sourceImage;
        private Image<Bgr, byte> additionalImage;
        private Image<Bgr, byte> prepImage;
        private VideoCapture capture;
        public List<Rectangle> rois = new List<Rectangle>();

        public Image<Bgr, byte> SetSourceImage(string fileName)
        {
            sourceImage = new Image<Bgr, byte>(fileName);
            return sourceImage;
        }

        public Image<Bgr, byte> SetAddtionalImage(string fileName)
        {
            additionalImage = new Image<Bgr, byte>(fileName);
            return additionalImage;
        }

        public void OpenVideo(string fileName)
        {
            capture = new VideoCapture(fileName);

        }

        public Image<Bgr, byte> GetVideoFrame()
        {
            if (capture != null)
            {
                var frame = capture.QueryFrame();
                var image = frame.ToImage<Bgr, byte>();
                return image;
            }
            return null;
        }

        public void ShowImage(ImageBox imageBox, Image<Bgr, byte> image, bool mode = true)
        {
            double coeff;
            if (image.Width > image.Height)
            {
                coeff = (double)imageBox.Width / (double)image.Width;
            }
            else
            {
                coeff = (double)imageBox.Height / (double)image.Height;
            }
            if (mode == false)
            {

                imageBox.Height = image.Height;
                imageBox.Width = image.Width;
                imageBox.Image = image;
            }
            else
            {

                //imageBox.Image = image.Resize(imageBox.Width, imageBox.Height, Inter.Linear);
                //double coeff = (double)imageBox.Width / (double)image.Width;
                imageBox.Image = image.Resize((int)(image.Width * coeff), (int)(coeff * image.Height), Inter.Linear);
            }
        }

        public void ShowImage(ImageBox imageBox, Image<Hsv, byte> image)
        {
            double coeff = (double)imageBox.Width / (double)image.Width;
            imageBox.Image = image.Resize((int)(image.Width * coeff), (int)(coeff * image.Height), Inter.Linear);
        }

        public Image<Bgr, byte> EditImage(Image<Bgr, byte> image, double threshold, double linking, int colorCoeff, bool colorEffect, bool canny)
        {
            if (colorEffect == false && canny == false) return image;
            Image<Gray, byte> grayImage = image.Convert<Gray, byte>();
            grayImage = grayImage.PyrDown();
            grayImage = grayImage.PyrUp();
            double cannyThreshold = threshold;
            double cannyThresholdLinking = linking;
            var cannyEdges = grayImage.Canny(cannyThreshold, cannyThresholdLinking);
            var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
            if (canny == true)
            {
                return cannyEdgesBgr;
            }
            cannyEdgesBgr = cannyEdgesBgr.Resize(image.Width, image.Height, Inter.Linear);
            var resultImage = image.Sub(cannyEdgesBgr);
            if (colorEffect == true)
            {
                for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                    for (int x = 0; x < resultImage.Width; x++)
                        for (int y = 0; y < resultImage.Height; y++)
                        {
                            byte color = resultImage.Data[y, x, channel];
                            color = Convert.ToByte((int)color / colorCoeff * colorCoeff);
                            resultImage.Data[y, x, channel] = color;
                        }
            }
            return resultImage;
        }

        public Image<Bgr, byte> ReturnColorChannel(Image<Bgr, byte> image, string channelName)
        {
            int channelIndex = 0;
            if (channelName == "r") channelIndex = 2;
            if (channelName == "g") channelIndex = 1;
            if (channelName == "b") channelIndex = 0;
            var channel = sourceImage.Split()[channelIndex];
            VectorOfMat vm = new VectorOfMat();
            for (int i = 0; i < 3; i++)
            {
                if (i == channelIndex)
                {
                    vm.Push(channel);
                }
                else
                {
                    vm.Push(channel.CopyBlank());
                }
            }

            Image<Bgr, byte> destImage = sourceImage.CopyBlank();
            CvInvoke.Merge(vm, destImage);

            return destImage;
        }

        public Image<Bgr, byte> ReturnBW(Image<Bgr, byte> image)
        {
            var grayImage = new Image<Gray, byte>(sourceImage.Size);
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    grayImage.Data[y, x, 0] = Convert.ToByte(0.299 * image.Data[y, x, 2] + 0.587 *
                    image.Data[y, x, 1] + 0.114 * image.Data[y, x, 0]);
                }

            return grayImage.Convert<Bgr, byte>();
        }

        private double FitsByte(double value)
        {
            if (value > 255) return 255;
            if (value < 0) return 0;
            return value;
        }

        public Image<Bgr, byte> ReturnSepia(Image<Bgr, byte> image)
        {
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {

                    image.Data[y, x, 2] = Convert.ToByte(FitsByte(0.393 * image.Data[y, x, 2] + 0.769 * image.Data[y, x, 1] + 0.189 * image.Data[y, x, 0]));
                    image.Data[y, x, 1] = Convert.ToByte(FitsByte(0.349 * image.Data[y, x, 2] + 0.686 * image.Data[y, x, 1] + 0.168 * image.Data[y, x, 0]));
                    image.Data[y, x, 0] = Convert.ToByte(FitsByte(0.272 * image.Data[y, x, 2] + 0.534 * image.Data[y, x, 1] + 0.131 * image.Data[y, x, 0]));
                }
            return image;
        }

        public Image<Bgr, byte> ReturnContrast(Image<Bgr, byte> image, int contrast)
        {
            var resultImage = image.Copy();
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {

                    resultImage.Data[y, x, 2] = Convert.ToByte(FitsByte(image.Data[y, x, 2] * contrast));
                    resultImage.Data[y, x, 1] = Convert.ToByte(FitsByte(image.Data[y, x, 1] * contrast));
                    resultImage.Data[y, x, 0] = Convert.ToByte(FitsByte(image.Data[y, x, 0] * contrast));
                }
            return resultImage;
        }

        public Image<Bgr, byte> ReturnBrightness(Image<Bgr, byte> image, int brightness)
        {
            var resultImage = image.Copy();
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {

                    resultImage.Data[y, x, 2] = Convert.ToByte(FitsByte(image.Data[y, x, 2] + brightness));
                    resultImage.Data[y, x, 1] = Convert.ToByte(FitsByte(image.Data[y, x, 1] + brightness));
                    resultImage.Data[y, x, 0] = Convert.ToByte(FitsByte(image.Data[y, x, 0] + brightness));
                }
            return resultImage;
        }

        public Image<Bgr, byte> ReturnAddition(Image<Bgr, byte> image, double coeff)
        {
            var resultImage = image.Copy();
            additionalImage = additionalImage.Resize(image.Width, image.Height, Inter.Linear);
            coeff /= 10;

            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {

                    resultImage.Data[y, x, 2] = Convert.ToByte(FitsByte(image.Data[y, x, 2] * coeff + additionalImage.Data[y, x, 2] * (1 - coeff)));
                    resultImage.Data[y, x, 1] = Convert.ToByte(FitsByte(image.Data[y, x, 1] * coeff + additionalImage.Data[y, x, 1] * (1 - coeff)));
                    resultImage.Data[y, x, 0] = Convert.ToByte(FitsByte(image.Data[y, x, 0] * coeff + additionalImage.Data[y, x, 0] * (1 - coeff)));
                }
            return resultImage;
        }

        public Image<Bgr, byte> ReturnSubstraction(Image<Bgr, byte> image)
        {
            var resultImage = image.Copy();
            additionalImage = additionalImage.Resize(image.Width, image.Height, Inter.Linear);
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    bool isWhite = true;
                    for (int channel = 0; channel < 3; channel++)
                    {
                        if (additionalImage.Data[y, x, channel] < 250)
                        {
                            isWhite = false;
                        }
                    }
                    if (isWhite)
                    {
                        resultImage.Data[y, x, 0] = 0;
                        resultImage.Data[y, x, 1] = 0;
                        resultImage.Data[y, x, 2] = 0;
                    }
                }
            return resultImage;
        }

        public Image<Bgr, byte> ReturnIntersection(Image<Bgr, byte> image)
        {
            var resultImage = image.Copy();
            additionalImage = additionalImage.Resize(image.Width, image.Height, Inter.Linear);
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    bool isBlack = true;
                    for (int channel = 0; channel < 3; channel++)
                    {
                        if (additionalImage.Data[y, x, channel] > 10)
                        {
                            isBlack = false;
                        }
                    }
                    if (isBlack)
                    {
                        resultImage.Data[y, x, 0] = 0;
                        resultImage.Data[y, x, 1] = 0;
                        resultImage.Data[y, x, 2] = 0;
                    }
                }
            return resultImage;
        }

        public Image<Hsv, byte> ReturnH(Image<Bgr, byte> image, int hue)
        {
            var hsvImage = image.Convert<Hsv, byte>();
            for (int y = 0; y < hsvImage.Height; y++)
                for (int x = 0; x < hsvImage.Width; x++)
                {
                    hsvImage.Data[y, x, 0] = Convert.ToByte(hue);
                }

            return hsvImage;
        }

        public Image<Hsv, byte> ReturnS(Image<Bgr, byte> image, int saturation)
        {
            var hsvImage = image.Convert<Hsv, byte>();
            for (int y = 0; y < hsvImage.Height; y++)
                for (int x = 0; x < hsvImage.Width; x++)
                {
                    hsvImage.Data[y, x, 1] = Convert.ToByte(saturation);
                }

            return hsvImage;
        }

        public Image<Hsv, byte> ReturnV(Image<Bgr, byte> image, int value)
        {
            var hsvImage = image.Convert<Hsv, byte>();
            for (int y = 0; y < hsvImage.Height; y++)
                for (int x = 0; x < hsvImage.Width; x++)
                {
                    hsvImage.Data[y, x, 2] = Convert.ToByte(value);
                }

            return hsvImage;
        }

        public Image<Bgr, byte> ReturnBlur(Image<Bgr, byte> image)
        {
            var grayImage = image.Convert<Gray, byte>();
            List<byte> list = new List<byte>();
            int sh = 4;
            int N = 9;

            for (int y = sh; y < grayImage.Height - sh; y++)
                for (int x = sh; x < grayImage.Width - sh; x++)
                {
                    list.Clear();
                    for (int i = -1; i < 2; i++)
                        for (int j = -1; j < 2; j++)
                        {
                            list.Add(grayImage.Data[i + y, j + x, 0]);
                        }
                    list.Sort();
                    grayImage.Data[y, x, 0] = list[N / 2];

                }
            image = grayImage.Convert<Bgr, byte>();
            return image;
        }

        public Image<Bgr, byte> ReturnSharpen(Image<Bgr, byte> image, int[,] mat)
        {
            var grayImage = image.Convert<Gray, byte>();
            var result = grayImage.CopyBlank();


            for (int y = 1; y < grayImage.Height - 1; y++)
                for (int x = 1; x < grayImage.Width - 1; x++)
                {
                    int res = 0;
                    for (int i = -1; i < 2; i++)
                        for (int j = -1; j < 2; j++)
                        {
                            res += grayImage.Data[i + y, j + x, 0] * mat[i + 1, j + 1];
                        }
                    if (res > 255) res = 255;
                    if (res < 0) res = 0;
                    result.Data[y, x, 0] = (byte)res;

                }
            image = result.Convert<Bgr, byte>();
            return image;
        }

        public Image<Bgr, byte> ReturnAquarel(Image<Bgr, byte> image, int brightness, int contrast, double coeff)
        {
            additionalImage = additionalImage.Resize(image.Width, image.Height, Inter.Linear);
            var resultImage = image.Copy();
            resultImage = ReturnBrightness(image, brightness);
            resultImage = ReturnContrast(resultImage, contrast);
            resultImage = ReturnAddition(resultImage, coeff);
            return resultImage;
        }

        public Image<Bgr, byte> ReturnCartoon(Image<Bgr, byte> image, double coeff)
        {
            var resultImage = image.Copy();
            var edges = sourceImage.Convert<Gray, byte>();
            edges = edges.ThresholdBinaryInv(new Gray(100), new Gray(255));
            additionalImage = edges.Convert<Bgr, byte>();
            resultImage = ReturnAddition(resultImage, coeff);
            return resultImage;
        }

        public Image<Bgr, byte> ReturnScaled(Image<Bgr, byte> image, double sX, double sY)
        {
            var resultImage = new Image<Bgr, byte>((int)(image.Width * sX), (int)(image.Height * sY));
            for (int x = 0; x < resultImage.Width - 1; x++)
            {
                for (int y = 0; y < resultImage.Height - 1; y++)
                {
                    double X = x / sX;
                    double Y = y / sY;
                    double baseX = Math.Floor(X);
                    double baseY = Math.Floor(Y);

                    if (baseX >= image.Width - 1 || baseY >= image.Height - 1) continue;

                    double rX = X - baseX;
                    double rY = Y - baseY;
                    double irX = 1 - rX;
                    double irY = 1 - rY;

                    Bgr c = new Bgr();
                    Bgr c1 = new Bgr();
                    Bgr c2 = new Bgr();

                    c1.Blue = image.Data[(int)baseY, (int)baseX, 0] * irX + image.Data[(int)baseY, (int)baseX + 1, 0] * rX;
                    c1.Green = image.Data[(int)baseY, (int)baseX, 1] * irX + image.Data[(int)baseY, (int)baseX + 1, 1] * rX;
                    c1.Red = image.Data[(int)baseY, (int)baseX, 2] * irX + image.Data[(int)baseY, (int)baseX + 1, 2] * rX;

                    c2.Blue = image.Data[(int)baseY + 1, (int)baseX, 0] * irX + image.Data[(int)baseY + 1, (int)baseX + 1, 0] * rX;
                    c2.Green = image.Data[(int)baseY + 1, (int)baseX, 0] * irX + image.Data[(int)baseY + 1, (int)baseX + 1, 0] * rX;
                    c2.Red = image.Data[(int)baseY + 1, (int)baseX, 0] * irX + image.Data[(int)baseY + 1, (int)baseX + 1, 0] * rX;

                    c.Blue = c1.Blue * irY + c2.Blue * rY;
                    c.Green = c1.Green * irY + c2.Green * rY;
                    c.Red = c1.Red * irY + c2.Red * rY;

                    resultImage[y, x] = c;

                }
            }

            return resultImage;
        }

        public Image<Bgr, byte> ReturnShifted(Image<Bgr, byte> image, double sX, double sY)
        {
            var resultImage = image.CopyBlank();
            for (int x = 0; x < resultImage.Width - 1; x++)
            {
                for (int y = 0; y < resultImage.Height - 1; y++)
                {
                    int newX = x + Convert.ToInt32(sX * (image.Height - y));
                    int newY = y + Convert.ToInt32(sY * (image.Width - x));
                    if (newX < resultImage.Width && newY < resultImage.Height && newX >= 0 && newY >= 0)
                    {
                        resultImage[newY, newX] = image[y, x];
                    }
                }
            }

            return resultImage;
        }

        public Image<Bgr, byte> ReturnRotated(Image<Bgr, byte> image, double angle, int centerX = 0, int centerY = 0)
        {
            angle = angle / 57.2956;
            var resultImage = image.CopyBlank();
            for (int x = 0; x < resultImage.Width - 1; x++)
            {
                for (int y = 0; y < resultImage.Height - 1; y++)
                {
                    int newX = Convert.ToInt32(Math.Cos(angle) * (x - centerX) - Math.Sin(angle) * (y - centerY)) + centerX;
                    int newY = Convert.ToInt32(Math.Sin(angle) * (x - centerX) + Math.Cos(angle) * (y - centerY)) + centerY;
                    if (newX < resultImage.Width && newY < resultImage.Height && newX >= 0 && newY >= 0)
                    {
                        double X = newX;
                        double Y = newY;
                        double baseX = Math.Floor(X);
                        double baseY = Math.Floor(Y);

                        if (baseX >= image.Width - 1 || baseY >= image.Height - 1) continue;

                        double rX = X - baseX;
                        double rY = Y - baseY;
                        double irX = 1 - rX;
                        double irY = 1 - rY;

                        Bgr c = new Bgr();
                        Bgr c1 = new Bgr();
                        Bgr c2 = new Bgr();

                        c1.Blue = image.Data[(int)baseY, (int)baseX, 0] * irX + image.Data[(int)baseY, (int)baseX + 1, 0] * rX;
                        c1.Green = image.Data[(int)baseY, (int)baseX, 1] * irX + image.Data[(int)baseY, (int)baseX + 1, 1] * rX;
                        c1.Red = image.Data[(int)baseY, (int)baseX, 2] * irX + image.Data[(int)baseY, (int)baseX + 1, 2] * rX;

                        c2.Blue = image.Data[(int)baseY + 1, (int)baseX, 0] * irX + image.Data[(int)baseY + 1, (int)baseX + 1, 0] * rX;
                        c2.Green = image.Data[(int)baseY + 1, (int)baseX, 0] * irX + image.Data[(int)baseY + 1, (int)baseX + 1, 0] * rX;
                        c2.Red = image.Data[(int)baseY + 1, (int)baseX, 0] * irX + image.Data[(int)baseY + 1, (int)baseX + 1, 0] * rX;

                        c.Blue = c1.Blue * irY + c2.Blue * rY;
                        c.Green = c1.Green * irY + c2.Green * rY;
                        c.Red = c1.Red * irY + c2.Red * rY;

                        resultImage[y, x] = c;

                    }
                }
            }

            return resultImage;
        }

        public Image<Bgr, byte> ReturnReflected(Image<Bgr, byte> image, int qX, int qY)
        {
            var resultImage = image.CopyBlank();
            int newX, newY;
            for (int x = 0; x < resultImage.Width - 1; x++)
            {
                for (int y = 0; y < resultImage.Height - 1; y++)
                {
                    if (qX == -1)
                    {
                        newX = x * qX + image.Width - 1;
                    }
                    else
                    {
                        newX = x;
                    }
                    if (qY == -1)
                    {
                        newY = y * qY + image.Height - 1;
                    }
                    else
                    {
                        newY = y;
                    }
                    if (newX > image.Width - 1 || newY > image.Height - 1 || newX < 0 || newY < 0) continue;
                    resultImage[newY, newX] = image[y, x];

                }
            }

            return resultImage;
        }

        public Image<Bgr, byte> ReturnHomo(Image<Bgr, byte> image, PointF[] srcPoints)
        {
            var resultImage = image.CopyBlank();

            var destPoints = new PointF[]
            {
                 new PointF(0, 0),
                 new PointF(0, image.Height - 1),
                 new PointF(image.Width - 1, image.Height - 1),
                 new PointF(image.Width - 1, 0)
            };
            var homographyMatrix = CvInvoke.GetPerspectiveTransform(srcPoints, destPoints);

            CvInvoke.WarpPerspective(sourceImage, resultImage, homographyMatrix, resultImage.Size);

            return resultImage;
        }

        public Image<Bgr, byte> ReturnBinarized(Image<Bgr, byte> image, int thresholdValue)
        {
            var resultImage = image.CopyBlank();

            var grayImage = image.Convert<Gray, byte>();
            int kernelSize = 3;
            var bluredImage = grayImage.SmoothGaussian(kernelSize);
            var threshold = new Gray(thresholdValue);
            var color = new Gray(255);
            var binarizedImage = bluredImage.ThresholdBinary(threshold, color);

            resultImage = binarizedImage.Convert<Bgr, byte>();
            prepImage = resultImage;
            return resultImage;
        }




        public Image<Bgr, byte> ReturnContours(Image<Bgr, byte> image, int minArea, Label label)
        {
            if(prepImage == null)
            {
                prepImage = ReturnBinarized(image, 90);
            }
            var resultImage = prepImage.Convert<Gray, byte>();
            int trisCount = 0;
            int rectCount = 0;
            int circleCount = 0;
            // shapes
            var contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(
            resultImage,
             contours,
             null,
             RetrType.List,
             ChainApproxMethod.ChainApproxSimple);

            var contoursImage = sourceImage.Copy();
            for (int i = 0; i < contours.Size; i++)
            {
                
                //contoursImage.Draw(points, new Bgr(Color.GreenYellow), 2); // отрисовка точек

                var approxContour = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(contours[i], approxContour, CvInvoke.ArcLength(contours[i], true) * 0.05, true);
                var points = approxContour.ToArray();
                if (approxContour.Size == 3)
                {
                    var S = CvInvoke.ContourArea(approxContour, false);
                    if (S > minArea)
                    {
                        trisCount++;
                        var pointsTri = approxContour.ToArray();
                        contoursImage.Draw(new Triangle2DF(pointsTri[0], pointsTri[1], pointsTri[2]), new Bgr(Color.GreenYellow), 2);
                    }
                }
                if (isRectangle(points))
                {
                    var S = CvInvoke.ContourArea(approxContour, false);
                    if (S > minArea)
                    {
                        rectCount++;
                        contoursImage.Draw(CvInvoke.MinAreaRect(approxContour), new Bgr(Color.Blue), 2);
                    }
                }

            }

            //circles

            List<CircleF> circles = new List<CircleF>(CvInvoke.HoughCircles(resultImage, HoughModes.Gradient, 1.0, 250, 100, 50, 5, contoursImage.Width / 3));
            foreach (CircleF circle in circles)
            {
                CvInvoke.Circle(contoursImage, Point.Round(circle.Center), (int)circle.Radius, new Bgr(Color.Red).MCvScalar, 2);
                circleCount++;
                //resultImage.Draw(circle, new Bgr(Color.GreenYellow), 2);
            }
            label.Text = "Количество треугольников = " + trisCount + "\nКоличество прямоугольников = " + rectCount + "\nКоличество кругов = " + circleCount;
            return contoursImage;
        }

        private bool isRectangle(Point[] points)
        {
            int delta = 10; // максимальное отклонение от прямого угла
            LineSegment2D[] edges = PointCollection.PolyLine(points, true);
            for (int i = 0; i < edges.Length; i++) // обход всех ребер контура
            {
                double angle = Math.Abs(edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));
                if (angle < 90 - delta || angle > 90 + delta) // если угол непрямой
                {
                    return false;
                }
            }
            return true;
        }

        public Image<Bgr, byte> ReturnByColor(Image<Bgr, byte> image, int colorValue, int colorRange)
        {
            var hsvImage = image.Convert<Hsv, byte>(); 
            var hueChannel = hsvImage.Split()[0]; 
            byte color = (byte)colorValue; 
            byte rangeDelta = (byte)colorRange; 
            var resultImage = hueChannel.InRange(new Gray(color - rangeDelta), new Gray(color +
            rangeDelta));
            return resultImage.Convert<Bgr, byte>();
        }

        public Image<Bgr, byte> ReturnTextAreas(Image<Bgr, byte> image)
        {
            var resultImage = image.Copy();
            var thresh = resultImage.Convert<Gray, byte>();
            thresh._ThresholdBinaryInv(new Gray(128), new Gray(255));
            thresh._Dilate(5);

            Mat hierarchy = new Mat();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(thresh, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            
            for (int i = 0; i < contours.Size; i++)
            {
                if (CvInvoke.ContourArea(contours[i], false) > 50) //игнорирование маленьких контуров
                {
                    Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                    rois.Add(rect);
                    resultImage.Draw(rect, new Bgr(Color.Blue), 2);
                }
            }
            
            return resultImage;
        }

        public Image<Bgr, byte> ReturnROI(Image<Bgr, byte> image, int index)
        {
            var resultImage = image.Copy();
            resultImage.ROI = rois[index];
            return resultImage;
        }

        Tesseract _ocr = new Tesseract("D:\\Tesseract", "rus", OcrEngineMode.TesseractLstmCombined);
        public string ReturnText(Image<Bgr, byte> roiImg, string language)
        {
            

            _ocr.SetImage(roiImg); //фрагмент изображения, содержащий текст
            _ocr.Recognize(); //распознание текста
            Tesseract.Character[] words = _ocr.GetCharacters(); //получение найденных символов

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < words.Length; i++)
            {
                strBuilder.Append(words[i].Text);
            }

            return strBuilder.ToString();
        }

        
    }
}
