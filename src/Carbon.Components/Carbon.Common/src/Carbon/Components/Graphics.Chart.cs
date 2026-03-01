using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace Carbon.Components.Graphics;

#pragma warning disable CS0649 // Field 'Chart.graphic' is never assigned to, and will always have its default value null

/// <summary>
/// Carbon base-line for multithreaded Chart image generating.
/// </summary>
public struct Chart
{
	public string Name;
	public ChartSettings Settings;
	public ChartRect Rect;
	public IEnumerable<Layer> Layers;

	internal int width;
	internal int height;
	internal Color background;
	internal string[] verticalLabels;
	internal string[] horizontalLabels;
	internal Brush textColor;
	internal System.Drawing.Graphics graphic;
	internal Action<byte[], Exception> onProcessEnded;
	internal byte[] image;

	public static Chart Create(string name, int width, int height, ChartSettings settings, ChartRect rect,
		IEnumerable<Layer> layers, string[] verticalLabels, string[] horizontalLabels, Brush textColor, Color background)
	{
		Chart chart = default;

		chart.Name = name;
		chart.Settings = settings;
		chart.Rect = rect;
		chart.Layers = layers;
		chart.verticalLabels = verticalLabels;
		chart.horizontalLabels = horizontalLabels;
		chart.width = width;
		chart.height = height;
		chart.background = background;
		chart.textColor = textColor;

		return chart;
	}

	public void StartProcess(Action<byte[], Exception> onProcessEnded = null)
	{
		this.onProcessEnded = onProcessEnded;

		var thread = new ProcessingThread();
		thread.Chart = this;
		thread.Start();
		Community.Runtime.Core.persistence.StartCoroutine(thread.WaitFor());
	}

	public struct ChartSettings
	{
		public bool VerticalLabels;
		public bool HorizontalLabels;
		public Pen GridColor;
	}
	public struct ChartRect
	{
		public float Width;
		public float Height;
		public float X;
		public float Y;
	}

	public class Layer
	{
		public string Name;
		public ulong[] Data;
		public bool Disabled;
		public LayerSettings LayerSettings;

		public void ToggleDisabled()
		{
			Disabled = !Disabled;
		}
	}
	public class LayerSettings
	{
		public Color Color;
		public int Shadows;
	}

