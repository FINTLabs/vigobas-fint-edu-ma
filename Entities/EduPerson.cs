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
using Vigo.Bas.ManagementAgent.Log;

namespace VigoBAS.FINT.Edu
{
    class EduPerson
    {
        public string ElevPersonalSystemId { get; set; }
        //Fint elev
        public string ElevSystemIdUri { get; set; }
        public string ElevElevnummer { get; set; }
        public string ElevBrukernavn { get; set; }
        public string ElevFeidenavn { get; set; }
        public string ElevKontaktinformasjonEpostadresse { get; set; }
        public string ElevKontaktinformasjonMobiltelfonnummer { get; set; }
        public string ElevSystemId { get; set; }

        // Fint Person        
        public string PersonBilde { get; set; }
        public string PersonFodselsdato { get; set; }
        public string PersonFodselsnummer { get; set; }
        public string PersonNavnFornavn { get; set; }
        public string PersonNavnMellomnavn { get; set; }
        public string PersonNavnEtternavn { get; set; }
        public List<string> PersonBostedsadresseAdresselinje { get; set; }
        public string PersonBostedsadressePostnummer { get; set; }
        public string PersonBostedsadressePoststed { get; set; }
        public List<string> PersonPostadresseAdresselinje { get; set; }
        public string PersonPostadressePostnummer { get; set; }
        public string PersonPostadressePoststed { get; set; }
        public string PersonKontaktinfomasjonEpostadresse { get; set; }
        public string PersonKontaktinfomasjonMobiltelefonnummer { get; set; }
        public string PersonKontaktinfomasjonTelefonnummer { get; set; }
        public string PersonKontaktinfomasjonSip { get; set; }

        // Relasjoner elev
        public List<string> ElevforholdSkole { get; set; }
        public List<string> ElevforholdKategori { get; set; }
        public string ElevforholdHovedkategori { get; set; }
        public List<string> ElevforholdBasisgruppe { get; set; }
        public List<string> ElevforholdKontaktlarergruppe { get; set; }
        public List<string> ElevforholdUndervisningsgruppe { get; set; }

        //Fint skoleressurs
        public string SkoleressursSystemIdUri { get; set; }
        public string SkoleressursFeidenavn { get; set; }

        //Fint personalressurs
        public string PersonalAnsattnummer { get; set; }
        public string PersonalSystemId { get; set; }
        public string PersonalBrukernavn { get; set; }
        public string PersonalKontaktinformasjonEpostadresse { get; set; }
        public string PersonalKontaktinformasjonMobiltelefonnummer { get; set; }
        public string PersonalKontaktinformasjonTelefonnummer { get; set; }
        public string PersonalKontaktinformasjonSip { get; set; }

        //Relasjoner lærer
        public List<string> UndervisningsforholdSkole { get; set; }
        public List<string> UndervisningsforholdMedlemskap { get; set; }

        //Relasjon adm ansatt
        public List<string> AnsettelsesforholdSkole { get; set; }

        public List<string> RolleOgSkole { get; set; }
        public List<string> ElevkategoriOgSkole { get; set; }
        public List<String> Eksamensdatoer { get; set; }
        public int AntallEksamener { get; set; }

        //Feideattributter      
        public string EduPersonOrgDN { get; set; }
        public List<string> EduPersonOrgUnitDN { get; set; }
        public string EduPersonPrimaryOrgUnitDN { get; set; }
        public List<string> EduPersonEntitlement { get; set; }
        


        public string Anchor()
        {
            return ElevPersonalSystemId;
        }

        public override string ToString()
        {
            return ElevPersonalSystemId;   
        }

        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSObjectType.eduPerson;

