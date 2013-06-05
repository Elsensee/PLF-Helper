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
using Enums;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Parser
{
	/// <summary>
	/// Provides the parser for Molehill Empire.
	/// </summary>
	public class Parser
	{
		string currentCurrency;
		CultureInfo ciInfo;
		Game game;
		Language lang;
		string lastSearchElementInArray;
		int lastResultElementInArray = -1;
		int playersIndex = -1;
		int players1Index = -1;

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

		private string WelcomeOnBigMarketplace
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.welcomeOnBigMarketPlace[(int)lang];
				}
				return null;
			}
		}
		private string CurrentOffers
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.currentOffers[(int)lang];
				}
				return null;
			}
		}
		private string Total
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.total[(int)lang];
				}
				return null;
			}
		}
		private string DeleteFilter
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.deleteFilter[(int)lang];
				}
				return null;
			}
		}
		private string ListOfAllPlayersPoints
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.listOfAllPlayersPoints[(int)lang];
				}
				return null;
			}
		}
		private string ShowMyRanking
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.showMyRanking[(int)lang];
				}
				return null;
			}
		}
		private string Back
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.backForward[(int)lang, 0];
				}
				return null;
			}
		}
		private string Forward
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.backForward[(int)lang, 1];
				}
				return null;
			}
		}
		private string PlayersTotal
		{
			get
			{
				if (lang != Language.unknown)
				{
					return this.playersTotal[(int)lang];
				}
				return null;
			}
		}
		#endregion
		#region Constructors
		public Parser(Game game, Language lang)
		{
			if (game == Game.unknown)
			{
				throw new ArgumentException("No valid game given!", "game");
			}
			this.game = game;

			if (lang == Language.unknown)
			{
				throw new ArgumentException("No valid language given!", "lang");
			}
			this.lang = lang;

			this.ciInfo = new CultureInfo(ReturnLangCode());
		}
		public Parser(Game game, Language lang, int playersIndex, int players1Index) : this(game, lang)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}
		public Parser(Game game, string lang)
		{
			if (game == Game.unknown)
			{
				throw new ArgumentException("No valid game given!", "game");
			}
			this.game = game;

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
		public Parser(Game game, string lang, int playersIndex, int players1Index) : this(game, lang)
		{
			this.PlayersIndex = playersIndex;
			this.Players1Index = players1Index;
		}
		#endregion

		public bool Parse(string text, ref float[] prices, string[] plants, string[] currencies)
		{
			if (this.PlayersIndex > -1 && this.Players1Index > -1)
			{
				if (this.currentCurrency == null)
				{
					this.currentCurrency = currencies[(int)this.lang];
				}

				Regex plantRegex = new Regex(@"\n[\d\.]+\s+(?<product>(" + this.twoWords[(int)this.lang] + @"|\S+))\s+.+?\s+(?<price>[\d\.,]{3,}) " + this.currentCurrency + @"\s+[\d\.,]{3,} " + this.currentCurrency + @"\s+[^\n]+\n", RegexOptions.IgnoreCase);
				Regex playerRegex = new Regex(Regex.Escape(this.PlayersTotal) + @"\s+(?<player>[0-9]+)\s+" + Regex.Escape(this.ShowMyRanking));

				if (plantRegex.IsMatch(text))
				{
					return ParseMarket(plantRegex.Match(text), ref prices, plants);
				}
				else if (text.IndexOf(this.ListOfAllPlayersPoints) != -1 && playerRegex.IsMatch(text))
				{
					return ParseTownHall(playerRegex.Match(text), text, ref prices);
				}
			}
			return false;
		}

		public bool ParseMarket(Match match, ref float[] prices, string[] plants)
		{
			int plantIndex = SearchElementInArray(plants, match.Groups["product"].Value);
			if (plantIndex == -1)
			{
				return false;
			}
			float temp = prices[plantIndex];
			prices[plantIndex] = Single.Parse(match.Groups["price"].Value);
			return (prices[plantIndex] != temp); // Easy, huh?
		}

		public bool ParseTownHall(Match match, string text, ref float[] prices)
		{
			float tempPlayer = prices[this.PlayersIndex];
			float tempPlayer1Index = prices[this.Players1Index];
			prices[this.PlayersIndex] = Single.Parse(match.Groups["player"].Value);

			Regex player1PointRegex = new Regex(@"(?<position>[0-9]+)\..+\s+1\s*\n");
			if (player1PointRegex.IsMatch(text))
			{
				MatchCollection player1PointMatches = player1PointRegex.Matches(text);
				Match player1PointMatch = player1PointMatches[player1PointMatches.Count - 1];
				prices[this.Players1Index] = Single.Parse(player1PointMatch.Groups["position"].Value);
			}
			return (tempPlayer != prices[this.PlayersIndex]) || (tempPlayer1Index != prices[this.Players1Index]); // Also simple
		}

		public Parser Clone()
		{
			return new Parser(this.game, this.lang, this.PlayersIndex, this.Players1Index);
		}

		private string ReturnLangCode()
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

		private int SearchElementInArray(string[] searchArray, string searchString)
		{
			if (String.Compare(searchString, lastSearchElementInArray, false) == 0)
			{
				return lastResultElementInArray;
			}
			for (int i = 0; i < searchArray.Length; i++)
			{
				if (String.Compare(searchString, searchArray[i], false) == 0)
				{
					lastSearchElementInArray = searchString;
					lastResultElementInArray = i;
					return i;
				}
			}
			return -1;
		}
	}
}
