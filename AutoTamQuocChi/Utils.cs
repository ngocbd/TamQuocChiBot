using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTamQuocChi
{
    class Utils
    {
        private static Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
        public static void ClearText()
        {
            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();
            for (var i = 0; i < 20; i++)
            {
                AdbClient.Instance.ExecuteRemoteCommand("input keyevent KEYCODE_DEL", device, receiver);
            }
            return;
        }
        public static ulong getHashFromSample(string name)
        {

            string path = Path.Combine(Environment.CurrentDirectory, @"sample\", name);
            var hash = ImageHashing.ImageHashing.AverageHash(Image.FromFile(path));
            return hash;
        }
        public static bool Compare(Image img, string filename)
        {
            var hash1 = ImageHashing.ImageHashing.AverageHash(img);
            var hash2 = getHashFromSample(filename);
            Console.WriteLine("ImageObject :" + hash1);
            Console.WriteLine("Hashing :" + filename + " : " + hash2);
            string path = Path.Combine(Environment.CurrentDirectory, @"sample\", filename);
            img.Save(Path.Combine(Environment.CurrentDirectory, @"", "cropped_" + filename));
            var score = ImageHashing.ImageHashing.Similarity(img, Image.FromFile(path));
            Console.WriteLine("Similarity score :" + score);
            return score > 80;

        }
        public static bool CompareAt(DeviceData device, string filename, Rectangle rect)
        {
            Image screen = AdbClient.Instance.GetFrameBufferAsync(device, CancellationToken.None).Result;
            Image cropped = CropImage(screen, rect);
            return Compare(cropped, filename);

        }
        static void Main2(string[] args)
        {
            Console.WriteLine("Bot Started ..");


            ConsoleOutputReceiver receiver = new ConsoleOutputReceiver();
            var device = AdbClient.Instance.GetDevices().First();






            if (CompareAt(device, "firstLoginButton.png", new Rectangle(250, 1135, 217, 75)))
            {
                Console.WriteLine("In FirstLogin Screen");
                AdbClient.Instance.ExecuteRemoteCommand("input tap 350 1156", device, receiver);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 300 500", device, receiver);
                ClearText();

                AdbClient.Instance.ExecuteRemoteCommand("input text \"agrukuki\"", device, receiver);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 300 560", device, receiver);
                ClearText();
                Console.WriteLine("test");
                Thread.Sleep(1000);
                AdbClient.Instance.ExecuteRemoteCommand("input text abcD1234", device, receiver);
                Thread.Sleep(1000);
                AdbClient.Instance.ExecuteRemoteCommand("input keyevent 66", device, receiver);
                Thread.Sleep(1000);
                AdbClient.Instance.ExecuteRemoteCommand("input keyevent 66", device, receiver);
                Thread.Sleep(1000);
                AdbClient.Instance.ExecuteRemoteCommand("input keyevent 66", device, receiver);

                AdbClient.Instance.ExecuteRemoteCommand("input keyevent 66", device, receiver);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 360 650", device, receiver);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 350 1156", device, receiver);
                Thread.Sleep(7000);
                AdbClient.Instance.ExecuteRemoteCommand("input keyevent 4", device, receiver);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 100 1200", device, receiver);
                Thread.Sleep(5000);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 200 270", device, receiver);
                Thread.Sleep(3000);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 500 700", device, receiver);
                Thread.Sleep(3000);
                for (var i = 0; i < 25; i++)
                {
                    AdbClient.Instance.ExecuteRemoteCommand("input tap 130 1025", device, receiver);
                }
                for (var i = 0; i < 3; i++)
                {
                    AdbClient.Instance.ExecuteRemoteCommand("input tap 520 1025", device, receiver);
                }
                Thread.Sleep(5000);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 520 1200", device, receiver);
                Thread.Sleep(3000);
                AdbClient.Instance.ExecuteRemoteCommand("input tap 360 900", device, receiver);

            }
            else
            {
                Console.WriteLine("NotIn FirstLogin Screen");
            }

            Console.ReadKey();

        }
    }

}
