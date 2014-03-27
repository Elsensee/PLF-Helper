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
using System.Resources;
using System.Text.RegularExpressions;

namespace PLFHelper
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
		private static readonly Regex regexStrings = new Regex("(?<name>\\S+?)\\s*?=\\s?(?<value>.+)");

		/// <summary>
		/// Gets or sets the file name. After setting the class will be re-initialized.
		/// </summary>
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
					// We have to re-initialize this
					Initialize();
				}
			}
		}

		/// <summary>
		/// This method should be called as early as possible so we could tell the user what's really wrong.
		/// However, if it's not called it's not THAT worse.
		/// </summary>
		/// <exception cref="MissingManifestResourceException">There was no default file with localizations found.</exception>
		public static void Initialize()
		{
			string[] resources = assembly.GetManifestResourceNames();
			bool success = false;
			bool successEn = false;
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
				throw new MissingManifestResourceException();
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
		/// <returns>Returns the localized string in the current culture.</returns>
		/// <exception cref="ArgumentNullException">name or locale is null.</exception>
		public static string GetLocalizedString(string name)
		{
			return GetLocalizedString(name, currentCulture);
		}

		/// <summary>
		/// Gets a localized string.
		/// </summary>
		/// <param name="name">The name of the string.</param>
		/// <param name="locale">The two-letter-string of the culture the string should be returned.</param>
		/// <returns>Returns the localized string in the given culture.</returns>
		/// <exception cref="ArgumentNullException">name or locale is null.</exception>
		public static string GetLocalizedString(string name, string locale)
		{
			if (name == null || locale == null)
			{
				throw new ArgumentNullException((name == null) ? "name" : "locale");
			}

			try
			{
				using (var sr = new StreamReader(assembly.GetManifestResourceStream(typeof(LocalizationManager).Namespace + "." + fileName + "_" + locale + ".txt")))
				{
					while (!sr.EndOfStream)
					{
						string text = sr.ReadLine();
						// One line comments will be ignored
						if (text.StartsWith("#") || text.StartsWith(";") || text.StartsWith("//") || text.StartsWith("*"))
						{
							continue;
						}
						Match match = regexStrings.Match(text);
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
					return GetLocalizedString(name, "en");
				}
				throw;
			}

			return (locale != "en") ? GetLocalizedString(name, "en") : null;
		}
	}
}
