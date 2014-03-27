﻿/*
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

namespace Parser
{
	/// <summary>
	/// Provides the parser for Molehill Empire.
	/// </summary>
	public sealed class ParserMH : Parser
	{
		private readonly string[,] backForward = { { "<<< back", "forward >>>" }, { "<<< zurück", "weiter >>>" }, { "<<< terug", "verder >>>" } };
		private readonly string[] currency = { "wT", "gB", "gB" };
		private readonly string[] currentOffers = { "Current offers", "Aktuelle Angebote", "Huidige aanbiedingen" };
		private readonly string[] deleteFilter = { "[Delete filter - show all offers]", "[Filter löschen - alle Angebote zeigen]", "[Verwijder filter - laat alle aanbiedingen zien]" };
		private readonly string[] listOfAllPlayersPoints = { "List of all players according to score", "Liste aller Spieler nach Punktzahl", "Lijst met alle spelers gesorteerd op de hoogte van de scores" };
		private readonly string[] playersTotal = { "Players total:", "Spieler gesamt:", "Spelers totaal:" };
		private readonly string[] showMyRanking = { "Show my ranking", "wo bin ich?", "Waar ben ik?" };
		private readonly string[] total = { "Total", "Gesamt", "Totaal" };
		private readonly string[] twoWords = { "Red cabbage|Red currants|Gerber daisy|Cow lily|Water parsnip|Water violet|Water soldier|Water lily|Water knotweed|Marsh marigold|Swamp lantern|Angel's trumpet", "gelbe Teichrose", "Koe lelie|Water pastinaak|Rode aalbes|Rode kool" };
		private readonly string[] welcomeOnBigMarketPlace = { "Welcome to the market place!", "Willkommen auf dem großen Marktplatz!", "Welkom op de marktplaats!" };

		#region Properties
		/// <summary>
		/// Gets the language specific value for the "Back" string.
		/// </summary>
		private string Back
		{
			get
			{
				return (this.lang != Language.unknown) ? this.backForward[(int) this.lang, 0] : null;
			}
		}

		/// <summary>
		/// Gets the language specific value for the "Forward" string.
		/// </summary>
		private string Forward
		{
			get
			{
				return (this.lang != Language.unknown) ? this.backForward[(int) this.lang, 1] : null;
			}
		}

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
		/// Gets the language specific value for the "Current offers" string.
		/// </summary>
		private string CurrentOffers
		{
			get
			{
				return (this.lang != Language.unknown) ? this.currentOffers[(int) this.lang] : null;
			}
		}

		/// <summary>
		/// Gets the language specific value for the "Delete filter" string.
		/// </summary>
		private string DeleteFilter
		{
			get
			{
				return (this.lang != Language.unknown) ? this.deleteFilter[(int) this.lang] : null;
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
		/// Gets the language specific value for the "Show my ranking" string.
		/// </summary>
		private string ShowMyRanking
		{
			get
			{
				return (this.lang != Language.unknown) ? this.showMyRanking[(int) this.lang] : null;
			}
		}

		/// <summary>
		/// Gets the language specific value for the "Total" string.
		/// </summary>
		private string Total
		{
			get
			{
				return (this.lang != Language.unknown) ? this.total[(int) this.lang] : null;
			}
		}

		/// <summary>
		/// Gets the language specific value for all two words products.
		/// </summary>
		private string TwoWords
		{
			get
			{
				return (this.lang != Language.unknown) ? this.twoWords[(int) this.lang] : null;
			}
		}

		/// <summary>
		/// Gets the language specific value for the "Welcome on big marketplace" string.
		/// </summary>
		private string WelcomeOnBigMarketplace
		{
			get
			{
				return (this.lang != Language.unknown) ? this.welcomeOnBigMarketPlace[(int) this.lang] : null;
			}
		}
		#endregion
		#region Constructors
		/// <summary>
		/// Creates a new instance of the ParserMH class.
		/// </summary>
		/// <param name="lang">The language with which the parser should work.</param>
		public ParserMH(Language lang)
		{
			if (lang == Language.unknown)
			{
				throw new ArgumentException("No valid language given!", "lang");
			}
			this.lang = lang;

			this.ciInfo = new CultureInfo(this.ReturnLangCode());
		}

		/// <summary>
		/// Creates a new instance of the ParserMH class.
		/// </summary>
		/// <param name="lang">The language with which the parser should work.</param>
		/// <param name="playersIndex">The index of the players value in the values array.</param>
		/// <param name="players1Index">The index of the last player with one points value in the values array.</param>
		public ParserMH(Language lang, int playersIndex, int players1Index) : this(lang)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}

		/// <summary>
		/// Creates a new instance of the ParserMH class.
		/// </summary>
		/// <param name="lang">The language with which the parser should work.</param>
		public ParserMH(string lang)
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

			this.ciInfo = new CultureInfo(this.ReturnLangCode());
		}

		/// <summary>
		/// Creates a new instance of the ParserMH class.
		/// </summary>
		/// <param name="lang">The language with which the parser should work.</param>
		/// <param name="playersIndex">The index of the players value in the values array.</param>
		/// <param name="players1Index">The index of the last player with one points value in the values array.</param>
		public ParserMH(string lang, int playersIndex, int players1Index) : this(lang)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}
		#endregion

		// This one gets its documentation from the abstract function in the base class "Parser".
		public override bool Parse(string text, ref float[] values, string[] names)
		{
			if (this.PlayersIndex > -1 && this.Players1Index > -1)
			{
				var plantRegex = new Regex(@"\n[\d\.]+\s+(?<product>(" + this.TwoWords + @"|\S+))\s+.+?\s+(?<value>[\d\.,]{3,}) " + Regex.Escape(this.Currency) + @"\s+[\d\.,]{3,} " + Regex.Escape(this.Currency) + @"\s+[^\n]+\n", RegexOptions.IgnoreCase);
				var playerRegex = new Regex(Regex.Escape(this.PlayersTotal) + @"\s+(?<player>\d+)\s+" + Regex.Escape(this.ShowMyRanking));

				if (plantRegex.IsMatch(text))
				{
					return this.ParseMarket(plantRegex.Match(text), ref values, names);
				}
				else if (text.IndexOf(this.ListOfAllPlayersPoints) > -1 && playerRegex.IsMatch(text))
				{
					return this.ParseTownHall(playerRegex.Match(text), text, ref values);
				}
			}
			return false;
		}

		/// <summary>
		/// Parses the market in molehill empire.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <param name="values">The values array by reference.</param>
		/// <param name="names">The names array.</param>
		/// <returns><c>true</c> if parsing was successful, <c>false</c> if not.</returns>
		protected bool ParseMarket(Match match, ref float[] values, string[] names)
		{
			int nameIndex = this.SearchElementInArray(names, match.Groups["product"].Value);
			if (nameIndex == -1)
			{
				return false;
			}

			float temp = values[nameIndex];
			values[nameIndex] = Single.Parse(match.Groups["value"].Value, this.ciInfo);
			return (values[nameIndex] != temp); // Easy, huh?
		}

		/// <summary>
		/// Parses the town hall in molehill empire.
		/// </summary>
		/// <param name="match">The match.</param>
		/// <param name="text">The text which should be parsed.</param>
		/// <param name="values">The values array by reference.</param>
		/// <returns><c>true</c> if parsing was successful, <c>false</c> if not.</returns>
		protected bool ParseTownHall(Match match, string text, ref float[] values)
		{
			float tempPlayer = values[this.PlayersIndex];
			float tempPlayer1Index = values[this.Players1Index];
			values[this.PlayersIndex] = Single.Parse(match.Groups["player"].Value, this.ciInfo);

			// Let's move on to the last player with one point:
			var player1PointRegex = new Regex(@"(?<position>\d+)\..+\s+1\s*\n");
			if (player1PointRegex.IsMatch(text))
			{
				MatchCollection player1PointMatches = player1PointRegex.Matches(text);
				Match player1PointMatch = player1PointMatches[player1PointMatches.Count - 1];
				values[this.Players1Index] = Single.Parse(player1PointMatch.Groups["position"].Value, this.ciInfo);
			}
			// If ANYTHING changed return true
			return (tempPlayer != values[this.PlayersIndex]) || (tempPlayer1Index != values[this.Players1Index]);
		}
	}
}
