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
using System.Text.RegularExpressions;

namespace Parser
{
	/// <summary>
	/// Provides the parser for Molehill Empire.
	/// </summary>
	public class ParserMH : Parser
	{
		string currentCurrency;
		CultureInfo ciInfo;
		Language lang;

		readonly string[] welcomeOnBigMarketPlace = { "Welcome to the market place!", "Willkommen auf dem großen Marktplatz!", "Welkom op de marktplaats!" };
		readonly string[] currentOffers = { "Current offers", "Aktuelle Angebote", "Huidige aanbiedingen" };
		readonly string[] total = { "Total", "Gesamt", "Totaal" };
		readonly string[] deleteFilter = { "[Delete filter - show all offers]", "[Filter löschen - alle Angebote zeigen]", "[Verwijder filter - laat alle aanbiedingen zien]" };
		readonly string[] listOfAllPlayersPoints = { "List of all players according to score", "Liste aller Spieler nach Punktzahl", "Lijst met alle spelers gesorteerd op de hoogte van de scores" };
		readonly string[] showMyRanking = { "Show my ranking", "wo bin ich?", "Waar ben ik?" };
		readonly string[,] backForward = { { "<<< back", "forward >>>" }, { "<<< zurück", "weiter >>>" }, { "<<< terug", "verder >>>" } };
		readonly string[] playersTotal = { "Players total:", "Spieler gesamt:", "Spelers totaal:" };
		readonly string[] twoWords = { "Red cabbage|Red currants|Gerber daisy|Cow lily|Water parsnip|Water violet|Water soldier|Water lily|Water knotweed|Marsh marigold|Swamp lantern|Angel's trumpet", "gelbe Teichrose", "Koe lelie|Water pastinaak|Rode aalbes|Rode kool" };

		#region Properties
		private string WelcomeOnBigMarketplace
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.welcomeOnBigMarketPlace[(int)this.lang];
				}
				return null;
			}
		}

		private string CurrentOffers
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.currentOffers[(int)this.lang];
				}
				return null;
			}
		}

		private string Total
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.total[(int)this.lang];
				}
				return null;
			}
		}

		private string DeleteFilter
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.deleteFilter[(int)this.lang];
				}
				return null;
			}
		}

		private string ListOfAllPlayersPoints
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.listOfAllPlayersPoints[(int)this.lang];
				}
				return null;
			}
		}

		private string ShowMyRanking
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.showMyRanking[(int)this.lang];
				}
				return null;
			}
		}

		private string Back
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.backForward[(int)this.lang, 0];
				}
				return null;
			}
		}

		private string Forward
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.backForward[(int)this.lang, 1];
				}
				return null;
			}
		}

		private string PlayersTotal
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.playersTotal[(int)this.lang];
				}
				return null;
			}
		}

		private string TwoWords
		{
			get
			{
				if (this.lang != Language.unknown)
				{
					return this.twoWords[(int)this.lang];
				}
				return null;
			}
		}
		#endregion
		#region Constructors
		public ParserMH(Language lang)
		{
			if (lang == Language.unknown)
			{
				throw new ArgumentException("No valid language given!", "lang");
			}
			this.lang = lang;

			this.ciInfo = new CultureInfo(this.ReturnLangCode());
		}

		public ParserMH(Language lang, int playersIndex, int players1Index) : this(lang)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}

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

		public ParserMH(string lang, int playersIndex, int players1Index) : this(lang)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}
		#endregion

		public override bool Parse(string text, ref float[] values, string[] names, string[] currencies)
		{
			if (this.PlayersIndex > -1 && this.Players1Index > -1)
			{
				if (this.currentCurrency == null)
				{
					this.currentCurrency = currencies[(int)this.lang];
				}

				Regex plantRegex = new Regex(@"\n[\d\.]+\s+(?<product>(" + this.TwoWords + @"|\S+))\s+.+?\s+(?<value>[\d\.,]{3,}) " + this.currentCurrency + @"\s+[\d\.,]{3,} " + this.currentCurrency + @"\s+[^\n]+\n", RegexOptions.IgnoreCase);
				Regex playerRegex = new Regex(Regex.Escape(this.PlayersTotal) + @"\s+(?<player>\d+)\s+" + Regex.Escape(this.ShowMyRanking));

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

		public bool ParseMarket(Match match, ref float[] values, string[] names)
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

		public bool ParseTownHall(Match match, string text, ref float[] values)
		{
			float tempPlayer = values[this.PlayersIndex];
			float tempPlayer1Index = values[this.Players1Index];
			values[this.PlayersIndex] = Single.Parse(match.Groups["player"].Value, this.ciInfo);

			Regex player1PointRegex = new Regex(@"(?<position>\d+)\..+\s+1\s*\n");
			if (player1PointRegex.IsMatch(text))
			{
				MatchCollection player1PointMatches = player1PointRegex.Matches(text);
				Match player1PointMatch = player1PointMatches[player1PointMatches.Count - 1];
				values[this.Players1Index] = Single.Parse(player1PointMatch.Groups["position"].Value, this.ciInfo);
			}
			return (tempPlayer != values[this.PlayersIndex]) || (tempPlayer1Index != values[this.Players1Index]); // Also simple
		}
	}
}
