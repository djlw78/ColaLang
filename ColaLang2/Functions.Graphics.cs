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

namespace SplitAndMerge
{
    public static class COLA_GRAPHICS
    {
        public static List<ColaForm> ColaForms = new List<ColaForm>();
        public static Font activeFont = new Font("Arial", 12);
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

            PrintOperation print = new PrintOperation();
            print.values.Add("text", text);
            print.values.Add("x", x);
            print.values.Add("y", y);
            print.values.Add("font", new Font(COLA_GRAPHICS.activeFont, COLA_GRAPHICS.activeFont.Style));
            print.values.Add("brush", Brushes.DarkRed);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(print);
            COLA_GRAPHICS.ColaForms[winID].RefreshNeeded = true;

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

            DRectOperation rect = new DRectOperation();
            rect.values.Add("x", x);
            rect.values.Add("y", y);
            rect.values.Add("width", w);
            rect.values.Add("height", h);
            rect.values.Add("pen", Pens.Black);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(rect);
            COLA_GRAPHICS.ColaForms[winID].RefreshNeeded = true;

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

            FRectOperation rect = new FRectOperation();
            rect.values.Add("x", x);
            rect.values.Add("y", y);
            rect.values.Add("width", w);
            rect.values.Add("height", h);
            rect.values.Add("brush", Brushes.Red);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(rect);
            COLA_GRAPHICS.ColaForms[winID].RefreshNeeded = true;

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

            DLine line = new DLine();
            line.values.Add("x", x);
            line.values.Add("y", y);
            line.values.Add("x1", x1);
            line.values.Add("y1", y1);
            //rect.values.Add("brush", Brushes.Red);

            string[] winParse = window.Split(' ');
            int winID = int.Parse(winParse[1]);

            COLA_GRAPHICS.ColaForms[winID].operations.Add(line);
            COLA_GRAPHICS.ColaForms[winID].RefreshNeeded = true;

            return Variable.EmptyInstance;
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
            Pen a = new Pen(Color.Red);
            a.Width = 10;
            gfx.DrawLine(a, (float)values["x"], (float)values["y"], (float)values["x1"], (float)values["y1"]);
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

        public ColaForm()
        {
            //InitializeComponent();
            font = new Font("Arial", 24, FontStyle.Italic);
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