using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Klase;

namespace HCI_PZ1
{
	/// <summary>
	/// Interaction logic for ReadWindow.xaml
	/// </summary>
	public partial class ReadWindow : Window
	{
		public ReadWindow(VideoIgra videoIgra)
		{
			InitializeComponent();

			labelNaziv.Content = videoIgra.Naziv.ToUpper();
			labelDatum.Content = $"Datum izlaska: {videoIgra.Datum.ToString("dd.MM.yyyy.")}";
			labelRedniBroj.Content = $"Redni broj izdanja: {videoIgra.RedniBroj.ToString()}";
			labelKreirano.Content = videoIgra.Kreirano.ToString();

			try
			{
				slikaPregled.Source = new BitmapImage(new Uri(videoIgra.Slika));
			}
			catch (FileNotFoundException)
			{
				this.Close();
				throw new FileNotFoundException("Nije moguće učitati sliku.");
			}
			
			LoadRTBFromPath(videoIgra.RtfPutanja);
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void buttonClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void buttonMinimize_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void LoadRTBFromPath(string pathToRTF)
		{
			try
			{
				using (FileStream fileStream = new FileStream(pathToRTF, FileMode.Open))
				{
					TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
					range.Load(fileStream, DataFormats.Rtf);
				}
			}
			catch (FileNotFoundException)
			{
				this.Close();
				throw new FileNotFoundException("Nije moguće učitati RTF fajl.");
			}
		}
	}
}
