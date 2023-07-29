using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace TeamVacationPlanner.EspnApi
{
    public class SportsEvent
    {
        public string CompetitionA { get; set; }
        public string CompetitionB { get; set; }
        public string LocationA { get; set; }
        public string LocationB { get; set; }
        public DateTime DateTimeA { get; set; }
        public DateTime DateTimeB { get; set; }
        public string Row
        {
            get
            {
                return $"| {CompetitionA,-13} | {CompetitionB,-13} | {LocationA,-10} | {LocationB,-10} | {DateTimeA.ToShortDateString(),-8} | {DateTimeB.ToShortDateString(),-8} |";
            }
        }
    }

    public class ESPNApiResponse
    {
        public List<SportData> Sports { get; set; }
    }

    public class SportData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<LeagueData> Leagues { get; set; }
    }

    public class LeagueData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<TeamData> Teams { get; set; }
    }

    public class TeamData
    {
        public TeamInfo Team { get; set; }
    }

    public class TeamInfo
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ShortDisplayName { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Location { get; set; }
        public string Slug { get; set; }
        public string Abbreviation { get; set; }
        // You can include other properties as needed
    }

    public class Parent
    {
        public int Id { get; set; }
    }

    public class Groups
    {
        public int Id { get; set; }
        public Parent Parent { get; set; }
        public bool IsConference { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Clubhouse { get; set; }
        public string Color { get; set; }
        public string Logo { get; set; }
        public string RecordSummary { get; set; }
        public string SeasonSummary { get; set; }
        public string StandingSummary { get; set; }
        public Groups Groups { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public string State { get; set; }
        public int? ZipCode { get; set; }

        //public States StateAbbreviation
        //{
        //    get
        //    {
        //        switch (State.ToUpper())
        //        {
        //            case "ALABAMA":
        //                return States.AL;
        //            case "ALASKA":
        //                return States.AK;
        //            case "AMERICAN SAMOA":
        //                return States.AS;
        //            case "ARIZONA":
        //                return States.AZ;
        //            case "ARKANSAS":
        //                return States.AR;
        //            case "CALIFORNIA":
        //                return States.CA;
        //            case "COLORADO":
        //                return States.CO;
        //            case "CONNECTICUT":
        //                return States.CT;
        //            case "DELAWARE":
        //                return States.DE;
        //            case "DISTRICT OF COLUMBIA":
        //                return States.DC;
        //            case "FEDERATED STATES OF MICRONESIA":
        //                return States.FM;
        //            case "FLORIDA":
        //                return States.FL;
        //            case "GEORGIA":
        //                return States.GA;
        //            case "GUAM":
        //                return States.GU;
        //            case "HAWAII":
        //                return States.HI;
        //            case "IDAHO":
        //                return States.ID;
        //            case "ILLINOIS":
        //                return States.IL;
        //            case "INDIANA":
        //                return States.IN;
        //            case "IOWA":
        //                return States.IA;
        //            case "KANSAS":
        //                return States.KS;
        //            case "KENTUCKY":
        //                return States.KY;
        //            case "LOUISIANA":
        //                return States.LA;
        //            case "MAINE":
        //                return States.ME;
        //            case "MARSHALL ISLANDS":
        //                return States.MH;
        //            case "MARYLAND":
        //                return States.MD;
        //            case "MASSACHUSETTS":
        //                return States.MA;
        //            case "MICHIGAN":
        //                return States.MI;
        //            case "MINNESOTA":
        //                return States.MN;
        //            case "MISSISSIPPI":
        //                return States.MS;
        //            case "MISSOURI":
        //                return States.MO;
        //            case "MONTANA":
        //                return States.MT;
        //            case "NEBRASKA":
        //                return States.NE;
        //            case "NEVADA":
        //                return States.NV;
        //            case "NEW HAMPSHIRE":
        //                return States.NH;
        //            case "NEW JERSEY":
        //                return States.NJ;
        //            case "NEW MEXICO":
        //                return States.NM;
        //            case "NEW YORK":
        //                return States.NY;
        //            case "NORTH CAROLINA":
        //                return States.NC;
        //            case "NORTH DAKOTA":
        //                return States.ND;
        //            case "NORTHERN MARIANA ISLANDS":
        //                return States.MP;
        //            case "OHIO":
        //                return States.OH;
        //            case "OKLAHOMA":
        //                return States.OK;
        //            case "OREGON":
        //                return States.OR;
        //            case "PALAU":
        //                return States.PW;
        //            case "PENNSYLVANIA":
        //                return States.PA;
        //            case "PUERTO RICO":
        //                return States.PR;
        //            case "RHODE ISLAND":
        //                return States.RI;
        //            case "SOUTH CAROLINA":
        //                return States.SC;
        //            case "SOUTH DAKOTA":
        //                return States.SD;
        //            case "TENNESSEE":
        //                return States.TN;
        //            case "TEXAS":
        //                return States.TX;
        //            case "UTAH":
        //                return States.UT;
        //            case "VERMONT":
        //                return States.VT;
        //            case "VIRGIN ISLANDS":
        //                return States.VI;
        //            case "VIRGINIA":
        //                return States.VA;
        //            case "WASHINGTON":
        //                return States.WA;
        //            case "WEST VIRGINIA":
        //                return States.WV;
        //            case "WISCONSIN":
        //                return States.WI;
        //            case "WYOMING":
        //                return States.WY;
        //            default:
        //                throw new KeyNotFoundException();
        //        }
        //    }
        //}
        public string StateName
        {
            get
            {
                switch (State?.ToUpperInvariant() ?? City?.Split(',')?.Last()?.TrimStart().ToUpperInvariant())
                {
                    case "ALABAMA":
                        return "AL";
                    case "ALASKA":
                        return "AK";
                    case "AMERICAN SAMOA":
                        return "AS";
                    case "ARIZONA":
                        return "AZ";
                    case "ARKANSAS":
                        return "AR";
                    case "CALIFORNIA":
                        return "CA";
                    case "COLORADO":
                        return "CO";
                    case "CONNECTICUT":
                        return "CT";
                    case "DELAWARE":
                        return "DE";
                    case "DISTRICT OF COLUMBIA":
                        return "DC";
                    case "FEDERATED STATES OF MICRONESIA":
                        return "FM";
                    case "FLORIDA":
                        return "FL";
                    case "GEORGIA":
                        return "GA";
                    case "GUAM":
                        return "GU";
                    case "HAWAII":
                        return "HI";
                    case "IDAHO":
                        return "ID";
                    case "ILLINOIS":
                        return "IL";
                    case "INDIANA":
                        return "IN";
                    case "IOWA":
                        return "IA";
                    case "KANSAS":
                        return "KS";
                    case "KENTUCKY":
                        return "KY";
                    case "LOUISIANA":
                        return "LA";
                    case "MAINE":
                        return "ME";
                    case "MARSHALL ISLANDS":
                        return "MH";
                    case "MARYLAND":
                        return "MD";
                    case "MASSACHUSETTS":
                        return "MA";
                    case "MICHIGAN":
                        return "MI";
                    case "MINNESOTA":
                        return "MN";
                    case "MISSISSIPPI":
                        return "MS";
                    case "MISSOURI":
                        return "MO";
                    case "MONTANA":
                        return "MT";
                    case "NEBRASKA":
                        return "NE";
                    case "NEVADA":
                        return "NV";
                    case "NEW HAMPSHIRE":
                        return "NH";
                    case "NEW JERSEY":
                        return "NJ";
                    case "NEW MEXICO":
                        return "NM";
                    case "NEW YORK":
                        return "NY";
                    case "NORTH CAROLINA":
                        return "NC";
                    case "NORTH DAKOTA":
                        return "ND";
                    case "NORTHERN MARIANA ISLANDS":
                        return "MP";
                    case "OHIO":
                        return "OH";
                    case "OKLAHOMA":
                        return "OK";
                    case "OREGON":
                        return "OR";
                    case "PALAU":
                        return "PW";
                    case "PENNSYLVANIA":
                        return "PA";
                    case "PUERTO RICO":
                        return "PR";
                    case "RHODE ISLAND":
                        return "RI";
                    case "SOUTH CAROLINA":
                        return "SC";
                    case "SOUTH DAKOTA":
                        return "SD";
                    case "TENNESSEE":
                        return "TN";
                    case "TEXAS":
                        return "TX";
                    case "UTAH":
                        return "UT";
                    case "VERMONT":
                        return "VT";
                    case "VIRGIN ISLANDS":
                        return "VI";
                    case "VIRGINIA":
                        return "VA";
                    case "WASHINGTON":
                        return "WA";
                    case "WEST VIRGINIA":
                        return "WV";
                    case "WISCONSIN":
                        return "WI";
                    case "WYOMING":
                        return "WY";
                    default:
                        return State;
                }
            }
        }
    }

    public class Venue
    {
        public string FullName { get; set; }
        public Address Address { get; set; }
    }

    public class Type
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Abbreviation { get; set; }
        public string Slug { get; set; }
        //public string Type { get; set; }
    }

    public class Competitor
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
        public string HomeAway { get; set; }
        public bool Winner { get; set; }
        public Team Team { get; set; }
        //public Score Score { get; set; }
        public List<Record> Record { get; set; }
    }

    public class Score
    {
        public int Value { get; set; }
        public string DisplayValue { get; set; }
    }

    public class Record
    {
        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public string DisplayName { get; set; }
        public string ShortDisplayName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string DisplayValue { get; set; }
    }

    public class Athlete
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string ShortName { get; set; }
        public List<Link> Links { get; set; }
        public string Record { get; set; }
        public string Saves { get; set; }
    }

    public class Link
    {
        public List<string> Rel { get; set; }
        public string Href { get; set; }
    }

    public class FeaturedAthlete
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ShortDisplayName { get; set; }
        public string Abbreviation { get; set; }
        public int PlayerId { get; set; }
        public Athlete Athlete { get; set; }
        public Team Team { get; set; }
    }

    public class Status
    {
        public double Clock { get; set; }
        public string DisplayClock { get; set; }
        public int Period { get; set; }
        public Type Type { get; set; }
        public List<FeaturedAthlete> FeaturedAthletes { get; set; }
        public int HalfInning { get; set; }
        public string PeriodPrefix { get; set; }
    }

    public class Event
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public List<Competition> Competitions { get; set; }
        public Competition Competition { get { return Competitions.FirstOrDefault(); } }
    }

    public class Competition
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Attendance { get; set; }
        public Type Type { get; set; }
        public bool TimeValid { get; set; }
        public bool NeutralSite { get; set; }
        public bool BoxscoreAvailable { get; set; }
        public bool TicketsAvailable { get; set; }
        public Venue Venue { get; set; }
        public List<Competitor> Competitors { get; set; }
        public List<object> Notes { get; set; }
        public List<object> Broadcasts { get; set; }
        public Status Status { get; set; }
    }

    public class Season
    {
        public int Year { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Half { get; set; }
    }

    public class RequestedSeason
    {
        public int Year { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class ESPNApiResponse2
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public Season Season { get; set; }
        public Team Team { get; set; }
        public DateTime Allstarsgame { get; set; }
        public List<Event> Events { get; set; }
        public RequestedSeason RequestedSeason { get; set; }
    }

    public enum States
    {
        AL,
        AK,
        AS,
        AZ,
        AR,
        CA,
        CO,
        CT,
        DE,
        DC,
        FM,
        FL,
        GA,
        GU,
        HI,
        ID,
        IL,
        IN,
        IA,
        KS,
        KY,
        LA,
        ME,
        MH,
        MD,
        MA,
        MI,
        MN,
        MS,
        MO,
        MT,
        NE,
        NV,
        NH,
        NJ,
        NM,
        NY,
        NC,
        ND,
        MP,
        OH,
        OK,
        OR,
        PW,
        PA,
        PR,
        RI,
        SC,
        SD,
        TN,
        TX,
        UT,
        VT,
        VI,
        VA,
        WA,
        WV,
        WI,
        WY
    }
}