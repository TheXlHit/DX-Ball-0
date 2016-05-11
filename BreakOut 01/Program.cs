using System;

namespace BreakOut_01
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static bool IsTournament = false;
        public static Resolution FullscreenResolution = null;
        public static bool IsFullscrenStartup = true;
        public static bool IsShowFSK18 = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Drawing.Rectangle r = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            FullscreenResolution = new Resolution();
            FullscreenResolution.Width = r.Width;
            FullscreenResolution.Height = r.Height;

            if (r.Height >= 768 && r.Width >= 1024)
            {
                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    for (int i = 1; i < Environment.GetCommandLineArgs().Length; i++)
                    {
                        if (Environment.GetCommandLineArgs()[i].ToLower() == "-t")
                        {
                            IsTournament = true;
                        }

                        if (Environment.GetCommandLineArgs()[i].ToLower() == "-f")
                        {
                            IsFullscrenStartup = false;
                        }

                        if (Environment.GetCommandLineArgs()[i].ToLower() == "-s")
                        {
                            IsShowFSK18 = true;
                        }

                        if (Environment.GetCommandLineArgs()[i].ToLower().StartsWith("-r=") && Environment.GetCommandLineArgs()[i].ToLower().Contains("x"))
                        {
                            string[] s = Environment.GetCommandLineArgs()[i].ToLower().Replace("-r=", "").Split(new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);
                            if (s.Length == 2)
                            {
                                FullscreenResolution = new Resolution();
                                float.TryParse(s[0], out FullscreenResolution.Width);
                                float.TryParse(s[1], out FullscreenResolution.Height);
                            }
                        }
                    }
                }

                using (var game = new Game1())
                    game.Run();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Screen Size must be at least 1024x768");
            }
        }
    }
#endif
}
