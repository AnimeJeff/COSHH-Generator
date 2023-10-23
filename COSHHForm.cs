﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;

namespace COSHH_Generator
{
    class COSHHForm
    {
        static string templatePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, ".template.docx");

        private static void FillStudentInfo(in WordprocessingDocument doc, in string title,in string name,in string college,in string year, in string date)
        {
            IEnumerable<TableRow> rows = doc.MainDocumentPart!.Document.Body!.Elements<Table>().First().Elements<TableRow>();
            var titleCell = rows.ElementAt(0).Elements<TableCell>().ElementAt(1).Elements<Paragraph>().First().Elements<Run>().First().Elements<Text>().First();   
            var nameCell = rows.ElementAt(1).Elements<TableCell>().ElementAt(1).Elements<Paragraph>().First().Elements<Run>().First().Elements<Text>().First();
            var dateCell = rows.ElementAt(1).Elements<TableCell>().ElementAt(3).Elements<Paragraph>().First().Elements<Run>().First().Elements<Text>().First();
            var collegeCell = rows.ElementAt(2).Elements<TableCell>().ElementAt(1).Elements<Paragraph>().First().Elements<Run>().First().Elements<Text>().First();
            var yearCell = rows.ElementAt(2).Elements<TableCell>().ElementAt(3).Elements<Paragraph>().First().Elements<Run>().First().Elements<Text>().First();
            dateCell.Text = date;
            titleCell.Text = title;
            nameCell.Text = name;
            collegeCell.Text = college;
            yearCell.Text = year;
        }