            /* Possible method for getting the attributes for the csentry with less code
            *  Problem: GetProperties return 0 attributes for the EduGroup class. It works here
            * 
           Type eduPersonType = typeof(EduPerson);

           var attributes = eduPersonType.GetProperties();

           foreach (var attribute in attributes)
           {

           }
           */
            Logger.Log.DebugFormat("Adding attributes to CS for resource: {0}", ElevPersonalSystemId);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevPersonalSystemId,ElevPersonalSystemId));

            if (!string.IsNullOrEmpty(ElevSystemId))
            {
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevSystemId,ElevSystemId));
            }
            if (!string.IsNullOrEmpty(ElevSystemIdUri))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevSystemIdUri, ElevSystemIdUri));
            }

            if (!string.IsNullOrEmpty(ElevElevnummer))
            {

                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevElevnummer,ElevElevnummer));
            }

            if (!string.IsNullOrEmpty(ElevBrukernavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevBrukernavn,ElevBrukernavn));
            }

            if (!string.IsNullOrEmpty(ElevFeidenavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevFeidenavn,ElevFeidenavn));
            }

            if (!string.IsNullOrEmpty(ElevKontaktinformasjonEpostadresse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevKontaktinformasjonEpostadresse,ElevKontaktinformasjonEpostadresse));
            }

            if (!string.IsNullOrEmpty(ElevKontaktinformasjonMobiltelfonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevKontaktinformasjonMobiltelefonnummer,ElevKontaktinformasjonMobiltelfonnummer));
            }

            if (!string.IsNullOrEmpty(PersonBilde))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonBilde,PersonBilde));
            }

            if (!string.IsNullOrEmpty(PersonFodselsdato))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonFodselsdato,PersonFodselsdato));
            }

            if (!string.IsNullOrEmpty(PersonFodselsnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonFodselsnummer,PersonFodselsnummer));
            }

            if (!string.IsNullOrEmpty(PersonNavnFornavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonNavnFornavn,PersonNavnFornavn));
            }

            if (!string.IsNullOrEmpty(PersonNavnMellomnavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonNavnMellomnavn,PersonNavnMellomnavn));
            }

            if (!string.IsNullOrEmpty(PersonNavnEtternavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonNavnEtternavn,PersonNavnEtternavn));
            }
            if (!string.IsNullOrEmpty(PersonKontaktinfomasjonMobiltelefonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonKontaktinformasjonMobiltelefonnummer, PersonKontaktinfomasjonMobiltelefonnummer));
            }
            if (!string.IsNullOrEmpty(PersonKontaktinfomasjonEpostadresse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonKontaktinformasjonEpostadresse, PersonKontaktinfomasjonEpostadresse));
            }
            if (PersonBostedsadresseAdresselinje != null && PersonBostedsadresseAdresselinje.Count > 0)
            {
                IList<object> lines = new List<object>();
                foreach (var line in PersonBostedsadresseAdresselinje)
                {
                    if (line is String && !string.IsNullOrEmpty(line))
                    {
                        lines.Add(line.ToString());
                    }
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonKontaktinformasjonBostedsadresseAdresselinje, lines));
            }
            if (!string.IsNullOrEmpty(PersonBostedsadressePostnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonKontaktinformasjonBostedsadressePostnummer, PersonBostedsadressePostnummer));
            }
            if (!string.IsNullOrEmpty(PersonBostedsadressePoststed))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonKontaktinformasjonBostedsadressePoststed, PersonBostedsadressePoststed));
            }
            if (PersonPostadresseAdresselinje != null && PersonPostadresseAdresselinje.Count > 0)
            {
                IList<object> lines = new List<object>();
                foreach (var line in PersonPostadresseAdresselinje)
                {
                    lines.Add(line.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonKontaktinformasjonPostadresseAdresselinje,lines));
            }
            if (!string.IsNullOrEmpty(PersonPostadressePostnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonKontaktinformasjonPostadressePostnummer,PersonPostadressePostnummer));
            }
            if (!string.IsNullOrEmpty(PersonPostadressePoststed))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonKontaktinformasjonPostadressePoststed,PersonPostadressePoststed));
            }

            if (ElevforholdBasisgruppe != null && ElevforholdBasisgruppe.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in ElevforholdBasisgruppe)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdBasisgruppe, members));
            }
            if (ElevforholdKontaktlarergruppe != null && ElevforholdKontaktlarergruppe.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in ElevforholdKontaktlarergruppe)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdKontaktlarergruppe, members));
            }
            if (ElevforholdUndervisningsgruppe != null && ElevforholdUndervisningsgruppe.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in ElevforholdUndervisningsgruppe)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdUndervisningsgruppe, members));
            }
            if (ElevforholdSkole != null && ElevforholdSkole.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in ElevforholdSkole)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdSkole,members));
            }
            if (ElevforholdKategori != null && ElevforholdKategori.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in ElevforholdKategori)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdKategori, members));
            }
            if (!string.IsNullOrEmpty(ElevforholdHovedkategori))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdHovedkategori, ElevforholdHovedkategori));
            }
            // Personal skoleressurs
            if (!string.IsNullOrEmpty(SkoleressursSystemIdUri))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleressursSystemIdUri, SkoleressursSystemIdUri));
            }
            if (!string.IsNullOrEmpty(SkoleressursFeidenavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SkoleressursFeidenavn,SkoleressursFeidenavn));
            }

            if (!string.IsNullOrEmpty(PersonalSystemId))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonalSystemId,PersonalSystemId));
            }

            if (!string.IsNullOrEmpty(PersonalAnsattnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonalAnsattnummer,PersonalAnsattnummer));
            }
            if (!string.IsNullOrEmpty(PersonalBrukernavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonalBrukernavn,PersonalBrukernavn));
            }
            if (!string.IsNullOrEmpty(PersonalKontaktinformasjonEpostadresse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonalKontaktinformasjonEpostadresse,PersonalKontaktinformasjonEpostadresse));
            }
            if (!string.IsNullOrEmpty(PersonalKontaktinformasjonMobiltelefonnummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonalKontaktinformasjonMobiltelefonnummer,PersonalKontaktinformasjonMobiltelefonnummer));
            }
            if (!string.IsNullOrEmpty(PersonalKontaktinformasjonSip))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.PersonalKontaktinformasjonSip,PersonalKontaktinformasjonSip));
            }
            if (UndervisningsforholdMedlemskap != null && UndervisningsforholdMedlemskap.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in UndervisningsforholdMedlemskap)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.UndervisningsforholdMedlemskap,members));
            }
            if (UndervisningsforholdSkole != null && UndervisningsforholdSkole.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in UndervisningsforholdSkole)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.UndervisningsforholdSkole,members));
            }
            if (AnsettelsesforholdSkole != null && AnsettelsesforholdSkole.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in AnsettelsesforholdSkole)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.AnsettelsesforholdSkole, members));
            }
            if (EduPersonEntitlement != null && EduPersonEntitlement.Count >0)
            {
                IList<object> entitlements = new List<object>();
                foreach (var entitlement in EduPersonEntitlement)
                {
                    entitlements.Add(entitlement.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.EduPersonEntitlement, entitlements));
            }
            if (!string.IsNullOrEmpty(EduPersonPrimaryOrgUnitDN))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.EduPersonPrimaryOrgUnitDN, EduPersonPrimaryOrgUnitDN));
            }
            else
            {
                if (EduPersonOrgUnitDN != null && EduPersonOrgUnitDN.Count > 0)
                {
                    string firstOrgUnitDN = EduPersonOrgUnitDN[0];
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.EduPersonPrimaryOrgUnitDN, firstOrgUnitDN));
                }
            }
            if (EduPersonOrgUnitDN != null && EduPersonOrgUnitDN.Count > 0)
            {
                IList<object> orgUnits = new List<object>();
                foreach (var orgUnit in EduPersonOrgUnitDN)
                {
                    orgUnits.Add(orgUnit.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.EduPersonOrgUnitDN, orgUnits));
            }
            if (!string.IsNullOrEmpty(EduPersonOrgDN))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.EduPersonOrgDN, EduPersonOrgDN));
            }
            if (RolleOgSkole != null && RolleOgSkole.Count > 0)
            {
                IList<object> items = new List<object>();
                foreach (var item in RolleOgSkole)
                {
                    items.Add(item.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.RolleOgSkole, items));
            }
            if (ElevkategoriOgSkole != null && ElevkategoriOgSkole.Count > 0)
            {
                IList<object> items = new List<object>();
                foreach (var item in ElevkategoriOgSkole)
                {
                    items.Add(item.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevkategoriOgSkole, items));
            }
            if (Eksamensdatoer != null && Eksamensdatoer.Count > 0)
            {
                IList<object> items = new List<object>();
                foreach (var item in Eksamensdatoer)
                {
                    items.Add(item.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.Eksamensdatoer, items));
            }
            if (AntallEksamener > 0)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.AntallEksamener, AntallEksamener));
                Logger.Log.DebugFormat("Number of exams for eduPerson {0} is {1}", Anchor(), AntallEksamener);
            }
            return csentry;

        }
    }
}
