using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace COSHH_Generator.Scrapers
{
    static class SDSParser
    {
        private static Regex HazardStatementRegex = new Regex(@"^H\d{3}[dfDFi]{0,2}(?:\s?\+\s?H\d{3}){0,2}", RegexOptions.Multiline);
        private static Regex PrecautionaryStatementRegex = new Regex(@"P\d{3}(?:\s?\+\s?P\d{3}){0,2}", RegexOptions.Multiline);
        private static Regex EUHazardStatementRegex = new Regex(@"EUH\d{3}A?", RegexOptions.Multiline);
        //TODO check for malodour
        public static async Task<SafetyData> Extract(string url)
        {
            Trace.WriteLine("Downloading");
            string text = string.Empty;
            HttpResponseMessage response = await Client.GetAsync(url);
            Stream pdfStream = await response.Content.ReadAsStreamAsync();
            Trace.WriteLine("Downloaded");
            List<string> hazardCodes = new List<string>();
            List<string> precautionaryCodes = new List<string>();
            List<string> euHazardCodes = new List<string>();

            PdfReader reader = new PdfReader(pdfStream);
            Trace.WriteLine("Extracting");
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                string pageText = PdfTextExtractor.GetTextFromPage(reader, page);
                //Console.WriteLine(pageText.Contains("SECTION 2: Hazards identification"));
                if (pageText.Contains("SECTION 2: Hazards identification"))
                {

                    pageText = pageText + PdfTextExtractor.GetTextFromPage(reader, page + 1).Split("SECTION 3").First();
                    Trace.WriteLine(pageText);
                    Trace.WriteLine(pageText.IndexOf("Hazard statement"));
                    pageText = pageText.Split("Hazard statement", 1).Last().Split("Reduced Labeling").First();
                    hazardCodes = HazardStatementRegex.Matches(pageText).Cast<Match>().Select(match => match.Value.Replace(" ", "")).ToList();
                    precautionaryCodes = PrecautionaryStatementRegex.Matches(pageText).Cast<Match>().Select(match => match.Value.Replace(" ", "")).ToList();
                    euHazardCodes = EUHazardStatementRegex.Matches(pageText).Cast<Match>().Select(match => match.Value.Replace(" ", "")).ToList();

                    //hazardCodes.ForEach(it => Console.WriteLine(it));
                    //precautionaryCodes.ForEach(it => Console.WriteLine(it));
                    //euHazardCodes.ForEach(it => Console.WriteLine(it));
                }
                else if (pageText.Contains("SECTION 9"))
                {
                    //TODO check for malodorous
                    pageText += PdfTextExtractor.GetTextFromPage(reader, page + 1).Split("SECTION 10").First();
                    //Trace.WriteLine(pageText.Split("Odor ").Last().Split("d)").First());
                    break;
                }
            }

            SafetyData safetyData = new SafetyData(hazardCodes, precautionaryCodes);
            return safetyData;
        }

    }
}
