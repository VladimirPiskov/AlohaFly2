using NLog;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AlohaFly.Fonts
{
    public static class FontInstall
    {
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                         string lpFileName);
        private readonly static string templateFolder = @"\Data\";
        private readonly static string fontPath = @"HelveticaNeue.ttc";

        static Logger logger = LogManager.GetCurrentClassLogger();
        public static void Install()
        {
            try
            {
                int result = -1;
                int error = 0;
                // Try install the font.
                result = AddFontResource(System.AppDomain.CurrentDomain.BaseDirectory + @"\" + templateFolder + fontPath);
                error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    logger.Error(new Win32Exception(error).Message);
                }
                else
                {
                    logger.Debug((result == 0) ? "Font is already installed." :
                                                      "Font installed successfully.");
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }
    }
}
