// VIGOBAS Identity Management System 
//  Copyright (C) 2022  Vigo IKS 
//  
//  Documentation - visit https://vigobas.vigoiks.no/ 
//  
//  This program is free software: you can redistribute it and/or modify 
//  it under the terms of the GNU Affero General Public License as 
//  published by the Free Software Foundation, either version 3 of the 
//  License, or (at your option) any later version. 
//  
//  This program is distributed in the hope that it will be useful, 
//  but WITHOUT ANY WARRANTY, without even the implied warranty of 
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//  GNU Affero General Public License for more details. 
//  
//  You should have received a copy of the GNU Affero General Public License 
//  along with this program.  If not, see https://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using FINT.Model.Felles.Kompleksedatatyper;
using static VigoBAS.FINT.Edu.Constants;
using Vigo.Bas.ManagementAgent.Log;

namespace VigoBAS.FINT.Edu.Utilities
{
    class Tools
    {
        public static string GetUriStringFromIdValue (string felleskomponentUri, string classPath, string idAttribute, string idValue)
        {
            string uriString = string.Empty;
            uriString = felleskomponentUri.TrimEnd('/')  + classPath + Delimiter.path + FintAttribute.organisasjonsId + Delimiter.path + idValue;
            return  NormalizeUri(uriString) ;
        }
        public static string GetIdValueFromLink(IEnumerable<ILinkObject> links)
        {
            string href = LinkToString(links);
            string idValue = href.Split('/').Last();

            return idValue;
        }
        public static string GetIdValueFromUri(string uri)
        {
            string idValue = uri.Split('/').Last();

            return idValue;
        }
        public static string LinkToString(ILinkObject link)
        {
            var uriAsString = link.Href.ToString();
            var normalizedUri = NormalizeUri(uriAsString);
            return normalizedUri;
        }

        public static string LinkToString(IEnumerable<ILinkObject> links)
        {
            var uriAsString = string.Empty;
            var normalizedUri = string.Empty;
            var noOfLinks = links.Count();
            //if (noOfLinks > 1)
            //{
            //    var linksAsString = string.Empty;
            //    foreach (var link in links)
            //    {
            //        var linkString = link.Href.ToString();
            //        linksAsString += linkString + Delimiter.listDelimiter;
            //    }
            //    var message = "Found more than one link in self: " + linksAsString;
            //    Logger.Log.InfoFormat(message);   
            //}
            uriAsString = links.First().Href.ToString();
            normalizedUri = NormalizeUri(uriAsString);

            return normalizedUri;
        }
        public static string NormalizeUri(string uri)
        {
            string pattern = @"(?<path>.*/)(.+)";
            string replacement = "${path}";
            var path = Regex.Replace(uri, pattern, replacement).ToLower();
            string pattern2 = @"(.*/)(?<id>.+)";
            string replacement2 = "${id}";
            var idValue = Regex.Replace(uri, pattern2, replacement2);
            var normalizedUri = path + idValue;
            return normalizedUri;
        }

        public static bool PeriodIsValid(IStateValue stateValue, int daysBefore, int daysAhead)
        {
            bool periodIsValid = false;

            var compareDate = DateTime.Today;

            var period = GetPeriodFromStateValue(stateValue);             

            var periodStart = period.Start.Date.AddDays(-daysBefore);
            var periodSlutt = GetPeriodeSluttAsDate(period, infinityDate).AddDays(daysAhead);

            if (periodStart <= compareDate && compareDate <= periodSlutt )
            {
                periodIsValid = true;
            }
            else
            {
                Logger.Log.DebugFormat("Period starting {0} ending {1} is not considered valid", periodStart.ToString(), periodSlutt.ToString());
            }
            return periodIsValid;
        }
        public static bool ExamgroupsShouldBeVisible(DateTime visibleFromDate, DateTime visibleToDate)
        {
            return (visibleFromDate <= DateTime.Today) && (DateTime.Today <= visibleToDate);
        }
        public static bool ExamgroupIsInVisiblePeriod(IStateValue stateValue, DateTime visibleFromDate, DateTime visibleToDate) 
        {
            var period = GetPeriodFromStateValue(stateValue);
            var periodStart = period.Start;
            var periodSlutt = GetPeriodeSluttAsDate(period, infinityDate);

            return (visibleFromDate <= periodStart) && (periodSlutt <= visibleToDate);
        }
        public static string Decrypt(SecureString inStr)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(inStr);
            string decrString = Marshal.PtrToStringUni(ptr);
            return decrString;
        }

        public static string GetFintType(string fintUri)
        {
            var segments = fintUri.Split(Delimiter.path);
            var noOfSegments = segments.Length;
            var fintType = segments[noOfSegments - 3];

            return fintType;
        }

        public static string GetUriPathForClass(string uriString)
        {
            var uri = new Uri(uriString);
            var uriPath = uri.AbsolutePath;
            string pattern = @"(?<path>.*)(/.+/.+)";
            string replacement = "${path}";
            var result = Regex.Replace(uriPath, pattern, replacement);

            return result;
        }

        public static string GetAbsoluteUriForClass(string uriString)
        {
            var uri = new Uri(uriString);
            var uriPath = uri.AbsoluteUri;
            string pattern = @"(?<path>.*)(/.+/.+)";
            string replacement = "${path}";
            var result = Regex.Replace(uriPath, pattern, replacement);

            return result;
        }
        // <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        /// <summary>
        /// Escapes a string according to the URI data string rules given in RFC 3986.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped value.</returns>
        /// <remarks>
        /// The <see cref="Uri.EscapeDataString"/> method is <i>supposed</i> to take on
        /// RFC 3986 behavior if certain elements are present in a .config file.  Even if this
        /// actually worked (which in my experiments it <i>doesn't</i>), we can't rely on every
        /// host actually having this configuration element present.
        /// </remarks>
        public static string EscapeUriDataStringRfc3986(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }
        private static Periode GetPeriodFromStateValue (IStateValue stateValue )
        {            ;
            if (stateValue.Type == "Array")
            {
                return JsonConvert.DeserializeObject<List<Periode>>(stateValue.Value)[0];
            }
                return JsonConvert.DeserializeObject<Periode>(stateValue.Value);
        }
        private static DateTime GetPeriodeSluttAsDate (Periode period, string infinityDate)
        {
            string sluttDateAsString = (period?.Slutt != null) ? period.Slutt.ToString() : infinityDate;

            return DateTime.Parse(sluttDateAsString).Date;
        }
        public enum DataRetrievalStatus 
        {
            DownloadOK,
            FileReadOK,
            FileReadFailed
        }
    }
}