	internal void DrawChart(System.Drawing.Graphics graphic, IEnumerable<Layer> layers, string[] verticalLabels, string[] horizontalLabels)
	{
		if (Settings.GridColor == null)
		{
			Settings.GridColor = Pens.DimGray;
		}

		var yAxisLabels = Enumerable.Range(0, verticalLabels.Length).Select(i => verticalLabels[i]).ToArray();
		var rightAlignment = new StringFormat
		{
			LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Far
		};
		var centerAlignment = new StringFormat
		{
			LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center
		};
		var font = new Font("Arial", 15);

		if (Settings.HorizontalLabels)
		{
			for (int i = 0; i < horizontalLabels.Length; i++)
			{
				var x = Rect.X + (i) * (Rect.Width / (horizontalLabels.Length - 1));
				var y = Rect.Y + Rect.Height + 5;
				graphic.DrawString(horizontalLabels[i], font, textColor, x, y + 15, centerAlignment);
			}
		}

		if (Settings.VerticalLabels)
		{
			for (int i = 0; i < yAxisLabels.Length; i++)
			{
				var x = Rect.X - 15;
				var y = Rect.Y + Rect.Height - i * (Rect.Height / (yAxisLabels.Length - 1));
				graphic.DrawString(yAxisLabels[i], font, textColor, x, y, rightAlignment);
			}
		}

		for (int i = 0; i < yAxisLabels.Length; i++)
		{
			var y = Rect.Y + Rect.Height - (i) * (Rect.Height / (yAxisLabels.Length - 1));
			graphic.DrawLine(Pens.DimGray, Rect.X, y, Rect.X + Rect.Width, y);
		}

		for (int i = 0; i < horizontalLabels.Length; i++)
		{
			var x = Rect.X + (i) * (Rect.Width / (horizontalLabels.Length - 1));
			graphic.DrawLine(Pens.DimGray, x, Rect.Y, x, Rect.Y + Rect.Height);
		}

		graphic.DrawLine(Pens.DimGray, Rect.X, Rect.Y, Rect.X, Rect.Y + Rect.Height);
		graphic.DrawLine(Pens.DimGray, Rect.X, Rect.Y + Rect.Height, Rect.X + Rect.Width, Rect.Y + Rect.Height);

		foreach (var layer in layers)
		{
			if (layer.Disabled)
			{
				continue;
			}

			DrawChartContentShadows(graphic, layer.Data, Rect.Width, Rect.Height, Rect.X, Rect.Y, layer.LayerSettings);
		}

		foreach (var layer in layers)
		{
			if (layer.Disabled)
			{
				continue;
			}

			DrawChartContentLineDots(graphic, layer.Data, Rect.Width, Rect.Height, Rect.X, Rect.Y, layer.LayerSettings);
		}
	}
	internal void DrawChartContentShadows(System.Drawing.Graphics graphic, ulong[] data, float chartWidth, float chartHeight, float chartX, float chartY, LayerSettings layerSettings)
	{
		var highestValue = data.Max();
		var spaceBetweenPoints = chartWidth / (data.Length - 1);

		for (int i = 0; i < data.Length; i++)
		{
			var final = i >= data.Length - 1;
			var x = chartX + spaceBetweenPoints * i;
			var y = chartY + chartHeight - data[i] * (chartHeight / highestValue);

			var nextX = final ? x : chartX + spaceBetweenPoints * (i + 1);
			var nextY = final ? y : chartY + chartHeight - data[i + 1] * (chartHeight / highestValue);

			void CreateShadow(float multiply, int alpha)
			{
				PointF[] shadowPoints =
				[
					new PointF(x, y * multiply),
                    new PointF(nextX, nextY * multiply),
                    new PointF(nextX, chartY + chartHeight),
                    new PointF(x, chartY + chartHeight),
				];

				var color = Color.FromArgb(alpha, layerSettings.Color);
				graphic.FillPolygon(new SolidBrush(color), shadowPoints);
			}

			for (float s = 1; s < layerSettings.Shadows; s++)
			{
				CreateShadow(
					s.Scale(0, layerSettings.Shadows, 1f, 0.75f),
					(int)s.Scale(0, layerSettings.Shadows, 25f, 0f));
			}

			if (layerSettings.Shadows > 0)
			{
				CreateShadow(1, (int)(layerSettings.Color.A * 0.2f));
			}
		}
	}
	internal void DrawChartContentLineDots(System.Drawing.Graphics graphic, ulong[] data, float chartWidth, float chartHeight, float chartX, float chartY, LayerSettings layerSettings)
	{
		var highestValue = data.Max();
		var spaceBetweenPoints = chartWidth / (data.Length - 1);
		var linePen = new Pen(layerSettings.Color, 2);
		var markerBrush = new SolidBrush(layerSettings.Color);

		for (int i = 0; i < data.Length; i++)
		{
			var final = i >= data.Length - 1;
			var x = chartX + spaceBetweenPoints * i;
			var y = chartY + chartHeight - data[i] * (chartHeight / highestValue);

			var nextX = final ? x : chartX + spaceBetweenPoints * (i + 1);
			var nextY = final ? y : chartY + chartHeight - data[i + 1] * (chartHeight / highestValue);

			graphic.DrawLine(linePen, x, y, nextX, nextY);
			graphic.FillEllipse(markerBrush, x - 7.5f, y - 7.5f, 15, 15);
			// graphic.DrawString($"{data[i]:n0}", font, Brushes.White, x, y);
		}
	}

	public class ProcessingThread : BaseThreadedJob
	{
		public Chart Chart;
		public Exception Exception;

		public override void ThreadFunction()
		{
			try
			{
				using var chartBitmap = new Bitmap(Chart.width, Chart.height);
				using var graphic = global::System.Drawing.Graphics.FromImage(chartBitmap);

				graphic.Clear(Chart.background);

				graphic.SmoothingMode = SmoothingMode.AntiAlias;
				graphic.CompositingQuality = CompositingQuality.HighQuality;
				graphic.PageUnit = GraphicsUnit.Display;
				graphic.TextRenderingHint = TextRenderingHint.AntiAlias;
				graphic.InterpolationMode = InterpolationMode.HighQualityBilinear;

				Chart.DrawChart(graphic, Chart.Layers, Chart.verticalLabels, Chart.horizontalLabels);

				using var memory = new MemoryStream();
				chartBitmap.Save(memory, ImageFormat.Png);

				Chart.image = memory.ToArray();
			}
			catch (Exception ex)
			{
				Exception = ex;
				Logger.Error("Chart processing failed! Report to developers", ex);
			}

			base.ThreadFunction();
		}

		public override void OnFinished()
		{
			Chart.onProcessEnded?.Invoke(Chart.image, Exception);

			base.OnFinished();
		}
	}
}
