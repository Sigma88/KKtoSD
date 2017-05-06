using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Kopernicus;
using SigmaDimensionsPlugin;


namespace KKtoSDPlugin
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class KKGroups : MonoBehaviour
    {
        void Awake()
        {
            // Build a list of all instances by position
            Dictionary<Vector3, string[]> positionsList = new Dictionary<Vector3, string[]>();

            foreach (ConfigNode STATIC in GameDatabase.Instance.GetConfigNodes("STATIC"))
            {
                foreach (ConfigNode Instance in STATIC.GetNodes("Instances"))
                {
                    if (Instance.HasValue("CelestialBody") && Instance.HasValue("Group") && Instance.HasValue("RadialPosition"))
                    {
                        Vector3Parser position = new Vector3Parser();
                        position.SetFromString(Instance.GetValue("RadialPosition"));

                        string[] group = new[] { Instance.GetValue("CelestialBody"), Instance.GetValue("Group") };

                        if (!positionsList.ContainsKey(position))
                            positionsList.Add(position, group);
                    }
                }
            }


            // Build a list of all mods divided by group
            Dictionary<string, Dictionary<string, List<object>>> planetsList = new Dictionary<string, Dictionary<string, List<object>>>();

            foreach (Vector3 position in positionsList.Keys)
            {
                string[] group = positionsList[position];
                CelestialBody body = FlightGlobals.Bodies.FirstOrDefault(b => b.name == group[0]);
                if (body == null) continue;
                PQSCity mod = body.GetComponentsInChildren<PQSCity>(true).FirstOrDefault(m => m.repositionRadial == position);
                if (mod == null) continue;

                if (!planetsList.ContainsKey(group[0]))
                    planetsList.Add(group[0], new Dictionary<string, List<object>>());

                if (!planetsList[group[0]].ContainsKey(group[1]))
                    planetsList[group[0]].Add(group[1], new List<object>());

                planetsList[group[0]][group[1]].Add(mod);
            }


            // Send the groups to SD
            foreach (string planet in planetsList.Keys)
            {
                if (!PQSCityGroups.ExternalGroups.ContainsKey(planet))
                    PQSCityGroups.ExternalGroups.Add(planet, new Dictionary<string, List<object>>());

                foreach (string group in planetsList[planet].Keys)
                {
                    if (!PQSCityGroups.ExternalGroups[planet].ContainsKey(group))
                        PQSCityGroups.ExternalGroups[planet].Add(group, new List<object>());

                    foreach (object mod in planetsList[planet][group])
                    {
                        if (!PQSCityGroups.ExternalGroups[planet][group].Contains(mod))
                            PQSCityGroups.ExternalGroups[planet][group].Add(mod);
                    }
                }
            }
        }
    }
}
