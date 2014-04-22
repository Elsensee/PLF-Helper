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
using System.Text.RegularExpressions;

using Google.GData.Spreadsheets;

using PLFHelper.Localization;

namespace PLFHelper
{
	/// <summary>
	/// Description of PriceList.
	/// </summary>
	public class PriceList
	{
		protected SpreadsheetEntry currentSpreadsheet;
		protected WorksheetEntry currentWorksheet;
		private const uint FIRST_ROW = 3u;
		private SpreadsheetHelper helper;
		protected string languageServer;
		// Verbatim string to avoid double backslash
		protected readonly Regex serverRegex = new Regex(@"Server\s+((?<lang>EN|NL)\s+)?(?<server>(?(lang)(\d)|(\d{2})))", RegexOptions.ExplicitCapture);
		protected SpreadsheetEntry[] spreadsheets;
		protected WorksheetEntry[] worksheets;

		/// <summary>
		/// Gets the name of the current Spreadsheet.
		/// </summary>
		/// <exception cref="System.NullReferenceException"></exception>
		public string SpreadsheetName
		{
			get
			{
				if (this.currentSpreadsheet == null)
				{
					throw new NullReferenceException();
				}
				return this.currentSpreadsheet.Title.Text;
			}
		}

		/// <summary>
		/// Creates a new instance of the PriceList class with a username and a password to login.
		/// </summary>
		/// <param name="username">The username which should be used to login.</param>
		/// <param name="password">The password which should be used to login.</param>
		public PriceList(string username, string password)
		{
			this.helper = new SpreadsheetHelper(username, password);
			password = null;
		}

		/// <summary>
		/// Creates a new instance of the PriceList class with an already existing <typeparamref name="Google.GData.Spreadsheets.SpreadsheetsService" /> object.
		/// </summary>
		/// <param name="service">The <typeparamref name="Google.GData.Spreadsheets.SpreadsheetsService" /> object with which the instance should be created.</param>
		/// <exception cref="System.ArgumentException">The service doesn't provide any authentication</exception>
		public PriceList(SpreadsheetsService service)
		{
			this.helper = new SpreadsheetHelper(service);
		}

		/// <summary>
		/// Logs the user out.
		/// </summary>
		public void Logout()
		{
			this.helper.Logout();
		}

		/// <summary>
		/// Gets Pricelists accessible by the user.
		/// </summary>
		/// <returns>Returns a <typeparamref name="Google.GData.Spreadsheets.SpreadsheetEntry" /> array.</returns>
		/// <exception cref="System.NullReferenceException">helper is null.</exception>
		public SpreadsheetEntry[] GetSpreadsheets()
		{
			if (this.spreadsheets != null)
			{
				return this.spreadsheets;
			}
			if (this.helper == null)
			{
				throw new NullReferenceException();
			}

			// Gets every spreadsheet that has a 'Server ' in their name
			var tempResult = this.helper.GetSpreadsheets("Server ");
			var realResult = new List<SpreadsheetEntry>(tempResult.Length);
			foreach (var resultEntry in tempResult)
			{
				if (this.serverRegex.IsMatch(resultEntry.Title.Text))
				{
					realResult.Add(resultEntry);
				}
			}

			return this.spreadsheets = realResult.ToArray();
		}

		/// <summary>
		/// Selects a spreadsheet.
		/// </summary>
		/// <param name="entry">The <typeparamref name="Google.GData.Spreadsheets.SpreadsheetEntry" /> which should be selected.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="entry"/> is null.</exception>
		/// <exception cref="System.ArgumentException"><paramref name="entry"/> is no pricelist.</exception>
		public void SelectSpreadsheet(SpreadsheetEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			// Just in case...
			if (!this.serverRegex.IsMatch(entry.Title.Text))
			{
				throw new ArgumentException(LocalizationManager.GetLocalizedFormatString("IsNoPricelist", entry.Title.Text));
			}

			if (this.currentSpreadsheet != entry)
			{
				// Set everything to null so the cache works properly.
				this.worksheets = null;
				this.currentWorksheet = null;
				this.currentSpreadsheet = entry;
			}
		}

		/// <summary>
		/// Gets worksheets available in the currently selected spreadsheet.
		/// </summary>
		/// <returns>Returns a <typeparamref name="Google.GData.Spreadsheets.WorksheetEntry" /> array.</returns>
		/// <exception cref="System.NullReferenceException">currentSpreadsheet is null or helper is null.</exception>
		public WorksheetEntry[] GetWorksheets()
		{
			if (this.worksheets != null)
			{
				return this.worksheets;
			}
			if (this.currentSpreadsheet == null || this.helper == null)
			{
				throw new NullReferenceException();
			}

			return this.worksheets = this.helper.GetWorksheets(this.currentSpreadsheet);
		}

