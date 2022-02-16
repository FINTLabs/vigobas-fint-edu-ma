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
using FINT.Model.Resource;
using FINT.Model.Utdanning.Utdanningsprogram;
using FINT.Model.Utdanning.Elev;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class SkoleressursResourceFactory
    {
        public static SkoleressursResource Create(IEmbeddedResourceObject skoleressursData, string updateValue)
        {
            var feidenavn = new Identifikator();
            var systemId = new Identifikator();

            var values = skoleressursData.State;

            if (values.TryGetValue(FintAttribute.feidenavn, out IStateValue feidenavnValue))
            {
                feidenavn =
                    JsonConvert.DeserializeObject<Identifikator>(feidenavnValue.Value);
            }
            else
            {
                if (updateValue != FintAttribute.feidenavn)
                {
                    feidenavn = null;
                }
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

            var skoleressursResource = new SkoleressursResource
            {
                Feidenavn = feidenavn,
                SystemId = systemId,
            };

            var links = skoleressursData.Links;

            foreach (var linkKey in links.Keys)
            {
                switch (linkKey)
                {
                    case ResourceLink.personalResource:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                skoleressursResource.AddPersonalressurs(link);
                            }
                            break;
                        }
                    case ResourceLink.teachingRelationship:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                skoleressursResource.AddUndervisningsforhold(link);
                            }
                            break;
                        }
                    case ResourceLink.school:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                skoleressursResource.AddSkole(link);
                            }
                            break;
                        }
                }
            }
            return skoleressursResource;
        }
    }
}
