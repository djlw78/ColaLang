using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace SplitAndMerge
{
    public static class COLA_GRAPHICS
    {
        public static List<ColaForm> ColaForms = new List<ColaForm>();
        public static Font activeFont = new Font("Arial", 12);
        public static Dictionary<string, Image> images = new Dictionary<string, Image>();
        public static Color brcolor = Color.Black;
        public static Color lncolor = Color.Black;
        public static float brwidth = 1;

        public static void HandleWindow()
        {
            ColaForm cf = ColaForms.Last();
            Application.Run(cf);
        }
    }

    class GraphicsCreateObject : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 3, m_name);

            var title = Utils.GetSafeString(args, 0);
            var width = Utils.GetSafeInt(args, 1);
            var height = Utils.GetSafeInt(args, 2);

            ColaForm cf = new ColaForm();
            cf.Width = width;
            cf.Height = height;
            cf.Text = title;
            cf.ID = COLA_GRAPHICS.ColaForms.Count;
            COLA_GRAPHICS.ColaForms.Add(cf);
            Thread cwt = new Thread(new ThreadStart(COLA_GRAPHICS.HandleWindow));
            cwt.Start();

            return new Variable(cf.ToString());
        }
    }

    class GraphicsDrawText : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 4, m_name);

            var window = Utils.GetSafeString(args, 0);
            var text = Utils.GetSafeString(args, 1);
            var x = Utils.GetSafeFloat(args, 2);
            var y = Utils.GetSafeFloat(args, 3);

            Brush b = new SolidBrush(COLA_GRAPHICS.brcolor);

            PrintOperation print = new PrintOperation();
            print.values.Add("text", text);
            print.values.Add("x", x);
            print.values.Add("y", y);
            print.values.Add("font", new Font(COLA_GRAPHICS.activeFont, COLA_GRAPHICS.activeFont.Style));
            print.values.Add("brush", b);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(print);

            return Variable.EmptyInstance;
        }
    }

    class GraphicsDrawRect : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 4, m_name);

            var window = Utils.GetSafeString(args, 0);
            var x = Utils.GetSafeFloat(args, 1);
            var y = Utils.GetSafeFloat(args, 2);
            var w = Utils.GetSafeFloat(args, 3);
            var h = Utils.GetSafeFloat(args, 4);

            Pen p = new Pen(COLA_GRAPHICS.lncolor);
            p.Width = COLA_GRAPHICS.brwidth;

            DRectOperation rect = new DRectOperation();
            rect.values.Add("x", x);
            rect.values.Add("y", y);
            rect.values.Add("width", w);
            rect.values.Add("height", h);
            rect.values.Add("pen", p);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(rect);

            return Variable.EmptyInstance;
        }
    }
    class GraphicsFillRect : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 4, m_name);

            var window = Utils.GetSafeString(args, 0);
            var x = Utils.GetSafeFloat(args, 1);
            var y = Utils.GetSafeFloat(args, 2);
            var w = Utils.GetSafeFloat(args, 3);
            var h = Utils.GetSafeFloat(args, 4);

            Brush b = new SolidBrush(COLA_GRAPHICS.brcolor);

            FRectOperation rect = new FRectOperation();
            rect.values.Add("x", x);
            rect.values.Add("y", y);
            rect.values.Add("width", w);
            rect.values.Add("height", h);
            rect.values.Add("brush", b);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(rect);

            return Variable.EmptyInstance;
        }
    }

    class GraphicsDrawLine : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 4, m_name);

            var window = Utils.GetSafeString(args, 0);
            var x = Utils.GetSafeFloat(args, 1);
            var y = Utils.GetSafeFloat(args, 2);
            var x1 = Utils.GetSafeFloat(args, 3);
            var y1 = Utils.GetSafeFloat(args, 4);

            Pen p = new Pen(COLA_GRAPHICS.lncolor);
            p.Width = COLA_GRAPHICS.brwidth;

            DLine line = new DLine();
            line.values.Add("x", x);
            line.values.Add("y", y);
            line.values.Add("x1", x1);
            line.values.Add("y1", y1);
            line.values.Add("pen", p);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(line);

            return Variable.EmptyInstance;
        }
    }

    class GraphicsDrawImage : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 6, m_name);

            var window = Utils.GetSafeString(args, 0);
            var img = Utils.GetSafeString(args, 1);
            var x = Utils.GetSafeFloat(args, 2);
            var y = Utils.GetSafeFloat(args, 3);
            var w = Utils.GetSafeFloat(args, 4);
            var h = Utils.GetSafeFloat(args, 5);

            DrawImageOperation image = new DrawImageOperation();
            image.values.Add("x", x);
            image.values.Add("y", y);
            image.values.Add("w", w);
            image.values.Add("h", h);
            image.values.Add("image", img);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(image);

            return Variable.EmptyInstance;
        }
    }

    class GraphicsRefresh : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var window = Utils.GetSafeString(args, 0);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].RefreshNeeded = true;

            return Variable.EmptyInstance;
        }
    }

    class GraphicsClear : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var window = Utils.GetSafeString(args, 0);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Clear();

            return Variable.EmptyInstance;
        }
    }

    class GraphicsSetLineColor : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 3, m_name);

            var r = Utils.GetSafeInt(args, 0);
            var g = Utils.GetSafeInt(args, 1);
            var b = Utils.GetSafeInt(args, 2);

            COLA_GRAPHICS.lncolor = Color.FromArgb(255,r,g,b);

            return Variable.EmptyInstance;
        }
    }

    class GraphicsSetLineWidth : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var w = Utils.GetSafeFloat(args, 0);

            COLA_GRAPHICS.brwidth = w;

            return Variable.EmptyInstance;
        }
    }

    class GraphicsSetFillColor : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 3, m_name);

            var r = Utils.GetSafeInt(args, 0);
            var g = Utils.GetSafeInt(args, 1);
            var b = Utils.GetSafeInt(args, 2);

            COLA_GRAPHICS.brcolor = Color.FromArgb(255, r, g, b);

            return Variable.EmptyInstance;
        }
    }
    class GraphicsLoadImage : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var path = Utils.GetSafeString(args, 0);
            var name = Utils.GetSafeString(args, 1);

            Image i = Image.FromFile(path);
            COLA_GRAPHICS.images.Add(name, i);

            return Variable.EmptyInstance;
        }
    }

    class GraphicsGetMouseEvent : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);

            var win = Utils.GetSafeString(args, 0);

            string[] winParse = win.Split(' ');
            int winID = int.Parse(winParse[1]);

            List<object> output = new List<object>();
            output.Add(COLA_GRAPHICS.ColaForms[winID].Button);
            output.Add(COLA_GRAPHICS.ColaForms[winID].LastClickX);
            output.Add(COLA_GRAPHICS.ColaForms[winID].LastClickY);

            if (output[0] != "")
            {
                COLA_GRAPHICS.ColaForms[winID].Button = "";
                COLA_GRAPHICS.ColaForms[winID].LastClickX = 0;
                COLA_GRAPHICS.ColaForms[winID].LastClickY = 0;
            }

            return new Variable(output);
        }
    }

    public class FormOperation
    {
        public Dictionary<string, object> values = new Dictionary<string, object>();
        public int ID = 0;

        public virtual void Run(Graphics gfx)
        {

        }
    }

	#region Operations

	public class PrintOperation : FormOperation
    {
        public override void Run(Graphics gfx)
        {
            gfx.DrawString((string)values["text"], (Font)values["font"], (Brush)values["brush"], (float)values["x"], (float)values["y"]);
            base.Run(gfx);
        }
    }
    public class DRectOperation : FormOperation
    {
        public override void Run(Graphics gfx)
        {
            gfx.DrawRectangle((Pen)values["pen"], (float)values["x"], (float)values["y"], (float)values["width"], (float)values["height"]);
            base.Run(gfx);
        }
    }

    public class FRectOperation : FormOperation
    {
        public override void Run(Graphics gfx)
        {
            gfx.FillRectangle((Brush)values["brush"], (float)values["x"], (float)values["y"], (float)values["width"], (float)values["height"]);
            base.Run(gfx);
        }
    }

    public class DLine : FormOperation
    {
        public override void Run(Graphics gfx)
        {
            gfx.DrawLine((Pen)values["pen"], (float)values["x"], (float)values["y"], (float)values["x1"], (float)values["y1"]);
            base.Run(gfx);
        }
    }
    public class DrawImageOperation : FormOperation
    {
        public override void Run(Graphics gfx)
        {
            gfx.DrawImage(COLA_GRAPHICS.images[(string)values["image"]], (float)values["x"], (float)values["y"], (float)values["w"], (float)values["h"]);
            base.Run(gfx);
        }
    }
    #endregion

    public class ColaForm : Form
    {
        Font font;
        public int ID;
        public bool RefreshNeeded = false;
        public List<FormOperation> operations = new List<FormOperation>();
        public System.Windows.Forms.Timer timer;
        public float LastClickX = 0;
        public float LastClickY = 0;
        public string Button = "";

        public ColaForm()
        {
            this.MouseClick += mouseClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5;
            timer.Tick += new EventHandler(Tick);
            timer.Enabled = true;

            base.OnLoad(e);
        }

        void Tick(Object myObject, EventArgs myEventArgs)
        {
            if (RefreshNeeded)
            {
                RefreshNeeded = false;
                Refresh();
            }
        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Button = "LEFT";
            else if (e.Button == MouseButtons.Right)
                Button = "RIGHT";
            else if (e.Button == MouseButtons.Middle)
                Button = "MIDDLE";

            LastClickX = e.X;
            LastClickY = e.Y;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics gfx = e.Graphics;

            foreach (FormOperation operation in operations)
            {
                operation.Run(gfx);
            }
        }
        public override string ToString()
        {
            return "COLA_WINDOW " + ID;
        }
    }
}