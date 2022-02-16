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
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Utdanning.Utdanningsprogram;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class SkoleFactory
    {
        public static Skole Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var systemId = new Identifikator();
            var skolenummer = new Identifikator();
            string navn = String.Empty;
            string domenenavn = String.Empty;
            string juridiskNavn = String.Empty;
            string organisasjonsnavn = String.Empty;
            var organisasjonsnummer = new Identifikator();
            var forretningsadresse = new Adresse();
            var kontaktinformasjon = new Kontaktinformasjon();


            if (values.TryGetValue(FintAttribute.systemId, out IStateValue systemIDValue))
            {
                systemId = JsonConvert.DeserializeObject<Identifikator>(systemIDValue.Value);
            }
            if (values.TryGetValue(FintAttribute.organisasjonsnummer, out IStateValue organisasjonsnummerValue))
            {
                organisasjonsnummer = JsonConvert.DeserializeObject<Identifikator>(organisasjonsnummerValue.Value);
            }

            if (values.TryGetValue(FintAttribute.navn, out IStateValue navnValue))
            {
                navn = navnValue.Value;
            }
            if (values.TryGetValue(FintAttribute.domenenavn, out IStateValue domenenavnValue))
            {
                domenenavn = domenenavnValue.Value;
            }
            if (values.TryGetValue(FintAttribute.juridiskNavn, out IStateValue juridiskNavnValue))
            {
                juridiskNavn = juridiskNavnValue.Value;
            }
            if (values.TryGetValue(FintAttribute.organisasjonsnavn, out IStateValue organisasjonsnavnValue))
            {
                organisasjonsnavn = organisasjonsnavnValue.Value;
            }
            if (values.TryGetValue(FintAttribute.forretningsadresse, out IStateValue forretningsadresseValue))
            {
                forretningsadresse = JsonConvert.DeserializeObject<Adresse>(forretningsadresseValue.Value);
            }
            if (values.TryGetValue(FintAttribute.kontaktinformasjon, out IStateValue kontaktinformasjonValue))
            {
                kontaktinformasjon = JsonConvert.DeserializeObject<Kontaktinformasjon>(kontaktinformasjonValue.Value);
            }
            return new Skole
            {
                SystemId = systemId,
                Organisasjonsnummer = organisasjonsnummer,
                Skolenummer = skolenummer,
                Navn = navn,
                Domenenavn = domenenavn,
                JuridiskNavn = juridiskNavn,
                Organisasjonsnavn = organisasjonsnavn,
                Forretningsadresse = forretningsadresse,
                Kontaktinformasjon = kontaktinformasjon,
            };
        }
    }
}

