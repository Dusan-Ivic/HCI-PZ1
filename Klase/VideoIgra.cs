using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Klase
{
	[Serializable]
	public class VideoIgra
	{
		public int RedniBroj { get; set; }      // Redni broj izdanja
		public string Naziv { get; set; }       // Naziv igre
		public DateTime Datum { get; set; }       // Datum izlaska
		public string Slika { get; set; }       // Putanja do slike
		public string RtfPutanja { get; set; }     // Putanja do RTF fajla

		public DateTime Kreirano { get; set; }	// Datum i vreme kreiranja objekta klase

		public VideoIgra()
		{

		}

		public VideoIgra(int redniBroj, string naziv, DateTime datum, string slika, string rtfPutanja, DateTime kreirano)
		{
			RedniBroj = redniBroj;
			Naziv = naziv;
			Datum = datum;
			Slika = slika;
			RtfPutanja = rtfPutanja;
			Kreirano = kreirano;
		}
	}
}
