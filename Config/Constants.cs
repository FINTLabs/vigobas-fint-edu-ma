// VIGOBAS Identity Management System 
//  Copyright (C) 2022  Vigo IKS 
//  
//  Documentation - visit https://vigobas.vigoiks.no/ 
//  
//  This program is free software: you can redistribute it and/or modify 
//  it under the terms of the GNU Affero General Public License as 
//  published by the Free Software Foundation, either version 3 of the 
//  License, or (at your option) any later version. 
//  
//  This program is distributed in the hope that it will be useful, 
//  but WITHOUT ANY WARRANTY, without even the implied warranty of 
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
//  GNU Affero General Public License for more details. 
//  
//  You should have received a copy of the GNU Affero General Public License 
//  along with this program.  If not, see https://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VigoBAS.FINT.Edu
{
    class Constants
    {
        public const string dateFormat = "yyyy-MM-dd";
        public const string hourFormat = "HH\\:mm\\:ss";
        public const string zeroDate = "1900-01-01";
        public const string infinityDate = "2099-01-01";
        public const int initialDelayMilliseconds = 500; 
        public const double factor = 1.5;
        public const int retrylimit = 10;

        public struct Delimiter
        {
            public const char listDelimiter = ';';
            public const char path = '/';
            public const char roleSchool = '@';
            public const char categorySchool = '@';
            public const char levelAtSchool = '@';
        }
        

        public struct Param
        {
            public const string clientId = "ClientId";
            public const string openIdSecret = "OpenIdSecret";
            public const string username = "Username";
            public const string password = "Password";
            public const string assetId = "AssetId";
            //public const string xClient = "xClient";
            public const string scope = "Scope";
            public const string idpUri = "IdpUri";
            public const string felleskomponentUri = "Uri felleskomponent";
            public const string httpClientTimeout = "Http Client Timeout (minutter)";
            public const string organisasjonsnummer = "Organisasjonsnummer";
            public const string organisasjonsIdTopOrgElement = "OrganisasjonsId som representerer organisasjon";
            public const string schoolnumbersToImport = "Skoler som skal importeres";

            public const string useLocalCache = "Importer fra lokal cache";
            public const string abortIfResourceTypeEmpty = "Stopp import hvis en ressurstype er tom";
            public const string abortIfDownloadError = "Stopp import ved nedlastingsfeil";
            public const string importStudentsWithoutStudyRelationShip = "Importer elever uten skoletilknytning";

            public const string useGroupMembershipResources = "Bruk gruppemedlemskapsressurser";

            public const string generateAggrStudentGroups = "Generer grupper for alle elever";
            public const string generateAggrFacultyGroups = "Generer grupper for alle lærere";
            public const string generateAggrEmployeeGroups = "Generer grupper for alle ansatte";
            public const string generateProgAreaGroups = "Generer programområdegrupper";

            public const string examCategoriesToImport = "Eksamensformer til import";
            public const string examCategoriesToAggregatePerDate = "Samlegrupper per dato for eksamensformer";
            public const string examgroupsVisibleFromDate = "Eksamensgrupper synlig fra dato (yyyy-MM-dd)";
            //public const string examPeriodStartDate = "Eksamensperiode startdato (yyyy-MM-dd)";
            public const string examgroupsVisibleToDate = "Eksamensgrupper synlig til dato (yyyy-MM-dd)";

            public const string daysBeforeStudentStarts = "Antall dager før aktivt elevforhold";
            public const string daysBeforeStudentEnds = "Antall dager etter aktivt elevforhold";
            public const string importPurePrivateStudents = "Importer rene privatister";

            public const string daysBeforeGroupStarts = "Antall dager før gruppen starter";
            public const string daysBeforeGroupEnds = "Antall dager etter gruppen slutter";
            public const string daysBeforeEmploymentStarts = "Antall dager før aktivt arbeidsforhold";
            public const string daysBeforeEmploymentEnds = "Antall dager etter aktivt arbeidsforhold";
            public const string excludedResourceTypes = "Ignorer ressurskategorier";
            public const string excludedEmploymentTypes = "Ignorer arbeidsforholdtyper";
            public const string excludedPositionCodes = "Ignorer stillingskoder";
            public const string waitTime = "Ventetid oppdateringskall (millisekunder)";
            public const string lowerLimit = "Nedre grense statuskø";
            public const string upperLimit = "Øvre grense statuskø";
        }

        public struct DefaultValue
        {
            public const string xClient = "test";
            public const string scope = "fint-client";
            public const string httpClientTimeout = "2";
            public const string felleskomponentUri = "https://play-with-fint.felleskomponent.no";
            public const string accessTokenUri = "https://namidp01.rogfk.no/nidp/oauth/nam/token";
            public const string utdanningElevPersonUri = "/utdanning/elev/person";
            public const string utdanningElevElevUri = "/utdanning/elev/elev";
            public const string utdanningElevElevforholdUri = "/utdanning/elev/elevforhold";
            public const string utdanningElevUndervisningsforholdUri = "/utdanning/elev/undervisningsforhold";
            public const string utdanningElevMedlemskapUri = "/utdanning/elev/medlemskap";
            public const string utdanningElevBasisgruppeUri = "/utdanning/elev/basisgruppe";
            public const string utdanningElevBasisgruppeMedlemskapUri = "/utdanning/elev/basisgruppemedlemskap";
            public const string utdanningElevKontaktlarergruppeUri = "/utdanning/elev/kontaktlarergruppe";
            public const string utdanningElevKontaktlarergruppeMedlemskapUri = "/utdanning/elev/kontaktlarergruppemedlemskap";
            public const string utdanningVurderingEksamensgruppeUri = "/utdanning/vurdering/eksamensgruppe";
            public const string utdanningElevSkoleressursUri = "/utdanning/elev/skoleressurs";
            public const string utdanningTimeplanUndervisningsgruppeUri = "/utdanning/timeplan/undervisningsgruppe";
            public const string utdanningTimeplanUndervisningsgruppeMedlemskapUri = "/utdanning/timeplan/undervisningsgruppemedlemskap";
            public const string utdanningTimeplanFagUri = "/utdanning/timeplan/fag";
            public const string utdanningUtdanningsprogramArstrinnUri = "/utdanning/utdanningsprogram/arstrinn";
            public const string utdanningUtdanningsprogramProgramomradeUri = "/utdanning/utdanningsprogram/programomrade";
            public const string utdanningUtdanningsprogramUtdanningsprogramUri = "/utdanning/utdanningsprogram/utdanningsprogram";
            public const string utdanningUtdanningsprogramSkoleUri = "/utdanning/utdanningsprogram/skole";
            public const string administrasjonPersonalPersonUri = "/administrasjon/personal/person";
            public const string administrasjonPersonalPersonalRessursUri = "/administrasjon/personal/personalressurs";
            public const string administrasjonPersonalArbeidsforholdUri = "/administrasjon/personal/arbeidsforhold";
            public const string administrasjonOrganisasjonOrganisasjonselementUri = "/administrasjon/organisasjon/organisasjonselement";
            public const string listValues = "Adskilt med ,";
        }

        public struct HalObject
        {
            public const string _links = "_links";
            public const string _embedded = "_embedded";
            public const string _entries = "_entries";
        }

        public struct StateLink
        {
            public const string total_items = "total_items";
        }

        public struct HttpHeader
        {
            public const string X_Org_Id = "X-Org-Id";
            public const string X_Client = "X-Client";
            public const string Location = "Location";
        }

        public struct HttpVerb
        {
            public const string GET = "GET";
            public const string PUT = "PUT";
            public const string POST = "POST";
        }

        public struct GroupType
        {
            public const string aggrEmp = "aggr.emp";
            public const string aggrFac = "aggr.fac";
            public const string aggrSta = "aggr.sta";         
            public const string aggrAll = "aggr.all";
            public const string aggrStu = "aggr.stu";
        }
        public struct ResourceLink
        {
            public const string person = "person";
            public const string student = "elev";
            public const string member = "medlem";
            public const string group = "gruppe";
            public const string basicGroup = "basisgruppe";
            public const string contactTeacherGroup = "kontaktlarergruppe";
            public const string studyGroup = "undervisningsgruppe";
            public const string examGroup = "eksamensgruppe";
            public const string groupMembership = "gruppemedlemskap";
            public const string subject = "fag";
            public const string level = "trinn";
            public const string programmearea = "programomrade";
            public const string studyprogramme = "utdanningsprogram";
            public const string personalResource = "personalressurs";
            public const string studentRelationship = "elevforhold";
            public const string studentCategory = "kategori";
            public const string teachingRelationship = "undervisningsforhold";
            public const string employment = "arbeidsforhold";
            public const string self = "self";
            public const string school = "skole";
            public const string schoolresource = "skoleressurs";
            public const string organization = "organisasjon";
            public const string grepreference = "grepreferanse";
            public const string parent = "overordnet";
            public const string workplace = "arbeidssted";
            public const string resourceCategory = "personalressurskategori";
            public const string employmentType = "arbeidsforholdstype";
            public const string positionCode = "stillingskode";
            public const string leader = "leder";
            public const string eksamensform = "eksamensform";
        }

        public struct ClassType
        {
            public const string basicGroup = "basisgruppe";
            public const string contactTeacherGroup = "kontaktlarergruppe";
            public const string studyGroup = "undervisningsgruppe";
            
            public const string examGroup = "eksamensgruppe";
            public const string subject = "fag";
            public const string level = "arstrinn";
            public const string programmeArea = "programomrade";
            public const string educationProgramme = "utdanningsprogram";
            public const string studentRelationship = "elevforhold";
            public const string teachingRelationship = "undervisningsforhold";
            public const string schoolresource = "skoleressurs";
        }

        public struct FintAttribute
        {
            public const string systemId = "systemId";
            public const string navn = "navn";
            public const string beskrivelse = "beskrivelse";
            public const string periode = "periode";
            public const string elevnummer = "elevnummer";
            public const string brukernavn = "brukernavn";
            public const string feidenavn = "feidenavn";
            public const string hovedskole = "hovedskole";
            public const string kontaktinformasjon = "kontaktinformasjon";
            public const string postadresse = "postadresse";
            public const string bostedsadresse = "bostedsadresse";
            public const string fodselsnummer = "fodselsnummer";
            public const string fodselsdato = "fodselsdato";
            public const string ansattnummer = "ansattnummer";
            public const string skolenummer = "skolenummer";
            public const string organisasjonsId = "organisasjonsId";
            public const string organisasjonsKode = "organisasjonsKode";
            public const string organisasjonsnummer = "organisasjonsnummer";
            public const string kortnavn = "kortnavn";
            public const string organisasjonsnavn = "organisasjonsnavn";
            public const string domenenavn = "domenenavn";
            public const string juridiskNavn = "juridiskNavn";
            public const string forretningsadresse = "forretningsadresse";
            public const string gyldighetsperiode = "gyldighetsperiode";
            public const string hovedstilling = "hovedstilling";
        }

        public struct CSObjectType
        {
            public const string eduPerson = "eduPerson";
            public const string eduGroup = "eduGroup";
            public const string eduOrgUnit = "eduOrgUnit";
            public const string eduOrgUnitGroup = "eduOrgUnitGroup";
            public const string eduOrg = "eduOrg";
            public const string eduSubject = "eduSubject";
            public const string eduStudentRelationship = "eduStudentRelationship";
        }

        public struct CSAttribute
        {
            public const string systemIdUri = "systemIdUri";
            public const string ElevPersonalSystemId = "ElevPersonalSystemId";
            public const string ElevSystemId = "ElevSystemId";
            public const string ElevSystemIdUri = "ElevSystemIdUri";
            public const string ElevElevnummer = "ElevElevnummer";
            public const string ElevBrukernavn = "ElevBrukernavn";
            public const string ElevFeidenavn = "ElevFeidenavn";
            public const string ElevKontaktinformasjonEpostadresse = "ElevKontaktinformasjonEpostadresse";
            public const string ElevKontaktinformasjonMobiltelefonnummer = "ElevKontaktinformasjonMobiltelefonnummer";
            public const string PersonBilde = "PersonBilde";
            public const string PersonFodselsdato = "PersonFodselsdato";
            public const string PersonFodselsnummer = "PersonFodselsnummer";
            public const string PersonNavnFornavn = "PersonNavnFornavn";
            public const string PersonNavnMellomnavn = "PersonNavnMellomnavn";
            public const string PersonNavnEtternavn = "PersonNavnEtternavn";
            public const string PersonKontaktinformasjonMobiltelefonnummer = "PersonKontaktinformasjonMobiltelefonnummer";
            public const string PersonKontaktinformasjonEpostadresse = "PersonKontaktinformasjonEpostadresse";
            public const string PersonKontaktinformasjonBostedsadresseAdresselinje = "PersonKontaktinformasjonBostedsadresseAdresselinje";
            public const string PersonKontaktinformasjonBostedsadressePostnummer = "PersonKontaktinformasjonBostedsadressePostnummer";
            public const string PersonKontaktinformasjonBostedsadressePoststed = "PersonKontaktinformasjonBostedsadressePoststed";
            public const string PersonKontaktinformasjonPostadresseAdresselinje = "PersonKontaktinformasjonPostadresseAdresselinje";
            public const string PersonKontaktinformasjonPostadressePostnummer = "PersonKontaktinformasjonPostadressePostnummer";
            public const string PersonKontaktinformasjonPostadressePoststed = "PersonKontaktinformasjonPostadressePoststed";
            public const string ElevforholdMedlemskap = "ElevforholdMedlemskap";
            public const string ElevforholdBasisgruppe = "ElevforholdBasisgruppe";
            public const string ElevforholdKontaktlarergruppe = "ElevforholdKontaktlarergruppe";
            public const string ElevforholdUndervisningsgruppe = "ElevforholdUndervisningsgruppe";
            public const string ElevforholdSkole = "ElevforholdSkole";
            public const string ElevforholdKategori = "ElevforholdKategori";
            public const string ElevforholdHovedkategori = "ElevforholdHovedkategori";
            public const string ElevforholdProgramomrade = "ElevforholdProgramomrade";
            public const string SkoleressursFeidenavn = "SkoleressursFeidenavn";
            public const string SkoleressursSystemIdUri = "SkoleressursSystemIdUri";
            public const string PersonalBrukernavn = "PersonalBrukernavn";
            public const string PersonalAnsattnummer = "PersonalAnsattnummer";
            public const string PersonalSystemId = "PersonalSystemId";
            public const string PersonalKontaktinformasjonEpostadresse = "PersonalKontaktinformasjonEpostadresse";
            public const string PersonalKontaktinformasjonMobiltelefonnummer = "PersonalKontaktinformasjonMobiltelefonnummer";
            public const string PersonalKontaktinformasjonSip = "PersonalKontaktinformasjonSip";
            public const string UndervisningsforholdSkole = "UndervisningsforholdSkole";
            public const string UndervisningsforholdMedlemskap = "UndervisningsforholdMedlemskap";
            public const string AnsettelsesforholdSkole = "AnsettelsesforholdSkole";
            public const string RolleOgSkole = "RolleOgSkole";
            public const string ElevkategoriOgSkole = "ElevkategoriOgSkole";
            public const string Eksamensdatoer = "Eksamensdatoer";
            public const string AntallEksamener = "AntallEksamener";

            public const string EduPersonEntitlement = "EduPersonEntitlement";
            public const string EduPersonOrgDN = "EduPersonOrgDN";
            public const string EduPersonPrimaryOrgUnitDN = "EduPersonPrimaryOrgUnitDN";
            public const string EduPersonOrgUnitDN = "EduPersonOrgUnitDN";

            public const string GruppeSystemIdUri = "GruppeSystemIdUri";
            public const string GruppeSystemId = "GruppeSystemId";
            public const string GruppeNavn = "GruppeNavn";
            public const string EduGroupType = "EduGroupType";
            public const string EduGroupExamCategory = "EduGroupExamCategory";
            public const string GruppeBeskrivelse = "GruppeBeskrivelse";
            public const string GruppePeriodeStart = "GruppePeriodeStart";
            public const string GruppePeriodeSlutt = "GruppePeriodeSlutt";
            public const string GruppePeriodeStartTime = "GruppePeriodeStartTime";
            public const string GruppePeriodeSluttTime = "GruppePeriodeSluttTime";
            public const string GruppeSkoleRef = "GruppeSkoleRef";
            public const string GruppeSkoleSkolenummer = "GruppeSkoleSkolenummer";
            public const string GruppeElevListe = "GruppeElevListe";
            public const string GruppeElevAntall = "GruppeElevAntall";
            public const string GruppeLarerListe = "GruppeLarerListe";
            public const string GruppeLarerOgElevListe = "GruppeLarerOgElevListe";
            public const string GruppeGruppeListe = "GruppeBasisgruppeListe";
            public const string GruppeFagRef = "GruppeFagRef";

            public const string SkoleSystemId = "SkoleSystemId";
            public const string SkoleOrganisasjonsnummer = "SkoleOrganisasjonsnummer";
            public const string SkoleSkolenummer = "SkoleSkolenummer";
            public const string SkoleNavn = "SkoleNavn";
            public const string SkoleDomenenavn = "SkoleDomenenavn";
            public const string SkoleJuridiskNavn = "SkoleJuridiskNavn";
            public const string SkoleOrganisasjonsnavn = "SkoleOrganisasjonsnavn";
            public const string SkoleForretningsadresseAdresselinje = "SkoleForretningsadresseAdresselinje";
            public const string SkoleForretningsadressePostnummer = "SkoleForretningsadressePostnummer";
            public const string SkoleForretningsadressePoststed = "SkoleForretningsadressePoststed";
            public const string SkoleKontaktinformasjonEpostadresse = "SkoleKontaktinformasjonEpostadresse";
            public const string SkoleKontaktinformasjonTelefonnummer = "SkoleKontaktinformasjonTelefonnummer";
            public const string SkoleKontaktinformasjonMobiltelefonnummer = "SkoleKontaktinformasjonMobiltelefonnummer";
            public const string SkoleKontaktinformasjonNettsted = "SkoleKontaktinformasjonNettsted";
            public const string SkoleKontaktinformasjonSip = "SkoleKontaktinformasjonSip";
            public const string SkoleElevforhold = "SkoleElevforhold";
            public const string SkoleUndervisningsforhold = "SkoleUndervisningsforhold";
            public const string SkoleAnsettelsesforhold = "SkoleAnsettelsesforhold";
            public const string Skoleeier = "Skoleeier";
            public const string SkoleOrganisasjonselement = "SkoleOrganisasjonselement";

            public const string OrganisasjonOrganisasjonsIdUri = "OrganisasjonOrganisasjonsIdUri";
            public const string OrganisasjonOrganisasjonsId = "OrganisasjonOrganisasjonsId";
            public const string OrganisasjonOrganisasjonsnummer = "OrganisasjonOrganisasjonsnummer";
            public const string OrganisasjonOrganisasjonnummer = "OrganisasjonOrganisasjonnummer";
            public const string OrganisasjonNavn = "OrganisasjonNavn";
            public const string OrganisasjonDomenenavn = "OrganisasjonDomenenavn";
            public const string OrganisasjonJuridiskNavn = "OrganisasjonJuridiskNavn";
            public const string OrganisasjonOrganisasjonsnavn = "OrganisasjonOrganisasjonsnavn";
            public const string OrganisasjonForretningsadresseAdresselinje = "OrganisasjonForretningsadresseAdresselinje";
            public const string OrganisasjonForretningsadressePostnummer = "OrganisasjonForretningsadressePostnummer";
            public const string OrganisasjonForretningsadressePoststed = "OrganisasjonForretningsadressePoststed";
            public const string OrganisasjonKontaktinformasjonEpostadresse = "OrganisasjonKontaktinformasjonEpostadresse";
            public const string OrganisasjonKontaktinformasjonTelefonnummer = "OrganisasjonKontaktinformasjonTelefonnummer";
            public const string OrganisasjonKontaktinformasjonMobiltelefonnummer = "OrganisasjonKontaktinformasjonMobiltelefonnummer";
            public const string OrganisasjonKontaktinformasjonNettsted = "OrganisasjonKontaktinformasjonNettsted";
            public const string OrganisasjonKontaktinformasjonSip = "OrganisasjonKontaktinformasjonSip";

            public const string FagSystemIdUri = "FagSystemIdUri";
            public const string FagSystemId = "FagSystemId";
            public const string FagNavn = "FagNavn";
            public const string FagBeskrivelse = "FagBeskrivelse";

            public const string GroupId = "GroupId";
            public const string GroupName = "GroupName";
            public const string GroupType = "GroupType";
            public const string SchoolRef = "SchoolRef";
            public const string GroupMembers = "GroupMembers";

            public const string ElevforholdSystemIdUri = "ElevforholdSystemIdUri";
            public const string ElevforholdSystemId = "ElevforholdSystemId";
            public const string ElevforholdGyldighetsperiodeStart = "ElevforholdGyldighetsperiodeStart";
            public const string ElevforholdGyldighetsperiodeSlutt = "ElevforholdGyldighetsperiodeSlutt";
            public const string ElevforholdHovedskole = "ElevforholdHovedskole";
            public const string ElevforholdElevkategori = "ElevforholdElevkategori";
            public const string ElevforholdElevRef = "ElevforholdElevRef";
            public const string ElevforholdSkoleRef = "ElevforholdSkoleRef";
            public const string ElevforholdBasisgruppeRef = "ElevforholdBasisgruppeRef";
        }

        public struct Feide
        {
            public const string RoleStudent = "student";
            public const string RoleFaculty = "faculty";

            public const string PrefixOrgNumber = "NO";

            public const string PrefixUrnGrep = "urn:mace:feide.no:go:grep:";
            //public const string PrefixUrnGrepUuid = "urn:mace:feide.no:go:grep:uuid:";
            public const string PrefixUrnGroup = "urn:mace:feide.no:go:group:";
            public const string PrefixUrnGroupId = "urn:mace:feide.no:go:groupid:";

            public const string Basisgruppekode = "b";
            public const string undervisningsgruppekode = "u";
            public const string annengruppekode = "a";

            public const string delimiter = ":";
        }

        public struct Grep
        {
            public const string baseDataUrl = "https://data.udir.no/kl06/";

            public const string opplaeringsfag = "opplaeringsfag";
            public const string fagkoder = "fagkoder";
            public const string utdanningsprogram = "utdanningsprogram";
            public const string programomraader = "programomraader";
            public const string aarstrinn = "aarstrinn";
        }       
    }
}
