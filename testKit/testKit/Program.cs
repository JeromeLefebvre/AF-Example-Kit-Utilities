﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using System.ServiceProcess;
using OSIsoft.AF.PI;

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
                foreach (var attr in elem.Attributes)
                    if (attr.DataReference != null)
                        attr.DataReference.CreateConfig();
            StopService("PIAnalysisManager", 10000);
            
            StartService("PIAnalysisManager", 10000);
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
    }
}
