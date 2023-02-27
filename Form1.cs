using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Google.OrTools.ConstraintSolver;

namespace BasicExample
{
    public partial class Form1 : Form
    {
        static Label[] row0;
        static Label[] row1;
        static Label[] row2;
        static Label[] row3;
        static Label[] row4;
        static Label[] row5;
        static Label[] row6;
        static Label[] row7;
        static Label[] row8;
        static Label[] row9;
        public static readonly Auto auto0 = new();
        public static readonly Auto auto1 = new();
        public static readonly Auto auto2 = new();
        public static readonly Auto auto3 = new();
        public static readonly Auto auto4 = new();
        static Auto[] auto;
        static List<string[]> laadVlissingen;
        static List<string[]> laadTerneuzen;
        static List<string[]> laadAntwerpen;
        static Auto huidigeAuto = new();
        public List<Auto> beschikbareAutos;
        public List<Label[]> rows = new();
        static int uur;
        static int minuut;
        static string huidigeTijd;
        public Form1()
        {
            InitializeComponent();
            row0 = new Label[6] { ophaaltijd0, aankomsttijd0, ophaalPlaats0, aankomstPlaats0, loods0, bestuurder0};
            row1 = new Label[6] { ophaaltijd1, aankomsttijd1, ophaalPlaats1, aankomstPlaats1, loods1, bestuurder1 };
            row2 = new Label[6] { ophaaltijd2, aankomsttijd2, ophaalPlaats2, aankomstPlaats2, loods2, bestuurder2 };
            row3 = new Label[6] { ophaaltijd3, aankomsttijd3, ophaalPlaats3, aankomstPlaats3, loods3, bestuurder3 };
            row4 = new Label[6] { ophaaltijd4, aankomsttijd4, ophaalPlaats4, aankomstPlaats4, loods4, bestuurder4 };
            row5 = new Label[6] { ophaaltijd5, aankomsttijd5, ophaalPlaats5, aankomstPlaats5, loods5, bestuurder5 };
            row6 = new Label[6] { ophaaltijd6, aankomsttijd6, ophaalPlaats6, aankomstPlaats6, loods6, bestuurder6 };
            row7 = new Label[6] { ophaaltijd7, aankomsttijd7, ophaalPlaats7, aankomstPlaats7, loods7, bestuurder7 };
            row8 = new Label[6] { ophaaltijd8, aankomsttijd8, ophaalPlaats8, aankomstPlaats8, loods8, bestuurder8 };
            row9 = new Label[6] { ophaaltijd9, aankomsttijd9, ophaalPlaats9, aankomstPlaats9, loods9, bestuurder9 };
            rows.Add(row0);
            rows.Add(row1);
            rows.Add(row2);
            rows.Add(row3);
            rows.Add(row4);
            rows.Add(row5);
            rows.Add(row6);
            rows.Add(row7);
            rows.Add(row8);
            rows.Add(row9);
            auto0.SetData("Bestuurder 1", 300, 6f, "Vlissingen");
            auto1.SetData("Bestuurder 2", 300, 7f, "Vlissingen");
            auto2.SetData("Bestuurder 3", 300, 5.5f, "Terneuzen");
            auto3.SetData("Bestuurder 4", 300, 5.5f, "Antwerpen");
            auto4.SetData("Bestuurder 5", 300, 6.5f, "Antwerpen");
            auto = new Auto[5] { auto0, auto1, auto2, auto3, auto4 };
            laadVlissingen = new List<string[]>();
            laadTerneuzen = new List<string[]>();
            laadAntwerpen = new List<string[]>();
            ResetRooster();
            UpdateAutos();
            dataButton.Click += new System.EventHandler(this.DataButton_Click);
            hourButton.Click += new System.EventHandler(this.HourButton_Click);
            minuteButton.Click += new System.EventHandler(this.MinuteButton_Click);
        }
        private void DataButton_Click(object sender, System.EventArgs e)
        {
            if (loodsText.Text == "" || ophaaltijdText.Text == "" || ophaalplaatsText.Text == "" || bestemmingText.Text == "")
                warningText.Text = "Vul eerst alle data in!";
            else
            {
                warningText.Text = "";
                AddData();
            }
        }

