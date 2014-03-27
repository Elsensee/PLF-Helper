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
		/// Gets/sets a string which describes the current game (for later versions).
		/// </summary>
		public string Game { get; set; }

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
		/// <param name="game">The string with the currently selected game.</param>
		/// <param name="sheetEntries">The current array of SpreadsheetEntry objects.</param>
		/// <param name="sheetService">The current SpreadsheetService object.</param>
		public RestoreData(Settings settings, float[] values, string game, SpreadsheetEntry[] sheetEntries, SpreadsheetsService sheetService)
		{
			this.Settings = settings;
			this.Values = values;
			this.Game = game;
			this.SheetEntries = sheetEntries;
			this.SheetService = SheetService;
		}
	}
}
