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
using FINT.Model.Utdanning.Timeplan;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class PeriodeFactory
    {
        public static Periode Create(IReadOnlyDictionary<string, IStateValue> values)
        {
            var periode = new Periode();
            if (values.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue periodeValue))
            {
               periode = JsonConvert.DeserializeObject<Periode>(periodeValue.Value);

            }
            if (periode.Start != null && periode.Slutt != null)
            {
                return periode;
            }
            else 
            {
                return null;
            }

        }
    }
}