        private void HourButton_Click(object sender, System.EventArgs e)
        {
            warningText.Text = "+60 minuten";
            foreach (Label[] row in rows)
            {
                if (row[1].Text == "")
                    continue;
                else if (TimePassed(time.Text, 60, row[1].Text) == true)
                {
                    if (row[5].Text == auto[0].bestuurder)
                        huidigeAuto = auto[0];
                    else if (row[5].Text == auto[1].bestuurder)
                        huidigeAuto = auto[1];
                    else if (row[5].Text == auto[2].bestuurder)
                        huidigeAuto = auto[2];
                    else if (row[5].Text == auto[3].bestuurder)
                        huidigeAuto = auto[3];
                    else if (row[5].Text == auto[4].bestuurder)
                        huidigeAuto = auto[4];
                    huidigeAuto.KMOver -= CalcDistance(row[2].Text, row[3].Text);
                    huidigeAuto.huidigePlaats = ToPlaats(row[3].Text);
                    huidigeAuto.passagier0 = "";
                    EmptyRow(row);
                }
                else if (TimePassed(time.Text, 60, row[0].Text) == true)
                {
                    if (row[5].Text == auto[0].bestuurder)
                        huidigeAuto = auto[0];
                    else if (row[5].Text == auto[1].bestuurder)
                        huidigeAuto = auto[1];
                    else if (row[5].Text == auto[2].bestuurder)
                        huidigeAuto = auto[2];
                    else if (row[5].Text == auto[3].bestuurder)
                        huidigeAuto = auto[3];
                    else if (row[5].Text == auto[4].bestuurder)
                        huidigeAuto = auto[4];
                    huidigeAuto.passagier0 = row[4].Text;
                }
            }
            RoosterToTop();
            time.Text = CalcTime(time.Text, 60);
            UpdateAutos();
            foreach (Auto auto in auto)
            {
                if (laadVlissingen.Count == 0)
                {
                    laadVlissingen.Add(new string[2] { CalcTime(time.Text, 1410), "" });
                }
                if (auto.opdrachten.Count == 0 && auto.huidigePlaats == Plaatsen.Vlissingen && auto.KMOver < auto.actieradius - 25)
                {
                    laadVlissingen.Add(new string[2] { CalcTime(laadVlissingen[^1][0], 30), auto.bestuurder });
                }
                else if (auto.opdrachten.Count == 0)
                    continue;
                else if (auto.huidigePlaats == Plaatsen.Vlissingen && TimePassed(CalcTime(laadVlissingen[^1][0], 30), 30 + CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadVlissingen.Add(new string[2] { CalcTime(laadVlissingen[^1][0], 30), auto.bestuurder });
                }
                else if (auto.opdrachten[0].Split(',')[2] == "Vlissingen" && TimePassed(CalcTime(laadVlissingen[^1][0], 30), 30 + CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadVlissingen.Add(new string[2] { CalcTime(CalcTime(laadVlissingen[^1][0], 30), CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2])), auto.bestuurder });
                }
                else if (auto.KMOver <= CalcDistance(auto.huidigePlaats.ToString(), "Vlissingen") + 50 && TimePassed(CalcTime(laadVlissingen[^1][0], 30), 30 + CalcTravelTime(auto.huidigePlaats.ToString(), "Vlissingen") + CalcTravelTime("Vlissingen", auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]))
                {
                    laadVlissingen.Add(new string[2] { CalcTime(CalcTime(laadVlissingen[^1][0], 30), CalcTravelTime(auto.huidigePlaats.ToString(), "Vlissingen")), auto.bestuurder });
                }
            }
            if (laadVlissingen.Count != 0)
            {
                for (int i = laadVlissingen.Count - 1; i >= 0; i--)
                {
                    if (TimePassed(time.Text, 0, laadVlissingen[i][0]) == true)
                    {
                        foreach (Auto auto in auto)
                        {
                            if (auto.bestuurder == laadVlissingen[i][1])
                            {
                                auto.KMOver = auto.actieradius;
                                auto.huidigePlaats = Plaatsen.Vlissingen;
                            }
                        }
                    }
                    if (TimePassed(CalcTime(time.Text, 1410), 0, laadVlissingen[i][0]) == true)
                    {
                        laadVlissingen.Remove(laadVlissingen[i]);
                    }
                }
            }
            foreach (Auto auto in auto)
            {
                foreach (string[] oplaadOpdracht in laadVlissingen)
                {
                    if (oplaadOpdracht[1] == auto.bestuurder)
                        continue;
                }
                if (auto.opdrachten.Count == 0 && auto.huidigePlaats == Plaatsen.Terneuzen && auto.KMOver < auto.actieradius - 25)
                {
                    laadTerneuzen.Add(new string[2] { time.Text, auto.bestuurder });
                }
                else if (auto.opdrachten.Count == 0)
                    continue;
                else if (auto.huidigePlaats == Plaatsen.Terneuzen && TimePassed(time.Text, 30, auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadTerneuzen.Add(new string[2] { time.Text, auto.bestuurder });
                }
                else if (auto.opdrachten[0].Split(',')[2] == "Terneuzen" && TimePassed(time.Text, 30 + CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadTerneuzen.Add(new string[2] { CalcTime(time.Text, CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2])), auto.bestuurder });
                }
                else if (auto.KMOver <= CalcDistance(auto.huidigePlaats.ToString(), "Terneuzen") + 50 && TimePassed(time.Text, 30 + CalcTravelTime(auto.huidigePlaats.ToString(), "Terneuzen") + CalcTravelTime("Terneuzen", auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]))
                {
                    laadTerneuzen.Add(new string[2] { CalcTime(time.Text, CalcTravelTime(auto.huidigePlaats.ToString(), "Terneuzen")), auto.bestuurder });
                }
            }
            if (laadTerneuzen.Count != 0)
            {
                for (int i = laadTerneuzen.Count - 1; i >= 0; i--)
                {
                    if (TimePassed(time.Text, 0, laadTerneuzen[i][0]) == true)
                    {
                        foreach (Auto auto in auto)
                        {
                            if (auto.bestuurder == laadTerneuzen[i][1])
                            {
                                auto.KMOver = auto.actieradius;
                                auto.huidigePlaats = Plaatsen.Terneuzen;
                            }
                        }
                    }
                    if (TimePassed(CalcTime(time.Text, 1410), 0, laadTerneuzen[i][0]) == true)
                    {
                        laadTerneuzen.Remove(laadTerneuzen[i]);
                    }
                }
            }
            foreach (Auto auto in auto)
            {
                foreach (string[] oplaadOpdracht in laadTerneuzen)
                {
                    if (oplaadOpdracht[1] == auto.bestuurder)
                        continue;
                }
                if (auto.opdrachten.Count == 0 && auto.huidigePlaats == Plaatsen.Antwerpen && auto.KMOver < auto.actieradius - 25)
                {
                    laadAntwerpen.Add(new string[2] { time.Text, auto.bestuurder });
                }
                else if (auto.opdrachten.Count == 0)
                    continue;
                else if (auto.huidigePlaats == Plaatsen.Antwerpen && TimePassed(time.Text, 30, auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadAntwerpen.Add(new string[2] { time.Text, auto.bestuurder });
                }
                else if (auto.opdrachten[0].Split(',')[2] == "Antwerpen" && TimePassed(time.Text, 30 + CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadAntwerpen.Add(new string[2] { CalcTime(time.Text, CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2])), auto.bestuurder });
                }
                else if (auto.KMOver <= CalcDistance(auto.huidigePlaats.ToString(), "Antwerpen") + 50 && TimePassed(time.Text, 30 + CalcTravelTime(auto.huidigePlaats.ToString(), "Antwerpen") + CalcTravelTime("Antwerpen", auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]))
                {
                    laadAntwerpen.Add(new string[2] { CalcTime(time.Text, CalcTravelTime(auto.huidigePlaats.ToString(), "Antwerpen")), auto.bestuurder });
                }
            }
            if (laadAntwerpen.Count != 0)
            {
                for (int i = laadAntwerpen.Count - 1; i >= 0; i--)
                {
                    if (TimePassed(time.Text, 0, laadAntwerpen[i][0]) == true)
                    {
                        foreach (Auto auto in auto)
                        {
                            if (auto.bestuurder == laadAntwerpen[i][1])
                            {
                                auto.KMOver = auto.actieradius;
                                auto.huidigePlaats = Plaatsen.Antwerpen;
                            }
                        }
                    }
                    if (TimePassed(CalcTime(time.Text, 1410), 0, laadAntwerpen[i][0]) == true)
                    {
                        laadAntwerpen.Remove(laadAntwerpen[i]);
                    }
                }
            }
        }

