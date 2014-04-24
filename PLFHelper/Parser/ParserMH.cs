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
using System.Text.RegularExpressions;

using PLFHelper.Localization;

namespace PLFHelper.Parser
{
	/// <summary>
	/// Provides the parser for Molehill Empire.
	/// </summary>
	public sealed class ParserMH : Parser
	{
		private readonly string[] currency = { "wT", "gB", "gB" };
		private readonly string[] listOfAllPlayersPoints = { "List of all players according to score", "Liste aller Spieler nach Punktzahl", "Lijst met alle spelers gesorteerd op de hoogte van de scores" };
		private readonly string[] names;
		private readonly string[] playersTotal = { "Players total:", "Spieler gesamt:", "Spelers totaal:" };
		private readonly string twoWords;
		// Regular expressions
		private readonly Regex regexMarket;
		private readonly Regex regexPlayerOnePoint;
		private readonly Regex regexTownHall;

		#region Properties
		/// <summary>
		/// Gets the language specific value for the in game currency.
		/// </summary>
		private string Currency
		{
			get
			{
				return (this.lang != Language.unknown) ? this.currency[(int) this.lang] : null;
			}
		}

		/// <summary>
		/// Gets the language specific value for the "List of all players points" string.
		/// </summary>
		private string ListOfAllPlayersPoints
		{
			get
			{
				return (this.lang != Language.unknown) ? this.listOfAllPlayersPoints[(int) this.lang] : null;
			}
		}

		/// <summary>
		/// Gets the language specific value for the "Players total" string.
		/// </summary>
		private string PlayersTotal
		{
			get
			{
				return (this.lang != Language.unknown) ? this.playersTotal[(int) this.lang] : null;
			}
		}

		/// <summary>
		/// Gets if the parsing was successful
		/// </summary>
		public bool Successful
		{
			get; private set;
		}
		#endregion
		#region Constructors
		/// <summary>
		/// Creates a new instance of the ParserMH class.
		/// </summary>
		/// <param name="lang">The language with which the parser should work.</param>
		/// <param name="names">String array of all the plant names.</param>
		public ParserMH(Language lang, string[] names)
		{
			LocalizationManager.Initialize();

			if (lang == Language.unknown)
			{
				throw new ArgumentException(LocalizationManager.GetLocalizedString("NoValidLanguage"), "lang");
			}
			this.lang = lang;

			this.ciInfo = new CultureInfo(this.ReturnLangCode());

			this.names = names;
			this.twoWords = FilterTwoWords(names) + "|";

			// Create regular expressions we need once so we don't have to recompile them everytime we want to use them.
			string escapedCurrency = Regex.Escape(this.Currency);
			this.regexMarket = new Regex(@"^[\d.,]+\s+(?<product>" + this.twoWords + @"\S+).+(?<value>[\d.,]{3,}) " + escapedCurrency + @"\s+[\d.,]{3,} " + escapedCurrency + ".+$", RegexOptions.Multiline);
			this.regexPlayerOnePoint = new Regex(@"^(?<position>\d+)[.,].+\s+1$", RegexOptions.Multiline | RegexOptions.RightToLeft);
			this.regexTownHall = new Regex("^" + Regex.Escape(this.PlayersTotal) + @" (?<player>\d+).+$", RegexOptions.Multiline);
		}

		/// <summary>
		/// Creates a new instance of the ParserMH class.
		/// </summary>
		/// <param name="lang">The language with which the parser should work.</param>
		/// <param name="names">String array of all the plant names.</param>
		/// <param name="playersIndex">The index of the players value in the values array.</param>
		/// <param name="players1Index">The index of the last player with one points value in the values array.</param>
		public ParserMH(Language lang, string[] names, int playersIndex, int players1Index) : this(lang, names)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}

		/// <summary>
		/// Creates a new instance of the ParserMH class.
		/// </summary>
		/// <param name="lang">The language with which the parser should work.</param>
		/// <param name="names">String array of all the plant names.</param>
		public ParserMH(string lang, string[] names)
		{
			LocalizationManager.Initialize();

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
					throw new ArgumentException(LocalizationManager.GetLocalizedString("NoValidLanguage"), "lang");
			}

