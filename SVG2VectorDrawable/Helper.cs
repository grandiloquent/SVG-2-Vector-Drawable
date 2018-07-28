namespace Helper
{
	using System.Windows.Forms;
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml;
	using System.Xml.Linq;
	using System.Reflection;
	using System.Diagnostics;
	using System.Text.RegularExpressions;
	using System.Globalization;
	
	
	public	static class Formatter
	{

		public static string RemoveComments(string value)
		{

			var blockComments = @"/\*(.*?)\*/";
			var lineComments = @"//(.*?)\r?\n";
			var strings = @"""((\\[^\n]|[^""\n])*)""";
			var verbatimStrings = @"@(""[^""]*"")+";
			string noComments = Regex.Replace(value,
				                    blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
				                    me => {
					if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
						return me.Value.StartsWith("//") ? Environment.NewLine : "";
					// Keep the literal strings
					return me.Value;
				},
				                    RegexOptions.Singleline);
			return Regex.Replace(noComments, "[\r\n]+", Environment.NewLine);
		}

		public const string ChineseZodiac = "鼠牛虎兔龙蛇马羊猴鸡狗猪";

		public static string FormatNginxConf(string value)
		{
			var sb = new StringBuilder();
			var count = 0;
			foreach (var item in value) {
				if (item == '{') {
					sb.AppendLine("{");
					count++;
				} else if (item == '}') {
					sb.AppendLine('\t'.Repeat(count) + "}");

					count--;
				} else if (item == ';') {
					sb.AppendLine(";");
					sb.Append('\t'.Repeat(count));
				} else if (item == '\r' || item == '\n' || item == '\t') {

					continue;
				} else {
					sb.Append(item);
				}

			}
			return sb.ToString();
		}
		public static string FormatBlockComment(string value)
		{
			var sb = new StringBuilder();
			var cacheSb = new StringBuilder();

			sb.Append("/*\r\n\r\n");
			foreach (var item in value.Split(new char[] { '\n' })) {
				if (item.IsReadable()) {
					foreach (var l in item.Split(' ')) {
						if (l.IsReadable()) {

							cacheSb.Append(l.Trim()).Append(' ');
							if (cacheSb.Length > 50) {
								sb.Append(cacheSb).AppendLine();
								cacheSb.Clear();
							}
						}
					}
					if (cacheSb.Length > 0) {
						sb.Append(cacheSb).AppendLine().AppendLine();
						cacheSb.Clear();
					}

				}
			}
			sb.Append("*/\r\n");
			return sb.ToString();
		}

		public static IEnumerable<string> FormatMethodList(string value)
		{
			var count = 0;
			var sb = new StringBuilder();
			var ls = new List<string>();
			for (int i = 0; i < value.Length; i++) {
				sb.Append(value[i]);

				if (value[i] == '{') {
					count++;
				} else if (value[i] == '}') {
					count--;
					if (count == 0) {
						ls.Add(sb.ToString());
						sb.Clear();
					}
				}

			}
			//if (ls.Any())
			//{
			//    var firstLine = ls[0];
			//    ls.RemoveAt(0);
			//    ls.Add(firstLine.)

			//}
			return ls;
			//return ls.Select(i => i.Split(new char[] { '{' }, 2).First().Trim() + ";").OrderBy(i => i.Trim());

		}

		public static string FormatStringBuilder(string value)
		{

			var sb = new StringBuilder();

			sb.AppendLine("var sb = new StringBuilder();");

			var ls = value.Split('\n').Where(i => i.IsReadable()).Select(i => i.Trim());

			foreach (var item in ls) {
				sb.AppendFormat("sb.AppendLine({0});\r\n", item.ToLiteral());
			}

			return sb.ToString();
		}
	}
	public static class Extensions
	{
		public static readonly char DirectorySeparatorChar = '\\';
		public static readonly char AltDirectorySeparatorChar = '/';
		public static readonly char VolumeSeparatorChar = ':';
        
		
		public static String GetFileName(this String path)
		{
			if (path != null) {


				int length = path.Length;
				for (int i = length; --i >= 0;) {
					char ch = path[i];
					if (ch == DirectorySeparatorChar || ch == AltDirectorySeparatorChar || ch == VolumeSeparatorChar)
						return path.Substring(i + 1, length - i - 1);

				}
			}
			return path;
		}
		public static String GetFileNameWithoutExtension(this String path)
		{
			path = GetFileName(path);
			if (path != null) {
				int i;
				if ((i = path.LastIndexOf('.')) == -1)
					return path; // No path extension found
                else
					return path.Substring(0, i);
			}
			return null;
		}
		public static string GetDirectoryName(this string p)
		{
			return Path.GetDirectoryName(p);
		}
		public static String ReadAllText(this String path)
		{
			// https://referencesource.microsoft.com/#mscorlib/system/io/file.cs,8a84c56a62fd8d45
			using (StreamReader sr = new StreamReader(path, new UTF8Encoding(false), true, 1024))
				return sr.ReadToEnd();
		}
		public static bool IsReadable(this string value)
		{
			return  !string.IsNullOrWhiteSpace(value);
		}
		public static string Repeat(this char c, int count)
		{
			return new String(c, count);
		}
		public static void WriteAllText(this String path, String contents)
		{
			using (StreamWriter sw = new StreamWriter(path, false, new UTF8Encoding(false), 1024))
				sw.Write(contents);
		}
		public static string ToLiteral(this string input)
		{
			var literal = new StringBuilder(input.Length + 2);
			literal.Append("\"");
			foreach (var c in input) {
				switch (c) {
					case '\'':
						literal.Append(@"\'");
						break;
					case '\"':
						literal.Append("\\\"");
						break;
					case '\\':
						literal.Append(@"\\");
						break;
					case '\0':
						literal.Append(@"\0");
						break;
					case '\a':
						literal.Append(@"\a");
						break;
					case '\b':
						literal.Append(@"\b");
						break;
					case '\f':
						literal.Append(@"\f");
						break;
					case '\n':
						literal.Append(@"\n");
						break;
					case '\r':
						literal.Append(@"\r");
						break;
					case '\t':
						literal.Append(@"\t");
						break;
					case '\v':
						literal.Append(@"\v");
						break;
					default:
						if (Char.GetUnicodeCategory(c) != UnicodeCategory.Control) {
							literal.Append(c);
						} else {
							literal.Append(@"\u");
							literal.Append(((ushort)c).ToString("x4"));
						}
						break;
				}
			}
			literal.Append("\"");
			return literal.ToString();
		}
		public static string GetCommandLinePath(this string fileName)
		{
			
			return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileName);
		}
		
	}
	public static class Helpers
	{
		public static void OpenExecutableDirectory()
		{

			Process.Start("".GetCommandLinePath());
		}
		public static void OnClipboardString(Func<String,String> action)
		{
			
			try {
				var result =	action(Clipboard.GetText());
				if (!string.IsNullOrWhiteSpace(result))
					Clipboard.SetText(result);
			} catch (Exception ex) {
				
			}
		}
		
		public static void OnClipboardFileSystem(Action<String> action)
		{
			
			try {
				var path = Clipboard.GetText().Trim();
				if (Directory.Exists(path) || File.Exists(path))
					action(path);
				else {
					var collection = Clipboard.GetFileDropList();
					if (collection.Count > 0) {
						path = collection[0];
						action(path);
					}
				}
				
			} catch (Exception ex) {
				
			}
		}
	}
	public static class AndroidExtensions
	{
		
		
		public static String FormatMenuItemToCode(string value)
		{
			
			var xd = XDocument.Parse(value);
			
			var items = xd.Descendants().Where(i => i.Name.LocalName == "item").ToArray();
			var ls1 = new List<String>();
			var ls2 = new List<String>();
			var ls3 = new List<String>();
			
			for (int i = 0; i < items.Length; i++) {
				var id = items[i].Attributes().First(iv => iv.Name.LocalName == "id").Value.Split('/').Last();
				ls1.Add(string.Format("findItem(R.id.{0}).isVisible = {1}", id, "false"));
				ls2.Add(string.Format("findItem(R.id.{0}).isVisible = {1}", id, "true"));
				
				ls3.Add(string.Format("R.id.{0} -> ", id));
			}
			
		 
			
			var sb = new StringBuilder();
			
			sb.AppendLine(string.Join(Environment.NewLine, ls1.OrderBy(i => i)));
			sb.AppendLine(string.Join(Environment.NewLine, ls2.OrderBy(i => i)));
			sb.AppendLine(string.Join(Environment.NewLine, ls3.OrderBy(i => i)));
			
			
			return sb.ToString();
			
		}
		
		public static String FormatFun(string value)
		{
			  
			var lines = value.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
			var singleItems = lines.Where(i => (i.StartsWith("val") || i.StartsWith("fun ") || i.StartsWith("private fun") || i.StartsWith("private val")) && i.Contains(") = ") && !i.EndsWith("{")).ToArray();
			var sss = lines.Except(singleItems).ToArray();
			var ls = Formatter.FormatMethodList(string.Join("\n", lines.Where(i => !singleItems.Contains(i)))).Select(i => i.Trim()).OrderBy(i => Regex.Match(i, "fun ([^\\(]*?)(?:\\()").Groups[1].Value).ToArray();

			return string.Join("\n", singleItems.OrderBy(i => i)) + "\n" + string.Join("\n", ls);
		}
	}
}