        private void MinuteButton_Click(object sender, System.EventArgs e)
        {
            warningText.Text = "+5 minuten";
            foreach (Label[] row in rows)
            {
                if (row[1].Text == "")
                    continue;
                else if (TimePassed(time.Text, 5, row[1].Text) == true)
                {
                    if (row[5].Text == auto[0].bestuurder)
                        huidigeAuto = auto[0];
                    else if (row[5].Text == auto[1].bestuurder)
                        huidigeAuto = auto[1];
                    else if (row[5].Text == auto[2].bestuurder)
                        huidigeAuto = auto[2];
                    else if (row[5].Text == auto[3].bestuurder)
                        huidigeAuto = auto[3];
                    else if (row[5].Text == auto[4].bestuurder)
                        huidigeAuto = auto[4];
                    huidigeAuto.KMOver -= CalcDistance(row[2].Text, row[3].Text);
                    huidigeAuto.huidigePlaats = ToPlaats(row[3].Text);
                    huidigeAuto.passagier0 = "";
                    EmptyRow(row);
                }
                else if (TimePassed(time.Text, 5, row[0].Text) == true)
                {
                    if (row[5].Text == auto[0].bestuurder)
                        huidigeAuto = auto[0];
                    else if (row[5].Text == auto[1].bestuurder)
                        huidigeAuto = auto[1];
                    else if (row[5].Text == auto[2].bestuurder)
                        huidigeAuto = auto[2];
                    else if (row[5].Text == auto[3].bestuurder)
                        huidigeAuto = auto[3];
                    else if (row[5].Text == auto[4].bestuurder)
                        huidigeAuto = auto[4];
                    huidigeAuto.passagier0 = row[4].Text;
                }
            }
            RoosterToTop();
            time.Text = CalcTime(time.Text, 5);
            UpdateAutos();
            foreach (Auto auto in auto)
            {
                if (laadVlissingen.Count == 0)
                {
                    laadVlissingen.Add(new string[2] { CalcTime(time.Text, 1410), "" });
                }
                if (auto.opdrachten.Count == 0 && auto.huidigePlaats == Plaatsen.Vlissingen && auto.KMOver < auto.actieradius - 25)
                {
                    laadVlissingen.Add(new string[2] { CalcTime(laadVlissingen[^1][0], 30), auto.bestuurder });
                }
                else if (auto.opdrachten.Count == 0)
                    continue;
                else if (auto.huidigePlaats == Plaatsen.Vlissingen && TimePassed(CalcTime(laadVlissingen[^1][0], 30), 30 + CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadVlissingen.Add(new string[2] { CalcTime(laadVlissingen[^1][0], 30), auto.bestuurder });
                }
                else if (auto.opdrachten[0].Split(',')[2] == "Vlissingen" && TimePassed(CalcTime(laadVlissingen[^1][0], 30), 30 + CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadVlissingen.Add(new string[2] { CalcTime(CalcTime(laadVlissingen[^1][0], 30), CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2])), auto.bestuurder });
                }
                else if (auto.KMOver <= CalcDistance(auto.huidigePlaats.ToString(), "Vlissingen") + 50 && TimePassed(CalcTime(laadVlissingen[^1][0], 30), 30 + CalcTravelTime(auto.huidigePlaats.ToString(), "Vlissingen") + CalcTravelTime("Vlissingen", auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]))
                {
                    laadVlissingen.Add(new string[2] { CalcTime(CalcTime(laadVlissingen[^1][0], 30), CalcTravelTime(auto.huidigePlaats.ToString(), "Vlissingen")), auto.bestuurder });
                }
            }
            if (laadVlissingen.Count != 0)
            {
                for (int i = laadVlissingen.Count - 1; i >= 0; i--)
                {
                    if (TimePassed(time.Text, 0, laadVlissingen[i][0]) == true)
                    {
                        foreach (Auto auto in auto)
                        {
                            if (auto.bestuurder == laadVlissingen[i][1])
                            {
                                auto.KMOver = auto.actieradius;
                                auto.huidigePlaats = Plaatsen.Vlissingen;
                            }
                        }
                    }
                    if (TimePassed(CalcTime(time.Text, 1410), 0, laadVlissingen[i][0]) == true)
                    {
                        laadVlissingen.Remove(laadVlissingen[i]);
                    }
                }
            }
            foreach (Auto auto in auto)
            {
                foreach (string[] oplaadOpdracht in laadVlissingen)
                {
                    if (oplaadOpdracht[1] == auto.bestuurder)
                        continue;
                }
                if (auto.opdrachten.Count == 0 && auto.huidigePlaats == Plaatsen.Terneuzen && auto.KMOver < auto.actieradius - 25)
                {
                    laadTerneuzen.Add(new string[2] { time.Text, auto.bestuurder });
                }
                else if (auto.opdrachten.Count == 0)
                    continue;
                else if (auto.huidigePlaats == Plaatsen.Terneuzen && TimePassed(time.Text, 30, auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadTerneuzen.Add(new string[2] { time.Text, auto.bestuurder });
                }
                else if (auto.opdrachten[0].Split(',')[2] == "Terneuzen" && TimePassed(time.Text, 30 + CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadTerneuzen.Add(new string[2] { CalcTime(time.Text, CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2])), auto.bestuurder });
                }
                else if (auto.KMOver <= CalcDistance(auto.huidigePlaats.ToString(), "Terneuzen") + 50 && TimePassed(time.Text, 30 + CalcTravelTime(auto.huidigePlaats.ToString(), "Terneuzen") + CalcTravelTime("Terneuzen", auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]))
                {
                    laadTerneuzen.Add(new string[2] { CalcTime(time.Text, CalcTravelTime(auto.huidigePlaats.ToString(), "Terneuzen")), auto.bestuurder });
                }
            }
            if (laadTerneuzen.Count != 0)
            {
                for (int i = laadTerneuzen.Count - 1; i >= 0; i--)
                {
                    if (TimePassed(time.Text, 0, laadTerneuzen[i][0]) == true)
                    {
                        foreach (Auto auto in auto)
                        {
                            if (auto.bestuurder == laadTerneuzen[i][1])
                            {
                                auto.KMOver = auto.actieradius;
                                auto.huidigePlaats = Plaatsen.Terneuzen;
                            }
                        }
                    }
                    if (TimePassed(CalcTime(time.Text, 1410), 0, laadTerneuzen[i][0]) == true)
                    {
                        laadTerneuzen.Remove(laadTerneuzen[i]);
                    }
                }
            }
            foreach (Auto auto in auto)
            {
                foreach (string[] oplaadOpdracht in laadTerneuzen)
                {
                    if (oplaadOpdracht[1] == auto.bestuurder)
                        continue;
                }
                if (auto.opdrachten.Count == 0 && auto.huidigePlaats == Plaatsen.Antwerpen && auto.KMOver < auto.actieradius - 25)
                {
                    laadAntwerpen.Add(new string[2] { time.Text, auto.bestuurder });
                }
                else if (auto.opdrachten.Count == 0)
                    continue;
                else if (auto.huidigePlaats == Plaatsen.Antwerpen && TimePassed(time.Text, 30, auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadAntwerpen.Add(new string[2] { time.Text, auto.bestuurder });
                }
                else if (auto.opdrachten[0].Split(',')[2] == "Antwerpen" && TimePassed(time.Text, 30 + CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]) == false && auto.KMOver < auto.actieradius - 25)
                {
                    laadAntwerpen.Add(new string[2] { CalcTime(time.Text, CalcTravelTime(auto.huidigePlaats.ToString(), auto.opdrachten[0].Split(',')[2])), auto.bestuurder });
                }
                else if (auto.KMOver <= CalcDistance(auto.huidigePlaats.ToString(), "Antwerpen") + 50 && TimePassed(time.Text, 30 + CalcTravelTime(auto.huidigePlaats.ToString(), "Antwerpen") + CalcTravelTime("Antwerpen", auto.opdrachten[0].Split(',')[2]), auto.opdrachten[0].Split(',')[0]))
                {
                    laadAntwerpen.Add(new string[2] { CalcTime(time.Text, CalcTravelTime(auto.huidigePlaats.ToString(), "Antwerpen")), auto.bestuurder });
                }
            }
            if (laadAntwerpen.Count != 0)
            {
                for (int i = laadAntwerpen.Count - 1; i >= 0; i--)
                {
                    if (TimePassed(time.Text, 0, laadAntwerpen[i][0]) == true)
                    {
                        foreach (Auto auto in auto)
                        {
                            if (auto.bestuurder == laadAntwerpen[i][1])
                            {
                                auto.KMOver = auto.actieradius;
                                auto.huidigePlaats = Plaatsen.Antwerpen;
                            }
                        }
                    }
                    if (TimePassed(CalcTime(time.Text, 1410), 0, laadAntwerpen[i][0]) == true)
                    {
                        laadAntwerpen.Remove(laadAntwerpen[i]);
                    }
                }
            }
        }