			this.ciInfo = new CultureInfo(this.ReturnLangCode());

			this.names = names;
			this.twoWords = FilterTwoWords(names) + "|";

			// Create regular expressions we need once so we don't have to recompile them everytime we want to use them.
			string escapedCurrency = Regex.Escape(this.Currency);
			this.regexMarket = new Regex(@"^[\d.,]+\s+(?<product>" + this.twoWords + @"\S+).+(?<value>[\d.,]{3,}) " + escapedCurrency + @"\s+[\d.,]{3,} " + escapedCurrency + ".+$", RegexOptions.Multiline);
			this.regexPlayerOnePoint = new Regex(@"^(?<position>\d+)[.,].+\s+1$", RegexOptions.Multiline | RegexOptions.RightToLeft);
			this.regexTownHall = new Regex("^" + Regex.Escape(this.PlayersTotal) + @" (?<player>\d+).+$", RegexOptions.Multiline);
		}

		/// <summary>
		/// Creates a new instance of the ParserMH class.
		/// </summary>
		/// <param name="lang">The language with which the parser should work.</param>
		/// <param name="names">String array of all the plant names.</param>
		/// <param name="playersIndex">The index of the players value in the values array.</param>
		/// <param name="players1Index">The index of the last player with one points value in the values array.</param>
		public ParserMH(string lang, string[] names, int playersIndex, int players1Index) : this(lang, names)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}
		#endregion

		// This one gets its documentation from the abstract function in the base class "Parser".
		public override float[] Parse(string text, float[] values)
		{
			if (this.PlayersIndex > -1 && this.Players1Index > -1)
			{
				if (this.regexMarket.IsMatch(text))
				{
					return ParseMarket(this.regexMarket.Match(text), values);
				}
				else if (text.IndexOf(this.ListOfAllPlayersPoints) > -1 && this.regexTownHall.IsMatch(text))
				{
					return ParseTownHall(this.regexTownHall.Match(text), text, values);
				}
			}
			this.Successful = false;
			return values;
		}

		/// <summary>
		/// Parses the market in molehill empire.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <param name="values">The float array with all values for plants and... you know...</param>
		/// <returns>Returns the changed <paramref name="values"/> array.</returns>
		private float[] ParseMarket(Match match, float[] values)
		{
			int nameIndex = SearchElementInArray(this.names, match.Groups["product"].Value);
			if (nameIndex == -1)
			{
				this.Successful = false;
				return values;
			}

			float temp = values[nameIndex];
			values[nameIndex] = Single.Parse(match.Groups["value"].Value, this.ciInfo);
			this.Successful = (Math.Abs(values[nameIndex] - temp) > Single.Epsilon);
			return values;
		}

		/// <summary>
		/// Parses the town hall in molehill empire.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <param name="text">The text which should be parsed.</param>
		/// <param name="values">The float array with all values for plants and... you know...</param>
		/// <returns>Returns the changed <paramref name="values"/> array.</returns>
		private float[] ParseTownHall(Match match, string text, float[] values)
		{
			float tempPlayer = values[this.PlayersIndex];
			float tempPlayer1Index = values[this.Players1Index];
			values[this.PlayersIndex] = Single.Parse(match.Groups["player"].Value, this.ciInfo);

			// Let's move on to the last player with one point:
			if (this.regexPlayerOnePoint.IsMatch(text))
			{
				var player1PointMatch = this.regexPlayerOnePoint.Match(text);
				values[this.Players1Index] = Single.Parse(player1PointMatch.Groups["position"].Value, this.ciInfo);
			}
			// If ANYTHING set Successful to true.
			this.Successful = (Math.Abs(tempPlayer - values[this.PlayersIndex]) > Single.Epsilon) || (Math.Abs(tempPlayer1Index - values[this.Players1Index]) > Single.Epsilon);
			return values;
		}
	}
}
