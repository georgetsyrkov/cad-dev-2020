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
        private Color _backGroundColor;

        public MainForm()
        {
            this._backGroundColor = Color.FromArgb(Color.White.ToArgb());

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
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            float R_color = (float)this._backGroundColor.R / (float)255;
            float G_color = (float)this._backGroundColor.G / (float)255;
            float B_color = (float)this._backGroundColor.B / (float)255;

            gl.ClearColor(R_color, G_color, B_color, 1);
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


        private int YMaximum = 15;
        private float commonScaleFactor = 1;

        private float offsetX = 0;
        private float offsetY = 0;

        private float m_xRotate = 0.0f;
        private float m_yRotate = 0.0f;

        private void OpenGLControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.LineWidth(1.0f);
            gl.PointSize(2.0f);

            gl.Color(0.7f, 0.7f, 0.7f, 1);

            gl.Translate(0, -100 / 5, 0);
            gl.Translate(offsetX / 7, offsetY / 7, 0);


            gl.Scale(commonScaleFactor, commonScaleFactor, commonScaleFactor);

            gl.Rotate(m_xRotate, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotate, 0.0f, 1.0f, 0.0f);

            float cap_size = (float)(YMaximum * 0.03);

            GLHelper.DrawAxis3D(gl, false, false,
                                    0, 0, 0,
                                    (float)(commonScaleFactor * 0.5),
                                    0, (float)YMaximum,
                                    0, (float)YMaximum,
                                    0, (float)YMaximum,
                                    2, cap_size,
                                    true,
                                    true, "ОСЬ X",
                                    true, "ОСЬ Y",
                                    true, "ОСЬ Z");

            gl.Flush();
        }
    }
}