        private static void AddSubstance(in WordprocessingDocument doc, in string substanceName, in string massVolume, SafetyData safetyData)
        {
            var SubstanceTable = doc.MainDocumentPart!.Document.Body!.Elements<Table>().ElementAt(2);
            TableRow row = SubstanceTable.Elements<TableRow>().Last();
            SubstanceTable.Descendants<TableRow>().Last().InsertAfterSelf(row.CloneNode(true));
            TableCell substanceNameCell = row.Elements<TableCell>().ElementAt(0);
            substanceNameCell.Elements<Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = substanceName;

            TableCell massVolumeCell = row.Elements<TableCell>().ElementAt(1);
            massVolumeCell.Elements<Paragraph>().First().Elements<Run>().First().Elements<Text>().First().Text = massVolume;

            Run hazardsCell = row.Elements<TableCell>().ElementAt(2).Elements<Paragraph>().First().AppendChild(new Run());

            foreach (string hazardStatement in safetyData.HazardStatements)
            {
                hazardsCell.AppendChild(new Text(hazardStatement));
                hazardsCell.AppendChild(new Break());
            }
            IEnumerable<Run> exposureRouteCell = row.Elements<TableCell>().ElementAt(3).Descendants<Run>();
            
            var eyeCheckBox = exposureRouteCell.ElementAt(2).GetFirstChild<Text>()!;
            var skinCheckBox = exposureRouteCell.ElementAt(4).GetFirstChild<Text>()!;
            var inhalationCheckBox = exposureRouteCell.ElementAt(6).GetFirstChild<Text>()!;
            var ingestionCheckBox = exposureRouteCell.ElementAt(8).GetFirstChild<Text>()!;
            
            eyeCheckBox.Text = safetyData.Eyes ? "☒" : "☐";
            skinCheckBox.Text = safetyData.Skin ? "☒" : "☐";
            inhalationCheckBox.Text = safetyData.Inhalation ? "☒" : "☐";
            ingestionCheckBox.Text = safetyData.Ingestion ? "☒" : "☐";

            IEnumerable<Run> controlMeasuresCell = row.Elements<TableCell>().ElementAt(4).Descendants<Run>();
            
            var consultSpillageCheckBox = controlMeasuresCell.ElementAt(0).GetFirstChild<Text>()!;
            var safetySpecCheckBox = controlMeasuresCell.ElementAt(3).GetFirstChild<Text>()!;
            var labCoatCheckBox = controlMeasuresCell.ElementAt(5).GetFirstChild<Text>()!;
            var glovesCheckBox = controlMeasuresCell.ElementAt(7).GetFirstChild<Text>()!;
            var fumehoodCheckBox = controlMeasuresCell.ElementAt(10).GetFirstChild<Text>()!;
            var noNakedFlamesCheckBox = controlMeasuresCell.ElementAt(13).GetFirstChild<Text>()!;
            var useWaterBathCheckBox = controlMeasuresCell.ElementAt(17).GetFirstChild<Text>()!;
            var pregnantCheckBox = controlMeasuresCell.ElementAt(20).GetFirstChild<Text>()!;
            var notNearWaterCheckBox = controlMeasuresCell.ElementAt(22).GetFirstChild<Text>()!;
            var dropwiseCheckBox = controlMeasuresCell.ElementAt(24).GetFirstChild<Text>()!;
            var notExposeToAirCheckBox = controlMeasuresCell.ElementAt(27).GetFirstChild<Text>()!;

            consultSpillageCheckBox.Text = safetyData.ConsultSpill ? "☒" : "☐";
            safetySpecCheckBox.Text = safetyData.Goggles ? "☒" : "☐";
            labCoatCheckBox.Text = safetyData.LabCoat ? "☒" : "☐";
            glovesCheckBox.Text = safetyData.Gloves ? "☒" : "☐";
            fumehoodCheckBox.Text = safetyData.Fumehood ? "☒" : "☐";
            noNakedFlamesCheckBox.Text = safetyData.NoNakedFlames ? "☒" : "☐";
            useWaterBathCheckBox.Text = safetyData.UseWaterBath ? "☒" : "☐";
            pregnantCheckBox.Text = safetyData.NotUseWhenPregnant ? "☒" : "☐";
            notNearWaterCheckBox.Text = safetyData.NotNearWater ? "☒" : "☐";
            dropwiseCheckBox.Text = safetyData.Dropwise ? "☒" : "☐";
            notExposeToAirCheckBox.Text = safetyData.NotExposeToAir ? "☒" : "☐";

            //consultSpillageCheckBox.Text = "☒";
            //safetySpecCheckBox.Text = "☒";
            //labCoatCheckBox.Text = "☒";
            //glovesCheckBox.Text = "☒";
            //fumehoodCheckBox.Text = "☒";
            //noNakedFlamesCheckBox.Text = "☒";
            //useWaterBathCheckBox.Text = "☒";
            //pregnantCheckBox.Text = "☒";
            //notNearWaterCheckBox.Text = "☒";
            //dropwiseCheckBox.Text = "☒";
            //notExposeToAirCheckBox.Text = "☒";
            return;
            ////Specific Safety or Risk Implication Table
            //Table specificSafetyTable = doc.MainDocumentPart!.Document.Body!.Elements<Table>().ElementAt(3);

            //var fireExplosionRow = specificSafetyTable.Elements<TableRow>().ElementAt(1);
            //var fireExplosionCheckBox = fireExplosionRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
            //var fireExplosionPreventionCell = fireExplosionRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();
            
            //var thermalRunawayRow = specificSafetyTable.Elements<TableRow>().ElementAt(1);
            //var thermalRunawayCheckBox = thermalRunawayRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
            //var thermalRunawayPreventionCell = thermalRunawayRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();

            //var gasReleaseRow = specificSafetyTable.Elements<TableRow>().ElementAt(1);
            //var gasReleaseCheckBox = gasReleaseRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
            //var gasReleasePreventionCell = gasReleaseRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();

            //var malodorousRow = specificSafetyTable.Elements<TableRow>().ElementAt(1);
            //var malodorousCheckBox = malodorousRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
            //var malodorousPreventionCell = malodorousRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();


            //Console.WriteLine(fireExplosionPreventionCell.Text = "lol");

            //if (safetyData.Flammable | safetyData.Explosive)
            //{
            //    fireExplosionCheckBox.Text = "☒";
            //    //List<string> preventions = new List<string>(){
            //    //"Keep flammable materials away from sources of ignition.",
            //    //"Keep away from sources of ignition / sparks",
            //    //"Keep away from sources of ignition"
            //    //"Keep away from naked flames and sources of ignition"
            //    //"Keep solvents away from sources of ignition"
            //    //"Do not heat flammable solvents directly on a hot plate. Keep electrolysis in fumehood."
            //    //}

            //    fireExplosionPreventionCell.Text = "Keep away from ignition sources.";
            //}
            //if (safetyData.ThermalRunaway)
            //{
            //    thermalRunawayCheckBox.Text = "☒";
            //    thermalRunawayPreventionCell.Text = "Dropwise addition by dropping funnel.";
            //}
            //if (safetyData.GasRelease)
            //{
            //    gasReleaseCheckBox.Text = "☒";
            //    //"Ensure ventilation (guard tube)."
            //    //"Hydrogen gas release on small scale. Ensure sufficient ventilation and use of open systems."
            //    //"Ventilation, use of open reaction flask"
            //    //"Keep in fumehood"
            //    //"Ensure test tubes are pointed away from others and into fume hood when reactions are taking place."
            //    //"Avoid contact of sodium hypochlorite solutions with acids"
            //    gasReleasePreventionCell.Text = "Keep the fumehood sash pulled down.";
            //}
            //if (safetyData.Malodorous)
            //{
            //    malodorousCheckBox.Text = "☒";
            //    //"Contained in a fume hood."
            //    //"Perform all reactions in fume hood where possible."
            //    //"Keep in fumehood"
            //    //"Use a fume hood for all experiments"
            //    malodorousPreventionCell.Text = "Perform all reactions in fume hood where possible and keep the fumehood sash pulled down.";
            //}
            ////special measures
            ////Dispose of all transition metal waste in designated waste containers.
            ////"Ensure all reactions are performed on a small (test tube) scale. Assume unknown samples to analyse are highly toxic and hazardous."
        }

