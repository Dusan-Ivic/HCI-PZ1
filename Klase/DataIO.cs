﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Klase
{
	public class DataIO
	{
		public void SerializeObject<T>(T serializableObject, string fileName)
		{
			if (serializableObject == null) { return; }

			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
				using (MemoryStream stream = new MemoryStream())
				{
					serializer.Serialize(stream, serializableObject);
					stream.Position = 0;
					xmlDocument.Load(stream);
					xmlDocument.Save(fileName);
					stream.Close();
				}
			}
			catch (Exception ex)
			{
				//Log exception here
				Console.WriteLine(ex.Message);
			}
		}

		public T DeSerializeObject<T>(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) { return default(T); }

			T objectOut = default(T);

			try
			{
				string attributeXml = string.Empty;

				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(fileName);
				string xmlString = xmlDocument.OuterXml;

				using (StringReader read = new StringReader(xmlString))
				{
					Type outType = typeof(T);

					XmlSerializer serializer = new XmlSerializer(outType);
					using (XmlReader reader = new XmlTextReader(read))
					{
						objectOut = (T)serializer.Deserialize(reader);
						reader.Close();
					}

					read.Close();
				}
			}
			catch (Exception ex)
			{
				//Log exception here
				Console.WriteLine(ex.Message);
			}

			return objectOut;
		}
	}
}
