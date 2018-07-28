 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Helper;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace SVG2VectorDrawable
{
	 
	public partial class MainForm : Form
	{
		public MainForm()
		{
			 
			InitializeComponent();
			
			 
		}
		
		private void ConvertSVGToVector(string path)
		{
			 
			var xd = XDocument.Parse(path.ReadAllText());
			
			// Correspondingly "android:width", if SVG does not contain this property, use the default value 24dp.
			var width = "24";
			var height = "24";
			try {
				width = xd.Root.Attribute("width").Value;
			} catch {
			}
			try {
				height = xd.Root.Attribute("height").Value;
			} catch {
			}
			
			var viewport = xd.Root.Attribute("viewBox").Value.Split(' ');
			var wn = int.Parse(viewport[viewport.Length - 2]);
			var hn = int.Parse(viewport[viewport.Length - 1]);

			// The square is better laid out, 
			// so compare the height and width and choose 
			// the largest one as the side length of the square.
			var wv = (Math.Max(wn, hn).ToString());
            

			var paths = xd.Root.Descendants().Where(i => i.Name.LocalName == "path").ToArray();

			XNamespace android = "http://schemas.android.com/apk/res/android";
			XNamespace tools = "http://schemas.android.com/tools";

			var n = new XDocument();
			var vector = new XElement("vector",

				             new XAttribute(XNamespace.Xmlns + "android", "http://schemas.android.com/apk/res/android"),
				             new XAttribute(XNamespace.Xmlns + "tools", "http://schemas.android.com/tools"),

				             new XAttribute(android + "width", width + "dp"),
				             new XAttribute(android + "height", height + "dp"),
				             new XAttribute(android + "viewportWidth", wv + ".0"),
				             new XAttribute(android + "viewportHeight", wv + ".0"));

			if (wn > hn) {
				// If the width is greater than the height, 
				// offset along the Y axis to ensure the final image is centered
				var group = new XElement("group",
					            new XAttribute(android + "translateY", (wn - (hn * 1.0f)) / 2));
				foreach (var item in paths) {
					group.Add(new XElement("path",
						new XAttribute(android + "fillColor", "#FF000000"),
						new XAttribute(android + "pathData", item.Attribute("d").Value),
						new XAttribute(tools + "ignore", "InvalidVectorPath")

					));
				}
				vector.Add(group);
			} else if (hn > wn) {
				var group = new XElement("group",
					            new XAttribute(android + "translateX", (hn - (wn * 1.0f)) / 2));
				foreach (var item in paths) {
					group.Add(new XElement("path",
						new XAttribute(android + "fillColor", "#FF000000"),
						new XAttribute(android + "pathData", item.Attribute("d").Value),
						new XAttribute(tools + "ignore", "InvalidVectorPath")

					));
				}
				vector.Add(group);
			} else {
				foreach (var item in paths) {
					vector.Add(new XElement("path",
						new XAttribute(android + "fillColor", "#FF000000"),
						new XAttribute(android + "pathData", item.Attribute("d").Value),
						new XAttribute(tools + "ignore", "InvalidVectorPath")

					));
				}

			}
			var dir = path.GetDirectoryName();
			var fileName = path.GetFileNameWithoutExtension().Replace("-", "_");
			if (!fileName.StartsWith("ic_"))
				fileName = "ic_" + fileName;
			vector.Save(Path.Combine(dir, fileName) + ".xml");
			var whiteFileName = fileName.GetFileNameWithoutExtension();
			if (whiteFileName.Count(i => i == '_') > 1) {
				var pos = whiteFileName.LastIndexOf('_');
           
				if (pos > -1) {
					whiteFileName = fileName.Substring(0, pos + 1) + "white" + fileName.Substring(pos);
				}
			} else {
				whiteFileName += "_white";
			}
			(Path.Combine(dir, whiteFileName + ".xml")).WriteAllText(vector.ToString().Replace("#FF000000", "#FFFFFFFF"));
 
		}
		void OkClick(object sender, EventArgs e)
		{
			Helpers.OnClipboardFileSystem((path) => {
			                              	
			                              	if(path.ToLower().EndsWith(".svg")){
			                              		ConvertSVGToVector(path);
			                              	}else{
			                              		var files=System.IO.Directory.GetFiles(path,"*.svg");
			                              		foreach (var element in files) {
			                              		
			                              			ConvertSVGToVector(element);
			                              		}
			                              	}
			                              	
			});
		}
	}
}
