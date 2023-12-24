using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHTTPServer.attributes;

namespace MyHTTPServer.models
{
    public class Form
    {
        public string City { get; set; }
        public string WorkAdress { get; set; }
        public string Profession { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Birthday { get; set; }
        public string Phone { get; set; }
        public string NWLink { get; set; }

        public Form(string City, string WorkAdress, string Profession, string Name, string Surname, string Birthday, string Phone, string NWLink)
        {
            this.City = City;
            this.WorkAdress = WorkAdress;
            this.Profession = Profession;
            this.Name = Name;
            this.Surname = Surname;
            this.Birthday = Birthday;
            this.Phone = Phone;
            this.NWLink = NWLink;
        }
        public override string ToString()
        {
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(City); sb.Append(" ");
        //    sb.Append(WorkAdress); sb.Append(" ");
        //    sb.Append(Profession); sb.Append(" ");
        //    sb.Append(Name); sb.Append(" ");
        //    sb.Append(Surname); sb.Append(" ");
        //    sb.Append(Birthday); sb.Append(" ");
        //    sb.Append(Phone); sb.Append(" ");
        //    sb.Append(NWLink);
            return $"City= {City} \n Address= {WorkAdress} \n Profession= {Profession} \n Name= {Name} \n Surname= {Surname} \n Birth Date = {Birthday} \n Phone Number = {Phone} \n Email ={NWLink}";//sb.ToString();
        }
    }
}
