
//#r "nuget:OpenCvSharp4.Windows"

#r "nuget:OpenCvSharp4"
#r "nuget:OpenCvSharp4.runtime.ubuntu.18.04-x64"
 
open OpenCvSharp
open System

let test () =
   use src = new Mat(__SOURCE_DIRECTORY__ + "/lenna.png", ImreadModes.Grayscale)
   use dst = new Mat()
           
   Cv2.Canny(InputArray.Create(src), OutputArray.Create(dst), 50.0, 200.0);
   using(new Window("src image", src)) 
   using (new Window("dst image", dst)) 
   Cv2.WaitKey()

test()