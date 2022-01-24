using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// Interaction logic for AddWindow.xaml
	/// </summary>
	public partial class AddWindow : Window
	{
		private Dictionary<string, Brush> kolekcijaBoja;

		private BitmapImage imagePath;

		public AddWindow()
		{
			InitializeComponent();

			textBoxRedniBroj.Foreground = Brushes.LightSlateGray;
			textBoxNaziv.Foreground = Brushes.LightSlateGray;
			rtbEditor.Foreground = Brushes.LightSlateGray;

			textBoxRedniBroj.Text = "unesite redni broj";
			textBoxNaziv.Text = "unesite naziv video igre";
			rtbEditor.AppendText("unesite kratak opis igre");

			slikaPregled.Source = new BitmapImage(new Uri("no-image.jpg", UriKind.Relative));

			cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
			cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

			var values = typeof(Brushes).GetProperties().Select(p => new { Name = p.Name, Brush = p.GetValue(null) as Brush }).ToArray();
			var names = values.Select(v => v.Name);
			var hexvals = values.Select(v => v.Brush);
			kolekcijaBoja = names.Zip(hexvals, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
			cmbFontColor.ItemsSource = kolekcijaBoja.Keys;
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

			if (GetRichTextBoxContent(rtbEditor).Trim().Equals("") || GetRichTextBoxContent(rtbEditor).Trim().Equals("unesite kratak opis igre") || GetRichTextBoxContent(rtbEditor).Trim().Equals("Polje ne sme biti prazno!"))
			{
				rtbEditor.Document.Blocks.Clear();

				status = false;
				rtbEditor.Foreground = Brushes.Red;
				rtbEditor.BorderBrush = Brushes.Red;
				rtbEditor.AppendText("Polje ne sme biti prazno!");
			}
			else if (GetWordCount(rtbEditor) == 0)
			{
				rtbEditor.Document.Blocks.Clear();
				
				status = false;
				rtbEditor.Foreground = Brushes.Red;
				rtbEditor.BorderBrush = Brushes.Red;
				rtbEditor.AppendText("Polje ne sme biti prazno!");
			}

			if (imagePath == null)
			{
				status = false;
				buttonSlika.Foreground = Brushes.Red;
				buttonSlika.BorderBrush = Brushes.Red;
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
			if (GetRichTextBoxContent(rtbEditor).Trim().Equals(String.Empty))
			{
				rtbEditor.Foreground = Brushes.LightSlateGray;
				rtbEditor.AppendText("unesite kratak opis igre");
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
				textBoxRedniBroj.Text = "unesite redni broj";
				textBoxRedniBroj.Foreground = Brushes.LightSlateGray;
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
				textBoxNaziv.Text = "unesite naziv video igre";
				textBoxNaziv.Foreground = Brushes.LightSlateGray;
			}
		}

		private void DatePickerDatum_GotFocus(object sender, RoutedEventArgs e)
		{
			datePickerDatum.Foreground = Brushes.Black;
			datePickerDatum.ClearValue(DatePicker.BorderBrushProperty);
		}

		private void DatePickerDatum_LostFocus(object sender, RoutedEventArgs e)
		{
			if (datePickerDatum.SelectedDate.HasValue)
			{
				datePickerDatum.Foreground = Brushes.Black;
				datePickerDatum.ClearValue(DatePicker.BorderBrushProperty);
			}
		}

		private void SlikaPregled_ImageFailed(object sender, ExceptionRoutedEventArgs e)
		{
			slikaPregled.Source = new BitmapImage(new Uri("no-image.jpg", UriKind.Relative));
		}

		private void buttonAdd_Click(object sender, RoutedEventArgs e)
		{
			if (validateInput())
			{
				int redniBroj = int.Parse(textBoxRedniBroj.Text);
				string naziv = textBoxNaziv.Text;
				string slika = imagePath.ToString();

				DateTime? tempDatum = datePickerDatum.SelectedDate;
				DateTime datum = tempDatum.HasValue ? tempDatum.Value : default(DateTime);

				string rtfputanja = DateTime.Now.ToString("yyyyMMddTHHmmss") + ".rtf";

				SaveRTBToPath(rtfputanja);

				DateTime kreirano = DateTime.Now;

				VideoIgra videoIgra = new VideoIgra(redniBroj, naziv, datum, slika, rtfputanja, kreirano);
				MainWindow.SerijalVideoIgara.Add(videoIgra);

				this.Close();
			}
			else
			{
				MessageBox.Show("Polja nisu dobro popunjena!", "Greska!", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void SaveRTBToPath(string pathToRTF)
		{
			using (FileStream fileStream = new FileStream(pathToRTF, FileMode.Create))
			{
				TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
				range.Save(fileStream, DataFormats.Rtf);
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

			MatchCollection words = Regex.Matches(text, @"[\w]+");

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
