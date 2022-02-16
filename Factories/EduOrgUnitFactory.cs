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
using FINT.Model.Utdanning.Basisklasser;
using FINT.Model.Administrasjon.Personal;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Utdanning.Utdanningsprogram;
using FINT.Model.Resource;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class EduOrgUnitFactory
    {
        public static EduOrgUnit Create(string systemIdUri, Skole skole, string skoleeier)
        {
            var systemId = skole.SystemId.Identifikatorverdi;
            var organisasjonsnummer = skole?.Organisasjonsnummer.Identifikatorverdi;
            var skolenummer = skole?.Skolenummer.Identifikatorverdi;
            var navn = skole?.Navn;
            var domenenavn = skole?.Domenenavn;
            var juridiskNavn = skole?.JuridiskNavn;
            var organisasjonsnavn = skole?.Organisasjonsnavn;
            var adresselinjer = skole?.Forretningsadresse?.Adresselinje;
            var postnummer = skole?.Forretningsadresse.Postnummer;
            var poststed = skole?.Forretningsadresse.Poststed;
            var epostadresse = skole?.Kontaktinformasjon?.Epostadresse;
            var telefonnummer = skole?.Kontaktinformasjon?.Telefonnummer;
            var nettsted = skole?.Kontaktinformasjon?.Nettsted;
            var mobilnummer = skole?.Kontaktinformasjon?.Mobiltelefonnummer;
            var sip = skole?.Kontaktinformasjon?.Sip;

            var elevforhold = new List<string>();
            var undervisningsforhold = new List<string>();
            var ansettelsesforhold = new List<string>();
            


            return new EduOrgUnit
            {
                SystemIdUri = systemIdUri,
                SkoleSystemId = systemId,
                SkoleOrganisasjonsnummer = organisasjonsnummer,
                SkoleSkolenummer = skolenummer,
                SkoleNavn = navn,
                SkoleDomenenavn = domenenavn,
                SkoleJuridiskNavn = juridiskNavn,
                SkoleOrganisasjonsnavn = organisasjonsnavn,
                SkoleForretningsadresseAdresselinje = adresselinjer,
                SkoleForretningsadressePostnummer = postnummer,
                SkoleForretningsadressePoststed = poststed,
                SkoleKontaktinformasjonEpostadresse = epostadresse,
                SkoleKontaktinformasjonTelefonnummer = telefonnummer,
                SkoleKontaktinformasjonMobiltelefonnummer = mobilnummer,
                SkoleKontaktinformasjonNettsted = nettsted,
                SkoleKontaktinformasjonSip = sip,     
                SkoleElevforhold = elevforhold,
                SkoleUndervisningsforhold = undervisningsforhold,
                SkoleAnsettelsesforhold = ansettelsesforhold,
                Skoleeier = skoleeier,
            };
        }
        public static EduOrgUnit Create(string systemIdUri, SkoleResource skoleResource, string skoleeier)
        {
            var systemId = skoleResource.SystemId.Identifikatorverdi;
            var organisasjonsnummer = skoleResource?.Organisasjonsnummer.Identifikatorverdi;
            var skolenummer = skoleResource?.Skolenummer.Identifikatorverdi;
            var navn = skoleResource?.Navn;
            var domenenavn = skoleResource?.Domenenavn;
            var juridiskNavn = skoleResource?.JuridiskNavn;
            var organisasjonsnavn = skoleResource?.Organisasjonsnavn;
            var adresselinjer = skoleResource?.Forretningsadresse?.Adresselinje;
            var postnummer = skoleResource?.Forretningsadresse?.Postnummer;
            var poststed = skoleResource?.Forretningsadresse?.Poststed;
            var epostadresse = skoleResource?.Kontaktinformasjon?.Epostadresse;
            var telefonnummer = skoleResource?.Kontaktinformasjon?.Telefonnummer;
            var nettsted = skoleResource?.Kontaktinformasjon?.Nettsted;
            var mobilnummer = skoleResource?.Kontaktinformasjon?.Mobiltelefonnummer;
            var sip = skoleResource?.Kontaktinformasjon?.Sip;

            var elevforhold = new List<string>();
            var undervisningsforhold = new List<string>();
            var ansettelsesforhold = new List<string>();


            return new EduOrgUnit
            {
                SystemIdUri = systemIdUri,
                SkoleSystemId = systemId,
                SkoleOrganisasjonsnummer = organisasjonsnummer,
                SkoleSkolenummer = skolenummer,
                SkoleNavn = navn,
                SkoleDomenenavn = domenenavn,
                SkoleJuridiskNavn = juridiskNavn,
                SkoleOrganisasjonsnavn = organisasjonsnavn,
                SkoleForretningsadresseAdresselinje = adresselinjer,
                SkoleForretningsadressePostnummer = postnummer,
                SkoleForretningsadressePoststed = poststed,
                SkoleKontaktinformasjonEpostadresse = epostadresse,
                SkoleKontaktinformasjonTelefonnummer = telefonnummer,
                SkoleKontaktinformasjonMobiltelefonnummer = mobilnummer,
                SkoleKontaktinformasjonNettsted = nettsted,
                SkoleKontaktinformasjonSip = sip,
                SkoleElevforhold = elevforhold,
                SkoleUndervisningsforhold = undervisningsforhold,
                SkoleAnsettelsesforhold = ansettelsesforhold,
                Skoleeier = skoleeier,
            };
        }
    }
}

