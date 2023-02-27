using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicExample
{
    public enum Plaatsen
    {
        Vlissingen,
        Terneuzen,
        Antwerpen,
        Gent
    };
    public class Auto
    {
        public string bestuurder;
        public int actieradius;
        public int KMOver;
        public int KMOpRooster;
        public float wisselTijd;
        public Plaatsen wisselPlaats;
        public Plaatsen huidigePlaats;
        public string passagier0;
        public string passagier1;
        public string passagier2;
        public string passagier3;
        public List<string> opdrachten;

        public void SetData(string naam, int autoActieradius, float tijd, string plaats)
        {
            bestuurder = naam;
            actieradius = autoActieradius;
            KMOver = actieradius;
            KMOpRooster = 0;
            wisselTijd = tijd;
            wisselPlaats = ToPlaats(plaats);
            huidigePlaats = wisselPlaats;
            passagier0 = "";
            passagier1 = "";
            passagier2 = "";
            passagier3 = "";
            opdrachten = new List<string>();
        }

        public static Plaatsen ToPlaats(string plaats)
        {
            if (plaats == "Vlissingen")
                return Plaatsen.Vlissingen;
            else if (plaats == "Terneuzen")
                return Plaatsen.Terneuzen;
            else if (plaats == "Antwerpen")
                return Plaatsen.Antwerpen;
            else if (plaats == "Gent")
                return Plaatsen.Gent;
            else
                return Plaatsen.Vlissingen;
        }
    }
}
