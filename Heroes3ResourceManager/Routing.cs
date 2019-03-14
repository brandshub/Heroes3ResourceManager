using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class Routing
    {
        public string Name { get; set; }
        //first has highest priority
        public List<string> DefaultResourcePriorities { get; set; }
        public List<RoutingRule> ExplicitRules { get; set; }

        public Routing Fallback { get; set; }

        public Routing()
        {
            DefaultResourcePriorities = new List<string>();
            ExplicitRules = new List<RoutingRule>();
        }

        public LodFile Resolve(Heroes3Master master, string fileName)
        {
            var masterResoucesForFile = master.NameToFileMap[fileName.ToLower()];
            // if no, exception is thrown

            if (masterResoucesForFile.Count == 1)
                return master.GetByName(masterResoucesForFile[0]);


            if (ExplicitRules.Count > 0)
            {
                var explicitRule = ExplicitRules.FirstOrDefault(s => string.Compare(fileName, s.FileName, true) == 0);
                if (explicitRule != null)
                {
                    if (explicitRule.ResourseFilePriorities != null && explicitRule.ResourseFilePriorities.Length > 0)
                    {
                        foreach (var rName in explicitRule.ResourseFilePriorities)
                        {
                            if (masterResoucesForFile.Any(s => string.Compare(s, rName, true) == 0))
                                return master.GetByName(rName);
                        }
                    }
                }
            }

            foreach (var defaultResource in DefaultResourcePriorities)
            {
                if (masterResoucesForFile.Any(s => string.Compare(s, defaultResource, true) == 0))
                {
                    return master.GetByName(defaultResource);
                }
            }

            if (Fallback != null)
                return Fallback.Resolve(master, fileName);

            throw new Exception("File: " + fileName + " was not found for routing " + Name);
        }

        public override string ToString()
        {
            return Name;
        }

        public class RoutingRule
        {
            public string FileName { get; set; }
            public string[] ResourseFilePriorities { get; set; }
        }

        public static Routing Default;
        public static Routing Hota;

        static Routing()
        {
            Default = new Routing { Name = "Default", DefaultResourcePriorities = { "h3bitmap.lod", "h3sprite.lod" } };

           // Hota = new Routing { Name = "Hota", DefaultResourcePriorities = { "HotA_lng.lod", "HotA.lod" }, Fallback = Default };
            // problem with canonical un32 un44 is different from hota one, temporarily saving to h3sprite
            Hota = new Routing
            {
                Name = "Hota",
                DefaultResourcePriorities = { "HotA_lng.lod", "HotA.lod" },
                ExplicitRules = { 
                    new RoutingRule { FileName="un32.def", ResourseFilePriorities= new[] { "h3sprite.lod"}},
                    new RoutingRule { FileName="un44.def", ResourseFilePriorities= new[]{ "h3sprite.lod"}}},

                Fallback = Default
            };
        }


    }
}
