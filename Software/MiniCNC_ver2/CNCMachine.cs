using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace MiniCNC_ver2
{
    public class CNCMachine 
    {
        public string[] DataReceive;
        public int State;
        public string[] gcode;

        private string gcodeConvert;
        public string[] cncGcode;

        private const int maxCNCGrid = 600;
        private const int maxCNC = 150;
        private const int pixelCNC = maxCNCGrid / maxCNC;
        private const int lineMax = 2;
        public struct Draw
        {
            public double xLast;
            public double yLast;
            public double xNext;
            public double yNext;
            public double I;
            public double J;
            public string[] buff;
        }
        private Draw draw;

        private string convertGcode(string g, double x, double y)
        {
            string gcode = g + " X" + x.ToString() + " Y" + y.ToString() + '\n';
            return gcode;
        }
        public void drawFromGcode(string[] data, Grid main)
        {
            gcodeConvert = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                while (!(data[i].Contains("G0") && data[i].Contains("X")))
                {
                    i++;
                    if (i == data.Length)
                    {
                        cncGcode = gcodeConvert.Split('\n');
                        return;
                    }
                }
                draw.buff = data[i].Split(' ');
                gcodeProcess(main);
            }
            cncGcode = gcodeConvert.Split('\n');
        }
        private void gcodeProcess(Grid main)
        {
            switch (draw.buff[0])
            {
                case "G00":
                    draw.xLast = Convert.ToDouble(draw.buff[1].Replace("X", string.Empty));
                    draw.yLast = Convert.ToDouble(draw.buff[2].Replace("Y", string.Empty));
                    gcodeConvert += convertGcode("G00", draw.xLast, draw.yLast);
                    break;
                case "G01":
                    draw.xNext = Convert.ToDouble(draw.buff[1].Replace("X", string.Empty));
                    draw.yNext = Convert.ToDouble(draw.buff[2].Replace("Y", string.Empty));
                    main.Children.Add(drawLine(draw.xNext, draw.yNext));
                    break;
                case "G02":
                    draw.xNext = Convert.ToDouble(draw.buff[1].Replace("X", string.Empty));
                    draw.yNext = Convert.ToDouble(draw.buff[2].Replace("Y", string.Empty));
                    draw.I = Convert.ToDouble(draw.buff[4].Replace("I", string.Empty));
                    draw.J = Convert.ToDouble(draw.buff[5].Replace("J", string.Empty));
                    drawArcCw(main);
                    break;
                case "G03":
                    draw.xNext = Convert.ToDouble(draw.buff[1].Replace("X", string.Empty));
                    draw.yNext = Convert.ToDouble(draw.buff[2].Replace("Y", string.Empty));
                    draw.I = Convert.ToDouble(draw.buff[4].Replace("I", string.Empty));
                    draw.J = Convert.ToDouble(draw.buff[5].Replace("J", string.Empty));
                    drawArcCcw(main);
                    break;
            }
        }
        private Line drawLine(double xNext, double yNext)
        {
            Line gcodeLine = new Line();
            gcodeLine.Visibility = Visibility.Visible;
            gcodeLine.StrokeThickness = 2;
            gcodeLine.Stroke = System.Windows.Media.Brushes.White;
            gcodeLine.X1 = draw.xLast * pixelCNC;
            gcodeLine.Y1 = maxCNCGrid - draw.yLast * pixelCNC;
            gcodeLine.X2 = xNext * pixelCNC;
            gcodeLine.Y2 = maxCNCGrid - yNext * pixelCNC;
            //CNCGrid.Children.Add(gcodeLine);
            draw.xLast = xNext;
            draw.yLast = yNext;
            gcodeConvert += convertGcode("G01", draw.xLast, draw.yLast);
            return gcodeLine;
        }
        private void drawArcCw(Grid main)
        {
            if (draw.I < -100 || draw.I > 100 || draw.J < -100 || draw.J > 100)
            {
                main.Children.Add(drawLine(draw.xNext, draw.yNext));
                return;
            }
            // declare variable
            double circleX = draw.xLast + draw.I;
            double circleY = draw.yLast + draw.J;
            double xNew;
            double yNew;

            // caculate arc
            double dx = draw.xLast - draw.xNext;
            double dy = draw.yLast - draw.yNext;
            double chord = Math.Sqrt(dx * dx + dy * dy);
            double radius = Math.Sqrt(draw.I * draw.I + draw.J * draw.J);
            double alpha = 2 * Math.Asin(chord / (2 * radius));
            double arc = alpha * radius;
            double beta;

            // sub divide alpha
            int segments = 1;
            if (arc > lineMax)
            {
                segments = Convert.ToInt32(arc / lineMax);
                beta = alpha / segments;
            }
            else
                beta = alpha;

            // caculate current angle
            double currentAngle = Math.Atan2(-draw.J, -draw.I);
            if (currentAngle <= 0)
                currentAngle = currentAngle + 2 * Math.PI;

            // plot arc cw
            double nextAngle = currentAngle;
            for (int segment = 1; segment < segments; segment++)
            {
                nextAngle = nextAngle - beta;
                if (nextAngle < 0)
                    nextAngle = nextAngle + 2 * Math.PI;
                xNew = circleX + radius * Math.Cos(nextAngle);
                yNew = circleY + radius * Math.Sin(nextAngle);
                main.Children.Add(drawLine(xNew, yNew));
            }
            // draw final line
            main.Children.Add(drawLine(draw.xNext, draw.yNext));
        }
        private void drawArcCcw(Grid main)
        {
            if (draw.I < -100 || draw.I > 100 || draw.J < -100 || draw.J > 100)
            {
                main.Children.Add(drawLine(draw.xNext, draw.yNext));
                return;
            }
            // declare variable
            double circleX = draw.xLast + draw.I;
            double circleY = draw.yLast + draw.J;
            double xNew;
            double yNew;

            // caculate arc
            double dx = draw.xLast - draw.xNext;
            double dy = draw.yLast - draw.yNext;
            double chord = Math.Sqrt(dx * dx + dy * dy);
            double radius = Math.Sqrt(draw.I * draw.I + draw.J * draw.J);
            double alpha = 2 * Math.Asin(chord / (2 * radius));
            double arc = alpha * radius;
            double beta;

            // sub divide alpha
            int segments = 1;
            if (arc > lineMax)
            {
                segments = Convert.ToInt32(arc / lineMax);
                beta = alpha / segments;
            }
            else
                beta = alpha;

            // caculate current angle
            double currentAngle = Math.Atan2(-draw.J, -draw.I);
            if (currentAngle <= 0)
                currentAngle = currentAngle + 2 * Math.PI;

            // plot arc cw
            double nextAngle = currentAngle;
            for (int segment = 1; segment < segments; segment++)
            {
                nextAngle = nextAngle + beta;
                if (nextAngle > 2 * Math.PI)
                    nextAngle = nextAngle - 2 * Math.PI;
                xNew = circleX + radius * Math.Cos(nextAngle);
                yNew = circleY + radius * Math.Sin(nextAngle);
                main.Children.Add(drawLine(xNew, yNew));
            }
            // draw final line
            main.Children.Add(drawLine(draw.xNext, draw.yNext));
        }
       /* private void drawLineCNC(double xNext, double yNext)
        {
            Line gcodeLine = new Line();
            gcodeLine.Visibility = Visibility.Visible;
            gcodeLine.StrokeThickness = 2;
            gcodeLine.Stroke = System.Windows.Media.Brushes.Red;
            gcodeLine.X1 = CNCDraw.xLast * pixelCNC;
            gcodeLine.Y1 = maxCNCGrid - CNCDraw.yLast * pixelCNC;
            gcodeLine.X2 = xNext * pixelCNC;
            gcodeLine.Y2 = maxCNCGrid - yNext * pixelCNC;
            CNCGrid.Children.Add(gcodeLine);
            CNCDraw.xLast = xNext;
            CNCDraw.yLast = yNext;
        }
        private void drawFromCNC()
        {
            if (Convert.ToDouble(CNCDraw.buff[2].Replace("Z", string.Empty)) != 0)
            {
                double xNew = Convert.ToDouble(CNCDraw.buff[0].Replace("X", string.Empty));
                double yNew = Convert.ToDouble(CNCDraw.buff[1].Replace("Y", string.Empty));
                drawLineCNC(xNew, yNew);
            }
            if (Convert.ToDouble(CNCDraw.buff[2].Replace("Z", string.Empty)) == 0)
            {
                double xNew = Convert.ToDouble(CNCDraw.buff[0].Replace("X", string.Empty));
                double yNew = Convert.ToDouble(CNCDraw.buff[1].Replace("Y", string.Empty));
                CNCDraw.xLast = xNew;
                CNCDraw.yLast = yNew;
            }
        }*/
    }
}
