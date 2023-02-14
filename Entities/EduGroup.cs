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
    class EduGroup
    {
        public string GruppeSystemIdUri;
        public string EduGroupType;
        // Fint gruppe
        public string GruppeSystemId;
        public string GruppeBeskrivelse;
        public string GruppeNavn;
        public string GruppePeriodeStart;
        public string GruppePeriodeSlutt;
        public string GruppeSkoleRef;
        public string GruppeSkoleSkolenummer;
        //public string GruppeGrepReferanse;
        public Grepkode Grepkode;
        public string GruppeFagRef;
        public EduGroup Utdanningsprogram;
        //public string GruppeVigoReferanse;
        public List<string> GruppeElevListe;
        public List<string> GruppeLarerListe;
        public List<string> GruppeGruppeListe;


        // Fint basisgruppe
        //public string BasisgruppeTrinn;

        // Fint undervisningsgruppe
        //public List<string> UndervisningsgruppeFag;

        public string Anchor()
        {
            return GruppeSystemIdUri;
        }

        public override string ToString()
        {
            return GruppeSystemIdUri;
        }

        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSObjectType.eduGroup;

            /* Possible method for getting the attributes for the csentry with less code
             * Problem: GetProperties return 0 attributes. This works for the EduPerson class 
             * 
             Type eduGroupType = typeof(EduGroup);

            var attributes = eduGroupType.GetProperties();

            */
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeSystemIdUri, GruppeSystemIdUri));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeSystemId, GruppeSystemId));

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeNavn, GruppeNavn));

            if (!string.IsNullOrEmpty(EduGroupType))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.EduGroupType, EduGroupType));
            }

            if (!string.IsNullOrEmpty(GruppeBeskrivelse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeBeskrivelse, GruppeBeskrivelse));
            }
            if (!string.IsNullOrEmpty(GruppeFagRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeFagRef, GruppeFagRef));
            }
            if (!string.IsNullOrEmpty(GruppePeriodeStart))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppePeriodeStart, GruppePeriodeStart));
            }
            if (!string.IsNullOrEmpty(GruppePeriodeSlutt))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppePeriodeSlutt, GruppePeriodeSlutt));
            }
            if (!string.IsNullOrEmpty(GruppeSkoleRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeSkoleRef, GruppeSkoleRef));
            }
            if (!string.IsNullOrEmpty(GruppeSkoleSkolenummer))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeSkoleSkolenummer, GruppeSkoleSkolenummer));
            }
            IList<object> studfacmembers = new List<object>();
            if (GruppeElevListe != null && GruppeElevListe.Count > 0)
            {
                IList<object> members = new List<object>();
                int noOfMembers = 0;

                foreach (var member in GruppeElevListe)
                {
                    var memberUri = member.ToString();
                    members.Add(memberUri);
                    noOfMembers++;
                    studfacmembers.Add(memberUri);
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeElevListe, members));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeElevAntall, noOfMembers.ToString()));
            }
            if (GruppeLarerListe != null && GruppeLarerListe.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in GruppeLarerListe)
                {
                    var memberUri = member.ToString();
                    members.Add(memberUri);

                    if (!studfacmembers.Contains(memberUri))
                    {
                        studfacmembers.Add(memberUri);
                    }
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeLarerListe, members));
            }
            if (studfacmembers.Count > 0)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeLarerOgElevListe, studfacmembers));
            }
            if (GruppeGruppeListe != null && GruppeGruppeListe.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in GruppeGruppeListe)
                {
                    var memberUri = member.ToString();
                    members.Add(memberUri);

                    if (!studfacmembers.Contains(memberUri))
                    {
                        studfacmembers.Add(memberUri);
                    }
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GruppeGruppeListe, members));
            }

            return csentry;
        }
    }
}
