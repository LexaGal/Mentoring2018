using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace MentoringTasks
{
    public class PageConverter
    {
        public PageConverter()
        {
            var o = new ChromeOptions();
            o.AddArguments("disable-extensions");
            o.AddArguments("--start-maximized");
            _driver = new ChromeDriver(o);
        }

        private IWebDriver _driver;

        public MemoryStream GetPdf(string url)
        {
            return ConvertImageToPdf(GetEntireScreenshot(url));
        }

        public MemoryStream ConvertImageToPdf(MemoryStream inStream)
        {
            iTextSharp.text.Rectangle pageSize;
            using (var image = new Bitmap(inStream))
            {
                pageSize = new iTextSharp.text.Rectangle(0, 0, image.Width, image.Height);
            }
            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                document.Open();
                var img = iTextSharp.text.Image.GetInstance(inStream.ToArray());
                document.Add(img);
                document.Close();
                var outStream = new MemoryStream(ms.ToArray());
                return outStream;
            }
        }

        private MemoryStream GetEntireScreenshot(string url)
        {
            _driver.Manage().Window.Maximize();
            _driver.Navigate().GoToUrl(url);

            // Get the total size of the page
            var totalWidth =
                 (int)(long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.offsetWidth");
            var totalHeight =
                (int)(long)((IJavaScriptExecutor)_driver).ExecuteScript("return  document.body.parentNode.scrollHeight");
            var viewportWidth =
                (int)(long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.clientWidth");
            var viewportHeight =
                (int)(long)((IJavaScriptExecutor)_driver).ExecuteScript("return window.innerHeight");

            // We only care about taking multiple images together if it doesn't already fit
            if (totalWidth <= viewportWidth && totalHeight <= viewportHeight)
            {
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                var ms = new MemoryStream();
                ScreenshotToImage(screenshot).Save(ms, ImageFormat.Png);
                return ms;
            }
            // Split the screen in multiple Rectangles
            var rectangles = new List<Rectangle>();
            // Loop until the totalHeight is reached
            for (var y = 0; y < totalHeight; y += viewportHeight)
            {
                var newHeight = viewportHeight;
                // Fix if the height of the element is too big
                if (y + viewportHeight > totalHeight)
                {
                    newHeight = totalHeight - y;
                }
                // Loop until the totalWidth is reached
                for (var x = 0; x < totalWidth; x += viewportWidth)
                {
                    var newWidth = viewportWidth;
                    // Fix if the Width of the Element is too big
                    if (x + viewportWidth > totalWidth)
                    {
                        newWidth = totalWidth - x;
                    }
                    // Create and add the Rectangle
                    var currRect = new Rectangle(x, y, newWidth, newHeight);
                    rectangles.Add(currRect);
                }
            }
            // Build the Image
            var stitchedImage = new Bitmap(totalWidth, totalHeight);
            // Get all Screenshots and stitch them together
            var previous = Rectangle.Empty;
            foreach (var rectangle in rectangles)
            {
                // Calculate the scrolling (if needed)
                if (previous != Rectangle.Empty)
                {
                    var xDiff = rectangle.Right - previous.Right;
                    var yDiff = rectangle.Bottom - previous.Bottom;
                    // Scroll
                    ((IJavaScriptExecutor)_driver).ExecuteScript($"window.scrollBy({xDiff}, {yDiff})");
                }
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                // Build an Image out of the Screenshot
                var screenshotImage = ScreenshotToImage(screenshot);
                // Calculate the source Rectangle
                var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height,
                    rectangle.Width, rectangle.Height);
                // Copy the Image
                using (var graphics = Graphics.FromImage(stitchedImage))
                {
                    graphics.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                }
                // Set the Previous Rectangle
                previous = rectangle;
            }
            var stream = new MemoryStream();
            stitchedImage.Save(stream, ImageFormat.Png);
            return stream;
        }

        private static Image ScreenshotToImage(Screenshot screenshot)
        {
            Image screenshotImage;
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                screenshotImage = Image.FromStream(memStream);
            }
            return screenshotImage;
        }
    }
}