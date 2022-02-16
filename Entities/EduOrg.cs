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
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MetadirectoryServices;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class EduOrg
    {
        public string OrganisasjonOrganisasjonsIdUri;
        public string OrganisasjonOrganisasjonsId;
        public string OrganisasjonOrganisasjonsnummer;
        public string OrganisasjonNavn;
        public string OrganisasjonOrganisasjonsnavn;
        public string OrganisasjonDomenenavn;
        public string OrganisasjonJuridiskNavn;
        public List<string> OrganisasjonForretningsadresseAdresselinje;
        public string OrganisasjonForretningsadressePostnummer;
        public string OrganisasjonForretningsadressePoststed;
        public string OrganisasjonKontaktinformasjonEpostadresse;
        public string OrganisasjonKontaktinformasjonTelefonnummer;
        public string OrganisasjonKontaktinformasjonMobiltelefonnummer;
        public string OrganisasjonKontaktinformasjonNettsted;
        public string OrganisasjonKontaktinformasjonSip;

        public string Anchor()
        {
            return OrganisasjonOrganisasjonsIdUri;
        }

        public override string ToString()
        {
            return OrganisasjonOrganisasjonsIdUri;
        }

        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSObjectType.eduOrg;

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonOrganisasjonsIdUri, OrganisasjonOrganisasjonsIdUri));

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonOrganisasjonsId, OrganisasjonOrganisasjonsId));

            if (!string.IsNullOrEmpty(OrganisasjonNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonNavn, OrganisasjonNavn));
            }
            if (!string.IsNullOrEmpty(OrganisasjonOrganisasjonsnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonOrganisasjonsnummer, OrganisasjonOrganisasjonsnummer));
            }
            if (!string.IsNullOrEmpty(OrganisasjonDomenenavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonDomenenavn, OrganisasjonDomenenavn));
            }
            if (!string.IsNullOrEmpty(OrganisasjonJuridiskNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonJuridiskNavn, OrganisasjonJuridiskNavn));
            }
            if (!string.IsNullOrEmpty(OrganisasjonOrganisasjonsnavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonOrganisasjonsnavn, OrganisasjonOrganisasjonsnavn));
            }
            if (OrganisasjonForretningsadresseAdresselinje != null && OrganisasjonForretningsadresseAdresselinje.Count > 0)
            {
                IList<object> lines = new List<object>();
                foreach (var line in OrganisasjonForretningsadresseAdresselinje)
                {
                    lines.Add(line.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonForretningsadresseAdresselinje, lines));
            }
            if (!string.IsNullOrEmpty(OrganisasjonForretningsadressePostnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonForretningsadressePostnummer, OrganisasjonForretningsadressePostnummer));
            }
            if (!string.IsNullOrEmpty(OrganisasjonForretningsadressePoststed))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonForretningsadressePoststed, OrganisasjonForretningsadressePoststed));
            }
            if (!string.IsNullOrEmpty(OrganisasjonKontaktinformasjonEpostadresse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonKontaktinformasjonEpostadresse, OrganisasjonKontaktinformasjonEpostadresse));
            }
            if (!string.IsNullOrEmpty(OrganisasjonKontaktinformasjonTelefonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonKontaktinformasjonTelefonnummer, OrganisasjonKontaktinformasjonTelefonnummer));
            }
            if (!string.IsNullOrEmpty(OrganisasjonKontaktinformasjonMobiltelefonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonKontaktinformasjonMobiltelefonnummer, OrganisasjonKontaktinformasjonMobiltelefonnummer));
            }
            if (!string.IsNullOrEmpty(OrganisasjonKontaktinformasjonNettsted))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonKontaktinformasjonNettsted, OrganisasjonKontaktinformasjonNettsted));
            }
            if (!string.IsNullOrEmpty(OrganisasjonKontaktinformasjonSip))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.OrganisasjonKontaktinformasjonSip, OrganisasjonKontaktinformasjonSip));
            }
            return csentry;
        }
    }
}
