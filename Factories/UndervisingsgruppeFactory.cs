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
using FINT.Model.Utdanning.Basisklasser;
using FINT.Model.Utdanning.Elev;
using FINT.Model.Utdanning.Timeplan;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class UndervisningsgruppeFactory
    {
        public static Undervisningsgruppe Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var systemId = new Identifikator();
            string navn = String.Empty;
            string beskrivelse = String.Empty;

            if (values.TryGetValue(FintAttribute.systemId, out IStateValue systemIDValue))
            {
                systemId = JsonConvert.DeserializeObject<Identifikator>(systemIDValue.Value);
            }
            if (values.TryGetValue(FintAttribute.navn, out IStateValue navnValue))
            {
                navn = navnValue.Value;
            }
            if (values.TryGetValue(FintAttribute.beskrivelse, out IStateValue beskrivelseValue))
            {
                beskrivelse = beskrivelseValue.Value;
            }

            return new Undervisningsgruppe
            {
                SystemId = systemId,
                Beskrivelse = beskrivelse,
                Navn = navn,
            };
        }
    }
}