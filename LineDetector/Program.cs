using System;
using System.IO;
using OpenCvSharp;

namespace LineDetector
{
    class Program
    {
        private const string INPUT_PATH = @"C:\Users\Vitalie\Desktop\Optar\Input";
        private const string EDGEDETECTION_PATH = @"C:\Users\Vitalie\Desktop\Optar\EdgeDetection";
        private const string LINEDETECTION_PATH = @"C:\Users\Vitalie\Desktop\Optar\LineDetection";

        static void Main(string[] args)
        {
            string[] fileArray = Directory.GetFiles(INPUT_PATH);
            foreach (var fullPath in fileArray)
            {
                string fileName = Path.GetFileName(fullPath);
                // LineEdgeDetection(fileName);
                EdgeDetection(fileName);
            }
        }

        public static void LineEdgeDetection(string fileName)
        {
            Mat img = new Mat(Path.Combine(INPUT_PATH, fileName), ImreadModes.AnyColor);
            Mat gray = new Mat();

            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            int kernel_size = 5;

            Mat blur_gray = new Mat();
            Cv2.GaussianBlur(gray, blur_gray, new Size { Height = kernel_size, Width = kernel_size }, 0);

            int low_treshold = 150;
            int high_threshold = 200;

            Mat edges = new Mat();
            Cv2.Canny(blur_gray, edges, low_treshold, high_threshold);

            double rho = 1; // distance resolution in pixels of the Hough grid
            double theta = Math.PI / 180; //angular resolution in radians of the Hough grid
            int threshold = 15; // minimum number of votes (intersections in Hough grid cell)
            int min_line_length = 50; // minimum number of pixels making up a line
            int max_line_gap = 13; // maximum gap in pixels between connectable line segments

            // creating a blank to draw lines on
            Mat line_image = new Mat(rows: img.Rows, cols: img.Cols, type: img.Type());

            // Run Hough on edge detected image
            // Output "lines" is an array containing endpoints of detected line segments
            var lines = Cv2.HoughLinesP(edges, rho, theta, threshold, min_line_length, max_line_gap);

            foreach (LineSegmentPoint line in lines)
            {
                Console.WriteLine($"{line.P1}, {line.P2}");
                Cv2.Line(line_image, line.P1, line.P2, Scalar.Red, 2, LineTypes.Link8);
            }

            Mat line_edges = new Mat();

            Cv2.AddWeighted(img, 0.8, line_image, 1, 0, line_edges);

            Cv2.ImWrite(Path.Combine(LINEDETECTION_PATH, fileName), line_edges);
        }

        public static void EdgeDetection(string fileName)
        {
            Mat src = new Mat(Path.Combine(INPUT_PATH, fileName), ImreadModes.AnyColor);
            Mat dst = new Mat();

            Cv2.Canny(src, dst, 500, 600);

            Cv2.ImWrite(Path.Combine(EDGEDETECTION_PATH, fileName), dst);
        }
    }
}
