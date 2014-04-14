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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PLFHelper.Localization
{
	/// <summary>
	/// Own localization manager for getting localized strings.
	/// No need for satellite assemblies, but embedded text files.
	/// </summary>
	internal static class LocalizationManager
	{
		private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
		private static string currentCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
		private static string fileName = "strings";
		// Verbatim string to avoid double backslash
		private static readonly Regex regexStrings = new Regex(@"(?<name>\S+?)\s*?=\s?(?<value>.+)");

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
				success |= resources[i].Contains(fileName + "_" + currentCulture + ".txt");
				successEn |= resources[i].Contains(fileName + "_en.txt") || resources[i].Contains(fileName + ".txt");

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
		/// <exception cref="System.IO.FileNotFoundException">The file wasn't found in the res</exception>
		public static string GetLocalizedString(string name, string locale = null, bool fileWithoutSuffix = false)
		{
			// If name is null we won't find any string...
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			// Set this to currentCulture or en if locale is null
			if (locale == null)
			{
				locale = currentCulture ?? "en";
			}

			string file = (fileWithoutSuffix) ? FileName + ".txt" : FileName + "_" + locale + ".txt";

			try
			{
				using (var sr = new StreamReader(assembly.GetManifestResourceStream(typeof(LocalizationManager).Namespace + "." + file)))
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
						if (match.Groups["name"].Value == name)
						{
							return match.Groups["value"].Value;
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
				if (locale != "en")
				{
					return GetLocalizedString(name, "en", false);
				}
				else if (locale == "en" && !fileWithoutSuffix)
				{
					return GetLocalizedString(name, "en", true);
				}
				throw;
			}

			return (locale != "en") ? GetLocalizedString(name, "en", false) : ((locale == "en" && !fileWithoutSuffix) ? GetLocalizedString(name, "en", true) : null);
		}
	}
}
