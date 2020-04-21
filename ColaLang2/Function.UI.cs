using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.FreeGlut;
using OpenGL;
using OpenGL.Platform;
namespace SplitAndMerge
{
    class COLA_UI
    {
        public static void Init()
        {
            ParserFunction.RegisterFunction("MsgBox", new MsgBox());
        }

    }

    #region OpenGL
    class GLCreateWindow : ParserFunction
    {
        private static int width = 800, height = 720;
      //  private static ShaderProgram program;
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 3, m_name);

            var title = Utils.GetSafeString(args, 0);
            width = Utils.GetSafeInt(args, 1);
            height = Utils.GetSafeInt(args, 2);

            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow(title);

            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);

           // program = new ShaderProgram(VertexShader, FregmentShader);

          //  program.Use();

            Glut.glutMainLoop();
            //OpenGL.Platform.Window.CreateWindow(title, width, height);
            //while (OpenGL.Platform.Window.Open)
            //{
            //    OpenGL.Platform.Window.HandleEvents();
                
            //}
            return Variable.EmptyInstance;
        }

        private static void OnDisplay()
        {

        }

        private static void OnRenderFrame()
        {
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Glut.glutSwapBuffers();
        }

        public static string VertexShader = @"
in vec3 vertexPosition;

uniform mat4 projection_matrix;
unifrom mat4 view_matrix;
unifrom mat4 model_matrix;

void main(void)
{
    gl_position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);

}

";

        public static string FregmentShader = @"
void main(void)
{
   gl_FragColor = vec4(1, 1, 1, 1);
}


";
    }
    #endregion

    class MsgBox : ParserFunction
    {
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var title = Utils.GetSafeString(args, 0);
            var msg = Utils.GetSafeString(args, 1);

            MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            return Variable.EmptyInstance;
        }
    }
}
