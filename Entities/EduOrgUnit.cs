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
    class EduOrgUnit
    {
        public string SystemIdUri;
        public string SkoleSystemId;
        public string SkoleOrganisasjonsnummer;
        public string SkoleSkolenummer;
        public string SkoleNavn;
        public string SkoleDomenenavn;
        public string SkoleJuridiskNavn;
        public string SkoleOrganisasjonsnavn;
        public List<string> SkoleForretningsadresseAdresselinje;
        public string SkoleForretningsadressePostnummer;
        public string SkoleForretningsadressePoststed;
        public string SkoleKontaktinformasjonEpostadresse;
        public string SkoleKontaktinformasjonTelefonnummer;
        public string SkoleKontaktinformasjonMobiltelefonnummer;
        public string SkoleKontaktinformasjonNettsted;
        public string SkoleKontaktinformasjonSip;
        public string SkoleOrganisasjonselement;
        public string Skoleeier;

        public List<string> SkoleElevforhold;
        public List<string> SkoleUndervisningsforhold;
        public List<string> SkoleAnsettelsesforhold;

        public string Anchor()
        {
            return SystemIdUri;
        }

        public override string ToString()
        {
            return SystemIdUri;
        }

        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSObjectType.eduOrgUnit;


            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.systemIdUri, SystemIdUri));

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleSystemId, SkoleSystemId));

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleNavn, SkoleNavn));

            if (!string.IsNullOrEmpty(SkoleOrganisasjonsnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleOrganisasjonsnummer, SkoleOrganisasjonsnummer));
            }
            if (!string.IsNullOrEmpty(SkoleSkolenummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleSkolenummer, SkoleSkolenummer));
            }
            if (!string.IsNullOrEmpty(SkoleDomenenavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleDomenenavn, SkoleDomenenavn));
            }
            if (!string.IsNullOrEmpty(SkoleJuridiskNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleJuridiskNavn, SkoleJuridiskNavn));
            }
            if (!string.IsNullOrEmpty(SkoleOrganisasjonsnavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleOrganisasjonsnavn, SkoleOrganisasjonsnavn));
            }
            if (SkoleForretningsadresseAdresselinje != null && SkoleForretningsadresseAdresselinje.Count > 0)
            {
                IList<object> lines = new List<object>();
                foreach (var line in SkoleForretningsadresseAdresselinje)
                {
                    lines.Add(line.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleForretningsadresseAdresselinje, lines));
            }
            if (!string.IsNullOrEmpty(SkoleForretningsadressePostnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleForretningsadressePostnummer, SkoleForretningsadressePostnummer));
            }
            if (!string.IsNullOrEmpty(SkoleForretningsadressePoststed))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleForretningsadressePoststed, SkoleForretningsadressePoststed));
            }
            if (!string.IsNullOrEmpty(SkoleKontaktinformasjonEpostadresse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleKontaktinformasjonEpostadresse, SkoleKontaktinformasjonEpostadresse));
            }
            if (!string.IsNullOrEmpty(SkoleKontaktinformasjonTelefonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleKontaktinformasjonTelefonnummer, SkoleKontaktinformasjonTelefonnummer));
            }
            if (!string.IsNullOrEmpty(SkoleKontaktinformasjonMobiltelefonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleKontaktinformasjonMobiltelefonnummer, SkoleKontaktinformasjonMobiltelefonnummer));
            }
            if (!string.IsNullOrEmpty(SkoleKontaktinformasjonNettsted))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleKontaktinformasjonNettsted, SkoleKontaktinformasjonNettsted));
            }
            if (!string.IsNullOrEmpty(SkoleKontaktinformasjonSip))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleKontaktinformasjonSip, SkoleKontaktinformasjonSip));
            }
            if (!string.IsNullOrEmpty(Skoleeier))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.Skoleeier, Skoleeier));
            }
            if (SkoleElevforhold != null && SkoleElevforhold.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in SkoleElevforhold)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleElevforhold, members));
            }
            if (SkoleUndervisningsforhold != null && SkoleUndervisningsforhold.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in SkoleUndervisningsforhold)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleUndervisningsforhold, members));
            }
            if (SkoleAnsettelsesforhold != null && SkoleAnsettelsesforhold.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in SkoleAnsettelsesforhold)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleAnsettelsesforhold, members));
            }
            if (!string.IsNullOrEmpty(SkoleOrganisasjonselement))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleOrganisasjonselement, SkoleOrganisasjonselement));
            }
            return csentry;
        }
    }
}
