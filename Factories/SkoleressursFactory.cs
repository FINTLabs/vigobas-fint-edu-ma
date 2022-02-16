﻿// VIGOBAS Identity Management System 
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
using FINT.Model.Utdanning.Elev;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class SkoleressursFactory
    {
        public static Skoleressurs Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var feidenavn = new Identifikator();
            var systemId = new Identifikator();

            if (values.TryGetValue(FintAttribute.feidenavn, out IStateValue feidenavnValue))
            {
                feidenavn =
                    JsonConvert.DeserializeObject<Identifikator>(feidenavnValue.Value);
            }
            else
            {
                feidenavn = null;
            }
            if (values.TryGetValue(FintAttribute.systemId, out IStateValue systemIdValue))
            {
                systemId =
                    JsonConvert.DeserializeObject<Identifikator>(systemIdValue.Value);
            }
            else
            {
                systemId = null;
            }

            return new Skoleressurs
            {
                Feidenavn = feidenavn,
                SystemId = systemId,
            };
        }
    }
}