        public static bool TimePassed(string currentTime, int minutes, string time)
        {
            if (currentTime == "" || time == "")
                return false;
            uur = Convert.ToInt32(currentTime.Split(':')[0]);
            minuut = Convert.ToInt32(currentTime.Split(':')[1]);
            if (Convert.ToInt32(time.Split(':')[1]) - minuut + 60 * (Convert.ToInt32(time.Split(':')[0]) - uur) - minutes < 0)
            {
                return true;
            }
            else
                return false;
        }

        public void AddData()
        {
            beschikbareAutos = new List<Auto>();
            foreach (Auto auto in auto)
            {
                if (auto.KMOver > CalcDistance(ophaalplaatsText.Text, bestemmingText.Text))
                {
                    beschikbareAutos.Add(auto);
                }
            }
            if (beschikbareAutos.Count != 0)
            {
                for (int i = beschikbareAutos.Count - 1; i >= 0; i--)
                {
                    huidigeAuto = beschikbareAutos[i];
                    if (huidigeAuto.huidigePlaats.ToString() != ophaalplaatsText.Text)
                    {
                        beschikbareAutos.Remove(huidigeAuto);
                        beschikbareAutos.Add(huidigeAuto);
                    }
                    if ((TimePassed(ophaaltijdText.Text, CalcTravelTime(ophaalplaatsText.Text, bestemmingText.Text) + CalcTravelTime(bestemmingText.Text, huidigeAuto.wisselPlaats.ToString()) + 30, ToTijd(huidigeAuto.wisselTijd)) == TimePassed(ToTijd(huidigeAuto.wisselTijd), CalcTravelTime(huidigeAuto.wisselPlaats.ToString(), ophaalplaatsText.Text) + 15, ophaaltijdText.Text)) ||
                        (TimePassed(ophaaltijdText.Text, CalcTravelTime(ophaalplaatsText.Text, bestemmingText.Text) + CalcTravelTime(bestemmingText.Text, huidigeAuto.wisselPlaats.ToString()) + 30, ToTijd(huidigeAuto.wisselTijd + 12f)) == TimePassed(ToTijd(huidigeAuto.wisselTijd + 12f), CalcTravelTime(huidigeAuto.wisselPlaats.ToString(), ophaalplaatsText.Text) + 15, ophaaltijdText.Text)))
                    {
                        beschikbareAutos.Remove(huidigeAuto);
                    }
                    foreach (string opdracht in huidigeAuto.opdrachten)
                    {
                        if (opdracht.Split(',')[3] == ophaalplaatsText.Text)
                        {
                            beschikbareAutos.Remove(huidigeAuto);
                            beschikbareAutos.Insert(0, huidigeAuto);
                        }
                        else
                        {
                            beschikbareAutos.Remove(huidigeAuto);
                            beschikbareAutos.Add(huidigeAuto);
                        }
                        if (beschikbareAutos.Contains(huidigeAuto) == false)
                        {
                            continue;
                        }
                        else if ((TimePassed(opdracht.Split(',')[1], CalcTravelTime(opdracht.Split(',')[3], ophaalplaatsText.Text), ophaaltijdText.Text) == false || TimePassed(ophaaltijdText.Text, CalcTravelTime(ophaalplaatsText.Text, bestemmingText.Text) + CalcTravelTime(bestemmingText.Text, opdracht.Split(',')[2]), opdracht.Split(',')[0]) == false) && huidigeAuto.KMOver > CalcDistance(ophaalplaatsText.Text, bestemmingText.Text) + huidigeAuto.KMOpRooster)
                        {
                            continue;
                        }
                        else
                        {
                            beschikbareAutos.Remove(huidigeAuto);
                        }
                    }
                }
            }
            if (beschikbareAutos.Count == 1)
            {
                AddRooster();
            }
            else if (beschikbareAutos.Count == 0)
            {
                foreach (Auto auto in auto)
                {
                    if (auto.opdrachten.Count != 0)
                    {
                        if (auto.KMOver > CalcDistance(auto.opdrachten[^1], ophaalplaatsText.Text) + CalcDistance(ophaalplaatsText.Text, bestemmingText.Text))
                        {
                            beschikbareAutos.Add(auto);
                        }
                    }
                    else if (auto.KMOver > CalcDistance(auto.huidigePlaats.ToString(), ophaalplaatsText.Text) + CalcDistance(ophaalplaatsText.Text, bestemmingText.Text))
                    {
                        beschikbareAutos.Add(auto);
                    }
                }
                for (int i = beschikbareAutos.Count - 1; i >= 0; i--)
                {
                    huidigeAuto = beschikbareAutos[i];
                    if (huidigeAuto.huidigePlaats.ToString() != ophaalplaatsText.Text)
                    {
                        beschikbareAutos.Remove(huidigeAuto);
                        beschikbareAutos.Add(huidigeAuto);
                    }
                    if ((TimePassed(ophaaltijdText.Text, CalcTravelTime(ophaalplaatsText.Text, bestemmingText.Text) + CalcTravelTime(bestemmingText.Text, huidigeAuto.wisselPlaats.ToString()) + 30, ToTijd(huidigeAuto.wisselTijd)) == TimePassed(ToTijd(huidigeAuto.wisselTijd), CalcTravelTime(huidigeAuto.wisselPlaats.ToString(), ophaalplaatsText.Text) + 15, ophaaltijdText.Text)) ||
                        (TimePassed(ophaaltijdText.Text, CalcTravelTime(ophaalplaatsText.Text, bestemmingText.Text) + CalcTravelTime(bestemmingText.Text, huidigeAuto.wisselPlaats.ToString()) + 30, ToTijd(huidigeAuto.wisselTijd + 12f)) == TimePassed(ToTijd(huidigeAuto.wisselTijd + 12f), CalcTravelTime(huidigeAuto.wisselPlaats.ToString(), ophaalplaatsText.Text) + 15, ophaaltijdText.Text)))
                    {
                        beschikbareAutos.Remove(huidigeAuto);
                    }
                    foreach (string opdracht in huidigeAuto.opdrachten)
                    {
                        if (opdracht.Split(',')[3] == ophaalplaatsText.Text)
                        {
                            beschikbareAutos.Remove(huidigeAuto);
                            beschikbareAutos.Insert(0, huidigeAuto);
                        }
                        else
                        {
                            beschikbareAutos.Remove(huidigeAuto);
                            beschikbareAutos.Add(huidigeAuto);
                        }
                        if (beschikbareAutos.Contains(huidigeAuto) == false)
                        {
                            continue;
                        }
                        else if ((TimePassed(opdracht.Split(',')[1], CalcTravelTime(opdracht.Split(',')[3], ophaalplaatsText.Text), ophaaltijdText.Text) == false || TimePassed(ophaaltijdText.Text, CalcTravelTime(ophaalplaatsText.Text, bestemmingText.Text) + CalcTravelTime(bestemmingText.Text, opdracht.Split(',')[2]), opdracht.Split(',')[0]) == false) && huidigeAuto.KMOver > CalcDistance(ophaalplaatsText.Text, bestemmingText.Text) + huidigeAuto.KMOpRooster)
                        {
                            if (huidigeAuto == beschikbareAutos[0])
                                i += 1;
                            continue;
                        }
                        else
                        {
                            beschikbareAutos.Remove(huidigeAuto);
                        }
                    }
                }
                if (beschikbareAutos.Count == 0)
                    warningText.Text = "Geen auto's beschikbaar";
                else if (beschikbareAutos.Count == 1)
                {
                    AddRooster();
                }
                else
                {
                    for (int i = 1; i <= beschikbareAutos.Count - 1; i++)
                    {
                        if (beschikbareAutos[i].opdrachten.Count == 0 && beschikbareAutos[0].opdrachten.Count == 0)
                        {
                            if (CalcDistance(beschikbareAutos[i].huidigePlaats.ToString(), bestemmingText.Text) < CalcDistance(beschikbareAutos[0].huidigePlaats.ToString(), bestemmingText.Text))
                            {
                                beschikbareAutos.Insert(0, beschikbareAutos[i]);
                                beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                            }
                        }
                        else if (beschikbareAutos[i].opdrachten.Count == 0 && beschikbareAutos[0].opdrachten.Count != 0)
                        {
                            if (CalcDistance(beschikbareAutos[i].huidigePlaats.ToString(), bestemmingText.Text) < CalcDistance(beschikbareAutos[0].opdrachten[beschikbareAutos[i].opdrachten.Count - 1], bestemmingText.Text))
                            {
                                beschikbareAutos.Insert(0, beschikbareAutos[i]);
                                beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                            }
                        }
                        else if (beschikbareAutos[i].opdrachten.Count != 0 && beschikbareAutos[0].opdrachten.Count == 0)
                        {
                            if (CalcDistance(beschikbareAutos[i].opdrachten[^1], bestemmingText.Text) < CalcDistance(beschikbareAutos[0].huidigePlaats.ToString(), bestemmingText.Text))
                            {
                                beschikbareAutos.Insert(0, beschikbareAutos[i]);
                                beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                            }
                        }
                        else if (CalcDistance(beschikbareAutos[i].opdrachten[^1], bestemmingText.Text) < CalcDistance(beschikbareAutos[0].opdrachten[^1], bestemmingText.Text))
                        {
                            beschikbareAutos.Insert(0, beschikbareAutos[i]);
                            beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                        }
                    }
                    for (int i = 1; i <= beschikbareAutos.Count - 1; i++)
                    {
                        if (beschikbareAutos[i].KMOver - beschikbareAutos[i].KMOpRooster > beschikbareAutos[0].KMOver - beschikbareAutos[0].KMOpRooster)
                        {
                            beschikbareAutos.Insert(0, beschikbareAutos[i]);
                            beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                        }
                    }
                    AddRooster();
                }
            }
            else
            {
                for (int i = 1; i <= beschikbareAutos.Count - 1; i++)
                {
                    if (beschikbareAutos[i].opdrachten.Count == 0 && beschikbareAutos[0].opdrachten.Count == 0)
                    {
                        if (CalcDistance(beschikbareAutos[i].huidigePlaats.ToString(), bestemmingText.Text) < CalcDistance(beschikbareAutos[0].huidigePlaats.ToString(), bestemmingText.Text))
                        {
                            beschikbareAutos.Insert(0, beschikbareAutos[i]);
                            beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                        }
                    }
                    else if (beschikbareAutos[i].opdrachten.Count == 0 && beschikbareAutos[0].opdrachten.Count != 0)
                    {
                        if (CalcDistance(beschikbareAutos[i].huidigePlaats.ToString(), bestemmingText.Text) < CalcDistance(beschikbareAutos[0].opdrachten[^1], bestemmingText.Text))
                        {
                            beschikbareAutos.Insert(0, beschikbareAutos[i]);
                            beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                        }
                    }
                    else if (beschikbareAutos[i].opdrachten.Count != 0 && beschikbareAutos[0].opdrachten.Count == 0)
                    {
                        if (CalcDistance(beschikbareAutos[i].opdrachten[^1], bestemmingText.Text) < CalcDistance(beschikbareAutos[0].huidigePlaats.ToString(), bestemmingText.Text))
                        {
                            beschikbareAutos.Insert(0, beschikbareAutos[i]);
                            beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                        }
                    }
                    else if (CalcDistance(beschikbareAutos[i].opdrachten[^1], bestemmingText.Text) < CalcDistance(beschikbareAutos[0].opdrachten[^1], bestemmingText.Text))
                    {
                        beschikbareAutos.Insert(0, beschikbareAutos[i]);
                        beschikbareAutos.Remove(beschikbareAutos[i + 1]);
                    }
                }
                for (int i = 1; i <= beschikbareAutos.Count - 1; i++)
                {
                    if (beschikbareAutos[i].KMOver - beschikbareAutos[i].KMOpRooster > beschikbareAutos[0].KMOver - beschikbareAutos[0].KMOpRooster)
                    {
                        beschikbareAutos.Insert(0, huidigeAuto);
                    }
                }
                AddRooster();
            }
            ophaaltijdText.Text = "";
            ophaalplaatsText.Text = "";
            bestemmingText.Text = "";
            loodsText.Text = "";
        }

