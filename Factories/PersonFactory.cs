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
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    public class PersonFactory
    {
        public static Person Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var kontaktinformasjon = new Kontaktinformasjon();
            var postadresse = new Adresse();
            var bostedsadresse = new Adresse();
            var fodselsnummer = new Identifikator();
            DateTime fodselsdato = new DateTime();
            var navn = new Personnavn();

            if (values.TryGetValue(FintAttribute.kontaktinformasjon, out IStateValue dictVal))
            {
                kontaktinformasjon =
                    JsonConvert.DeserializeObject<Kontaktinformasjon>(dictVal.Value);
            }
            if (values.TryGetValue(FintAttribute.postadresse, out IStateValue dictVal1))
            {
                postadresse = JsonConvert.DeserializeObject<Adresse>(dictVal1.Value);
            }
            if (values.TryGetValue(FintAttribute.bostedsadresse, out IStateValue dictVal2))
            {
                bostedsadresse = JsonConvert.DeserializeObject<Adresse>(dictVal2.Value);
            }
            if (values.TryGetValue(FintAttribute.fodselsnummer, out IStateValue dictVal3))
            {
                fodselsnummer = JsonConvert.DeserializeObject<Identifikator>(dictVal3.Value);
            }
            if (values.TryGetValue(FintAttribute.fodselsdato, out IStateValue dictVal4))
            {
                fodselsdato = DateTime.Parse(dictVal4.Value);
            }
            if (values.TryGetValue(FintAttribute.navn, out IStateValue dictVal5))
            {
                navn = JsonConvert.DeserializeObject<Personnavn>(dictVal5.Value);
            }
            return new Person
            {
                Kontaktinformasjon = kontaktinformasjon,
                Postadresse = postadresse,
                Bostedsadresse = bostedsadresse,
                Fodselsnummer = fodselsnummer,
                Fodselsdato = fodselsdato,
                Navn = navn
            };
        }
    }
}