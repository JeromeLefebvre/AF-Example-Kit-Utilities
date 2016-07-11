using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.UnitsOfMeasure;
using System;

namespace ConvertFromAmericanCurrency
{
    class Program
    {
        static void Main(string[] args)
        {
            PISystem system = PISystem.CreatePISystem("localhost");
            AFDatabase db = system.Databases["kit6"];

            string newCurrencyName = "円";
            string newAbreviation = "円";
            string oldUOMClassName = "US Currency";
            string newUOMClassName = "JP Currency";

            UOMClass newUOMClass = createUOM(system, newUOMClassName, newCurrencyName, newAbreviation);
            UOM newUOM = newUOMClass.CanonicalUOM;

            UOMClass oldUOMClas = system.UOMDatabase.UOMClasses[oldUOMClassName];

            Console.WriteLine("Will now change all attributes using the old UOM to the new UOM");
            Console.ReadLine();
            try { 
                foreach (AFElementTemplate et in db.ElementTemplates)
                {
                    foreach (AFAttributeTemplate at in et.AttributeTemplates)
                    {
                        convertAttributeTemplate(at, oldUOMClas, newUOM);
                    }
                }
                db.CheckIn();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to change an attribute's UOM");
                Console.WriteLine(e);
                return;
            }
            Console.ReadLine();
        }

        static void convertAttributeTemplate(AFAttributeTemplate at, UOMClass uomclass, UOM newUOM)
        {
            if (at.DefaultUOM != null && at.DefaultUOM.Class == uomclass)
            {
                at.DefaultUOM = newUOM;
                at.DataReference.UOM = newUOM;
            }
            foreach (AFAttributeTemplate child in at.AttributeTemplates)
            {
                convertAttributeTemplate(child, uomclass, newUOM);
            }
        }

        static UOMClass createUOM(PISystem system, string UOMClassName, string UOMCanonicalName, string UOMCanonicalAbbreviation)
        {
            UOMDatabase uomdb = system.UOMDatabase;
            UOMClass UOMClass = uomdb.UOMClasses[UOMClassName];
            if (UOMClass == null)
            {
                UOMClass = uomdb.UOMClasses.Add(UOMClassName, UOMCanonicalName, UOMCanonicalAbbreviation);
                system.CheckIn();
            }
            if (UOMClass.CanonicalUOM.Name != UOMCanonicalName)
            {
                throw new System.ArgumentException("The current existing UOM Classes has a different canonical UOM");
            }
            return UOMClass;
        }
    }
}