        public void AddRooster()
        {
            foreach (Label[] row in rows)
            {
                if (row[0].Text == "")
                {
                    row[0].Text = ophaaltijdText.Text;
                    row[1].Text = CalcTime(ophaaltijdText.Text, CalcTravelTime(ophaalplaatsText.Text, bestemmingText.Text));
                    row[2].Text = ophaalplaatsText.Text;
                    row[3].Text = bestemmingText.Text;
                    row[4].Text = loodsText.Text;
                    row[5].Text = beschikbareAutos[0].bestuurder;
                    beschikbareAutos[0].KMOpRooster += CalcDistance(ophaalplaatsText.Text, bestemmingText.Text);
                    if (beschikbareAutos[0].opdrachten.Contains(ophaaltijdText.Text + "," + row[1].Text + "," + ophaalplaatsText.Text + "," + bestemmingText.Text) == false)
                    {
                        beschikbareAutos[0].opdrachten.Add(ophaaltijdText.Text + "," + row[1].Text + "," + ophaalplaatsText.Text + "," + bestemmingText.Text);
                    }
                    for (int i = 0; i <= beschikbareAutos[0].opdrachten.Count - 2; i++)
                    {
                        if (beschikbareAutos[0].opdrachten[i].Split(',')[3] != beschikbareAutos[0].opdrachten[i + 1].Split(',')[2])
                        {
                            beschikbareAutos[0].KMOpRooster += CalcDistance(beschikbareAutos[0].opdrachten[i].Split(',')[3], beschikbareAutos[0].opdrachten[i + 1].Split(',')[2]);
                        }
                    }
                    break;
                }
            }
        }

