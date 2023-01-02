using System;

namespace PiHealth.Web.Model
{
    public class SignupModel
    {
        public string initial { get; set; }
        public string name { get; set; }
        public float? age { get; set; }
        public string gender { get; set; }
        public string address { get; set; }
        public string mobileNumber { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public Boolean isAgreed { get; set; }
    }
}
