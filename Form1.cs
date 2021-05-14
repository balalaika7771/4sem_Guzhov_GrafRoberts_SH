using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GRAF2_ROBERTS
{

	public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
			pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			timer1.Interval = 10;
			timer1.Start();
		}

		TCube3D cube = new TCube3D("C:\\Qt\\cube.obj");//для создания кубика нам нужно дать ему путь к файлу из которого он будет читать координаты своих вершин
		double teta = 1, phi = 1, zer = 1;   //углы для поворота
		double RRRX = -200, RRRY = 100;//перемещение кубика по экрану 
		double zoom = 1;//увеличение кубика
		void Form1_KeyDown(object sender, KeyEventArgs e)
        {
		 if(e.KeyValue == (char)Keys.Up)//тут просто реакция на нажатие клавиш
            {
				teta += (Math.PI / (360));
			}
			if (e.KeyValue == (char)Keys.Down)
			{
				teta -= (Math.PI / (360));
			}

			if (e.KeyValue == (char)Keys.Left)
			{
				phi -= (Math.PI / (360));
			}
			if (e.KeyValue == (char)Keys.Right)
			{
				phi += (Math.PI / (360));
			}

			if (e.KeyValue == (char)Keys.M)
			{
				zer += (Math.PI / (360));
			}
			if (e.KeyValue == (char)Keys.N)
			{
				zer -= (Math.PI / (360));
			}


			if (e.KeyValue == (char)Keys.W)
			{
				RRRX += 10;
			}
			if (e.KeyValue == (char)Keys.S)
			{
				RRRX -= 10;
			}

			if (e.KeyValue == (char)Keys.D)
			{
				RRRY += 10;
			}
			if (e.KeyValue == (char)Keys.A)
			{
				RRRY -= 10;
			}


			if (e.KeyValue == (char)Keys.Z)
			{
				zoom += 0.1;
			}
			if (e.KeyValue == (char)Keys.X)
			{
				zoom -= 0.1;
			}
		}

       
		private void timer1_Tick(object sender, EventArgs e)
        {
			Graphics g = Graphics.FromImage(pictureBox1.Image);
			SolidBrush b = new SolidBrush(Color.White);
			g.FillRectangle(b, 0, 0, pictureBox1.Width, pictureBox1.Height);//каждую итерацию эта команда закрашивает окно полностью, чтобы нарисовать новый квадрат и чтобы получалась анимация
			Pen p = new Pen(Color.Red);//создание кисти
			cube.view_transformation(phi, teta, zer, RRRX, RRRY, zoom);//изменили положение кубика 
			for (int i = 0; i < cube.trian.Count; i = i + 3)//идём по массиву который хранит координаты полигонов, каждый шаг - 3 потому что вершин у полигона всего 3
			{
				//загружаем наш полигон в алгоритм робертса, если плоскость полигона будет лежать к нам лицом то будем её рисовать
				if (proverka(cube.WN[cube.trian[i]].getx(), cube.WN[cube.trian[i]].gety(), cube.WN[cube.trian[i]].getz(), cube.WN[cube.trian[i + 1]].getx(), cube.WN[cube.trian[i + 1]].gety(), cube.WN[cube.trian[i + 1]].getz(), cube.WN[cube.trian[i + 2]].getx(), cube.WN[cube.trian[i + 2]].gety(), cube.WN[cube.trian[i + 2]].getz(), cube.pointw.getx(), cube.pointw.gety(), cube.pointw.getz()))
				{
					//рисуем только две линии из треугольника - полигона, мы изначально знаем(потому что мы сами забивали в cube.objкак нужно рисовать полигоны) что первые две линии идут по контуру кубика а третья пересикает грань кубика поперёк,
					g.DrawLine(p, Convert.ToInt32(cube.v[cube.trian[i]].getx()), Convert.ToInt32(cube.v[cube.trian[i]].gety()), Convert.ToInt32(cube.v[cube.trian[i + 1]].getx()), Convert.ToInt32(cube.v[cube.trian[i + 1]].gety()));
					g.DrawLine(p, Convert.ToInt32(cube.v[cube.trian[i + 1]].getx()), Convert.ToInt32(cube.v[cube.trian[i + 1]].gety()), Convert.ToInt32(cube.v[cube.trian[i + 2]].getx()), Convert.ToInt32(cube.v[cube.trian[i + 2]].gety()));
			}
			}
			pictureBox1.Invalidate();
		}

		public bool proverka(double v1x, double v1y, double v1z, double v2x, double v2y, double v2z, double v3x, double v3y, double v3z, double wx, double wy, double wz)
		{
			double Vec1X = v1x - v2x;
			double Vec2X = v3x - v2x;
			double Vec1Y = v1y - v2y;
			double Vec2Y = v3y - v2y;
			double Vec1Z = v1z - v2z;
			double Vec2Z = v3z - v2z;

			double A = Vec1Y * Vec2Z - Vec2Y * Vec1Z;
			double B = Vec1Z * Vec2X - Vec2Z * Vec1X;
			double C = Vec1X * Vec2Y - Vec2X * Vec1Y;
			double D = -(A * v1x + B * v1y + C * v1z);

			if ((A * wx + B * wy + C * wz + D) < 0)
			{
				A = A * -1;
				B = B * -1;
				C = C * -1;
				D = D * -1;
			}
			if (A * 0 + B * 0 + C * 9999999999 + D > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	
	
	}


	class TPoint3D
	{
		private double x, y, z;

		public TPoint3D(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public void setx(double value) { x = value; }
		public void sety(double value) { y = value; }
		public void setz(double value) { z = value; }
		public void set(double value1, double value2, double value3)
		{
			x = value1; y = value2; z = value3;
		}

		public double getx() { return x; }
		public double gety() { return y; }
		public double getz() { return z; }

	};

	class TPoint
	{
		private double x, y;

		public void setx(double value) { x = value; }
		public void sety(double value) { y = value; }
		public double getx() { return x; }
		public double gety() { return y; }
	};

	class TCube3D
	{
		//в w из cube.obj записываем координаты вершин
		//мы знаем в каком порядке мы из записывали и в каком они тогда будум в w
		//поэтому в trian из cube.obj мы записываем НОМЕРА вершин которые храняться в w по которым можно нарисовать кубик 
		public List<TPoint3D> w = new List<TPoint3D>();  //мировые координаты
		public List<TPoint> v = new List<TPoint>();  //видиовые координаты 
		public List<int> trian = new List<int>();
		//далее мы каждую вершину из w умножаем на матрицы поворотв, мастшабирования, перемещения и получаем
		//получаные точки записываем в WN и проэцируем на плоскость z записывая уже двумерные точки в v
		//pointw это центр кубика, он нужен для проверки робертса
		

		public TPoint3D pointw;
		public List<TPoint3D> WN = new List<TPoint3D>();

		public TCube3D(string file_name)
		{
			String str;
			StreamReader sr = new StreamReader(file_name);
			str = sr.ReadLine();
			while (str != null)
			{
				if (str.Length >= 1)
				{
					if (str[0] == 'v')//если v то далее будем читать вершину кубика
					{
						int x, y, z;


						x = Int16.Parse(sr.ReadLine());
						y = Int16.Parse(sr.ReadLine());
						z = Int16.Parse(sr.ReadLine());
						TPoint3D point = new TPoint3D(x, y, z);
						w.Add(point);
						WN.Add(point);
					}
					if (str[0] == 'k')//если k то далее будем читать номера вершин по которым нужно нарисовать треугольник 
					{
						int p1, p2, p3;
						p1 = Int16.Parse(sr.ReadLine());
						p2 = Int16.Parse(sr.ReadLine());
						p3 = Int16.Parse(sr.ReadLine());

						trian.Add(p1);
						trian.Add(p2);
						trian.Add(p3);

					}
				}
				str = sr.ReadLine();
			}
			//close the file
			sr.Close();

			for (int i = 0; i < w.Count; i++)
			{
				TPoint op = new TPoint();

				op.setx(0);
				op.sety(0);
				v.Add(op);
			}
		}

		public void view_transformation(double LX, double LY, double LZ, double RRRX, double RRRY, double zoom)
		{
			//каждый кадр мы берём кубик который считали из файла и изменяем его по этим параметрам 
			double KX1, KY1, KZ1;
			double KX2, KY2, KZ2;
			double KX3, KY3, KZ3;
			for (int i = 0; i < w.Count; i++)
			{//берём по очереди все вершины кубика

				// умножаем на матрицу поворота X
				KX1 = w[i].getx();
				KY1 = w[i].gety() * Math.Cos(LX) + w[i].getz() * Math.Sin(LX);
				KZ1 = -w[i].gety() * Math.Sin(LX) + w[i].getz() * Math.Cos(LX);

				// умножаем на матрицу поворота Y
				KX2 = KX1 * Math.Cos(LY) - KZ1 * Math.Sin(LY);
				KY2 = KY1;
				KZ2 = KX1 * Math.Sin(LY) + KZ1 * Math.Cos(LY);

				// умножаем на матрицу поворота Z
				KX3 = KX2 * Math.Cos(LZ) + KY2 * Math.Sin(LZ);
				KY3 = -KX2 * Math.Sin(LZ) + KY2 * Math.Cos(LZ);
				KZ3 = KZ2;

				KX3 += RRRX;// перемещаем кубик в просранстве 
				KY3 += RRRY;

				KX3 *= zoom;// изменяем размер
				KY3 *= zoom;
				KZ3 *= zoom;

				TPoint3D NN = new TPoint3D(KX3, KY3, KZ3);
				WN[i] = NN;
				v[i].setx(KX3 * (-Math.Sin(0)) + KY3 * (Math.Cos(0)));
				v[i].sety(KX3 * (-Math.Cos(0) * Math.Cos(0)) - KY3 * (Math.Cos(0) * Math.Sin(0)) + KZ3 * (Math.Sin(0)));
			}

			KX1 = 10; // нам по тем же параметрам нужно переместить точку центра кубика
			KY1 = 10 * Math.Cos(LX) + 10 * Math.Sin(LX);
			KZ1 = -10 * Math.Sin(LX) + 10 * Math.Cos(LX);


			KX2 = KX1 * Math.Cos(LY) - KZ1 * Math.Sin(LY);
			KY2 = KY1;
			KZ2 = KX1 * Math.Sin(LY) + KZ1 * Math.Cos(LY);


			KX3 = KX2 * Math.Cos(LZ) + KY2 * Math.Sin(LZ);
			KY3 = -KX2 * Math.Sin(LZ) + KY2 * Math.Cos(LZ);
			KZ3 = KZ2;

			KX3 += RRRX;// перемещаем кубик в просранстве 
			KY3 += RRRY;

			KX3 *= zoom;// изменяем размер
			KY3 *= zoom;
			KZ3 *= zoom;

			TPoint3D N = new TPoint3D(KX3, KY3, KZ3);
			pointw = N;

		}
	};
}
