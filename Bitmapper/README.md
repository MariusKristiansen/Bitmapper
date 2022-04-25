# Bitmapper 
The bitmapper class is used to translate byte arrays into bitmap files the simulation can assemble into .mp4 files.  
### Constructor 
```cs         
public BitmapGenerator(int dimention, bool greyscale=false)

public BitmapGenerator(int dimention, string filename, bool greyscale = false)

public BitmapGenerator(int xSize, int ySize, string filename, bool greyscale = false)

```


## Methods
Publicly accessible methods  
### InsertPixel
```cs
public void InsertPixel(int x, int y, byte value)
```
```cs
public void InsertPixel(int x, int y, byte r, byte g, byte b, byte alpha)
```
Used to insert a byte value into the given x, y address in the final bitmap image.  

### Save
```cs
public void Save()
```
To save the loaded image, use the `Save()` method. This will write the byte array to a bmp file.  

### FileStream
```cs
public static void FileStream(string filename, string outputDir, string prefix = "img", int xDim = 10, int resolution = 1000, bool greyscale = true)
```
The class can also generate a set of files that can be assembled into an animation. The method takes in a .csv file given by the filename parameter. The file must be comma seperated byte values where each line is translated into one image.  Each file will automatically be named `{prefix}%0d.png` in order, starting with index 0 (prefix00000000.png).    