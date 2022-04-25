using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bitmapper
{
    public class BitmapGenerator
    {
        public byte[] BMP { get; set; }
        private int dim;
        private int xDim;
        private int yDim;
        public int Dim
        {
            get
            {
                return dim;
            }

            set
            {
                if (value < 0)
                {
                    Console.WriteLine("DIM MUST BE POSITIVE, setting to abs(dim)");
                    dim = Math.Abs(value);
                } else if (value == 0)
                {
                    throw new Exception("dim argument cannot be Zero!");
                } else
                {
                    dim = value;
                }
            }
        }
        public int XDim
        {
            get
            {
                return xDim;
            }

            set
            {
                if (value < 0)
                {
                    Console.WriteLine("DIM MUST BE POSITIVE, setting to abs(dim)");
                    xDim = Math.Abs(value);
                }
                else if (value == 0)
                {
                    throw new Exception("dim argument cannot be Zero!");
                }
                else
                {
                    xDim = value;
                }
            }
        }
        public int YDim
        {
            get
            {
                return yDim;
            }

            set
            {
                if (value < 0)
                {
                    Console.WriteLine("DIM MUST BE POSITIVE, setting to abs(dim)");
                    yDim = Math.Abs(value);
                }
                else if (value == 0)
                {
                    throw new Exception("dim argument cannot be Zero!");
                }
                else
                {
                    yDim = value;
                }
            }
        }
        public int Stride { get; set; } = 4;
        public string Filename { get; set; } = "image.png";
        public string Extention { get; set; } = ".png";
        private ulong size;
        public bool Greyscale { get; set; } = false;
        public bool Square { get; set; } = true;
        private static int imgIndex = 0;

        public BitmapGenerator(int dimention, string filename, bool greyscale = false)
        {
            ControlFilename(filename);
            Dim = dimention;
            XDim = dimention;
            YDim = dimention;
            Greyscale = greyscale;
            if (Greyscale)
            {
                size = ((ulong)Dim) * (ulong)Dim;
            }
            else
            {
                size = ((ulong)Stride * (ulong)Dim) * (ulong)Dim;
            }
            BMP = new byte[size];
        }

        public BitmapGenerator(int xSize, int ySize, string filename, bool greyscale = false)
        {
            ControlFilename(filename);
            XDim = xSize;
            YDim = ySize;
            Square = false;
            Greyscale = greyscale;
            if (Greyscale)
            {
                size = ((ulong)XDim) * (ulong)YDim;
            }
            else
            {
                size = ((ulong)Stride * (ulong)XDim) * (ulong)YDim;
            }
            
            BMP = new byte[size];
        }

        public BitmapGenerator(int dimention, bool greyscale=false)
        {
            Dim = dimention;
            XDim = dimention;
            YDim = dimention;
            Greyscale = greyscale;
            if (Greyscale)
            {
                size = ((ulong)Dim) * (ulong)Dim;
            } else
            {
                size = ((ulong)Stride * (ulong)Dim) * (ulong)Dim;
            }
            BMP = new byte[size];

        }


        private void ControlFilename(string filename)
        {
            // Checks to see if the filename already contains the filetype
            if (filename.Contains(".bmp"))
            {
                Filename = filename.Trim(); 
            } else
            {
                var name = filename.Trim();
                var index = name.IndexOf('.');
                if (index > 0) name = name.Substring(0, index);
                Filename = $"{name}{Extention}";
            }
        }

        public void InsertPixel(int x, int y, byte value)
        {
            var offset = (Dim * Stride) * y + (x * 4);
            var rAddress = offset;
            var gAddress = offset + 1;
            var bAddress = offset + 2;
            var alphaAddress = offset + 3;
            BMP[rAddress] = value;
            BMP[gAddress] = value;
            BMP[bAddress] = value;
            BMP[alphaAddress] = 255;
        }

        public void InsertGreyscalePixel(int x, int y, byte value)
        {
            var offset = Dim * y + x ;
            BMP[offset] = value;
        }

        public void InsertPixel(int x, int y, byte r, byte g, byte b, byte alpha)
        {
            var offset = (Dim * Stride) * y + (x * 4);
            if (offset + 3 > BMP.Length || offset < -3) return;
            if (x >= XDim || x < 0 || y >= YDim || y < 0) return;
            var rAddress = offset;
            var gAddress = offset + 1;
            var bAddress = offset + 2;
            var alphaAddress = offset + 3;
            BMP[rAddress] = b;
            BMP[gAddress] = g;
            BMP[bAddress] = r;
            BMP[alphaAddress] = alpha;
        }

        public static void WideFileStream(string filename, string outputDir, int xScale, int yScale, int xResolution, int yResolution, string prefix = "img")
        {
            // implement later... Functionality to combine two maps into one double wide image file. 
            Console.WriteLine($"Generating image set\nInput file: {filename}\nOutput folder: {outputDir}\nFile pattern: {prefix}dddddddd.png");
            var outputPath = $"{outputDir}{prefix}";
            using (var reader = new StreamReader(filename))
            {
                reader.ReadLine();
                var i = 0;
                while (!reader.EndOfStream)
                {
                    var bitmap = new BitmapGenerator(xResolution, yResolution, $"{outputPath}{i:D8}");
                    var line = reader.ReadLine();
                    var values = line.Split(',').ToList();
                    var sublist = values.GetRange(1, values.Count - 1);
                    var numlist = sublist.Select(i => int.Parse(i)).ToList();
                    if (numlist.Count() <= 0 || numlist is null) break;
                    var max = numlist.Max();
                    var min = numlist.Min();
                    var scaled = numlist.Select(i => (byte)Math.Round((255 * i) / (double)(max - min))).ToArray();
                    var xfactor = xResolution / xScale;
                    var yfactor = yResolution / yScale;
                    for (int y = 0; y < yResolution; y++)
                    {
                        for (int x = 0; x < xResolution; x++)
                        {
                            byte val = scaled[x / xfactor * xScale + y / yfactor];
                            bitmap.InsertPixel(x, y, val);
                        }
                    }
                    bitmap.Save();
                    i++;
                }
            }
        }

        public static void ParallelFileStream(string filename, string outputDir, string prefix = "img", int xDim = 10, int resolution = 1000)
        {
            var watch = Stopwatch.StartNew();
            Console.WriteLine($"Generating image set\nInput file: {filename}\nOutput folder: {outputDir}\nFile pattern: {prefix}dddddddd.png");
            var outputPath = $"{outputDir}{prefix}";
            var lines = File.ReadAllLines(filename).ToList();
            var numbers = (from line in lines
                           let splitline = line.Split(',').ToList()
                           from entry in splitline.GetRange(1, splitline.Count() - 1)
                           select int.Parse(entry)).ToList();
            var globalMax = numbers.Max();
            Console.WriteLine($"\n\n\n\nGLOBAL MAX: {globalMax}\n\n\n");

            imgIndex = 0; // Will I be out of scope??

            Parallel.ForEach(lines, line => {
                if (line is null || line.Length < 2)
                {

                } else
                {
                    var bitmap = new BitmapGenerator(resolution, $"{outputPath}{imgIndex:D8}");
                    var values = line.Split(',').ToList();
                    var sublist = values.GetRange(1, values.Count - 1);
                    var numlist = sublist.Select(i => int.Parse(i)).ToList();
                    //if (numlist.Count() <= 0 || numlist is null) break;
                    var max = globalMax;
                    var min = numlist.Min();
                    var scaled = numlist.Select(i => (byte)Math.Round((255 * i) / (double)(max - min))).ToArray();
                    var factor = resolution / xDim;
                    for (int y = 0; y < resolution; y++)
                    {
                        for (int x = 0; x < resolution; x++)
                        {
                            byte val = scaled[x / factor * xDim + y / factor];
                            bitmap.InsertPixel(y, x, val);
                        }
                    }
                    bitmap.ParallelSave();
                    imgIndex++;
                }
                
            });
            watch.Stop();
            Console.WriteLine($"Operation took: {watch.ElapsedMilliseconds/1000.0:f4}Seconds");
            
        }

        public static void FileStream(string filename, string outputDir, string prefix = "img", int xDim = 10, int resolution = 1000, bool greyscale = true)
        {
            var watch = Stopwatch.StartNew();
            Console.WriteLine($"==>\nGenerating image set\nInput file: {filename}\nOutput folder: {outputDir}\nFile pattern: {prefix}dddddddd.png");
            var outputPath = $"{outputDir}{prefix}";
            var lines = File.ReadAllLines(filename).ToList();
            var numbers = (from line in lines
                           let splitline = line.Split(',').ToList()
                           from entry in splitline.GetRange(1, splitline.Count() - 1)
                           select int.Parse(entry)).ToList();
            var globalMax = numbers.Max();
            //Console.WriteLine($"\n\n\n\nGLOBAL MAX: {globalMax}\n\n\n");
            using (var reader = new StreamReader(filename))
            {
                //reader.ReadLine();
                var i = 0;
                
                
                while (!reader.EndOfStream)
                {
                    var bitmap = new BitmapGenerator(resolution, $"{outputPath}{i:D8}");
                    var line = reader.ReadLine();
                    var values = line.Split(',').ToList();
                    var sublist = values.GetRange(1, values.Count - 1);
                    var numlist = sublist.Select(i => int.Parse(i)).ToList();
                    if (numlist.Count() <= 0 || numlist is null) break;
                    //var max = numlist.Max();
                    var max = globalMax;
                    var min = numlist.Min();
                    var scaled = numlist.Select(i => (byte)Math.Round((255 * i) / (double)(max - min))).ToArray();
                    var factor = resolution / xDim;
                    for (int y = 0; y < resolution; y++)
                    {
                        for (int x = 0; x < resolution; x++)
                        {
                            byte val = scaled[x / factor * xDim + y / factor];
                            if (greyscale)
                            {
                                bitmap.InsertPixel(y, x, val);
                            } else
                            {
                                var iVal = val * 2;
                                int red;
                                int green;
                                if (iVal == 0)
                                {
                                    red = 0;
                                    green = 0;
                                }else if (iVal < 256)
                                {
                                    red = 255;
                                    green = val;
                                }
                                else
                                {
                                    red = 255 - iVal % 256;
                                    green = 255;
                                }
                                bitmap.InsertPixel(y, x, (byte)red, (byte)green, 0, 255);
                            }
                        }
                    }
                    bitmap.Save();
                    i++;
                }
            }
            watch.Stop();
            Console.WriteLine($"Operation took: {watch.ElapsedMilliseconds / 1000.0:f2}Seconds");
        }

        public void Save()
        {
            // Write the byte array to file. 
            unsafe
            {
                fixed  (byte* ptr = BMP)
                {
                    using (Bitmap image = new Bitmap(Dim, Dim, Dim * Stride, PixelFormat.Format32bppRgb, new IntPtr(ptr)))//(TextWriter sw = new StreamWriter(Filename))
                    {
                        image.Save(Filename);
                    }
                }
            }
            
        }

        public void ParallelSave()
        {
            using (Bitmap image = new Bitmap(Dim, Dim)){
                for (int y = 0; y < Dim; y++)
                {
                    for (int x = 0; x < Dim; x++)
                    {
                        image.SetPixel(x, y, Color.FromArgb(255,BMP[x * Dim + y * Dim], BMP[x * Dim + y * Dim], BMP[x * Dim + y * Dim]));
                    }
                }
                image.Save(Filename);
            }
        }

        public void SaveGrayscale()
        {
            // Write the byte array to file. 
            if (!Greyscale) throw new Exception("MAPPER IS NOT IN GRAYSCALE MODE!");
            using (Bitmap image = new Bitmap(Dim, Dim, PixelFormat.Format16bppGrayScale))//(TextWriter sw = new StreamWriter(Filename))
            {
                for (int i = 0; i < Dim; i++)
                {
                    for (int j = 0; j < Dim; j++)
                    {
                        image.SetPixel(i, j, Color.FromArgb(BMP[j * Dim + i], 0, 0));
                    }
                }
                image.Save(Filename);
            }

        }

        public void SaveNotSquare()
        {
            // Write the byte array to file. 
            if (Square) throw new Exception("MAPPER IS IN SQUARE MODE!");
            unsafe
            {
                fixed (byte* ptr = BMP)
                {
                    using (Bitmap image = new Bitmap(XDim, YDim, XDim * Stride, PixelFormat.Format32bppRgb, new IntPtr(ptr)))//(TextWriter sw = new StreamWriter(Filename))
                    {
                        image.Save(Filename);
                    }
                }
            }

        }
    }
}
