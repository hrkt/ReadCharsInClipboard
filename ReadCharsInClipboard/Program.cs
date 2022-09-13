using System;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using System.Drawing.Imaging;
using System.Windows.Forms;

/// <summary>
/// A console application that does OCR when the image is in the clipboard. 
///  Its output is copied into the clipboard in text form.
/// </summary>

namespace ReadCharsInClipboard
{
    /*
     * Console application
     */
    internal class Program
    {

        /*
         * get image from clipboard.
         * It returns null if the image does not exist ( or top of the history) of clipboard.
         */
        static SoftwareBitmap GetImageFromClipboard()
        {
            if (!Clipboard.ContainsImage())
            {
                return null;
            }

            System.Drawing.Image image = Clipboard.GetImage();
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Bmp);
            var decorder = BitmapDecoder.CreateAsync(ms.AsRandomAccessStream());
            decorder.AsTask().Wait();

            BitmapDecoder bitmapDecoder = decorder.GetResults();
            Windows.Foundation.IAsyncOperation<SoftwareBitmap> asyncOperation2 = bitmapDecoder.GetSoftwareBitmapAsync();
            asyncOperation2.AsTask().Wait();

            SoftwareBitmap bitmap = asyncOperation2.GetResults();

            return bitmap;
        }

        /*
         * It reads chars in the clipboard by OCR and returns text.
         */
        static String tryReadChars(SoftwareBitmap bitmap)
        {
            // execute OCR
            // TODO: It might be better to change the language setting by OS settings.
            OcrEngine engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("ja-JP"));
            var result = engine.RecognizeAsync(bitmap);
            result.AsTask().Wait();

            OcrResult ocrResult = result.GetResults();
            return ocrResult.Text;
        }

        /*
         * Entry point
         */
        [STAThread]
        static void Main(string[] args)
        {
            SoftwareBitmap bitmap = GetImageFromClipboard();
            if (null != bitmap)
            {
                var txt = tryReadChars(bitmap);
                Clipboard.SetText(txt);
            }
        }
    }
}
