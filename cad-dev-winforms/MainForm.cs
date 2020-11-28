using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cad_dev_winforms
{
    public partial class MainForm : Form
    {
        private int AxisLength = 15;
        private float commonScaleFactor = 3;
        private bool moving = false;
        private bool rotating = false;
        private int pressedX = 0;
        private int pressedY = 0;
        private float pressedShiftX = 0;
        private float pressedShiftY = 0;
        private float m_xRotate = 0.0f;
        private float m_yRotate = 0.0f;

        private bool DrawTask1 = false;
        private bool DrawTask2 = false;

        public MainForm()
        {
            InitializeComponent();

            this.openGLControl.DrawFPS = true;

            this.openGLControl.OpenGLInitialized += OpenGLControl_OpenGLInitialized;
            this.openGLControl.Resized += OpenGLControl_Resized;
            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;

            this.Size = new Size(800, 600);
        }

        private void OpenGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.ShadeModel(SharpGL.Enumerations.ShadeModel.Smooth);
        }

        private void OpenGLControl_Resized(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(35.0f, (double)Width / (double)Height, 0.01, 1000.0);
            gl.LookAt(0, 30, 140, 0, 0, 0, 0, 1, 0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void OpenGLControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            GLHelper.DrawBackground(gl);

            gl.LineWidth(1.0f);
            gl.PointSize(2.0f);

            gl.Color(0.3f, 0.3f, 0.3f, 1);

            gl.Translate(0, -100 / 5, 0);
            gl.Translate(pressedShiftX / 7, pressedShiftY / 7, 0);

            gl.Scale(commonScaleFactor, commonScaleFactor, commonScaleFactor);

            gl.Rotate(m_xRotate, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotate, 0.0f, 1.0f, 0.0f);

            float cap_size = (float)(AxisLength * 0.03);

            GLHelper.DrawAxis3D(gl, false, false,
                                    0, 0, 0,
                                    (float)(commonScaleFactor * 0.5),
                                    0, (float)AxisLength,
                                    0, (float)AxisLength,
                                    0, (float)AxisLength,
                                    2, cap_size,
                                    true,
                                    true, "ОСЬ X",
                                    true, "ОСЬ Y",
                                    true, "ОСЬ Z");

            gl.Color(1f, 0.0f, 0.0f, 1);

            if (DrawTask1)
            {
                
            }

            if (DrawTask2)
            {

            }

            gl.Flush();
        }


        private void openGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.pressedX = e.X;
            this.pressedY = e.Y;

            if (e.Button == MouseButtons.Right) { moving = true; }
            if (e.Button == MouseButtons.Left) { rotating = true; }
        }

        private void openGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            float deltaX = pressedX - e.X;
            float deltaY = pressedY - e.Y;
            if (moving)
            {
                pressedShiftX = pressedShiftX - deltaX;
                pressedShiftY = pressedShiftY + deltaY;
            }
            if (rotating)
            {
                m_xRotate -= deltaY / 2;
                m_yRotate -= deltaX / 2;
            }
            this.pressedX = e.X;
            this.pressedY = e.Y;
        }

        private void openGLControl_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
            rotating = false;
        }



        private void buttonReset_Click(object sender, EventArgs e)
        {
            this.DrawTask1 = false;
            this.DrawTask2 = false;
        }

        private void buttonPrimitives_Click(object sender, EventArgs e)
        {
            this.DrawTask1 = true;
        }

        private void buttonSTL_Click(object sender, EventArgs e)
        {
            this.DrawTask2 = true;
        }
    }
}
