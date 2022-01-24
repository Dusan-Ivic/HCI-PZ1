using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Klase;
using System.IO;

namespace HCI_PZ1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private DataIO serializer = new DataIO();
		public static BindingList<VideoIgra> SerijalVideoIgara { get; set; }

		public MainWindow()
		{
			SerijalVideoIgara = serializer.DeSerializeObject<BindingList<VideoIgra>>("serijal.xml");

			if (SerijalVideoIgara == null)
			{
				SerijalVideoIgara = new BindingList<VideoIgra>();
			}

			DataContext = this;

			InitializeComponent();
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

		private void buttonAdd_Click(object sender, RoutedEventArgs e)
		{
			AddWindow addWindow = new AddWindow();
			addWindow.ShowDialog();
		}

		private void ButtonDelete_Click(object sender, RoutedEventArgs e)
		{
			VideoIgra videoIgra = (VideoIgra)dataGridSerijalVideoIgara.SelectedItem;
			try
			{
				SerijalVideoIgara.Remove(videoIgra);

				File.Delete(videoIgra.RtfPutanja);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ButtonEdit_Click(object sender, RoutedEventArgs e)
		{
			VideoIgra videoIgra = (VideoIgra)dataGridSerijalVideoIgara.SelectedItem;
			try
			{
				EditWindow editWindow = new EditWindow(videoIgra);

				editWindow.ShowDialog();

				SerijalVideoIgara[dataGridSerijalVideoIgara.SelectedIndex] = editWindow.videoIgra;
			}
			catch (FileNotFoundException ex)
			{
				MessageBoxResult mbr = MessageBox.Show($"{ex.Message}\nVideo igra će biti obrisana!", "Da li želite da nastavite?", MessageBoxButton.YesNo, MessageBoxImage.Error);
				if (mbr == MessageBoxResult.Yes)
				{
					SerijalVideoIgara.Remove(videoIgra);

					dataGridSerijalVideoIgara.Items.Refresh();

					File.Delete(videoIgra.RtfPutanja);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ButtonRead_Click(object sender, RoutedEventArgs e)
		{
			VideoIgra videoIgra = (VideoIgra)dataGridSerijalVideoIgara.SelectedItem;
			try
			{
				ReadWindow readWindow = new ReadWindow(videoIgra);

				readWindow.ShowDialog();
			}
			catch (FileNotFoundException ex)
			{
				MessageBoxResult mbr = MessageBox.Show($"{ex.Message}\nVideo igra će biti obrisana!", "Da li želite da nastavite?", MessageBoxButton.YesNo, MessageBoxImage.Error);
				if (mbr == MessageBoxResult.Yes)
				{
					SerijalVideoIgara.Remove(videoIgra);

					File.Delete(videoIgra.RtfPutanja);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			serializer.SerializeObject<BindingList<VideoIgra>>(SerijalVideoIgara, "serijal.xml");
		}

		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			serializer.SerializeObject<BindingList<VideoIgra>>(SerijalVideoIgara, "serijal.xml");
		}
		
	}
}
