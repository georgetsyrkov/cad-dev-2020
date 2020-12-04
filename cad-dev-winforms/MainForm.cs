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
        private bool zooming = false;
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

            this.openGLControl.OpenGLInitialized += OpenGLControl_OpenGLInitialized; //
            this.openGLControl.Resized += OpenGLControl_Resized;                     // "подписка" на основные события компоненты отрисовки OpenGL графики
            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;               //

            this.Size = new Size(800, 600);
        }

        // Метод первоначальной инициализации
        //
        //
        private void OpenGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.ShadeModel(SharpGL.Enumerations.ShadeModel.Smooth);
        }

        // Событие после изменения размера окна придожения,
        // меняются пропорции площади отрисовки, 
        // поэтому необходимо вносить корректировки в перспективу
        private void OpenGLControl_Resized(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(35.0f, (double)Width / (double)Height, 0.01, 1000.0);
            gl.LookAt(0, 30, 140, 0, 0, 0, 0, 1, 0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        // Самый главный метод, который запускается по событию отрисовки каждого кадра
        //
        //
        private void OpenGLControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            // Рисуем фон
            GLHelper.DrawBackground(gl);

            // Перемещаем координаты общей системы координат
            gl.Translate(0, -100 / 5, 0);
            gl.Translate(pressedShiftX / 7, pressedShiftY / 7, 0);

            // Изменяем масштаб всего
            gl.Scale(commonScaleFactor, commonScaleFactor, commonScaleFactor);

            // Вращение общей системы координат
            gl.Rotate(m_xRotate, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotate, 0.0f, 1.0f, 0.0f);


            // Устанавливаем цвет линий
            gl.Color(0.3f, 0.3f, 0.3f, 1);

            // Отрисовка системы координат
            GLHelper.DrawAxis3D(gl, false, false,
                                    0, 0, 0,
                                    (float)(commonScaleFactor * 0.5),
                                    0, (float)AxisLength,
                                    0, (float)AxisLength,
                                    0, (float)AxisLength,
                                    2, (float)(AxisLength * 0.03),
                                    true,
                                    true, "ОСЬ X",
                                    true, "ОСЬ Y",
                                    true, "ОСЬ Z");


            gl.Color(1f, 0.0f, 0.0f, 1);

            if (DrawTask1)
            {
                // Место кода для отрисовки первого задания
                // ...
                //

                bool lines = true; bool polygons = true;


                #region cylinder
                int ng = 24; int radius = 10; int length = 60;

                if (ng > 127) { ng = 127; }
                double[,] vert = new double[128, 2];

                int c = 0;
                for (int i = 0; i < ng + 1; i++)
                {
                    double seta = (double)i * 360.0 / ng;
                    double vx = Math.Sin((Math.PI * seta / 180.0)) * radius;
                    double vy = Math.Cos((Math.PI * seta / 180.0)) * radius;
                    vert[c, 0] = vx; vert[c, 1] = vy;
                    c += 1;
                }

                if (lines)
                {
                    gl.Color(0.5f, 0.5f, 0.5f);
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int i = 0; i < ng; i++)
                    {
                        gl.Vertex(0, vert[i, 0], vert[i, 1]);
                        gl.Vertex(length, vert[i, 0], vert[i, 1]);
                        gl.Vertex(length, vert[i + 1, 0], vert[i + 1, 1]);
                        gl.Vertex(0, vert[i + 1, 0], vert[i + 1, 1]);
                    }
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int i = 0; i < ng; i++) { gl.Vertex(length, vert[i, 0], vert[i, 1]); }
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int i = 0; i < ng; i++) { gl.Vertex(0, vert[i, 0], vert[i, 1]); }
                    gl.End();
                }

                if (polygons)
                {
                    gl.Color(0.7f, 0.8f, 0.9f);
                    gl.Begin(OpenGL.GL_QUADS);
                    for (int i = 0; i < ng; i++)
                    {
                        gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(0, vert[i, 0], vert[i, 1]);
                        gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(length, vert[i, 0], vert[i, 1]);
                        gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(length, vert[i + 1, 0], vert[i + 1, 1]);
                        gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(0, vert[i + 1, 0], vert[i + 1, 1]);
                    }
                    gl.End();

                    gl.Color(0.7f, 0.8f, 0.9f);
                    gl.Begin(OpenGL.GL_POLYGON);
                    for (int i = 0; i < ng; i++) { gl.Vertex(0, vert[i, 0], vert[i, 1]); }
                    gl.End();

                    gl.Begin(OpenGL.GL_POLYGON);
                    for (int i = ng - 1; i >= 0; i--) { gl.Vertex(length, vert[i, 0], vert[i, 1]); }
                    gl.End();
                }
                #endregion

                gl.Translate(0, 25, 0);

                #region konus
                int kon_ng = 32; int kon_radius1 = 10; int kon_radius2 = 5; int kon_length = 30;

                if (kon_ng > 127) { kon_ng = 127; }
                double[,] kon_vert1 = new double[128, 2];
                double[,] kon_vert2 = new double[128, 2];

                int cc = 0;
                for (int i = 0; i < kon_ng + 1; i++)
                {
                    double seta = (double)i * 360.0 / kon_ng;

                    double v1x = Math.Sin((Math.PI * seta / 180.0)) * kon_radius1;
                    double v1y = Math.Cos((Math.PI * seta / 180.0)) * kon_radius1;
                    kon_vert1[cc, 0] = v1x; kon_vert1[cc, 1] = v1y;

                    double v2x = Math.Sin((Math.PI * seta / 180.0)) * kon_radius2;
                    double v2y = Math.Cos((Math.PI * seta / 180.0)) * kon_radius2;
                    kon_vert2[cc, 0] = v2x; kon_vert2[cc, 1] = v2y;

                    cc += 1;
                }

                if (lines)
                {
                    gl.Color(0.5f, 0.5f, 0.5f);
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int i = 0; i < kon_ng; i++)
                    {
                        gl.Vertex(0, kon_vert1[i, 0], kon_vert1[i, 1]);
                        gl.Vertex(kon_length, kon_vert2[i, 0], kon_vert2[i, 1]);
                        gl.Vertex(kon_length, kon_vert2[i + 1, 0], kon_vert2[i + 1, 1]);
                        gl.Vertex(0, kon_vert1[i + 1, 0], kon_vert1[i + 1, 1]);
                    }
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int i = 0; i < kon_ng; i++) { gl.Vertex(0, kon_vert1[i, 0], kon_vert1[i, 1]); }
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int i = 0; i < kon_ng; i++) { gl.Vertex(kon_length, kon_vert2[i, 0], kon_vert2[i, 1]); }
                    gl.End();
                }

                if (polygons)
                {
                    gl.Color(0.7f, 0.8f, 0.9f);
                    gl.Begin(OpenGL.GL_QUADS);
                    for (int i = 0; i < kon_ng; i++)
                    {
                        gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(0, kon_vert1[i, 0], kon_vert1[i, 1]);
                        gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(kon_length, kon_vert2[i, 0], kon_vert2[i, 1]);
                        gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(kon_length, kon_vert2[i + 1, 0], kon_vert2[i + 1, 1]);
                        gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(0, kon_vert1[i + 1, 0], kon_vert1[i + 1, 1]);
                    }
                    gl.End();

                    gl.Color(0.7f, 0.8f, 0.9f);
                    gl.Begin(OpenGL.GL_POLYGON);
                    for (int i = 0; i < kon_ng; i++) { gl.Vertex(0, kon_vert1[i, 0], kon_vert1[i, 1]); }
                    gl.End();

                    gl.Begin(OpenGL.GL_POLYGON);
                    for (int i = kon_ng - 1; i >= 0; i--) { gl.Vertex(kon_length, kon_vert2[i, 0], kon_vert2[i, 1]); }
                    gl.End();
                }
                #endregion

                gl.Translate(0, 25, 0);

                #region paral
                int lengthX = 5; int lengthY = 20; int lengthZ = 10;
                if (polygons)
                {
                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(0, lengthY / 2, lengthZ / 2);
                    gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(0, -lengthY / 2, lengthZ / 2);
                    gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(0, -lengthY / 2, -lengthZ / 2);
                    gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(0, lengthY / 2, -lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(lengthX, lengthY / 2, lengthZ / 2);
                    gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(lengthX, lengthY / 2, -lengthZ / 2);
                    gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(lengthX, -lengthY / 2, -lengthZ / 2);
                    gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(lengthX, -lengthY / 2, lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(0, lengthY / 2, lengthZ / 2);
                    gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(lengthX, lengthY / 2, lengthZ / 2);
                    gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(lengthX, -lengthY / 2, lengthZ / 2);
                    gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(0, -lengthY / 2, lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(lengthX, lengthY / 2, -lengthZ / 2);
                    gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(0, lengthY / 2, -lengthZ / 2);
                    gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(0, -lengthY / 2, -lengthZ / 2);
                    gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(lengthX, -lengthY / 2, -lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(0, lengthY / 2, lengthZ / 2);
                    gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(0, lengthY / 2, -lengthZ / 2);
                    gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(lengthX, lengthY / 2, -lengthZ / 2);
                    gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(lengthX, lengthY / 2, lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_QUADS);
                    gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(0, -lengthY / 2, lengthZ / 2);
                    gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(lengthX, -lengthY / 2, lengthZ / 2);
                    gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(lengthX, -lengthY / 2, -lengthZ / 2);
                    gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(0, -lengthY / 2, -lengthZ / 2);
                    gl.End();
                }

                if (lines)
                {
                    gl.Color(0.5f, 0.5f, 0.5f);

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Vertex(0, lengthY / 2, lengthZ / 2);
                    gl.Vertex(0, lengthY / 2, -lengthZ / 2);
                    gl.Vertex(0, -lengthY / 2, -lengthZ / 2);
                    gl.Vertex(0, -lengthY / 2, lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Vertex(lengthX, lengthY / 2, lengthZ / 2);
                    gl.Vertex(lengthX, lengthY / 2, -lengthZ / 2);
                    gl.Vertex(lengthX, -lengthY / 2, -lengthZ / 2);
                    gl.Vertex(lengthX, -lengthY / 2, lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Vertex(0, lengthY / 2, lengthZ / 2);
                    gl.Vertex(lengthX, lengthY / 2, lengthZ / 2);
                    gl.Vertex(lengthX, -lengthY / 2, lengthZ / 2);
                    gl.Vertex(0, -lengthY / 2, lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Vertex(0, lengthY / 2, -lengthZ / 2);
                    gl.Vertex(lengthX, lengthY / 2, -lengthZ / 2);
                    gl.Vertex(lengthX, -lengthY / 2, -lengthZ / 2);
                    gl.Vertex(0, -lengthY / 2, -lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Vertex(0, lengthY / 2, lengthZ / 2);
                    gl.Vertex(lengthX, lengthY / 2, lengthZ / 2);
                    gl.Vertex(lengthX, lengthY / 2, -lengthZ / 2);
                    gl.Vertex(0, lengthY / 2, -lengthZ / 2);
                    gl.End();

                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Vertex(0, -lengthY / 2, lengthZ / 2);
                    gl.Vertex(lengthX, -lengthY / 2, lengthZ / 2);
                    gl.Vertex(lengthX, -lengthY / 2, -lengthZ / 2);
                    gl.Vertex(0, -lengthY / 2, -lengthZ / 2);
                    gl.End();
                }
                #endregion

                gl.Translate(0, 25, 0);

                #region sphere

                int sph_ng = 24; float sph_radius = 10;

                List<double[,]> slices = new List<double[,]>();

                double dPhi = 2 * Math.PI / sph_ng;
                double dPsi = 2 * Math.PI / sph_ng;
                for (int i = 0; i <= sph_ng; i++)
                {
                    double[,] slice = new double[sph_ng + 1, 3];

                    double Psi = -Math.PI + dPsi * i;

                    for (int j = 0; j <= sph_ng; ++j)
                    {
                        double Phi = dPhi * j;

                        double x = (sph_radius * Math.Cos(Phi));
                        double y = (sph_radius * Math.Sin(Phi)) * Math.Sin(Psi);
                        double z = (sph_radius * Math.Sin(Phi)) * Math.Cos(Psi);
                        slice[j, 0] = x; slice[j, 1] = y; slice[j, 2] = z;
                    }
                    slices.Add(slice);
                }

                if (lines)
                {
                    gl.Color(0.5f, 0.5f, 0.5f);
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int i = 0; i < slices.Count - 1; i++)
                    {
                        for (int j = 0; j < sph_ng; ++j)
                        {
                            gl.Vertex(slices[i][j, 0], slices[i][j, 1], slices[i][j, 2]);
                            gl.Vertex(slices[i + 1][j, 0], slices[i + 1][j, 1], slices[i + 1][j, 2]);
                            gl.Vertex(slices[i + 1][j + 1, 0], slices[i + 1][j + 1, 1], slices[i + 1][j + 1, 2]);
                            gl.Vertex(slices[i][j + 1, 0], slices[i][j + 1, 1], slices[i][j + 1, 2]);
                        }
                    }
                    gl.End();
                }

                if (polygons)
                {
                    gl.Color(0.7f, 0.8f, 0.9f);
                    gl.Begin(OpenGL.GL_QUADS);
                    for (int i = 0; i < slices.Count - 1; i++)
                    {
                        for (int j = 0; j < sph_ng; ++j)
                        {
                            gl.Color(0.71f, 0.81f, 0.91f); gl.Vertex(slices[i][j, 0], slices[i][j, 1], slices[i][j, 2]);
                            gl.Color(0.73f, 0.83f, 0.93f); gl.Vertex(slices[i + 1][j, 0], slices[i + 1][j, 1], slices[i + 1][j, 2]);
                            gl.Color(0.76f, 0.86f, 0.96f); gl.Vertex(slices[i + 1][j + 1, 0], slices[i + 1][j + 1, 1], slices[i + 1][j + 1, 2]);
                            gl.Color(0.79f, 0.89f, 0.99f); gl.Vertex(slices[i][j + 1, 0], slices[i][j + 1, 1], slices[i][j + 1, 2]);
                        }
                    }
                    gl.End();
                }
                #endregion
            }

            if (DrawTask2)
            {
                // Место кода для отрисовки второго задания
                // ...
                //
            }

            gl.Flush();
        }

        #region Методы обработки событий манипулятора "мышь"
        private void openGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.pressedX = e.X;
            this.pressedY = e.Y;

            if (e.Button == MouseButtons.Right) { moving = true; }
            if (e.Button == MouseButtons.Left) { rotating = true; }
            if (e.Button == MouseButtons.Middle) { zooming = true; }
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
            if (zooming)
            {
                commonScaleFactor += deltaY * 0.01f;
            }
            this.pressedX = e.X;
            this.pressedY = e.Y;
        }

        private void openGLControl_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
            rotating = false;
            zooming = false;
        }
        #endregion

        #region Включение/отключение отрисовки заданий
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
        #endregion
    }
}
