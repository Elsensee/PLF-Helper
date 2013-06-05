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

namespace Enums
{
	public enum Game
	{
		unknown = -1,
		MolehillEmpire = 0
	}

	public enum Language
	{
		unknown = -1,
		EN = 0,
		DE = 1,
		NL = 2
	}

	public enum Level
	{
		unknown = -1,
		Salatschleuderer = 0,
		Erbsenzähler = 1,
		Tomatendealer = 2,
		Zwiebeltreter = 3,
		Erntehelfer = 4,
		Kartoffelschäler = 5,
		Grünzeugvertreter = 6,
		Maulwurfjäger = 7,
		Kleingärtner = 8,
		Blaubeerbaron = 9,
		Vogelscheucher = 10,
		Rosenkavalier = 11,
		Gemüseguru = 12,
		Kirschkernspucker = 13,
		Zaunkönig = 14,
		Walnusswächter = 15,
		Lilienlobbyist = 16,
		Orchideenzüchter = 17,
		Krokuspokus = 18,
		Unkrautschreck = 19,
		Gerberagerber = 20,
		Wurzelimperator = 21,
		Superimperator = 22,
		Seerosenfee = 23,
		Engelstrompeter = 24,
		Bohnenbaron = 25,
		Superzwerg = 26
	}

	public enum LevelPunkte
	{
		unknown = -1,
		Salatschleuderer = 0,
		Erbsenzähler = 300,
		Tomatendealer = 1000,
		Zwiebeltreter = 5000,
		Erntehelfer = 15000,
		Kartoffelschäler = 40000,
		Grünzeugvertreter = 100000,
		Maulwurfjäger = 200000,
		Kleingärtner = 350000,
		Blaubeerbaron = 550000,
		Vogelscheucher = 800000,
		Rosenkavalier = 1500000,
		Gemüseguru = 2500000,
		Kirschkernspucker = 4500000,
		Zaunkönig = 7500000,
		Walnusswächter = 15000000,
		Lilienlobbyist = 22000000,
		Orchideenzüchter = 30000000,
		Krokuspokus = 40000000,
		Unkrautschreck = 55000000,
		Gerberagerber = 70000000,
		Wurzelimperator = 99999999,
		Superimperator = 300000000,
		Seerosenfee = 450000000,
		Engelstrompeter = 600000000,
		Bohnenbaron = 750000000,
		Superzwerg = 900000000
	}

	public enum Months
    {
        January = 1,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }
}