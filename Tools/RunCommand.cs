using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Tools
{
    public static class RunCommand
    {
        public static string run(object input, string username = null, SecureString password = null, string target = "cmd.exe", bool seperate_lines = false)
        {
            List<string> commands = new List<string>();
            if (input.GetType().Name == "List`1")
            {
                commands = (List<string>)input;
            }
            else if (input.GetType().Name == "String")
            {
                commands.Add((string)input);
            }
            var process = new Process();
            var psi = new ProcessStartInfo();
            psi.FileName = target;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            //if (username != null)
            //{
            //    psi.Verb = "runas";
            //    psi.UserName = username;
            //    psi.Password = password;
            //}

            process.StartInfo = psi;
            process.Start();
            string output = string.Empty;
            string error = string.Empty;
            if(seperate_lines)
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        // Process each line of the output
                        string[] lines = e.Data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)
                        {
                            // Do something with each line (e.g., append to the output string)
                            output += line + Environment.NewLine;
                        }
                    }
                };
            }
            else
            {
                process.OutputDataReceived += (sender, e) => { output += e.Data; };
            }
            process.ErrorDataReceived += (sender, e) => { error += e.Data; };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            using (StreamWriter sw = process.StandardInput)
            {
                foreach (var cmd in commands)
                {
                    sw.WriteLine(cmd);
                }
            }
            process.WaitForExit();
            return output;
        }
    }
}
