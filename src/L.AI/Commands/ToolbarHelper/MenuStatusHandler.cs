using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using Microsoft.VisualStudio.Shell;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_AI.Commands.ToolbarHelper
{
    public static class MenuStatusHandler
    {
        private const string redBase64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsIAAA7CARUoSoAAAAGdSURBVDhPlVPdSgJBFB5nx59dla0sUCG1ArvYmy4Cb7qJiKKbLnqGeoKgZwh6gqiHKCjrBQKhC2+E0sKiP02D8m/92d2abzWzaFE/mBnOmfm+c+bMGRv5Bytrqy6+KHzETAchCT5S56dn9bb5g18CIIotba8p0M0JtaFHyjUn/HdeqVEUnTa7bhyqdrbdK9QV4GTFpenxcLkW2Mg+Mbemd3baUAVKjiNBLS17XupMWOYi1/CbAojMyenF58LkQu4NLktcjo+QeMj/wEWiyITCibQRuR8ZmC++k5lSJQAObBuiC4bxsZNMO/6mbYUqE8juXLSpUyojAwUFG5QM4Kyv3gRBgUDsu9rDIFRRwYmZNeCVNNdhYDcMk4MpkfVKKoxh0OEkIJAqiE6Gdx4UOMsby+RSvKVDN/bRJO3t/jgJBVq8Kw+6fYD2zMjuPJqkH5I+mVyNel7BgS1gus3caOHZ6Pm9V1p/dIvSdKlKHcYntrpA2keRYOvCP5ZTGVvi0fPwW32mLd4b2lS5JsKPguHOSNvyM/UCQnwZ4DsT8gUIB7dcevXTcQAAAABJRU5ErkJggg==";
        private const string yellowBase64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAGTSURBVDhPY2TAAty8PTiAlDYQm4MFGBhOAvHVXVt3/IBwEQDFAJBGLvY/3T9/M6WJCf74oyj5BWQQw/3nPD9ef+BgYmP5N+fbT5ZSZIPgBgA1a3Ow/dkG1CQVYv+QhZvzL1QGAr7/ZGbYcFj2z83HfM9+/GJxAxpyEyQONgBkM1DzTWejF3K2+q9AQjjB6RvCDFuPSz8CGqIOcgkTSBDkbJDNhDSDgKnGWwYVmc9SID0gPiPIdmamfx+rYi6zoTsbF/j6nZmhbYnur7//mPhBLtAGBRixmkEApFZE4CdIgzbIAHNYaJMC5MW/sgMpc3AYAEMSTJMCWJn/gfWAiJP3nvN8A3FIAVA9J0EGXH31noMFFM/EApBaUMICMq8ygeKSnfXfLFAigUgTBluOyfwGpUp4OgAlz9tP+F6CEgkhcOG2IMP1h/yvQHpAfLC7796+80dOSW3nwxc8/o9fc3EpS31mYmP9D5KCA0hSlvt99LLYC6BmZ6DtL0HieDOTkuQXLpA4KMAIZiZkADIISBGRnRkYAA9ZuQFRodt/AAAAAElFTkSuQmCC";
        private const string greenBase64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAGQSURBVDhPlVNdSwJBFJ39GF21MFS0NCIiSmIJevMxpBcfe+65P9AP6Q9Uj/0G66WnIHsMDII2AlEyzVL6cNdZ1+5xzYwy9cDszL0z59y7d+5I7A9kMhmNJp1GqutgLEcjn81mTdf8xg8BEFve9r7aknffZ4R4njW98IfKmhVocMXmzpHHUvYGhfoCRNYFd07qMTN2tVlVheb0dlyolsz087AdKfofuZC3SOQG/q4AIhPZuNuoJ+7XG3ANxfzNFEtehksksoxMZDiRNiKPIgPF5BurxZsxcGBLiO5IndezncKvtIeBU9z08YItd6RpZKCjYOOSAZz9CAqbljoEUl/VngQvUctDU6pbAyqlO08AR+10OfjkQg9aE8Yk6HFyEMgH6pzjnscFzvobXKVlXsZd2h7nAE3ibo/G2kVItLlz2O8DtGek5KuiSUYhfhtg0YL/CRzYCj6GYdirSyun1PPbwYrXV0s0ZSoStvrotbJYvA5WeEtJU/Qy/P8/pjnTBz8Khn9G2kMf0yAgRNMYz5mxT7hrt8EtdVJ5AAAAAElFTkSuQmCC";
        private const string blueBase64 = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsIAAA7CARUoSoAAAAGASURBVDhPlZPNSkJBGIbfOf6coxJh1MaorKgWLlu4aREEErQIuojuoC6i7iC6iiD7uYDARYsWQqLRDxhZEUiUJ4+e6XvPMTNJ1Afm75t53xlmvlH4h8zGuiVNSkraCwA5Kfnz41PbH/7yx4DCpo7sG6hvO5hofCFJI5i4s4N4NTRChwFV2+k0ahuIOOVqK2tjJvGmt4IuYq0ZH4UaxtRRI4Lio6HsjJgU/LjAnUVcqOrV6XesMNSTGC4RVycPYrLEkxgM8tjcuZ+YfGAZNuYT1HCsuLvWgWpZ74a7j90LQ2wm1V5dqeYoT5DihQ0qJlzrYLwp3RQN0j+3PQx1TJnSpL070FBeOwwuQp6GVc7C7ScHw9DS5GiQD+ElyHceFK5lYkk3b/AtXYQPmCT+dH/iKuswK9t5wPS0UKowSfoRxRUiuH6mhuMAq5tiqbGwmDwzcb9pqnLUxpzkfZhTbVqp7IyoiycRr8nuFT/eQfdnsjEbZZwX1vczdUIjaQb4zsA3c/aeyS3H8VwAAAAASUVORK5CYII=";


        public enum IconStatusColor
        {
            Red,
            Yellow,
            Green,
            Blue
        }

        private static Bitmap Base64ToBitmap(string base64String)
        {
            byte[] imageAsBytes = Convert.FromBase64String(base64String);
            using (MemoryStream memoryStream = new MemoryStream(imageAsBytes))
            {
                var bmpReturn = (Bitmap)System.Drawing.Image.FromStream(memoryStream);
                memoryStream.Close();
                return bmpReturn;
            }
        }

        private static CommandBarButton _iconButton = null;
        public static async Task SetIconColorAsync(IconStatusColor color)
        {
            var base64 = "";
            switch (color)
            {
                case IconStatusColor.Red:
                    base64 = redBase64;
                    break;
                case IconStatusColor.Yellow:
                    base64 = yellowBase64;
                    break;
                case IconStatusColor.Green:
                    base64 = greenBase64;
                    break;
                case IconStatusColor.Blue:
                    base64 = blueBase64;
                    break;
             }

            var newText = "";
            switch (color)
            {
                case IconStatusColor.Red:
                    newText = "[L.AI] Unable to connect to your AI backend. Check your settings.";
                    break;
                case IconStatusColor.Yellow:
                    newText = "[L.AI] Connecting to the AI backend...";
                    break;
                case IconStatusColor.Green:
                    newText = "[L.AI] Waiting for suggestion request.";
                    break;
                case IconStatusColor.Blue:
                    newText = "[L.AI] Processing the request.";
                    break;
            }

            var allCaptions = new List<string>();

            if(_iconButton != null)
            {
                var bmp = Base64ToBitmap(base64);
                _iconButton.Picture = (stdole.StdPicture)IconConverter.GetIPictureDispFromImage(bmp);
                _iconButton.Caption = newText;
                _iconButton.TooltipText = newText;
                return;
            }

            try
            {
                var dte2 = (DTE2)await LAIPackage.Instance.GetServiceAsync(typeof(DTE));
                CommandBars commandBars = (CommandBars)dte2.CommandBars;
                foreach (CommandBar commandBar in commandBars)
                {
                    foreach (CommandBarControl control in commandBar.Controls)
                    {
                        allCaptions.Add(control.Caption);
                        if (control is CommandBarButton button && control.Caption.Contains("[L.AI]"))
                        {
                            _iconButton = button;

                            var bmp = Base64ToBitmap(base64);
                            button.Picture = (stdole.StdPicture)IconConverter.GetIPictureDispFromImage(bmp);
                            button.Caption = newText;
                            return; // Exit the loop after finding the button
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LAIPackage.Serilog.Error($"[MenuStatusHandler] Error while trying to update the toolbar: {ex.Message}");
            }
            ;
        }
    }
}
