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
using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace PLFHelper
{
	/// <summary>
	/// Class which wraps the Spreadsheet classes provided by the Google Data API.
	/// </summary>
	public class SpreadsheetWrapper : IDisposable
	{
		protected const string appName = "Preistenlistenpfleger-Helper";
		protected CellFeed cellFeed;
		protected bool disposed = false;
		protected SpreadsheetsService service;

		/// <summary>
		/// Creates a new instance of the SpreadsheetWrapper class with a username and a password to login.
		/// </summary>
		/// <param name="username">The username which should be used to login.</param>
		/// <param name="password">The password which should be used to login.</param>
		public SpreadsheetWrapper(string username, string password)
		{
			this.service = new SpreadsheetsService(appName);
			this.service.setUserCredentials(username, password);
			password = null;
		}

		/// <summary>
		/// Creates a new instance of the SpreadsheetWrapper class with a already existing <typeparamref name="Google.GData.Spreadsheets.SpreadsheetsService" /> object.
		/// </summary>
		/// <param name="service">The <typeparamref name="Google.GData.Spreadsheets.SpreadsheetsService" /> object with which the instance should be created.</param>
		/// <exception cref="System.ArgumentException">The service doesn't provide any authentication</exception>
		public SpreadsheetWrapper(SpreadsheetsService service)
		{
			// Unfortunately we cannot check if password is not null.
			if (service.Credentials == null || service.Credentials.Username == null)
			{
				throw new ArgumentException(LocalizationManager.GetLocalizedString("ServiceNoAuthentication"), "service");
			}
			this.service = service;
		}

		/// <summary>
		/// Logs the user out.
		/// </summary>
		public void Logout()
		{
			this.service.setUserCredentials(null, null);
		}

		/// <summary>
		/// Get Spreadsheets accessible by the user. Can be filtered with <paramref name="title"/>.
		/// </summary>
		/// <param name="title">Filters the accessible spreadsheets. (optional)</param>
		/// <returns>Returns a <typeparamref name="Google.GData.Spreadsheets.SpreadsheetEntry" /> array.</returns>
		public virtual SpreadsheetEntry[] GetSpreadsheets(string title = null)
		{
			var query = new SpreadsheetQuery();
			if (title != null)
			{
				query.Title = title;
			}
			SpreadsheetFeed feed = this.service.Query(query);
			return (SpreadsheetEntry[]) feed.Entries;
		}

		/// <summary>
		/// Get Worksheets in the given <paramref name="spreadsheet"/>.
		/// </summary>
		/// <param name="spreadsheet">The <typeparamref name="Google.GData.Spreadsheets.SpreadsheetEntry" /> from which the worksheets should be returned.</param>
		/// <returns>Returns a <typeparamref name="Google.GData.Spreadsheets.WorksheetEntry" /> array.</returns>
		public virtual WorksheetEntry[] GetWorksheets(SpreadsheetEntry spreadsheet)
		{
			if (spreadsheet == null)
			{
				spreadsheet = GetSpreadsheets()[0];
			}

			return (WorksheetEntry[]) spreadsheet.Worksheets.Entries;
		}

		/// <summary>
		/// Get cells in the given <paramref name="worksheet" />.
		/// </summary>
		/// <param name="worksheet">The <typeparamref name="Google.GData.Spreadsheets.WorksheetEntry" /> from which the wcells should be returned.</param>
		/// <returns>Returns a <typeparamref name="Google.GData.Spreadsheets.CellEntry" /> array.</returns>
		public virtual CellEntry[] GetCells(WorksheetEntry worksheet)
		{
			if (worksheet == null)
			{
				worksheet = GetWorksheets()[0];
			}

			this.cellFeed = this.service.Query(new CellQuery(worksheet.CellFeedLink));
			return (CellEntry[]) this.cellFeed.Entries;
		}

		/// <summary>
		/// Updates a single <paramref name="cell"/> with <paramref name="content"/>
		/// </summary>
		/// <param name="cell">The <typeparamref name="Google.GData.Spreadsheets.CellEntry" /> which should be updated.</param>
		/// <param name="content">The updated value. (empty string for deleting the cell's content)</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="cell"/> is null.</exception>
		public virtual void UpdateCell(CellEntry cell, string content)
		{
			if (cell == null)
			{
				throw new ArgumentNullException("cell");
			}
			if (content == null)
			{
				content = String.Empty;
			}

			cell.InputValue = content;
			cell.Update();
		}

		/// <summary>
		/// Updates multiple cells in the given worksheet.
		/// </summary>
		/// <param name="worksheet">The <typeparamref name="Google.GData.Spreadsheets.WorksheetEntry" /> in which the cells should be updated.</param>
		/// <param name="cellEntries">A list of cellEntries, which contains updated values.</param>
		/// <exception cref="System.ArgumentNullException"><paramref name="worksheet"/> is null.</exception>
		/// <exception cref="System.ArgumentOutOfRangeException"><paramref name="cellEntries"/> contains 0 elements.</exception>
		/// <exception cref="System.NullReferenceException">this.cellFeed is null - that means GetCells() was never called.</exception>
		public virtual void UpdateCells(WorksheetEntry worksheet, IList<CellEntry> cellEntries)
		{
			// Catch some no-go's.
			if (worksheet == null)
			{
				throw new ArgumentNullException("worksheet");
			}
			if (cellEntries.Count == 0)
			{
				throw new ArgumentOutOfRangeException("cellEntries", cellEntries.Count);
			}
			if (cellEntries.Count == 1)
			{
				UpdateCell(cellEntries[0], cellEntries[0].InputValue);
				return;
			}
			if (this.cellFeed == null)
			{
				throw new NullReferenceException();
			}

			// Set up the batch request.
			var batchRequest = new CellFeed(new CellQuery(worksheet.CellFeedLink).Uri, this.service);
			foreach (var batchEntry in cellEntries)
			{
				batchEntry.BatchData = new GDataBatchEntryData(GDataBatchOperationType.update);
				batchRequest.Entries.Add(batchEntry);
			}
			// Submit it.
			var batchResponse = (CellFeed) this.service.Batch(batchRequest, new Uri(this.cellFeed.Batch));
			// Some debug code 
			#if DEBUG
			foreach (CellEntry entry in batchResponse.Entries)
			{
				System.Diagnostics.Debug.WriteLineIf(entry.BatchData.Status.Code != 200, entry.BatchData.Id + " failed: " + entry.BatchData.Status.Reason);
			}
			#endif
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
					if (this.service != null)
					{
						this.service.setUserCredentials(null, null);
						this.service = null;
					}
					if (this.cellFeed != null)
					{
						this.cellFeed != null;
					}
				}
				this.disposed = true;
			}
		}
		#endregion
	}
}
