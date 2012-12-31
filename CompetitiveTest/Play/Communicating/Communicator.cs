namespace SSU.CompetitiveTest.Play.Communicating {

    using System;
    using System.IO;
    using System.Diagnostics;

    public sealed class Communicator {

        #region Fields

        private readonly String name;

        private readonly String executablePath;

        private readonly ProcessStartInfo startInfo = new ProcessStartInfo();

        #endregion

        #region Properties

        public String Name { get { return name; } }

        public String ExecutablePath { get { return executablePath; } }

        #endregion

        #region Methods

        #region Suppress error dialogs

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern UInt32 SetErrorMode(UInt32 hHandle);

        static Communicator() {
            SetErrorMode(0x0002);
        }

        #endregion

        /// <summary>
        /// Creates communicator with specified executable.
        /// </summary>
        /// <param name="executablePath">Path to console application's executable</param>
        /// <exception cref="ArgumentException">The executable doesn't exist or the format is not supported</exception>
        public Communicator(String executablePath) {
            if (File.Exists(executablePath) && executablePath.ToLower().EndsWith(".exe")) {
                throw new ArgumentException("The executable either doesn\'t exist or has not supported format", "executablePath");
            }
            this.executablePath = executablePath;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = false;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = false;
            startInfo.FileName = executablePath;
            Int32 slash = executablePath.LastIndexOf('\\') + 1;
            name = executablePath.Substring(slash, executablePath.LastIndexOf('.') - slash);
        }

        /// <summary>
        /// Runs the application ones
        /// </summary>
        /// <param name="incoming">Data for application's standard input stream</param>
        /// <param name="timeLimit">Time bound for execution</param>
        /// <returns>Data from application's standard output and associated information</returns>
        /// <exception cref="InvalidOperationException">The executable file has been missed</exception>
        /// <exception cref="RuntimeErrorException">The process has exited with an error</exception>
        /// <exception cref="TimeLimitException">Time limit exceeded</exception>
        public RunOutcome Run(String incoming, TimeSpan timeLimit) {
            if (!File.Exists(executablePath)) {
                throw new InvalidOperationException("The executable file has been missed");
            }
            using (Process p = new Process()) {
                p.StartInfo = startInfo;
                p.Start();
                p.StandardInput.WriteLine(incoming);
                p.StandardInput.Flush();
                DateTime begin = DateTime.Now;
                p.WaitForExit((Int32)timeLimit.TotalMilliseconds);
                if (p.HasExited) {
                    if (p.ExitCode != 0) {
                        throw new RuntimeErrorException(p.ExitCode, "The process has exited with an error");
                    }
                    TimeSpan elapsed = DateTime.Now - begin;
                    return new RunOutcome(p.StandardOutput.ReadToEnd(), elapsed, begin);
                } else {
                    p.Kill();
                    throw new TimeLimitException("Time limit exceeded");
                }
            }
        }

        #endregion

    }

}
