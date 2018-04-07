using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample03.E3SClient.Entities
{
    public class Event
    {

        [JsonProperty("origin")]
        public string origin { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("visibility")]
        public string visibility { get; set; }
    }

    public class Staff
    {

        [JsonProperty("origin")]
        public string origin { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("visibility")]
        public string visibility { get; set; }
    }

    public class Hero
    {

        [JsonProperty("origin")]
        public string origin { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("visibility")]
        public string visibility { get; set; }
    }

    public class UPSA
    {

        [JsonProperty("origin")]
        public string origin { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("visibility")]
        public string visibility { get; set; }
    }

    public class Profile
    {

        [JsonProperty("Events")]
        public IList<Event> Events { get; set; }

        [JsonProperty("Staff")]
        public IList<Staff> Staff { get; set; }

        [JsonProperty("Hero")]
        public IList<Hero> Hero { get; set; }

        [JsonProperty("UPSA")]
        public IList<UPSA> UPSA { get; set; }

        [JsonProperty("Yammer")]
        public IList<Yammer> Yammer { get; set; }
    }

    public class Badge
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("category")]
        public string category { get; set; }

        [JsonProperty("e3sImageExists")]
        public bool e3sImageExists { get; set; }
    }

    public class Coordinates
    {

        [JsonProperty("lat")]
        public double lat { get; set; }

        [JsonProperty("lon")]
        public double lon { get; set; }
    }

    public class E3sExtensions
    {
    }

    public class Key
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("fullKey")]
        public string fullKey { get; set; }
    }

    public class Skill
    {

        [JsonProperty("expert")]
        public string expert { get; set; }

        [JsonProperty("primary")]
        public string primary { get; set; }
    }

    public class E3sAttributes
    {

        [JsonProperty("_e3sNode")]
        public string _e3sNode { get; set; }
    }

    public class Yammer
    {

        [JsonProperty("origin")]
        public string origin { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("visibility")]
        public string visibility { get; set; }
    }

    public class Social
    {

        [JsonProperty("Yammer")]
        public IList<Yammer> Yammer { get; set; }
    }


    [E3SMetaType("meta:people-suite:people-api:com.epam.e3s.app.people.api.data.pluggable.EmployeeEntity")]
	public class EmployeeEntity : E3SEntity
	{
        [JsonProperty("workStation")]
        public string workStation { get; set; }

        [JsonProperty("_e3sId")]
        public string _e3sId { get; set; }

        [JsonProperty("employmentStatus")]
        public string employmentStatus { get; set; }

        [JsonProperty("unitHead")]
        public string unitHead { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("upsaId")]
        public string upsaId { get; set; }

        [JsonProperty("profile")]
        public Profile profile { get; set; }

        [JsonProperty("ipDedicated")]
        public double ipDedicated { get; set; }

        [JsonProperty("_e3sCreated")]
        public long _e3sCreated { get; set; }

        [JsonProperty("addToDl")]
        public IList<string> addToDl { get; set; }

        [JsonProperty("citySum")]
        public string citySum { get; set; }

        [JsonProperty("badge")]
        public IList<Badge> badge { get; set; }

        [JsonProperty("isRm")]
        public bool isRm { get; set; }

        [JsonProperty("_e3sUpdated")]
        public long _e3sUpdated { get; set; }

        [JsonProperty("phone")]
        public IList<string> phone { get; set; }

        [JsonProperty("_e3sVersion")]
        public int _e3sVersion { get; set; }

        [JsonProperty("nativeName")]
        public string nativeName { get; set; }

        [JsonProperty("orgCategory")]
        public string orgCategory { get; set; }

        [JsonProperty("lastName")]
        public string lastName { get; set; }

        [JsonProperty("city")]
        public IList<string> city { get; set; }

        [JsonProperty("displayName")]
        public string displayName { get; set; }

        [JsonProperty("availability")]
        public string availability { get; set; }

        [JsonProperty("projectAll")]
        public string projectAll { get; set; }

        [JsonProperty("event")]
        public IList<string> myevent { get; set; }

        [JsonProperty("sourceIds")]
        public IList<string> sourceIds { get; set; }

        [JsonProperty("email")]
        public IList<string> email { get; set; }

        [JsonProperty("workPlace")]
        public string workPlace { get; set; }

        [JsonProperty("coordinates")]
        public Coordinates coordinates { get; set; }

        [JsonProperty("photo")]
        public IList<string> photo { get; set; }

        [JsonProperty("url")]
        public IList<string> url { get; set; }

        [JsonProperty("socialNetwork")]
        public IList<string> socialNetwork { get; set; }

        [JsonProperty("primaryTitle")]
        public string primaryTitle { get; set; }

        [JsonProperty("cefr")]
        public string cefr { get; set; }

        [JsonProperty("country")]
        public IList<string> country { get; set; }

        [JsonProperty("recognitionScore")]
        public string recognitionScore { get; set; }

        [JsonProperty("primarySkill")]
        public string primarySkill { get; set; }

        [JsonProperty("dl")]
        public IList<string> dl { get; set; }

        [JsonProperty("project")]
        public string project { get; set; }

        [JsonProperty("office")]
        public string office { get; set; }

        [JsonProperty("countrySum")]
        public string countrySum { get; set; }

        [JsonProperty("talk")]
        public IList<string> talk { get; set; }

        [JsonProperty("nonBillable")]
        public double nonBillable { get; set; }

        [JsonProperty("_e3sExtensions")]
        public E3sExtensions _e3sExtensions { get; set; }

        [JsonProperty("documentBoost")]
        public double documentBoost { get; set; }

        [JsonProperty("article")]
        public IList<string> article { get; set; }

        [JsonProperty("firstName")]
        public string firstName { get; set; }

        [JsonProperty("topic")]
        public IList<string> topic { get; set; }

        [JsonProperty("region")]
        public string region { get; set; }

        [JsonProperty("deliveryManager")]
        public IList<object> deliveryManager { get; set; }

        [JsonProperty("keys")]
        public IList<Key> keys { get; set; }

        [JsonProperty("phones")]
        public IList<string> phones { get; set; }

        [JsonProperty("hero")]
        public IList<string> hero { get; set; }

        [JsonProperty("title")]
        public IList<string> title { get; set; }

        [JsonProperty("nonBillableA")]
        public double nonBillableA { get; set; }

        [JsonProperty("principal")]
        public string principal { get; set; }

        [JsonProperty("trainer")]
        public IList<string> trainer { get; set; }

        [JsonProperty("skill")]
        public Skill skill { get; set; }

        [JsonProperty("englishAssessmentState")]
        public string englishAssessmentState { get; set; }

        [JsonProperty("phoneticFullName")]
        public IList<string> phoneticFullName { get; set; }

        [JsonProperty("npa")]
        public double npa { get; set; }

        [JsonProperty("unitContact")]
        public IList<string> unitContact { get; set; }

        [JsonProperty("_e3sAttributes")]
        public E3sAttributes _e3sAttributes { get; set; }

        [JsonProperty("social")]
        public Social social { get; set; }

        [JsonProperty("fullName")]
        public IList<string> fullName { get; set; }

        [JsonProperty("executiveAssistant")]
        public IList<string> executiveAssistant { get; set; }

        [JsonProperty("billable")]
        public double billable { get; set; }

        [JsonProperty("room")]
        public string room { get; set; }

        [JsonProperty("heroProgram")]
        public IList<string> heroProgram { get; set; }

        [JsonProperty("shortStartWorkDate")]
        public string shortStartWorkDate { get; set; }

        [JsonProperty("timeJournal")]
        public bool timeJournal { get; set; }



    }

    
}
