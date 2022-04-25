using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitmapper
{
    public static class BitmapTest
    {
        public static BitmapGenerator bmp;

        public static void Run()
        {
            //test();
            //OneImage();
            //ParallelMandelbrot(1000, 1000, 16, 0.281717921930775, 0.5771052841488505, 1.78E-9);
            ParallelMandelbrot(10000, 1000, 4, 0,0,1.5);
            //ParallelJulia(1000, 0.2755, 0.007, 500, 0, 0, 0.0001);
            //Julia(1000, 0.2755, 0.007, -1.5,1.5,-1.5,1.5);
            //Sequence();
        }

        private static void Sequence()
        {
            double r = -0.7150;
            double increment = 0.001;
            double im = 0.1889;
            for (int i = 0; i < 200; i++)
            {
                r -= increment;
                Julia(1000, r, im, -1.5, 1.5, -1.5, 1.5, $"sequence/img{i:d8}");
            }
            System.Diagnostics.Process.Start("CMD.exe", "/C ffmpeg -framerate 24 -i sequence/img%08d.png out.mp4");
        }

        private static void ParallelMandelbrot(int range, int numIterations = 100, int limit = 4, double? xCenter= null, double? yCenter = null, double? R = null, string filename = "pMandlebrot")
        {
            double xstart;// 0.5; //-2
            double xend;// 0.2; //1
            double ystart;// 0.1; // -1.5
            double yend;
            if (xCenter is null || yCenter is null || R is null)
            {
                xstart = -2;// 0.5; //-2
                xend = 1;// 0.2; //1
                ystart = -1.5;// 0.1; // -1.5
                yend = 1.5;// .4; // 1.5
            } else
            {
                xstart = (double)xCenter - (double)R;
                ystart = (double)yCenter - (double)R;
                xend = (double)xCenter + (double)R;
                yend = (double)yCenter + (double)R;
                filename += $"[X{xCenter.ToString().Replace(".", ",")}_Y{yCenter.ToString().Replace(".", ",")}]R{R.ToString().Replace(".", ",")}";
            }

            Console.WriteLine($"Calculating Mandelbrot set at x: {xCenter} y:{yCenter} Radius: {R} Iterations: {numIterations}\nOutput resolution {range}x{range} Output filename: {filename}.png\nPlease wait...");
            var irange = Enumerable.Range(0, range).ToArray();
            var indexes = irange.Select(i => (i, Enumerable.Range(0, range)));
            var i = irange.Select(i => xstart + (xend - xstart) * ((double)i / (irange.Length - 1))).ToArray();
            var j = irange.Select(i => ystart + (yend - ystart) * ((double)i / (irange.Length - 1))).ToArray();
            var xLength = i.Count();
            var yLength = j.Count();
            var bmp = new BitmapGenerator(xLength, filename, false);//new BitmapGenerator(xLength, yLength, "mandelbrot");
            var values = new (int, int, double, double)[xLength * yLength];
            byte[] imgData = new byte[xLength * yLength];
            var ind = 0;
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    values[ind] = (x, y, i[x], j[y]);
                    ind++;
                }
            }
            
            Parallel.ForEach(values, i => {
                
                var val = mandel(i.Item3, i.Item4, numIterations, limit);
                bmp.InsertPixel(i.Item1, i.Item2, val);
            });
            Console.WriteLine("Calculation complete, saving image...");
            bmp.Save();
        }

        private static byte mandel(double r, double i, int maxIterations, int limit)
        {
            double x0, y0, x, y, xlast, ylast;

            var n = 0;
            x0 = r;
            y0 = i;
            x = x0;
            y = y0;
            xlast = 0;
            ylast = 0;
            while (n < maxIterations && x * x + y * y <= limit)
            {
                x = (xlast * xlast) - (ylast * ylast) + x0;
                y = 2 * xlast * ylast + y0;

                xlast = x;
                ylast = y;
                n = n + 1;
            }
            return  (byte)Math.Ceiling((double)n / maxIterations * 255);
        }

        private static void ParallelJulia(int range, double r, double imag, int numIterations = 100, int limit = 4, double? xCenter = null, double? yCenter = null, double? R = null, string filename="pJulia")
        {
            {

                double xstart;
                double xend;
                double ystart;
                double yend;
                if (xCenter is null || yCenter is null || R is null)
                {
                    xstart = -1.5;
                    xend = 1.5;
                    ystart = -1.5;
                    yend = 1.5;
                }
                else
                {
                    xstart = (double)xCenter - (double)R;
                    ystart = (double)yCenter - (double)R;
                    xend = (double)xCenter + (double)R;
                    yend = (double)yCenter + (double)R;
                    filename += $"[X{xCenter.ToString().Replace(".", ",")}_Y{yCenter.ToString().Replace(".", ",")}]R{R.ToString().Replace(".", ",")}";
                }

                Console.WriteLine($"Calculating Julia set at x: {xCenter} y:{yCenter} Radius: {R} Iterations: {numIterations}\nOutput resolution {range}x{range} Output filename: {filename}.png\nPlease wait...");
                var irange = Enumerable.Range(0, range).ToArray();
                var indexes = irange.Select(i => (i, Enumerable.Range(0, range)));
                var i = irange.Select(i => xstart + (xend - xstart) * ((double)i / (irange.Length - 1))).ToArray();
                var j = irange.Select(i => ystart + (yend - ystart) * ((double)i / (irange.Length - 1))).ToArray();
                var xLength = i.Count();
                var yLength = j.Count();
                var bmp = new BitmapGenerator(xLength, filename, false);//new BitmapGenerator(xLength, yLength, "mandelbrot");
                var values = new (int, int, double, double)[xLength * yLength];
                byte[] imgData = new byte[xLength * yLength];
                var ind = 0;
                for (int x = 0; x < xLength; x++)
                {
                    for (int y = 0; y < yLength; y++)
                    {
                        values[ind] = (x, y, i[x], j[y]);
                        ind++;
                    }
                }
                

                Parallel.ForEach(values, i => {

                    var val = juli(i.Item3, i.Item4, r, imag, numIterations, limit);
                    bmp.InsertPixel(i.Item1, i.Item2, val);
                });
                Console.WriteLine("Calculation complete, saving image...");
                bmp.Save();
            }
        }


        private static byte juli(double r, double i, double real, double imag, int maxIterations, int limit)
        {
            var n = 0;
            var x = r;
            var y = i;
            double xTemp = 0;
            while (n < maxIterations && x * x + y * y <= limit)
            {
                xTemp = x * x - y * y;
                y = 2 * x * y + imag;
                x = xTemp + real;
                n++;
            }

            //byte value = (n > (maxIterations*0.95)) ? (byte)Math.Ceiling((double)n / maxIterations * 255) : (byte)0;
            return (byte)Math.Ceiling((double)n / maxIterations * 255);
        }

        private static void Mandelbrot(int range, string filename = "mandelbrot2")
        {
            var xstart = -2;// 0.5; //-2
            var xend = 1;// 0.2; //1
            var ystart = -1.5;// 0.1; // -1.5
            var yend = 1.5;// .4; // 1.5
            var irange = Enumerable.Range(0, range).ToArray();
            var i = irange.Select(i => xstart + (xend - xstart) * ((double)i / (irange.Length - 1)));
            var j = irange.Select(i => ystart + (yend - ystart) * ((double)i / (irange.Length - 1)));
            var xLength = i.Count();
            var yLength = j.Count();

            var bmp = new BitmapGenerator(xLength, filename, false);//new BitmapGenerator(xLength, yLength, "mandelbrot");

            byte[] imgData = new byte[xLength * yLength];

            var maxIterations = 500;
            int xpos = 0;
            int ypos = 0;
            double x0, y0, x, y, xlast, ylast;

            foreach (var yVal in j)
            {
                xpos = 0;
                foreach (var xVal in i)
                {
                    var n = 0;
                    x0 = xVal;
                    y0 = yVal;
                    x = x0;
                    y = y0;
                    xlast = 0;
                    ylast = 0;
                    while (n < maxIterations && x * x + y * y <= 4)
                    {
                        x = (xlast * xlast) - (ylast * ylast) + x0;
                        y = 2 * xlast * ylast + y0;

                        xlast = x;
                        ylast = y;
                        n = n + 1;
                    }
                    byte value = (byte)Math.Ceiling((double)n / maxIterations * 255);
                    bmp.InsertPixel(xpos, ypos, value);
                    xpos++;
                }
                ypos++;
            }
            bmp.Save();
        }

        private static void Julia(int range, double r, double imag, double xstart = -1.5, double xend = 1.5, double ystart = -1.5, double yend = 1.5, string filename = "julia")
        {
            var irange = Enumerable.Range(0, range).ToArray();
            var i = irange.Select(i => xstart + (xend - xstart) * ((double)i / (irange.Length - 1)));
            var j = irange.Select(i => ystart + (yend - ystart) * ((double)i / (irange.Length - 1)));
            var xLength = i.Count();
            var yLength = j.Count();

            var bmp = new BitmapGenerator(xLength, filename, false);//new BitmapGenerator(xLength, yLength, "mandelbrot");

            byte[] imgData = new byte[xLength * yLength];

            var maxIterations = 200;
            int xpos = 0;
            int ypos = 0;
            double x, y;

            foreach (var yVal in j)
            {
                xpos = 0;
                foreach (var xVal in i)
                {
                    var n = 0;
                    x = xVal;
                    y = yVal;
                    double xTemp = 0;
                    while (n < maxIterations && x * x + y * y <= 16)
                    {
                        xTemp = x * x - y * y;
                        y = 2 * x * y + imag;
                        x = xTemp + r;
                        n++;
                    }

                    //byte value = (n > (maxIterations*0.95)) ? (byte)Math.Ceiling((double)n / maxIterations * 255) : (byte)0;
                    byte value = (byte)Math.Ceiling((double)n / maxIterations * 255);
                    bmp.InsertPixel(xpos, ypos, value);


                    xpos++;
                }
                ypos++;
            }
            bmp.Save();
        }
    }
}
