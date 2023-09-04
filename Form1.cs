using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WinFormsApp1
{
    public partial class FrmMain : Form
    {

        int xpos = 50;
        float pitch = 0.0f;
        float elevator = 0.0f;
        float ailerons = 0.0f;
        float yaw = 0.0f;
        float roll = 0.0f;
        float altitude = 0.0f;
        float airspeed = 0.0f;
        float throttle = 0.0f;

        float maxAirspeedScalar = 100.0f;
        float airspeedAccel = 0.25f;
        float pitchAccel = 0.01f;
        float rollAccel = 0.01f;
        float altitudeAccel = 10.0f;
        float horizonRollScalar = 0.1f;
        float horizonPitchScalar = 0.1f;



        Rectangle viewport;
        Rectangle dashboard;
        Pt roadVanishingPt = null;
        Pt dashPtLeft = null;
        Pt dashPtRight = null;
        Line horizon;
        Font font = new Font("Lucida Console", 14);
        Pen horizonPen = new Pen(Color.Black, 2.0f);//color,width
        Pen penRed = new Pen(Color.Red, 1.0f);
        Pen penBlack = new Pen(Color.Black, 2.0f);
        Brush brushSky = new SolidBrush(Color.LightSkyBlue);
        Brush brushGround = new SolidBrush(Color.MediumSpringGreen);
        Brush brushBlack = new SolidBrush(Color.Black);
        Brush brushWhite = new SolidBrush(Color.White);
        Brush brushRed = new SolidBrush(Color.Red);


        public FrmMain()
        {
            InitializeComponent();
            viewport = new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 120);
            dashboard = new Rectangle(0, viewport.Height + 1, viewport.Width, 118);
            //controls - dashboard Rect is width x 150
            horizon = new Line(new Pt(viewport.X, viewport.Height / 2), new Pt(viewport.Width, viewport.Height / 2));
            roadVanishingPt = horizon.midpoint();
            altitude = 100.0f;
            throttle = 0.5f;
            airspeed = 50;

        }

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(brushSky, viewport);
            Point[] skyPoints = { new Point(0,0), new Point(this.ClientSize.Width, 0),
                new Point(Convert.ToInt32(horizon.b.x), Convert.ToInt32(horizon.b.y)), new Point(Convert.ToInt32(horizon.a.x), Convert.ToInt32(horizon.a.y)) };
            Point[] groundPoints = { new Point(Convert.ToInt32(horizon.a.x), Convert.ToInt32(horizon.a.y)), new Point(Convert.ToInt32(horizon.b.x), Convert.ToInt32(horizon.b.y)),
                new Point(this.ClientSize.Width, this.ClientSize.Height), new Point(0, this.ClientSize.Height) };

            g.FillPolygon(brushSky, skyPoints);
            g.FillPolygon(brushGround, groundPoints);
            g.DrawRectangle(penRed, viewport);
            g.FillRectangle(brushBlack, dashboard);
            g.DrawRectangle(penRed, dashboard);
            //g.FillRectangle(brushRed, xpos, 50, 100, 100);

            //draw horizon
            g.DrawLine(penBlack, horizon.a.x, horizon.a.y, horizon.b.x, horizon.b.y);

            g.DrawString("Throttle\tElevator\tAileron-Rudder\t", font, brushWhite, 10, dashboard.Y + 20);

            g.DrawString("" + Round(throttle) + "\t\t" + Round(elevator) + "\t\t" +
                Round(ailerons) + "\t", font, brushWhite, 10, dashboard.Y + 40);

            g.DrawString("Speed\t\tPitch\tAlt\tRoll\tYaw\t", font, brushWhite, 10, dashboard.Y + 60);

            g.DrawString("" + Round(airspeed) + "\t\t" + Round(pitch) + "\t" +
                Round(altitude) + "\t" + Round(roll) + "\t" +
                Round(yaw) + "\t", font, brushWhite, 10, dashboard.Y + 80);


        }

        private float Round(float speed)
        {
            if (speed > 0)
            {
                return ((int)(speed * 10.0f + 0.5f)) / 10.0f;
            }
            else
            {
                return ((int)(speed * 10.0f - 0.5f)) / 10.0f;
            }
        }




        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            xpos++;

            if (airspeed < maxAirspeedScalar * throttle)
            {
                airspeed = Math.Min(maxAirspeedScalar * throttle, airspeed + airspeedAccel);
            }
            else if (airspeed > maxAirspeedScalar * throttle)
            {
                airspeed = Math.Max(0.0f, airspeed - airspeedAccel);
            }

            if (Math.Abs(pitch - elevator) < 0.001f)
            {

            }
            else if (pitch < elevator)
            {
                pitch += pitchAccel;
            }
            else if (pitch > elevator)
            {
                pitch -= pitchAccel;
            }

            if (Math.Abs(roll - ailerons) < 0.001f)
            {

            }
            else if (roll < ailerons)
            {
                roll += rollAccel;
            }
            else if (roll > ailerons)
            {
                roll -= rollAccel;
            }
            altitude = Math.Max(0.0f, altitude + altitudeAccel * pitch);



            float dyScalar = 100.0f;
            float day = (-horizonRollScalar * roll + horizonPitchScalar * pitch) * dyScalar;
            float dby = (horizonRollScalar * roll + horizonPitchScalar * pitch) * dyScalar;
            horizon.a.y += day;
            horizon.b.y += dby;


            this.Refresh();

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("KeyDown: " + e.KeyValue.ToString());
            Console.WriteLine("KeyDown: " + e.KeyCode.ToString());
            if (e.KeyCode.ToString() == "Add")
            {
                throttle = Math.Min(throttle + 0.1f, 1.0f); // 1 is max throttle
            }
            if (e.KeyCode.ToString() == "Subtract")
            {
                throttle = Math.Max(0.0f, throttle - 0.1f); // 0 is min throttle
            }
            if (e.KeyCode.ToString() == "Right")
            {
                ailerons = Math.Min(1.0f, ailerons + 0.1f); // 1 is max rudder
            }
            if (e.KeyCode.ToString() == "Left")
            {
                ailerons = Math.Max(-1.0f, ailerons - 0.1f); // -1 is min rudder
            }
            if (e.KeyCode.ToString() == "Up")
            {
                elevator = Math.Min(elevator + 0.1f, 1.0f); // 1 is max elevator
            }
            if (e.KeyCode.ToString() == "Down")
            {
                elevator = Math.Max(-1.0f, elevator - 0.1f); // -1 is min elevator
            }

        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            // Console.WriteLine("KeyUp: " + e.ToString());

        }

    }


    public class Pt
    {
        public float x;
        public float y;

        public Pt(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public int xInt()
        {
            return (int)(x + 0.5f);
        }

        public int yInt()
        {
            return (int)(y + 0.5f);
        }
    }

    public class Line
    {
        public Pt a;
        public Pt b;

        public Line(Pt a, Pt b)
        {
            this.a = a;
            this.b = b;
        }

        public Pt getA()
        {
            return a;
        }
        public Pt getB()
        {
            return b;
        }
        public Pt midpoint()
        {
            return new Pt((a.x + b.x) / 2, (a.y + b.y) / 2);
        }

    }








}