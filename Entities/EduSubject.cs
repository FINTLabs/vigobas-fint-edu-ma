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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MetadirectoryServices;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class EduSubject
    {
        public string FagSystemIdUri;
        public string FagSystemId;
        public string FagNavn;
        public string FagBeskrivelse;

        public string Anchor()
        {
            return FagSystemIdUri;
        }
        public override string ToString()
        {
            return FagSystemIdUri;
        }
        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSObjectType.eduSubject;

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.FagSystemIdUri, FagSystemIdUri));

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.FagSystemId, FagSystemId));

            if (!string.IsNullOrEmpty(FagNavn))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.FagNavn, FagNavn));
            }
            if (!string.IsNullOrEmpty(FagBeskrivelse))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.FagBeskrivelse, FagBeskrivelse));
            }
            return csentry;
        }

    }
}
