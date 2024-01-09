using System.IO;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

 public class GestureIO
    {
        /// <summary>
        /// Reads a multistroke gesture from an XML file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <summary>
        /// Reads a multistroke gesture from an XML file
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
		public static DollarRecognizer.Unistroke ReadGestureFromXML(string xml) 
        {

			XmlTextReader xmlReader = null;
			DollarRecognizer.Unistroke gesture = null;

			try {

				xmlReader = new XmlTextReader(new StringReader(xml));
				gesture = ReadGesture(xmlReader);

			} finally {

				if (xmlReader != null)
					xmlReader.Close();
			}

			return gesture;
		}

        public static DollarRecognizer.Unistroke ReadGesture(XmlTextReader xmlReader)
        {
            List<Vector2> points = new List<Vector2>();
            string gestureName = "";
            try
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element) continue;
                    switch (xmlReader.Name)
                    {
                        case "Gesture":
                            gestureName = xmlReader["Name"];
                            if (gestureName.Contains("~")) // '~' character is specific to the naming convention of the MMG set
                                gestureName = gestureName.Substring(0, gestureName.LastIndexOf('~'));
                            if (gestureName.Contains("_")) // '_' character is specific to the naming convention of the MMG set
                                gestureName = gestureName.Replace('_', ' ');
                            break;
                        case "Point":
                            points.Add(new Vector2(
                                float.Parse(xmlReader["X"]),
                                float.Parse(xmlReader["Y"])
                            ));
                            break;
                    }
                }
            }
            finally
            {
                if (xmlReader != null)
                    xmlReader.Close();
            }
            return new DollarRecognizer.Unistroke(gestureName, points);
        }

        /// <summary>
        /// Writes a multistroke gesture to an XML file
        /// </summary>
        public static void WriteGesture(DollarRecognizer.Unistroke unistroke, string gestureName, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
                sw.WriteLine("<Gesture Name = \"{0}\">", gestureName);
                for (int i = 0; i < unistroke.Points.Length; i++)
                {
                    sw.WriteLine("\t\t<Point X = \"{0}\" Y = \"{1}\" T = \"0\" Pressure = \"0\" />",
                        unistroke.Points[i].x, unistroke.Points[i].y
                    );
                }
                sw.WriteLine("</Gesture>");
            }
        }

        public static void WriteGesture(Vector2[] points, string gestureName, string fileName){
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
                sw.WriteLine("<Gesture Name = \"{0}\">", gestureName);
                for (int i = 0; i < points.Length; i++)
                {
                    sw.WriteLine("\t\t<Point X = \"{0}\" Y = \"{1}\" T = \"0\" Pressure = \"0\" />",
                        points[i].x, points[i].y
                    );
                }
                sw.WriteLine("</Gesture>");
            }
        }
    }