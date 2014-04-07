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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using Google.GData.Spreadsheets;
using System;

namespace PLFHelper
{
	/// <summary>
	/// Can hold data for restoring if application crashes.
	/// </summary>
	[Serializable()]
	internal class RestoreData
	{
		/// <summary>
		/// Gets/sets the current setting object.
		/// </summary>
		public Settings Settings { get; set; }

		/// <summary>
		/// Gets/sets an array of SpreadsheetEntry objects.
		/// </summary>
		public SpreadsheetEntry[] SheetEntries { get; set; }

		/// <summary>
		/// Gets/sets the current SpreadsheetsService object.
		/// </summary>
		public SpreadsheetsService SheetService { get; set; }

		/// <summary>
		/// Gets/sets the current value array.
		/// </summary>
		public float[] Values { get; set; }

		/// <summary>
		/// Creates a new instance of the RestoreData class.
		/// </summary>
		/// <param name="settings">The current Settings object.</param>
		/// <param name="values">The current float-array with the values.</param>
		/// <param name="sheetEntries">The current array of SpreadsheetEntry objects.</param>
		/// <param name="sheetService">The current SpreadsheetService object.</param>
		public RestoreData(Settings settings, float[] values, SpreadsheetEntry[] sheetEntries, SpreadsheetsService sheetService)
		{
			this.Settings = settings;
			this.Values = values;
			this.SheetEntries = sheetEntries;
			this.SheetService = SheetService;
		}

		/// <summary>
		/// Saves the current settings object to <paramref name="path"/>.
		/// </summary>
		/// <param name="path">The path, the current settings object should be saved to.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
		public void Save(string path)
		{
			SaveRestoreData(this, path);
		}

		/// <summary>
		/// Saves the given <paramref name="restoreDataObject"/> to <paramref name="path"/>.
		/// </summary>
		/// <param name="restoreDataObject">The RestoreData object that sould be saved.</param>
		/// <param name="path">The path, the given RestoreData object should be saved to.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="restoreDataObject"/> or <paramref name="path"/> is null.</exception>
		public static void SaveRestoreData(RestoreData restoreDataObject, string path)
		{
			if (restoreDataObject == null)
			{
				throw new ArgumentNullException("restoreDataObject");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}

			try
			{
				using (var fileStream = new FileStream(path, FileMode.Create))
				{
					// Yup - we don't need that object after that so just... don't save it
					new BinaryFormatter().Serialize(fileStream, restoreDataObject);
				}
			}
			// Not the finest way, but it's okay to just say nothing...
			// We don't want to bother the user with that.
			catch (SecurityException) { }
		}

		/// <summary>
		/// Loads a RestoreData object from a given <paramref name="path"/>.
		/// </summary>
		/// <param name="path">The path, the RestoreData object should be loaded from.</param>
		/// <returns>A new RestoreData object with the data loaded from the file given in <paramref name="path"/>.</returns>
		/// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
		/// <exception cref="System.IO.FileNotFoundException">The file in <paramref name="path"/> does not exist.</exception>
		public static RestoreData LoadRestoreData(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}

			try
			{
				using (var fileStream = new FileStream(path, FileMode.Open))
				{
					// FileStream will be closed anyway.
					return (RestoreData) new BinaryFormatter().Deserialize(fileStream);
				}
			}
			// See above...
			catch (SecurityException)
			{
				// we have to return something... but we can't do some random settings...
				// so we just return null.
				return null;
			}
		}
	}
}
