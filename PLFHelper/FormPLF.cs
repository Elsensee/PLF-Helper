﻿/*
 * Copyright (c) 2013 Oliver Schramm
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
using System.Reflection;
using System.Windows.Forms;

using PLFHelper.Localization;

namespace PLFHelper
{
	/// <summary>
	/// This is the main form of the application
	/// </summary>
	public partial class FormPLF : Form
	{
		public readonly string title;

		/// <summary>
		/// Creates a new instance of the <c>FormPLF</c> class.
		/// </summary>
		/// <param name="internet"><c>true</c> if internet connection is available, <c>false</c> if not.</param>
		public FormPLF(bool internet)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			var titleAttribute = (AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), default(bool))[0];
			this.title = titleAttribute.Title;
		}

		/// <summary>
		/// Updates the title bar of the main window with information about the selected server.
		/// </summary>
		/// <param name="server">The server number.</param>
		/// <param name="serverLang">The language of this server.</param>
		public void UpdateTitleBar(int server = 0, string serverLang = "DE")
		{
			if (server > 0)
			{
				if (serverLang.ToUpper() == "DE")
				{
					serverLang = "";
				}
				this.Text = LocalizationManager.GetLocalizedFormatString("CurrentServer", this.title, ((serverLang != "") ? serverLang + " " : "") + server.ToString());
			}
			else
			{
				this.Text = this.title;
			}
		}
	}
}
