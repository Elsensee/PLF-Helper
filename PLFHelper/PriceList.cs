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

namespace PLFHelper
{
	/// <summary>
	/// Description of PriceList.
	/// </summary>
	public class PriceList : IDisposable
	{
		protected SpreadsheetEntry currentSpreadsheet;
		protected WorksheetEntry currentWorksheet;
		private bool disposed;
		private const uint FIRST_ROW = 3u;
		private SpreadsheetHelper helper;
		protected string languageServer;
		// Verbatim string to avoid double backslash
		protected readonly Regex serverRegex = new Regex(@"Server\s+((?<lang>EN|NL)\s+)?(?<server>(?(lang)(\d)|(\d{2})))", RegexOptions.ExplicitCapture);
		protected SpreadsheetEntry[] spreadsheets;
		protected WorksheetEntry[] worksheets;

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

		public PriceList(string username, string password)
		{
			this.helper = new SpreadsheetHelper(username, password);
			password = null;
		}

		public PriceList(SpreadsheetsService service)
		{
			this.helper = new SpreadsheetHelper(service);
		}

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

		public void SelectSpreadsheet(SpreadsheetEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			// Just in case...
			if (!this.serverRegex.IsMatch(entry.Title.Text))
			{
				// TODO: Add text
				throw new ArgumentException();
			}

			if (this.currentSpreadsheet != entry)
			{
				// Set everything to null so the cache works properly.
				this.worksheets = null;
				this.currentWorksheet = null;
				this.currentSpreadsheet = entry;
			}
		}

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

		public void SelectWorksheet(WorksheetEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			this.currentWorksheet = entry;
		}

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

		public void UpdateValues(uint column, int[] order, float[] values)
		{
			if (order.Length != values.Length)
			{
				// TODO: Add text
				throw new ArgumentException();
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
				cellEntries.Add(new CellEntry(FIRST_ROW + (uint)i, column, values[order[i]].ToString(ciInfo)));
			}

			this.helper.UpdateCells(this.currentWorksheet, cellEntries);
		}

		#region IDisposable implementation
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Every code example contains this function aswell.
		// So I included it here - don't know why we need this,
		// so no documentation here. lol.
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.helper != null)
					{
						this.helper.Dispose();
						this.helper = null;
					}
				}
				this.disposed = true;
			}
		}
		#endregion
	}
}
