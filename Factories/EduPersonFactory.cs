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
using FINT.Model.Felles;
using FINT.Model.Utdanning.Elev;
using FINT.Model.Administrasjon.Personal;
using FINT.Model.Felles.Kompleksedatatyper;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class EduPersonFactory
    {
        //public static EduPerson Create (Person person, Elev student, string groupUri)
        public static EduPerson Create(string systemIdUri, Person person, Elev student, string orgId)
        {
            var elevnummer = student.Elevnummer.Identifikatorverdi;
            var brukernavn = student?.Brukernavn?.Identifikatorverdi;
            var brukernavnGyldighetsperiodeStart = student?.Brukernavn?.Gyldighetsperiode?.Start.ToString(dateFormat);
            var brukernavnGyldighetsperiodeSlutt = student?.Brukernavn?.Gyldighetsperiode?.Slutt?.ToString(dateFormat);
            var feidenavn = student?.Feidenavn?.Identifikatorverdi;
            var systemId = student.SystemId.Identifikatorverdi;
            var epostadresse = student?.Kontaktinformasjon?.Epostadresse;
            var mobilnummer = student?.Kontaktinformasjon?.Mobiltelefonnummer;

            var bilde = person?.Bilde;
            var fodselsdato = person?.Fodselsdato;
            var fodselsnummer = person?.Fodselsnummer?.Identifikatorverdi;
            var fornavn = person?.Navn?.Fornavn;
            var mellomnavn = person?.Navn?.Mellomnavn;
            var etternavn = person?.Navn?.Etternavn;
            var privatEpostadresse = person?.Kontaktinformasjon?.Epostadresse;
            var privatMobilnummer = person?.Kontaktinformasjon?.Mobiltelefonnummer;

            var bostedsadresseAdresselinje = person?.Bostedsadresse?.Adresselinje;
            var bostedsadressePostnummer = person?.Bostedsadresse?.Postnummer;
            var bostedsadressePoststed = person?.Bostedsadresse?.Poststed;

            var postadresseAdresselinje = person?.Postadresse?.Adresselinje;
            var postadressePostnummer = person?.Postadresse?.Postnummer;
            var postadressePoststed = person?.Postadresse?.Poststed;

            var eduPersonOrgId = orgId;

            var basisgruppemedlemskap = new List<string>();
            var kontaktlarergruppemedlemskap = new List<string>();
            var undervisningsgruppemedlemskap = new List<string>();

            var elevforholdSkole = new List<string>();
            var elevforholdKategori = new List<string>();
            var undervisningsforholdSkole = new List<string>();
            var undervisningsforholdMedlemskap = new List<string>();
            var ansettelsesforholdSkole = new List<string>();

            var eduPersonEntitlement = new List<string>();
            var eduPersonPrimaryOrgUnitDN = string.Empty;
            var eduPersonOrgUnitDN = new List<string>();
            var rolleOgSkole = new List<string>();
            var kategoriOgSkole = new List<string>();



            return new EduPerson
            {
                ElevPersonalSystemId = systemIdUri,
                ElevSystemIdUri = systemIdUri,

                PersonBilde = bilde,
                PersonFodselsdato = fodselsdato?.ToString(dateFormat),
                PersonFodselsnummer = fodselsnummer,
                PersonNavnFornavn = fornavn,
                PersonNavnMellomnavn = mellomnavn,
                PersonBostedsadresseAdresselinje = bostedsadresseAdresselinje,
                PersonBostedsadressePostnummer = bostedsadressePostnummer,
                PersonBostedsadressePoststed = bostedsadressePoststed,
                PersonPostadresseAdresselinje = postadresseAdresselinje,
                PersonPostadressePostnummer = postadressePostnummer,
                PersonPostadressePoststed = postadressePoststed,
                PersonNavnEtternavn = etternavn,
                PersonKontaktinfomasjonEpostadresse = privatEpostadresse,
                PersonKontaktinfomasjonMobiltelefonnummer = privatMobilnummer,

                ElevElevnummer = elevnummer,
                ElevBrukernavn = brukernavn,
                ElevFeidenavn = feidenavn,
                ElevSystemId = systemId,
                ElevKontaktinformasjonEpostadresse = epostadresse,
                ElevKontaktinformasjonMobiltelfonnummer = mobilnummer,

                ElevforholdBasisgruppe = basisgruppemedlemskap,
                ElevforholdKontaktlarergruppe = kontaktlarergruppemedlemskap,
                ElevforholdUndervisningsgruppe = undervisningsgruppemedlemskap,
                ElevforholdSkole = elevforholdSkole,
                ElevforholdKategori = elevforholdKategori,
                UndervisningsforholdSkole = undervisningsforholdSkole,
                UndervisningsforholdMedlemskap = undervisningsforholdMedlemskap,
                AnsettelsesforholdSkole = ansettelsesforholdSkole,

                EduPersonEntitlement = eduPersonEntitlement,
                EduPersonOrgDN = eduPersonOrgId,
                EduPersonPrimaryOrgUnitDN = eduPersonPrimaryOrgUnitDN,
                EduPersonOrgUnitDN = eduPersonOrgUnitDN,
                RolleOgSkole = rolleOgSkole,
                ElevkategoriOgSkole = kategoriOgSkole
            };
        }
        public static EduPerson Create(string systemIdUri, Person person, Skoleressurs skoleressurs, Personalressurs personalressurs, string orgId)
        {
            var bilde = person?.Bilde;
            var fodselsdato = person?.Fodselsdato;
            var fodselsnummer = person.Fodselsnummer.Identifikatorverdi;
            var fornavn = person.Navn.Fornavn;
            var mellomnavn = person.Navn?.Mellomnavn;
            var etternavn = person.Navn.Etternavn;
            var bostedsadresseAdresselinje = person?.Bostedsadresse?.Adresselinje;
            var bostedsadressePostnummer = person?.Bostedsadresse?.Postnummer;
            var bostedsadressePoststed = person?.Bostedsadresse?.Poststed;
            var privatEpostadresse = person?.Kontaktinformasjon?.Epostadresse;
            var privatMobilnummer = person?.Kontaktinformasjon?.Mobiltelefonnummer;
            var postadresseAdresselinje = person?.Postadresse?.Adresselinje;
            var postadressePostnummer = person?.Postadresse?.Postnummer;
            var postadressePoststed = person?.Postadresse?.Poststed;
            
            var ansattnummer = personalressurs.Ansattnummer.Identifikatorverdi;
            var systemId = personalressurs.SystemId.Identifikatorverdi;
            var brukernavn = personalressurs?.Brukernavn.Identifikatorverdi;
            var personalEpostadresse = personalressurs?.Kontaktinformasjon?.Epostadresse;
            var personalMobiltelefonnummer = personalressurs?.Kontaktinformasjon?.Mobiltelefonnummer;
            var personalTelefonnummer = personalressurs?.Kontaktinformasjon?.Telefonnummer;
            var sip = personalressurs?.Kontaktinformasjon?.Sip;

            var feidenavn = skoleressurs?.Feidenavn?.Identifikatorverdi;

            var eduPersonOrgId = orgId;

            var basisgruppemedlemskap = new List<string>();
            var kontaktlarergruppemedlemskap = new List<string>();
            var undervisningsgruppemedlemskap = new List<string>();       

            var elevforholdSkole = new List<string>();
            var elevforholdKategori = new List<string>();

            var undervisningsforholdSkole = new List<string>();
            var ansettelsesforholdSkole = new List<string>();
            var undervisningsforholdMedlemskap = new List<string>();

            var eduPersonEntitlement = new List<string>();
            var eduPersonOrgUnitDN = new List<string>();
            var rolleOgSkole = new List<string>();
            var elevkategoriOgSkole = new List<string>();

            return new EduPerson
            {
                ElevPersonalSystemId = systemIdUri,
                SkoleressursSystemIdUri = systemIdUri,

                PersonFodselsdato = fodselsdato?.ToString(dateFormat),
                PersonFodselsnummer = fodselsnummer,
                PersonNavnFornavn = fornavn,
                PersonNavnMellomnavn = mellomnavn,
                PersonBostedsadresseAdresselinje = bostedsadresseAdresselinje,
                PersonBostedsadressePostnummer = bostedsadressePostnummer,
                PersonBostedsadressePoststed = bostedsadressePoststed,
                PersonPostadresseAdresselinje = postadresseAdresselinje,
                PersonPostadressePostnummer = postadressePostnummer,
                PersonPostadressePoststed = postadressePoststed,
                PersonNavnEtternavn = etternavn,
                PersonKontaktinfomasjonMobiltelefonnummer = privatMobilnummer,
                PersonKontaktinfomasjonEpostadresse = privatEpostadresse,

                PersonalAnsattnummer = ansattnummer,
                PersonalSystemId = systemId,
                PersonalBrukernavn = brukernavn,
                PersonalKontaktinformasjonEpostadresse = personalEpostadresse,
                PersonalKontaktinformasjonMobiltelefonnummer = personalMobiltelefonnummer,
                PersonalKontaktinformasjonSip = sip,

                SkoleressursFeidenavn = feidenavn,
                ElevforholdBasisgruppe = basisgruppemedlemskap,
                ElevforholdKontaktlarergruppe = kontaktlarergruppemedlemskap,
                ElevforholdUndervisningsgruppe = undervisningsgruppemedlemskap,
                ElevforholdSkole = elevforholdSkole,
                ElevforholdKategori = elevforholdKategori,
                UndervisningsforholdSkole = undervisningsforholdSkole,
                AnsettelsesforholdSkole = ansettelsesforholdSkole,
                UndervisningsforholdMedlemskap = undervisningsforholdMedlemskap,

                EduPersonEntitlement = eduPersonEntitlement,
                EduPersonOrgDN = eduPersonOrgId,
                EduPersonOrgUnitDN = eduPersonOrgUnitDN,
                RolleOgSkole = rolleOgSkole,
                ElevkategoriOgSkole = elevkategoriOgSkole
            };
        }
    }
}
