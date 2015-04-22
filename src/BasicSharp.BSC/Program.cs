using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSharp.BSC
{
    public class Program
    {
        const string VS_SHELL = @"%comspec% /k ""C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\vcvarsall.bat"" x86";

        string fileAddress;
        string fileName;

        static void Main(string[] args)
        {
#if DEBUG
            var fileAddress = @"C:\Users\Guilherme\Documents\Visual Studio 2013\Projects\ConsoleApplication2\ConsoleApplication2\bin\Debug\";
            var fileName = "teste.il";
#else
            var fileAddress = args[0];
            var fileName = args[1];
#endif

            var p = new Program(fileAddress, fileName);
            p.Run();
        }

        public Program(string fileAddress, string fileName)
        {
            this.fileAddress = fileAddress;
            this.fileName = fileName;
        }

        public void Run()
        {
            invokeShell(fileAddress, fileName);
        }

        void invokeShell(string fileAddress, string fileName)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = fileAddress;
                //process.StartInfo.FileName = fileName;

                setAsmArguments(process);

                // Redirects the standard input so that commands can be sent to the shell.
                process.StartInfo.RedirectStandardInput = true;
                // Runs the specified command and exits the shell immediately.
                //process.StartInfo.Arguments = @"/c ""dir""";

                process.OutputDataReceived += ProcessOutputDataHandler;
                process.ErrorDataReceived += ProcessErrorDataHandler;

                process.Start(VS_SHELL);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Send a directory command and an exit command to the shell
                process.StandardInput.WriteLine("dir");
                process.StandardInput.WriteLine("exit");

                process.WaitForExit();
            }
        }

        void setAsmArguments(Process process)
        {
            process.StartInfo.Arguments = "ilasm " + fileName;
        }

        private void ProcessErrorDataHandler(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void ProcessOutputDataHandler(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