		/// <summary>
		/// Selects a worksheet.
		/// </summary>
		/// <param name="entry">The <typeparamref name="Google.GData.Spreadsheets.WorksheetEntry" /> which should be selected.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="entry"/> is null.</exception>
		public void SelectWorksheet(WorksheetEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			this.currentWorksheet = entry;
		}

		/// <summary>
		/// Gets cells in the currently selected worksheet.
		/// </summary>
		/// <param name="queryType">Can be all for requesting all cells, DateRow for only request cells in the date row or PlantsColumn for only request cells in the column with plants names.</param>
		/// <returns>Returns <typeparamref name="Google.GData.Spreadsheets.CellEntry" /> array.</returns>
		/// <exception cref="System.NullReferenceException">currentWorksheet is null or helper is null.</exception>
		/// <exception cref="System.ArgumentNullException">An unspecified <paramref name="queryType"/> was given.</exception
		public CellEntry[] GetCells(CellQueryType queryType = CellQueryType.All)
		{
			if (this.currentWorksheet == null || this.helper == null)
			{
				throw new NullReferenceException();
			}

			CellQuery query = null;
			if ((queryType & CellQueryType.All) == CellQueryType.All)
			{
				query = new CellQuery(this.currentWorksheet.CellFeedLink);
				query.ReturnEmpty = ReturnEmptyCells.yes;
			}
			else if ((queryType & CellQueryType.DateRow) == CellQueryType.DateRow)
			{
				query = new CellQuery(this.currentWorksheet.CellFeedLink);
				query.ReturnEmpty = ReturnEmptyCells.yes;
				// Get only one row
				query.MinimumRow = query.MaximumRow = 2;
			}
			else if ((queryType & CellQueryType.PlantsColumn) == CellQueryType.PlantsColumn)
			{
				query = new CellQuery(this.currentWorksheet.CellFeedLink);
				query.ReturnEmpty = ReturnEmptyCells.yes;
				// Get only one column
				query.MinimumColumn = query.MaximumColumn = 1;
			}

			return this.helper.GetCells(query);
		}

		/// <summary>
		/// Gets cells in the currently selected worksheet in a given column.
		/// </summary>
		/// <param name="column">The column from which the cells should be returned.</param>
		/// <returns>Returns <typeparamref name="Google.GData.Spreadsheets.CellEntry" /> array.</returns>
		/// <exception cref="System.NullReferenceException">currentWorksheet is null or helper is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">column is out of range.</exception>
		public CellEntry[] GetCellsColumn(uint column)
		{
			if (this.currentWorksheet == null || this.helper == null)
			{
				throw new NullReferenceException();
			}
			if (column == 0)
			{
				throw new ArgumentOutOfRangeException("column");
			}

			var query = new CellQuery(this.currentWorksheet.CellFeedLink);
			query.ReturnEmpty = ReturnEmptyCells.yes;
			query.MinimumColumn = query.MaximumColumn = column;

			return this.helper.GetCells(query);
		}

		/// <summary>
		/// Updates cells in the currently selected worksheet.
		/// </summary>
		/// <param name="column">The column in which the cells should be updated.</param>
		// Left order and values undocumented on purpose.
		public void UpdateValues(uint column, int[] order, float[] values)
		{
			if (order.Length != values.Length)
			{
				throw new ArgumentException(LocalizationManager.GetLocalizedFormatString("LengthNotEqualToLength", "order", "values"));
			}

			var ciInfo = CultureInfo.GetCultureInfo("de-DE");
			if (this.languageServer.ToUpper() == "EN")
			{
				ciInfo = CultureInfo.GetCultureInfo("en-GB");
			}
			else if (this.languageServer.ToUpper() == "NL")
			{
				ciInfo = CultureInfo.GetCultureInfo("nl-NL");
			}

			var cellEntries = new List<CellEntry>(values.Length);
			for (int i = 0; i < order.Length; i++)
			{
				cellEntries.Add(new CellEntry(FIRST_ROW + (uint) i, column, values[order[i]].ToString(ciInfo)));
			}

			this.helper.UpdateCells(this.currentWorksheet, cellEntries);
		}
	}
}