        public static async Task Generate(string title, string name, string college, string year, string date,
            bool? fireExplosion, bool? thermalRunaway, bool? gasRelease, bool? malodorousSubstances, bool? specialMeasures,
            string fireExplosionText, string thermalRunawayText, string gasReleaseText, string malodorousSubstancesText, string specialMeasuresText,
            bool? halogenated, bool? hydrocarbon, bool? contaminated, bool? aqueous, bool? named, bool? silicaTLC,
            List<SubstanceEntry> substances, string outputPath, Action callback)
        {

            try
            {
                FileInfo fi = new FileInfo(templatePath);
                fi.CopyTo(outputPath, true);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(outputPath, true))
                {
                    FillStudentInfo(doc, title.Trim(), name.Trim(), college.Trim(), year.Trim(), date.Trim());

                    foreach (var substanceEntry in substances)
                    {
                        if (substanceEntry.DisplayName.Count() == 0) continue;
                        //MessageBox.Show(substanceEntry.name, substanceEntry.name, MessageBoxButton.YesNo);
                        SafetyData sds = substanceEntry.extractionTask != null ? await substanceEntry.extractionTask : new SafetyData();
                        var amount = substanceEntry.Amount == null ? "N/A" : substanceEntry.Amount.Trim();
                        amount = string.IsNullOrEmpty(amount) ? "N/A" : $"{amount} {substanceEntry.AmountUnit}";
                        AddSubstance(doc, substanceEntry.DisplayName, amount.Trim(), sds);
                    }
                    var substanceRows = doc.MainDocumentPart!.Document.Body!.Elements<Table>().ElementAt(2).Descendants<TableRow>();
                    if (substanceRows.Count() > 2)
                    {
                        substanceRows.Last().Remove();
                    }

                    Table specificSafetyTable = doc.MainDocumentPart!.Document.Body!.Elements<Table>().ElementAt(3);
                    var fireExplosionRow = specificSafetyTable.Elements<TableRow>().ElementAt(1);
                    var fireExplosionCheckBox = fireExplosionRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
                    var fireExplosionPreventionCell = fireExplosionRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();

                    var thermalRunawayRow = specificSafetyTable.Elements<TableRow>().ElementAt(2);
                    var thermalRunawayCheckBox = thermalRunawayRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
                    var thermalRunawayPreventionCell = thermalRunawayRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();

                    var gasReleaseRow = specificSafetyTable.Elements<TableRow>().ElementAt(3);
                    var gasReleaseCheckBox = gasReleaseRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
                    var gasReleasePreventionCell = gasReleaseRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();

                    var malodorousRow = specificSafetyTable.Elements<TableRow>().ElementAt(4);
                    var malodorousCheckBox = malodorousRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
                    var malodorousPreventionCell = malodorousRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();

                    var specialRow = specificSafetyTable.Elements<TableRow>().ElementAt(5);
                    var specialCheckBox = specialRow.Elements<TableCell>().ElementAt(1).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
                    var specialPreventionCell = specialRow.Elements<TableCell>().ElementAt(2).Descendants<Text>().First();

                    if (fireExplosion == true)
                    {
                        fireExplosionCheckBox.Text = "☒";
                        fireExplosionPreventionCell.Text = fireExplosionText;
                        //"Keep flammable materials away from sources of ignition.",
                        //"Keep away from sources of ignition / sparks",
                        //"Keep away from sources of ignition"
                        //"Keep away from naked flames and sources of ignition"
                        //"Keep solvents away from sources of ignition"
                        //"Do not heat flammable solvents directly on a hot plate. Keep electrolysis in fumehood."
                    }
                    if (thermalRunaway == true)
                    {
                        thermalRunawayCheckBox.Text = "☒";
                        thermalRunawayPreventionCell.Text = thermalRunawayText;
                        //"Dropwise addition by dropping funnel."
                    }
                    if (gasRelease == true)
                    {
                        gasReleaseCheckBox.Text = "☒";
                        gasReleasePreventionCell.Text = gasReleaseText;
                        //"Ensure ventilation (guard tube)."
                        //"Hydrogen gas release on small scale. Ensure sufficient ventilation and use of open systems."
                        //"Ventilation, use of open reaction flask"
                        //"Keep in fumehood"
                        //"Ensure test tubes are pointed away from others and into fume hood when reactions are taking place."
                        //"Avoid contact of sodium hypochlorite solutions with acids"
                    }
                    if (malodorousSubstances == true)
                    {
                        malodorousCheckBox.Text = "☒";
                        malodorousPreventionCell.Text = malodorousSubstancesText;
                        //"Contained in a fume hood."
                        //"Perform all reactions in fume hood where possible."
                        //"Keep in fumehood"
                        //"Use a fume hood for all experiments"
                    }
                    if (specialMeasures == true)
                    {
                        specialCheckBox.Text = "☒";
                        specialPreventionCell.Text = specialMeasuresText;
                    }
                    Table wasteDisposalTable = doc.MainDocumentPart!.Document.Body!.Elements<Table>().ElementAt(4);
                    var halogenatedCheckBox = wasteDisposalTable.Elements<TableRow>().ElementAt(1).Descendants<Run>().ElementAt(1).GetFirstChild<Text>()!;
                    var aqueousCheckBox = wasteDisposalTable.Elements<TableRow>().ElementAt(1).Descendants<Run>().ElementAt(7).GetFirstChild<Text>()!;
                    var hydrocarbonCheckBox = wasteDisposalTable.Elements<TableRow>().ElementAt(2).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
                    var namedCheckBox = wasteDisposalTable.Elements<TableRow>().ElementAt(2).Descendants<Run>().ElementAt(4).GetFirstChild<Text>()!;
                    var contaminatedCheckBox = wasteDisposalTable.Elements<TableRow>().ElementAt(3).Descendants<Run>().ElementAt(0).GetFirstChild<Text>()!;
                    var silicaCheckBox = wasteDisposalTable.Elements<TableRow>().ElementAt(3).Descendants<Run>().ElementAt(2).GetFirstChild<Text>()!;

                    halogenatedCheckBox.Text = halogenated == true ? "☒" : "☐";
                    hydrocarbonCheckBox.Text = hydrocarbon == true ? "☒" : "☐";
                    contaminatedCheckBox.Text = contaminated == true ? "☒" : "☐";
                    aqueousCheckBox.Text = aqueous == true ? "☒" : "☐";
                    namedCheckBox.Text = named == true ? "☒" : "☐";
                    silicaCheckBox.Text = silicaTLC == true ? "☒" : "☐";
                }
                FileAttributes attributes = File.GetAttributes(outputPath);
                attributes &= ~(FileAttributes.Hidden | FileAttributes.ReadOnly);
                File.SetAttributes(outputPath, attributes);
                if (MessageBox.Show("Success", "Generation Result", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(outputPath)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Generation Error", MessageBoxButton.OK);
            }
            callback();
            return;
        }


    }
    
}
