using rt.data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace rt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int x = 320;
        int y = 240;
        Vector2 c = new Vector2(320/2, 240/2);
        PixelFormat pf = PixelFormats.Bgr32;
        byte[] img;
        int stride;
        WriteableBitmap rensc;
        Player player;
        float step = 1;
        Map m = new Map();
        float rd = 500;
        
        public MainWindow()
        {
            InitializeComponent();

            
            stride = (x * pf.BitsPerPixel + 7)/8;
            img = new byte[x * y * 4];
            rensc = new WriteableBitmap(x, y, 96, 96, pf, null);
            ren.Source = rensc;

            // create the player
            player = new Player("player", new Vector3(90,90, 0), new Vector3(0,0,0.5f), new Vector3(0,0,0), 1);
            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromMilliseconds(10);
            tmr.Tick += Update;
            tmr.Start();

            

            


        }

        public void PutPixel(int xp, int yp, Vector3 c)
        {
            
            byte[] cd = { (byte)c.X, (byte)c.Y, (byte)c.Z, 0 };
            if (xp < 0 || xp > x-1 || yp < 0 || yp > y-1)
            {
                return;
            } else
            {
                rensc.WritePixels(new Int32Rect(xp, yp, 1, 1), cd, stride, 0);
                //for (int i = 0; i < 4; i++)
                //{
                //    img[(xp + x * yp)+1] = 255;
                //}
            }
                
            
            
        }

        public void FullUpdate()
        {

            rensc.WritePixels(new Int32Rect(0, 0, 320, 240), img, stride, 0);
            DrawBackground();
            for (int i = 0; i < x; i++)
            {
                float a = (float)i * player.fov / (float)x;
                float maxd = 50;

                RayIntersect ri = DistToWall(a);
                float h = y / ri.distance;
                float perc = 1 - (ri.distance / maxd);
                //float it =  (255 * MathF.Log(perc+1, 2));
                float it = perc * 255;

                if (ri.distance < maxd)
                {

                    
                    //Debug.WriteLine(it);
                    //Debug.WriteLine(it);
                    for (int j = 0; j < h; j++)
                    {

                        PutPixel(x - i, (int)(c.Y - h/2 + j), (((Wall)ri.gameObject).color / 255) * it);
                    }
                }
                
            }
            //DrawMap();

        }


        void DrawLine(Vector2 start, Vector2 end, Vector3 c)
        {
            float dx = MathF.Abs(end.X - start.X);
            float sx = start.X < end.X? 1 : -1;
            float dy = -MathF.Abs(end.Y - start.Y);
            float sy = start.Y < end.Y ? 1 : -1;
            int ey = (int)end.Y;
            int ex = (int)end.X;
            float error = dx + dy;
            int x = (int)(start.X);
            int y = (int)(start.Y);

            while (true)
            {
                PutPixel(x, y, c);
                if (x == ex && y == ey)
                {
                    break;
                }
                float e2 = 2 * error;
                if (e2 >= dy)
                {
                    if (x == ex)
                    {
                        break;
                    }
                    error = error + dy;
                    x = x + (int)sx;
                }
                if (e2 <= dx)
                {
                    if (y == ey)
                    {
                        break;
                    }
                    error = error + dx;
                    y = y + (int)sy;
                }
            }


        }

        void DrawMap()
        {
            
            foreach (Wall w in m.walls)
            {
                DrawLine(new Vector2(w.x1, w.y1), new Vector2(w.x2, w.y2), w.color);

                
            }
        }

        public void Update(object sender, EventArgs args)
        {
            rensc.WritePixels(new Int32Rect(0, 0, 320, 240), img, stride, 0);
            if (Keyboard.IsKeyDown(Key.W))
            {
                player.MoveForward(step);
            }
            else if (Keyboard.IsKeyDown(Key.S))
            {
                player.MoveForward(-step);
            }
            if (Keyboard.IsKeyDown(Key.D))
            {
                player.transform.Rotate(new Vector3(0, 0, 1), -0.05f);
            }
            else if (Keyboard.IsKeyDown(Key.A))
            {
                player.transform.Rotate(new Vector3(0, 0, 1), 0.05f);
            }

            FullUpdate();
            //Debug.WriteLine(newPos);
            //Debug.WriteLine(player.transform.location);
            //Vector3 newPos = player.transform.location + player.transform.Forward() * 10;
            //PutPixel((int)(player.transform.location.X), (int)(player.transform.location.Y), new Vector3(0,0, 255));
            //DrawLine(new Vector2(player.transform.location.X, player.transform.location.Y), new Vector2(newPos.X, newPos.Y), new Vector3(255, 0, 0));
            //Debug.WriteLine(System.DateTime.Now.Millisecond);
            
            // clear 




        }


        RayIntersect DistToWall(float ra)
        {
            // check distance from the player to each wall individually
            float d = float.MaxValue;

            //determine i angle
            float ld = float.MaxValue;
            RayIntersect ri = new RayIntersect(null, ld);

            foreach (Wall w in m.walls)
            { 
                Vector2 p1 = new Vector2(w.x1, w.y1);
                Vector2 p2 = new Vector2(w.x2, w.y2);
                Vector2 ro = new Vector2(player.transform.location.X, player.transform.location.Y);
                Vector3 prerot = player.transform.PreviewRotation(new Vector3(0, 0, 1), ((-player.fov/2 + ra) * MathF.PI)/180f);
                Vector2 rd = new Vector2(MathF.Cos(prerot.X) * MathF.Sin(prerot.Z), MathF.Cos(prerot.X) * MathF.Cos(prerot.Z));
                //Vector3 newPos = player.transform.location + player.transform.Forward() * 10;
                //DrawLine(new Vector2(c.X + player.transform.location.X, c.Y + player.transform.location.Y), new Vector2(c.X + newPos.X, c.Y + newPos.Y), new Vector3(0,255, 0));
                d = RayToLineInterseaction(ro, rd, p1, p2, 100);
                
                
                //Debug.WriteLine(d);
                
                if (d < ri.distance && d > 0)
                {
                    ri.distance = d;
                    ri.gameObject = w;
                }
                
            }
            //Debug.WriteLine(ld);
            return ri;
        }

        float RayToLineInterseaction(Vector2 ro, Vector2 rd, Vector2 p1, Vector2 p2, float length)
        {

            // ray
            //Vector2 re = ro + rd * length;
            //DrawLine(ro, re);

            //DrawLine(ro, ro + (rd * 10), new Vector3(255, 255, 0));
            Vector2 re = ro + (rd * length);

            // intersect point
            Vector2 r = (re - ro);
            Vector2 s = (p2 - p1);

            float d = r.X * s.Y - r.Y * s.X;
            float u = ((p1.X - ro.X) * r.Y - (p1.Y - ro.Y) * r.X) / d;
            float t = ((p1.X - ro.X) * s.Y - (p1.Y - ro.Y) * s.X) / d;

            if (u >= 0 && u <= 1 && t >= 0 && t <= 1)
            {
                // get the intersecton point
                Vector2 ip = ro + t * r;
                // get the distance from ro to ip
                float dst = (MathF.Sqrt((ip.X - ro.X) * (ip.X - ro.X) + (ip.Y - ro.Y) * (ip.Y - ro.Y)));
                //DrawLine(ro, ip, new Vector3(255, 255, 255));
                return dst;
            } else
            {
                return -1;
            }
            /**
            Vector2 v1 = ro -p1;
            Vector2 v2 = p2 - p1;
            Vector2 v3 = new Vector2(-rd.Y, rd.X);
            //float d = Vector2.Dot(v3, v2);
            float d = v2.X*v3.X + v2.Y*v3.Y;
            
            if (MathF.Abs(d) < 0.00001f)
            {
                //Debug.WriteLine(MathF.Abs(d));
                return -1;
            }

            float t1 = (v1.X * v2.Y) - (v1.Y * v2.X) /d ;
            //float t2 = Vector2.Dot(v3, v1) / d;
            float t2 = (v1.X*v3.X + v1.Y*v3.Y) /d;

            if (t1 >= 0.0f && (t2 >= 0.0f && t2 <= 1.0f))
            {
                
                return t1;
                
            }

            return -1;
            **/
            



        }
        
        void DrawBackground()
        {
            // lines screen width
            for (int i = 0; i < y; i++)
            {
                if ( i < y/2)
                {
                    DrawLine(new Vector2(0, i), new Vector2(x, i), new Vector3(255, 128,15));
                } else
                {
                    float mult = (((float)i / (float)y)) * 255;

                    DrawLine(new Vector2(0, i), new Vector2(x, i), (new Vector3(128,128,128) / y) * mult);
                }
            }
        }

        Vector2 NormalizeVector(Vector2 pt)
        {
            float length = MathF.Sqrt(pt.X * pt.X + pt.Y * pt.Y);
            pt = pt / length;
            return pt;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}
