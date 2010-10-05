/* Josip Medved <jmedved@jmedved.com> http://www.jmedved.com */

//2008-02-15: New version.
//2008-04-11: Cleaned code to match FxCop 1.36 beta 2.
//2008-11-05: Easter is also holiday.


using System;

namespace Medo.Localization.Croatia {

	/// <summary>
	/// Class for detecting Croatian hollidays.
	/// </summary>
	public static class Holiday {

		/// <summary>
		/// Returns true if given date is public holiday.
		/// Valid dates are defined from 1991-03-25. If lower date is given, exception will be thrown.
		/// </summary>
		/// <param name="date">Date to check.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Date must be larger or equal to 1991-03-25.</exception>
		public static bool IsHoliday(DateTime date) {
			if (date >= new DateTime(2002, 02, 16)) { return Helper.GetIsHoliday20020216(date); }
			if (date >= new DateTime(2001, 11, 15)) { return Helper.GetIsHoliday20011115(date); }
			if (date >= new DateTime(1996, 05, 08)) { return Helper.GetIsHoliday19960508(date); }
			if (date >= new DateTime(1991, 03, 25)) { return Helper.GetIsHoliday19910325(date); }

			throw new System.ArgumentOutOfRangeException("date", Resources.ExceptionDateMustBeLargerOrEqualTo19910325);
		}


		/// <summary>
		/// Returns easter date for given year.
		/// </summary>
		/// <param name="year">Year.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Year is out of range (1753-4000).</exception>
		public static DateTime GetEasterDate(int year) { 
			if ((year < 1753) || (year > 4000)) { throw new System.ArgumentOutOfRangeException("year", Resources.ExceptionYearIsOutOfRange); }

			int tmpMonth, tmpDay;

			int a, b, c, d, e, f, g, h, i, j, k, l, m;

			a = year % 19;
			b = year / 100;
			c = year % 100;
			d = b / 4;
			e = b % 4;
			f = (b + 8) / 25;
			g = (b - f + 1) / 3;
			h = (19 * a + b - d - g + 15) % 30;
			i = c / 4;
			j = c % 4;
			k = (32 + 2 * e + 2 * i - h - j) % 7;
			l = (a + 11 * h + 22 * k) / 451;
			m = (h + k - 7 * l + 114) % 31;

			tmpMonth = (h + k - 7 * l + 114) / 31;
			tmpDay = m + 1;

			return new DateTime(year, tmpMonth, tmpDay);
		}


		private static class Helper {

			internal static bool GetIsHoliday20020216(DateTime date) { //NN 13/02
				int m = date.Month;
				int d = date.Day;

				if ((m == 1) && (d == 1)) { return true; }               // Nova godina
				if ((m == 1) && (d == 6)) { return true; }               // Bogojavljanje ili Sveta tri kralja
				if ((m == 5) && (d == 1)) { return true; }               // Praznik rada
				if ((m == 6) && (d == 22)) { return true; }              // Dan antifašističke borbe
				if ((m == 6) && (d == 25)) { return true; }              // Dan državnosti
				if ((m == 8) && (d == 5)) { return true; }               // Dan pobjede i domovinske zahvalnosti
				if ((m == 8) && (d == 15)) { return true; }              // Velika Gospa
				if ((m == 10) && (d == 8)) { return true; }              // Dan neovisnosti
				if ((m == 11) && (d == 1)) { return true; }              // Svi sveti
				if ((m == 12) && (d == 25)) { return true; }             // božićni blagdani
				if ((m == 12) && (d == 26)) { return true; }             // božićni blagdani

				DateTime e = GetEasterDate(date.Year);
                if ((m == e.Month) && (d == e.Day)) { return true; }   // Uskrs - nije u NN

				DateTime e1 = e.AddDays(1);
				if ((m == e1.Month) && (d == e1.Day)) { return true; }   // Uskrsni ponedjeljak – drugi dan Uskrsa

				DateTime e60 = e.AddDays(60);
				if ((m == e60.Month) && (d == e60.Day)) { return true; } // Tijelovo

				return false;
			}

