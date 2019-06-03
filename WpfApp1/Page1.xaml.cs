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

using ZedGraph;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System.Windows.Media.Animation;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
    Image<Bgr, Byte> imageOne = new Image<Bgr, Byte>("C:\\Users\\Chase\\Desktop\\camoSample\\testSubjectOne.jpg");

    LineSegment2D[] lines = { };
    Mat edges = new Mat();

    public Page1()
    {
        InitializeComponent();

        BitmapImage bitmapOne = new BitmapImage(new Uri("C:\\Users\\Chase\\Desktop\\camoSample\\testSubjectOne.jpg"));

        imageControlOne.Source = bitmapOne;

        //Image<Bgr, Byte> img = mat.ToImage<Bgr, Byte>();
        //Mat mat2 = img.Mat
    }

    

    private void ConvertToGrayscale_Click(object sender, RoutedEventArgs e)
    {
        Image<Gray, Byte> grayOne = imageOne.Convert<Gray, Byte>();

        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\GraySampleOne.jpg", grayOne);

        BitmapImage bitmapGrayOne = new BitmapImage(new Uri("C:\\Users\\Chase\\Desktop\\camoSample\\GraySampleOne.jpg"));

        imageControlTwo.Source = bitmapGrayOne;
    }

    private void ThresholdBinary_Click(object sender, RoutedEventArgs e)
    {

        Image<Gray, Byte> grayOne = imageOne.Convert<Gray, Byte>();

        grayOne = grayOne.ThresholdBinary(new Gray(80), new Gray(250));
        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\ThresholdBinaryOne.jpg", grayOne);
        BitmapImage thresholdBinaryOne = new BitmapImage(new Uri("C:\\Users\\Chase\\Desktop\\camoSample\\ThresholdBinaryOne.jpg"));
        imageControlThree.Source = thresholdBinaryOne;


    }




    private void Canny_Click(object sender, RoutedEventArgs e)
    {
        Image<Bgr, Byte> sourceImg = new Image<Bgr, Byte>("C:\\Users\\Chase\\Desktop\\camoSample\\testSubjectOne.jpg");
        Mat source = sourceImg.Mat;
        Mat gray = new Mat();

        CvInvoke.CvtColor(source, gray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
        CvInvoke.Canny(gray, edges, 80, 250);

        Image<Bgr, Byte> cannyEdges = edges.ToImage<Bgr, Byte>();

        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\cannyEdges.jpg", cannyEdges);

        MCvScalar m = new MCvScalar(0, 0, 255);
        lines = CvInvoke.HoughLinesP(edges, 1, Math.PI / 180, 0, 0, 0);

        Image<Bgr, Byte> lineImage = sourceImg.CopyBlank();

        foreach (LineSegment2D line in lines)
        {
            lineImage.Draw(line, new Bgr(System.Drawing.Color.Red), 1);
            sourceImg.Draw(line, new Bgr(System.Drawing.Color.Red), 1);


        }

        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\cannyHoughPattern.jpg", lineImage);
        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\originalHoughPattern.jpg", sourceImg);

        BitmapImage bitmapLineImage = new BitmapImage(new Uri("C:\\Users\\Chase\\Desktop\\camoSample\\cannyHoughPattern.jpg"));


        imageControlFour.Source = bitmapLineImage;


    }




    private void Compare_Values_Click(object sender, RoutedEventArgs e)
    {

        Image<Bgr, Byte> src = new Image<Bgr, Byte>("C:\\Users\\Chase\\Desktop\\camoSample\\testSubjectOne.jpg");
        Image<Bgr, Byte> srcTwo = new Image<Bgr, Byte>("C:\\Users\\Chase\\Desktop\\camoSample\\testSubjectOne.jpg");
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
        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\roi_Edges.jpg", roi_Edges);

        Image<Bgr, Byte> roi_Edges_Two = roiEdgesTwo.ToImage<Bgr, Byte>();
        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\roi_Edges_Two.jpg", roi_Edges_Two);

        MCvScalar m = new MCvScalar(0, 0, 255);
        lines = CvInvoke.HoughLinesP(roiEdges, 1, Math.PI / 180, 0, 0, 0);
        double houghLinesCount = lines.Count();
        Image<Bgr, Byte> srcLineImage = src.CopyBlank();


        foreach (LineSegment2D line in lines)
        {
            srcLineImage.Draw(line, new Bgr(System.Drawing.Color.Red), 1);
            src.Draw(line, new Bgr(System.Drawing.Color.Red), 1);
        }

        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\srcLineImage.jpg", srcLineImage);
        CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\src.jpg", src);

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

        //TextBox0.0 matches closest to sector containing 5857.0
        //19999.0 matches closest to sector containing 15509.0

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

        valuesList.Text = str;

        templateValues.Text = houghLinesCount.ToString();

        finalResult.Text += houghLinesCount.ToString("0.0") + " matches closest to sector containing " + fin.ToString("0.0") + "\r\n" + "\r\n";

        System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
        System.Drawing.Rectangle redRect = new System.Drawing.Rectangle();

        int i = 0;
        while (i < matchList.Count())
        {
            redRect = rectangleListROI[i];

            if (matchList[i] == fin)
            {
                src.Draw(redRect, new Bgr(System.Drawing.Color.Red), 3);
                CvInvoke.Imwrite("C:\\Users\\Chase\\Desktop\\camoSample\\matchROI.jpg", src);
                BitmapImage bitmapMatchROI = new BitmapImage(new Uri("C:\\Users\\Chase\\Desktop\\camoSample\\matchROI.jpg"));
                imageControlFour.Source = bitmapMatchROI;
            }

            i++;
        }


         /*var top = Canvas.GetTop(redRect);
         var left = Canvas.GetLeft(redRect);
         TranslateTransform trans = new TranslateTransform();
         redRect.RenderTransform = trans;
         DoubleAnimation anim1 = new DoubleAnimation(top, newY - top, TimeSpan.FromSeconds(10));
         DoubleAnimation anim2 = new DoubleAnimation(left, newX - left, TimeSpan.FromSeconds(10));
         trans.BeginAnimation(TranslateTransform.XProperty, anim1);
         trans.BeginAnimation(TranslateTransform.YProperty, anim2);
            
            */

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


    private static void UnusedCode()
    {
        //byte[] k = grayOne.GetData();
        //string dataString = BitConverter.ToString(k);
        //tbOne.Text = dataString;

        /*Mat im1 = CvInvoke.Imread(strOne);
        Mat im2 = CvInvoke.Imread(strTwo);
        Mat grayOne = new Mat();
        Mat grayTwo = new Mat();*/

        /*CvInvoke.CvtColor(im1, grayOne, Emgu.CV.CvEnum.ColorConversion.Rgb2Gray);
        CvInvoke.CvtColor(im2, grayTwo, Emgu.CV.CvEnum.ColorConversion.Rgb2Gray);
        CvInvoke.Imwrite("C:\\Users\\chase\\OneDrive\\Desktop\\camoSample\\GraySampleOne.jpg", grayOne);
        CvInvoke.Imwrite("C:\\Users\\chase\\OneDrive\\Desktop\\camoSample\\GraySampleTwo.jpg", grayTwo);*/

        //Image<Gray, Byte> picOne = grayOne.ToImage<Gray, Byte>();
        //Image<Gray, Byte> picTwo = grayTwo.ToImage<Gray, Byte>();

        //Mat grayX = new Mat();
        //CvInvoke.Imwrite("C:\\Users\\chase\\OneDrive\\Desktop\\camoSample\\GraySampleX.jpg", zzz);

        /*int[] arr = {};
        arr = grayOne.SizeOfDimemsion;
        string arrStr= string.Join("", arr);
        System.Windows.MessageBox.Show(arrStr);

        int[] arr2 = { };
        arr2 = grayTwo.SizeOfDimemsion;
        string arrStr2 = string.Join("", arr2);
        System.Windows.MessageBox.Show(arrStr2);

        Mat binaryMat();
        CvInvoke.Imshow("Output", binaryMat);

        grayThree.ROI = System.Drawing.Rectangle.Empty;
        System.Drawing.Rectangle roi = new System.Drawing.Rectangle(50, 50, 50, 50);
        grayThree.ROI = roi;

        Gray avg = grayThree.GetAverage();

        float[] GrayHist;
        DenseHistogram hist = new DenseHistogram(256, new RangeF(0.0f, 256.0f));

        hist.Calculate(new Image<Gray, Byte>[] { grayThree }, true, null);
        hist.GetData();
        Mat m = new Mat();
        GrayHist = new float[256];
        hist.MatND.ManagedArray.CopyTo(GrayHist, 0);

         System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
        MCvScalar m = new MCvScalar();
        CvInvoke.Rectangle(grayThree, rect, m, 1, LineType.EightConnected, 0);
        CvInvoke.Imwrite("C:\\Users\\chase\\OneDrive\\Desktop\\camoSample\\ThresholdBinaryThree.jpg", grayThree);*/

        /* LineSegment2D[] lines;
        using (var vector = new VectorOfPointF())
        {
            CvInvoke.HoughLines(src, vector,
                1,
                Math.PI / 180,
                100);

            var linesList = new List<LineSegment2D>();

            for (var i = 0; i < vector.Size; i++)
            {
                var rho = vector[i].X;
                var theta = vector[i].Y;
                var pt1 = new System.Drawing.Point();
                var pt2 = new System.Drawing.Point();
                var a = Math.Cos(theta);
                var b = Math.Sin(theta);
                var x0 = a * rho;
                var y0 = b * rho;
                pt1.X = (int)Math.Round(x0 + src.Width * (-b));
                pt1.Y = (int)Math.Round(y0 + src.Height * (a));
                pt2.X = (int)Math.Round(x0 - src.Width * (-b));
                pt2.Y = (int)Math.Round(y0 - src.Height * (a));
                CvInvoke.Line(src, pt1, pt2, m, 1, Emgu.CV.CvEnum.LineType.EightConnected, 8);

                linesList.Add(new LineSegment2D(pt1, pt2));
            }

            lines = linesList.ToArray();
        } */

        /*for (int i = 0; i < lines.Length; i++)
        {
            LineSegment2D l = lines[i];
            double rho = l.Length;
            double deltaY = l.P2.Y - l.P1.Y;
            double deltaX = l.P2.X - l.P1.X;
            PointF angle = l.Direction;
            double angleInDegrees = Math.Atan2(deltaY, deltaX) * Math.PI / 180;
            double theta = angleInDegrees;

            double a = Math.Cos(theta);
            double b = Math.Sin(theta);
            double x0 = a * rho;
            double y0 = b * rho;
            int x1 = (int)(x0 + 1000 * (-b));
            int y1 = (int)(y0 + 1000 * (a));
            int x2 = (int)(x0 - 1000 * (-b));
            int y2 = (int)(y0 - 1000 * (a));

            System.Drawing.Point pointOne = new System.Drawing.Point(x1, y1);
            System.Drawing.Point pointTwo = new System.Drawing.Point(x2, y2);

            CvInvoke.Line(building, pointOne, pointTwo, m, 2, Emgu.CV.CvEnum.LineType.EightConnected);

        }*/


        /*foreach (LineSegment2D line in lines)
        { 
            double rho = line.Length;
            PointF theta = line.Direction;
            double a = Math.Cos(theta);
            double x0 = rho;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            LineSegment2D v = lines[i];
            int x1 = v.P1.X;
            int y1 = v.P1.Y;
            System.Drawing.Point p1 = new System.Drawing.Point(x1, y1);
            int x2 = v.P2.X;
            int y2 = v.P2.Y;
            System.Drawing.Point p2 = new System.Drawing.Point(x1, y1);
            List<System.Drawing.Point> pts = new List<System.Drawing.Point>();
            pts.Add(p1);
            pts.Add(p2);

            for (int j = 0; j < pts.Count; j++)
            {
                CvInvoke.Line(building, p1, p2, m, 3, Emgu.CV.CvEnum.LineType.EightConnected);
            }
        }*/
    }

    }
    
}