        public static string ToTijd(float getalTijd)
        {
            uur = Convert.ToInt32(MathF.Floor(getalTijd));
            minuut = Convert.ToInt32((getalTijd - uur) * 60);
            if (minuut < 10)
                huidigeTijd = uur.ToString() + ":0" + minuut.ToString();
            else
                huidigeTijd = uur.ToString() + ":" + minuut.ToString();
            return huidigeTijd;
        }

        public static string CalcTime(string currentTime, int minutes)
        {
            if (currentTime == "")
                return currentTime;
            uur = Convert.ToInt32(currentTime.Split(':')[0]);
            minuut = Convert.ToInt32(currentTime.Split(':')[1]);
            minuut += minutes;
            while(minuut >= 60)
            {
                uur += 1;
                minuut -= 60;
                if (uur >= 24)
                    uur -= 24;
            }
            if (minuut < 10)
                huidigeTijd = uur.ToString() + ":0" + minuut.ToString();
            else
                huidigeTijd = uur.ToString() + ":" + minuut.ToString();
            return huidigeTijd;
        }

        public static int CalcDistance(string plaats0, string plaats1)
        {
            if (plaats0 == "Vlissingen" && plaats1 == "Terneuzen")
                return 40;
            else if (plaats0 == "Vlissingen" && plaats1 == "Antwerpen")
                return 75;
            else if (plaats0 == "Vlissingen" && plaats1 == "Gent")
                return 60;
            else if (plaats0 == "Terneuzen" && plaats1 == "Vlissingen")
                return 40;
            else if (plaats0 == "Terneuzen" && plaats1 == "Antwerpen")
                return 75;
            else if (plaats0 == "Terneuzen" && plaats1 == "Gent")
                return 30;
            else if (plaats0 == "Antwerpen" && plaats1 == "Vlissingen")
                return 80;
            else if (plaats0 == "Antwerpen" && plaats1 == "Terneuzen")
                return 75;
            else if (plaats0 == "Antwerpen" && plaats1 == "Gent")
                return 50;
            else if (plaats0 == "Gent" && plaats1 == "Vlissingen")
                return 60;
            else if (plaats0 == "Gent" && plaats1 == "Terneuzen")
                return 30;
            else if (plaats0 == "Gent" && plaats1 == "Antwerpen")
                return 50;
            else
                return 0;
        }

