using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GrabConvertMethodContent
{
    class Program
    {
        static void Main(string[] args)
        {
            string expresion = "RunTime:= TimeEq('{ff83c535-f601-418e-ad7f-66c834c9c06a};State', 't', '*',\"Power\");\r\r\nRunTimeUnits := Convert(Convert(fkdasj, \"sss\"),\"ss\");\r\r\nElectricalPower := '{9712fdab-0a74-47b6-8859-06d771b3165f};Amps'*'{c3b9f139-2bcf-4bf5-a6e8-d1b883f407d2};Voltage'; //Rate of Electrical Energy consumed\r\r\nElectricalPowerUnits := Convert(ElectricalPower,\"W\");\r\r\nElectricalUsage := TagTot('{b9b44c82-fb8d-44a1-8aa4-9385f79b4a27};Current Electricity Load|{b241ee96-ef99-4e65-be3c-846c5be4faed};To Days', 't', '*'); //Total Energy Usage so far between 00:00 today and now\r\r\nMaxPower := TagMax('{b9b44c82-fb8d-44a1-8aa4-9385f79b4a27};Current Electricity Load','t','*');\r\r\nAvgPower := TagAvg('{b9b44c82-fb8d-44a1-8aa4-9385f79b4a27};Current Electricity Load','t','*');\r\r\nDowntimeDuration := TimeEq('{ff83c535-f601-418e-ad7f-66c834c9c06a};State','t','*',\"NoPower\");\r\r\nDowntimeDurationUnits := Convert(DowntimeDuration,\"s\");";

            List<string> uoms = new List<string>();
            parseForAllUOMs(expresion, ref uoms);
        }

        static void parseForAllUOMs(string expresion, ref List<string> uoms)
        {
            Regex findFirstConvert = new Regex(@"Convert\(((?<BR>\()|(?<-BR>\))|[^()]*)+\)");
            var match = findFirstConvert.Match(expresion);
            var firstConvert = match.Value;
            
            // Find the UOM in the convert that was found
 
            
            var UOMraw = firstConvert.Split(',').Last().Trim(')').TrimStart();
            UOMraw = UOMraw.TrimStart('\"');
            UOMraw = UOMraw.TrimEnd('\"');
            if (!uoms.Contains(UOMraw))
                uoms.Add(UOMraw);

            var firstArgument = firstConvert.Substring(8);
            if (firstArgument.Contains("Convert"))
                parseForAllUOMs(firstArgument, ref uoms);
            // look for UOMs in the first argument and in the last argument
            var remainder = expresion.Substring(match.Index + firstConvert.Length);
            if (remainder.Contains("Convert"))
                parseForAllUOMs(remainder, ref uoms);

        }
    }
}
