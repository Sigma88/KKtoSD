using UnityEngine;
using System.Collections.Generic;
using SigmaDimensionsPlugin;
using KerbalKonstructs.Core;
using System.Linq;


namespace KKtoSDPlugin
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class KKGroups : MonoBehaviour
    {
        void Awake()
        {
            // Version Check
            Debug.Log("[SigmaLog] Version Check:   KKtoSD v0.1.5");

            // Check if KK is installed
            if (AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "KerbalKonstructs") == null) return;

            // Get the groups from KK
            StaticInstance[] KKStatics = StaticDatabase.GetAllStatics();

            // Send the groups to SD
            foreach (StaticInstance KKStatic in KKStatics)
            {
                CelestialBody planet = KKStatic?.CelestialBody;
                string group = KKStatic?.Group;
                PQSCity mod = KKStatic?.pqsCity;

                if (planet == null || string.IsNullOrEmpty(group) || mod == null) continue;


                if (!PQSCityGroups.ExternalExceptions.ContainsKey(planet))
                    PQSCityGroups.ExternalExceptions.Add(planet, new Dictionary<string, List<object>>());

                if (!PQSCityGroups.ExternalExceptions[planet].ContainsKey(group))
                    PQSCityGroups.ExternalExceptions[planet].Add(group, new List<object>());

                if (!PQSCityGroups.ExternalExceptions[planet][group].Contains(mod))
                {
                    PQSCityGroups.ExternalExceptions[planet][group].Add(mod);
                }
            }
        }
    }
}
