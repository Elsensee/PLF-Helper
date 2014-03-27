/*
 * Copyright (c) 2013-2014 Oliver Schramm
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

namespace Parser
{
	/// <summary>
	/// Provides a common parser.
	/// </summary>
	public abstract class Parser
	{
		protected CultureInfo ciInfo;
		protected Language lang;
		protected string[] lastSearchArray;
		protected string lastSearchElementInArray;
		protected int lastResultElementInArray = -1;
		protected int playersIndex = -1;
		protected int players1Index = -1;

		#region Properties
		public int PlayersIndex
		{
			get
			{
				return this.playersIndex;
			}

			set
			{
				if (value > -1)
				{
					this.playersIndex = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		public int Players1Index
		{
			get
			{
				return this.players1Index;
			}

			set
			{
				if (value > -1)
				{
					this.players1Index = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}
		#endregion

		public abstract bool Parse(string text, ref float[] values, string[] names);

		protected virtual string ReturnLangCode()
		{
			switch (this.lang)
			{
				case Language.DE:
					return "de-DE";
				case Language.EN:
					return "en-GB";
				case Language.NL:
					return "nl-NL";
				default:
					return null;
			}
		}

		protected virtual int SearchElementInArray(string[] searchArray, string searchString)
		{
			if (searchArray == this.lastSearchArray && String.Compare(searchString, lastSearchElementInArray, false) == 0)
			{
				return this.lastResultElementInArray;
			}
			for (int i = 0; i < searchArray.Length; i++)
			{
				if (String.Compare(searchString, searchArray[i], false) == 0)
				{
					this.lastSearchArray = searchArray;
					this.lastSearchElementInArray = searchString;
					this.lastResultElementInArray = i;
					return i;
				}
			}
			return -1;
		}
	}

	public enum Language
	{
		unknown = -1,
		EN = 0,
		DE = 1,
		NL = 2
	}
}
