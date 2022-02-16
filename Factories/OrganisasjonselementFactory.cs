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
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Administrasjon.Organisasjon;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class OrganisasjonselementFactory
    {
        public static Organisasjonselement Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var organisasjonsId = new Identifikator();
            var organisasjonsKode = new Identifikator();
            var organisasjonsnummer = new Identifikator();
            var navn = string.Empty;
            var kortnavn = string.Empty;
            var organisasjonsnavn = string.Empty;
            var kontaktinformasjon = new Kontaktinformasjon();

            if (values.TryGetValue(FintAttribute.organisasjonsId, out IStateValue organisasjonsIdVal))
            {
                organisasjonsId = JsonConvert.DeserializeObject<Identifikator>(organisasjonsIdVal.Value);
            }
            if (values.TryGetValue(FintAttribute.organisasjonsKode, out IStateValue organisasjonsKodeValue))
            {
                organisasjonsKode =
                    JsonConvert.DeserializeObject<Identifikator>(organisasjonsKodeValue.Value);
            }
            if (values.TryGetValue(FintAttribute.systemId, out IStateValue organisasjonsnummerValue))
            {
                organisasjonsnummer =
                    JsonConvert.DeserializeObject<Identifikator>(organisasjonsnummerValue.Value);
            }
            if (values.TryGetValue(FintAttribute.navn, out IStateValue navnValue))
            {
                navn = navnValue.Value;
            }
            if (values.TryGetValue(FintAttribute.navn, out IStateValue kortnavnValue))
            {
                kortnavn = kortnavnValue.Value;
            }
            if (values.TryGetValue(FintAttribute.organisasjonsnavn, out IStateValue organisasjonsnavnValue))
            {
                organisasjonsnavn = organisasjonsnavnValue.Value;
            }
            if (values.TryGetValue(FintAttribute.kontaktinformasjon, out IStateValue kontaktinformasjonValue))
            {
                kontaktinformasjon =
                    JsonConvert.DeserializeObject<Kontaktinformasjon>(kontaktinformasjonValue.Value);
            }

            return new Organisasjonselement
            {
                OrganisasjonsId = organisasjonsId,
                OrganisasjonsKode = organisasjonsKode,
                Organisasjonsnummer = organisasjonsnummer,            
                Navn = navn,
                Kortnavn = kortnavn,
                Organisasjonsnavn = organisasjonsnavn,
                Kontaktinformasjon = kontaktinformasjon,                 
            };
        }
    }
}
