using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebEye.Controls.Wpf;
using System.Drawing;
using Microsoft.Win32;
using System.Windows.Threading;

using ZedGraph;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.UI;
using Emgu.CV.Util;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        private int i = 0;
        LineSegment2D[] lines = { };

        public Page2()
        {
            InitializeComponent();
        }

        private void StartPreviewControl_Click(object sender, RoutedEventArgs e)
        {
            List<WebCameraId> cameras = new List<WebCameraId>(webCameraControl1.GetVideoCaptureDevices());
            webCameraControl1.StartCapture(cameras[0]);
        }

        private void StartAnalyzingControl_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string path = "C:\\Users\\Chase\\Desktop\\camoSample\\realTime\\TRUE-TEST-IMAGE" + i.ToString() + ".jpg";
            Bitmap imageTestFrame = webCameraControl1.GetCurrentImage();

            Image<Bgr, Byte> testFrameImage = new Image<Bgr, byte>(Real_Time_Compare(imageTestFrame));
            //Image<Bgr, Byte> testFrameImage = new Image<Bgr, byte>(imageTestFrame);
            Mat trueTestImage = testFrameImage.Mat;

            CvInvoke.Imwrite(path, trueTestImage);
            BitmapImage bitmapIMG = new BitmapImage();
            Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);
            imageWebCam.Source = new BitmapImage(uri);
            i++;

        }

        public Bitmap Real_Time_Compare(Bitmap bit)
        {
            Image<Bgr, Byte> src = new Image<Bgr, Byte>(bit);
            Image<Bgr, Byte> srcReturn = new Image<Bgr, Byte>(bit);
            Image<Bgr, Byte> srcTwo = new Image<Bgr, Byte>(bit);
            Mat srcMat = src.Mat;
            Mat srcMatTwo = srcTwo.Mat;
            Mat grayscale = new Mat();
            Mat roiEdges = new Mat();
            Mat grayscaleTwo = new Mat();
            Mat roiEdgesTwo = new Mat();

            CvInvoke.CvtColor(srcMat, grayscale, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.CvtColor(srcMatTwo, grayscaleTwo, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.Canny(grayscale, roiEdges, 80, 250);
            CvInvoke.Canny(grayscaleTwo, roiEdgesTwo, 80, 250);

            Image<Bgr, Byte> roi_Edges = roiEdges.ToImage<Bgr, Byte>();
            CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\realTime\\realTime_Roi_Edges.jpg", roi_Edges);

            Image<Bgr, Byte> roi_Edges_Two = roiEdgesTwo.ToImage<Bgr, Byte>();
            CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\realTime\\realTime_roi_Edges_Two.jpg", roi_Edges_Two);

            MCvScalar m = new MCvScalar(0, 0, 255);
            lines = CvInvoke.HoughLinesP(roiEdges, 1, Math.PI / 180, 0, 0, 0);
            double houghLinesCount = lines.Count();
            Image<Bgr, Byte> srcLineImage = src.CopyBlank();


            foreach (LineSegment2D line in lines)
            {
                srcLineImage.Draw(line, new Bgr(System.Drawing.Color.Red), 1);
                src.Draw(line, new Bgr(System.Drawing.Color.Red), 1);
            }


            List<double> matchList = new List<double>();
            List<System.Drawing.Rectangle> rectangleListROI = new List<System.Drawing.Rectangle>();

            roi_Edges_Two.ROI = new System.Drawing.Rectangle(0, 0, roi_Edges_Two.Width / 3, roi_Edges_Two.Height / 2);
            System.Drawing.Rectangle rectangleOneROI = roi_Edges_Two.ROI;
            rectangleListROI.Add(rectangleOneROI);
            Mat roiForHoughOne = roi_Edges_Two.Mat;
            Mat roiCannyOne = new Mat();
            CvInvoke.Canny(roiForHoughOne, roiCannyOne, 80, 250);
            LineSegment2D[] leftTopLines = CvInvoke.HoughLinesP(roiCannyOne, 1, Math.PI / 180, 0, 0, 0);
            double leftTopMatch = leftTopLines.Count();
            matchList.Add(leftTopMatch);
            roi_Edges_Two.ROI = System.Drawing.Rectangle.Empty;

            roi_Edges_Two.ROI = new System.Drawing.Rectangle(roi_Edges_Two.Width / 3, 0, roi_Edges_Two.Width / 3, roi_Edges_Two.Height / 2);
            System.Drawing.Rectangle rectangleTwoROI = roi_Edges_Two.ROI;
            rectangleListROI.Add(rectangleTwoROI);
            Mat roiForHoughTwo = roi_Edges_Two.Mat;
            Mat roiCannyTwo = new Mat();
            CvInvoke.Canny(roiForHoughTwo, roiCannyTwo, 80, 250);
            LineSegment2D[] centerTopLines = CvInvoke.HoughLinesP(roiCannyTwo, 1, Math.PI / 180, 0, 0, 0);
            double centerTopMatch = centerTopLines.Count();
            matchList.Add(centerTopMatch);
            roi_Edges_Two.ROI = System.Drawing.Rectangle.Empty;

            roi_Edges_Two.ROI = new System.Drawing.Rectangle(2 * roi_Edges_Two.Width / 3, 0, roi_Edges_Two.Width / 3, roi_Edges_Two.Height / 2);
            System.Drawing.Rectangle rectangleThreeROI = roi_Edges_Two.ROI;
            rectangleListROI.Add(rectangleThreeROI);
            Mat roiForHoughThree = roi_Edges_Two.Mat;
            Mat roiCannyThree = new Mat();
            CvInvoke.Canny(roiForHoughThree, roiCannyThree, 80, 250);
            LineSegment2D[] rightTopLines = CvInvoke.HoughLinesP(roiCannyThree, 1, Math.PI / 180, 0, 0, 0);
            double rightTopMatch = rightTopLines.Count();
            matchList.Add(rightTopMatch);
            roi_Edges_Two.ROI = System.Drawing.Rectangle.Empty;

            roi_Edges_Two.ROI = new System.Drawing.Rectangle(0, roi_Edges_Two.Height / 2, roi_Edges_Two.Width / 3, roi_Edges_Two.Height / 2);
            System.Drawing.Rectangle rectangleFourROI = roi_Edges_Two.ROI;
            rectangleListROI.Add(rectangleFourROI);
            Mat roiForHoughFour = roi_Edges_Two.Mat;
            Mat roiCannyFour = new Mat();
            CvInvoke.Canny(roiForHoughFour, roiCannyFour, 80, 250);
            LineSegment2D[] leftBottomLines = CvInvoke.HoughLinesP(roiCannyFour, 1, Math.PI / 180, 0, 0, 0);
            double leftBottomMatch = leftBottomLines.Count();
            matchList.Add(leftBottomMatch);
            roi_Edges_Two.ROI = System.Drawing.Rectangle.Empty;

            roi_Edges_Two.ROI = new System.Drawing.Rectangle(roi_Edges_Two.Width / 3, roi_Edges_Two.Height / 2, roi_Edges_Two.Width / 3, roi_Edges_Two.Height / 2);
            Mat roiForHoughFive = roi_Edges_Two.Mat;
            System.Drawing.Rectangle rectangleFiveROI = roi_Edges_Two.ROI;
            rectangleListROI.Add(rectangleFiveROI);
            Mat roiCannyFive = new Mat();
            CvInvoke.Canny(roiForHoughFive, roiCannyFive, 80, 250);
            LineSegment2D[] centerBottomLines = CvInvoke.HoughLinesP(roiCannyFive, 1, Math.PI / 180, 0, 0, 0);
            double centerBottomMatch = centerBottomLines.Count();
            matchList.Add(centerBottomMatch);
            roi_Edges_Two.ROI = System.Drawing.Rectangle.Empty;

            roi_Edges_Two.ROI = new System.Drawing.Rectangle(2 * roi_Edges_Two.Width / 3, roi_Edges_Two.Height / 2, roi_Edges_Two.Width / 3, roi_Edges_Two.Height / 2);
            System.Drawing.Rectangle rectangleSixROI = roi_Edges_Two.ROI;
            rectangleListROI.Add(rectangleSixROI);
            Mat roiForHoughSix = roi_Edges_Two.Mat;
            Mat roiCannySix = new Mat();
            CvInvoke.Canny(roiForHoughSix, roiCannySix, 80, 250);
            LineSegment2D[] rightBottomLines = CvInvoke.HoughLinesP(roiCannySix, 1, Math.PI / 180, 0, 0, 0);
            double rightBottomMatch = rightBottomLines.Count();
            matchList.Add(rightBottomMatch);
            roi_Edges_Two.ROI = System.Drawing.Rectangle.Empty;

            double fin = CompareResults(matchList, houghLinesCount);

            List<string> list = new List<string>();
            string str = "";

            foreach (double item in matchList)
            {
                list.Add(item.ToString());
            }

            foreach (string s in list)
            {
                str += s + "\n\r";
            }

            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
            System.Drawing.Rectangle redRect = new System.Drawing.Rectangle();

            int i = 0;
            while (i < matchList.Count())
            {
                redRect = rectangleListROI[i];

                if (matchList[i] == fin)
                {
                    src.Draw(redRect, new Bgr(System.Drawing.Color.Red), 3);
                    
                    CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\realTime\\realTimeMatchROI.jpg", src);
                }

                i++;
            }

          /*  while (i < matchList.Count())
            {
                redRect = rectangleListROI[i];

                if (matchList[i] == fin && srcReturn)
                {
                    srcReturn.Draw(redRect, new Bgr(System.Drawing.Color.Green), 3);

                    CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\realTime\\realTimeMatchROI.jpg", srcReturn);
                }

                i++;
            }*/

            string filePath = "C:\\Users\\Chase\\Desktop\\camoSample\\realTime\\realTimeMatchROI.jpg";
            Bitmap xyz = new Bitmap(filePath);
            return xyz;
        }



        private static double CompareResults(List<double> matchlist, double houghlinesCount)
        {
            double houghLinesCount = houghlinesCount;
            List<double> matchList = matchlist;
            List<double> results = new List<double>();

            foreach (double item in matchList)
            {
                double val = houghLinesCount - item;
                double result = houghLinesCount - val;
                result = Math.Abs(result);
                results.Add(result);
            }

            double finalResult = results.Max();

            return finalResult;
        }

        private void StopPreviewControl_Click(object sender, RoutedEventArgs e)
        {
            webCameraControl1.StopCapture();
        }

    }
}
