using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Win32;

namespace HCI_PZ1
{
	/// <summary>
	/// Interaction logic for EditWindow.xaml
	/// </summary>
	public partial class EditWindow : Window
	{
		public VideoIgra videoIgra { get; set; }

		private Dictionary<string, Brush> kolekcijaBoja;

		private BitmapImage imagePath;

		public EditWindow(VideoIgra vi)
		{
			InitializeComponent();

			cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
			cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

			var values = typeof(Brushes).GetProperties().Select(p => new { Name = p.Name, Brush = p.GetValue(null) as Brush }).ToArray();
			var names = values.Select(v => v.Name);
			var hexvals = values.Select(v => v.Brush);
			kolekcijaBoja = names.Zip(hexvals, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
			cmbFontColor.ItemsSource = kolekcijaBoja.Keys;

			videoIgra = vi;

			textBoxRedniBroj.Text = videoIgra.RedniBroj.ToString();
			textBoxNaziv.Text = videoIgra.Naziv;

			datePickerDatum.SelectedDate = videoIgra.Datum;

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

		private bool validateInput()
		{
			bool status = true;

			if (textBoxRedniBroj.Text.Trim().Equals("") || textBoxRedniBroj.Text.Trim().Equals("unesite redni broj") || textBoxRedniBroj.Text.Trim().Equals("Polje ne sme biti prazno!"))
			{
				status = false;
				textBoxRedniBroj.Text = "Polje ne sme biti prazno!";
				textBoxRedniBroj.Foreground = Brushes.Red;
				textBoxRedniBroj.BorderBrush = Brushes.Red;
			}
			else
			{
				int number;
				bool isInputNumber = int.TryParse(textBoxRedniBroj.Text, out number);
				if (!isInputNumber)
				{
					status = false;
					textBoxRedniBroj.Text = "Unesite ceo broj!";
					textBoxRedniBroj.Foreground = Brushes.Red;
					textBoxRedniBroj.BorderBrush = Brushes.Red;
				}
				else if (number < 0)
				{
					status = false;
					textBoxRedniBroj.Text = "Unesite pozitivan broj!";
					textBoxRedniBroj.Foreground = Brushes.Red;
					textBoxRedniBroj.BorderBrush = Brushes.Red;
				}
			}

			if (textBoxNaziv.Text.Trim().Equals("") || textBoxNaziv.Text.Trim().Equals("unesite naziv video igre") || textBoxNaziv.Text.Trim().Equals("Polje ne sme biti prazno!"))
			{
				status = false;
				textBoxNaziv.Text = "Polje ne sme biti prazno!";
				textBoxNaziv.Foreground = Brushes.Red;
				textBoxNaziv.BorderBrush = Brushes.Red;
			}

			if (!datePickerDatum.SelectedDate.HasValue)
			{
				status = false;
				datePickerDatum.Foreground = Brushes.Red;
				datePickerDatum.BorderBrush = Brushes.Red;
			}

			if (imagePath == null)
			{
				imagePath = new BitmapImage(new Uri(videoIgra.Slika));
			}

			return status;
		}

		private void ButtonSlika_GotFocus(object sender, RoutedEventArgs e)
		{
			if (buttonSlika.Foreground == Brushes.Red || buttonSlika.BorderBrush == Brushes.Red)
			{
				buttonSlika.ClearValue(Button.BorderBrushProperty);
				buttonSlika.ClearValue(Button.ForegroundProperty);
			}
		}

		private void RtbEditor_GotFocus(object sender, RoutedEventArgs e)
		{
			if (GetRichTextBoxContent(rtbEditor).Trim().Equals("unesite kratak opis igre") || GetRichTextBoxContent(rtbEditor).Trim().Equals("Polje ne sme biti prazno!"))
			{
				rtbEditor.Document.Blocks.Clear();
				rtbEditor.Foreground = Brushes.Black;
				rtbEditor.ClearValue(RichTextBox.BorderBrushProperty);
			}
		}

		private void RtbEditor_LostFocus(object sender, RoutedEventArgs e)
		{
			if (GetRichTextBoxContent(rtbEditor).Trim().Equals(String.Empty) || GetWordCount(rtbEditor) == 0)
			{
				rtbEditor.Document.Blocks.Clear();

				rtbEditor.Foreground = Brushes.Black;
				LoadRTBFromPath(videoIgra.RtfPutanja);
			}
		}

		private void TextBoxRedniBroj_GotFocus(object sender, RoutedEventArgs e)
		{
			if (textBoxRedniBroj.Text.Trim().Equals("unesite redni broj") || textBoxRedniBroj.Text.Trim().Equals("Polje ne sme biti prazno!") || textBoxRedniBroj.Text.Trim().Equals("Unesite ceo broj!"))
			{
				textBoxRedniBroj.Text = "";
				textBoxRedniBroj.Foreground = Brushes.Black;
				textBoxRedniBroj.ClearValue(TextBox.BorderBrushProperty);
			}

		}

		private void TextBoxRedniBroj_LostFocus(object sender, RoutedEventArgs e)
		{
			if (textBoxRedniBroj.Text.Trim().Equals(String.Empty))
			{
				textBoxRedniBroj.Text = videoIgra.RedniBroj.ToString();
				textBoxRedniBroj.Foreground = Brushes.Black;
			}
		}

		private void TextBoxNaziv_GotFocus(object sender, RoutedEventArgs e)
		{
			if (textBoxNaziv.Text.Trim().Equals("unesite naziv video igre") || textBoxNaziv.Text.Trim().Equals("Polje ne sme biti prazno!"))
			{
				textBoxNaziv.Text = "";
				textBoxNaziv.Foreground = Brushes.Black;
				textBoxNaziv.ClearValue(TextBox.BorderBrushProperty);
			}
		}

		private void TextBoxNaziv_LostFocus(object sender, RoutedEventArgs e)
		{
			if (textBoxNaziv.Text.Trim().Equals(String.Empty))
			{
				textBoxNaziv.Text = videoIgra.Naziv;
				textBoxNaziv.Foreground = Brushes.Black;
			}
		}

		private void DatePickerDatum_GotFocus(object sender, RoutedEventArgs e)
		{
			datePickerDatum.Foreground = Brushes.Black;
			datePickerDatum.ClearValue(DatePicker.BorderBrushProperty);
		}

		private void DatePickerDatum_LostFocus(object sender, RoutedEventArgs e)
		{
			if (!datePickerDatum.SelectedDate.HasValue)
			{
				datePickerDatum.Foreground = Brushes.Black;
				datePickerDatum.SelectedDate = videoIgra.Datum;
			}
		}

		private void buttonEdit_Click(object sender, RoutedEventArgs e)
		{
			if (validateInput())
			{
				int redniBroj;
				int.TryParse(textBoxRedniBroj.Text, out redniBroj);
				videoIgra.RedniBroj = redniBroj;

				videoIgra.Naziv = textBoxNaziv.Text;

				videoIgra.Slika = imagePath.ToString();

				DateTime? tempDatum = datePickerDatum.SelectedDate;
				DateTime datum = tempDatum.HasValue ? tempDatum.Value : default(DateTime);
				videoIgra.Datum = datum;

				SaveRTBToPath(videoIgra.RtfPutanja);

				this.Close();
			}
			else
			{
				MessageBox.Show("Polja nisu dobro popunjena!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void SaveRTBToPath(string pathToRTF)
		{
			using (FileStream fileStream = new FileStream(pathToRTF, FileMode.Truncate))
			{
				TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
				range.Save(fileStream, DataFormats.Rtf);
			}
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

		private void RtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
		{
			object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
			btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
			btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
			temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
			btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
			cmbFontFamily.SelectedItem = temp;
			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
			cmbFontSize.Text = temp.ToString();

			temp = rtbEditor.Selection.GetPropertyValue(Inline.ForegroundProperty);

			foreach (string boja in kolekcijaBoja.Keys)
			{
				if (kolekcijaBoja[boja].ToString() == temp.ToString())
				{
					cmbFontColor.Text = boja;
					break;
				}
			}
		}

		private void RtbEditor_TextChanged(object sender, TextChangedEventArgs e)
		{
			int wordCount = GetWordCount(rtbEditor);

			labelBrojReci.Content = $"BROJ REČI: {wordCount}";
		}

		private int GetWordCount(RichTextBox rtb)
		{
			string text = GetRichTextBoxContent(rtb);

			MatchCollection words = Regex.Matches(text, @"[\w]+"); // [a-zA-Z0-9_]+

			return words.Count;
		}

		private string GetRichTextBoxContent(RichTextBox rtb)
		{
			TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

			return textRange.Text;
		}


		private void CmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbFontFamily.SelectedItem != null && !rtbEditor.Selection.IsEmpty)
			{
				rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
			}
		}

		private void CmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbFontSize.SelectedItem != null && !rtbEditor.Selection.IsEmpty)
			{
				rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.SelectedItem);
			}
		}

		private void CmbFontColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbFontColor.SelectedItem != null && !rtbEditor.Selection.IsEmpty)
			{
				rtbEditor.Selection.ApplyPropertyValue(Inline.ForegroundProperty, cmbFontColor.SelectedItem);
			}
		}

		private void ButtonSlika_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Izaberi sliku";
			dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
			  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
			  "Portable Network Graphic (*.png)|*.png";
			if (dlg.ShowDialog() == true)
			{
				imagePath = new BitmapImage(new Uri(dlg.FileName));
				slikaPregled.Source = imagePath;
			}
		}
	}
}
