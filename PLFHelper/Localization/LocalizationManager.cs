/*
 * Copyright (c) 2014 Oliver Schramm
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PLFHelper.Localization
{
	/// <summary>
	/// Own localization manager for getting localized strings.
	/// No need for satellite assemblies, but embedded text files.
	/// </summary>
	internal static class LocalizationManager
	{
		private static readonly string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
		private static string currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
		private static string fileName = "strings";
		private static readonly string ns = typeof(LocalizationManager).Namespace;
		private static Dictionary<string, string> strings = new Dictionary<string, string>();
		// Verbatim string to avoid double backslash
		private static readonly Regex regexStrings = new Regex(@"(?<name>\S+?)\s*?[:=]\s?(?<value>.+)");

		/// <summary>
		/// Gets or sets the file name. After setting the class will be re-initialized.
		/// </summary>
		/// <exception cref="System.ArgumentNullException"></exception>
		public static string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				if (!String.IsNullOrEmpty(value))
				{
					fileName = value;
					// We have to re-initialize
					Initialize();
				}
				else if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				// No exception if value is empty
			}
		}

		/// <summary>
		/// This method extracts the strings from the assembly to the current directory
		/// </summary>
		public static void Extract()
		{
			foreach (string resource in assembly.GetManifestResourceNames())
			{
				if (resource.Contains(FileName) && resource.EndsWith(".txt"))
				{
					using (var sr = new StreamReader(assembly.GetManifestResourceStream(resource)))
					{
						string resourceFileName = resource.Substring(resource.IndexOf(ns) + ns.Length + 1);

						using (var sw = new StreamWriter(appPath + Path.DirectorySeparatorChar + resourceFileName, false, Encoding.UTF8))
						{
							sw.AutoFlush = true;
							sw.WriteLine("; " + assembly.GetName().Version.ToString());
							while (!sr.EndOfStream)
							{
								sw.WriteLine(sr.ReadLine());
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// This method should be called as early as possible so we could tell the user what's really wrong.
		/// However, if it's not called it's not THAT worse.
		/// </summary>
		/// <exception cref="System.Resources.MissingManifestResourceException">There is no default file with localizations.</exception>
		public static void Initialize()
		{
			string[] resources = assembly.GetManifestResourceNames();
			var success = false;
			var successEn = false;
			for (int i = 0; i < resources.Length; i++)
			{
				success |= resources[i].Contains(FileName + "_" + currentCulture + ".txt");
				successEn |= resources[i].Contains(FileName + "_en.txt") || resources[i].Contains(FileName + ".txt");

				if (success && successEn)
				{
					break;
				}
			}
			// This should never happen.. if so this is critical
			if (!successEn)
			{
				throw new System.Resources.MissingManifestResourceException();
			}
			else if (!success)
			{
				currentCulture = "en";
			}

			if (!File.Exists(appPath + Path.DirectorySeparatorChar + FileName + "_" + currentCulture + ".txt") && !File.Exists(appPath + Path.DirectorySeparatorChar + FileName + ".txt"))
			{
				Extract();
			}
			bool underscore = File.Exists(appPath + Path.DirectorySeparatorChar + FileName + "_" + currentCulture + ".txt");
			bool oldFile = false;
			using (var sr = new StreamReader(appPath + Path.DirectorySeparatorChar + FileName + ((underscore) ? "_" + currentCulture : "") + ".txt"))
			{
				if (sr.ReadLine() != "; " + assembly.GetName().Version.ToString())
				{
					oldFile = true;
				}
			}
			if (oldFile)
			{
				Extract();
			}
		}

		/// <summary>
		/// Initializes the dictionary used for the strings for a fast lookup.
		/// </summary>
		/// <param name="locale">The two-letter-string of the culture of which the string should be returned.</param>
		public static void InitializeDictionary(string locale = null)
		{
			if (locale == null)
			{
				locale = currentCulture ?? "en";
			}

			strings = new Dictionary<string, string>();
			Initialize();

			bool underscoreEn = false;
			if (File.Exists(appPath + Path.DirectorySeparatorChar + FileName + "_en.txt"))
			{
				underscoreEn = true;
			}

			string file = FileName;
			string fileEn = FileName + ((underscoreEn) ? "_en.txt" : ".txt");
			if (locale != "en")
			{
				file += "_" + locale + ".txt";
			}
			else
			{
				file = fileEn;
			}

			using (var sr = new StreamReader(appPath + Path.DirectorySeparatorChar + file, true))
			{
				while (!sr.EndOfStream)
				{
					var text = sr.ReadLine();
					// Comments will be ignored
					if (text.StartsWith("#") || text.StartsWith(";") || text.StartsWith("//") || text.StartsWith("*"))
					{
						continue;
					}
					var match = regexStrings.Match(text);
					if (!match.Success)
					{
						continue;
					}
					strings.Add(match.Groups["name"].Value, match.Groups["value"].Value);
				}
			}

			if (locale != "en")
			{
				using (var sr = new StreamReader(appPath + Path.DirectorySeparatorChar + fileEn, true))
				{
					while (!sr.EndOfStream)
					{
						var text = sr.ReadLine();
						// Comments will be ignored
						if (text.StartsWith("#") || text.StartsWith(";") || text.StartsWith("//") || text.StartsWith("*"))
						{
							continue;
						}
						var match = regexStrings.Match(text);
						if (!match.Success)
						{
							continue;
						}
						if (!strings.ContainsKey(match.Groups["name"].Value))
						{
							strings.Add(match.Groups["name"].Value, match.Groups["value"].Value);
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets a localized string.
		/// </summary>
		/// <param name="name">The name of the string.</param>
		/// <param name="locale">The two-letter-string of the culture of which the string should be returned.</param>
		/// <param name="fileWithoutSuffix"><c>false</c> if file should be in the normal format: <c>file_locale.txt</c>, <c>true</c> if it should be just <c>file.txt</c>.
		/// Please don't speify that parameter by yourself.</param>
		/// <returns>Returns the localized string in the given culture or in english if it wasn't found in the given culture.</returns>
		/// <exception cref="System.ArgumentNullException">name is null.</exception>
		public static string GetLocalizedString(string name, bool fileWithoutSuffix = false)
		{
			// Check if dictionary is initialized and if not, do that
			if (strings.Count == 0)
			{
				InitializeDictionary();
			}
			// If name is null we won't find any string...
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			string result;
			if (!strings.TryGetValue(name, out result))
			{
				return null;
			}
			return result;
		}

		/// <summary>
		/// Gets a localized and formatted string.
		/// </summary>
		/// <param name="name">The name of the string.</param>
		/// <param name="args">A object array with 0 or more objects to format.</param>
		/// <returns>Returns the localized and formatted string in the given culture or in english if it wasn't found in the given culture.</returns>
		/// <exception cref="System.ArgumentNullException">name is null.</exception>
		/// <exception cref="System.IO.FileNotFoundException">The file wasn't found in the resources.</exception>
		public static string GetLocalizedFormatString(string name, params object[] args)
		{
			return String.Format(GetLocalizedString(name), args);
		}
	}
}