			internal static bool GetIsHoliday20011115(DateTime date) { //NN 96/01
				int m = date.Month;
				int d = date.Day;

				if ((m == 1) && (d == 1)) { return true; }               // Nova godina
				if ((m == 5) && (d == 1)) { return true; }               // Praznik rada
				if ((m == 6) && (d == 22)) { return true; }              // Dan antifašističke borbe
				if ((m == 6) && (d == 25)) { return true; }              // Dan državnosti
				if ((m == 8) && (d == 5)) { return true; }               // Dan pobjede i domovinske zahvalnosti
				if ((m == 8) && (d == 15)) { return true; }              // Velika Gospa
				if ((m == 10) && (d == 8)) { return true; }              // Dan neovisnosti
				if ((m == 11) && (d == 1)) { return true; }              // Svi sveti
				if ((m == 12) && (d == 25)) { return true; }             // božićni blagdani
				if ((m == 12) && (d == 26)) { return true; }             // božićni blagdani

				DateTime e = GetEasterDate(date.Year);
                if ((m == e.Month) && (d == e.Day)) { return true; }   // Uskrs - nije u NN

				DateTime e1 = e.AddDays(1);
				if ((m == e1.Month) && (d == e1.Day)) { return true; }   // Uskrsni ponedjeljak – drugi dan Uskrsa

				DateTime e60 = e.AddDays(60);
				if ((m == e60.Month) && (d == e60.Day)) { return true; } // Tijelovo

				return false;
			}

			internal static bool GetIsHoliday19960508(DateTime date) { //NN 33/96
				int m = date.Month;
				int d = date.Day;

				if ((m == 1) && (d == 1)) { return true; }               // Nova godina
				if ((m == 1) && (d == 6)) { return true; }               // Bogojavljanje ili Sveta tri kralja
				if ((m == 5) && (d == 1)) { return true; }               // Praznik rada
				if ((m == 5) && (d == 30)) { return true; }              // Dan državnosti
				if ((m == 6) && (d == 22)) { return true; }              // Dan antifašističke borbe
				if ((m == 8) && (d == 5)) { return true; }               // Dan pobjede i domovinske zahvalnosti
				if ((m == 8) && (d == 15)) { return true; }              // Velika Gospa
				if ((m == 11) && (d == 1)) { return true; }              // Svi sveti
				if ((m == 12) && (d == 25)) { return true; }             // božićni blagdani
				if ((m == 12) && (d == 26)) { return true; }             // božićni blagdani

				DateTime e = GetEasterDate(date.Year);
                if ((m == e.Month) && (d == e.Day)) { return true; }   // Uskrs - nije u NN

				DateTime e1 = e.AddDays(1);
				if ((m == e1.Month) && (d == e1.Day)) { return true; }   // Uskrsni ponedjeljak – drugi dan Uskrsa

				return false;
			}

			internal static bool GetIsHoliday19910325(DateTime date) { //NN 14/91
				int m = date.Month;
				int d = date.Day;

				if ((m == 1) && (d == 1)) { return true; }               // Nova godina
				if ((m == 1) && (d == 6)) { return true; }               // (neradni dan)
				if ((m == 1) && (d == 7)) { return true; }               // (neradni dan)
				if ((m == 5) && (d == 1)) { return true; }               // Praznik rada
				if ((m == 5) && (d == 30)) { return true; }              // Dan državnosti
				if ((m == 6) && (d == 22)) { return true; }              // Dan antifašističke borbe
				if ((m == 8) && (d == 15)) { return true; }              // Velika Gospa
				if ((m == 11) && (d == 1)) { return true; }              // Dan spomena na mrtve (neradni dan)
				if ((m == 12) && (d == 25)) { return true; }             // božićni blagdani
				if ((m == 12) && (d == 26)) { return true; }             // božićni blagdani

				DateTime e = GetEasterDate(date.Year);
                if ((m == e.Month) && (d == e.Day)) { return true; }   // Uskrs - nije u NN

				DateTime e1 = e.AddDays(1);
				if ((m == e1.Month) && (d == e1.Day)) { return true; }   // Uskrsni ponedjeljak – drugi dan Uskrsa (neradni dan)

				return false;
			}

		}


		private static class Resources {

			internal static string ExceptionDateMustBeLargerOrEqualTo19910325 { get { return "Date must be larger or equal to 1991-03-25."; } }

			internal static string ExceptionYearIsOutOfRange { get { return "Year is out of range (1753-4000)."; } }

		}

	}

}