        public static int CalcTravelTime(string plaats0, string plaats1)
        {
            if (plaats0 == "Vlissingen" && plaats1 == "Terneuzen")
                return 30;
            else if (plaats0 == "Vlissingen" && plaats1 == "Antwerpen")
                return 60;
            else if (plaats0 == "Vlissingen" && plaats1 == "Gent")
                return 60;
            else if (plaats0 == "Terneuzen" && plaats1 == "Vlissingen")
                return 30;
            else if (plaats0 == "Terneuzen" && plaats1 == "Antwerpen")
                return 60;
            else if (plaats0 == "Terneuzen" && plaats1 == "Gent")
                return 30;
            else if (plaats0 == "Antwerpen" && plaats1 == "Vlissingen")
                return 60;
            else if (plaats0 == "Antwerpen" && plaats1 == "Terneuzen")
                return 60;
            else if (plaats0 == "Antwerpen" && plaats1 == "Gent")
                return 45;
            else if (plaats0 == "Gent" && plaats1 == "Vlissingen")
                return 60;
            else if (plaats0 == "Gent" && plaats1 == "Terneuzen")
                return 30;
            else if (plaats0 == "Gent" && plaats1 == "Antwerpen")
                return 45;
            else
                return 0;
        }

        public static void ResetRow(Label[] row)
        {
            foreach (Label label in row)
            {
                label.Text = "";
            }
        }

