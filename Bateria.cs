using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M7Actividad3
{
    [ToolboxBitmap(typeof(Bateria), "base.png")]
    [Description("Muestra el nivel de carga de una batería")]
    public partial class Bateria : UserControl
    {
        private const int BATTERY_FULL = 0;
        private const int BATTERY_EMPTY = 209;
        private const int BATTERY_LEVEL_X_START_POSITION = 8;
        private const int BATTERY_LEVEL_Y_START_POSITION = 15;
        private const int BATTERY_LEVEL_X_END_POSITION = 16;
        private const int BATTERY_LEVEL_Y_END_POSITION = 24;
        int chargePercent;
        double chargeLevel;
        bool charging;
        bool dragging;
        int startDragXLocation;
        int startDragYLocation;

        public class Argumentos : EventArgs
        {
            int chargePercent;

            public Argumentos(int chargePercent)
            {
                this.chargePercent = chargePercent;
            }

            public int Valor
            { 
                get 
                { 
                    return chargePercent;
                }
                set
                {
                    chargePercent = value;
                }
            }
        }
        public delegate void ChangeBatteryLevel(Object sender, Argumentos argumentos);
        public event ChangeBatteryLevel BatteryLevelChanged;

        public Bateria()
        {
            InitializeComponent();
            chargeLevel = getChargeLevel();
            dragging = false;
        }

        [
            Description("Porcentaje de carga de la batería"),
            DefaultValue(0)
        ]
        public int Porcentaje
        {
            get
            {
                return chargePercent;
            }
            set
            {
                chargePercent = value;
                chargeLevel = getChargeLevel();
                Invalidate();
                ChangeBatteryLevel evento = this.BatteryLevelChanged;
                if(evento != null)
                {
                    evento(this, new Argumentos(chargePercent));
                }
            }
        }

        [
            Description("Establece si la batería está cargando o no"),
            DefaultValue(false)
        ]
        public bool Cargando
        {
            get
            {
                return charging;
            }
            set
            {
                charging = value;
                Invalidate();
            }
        }



        protected override void OnPaint(PaintEventArgs e)
        {   
            base.OnPaint(e);
            Bitmap image = getBatteryImage();
            Brush batteryLevelColor = getIndicatorColor();

            Graphics graphics = e.Graphics;
            this.DoubleBuffered = true;

            graphics.FillRectangle(batteryLevelColor, new Rectangle(
                BATTERY_LEVEL_X_START_POSITION, 
                BATTERY_LEVEL_Y_START_POSITION + (int) chargeLevel, 
                this.Width - BATTERY_LEVEL_X_END_POSITION, 
                this.Height - BATTERY_LEVEL_Y_END_POSITION));
            graphics.FillRectangle(Brushes.White, new Rectangle( 0, this.Height - 8, this.Width, this.Height - 30));

            graphics.DrawImage(image, 0, 0, this.Width, this.Height);

        }

        private void Bateria_Layout(object sender, LayoutEventArgs e)
        {
            if(e.AffectedProperty == "Bounds")
            {
                this.Width = (int) (this.Height / 2);
                Invalidate();
            }
        }

        private Bitmap getBatteryImage()
        {
            if (charging) return M7Actividad3.Properties.Resources.Cargando;
            if (Porcentaje == 0) return M7Actividad3.Properties.Resources.Agotada;

            return M7Actividad3.Properties.Resources.Base;
        }

        private Brush getIndicatorColor()
        {
            if (Porcentaje > 60) return Brushes.Green;
            if (Porcentaje > 30) return Brushes.Yellow;
            return Brushes.Red;
        }

        private double getChargeLevel()
        {
            double unit = (double) BATTERY_EMPTY / 100;
            return (double) BATTERY_EMPTY - (unit * chargePercent);
        }

        private bool isInsideBattery(int X, int Y)
        {
            if (
                X >= BATTERY_LEVEL_X_START_POSITION &&
                X <= this.Width - BATTERY_LEVEL_X_END_POSITION &&
                Y >= BATTERY_LEVEL_Y_START_POSITION &&
                Y <= BATTERY_LEVEL_Y_START_POSITION + BATTERY_EMPTY
            )
            {
                return true;
            }
                return false;
        }

        private int getBatteryPercentageByPosition(double Y)
        {
            Y = (double)(Y - BATTERY_LEVEL_Y_START_POSITION);

            double newPercent = 100.00 - (Y / ((double)BATTERY_EMPTY / 100.00));
            return (int)newPercent;
        }

        private bool mouseMoved(int X, int Y)
        {
            if(startDragXLocation != X || startDragYLocation != Y)
            {
                return true;
            }

            return false;
        }

        private void Bateria_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isInsideBattery(e.Location.X, e.Location.Y)) return;

            Porcentaje = getBatteryPercentageByPosition(e.Location.Y);
        }

        private void Bateria_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isInsideBattery(e.Location.X, e.Location.Y)) return;

            startDragXLocation = e.Location.X;
            startDragYLocation = e.Location.Y;
        }

        private void Bateria_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isInsideBattery(e.Location.X, e.Location.Y)) return;
            if (!mouseMoved(e.Location.X, e.Location.Y)) return;

            Porcentaje = getBatteryPercentageByPosition(e.Location.Y);
        }
    }
}
