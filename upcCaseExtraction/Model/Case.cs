using System;
namespace upcCaseExtraction
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Content
    {
        public List<Party> parties { get; set; }
        public List<Patent> patents { get; set; }
        public object division { get; set; }
        public object decision { get; set; }
        public object language { get; set; }
        public List<object> judges { get; set; }
        public List<object> spcs { get; set; }
        public string creationDate { get; set; }
        public string fullNumber { get; set; }
        public string number { get; set; }
        public string receiptDate { get; set; }
        public string type { get; set; }
        public int year { get; set; }
    }

    public class Party
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public string type { get; set; }
        public string companyName { get; set; }
        public Representative representative { get; set; }
    }

    public class Patent
    {
        public string number { get; set; }
        public string description { get; set; }
    }

    public class Representative
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
    }

    public class Root
    {
        public List<Content> content { get; set; }
        public int totalResults { get; set; }
    }



}

