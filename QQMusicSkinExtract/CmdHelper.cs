using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    class CmdHelper
    {
        /// <summary>    
        /// 执行DOS命令，返回DOS命令的输出    
        /// </summary>    
        /// <param name="cmd">dos命令</param>     
        /// <returns>返回DOS命令的输出</returns>    
        public static String ExecuteCmd(String cmd)
        {
            return ExecuteCmd(cmd, "", 0);
        }
        /// <summary>    
        /// 执行DOS命令，返回DOS命令的输出    
        /// </summary>    
        /// <param name="cmd">dos命令</param>    
        /// <param name="workingDirectory">工作目录</param>    
        /// <returns>返回DOS命令的输出</returns>    
        public static String ExecuteCmd(String cmd, String workingDirectory)
        {
            return ExecuteCmd(cmd, workingDirectory, 0);
        }

        /// <summary>    
        /// 执行DOS命令，返回DOS命令的输出    
        /// </summary>    
        /// <param name="cmd">dos命令</param>    
        /// <param name="ms">等待命令执行的时间（单位：毫秒），    
        /// 如果设定为0，则无限等待</param>    
        /// <returns>返回DOS命令的输出</returns>    
        public static String ExecuteCmd(String cmd, int ms)
        {
            return ExecuteCmd(cmd, "", ms);
        }
        /// <summary>    
        /// 执行DOS命令，返回DOS命令的输出    
        /// </summary>    
        /// <param name="cmd">dos命令</param>    
        /// <param name="workingDirectory">工作目录</param>   
        /// <param name="ms">等待命令执行的时间（单位：毫秒），    
        /// 如果设定为0，则无限等待</param>    
        /// <returns>返回DOS命令的输出</returns>    
        public static String ExecuteCmd(String cmd, String workingDirectory, int ms)
        {
            String output = ""; //输出字符串    
            if (cmd != null && !cmd.Equals(""))
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();//创建进程对象    
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = "cmd.exe";//设定需要执行的命令    
                startInfo.Arguments = "/C " + cmd;//“/C”表示执行完命令后马上退出    
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动    
                startInfo.RedirectStandardInput = false;//不重定向输入    
                startInfo.RedirectStandardOutput = true; //重定向输出    
                startInfo.CreateNoWindow = true;//不创建窗口
                if (workingDirectory != null)
                    startInfo.WorkingDirectory = workingDirectory;
                process.StartInfo = startInfo;
                try
                {
                    if (process.Start())//开始进程    
                    {
                        if (ms == 0)
                        {
                            process.WaitForExit();//这里无限等待进程结束    
                        }
                        else
                        {
                            process.WaitForExit(ms); //等待进程结束，等待时间为指定的毫秒    
                        }
                        output = process.StandardOutput.ReadToEnd();//读取进程的输出 
                    }
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                        process.Close();
                }
            }
            return output;    
        }

        /*
        public static void ExecuteCmd(String cmd)
        {

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            // process.Start();
        }
        public static void ExecuteCmd(String cmd, String workingDirectory)
        {

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;

            Process process = Process.Start(startInfo);
            process.WaitForExit();
            //   process.Start();
        }
            */
    }
}
