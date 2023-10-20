using rt.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
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
        Vector2 c;
        PixelFormat pf = PixelFormats.Bgr32;
        byte[] img;
        int stride;
        WriteableBitmap rensc;
        Player player;
        float step = 0.125f;
        Map m = new Map();
        float rdi = 500;
        

        // optimisaion vectors for raycasting
        Vector2 p1 = new Vector2(0, 0);
        Vector2 p2 = new Vector2(0, 0);
        Vector2 ro = new Vector2(0, 0);
        Vector2 prerot = new Vector2(0, 0);
        Vector2 rd = new Vector2(0, 0);
        
        public MainWindow()
        {
            InitializeComponent();

            c = new Vector2(x / 2, y / 2);
            stride = (x * pf.BitsPerPixel + 7)/8;
            img = new byte[x * y * 4];
            rensc = new WriteableBitmap(x, y, 96, 96, pf, null);
            ren.Source = rensc;

            // create the player
            player = new Player("player", new Vector3(0, 0, 0), new Vector3(0,0,0.5f), new Vector3(0,0,0), 1);

            // load map
            m.load();
            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromMilliseconds(5);
            tmr.Tick += Update;
            tmr.Start();

            

            


        }
        public void tmr()
        {
            
        }

        public void PutPixel(int xp, int yp, Vector3 c)
        {
            
            byte[] cd = { (byte)c.X, (byte)c.Y, (byte)c.Z , 0};
            if (xp < 0 || xp > x-1 || yp < 0 || yp > y-1)
            {
                //Debug.WriteLine("too big" + xp + " " +  yp);
                
                return;
            } else
            {
                //rensc.WritePixels(new Int32Rect(xp, yp, 1, 1), cd, stride, 0);
                //img[xp + (yp*x) + 1] = cd[0];
                //img[xp + (yp*x) + 2] = cd[1];
                //img[xp + (yp*x) + 3] = cd[2];
                for (int i = 0; i < 3; i++)
                {
                    
                    img[(xp * 4) + ((yp * 4) * x) + i] = cd[2-i];
                }
                

            }
                
            
            
        }

        public void FullUpdate()
        {

            float ct = DateTime.Now.Millisecond;
            DrawBackground();
            
            for (int i = 0; i < x; i++)
            {
                float a = (float)i * player.fov / (float)x;
                float maxd = 50;

                (RayIntersect ri, RayIntersect ri2) = DistToWall(a);
                

                if (ri.distance < maxd)
                {
                    if (((Wall)ri.gameObject).wtype == 2)
                    {
                        // draw window
                        if (ri2.gameObject != null)
                        {
                            DrawWallPixel(ri2, i, maxd);

                        }
                        DrawWallPixel(ri, i, maxd);

                    } else
                    {
                        DrawWallPixel(ri, i, maxd);
                    }

                    //Debug.WriteLine(it);
                    //Debug.WriteLine(it);
                    
                }
                
            }
            rensc.WritePixels(new Int32Rect(0, 0, x, y), img, stride, 0);
            
            float ct2 = DateTime.Now.Millisecond;
            float d = ct2 - ct;
            this.Title = "fps: " + (int)(1000 / d) + " frame time: " + d + "ms"; 
            
            //DrawMap();

        }

        void DrawWallPixel(RayIntersect ri, int i, float maxd)
        {
            
            Wall w = ((Wall)ri.gameObject);
            float h = y / ri.distance;
            float perc = 1 - (ri.distance / maxd);
            //float it =  (255 * MathF.Log(perc+1, 2));
            float it = perc * 255;
            
            for (int j = 0; j < h; j++)
            {
                
                
                Vector3 cit = (w.color / 255) * it;
                byte px = (byte)((256 / h * j) / 2);
                byte py = (byte)((ri.distToStart % 1) * 128);


                Vector3 tc = new Vector3(w.tex[(py * 3) + ((px * 3) * 128)], w.tex[(py * 3) + ((px * 3) * 128) + 1], w.tex[(py * 3) + ((px * 3) * 128) + 2]);
                int ta = w.alpha[(py * 3) + ((px * 3) * 128)];
                int psy = (int)(c.Y - h / 2 + j);
                if (ta > 0 && psy < y && psy > 0)
                {
                    PutPixel(x - i, psy, tc / 256 * cit);
                }
                

            }
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
            //rensc.WritePixels(new Int32Rect(0, 0, 320, 240), img, stride, 0);
            if (Keyboard.IsKeyDown(Key.W))
            {
                // raycast collision 
                RayIntersect ri = DistToWall(0).Item1;
                if (ri.distance > 0.25)
                {
                    player.MoveForward(step);
                }
                
            }
            else if (Keyboard.IsKeyDown(Key.S))
            {
                RayIntersect ri = DistToWall(180).Item1;
                if (ri.distance > 0.25)
                {
                    player.MoveForward(-step);
                }
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


        (RayIntersect, RayIntersect) DistToWall(float ra)
        {
            // check distance from the player to each wall individually
            float d = float.MaxValue;
            float dts = 0;
            

            //determine i angle
            float ld = float.MaxValue;
            RayIntersect ri = new RayIntersect(null, ld);
            RayIntersect ri2 = new RayIntersect(null, ld);
            
            
            

            foreach (Wall w in m.walls)
            { 
                Vector2 p1 = new Vector2(w.x1, w.y1);
                
                Vector2 p2 = new Vector2(w.x2, w.y2);
                Vector2 ro = new Vector2(player.transform.location.X, player.transform.location.Y);
                Vector3 prerot = player.transform.PreviewRotation(new Vector3(0, 0, 1), ((-player.fov/2 + ra) * MathF.PI)/180f);
                Vector2 rd = new Vector2(MathF.Cos(prerot.X) * MathF.Sin(prerot.Z), MathF.Cos(prerot.X) * MathF.Cos(prerot.Z));
                //Vector3 newPos = player.transform.location + player.transform.Forward() * 10;
                //DrawLine(new Vector2(c.X + player.transform.location.X, c.Y + player.transform.location.Y), new Vector2(c.X + newPos.X, c.Y + newPos.Y), new Vector3(0,255, 0));
                d = RayToLineInterseaction(ro, rd, p1, p2, 100, out dts);
                
                
                //Debug.WriteLine(dts);
                
                if (d < ri.distance && d > 0)
                {
                    ri2.distance = ri.distance;
                    ri2.gameObject = ri.gameObject;
                    ri2.distToStart = ri.distToStart;
                    ri.distance = d;
                    ri.gameObject = w;
                    ri.distToStart = dts;
                } else if (d > ri.distance && d < ri2.distance)
                {
                    ri2.distance = d;
                    ri2.gameObject = w;
                    ri2.distToStart = dts;
                }

                
                
                
            }
            //Debug.WriteLine(ld);
            return (ri, ri2);
        }

        float RayToLineInterseaction(Vector2 ro, Vector2 rd, Vector2 p1, Vector2 p2, float length, out float dts)
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
                dts = MathF.Abs((MathF.Sqrt((ip.X - p1.X) * (ip.X - p1.X) + (ip.Y - p1.Y) * (ip.Y - p1.Y))));
                //DrawLine(ro, ip, new Vector3(255, 255, 255));
                return dst;
            } else
            {
                dts = 0;
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
                    DrawLine(new Vector2(0, i), new Vector2(x-1, i), new Vector3(15, 128, 255));
                } else
                {
                    float mult = (((float)i / (float)y)) * 255;
                    //DrawFloor(i, mult);
                    DrawLine(new Vector2(0, i), new Vector2(x-1, i), (new Vector3(128,128,128) / y) * mult);
                }
            }
        }

        void DrawFloor(int py, float mult)
        {
            for (int i = 0; i < x; i++)
            {
                //assume texture origin 0,0

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
