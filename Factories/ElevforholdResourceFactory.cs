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
    class ElevforholdResourceFactory
    {
        public static ElevforholdResource Create(IEmbeddedResourceObject elevforholdData)
        {
            var systemId = new Identifikator();
            var gyldighetsperiode = new Periode();
            bool hovedskole = false;

            var values = elevforholdData.State;

            if (values.TryGetValue(FintAttribute.systemId, out IStateValue systemIdValue))
            {
                systemId =
                    JsonConvert.DeserializeObject<Identifikator>(systemIdValue.Value);
            }
            else
            {
                systemId = null;
            }
            if (values.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue periodeValue))
            {
                gyldighetsperiode = JsonConvert.DeserializeObject<Periode>(periodeValue.Value);
            }
            if (values.TryGetValue(FintAttribute.hovedskole , out IStateValue hovedskoleValue))
            {
                hovedskole = Convert.ToBoolean(hovedskoleValue.Value);
            }
            var elevforholdResource = new ElevforholdResource
            {
                SystemId = systemId,
                Gyldighetsperiode = gyldighetsperiode,
                Hovedskole = hovedskole
            };

            var links = elevforholdData.Links;

            foreach (var linkKey in links.Keys)
            {
                switch (linkKey)
                {
                    case ResourceLink.student:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                elevforholdResource.AddElev(link);
                            }
                            break;
                        }
                    case ResourceLink.studentCategory:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                elevforholdResource.AddKategori(link);
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
                                elevforholdResource.AddSkole(link);
                            }
                            break;
                        }
                    case ResourceLink.basicGroup:
                        { 
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                //elevforholdResource.AddBasisgruppe(link);
                            }
                            break;

                        }
                    case ResourceLink.programmearea:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                //elevforholdResource.AddProgramomrade(link);
                            }
                            break;
                        }
                }
            }
            return elevforholdResource;
        }
    }
}
