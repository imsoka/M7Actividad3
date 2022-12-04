using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        int chargePercent;
        double chargeLevel;
        bool charging;

        public Bateria()
        {
            InitializeComponent();
            chargePercent = 100;
            chargeLevel = getChargeLevel();
            charging = false;
        }

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
            }
        }

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

            graphics.FillRectangle(batteryLevelColor, new Rectangle( 8, 15 + (int) chargeLevel, this.Width - 16, this.Height - 24));
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
            if (chargePercent == 0) return M7Actividad3.Properties.Resources.Agotada;

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
    }
}
