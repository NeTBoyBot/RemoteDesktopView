using AForge.Video.DirectShow;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

public class Program
{
    const int SH_HIDE = 0;
    const int SH_SHOW = 5;

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);



    private static IPEndPoint consumerEndPoint;
    private static UdpClient client = new UdpClient();
    public static void Main()
    {
        //var consumerIp = ConfigurationManager.AppSettings.Get("consumerIp");
        var lists = Dns.GetHostEntry("188.191.21.25").AddressList;
        var consumerIp = Dns.GetHostEntry("188.191.21.25").AddressList[5].ToString();
        //consumerIp = "188.191.21.25";
        var consumerPort = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));
        consumerEndPoint = new IPEndPoint(IPAddress.Parse(consumerIp), consumerPort);

        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        //VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
        //videoSource.NewFrame += NewFrame;
        //videoSource.Start();
        Task.Run(() => NewFrame());
        Console.WriteLine("Press enter");
        Console.ReadLine();

        //ShowWindow(GetConsoleWindow(), 0);
    }

    private static async void NewFrame(/*object sender, AForge.Video.NewFrameEventArgs eventArgs*/)
    {
        Graphics gr;
        //var bmp = new Bitmap(eventArgs.Frame, 800, 600);
        var bmp = new Bitmap(800,600);
        
       
        while (true)
        {
            gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(0, 0, 0, 0, new Size(bmp.Width, bmp.Height));
            
            try
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    Console.Clear();
                    //Console.WriteLine(await client.SendAsync(bytes, bytes.Length, consumerEndPoint));
                    //Console.WriteLine($"Bytes received: {bytes.Length * sizeof(byte)}");
                    await client.SendAsync(bytes, bytes.Length, consumerEndPoint);
                }
            }
            catch
            {

            }
        }
    }

}