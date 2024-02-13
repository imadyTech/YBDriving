//https://kannmu.github.io/2023/06/10/Unity-Python-Interaction/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEditor;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;

public class PythonConnector : MonoBehaviour
{

    private ProcessStartInfo startInfo;
    private Process process;

    private UdpClient udpClient;
    private IPEndPoint pythonEndPoint;

    public CarController car;

    // Start is called before the first frame update
    void Start()
    {
        Kill_All_Python_Process();

        // 创建UDP通信的Client
        udpClient = new UdpClient();
        // 设置IP地址与端口号
        pythonEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31416);

        string pythonPath = "Scripts/Python/hellopUnity.py";
        // 获取Unity项目的数据路径
        string dataPath = Application.dataPath;
        // 拼接Python文件的完整路径
        string fullPath = dataPath + "/" + pythonPath;
        //UnityEngine.Debug.Log(dataPath);
        //fullPath = "D:\\unity-python\\Unity-Python\\Assets\\Scripts\\Python\\hellopython.py";
        // 设置命令行参数
        string command = "/c  Python & python \"" + fullPath + "\"";

        // 创建ProcessStartInfo对象
        startInfo = new ProcessStartInfo();
        // 设定执行cmd
        startInfo.FileName = "cmd.exe";
        // 输入参数是上一步的command字符串
        startInfo.Arguments = command;
        // 因为嵌入Unity中后台使用，所以设置不显示窗口
        startInfo.CreateNoWindow = true;
        // 这里需要设定为false
        startInfo.UseShellExecute = false;
        // 设置重定向这个进程的标准输出流，用于直接被Unity C#捕获，从而实现 Python -> Unity 的通信
        startInfo.RedirectStandardOutput = true;
        // 设置重定向这个进程的标准报错流，用于在Unity的C#中进行Debug Python里的bug
        startInfo.RedirectStandardError = true;

        // 创建Process
        process = new Process();
        process.StartInfo = startInfo;
        process.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
        process.ErrorDataReceived += new DataReceivedEventHandler(OnErrorDataReceived);

        //启动脚本Process，并且激活逐行读取输出与报错
        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();


        InitializeUDPListener();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //向Python发送识别指令
            byte[] message = Encoding.ASCII.GetBytes("CarSpeed: "+ 100f* car.delta.magnitude * car.timescale);
            udpClient.Send(message, message.Length, pythonEndPoint);
            UnityEngine.Debug.Log(">>> " + Encoding.ASCII.GetString(message));
        }
    }
    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            UnityEngine.Debug.Log("[Redirected] "+e.Data);
            if (e.Data == "StartRecognition")
            {
                print("Recognizing");
            }
        }
    }

    private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data.Contains("Error") || e.Data.Contains("Exception"))
        {
            UnityEngine.Debug.LogError("Python Error: " + e.Data);
        }
        else
        {
            UnityEngine.Debug.Log("Python Message: " + e.Data);
        }
    }

    void Kill_All_Python_Process()
    {
        Process[] allProcesses = Process.GetProcesses();
        foreach (Process process_1 in allProcesses)
        {
            try
            {
                // 获取进程的名称
                string processName = process_1.ProcessName;
                // 如果进程名称中包含"python"，则终止该进程
                if (processName.ToLower().Contains("python") && process_1.Id != Process.GetCurrentProcess().Id)
                {
                    process_1.Kill();
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                print(ex);
            }
        }
    }
    void OnApplicationQuit()
    {
        if (receiveThread != null) receiveThread.Abort();
        client.Close();

        // 在应用程序退出前执行一些代码
        UnityEngine.Debug.Log("应用程序即将退出，清理所有Python进程");
        // 结束所有Python进程
        Kill_All_Python_Process();
    }


    Thread receiveThread;
    UdpClient client;
    private void InitializeUDPListener()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(31415);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31415);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                UnityEngine.Debug.Log("<<< " + text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
}
