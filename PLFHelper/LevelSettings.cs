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

namespace PLFHelper
{
	/// <summary>
	/// Description of LevelSettings.
	/// </summary>
	[Serializable()]
	internal class LevelSettings
	{
		private int level;
		private int server;
		private bool showPlants;

		public int Level
		{
			get
			{
				return this.level;
			}

			set
			{
				if (value > -1)
				{
					this.level = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		public int Server
		{
			get
			{
				return this.server;
			}

			set
			{
				if (value > -1)
				{
					this.server = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		public bool ShowPlants
		{
			get
			{
				return this.showPlants;
			}

			set
			{
				this.showPlants = value;
			}
		}

		public LevelSettings(int level, int server, bool showPlants)
		{
			this.Level = level;
			this.Server = server;
			this.ShowPlants = showPlants;
		}
	}
}
