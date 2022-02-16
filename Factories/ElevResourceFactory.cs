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
using FINT.Model.Utdanning.Elev;
using FINT.Model.Resource;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;


namespace VigoBAS.FINT.Edu
{
    public class ElevResourceFactory
    {
        public static ElevResource Create(IEmbeddedResourceObject elevData)
        {
            var elevnummer = new Identifikator();
            var brukernavn = new Identifikator();
            var feidenavn = new Identifikator();
            var kontaktinformasjon = new Kontaktinformasjon();
            var systemId = new Identifikator();

            var values = elevData.State;

            if (values.TryGetValue(FintAttribute.elevnummer, out IStateValue elevnummerValue))
            {
                elevnummer = JsonConvert.DeserializeObject<Identifikator>(elevnummerValue.Value);
            }
            else
            {
                elevnummer = null;
            }
            if (values.TryGetValue(FintAttribute.brukernavn, out IStateValue brukernavnValue))
            {
                brukernavn =
                    JsonConvert.DeserializeObject<Identifikator>(brukernavnValue.Value);
            }
            else
            {
                brukernavn = null;
            }
            if (values.TryGetValue(FintAttribute.feidenavn, out IStateValue feidenavnValue))
            {
                feidenavn =
                    JsonConvert.DeserializeObject<Identifikator>(feidenavnValue.Value);
            }
            else
            {
                feidenavn = null;
            }
            if (values.TryGetValue(FintAttribute.kontaktinformasjon, out IStateValue kontaktinformasjonValue))
            {
                kontaktinformasjon =
                    JsonConvert.DeserializeObject<Kontaktinformasjon>(kontaktinformasjonValue.Value);
            }
            else
            {
                kontaktinformasjon = null;
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

            var elevResource = new ElevResource
            {
                Brukernavn = brukernavn,
                Elevnummer = elevnummer,
                Feidenavn = feidenavn,
                Kontaktinformasjon = kontaktinformasjon,
                SystemId = systemId
            };

            var links = elevData.Links;

            foreach (var linkKey in links.Keys)
            {
                switch (linkKey)
                {
                    case ResourceLink.person:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {                               
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                elevResource.AddPerson(link);
                            }
                            break;
                        }
                    case ResourceLink.studentRelationship:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                elevResource.AddElevforhold(link);
                            }
                            break;
                        }
                }
                
            }
            return elevResource;
        }
        public static ElevResource Create(IEmbeddedResourceObject elevData, string updateValue)
        {
            var elevnummer = new Identifikator();
            var brukernavn = new Identifikator();
            var feidenavn = new Identifikator();
            var kontaktinformasjon = new Kontaktinformasjon();
            var systemId = new Identifikator();

            var values = elevData.State;

            if (values.TryGetValue(FintAttribute.elevnummer, out IStateValue elevnummerValue))
            {
                elevnummer = JsonConvert.DeserializeObject<Identifikator>(elevnummerValue.Value);
            }
            else
            {
                elevnummer = null;
            }
            if (values.TryGetValue(FintAttribute.brukernavn, out IStateValue brukernavnValue))
            {
                brukernavn =
                    JsonConvert.DeserializeObject<Identifikator>(brukernavnValue.Value);
            }
            else
            {
                brukernavn = null;
            }
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
            if (values.TryGetValue(FintAttribute.kontaktinformasjon, out IStateValue kontaktinformasjonValue))
            {
                kontaktinformasjon =
                    JsonConvert.DeserializeObject<Kontaktinformasjon>(kontaktinformasjonValue.Value);
            }
            else
            {
                kontaktinformasjon = null;
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

            var elevResource = new ElevResource
            {
                Brukernavn = brukernavn,
                Elevnummer = elevnummer,
                Feidenavn = feidenavn,
                Kontaktinformasjon = kontaktinformasjon,
                SystemId = systemId
            };

            var links = elevData.Links;

            foreach (var linkKey in links.Keys)
            {
                switch (linkKey)
                {
                    case ResourceLink.person:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                elevResource.AddPerson(link);
                            }
                            break;
                        }
                    case ResourceLink.studentRelationship:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                elevResource.AddElevforhold(link);
                            }
                            break;
                        }
                }

            }
            return elevResource;
        }

        public static ElevResource Create(IRootResourceObject elevData)
        {
            var elevnummer = new Identifikator();
            var brukernavn = new Identifikator();
            var feidenavn = new Identifikator();
            var kontaktinformasjon = new Kontaktinformasjon();
            var systemId = new Identifikator();

            var values = elevData.State;

            if (values.TryGetValue(FintAttribute.elevnummer, out IStateValue elevnummerValue))
            {
                elevnummer = JsonConvert.DeserializeObject<Identifikator>(elevnummerValue.Value);
            }
            else
            {
                elevnummer = null;
            }
            if (values.TryGetValue(FintAttribute.brukernavn, out IStateValue brukernavnValue))
            {
                brukernavn =
                    JsonConvert.DeserializeObject<Identifikator>(brukernavnValue.Value);
            }
            else
            {
                brukernavn = null;
            }
            if (values.TryGetValue(FintAttribute.feidenavn, out IStateValue feidenavnValue))
            {
                feidenavn =
                    JsonConvert.DeserializeObject<Identifikator>(feidenavnValue.Value);
            }
            else
            {
                feidenavn = null;
            }
            if (values.TryGetValue(FintAttribute.kontaktinformasjon, out IStateValue kontaktinformasjonValue))
            {
                kontaktinformasjon =
                    JsonConvert.DeserializeObject<Kontaktinformasjon>(kontaktinformasjonValue.Value);
            }
            else
            {
                kontaktinformasjon = null;
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

            var elevResource = new ElevResource
            {
                Brukernavn = brukernavn,
                Elevnummer = elevnummer,
                Feidenavn = feidenavn,
                Kontaktinformasjon = kontaktinformasjon,
                SystemId = systemId
            };

            var links = elevData.Links;

            foreach (var linkKey in links.Keys)
            {
                switch (linkKey)
                {
                    case ResourceLink.person:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                elevResource.AddPerson(link);
                            }
                            break;
                        }
                    case ResourceLink.studentRelationship:
                        {
                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                var hrefValue = linkObject.Href.ToString();
                                var link = Link.with(hrefValue);
                                elevResource.AddElevforhold(link);
                            }
                            break;
                        }
                }

            }
            return elevResource;
        }
    }
}