        public static void ResetRooster()
        {
            ResetRow(row0);
            ResetRow(row1);
            ResetRow(row2);
            ResetRow(row3);
            ResetRow(row4);
            ResetRow(row5);
            ResetRow(row6);
            ResetRow(row7);
            ResetRow(row8);
            ResetRow(row9);
        }

        public static void EmptyRow(Label[] row)
        {
            row[0].Text = "";
            row[1].Text = "";
            row[2].Text = "";
            row[3].Text = "";
            row[4].Text = "";
            row[5].Text = "";
        }

        public void RoosterToTop()
        {
            for (int i = 0; i < rows.Count; i++)
            {
                Label[] row = rows[i];
                if (row[0].Text == "" && i < rows.Count - 1)
                {
                    row[0].Text = rows[i + 1][0].Text;
                    row[1].Text = rows[i + 1][1].Text;
                    row[2].Text = rows[i + 1][2].Text;
                    row[3].Text = rows[i + 1][3].Text;
                    row[4].Text = rows[i + 1][4].Text;
                    row[5].Text = rows[i + 1][5].Text;
                    EmptyRow(rows[i + 1]);
                }
            }
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

        public void UpdateAutos()
        {
            foreach (Auto auto in auto)
            {
                for (int i = auto.opdrachten.Count - 1; i >= 0; i--)
                {
                    if (TimePassed(time.Text, 0, auto.opdrachten[i].Split(',')[1]))
                    {
                        auto.KMOpRooster -= CalcDistance(auto.opdrachten[i].Split(',')[2], auto.opdrachten[i].Split(',')[3]);
                        auto.opdrachten.Remove(auto.opdrachten[i]);
                    }
                }
            }
            KMOver1Text.Text = auto[0].KMOver.ToString() + "/" + auto[0].actieradius.ToString();
            KMOver2Text.Text = auto[1].KMOver.ToString() + "/" + auto[1].actieradius.ToString();
            KMOver3Text.Text = auto[2].KMOver.ToString() + "/" + auto[2].actieradius.ToString();
            KMOver4Text.Text = auto[3].KMOver.ToString() + "/" + auto[3].actieradius.ToString();
            KMOver5Text.Text = auto[4].KMOver.ToString() + "/" + auto[4].actieradius.ToString();
            plaats1Text.Text = auto[0].huidigePlaats.ToString();
            plaats2Text.Text = auto[1].huidigePlaats.ToString();
            plaats3Text.Text = auto[2].huidigePlaats.ToString();
            plaats4Text.Text = auto[3].huidigePlaats.ToString();
            plaats5Text.Text = auto[4].huidigePlaats.ToString();
            opladen1Text.Text = "";
            opladen2Text.Text = "";
            opladen3Text.Text = "";
            opladen4Text.Text = "";
            opladen5Text.Text = "";
            if (laadVlissingen.Count != 0)
            {
                foreach (string[] afspraak in laadVlissingen)
                {
                    if (afspraak[1] == auto[0].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen1Text.Text = "Opladen...";
                    if (afspraak[1] == auto[1].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen2Text.Text = "Opladen...";
                    if (afspraak[1] == auto[2].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen3Text.Text = "Opladen...";
                    if (afspraak[1] == auto[3].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen4Text.Text = "Opladen...";
                    if (afspraak[1] == auto[4].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen5Text.Text = "Opladen...";
                }
            }
            if (laadTerneuzen.Count != 0)
            {
                foreach (string[] afspraak in laadTerneuzen)
                {
                    if (afspraak[1] == auto[0].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen1Text.Text = "Opladen...";
                    if (afspraak[1] == auto[1].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen2Text.Text = "Opladen...";
                    if (afspraak[1] == auto[2].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen3Text.Text = "Opladen...";
                    if (afspraak[1] == auto[3].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen4Text.Text = "Opladen...";
                    if (afspraak[1] == auto[4].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen5Text.Text = "Opladen...";
                }
            }
            if (laadAntwerpen.Count != 0)
            {
                foreach (string[] afspraak in laadAntwerpen)
                {
                    if (afspraak[1] == auto[0].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen1Text.Text = "Opladen...";
                    if (afspraak[1] == auto[1].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen2Text.Text = "Opladen...";
                    if (afspraak[1] == auto[2].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen3Text.Text = "Opladen...";
                    if (afspraak[1] == auto[3].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen4Text.Text = "Opladen...";
                    if (afspraak[1] == auto[4].bestuurder && TimePassed(time.Text, 0, afspraak[0]) == true)
                        opladen5Text.Text = "Opladen...";
                }
            }
        }
    }
}