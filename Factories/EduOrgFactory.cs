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
using FINT.Model.Resource;
using FINT.Model.Administrasjon.Organisasjon;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class EduOrgFactory
    {
        //public static EduGroup Create (Basisgruppe basicGroup, string memberUri)
        public static EduOrg Create(string organisasjonIdUri, string organisasjonsnummerFromConfig, Organisasjonselement  organisasjon)
        {
            var organisasjonsId = organisasjon.OrganisasjonsId?.Identifikatorverdi;
            var organisasjonsnummer = organisasjon?.Organisasjonsnummer?.Identifikatorverdi;
            var navn = organisasjon?.Navn;
            var organisasjonsnavn = organisasjon?.Organisasjonsnavn;
            var adresselinjer = organisasjon?.Forretningsadresse?.Adresselinje;
            var postnummer = organisasjon?.Forretningsadresse?.Postnummer;
            var poststed = organisasjon?.Forretningsadresse?.Poststed;
            var epostadresse = organisasjon?.Kontaktinformasjon?.Epostadresse;
            var telefonnummer = organisasjon?.Kontaktinformasjon?.Telefonnummer;
            var nettsted = organisasjon?.Kontaktinformasjon?.Nettsted;
            var mobilnummer = organisasjon?.Kontaktinformasjon?.Mobiltelefonnummer;
            var sip = organisasjon?.Kontaktinformasjon?.Sip;

            organisasjonsnummer = organisasjonsnummer ?? organisasjonsnummerFromConfig;

            return new EduOrg
            {
                OrganisasjonOrganisasjonsIdUri = organisasjonIdUri,
                OrganisasjonOrganisasjonsId = organisasjonsId,
                OrganisasjonOrganisasjonsnummer = organisasjonsnummer,
                OrganisasjonNavn = navn,
                OrganisasjonOrganisasjonsnavn = organisasjonsnavn,
                OrganisasjonForretningsadresseAdresselinje = adresselinjer,
                OrganisasjonForretningsadressePostnummer = postnummer,
                OrganisasjonForretningsadressePoststed = poststed,
                OrganisasjonKontaktinformasjonEpostadresse = epostadresse,
                OrganisasjonKontaktinformasjonTelefonnummer = telefonnummer,
                OrganisasjonKontaktinformasjonMobiltelefonnummer = mobilnummer,
                OrganisasjonKontaktinformasjonNettsted = nettsted,
                OrganisasjonKontaktinformasjonSip = sip,
            };
        }
    }
}


