/*
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
using System.Globalization;

namespace Parser
{
	/// <summary>
	/// Provides a common parser.
	/// </summary>
	public class Parser
	{
		CultureInfo ciInfo;
		Language lang;
		string[] lastSearchArray;
		string lastSearchElementInArray;
		int lastResultElementInArray = -1;
		int playersIndex = -1;
		int players1Index = -1;

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
					throw new ArgumentOutOfRangeException("value", "The value has to be greater than -1");
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
					throw new ArgumentOutOfRangeException("value", "The value has to be greater than -1");
				}
			}
		}
		#endregion
		#region Constructors
		protected Parser()
		{
			
		}
		public Parser(Language lang)
		{
			if (lang == Language.unknown)
			{
				throw new ArgumentException("No valid language given!", "lang");
			}
			this.lang = lang;

			this.ciInfo = new CultureInfo(ReturnLangCode());
		}
		public Parser(Language lang, int playersIndex, int players1Index) : this(lang)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}
		public Parser(string lang)
		{
			switch (lang.ToUpper())
			{
				case "EN":
					this.lang = Language.EN;
					break;
				case "DE":
					this.lang = Language.DE;
					break;
				case "NL":
					this.lang = Language.NL;
					break;
				default:
					throw new ArgumentException("No valid language given!", "lang");
			}

			this.ciInfo = new CultureInfo(ReturnLangCode());
		}
		public Parser(string lang, int playersIndex, int players1Index) : this(lang)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}
		#endregion

		public virtual bool Parse(string text, ref float[] prices, string[] plants, string[] currencies)
		{
			return false;
		}

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
				return lastResultElementInArray;
			}
			for (int i = 0; i < searchArray.Length; i++)
			{
				if (String.Compare(searchString, searchArray[i], false) == 0)
				{
					lastSearchArray = searchArray;
					lastSearchElementInArray = searchString;
					lastResultElementInArray = i;
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
