using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blue_Badge_Digital_Service
{
    public class BBDSApplication
    {

        public string applicationId { get; set; }
        public string applicationTypeCode { get; set; }
        public string localAuthorityCode { get; set; }
        public bool paymentTaken { get; set; }
        public DateTime submissionDate { get; set; }
        public string existingBadgeNumber { get; set; }
        public Party party { get; set; }
        public Eligibility eligibility { get; set; }
        public Artifacts artifacts { get; set; }

        public BBDSApplication(JObject jsonJObject)
        {
            applicationId = (string)jsonJObject["applicationId"];
            applicationTypeCode = (string) jsonJObject["applicationTypeCode"];
            localAuthorityCode = (string) jsonJObject["localAuthorityCode"];
            paymentTaken = (bool) jsonJObject["paymentTaken"];
           // submissionDate = DateTime.Parse((string)jsonJObject["submissionDate"]);
            existingBadgeNumber = (string) jsonJObject["existingBadgeNumber"];
            party = new Party();
            switch ((string) jsonJObject["party"]["typeCode"])
            {
                case "PERSON":
                    party.person = new Person
                    {
                        badgeHolderName = (string) jsonJObject["party"]["person"]["badgeHolderName"],
                        nino = (string) jsonJObject["party"]["person"]["nino"],
                        dob = (string) jsonJObject["party"]["person"]["dob"],
                        nameAtBirth = (string) jsonJObject["party"]["person"]["nameAtBirth"],
                        genderCode = (string) jsonJObject["party"]["person"]["genderCode"]
                    };
                    break;
            }
            party.contact = new Contact
            {
                fullName = (string)jsonJObject["party"]["contact"]["fullName"],
                buildingStreet = (string)jsonJObject["party"]["contact"]["buildingStreet"],
                line2 = (string)jsonJObject["party"]["contact"]["line2"],
                townCity = (string)jsonJObject["party"]["contact"]["townCity"],
                postCode = (string)jsonJObject["party"]["contact"]["postCode"],
                primaryPhoneNumber = (string)jsonJObject["party"]["contact"]["primaryPhoneNumber"],
                secondaryPhoneNumber = (string)jsonJObject["party"]["contact"]["secondaryPhoneNumber"],
                emailAddress = (string)jsonJObject["party"]["contact"]["emailAddress"],
            };
            eligibility = new Eligibility();
            eligibility.typeCode = (string)jsonJObject["eligibility"]["typeCode"];
            eligibility.descriptionOfConditions = (string)jsonJObject["eligibility"]["descriptionOfConditions"];
            switch (eligibility.typeCode)
            {
                case "PIP":
                case "DLA":
                case "AFRFCS":
                case "WPMS":
                    eligibility.benefit = new Benefit
                    {
                        expiryDate = (string)jsonJObject["eligibility"]["benefit"]["expirydate"],
                        isIndefinite = (bool)jsonJObject["eligibility"]["benefit"]["isIndefinite"]
                    };
                    break;
                case "BLIND":
                    eligibility.blind = new Blind
                    {
                        registeredAtLaId = (string) jsonJObject["eligibility"]["blind"]["registeredAtLaId"]
                    };
                    break;
                case "WALKD":
                    eligibility.walkingDifficulty = new WalkingDifficulty
                    {
                        walkingLengthOfTimeCode =
                            (string) jsonJObject["eligibility"]["walkingDifficulty"]["walkingLengthOfTimeCode"],
                        walkingSpeedCode = (string) jsonJObject["eligibility"]["walkingDifficulty"]["walkingSpeedCode"]
                    };
                    break;
            }
        }

        public class Contact
        {
            public string fullName { get; set; }
            public string buildingStreet { get; set; }
            public string line2 { get; set; }
            public string townCity { get; set; }
            public string postCode { get; set; }
            public string primaryPhoneNumber { get; set; }
            public string secondaryPhoneNumber { get; set; }
            public string emailAddress { get; set; }
        }

        public class Person
        {
            public string badgeHolderName { get; set; }
            public string nino { get; set; }
            public string dob { get; set; }
            public string nameAtBirth { get; set; }
            public string genderCode { get; set; }
        }

        public class Vehicle
        {
            public string registrationNumber { get; set; }
            public string typeCode { get; set; }
            public string usageFrequency { get; set; }
        }

        public class Organisation
        {
            public string badgeHolderName { get; set; }
            public bool isCharity { get; set; }
            public string charityNumber { get; set; }
            public List<Vehicle> vehicles { get; set; }
            public int numberOfBadges { get; set; }
        }

        public class Party
        {
            public string typeCode { get; set; }
            public Contact contact { get; set; }
            public Person person { get; set; }
            public Organisation organisation { get; set; }
        }

        public class Benefit
        {
            public bool isIndefinite { get; set; }
            public string expiryDate { get; set; }
        }

        public class WalkingAid
        {
            public string description { get; set; }
            public string usage { get; set; }
            public string howProvidedCode { get; set; }
        }

        public class Treatment
        {
            public string description { get; set; }
            public string time { get; set; }
        }

        public class Medication
        {
            public string name { get; set; }
            public bool isPrescribed { get; set; }
            public string frequency { get; set; }
            public string quantity { get; set; }
        }

        public class WalkingDifficulty
        {
            public List<string> typeCodes { get; set; }
            public string otherDescription { get; set; }
            public List<WalkingAid> walkingAids { get; set; }
            public string walkingLengthOfTimeCode { get; set; }
            public string walkingSpeedCode { get; set; }
            public List<Treatment> treatments { get; set; }
            public List<Medication> medications { get; set; }
        }

        public class DisabilityArms
        {
            public string drivingFrequency { get; set; }
            public bool isAdaptedVehicle { get; set; }
            public string adaptedVehicleDescription { get; set; }
        }

        public class HealthcareProfessional
        {
            public string name { get; set; }
            public string location { get; set; }
        }

        public class Blind
        {
            public string registeredAtLaId { get; set; }
        }

        public class ChildUnder3
        {
            public string bulkyMedicalEquipmentTypeCode { get; set; }
        }

        public class Eligibility
        {
            public string typeCode { get; set; }
            public string descriptionOfConditions { get; set; }
            public Benefit benefit { get; set; }
            public WalkingDifficulty walkingDifficulty { get; set; }
            public DisabilityArms disabilityArms { get; set; }
            public List<HealthcareProfessional> healthcareProfessionals { get; set; }
            public Blind blind { get; set; }
            public ChildUnder3 childUnder3 { get; set; }
        }

        public class Artifacts
        {
            public string proofOfEligibilityUrl { get; set; }
            public string proofOfAddressUrl { get; set; }
            public string proofOfIdentityUrl { get; set; }
            public string badgePhotoUrl { get; set; }
            public string proofOfEligibility { get; set; }
            public string proofOfAddress { get; set; }
            public string proofOfIdentity { get; set; }
            public string badgePhoto { get; set; }
        }


            



    }

}
