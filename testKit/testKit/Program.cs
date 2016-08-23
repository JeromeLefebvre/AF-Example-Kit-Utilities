using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using System.ServiceProcess;
using OSIsoft.AF.PI;
using OSIsoft.AF.Time;
using OSIsoft.AF.Analysis;
using OSIsoft.AF.Data;

namespace testKit
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = new PISystems().DefaultPISystem;
            var db = system.Databases.DefaultDatabase;
            var dataArchive = new PIServers().DefaultPIServer;

            var archiveAttr = db.Elements["PI Data Archive"].Attributes["Name"];
            if (archiveAttr != null)
                archiveAttr.SetValue(new AFValue(dataArchive.Name));
            archiveAttr = db.Elements["PI Data Archive"].Attributes["名"];
            if (archiveAttr != null)
                archiveAttr.SetValue(new AFValue(dataArchive.Name));
            db.CheckIn();
            foreach (var elem in db.Elements)
                createConfig(elem);
            db.CheckIn();

            StopService("PIAnalysisManager", 10000);
            
            StartService("PIAnalysisManager", 10000);

            foreach (var category in new List<string> { "Random Data", "Usage", "Cost", "Downtime" })
                ProgrammaticAnalysisRecalculation(system, db, db.AnalysisCategories["Random Data"]);
        }

        public static void createConfig(AFElement elem)
        {
            foreach (var attr in elem.Attributes)
                if (attr.DataReference != null)
                    attr.DataReference.CreateConfig();
            foreach (var child in elem.Elements)
                createConfig(child);
            elem.CheckIn();
        }
        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not stop the analysis service");
                Console.WriteLine(e.Message);
            }
        }

        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                Console.WriteLine("Could not start the analysis service");
            }
        }

        public static void ProgrammaticAnalysisRecalculation(PISystem system, AFDatabase database, AFCategory category)
        {
            // we start by generating timestamps we want to recalculate  
            // we could get them from existing recorded values, etc...  
            //  
            // here we simply generate yesterdays hourly values ...  
            var recalculationTimeStamps = new List<AFTime>();
            for (int i = 0; i < 24; i++)
            {
                recalculationTimeStamps.Add(new AFTime(DateTime.Today.Subtract(TimeSpan.FromDays(3)) + TimeSpan.FromHours(i)));
            }


            AFNamedCollectionList<AFAnalysis> analysislist;
            analysislist = AFAnalysis.FindAnalyses(database, null, null, null, category, null, null, AFStatus.None, AFSortField.ID, AFSortOrder.Ascending, 0, 1000);


            foreach (var afAnalysis in analysislist)
            {
                // we recalculate results  
                var results = Calculate(afAnalysis, recalculationTimeStamps);

                // we could delete values here, but I simply replce them instead  

                // we insert our new values  
                AFListData.UpdateValues(results, AFUpdateOption.Replace);
            }

        }


        /// <summary>  
        /// Adapted from Mike's example  
        /// Mike also talks about errors and warnings in his post, you should check it.  
        /// </summary>  
        /// <see cref="https://pisquare.osisoft.com/message/28537#28537"/>  
        /// <param name="analysis"></param>  
        /// <param name="times"></param>  
        /// <returns></returns>  
        private static List<AFValue> Calculate(AFAnalysis analysis, IEnumerable<AFTime> times)
        {

            var results = new List<AFValue>();
            var analysisConfiguration = analysis.AnalysisRule.GetConfiguration();
            var state = new AFAnalysisRuleState(analysisConfiguration);
            foreach (var time in times)
            {
                Console.WriteLine("Evaluating for {0}", time);
                state.Reset();
                state.SetExecutionTimeAndPopulateInputs(time);
                analysis.AnalysisRule.Run(state);
                if (state.EvaluationError == null)
                {
                    // this merges the state (results) with the configuration so its easier to loop with both...  
                    var resultSet = analysisConfiguration.ResolvedOutputs.Zip(state.Outputs, Tuple.Create);


                    foreach (var result in resultSet)
                    {
                        // for more clarty, we take out our data into clearer variables  
                        AFAnalysisRuleResolvedOutput analysisRow = result.Item1;
                        var calcRes = (AFValue)result.Item2;


                        // we filter to get only the results that have an output attribute ( an AFValue )  
                        if (analysisRow.Attribute != null)
                        {
                            // add new AF Value into the results table  
                            results.Add(new AFValue((AFAttribute)analysisRow.Attribute, calcRes.Value, calcRes.Timestamp));
                        }
                    }
                }
                else
                {
                    // errors occur quite frequently, for example, TagTot('attr1', 't', '*') will fail if computed at '*' = 't'
                    // but this does not occur when the analysis is running on event base.
                    //Console.WriteLine("An error occurred: {0}", state.EvaluationError.Message);
                }
            }

            return results;
        }
    }
}
