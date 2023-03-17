using System.Configuration;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;

namespace Consumer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(1.ToString());
            var port = int.Parse(ConfigurationManager.AppSettings.Get("port"));
            var client = new UdpClient(port);


            await Task.Run(async () =>
            {
                while (true)
                {

                    if (checkBox1.Checked)
                    {
                        var data = await client.ReceiveAsync();
                        //MessageBox.Show("asd");
                        //MessageBox.Show($"Bytes received: {data.Buffer.Length * sizeof(byte)}");
                        using (var ms = new MemoryStream(data.Buffer))
                        {
                            pictureBox1.Image = new Bitmap(ms);
                        }
                        //Text = $"Bytes received: {data.Buffer.Length * sizeof(byte)}";
                    }
                    else
                    {
                        Graphics gr;
                        //var bmp = new Bitmap(eventArgs.Frame, 800, 600);
                        var bmp = new Bitmap(1920, 1080);

                        gr = Graphics.FromImage(bmp);
                        gr.CopyFromScreen(0, 0, 0, 0, new Size(bmp.Width, bmp.Height));
                        try
                        {

                            using (var ms = new MemoryStream())
                            {
                                bmp.Save(ms, ImageFormat.Jpeg);

                                pictureBox1.Image = new Bitmap(ms);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            });
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.Select(ip => ip)
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToArray().FirstOrDefault();
        }
    }
}