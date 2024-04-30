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
    class EduStudentRelationship
    {
        public string ElevforholdSystemIdUri { get; set; }
        public string ElevforholdSystemId { get; set; }
        public string ElevforholdGyldighetsperiodeStart { get; set; }
        public string ElevforholdGyldighetsperiodeSlutt { get; set; }
        public bool? ElevforholdHovedskole { get; set; }        
        public string ElevforholdElevkategori { get; set; }
        public string ElevforholdProgramomrade { get; set; }
        public string ElevforholdElevRef { get; set; }
        public string ElevforholdSkoleRef { get; set; }
        public string ElevforholdBasisgruppeRef { get; set; }

        public string Anchor()
        {
            return ElevforholdSystemIdUri;
        }
        public override string ToString()
        {
            return ElevforholdSystemIdUri;
        }
        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSObjectType.eduStudentRelationship;

            Logger.Log.DebugFormat("Adding attributes to CS for resource: {0}", ElevforholdSystemIdUri);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdSystemIdUri, ElevforholdSystemIdUri));

            if (ElevforholdHovedskole.HasValue)
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdHovedskole, ElevforholdHovedskole));
            }
            if (!string.IsNullOrEmpty(ElevforholdGyldighetsperiodeStart))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdGyldighetsperiodeStart, ElevforholdGyldighetsperiodeStart));
            }
            if (!string.IsNullOrEmpty(ElevforholdGyldighetsperiodeSlutt))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdGyldighetsperiodeSlutt, ElevforholdGyldighetsperiodeSlutt));
            }
            if (!string.IsNullOrEmpty(ElevforholdElevkategori))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdElevkategori, ElevforholdElevkategori));
            }
            if (!string.IsNullOrEmpty(ElevforholdElevRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdElevRef, ElevforholdElevRef));
            }
            if (!string.IsNullOrEmpty(ElevforholdSkoleRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdSkoleRef, ElevforholdSkoleRef));
            }
            if (!string.IsNullOrEmpty(ElevforholdProgramomrade))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdProgramomrade, ElevforholdProgramomrade));
            }
            if (!string.IsNullOrEmpty(ElevforholdBasisgruppeRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.ElevforholdBasisgruppeRef, ElevforholdBasisgruppeRef));
            }

            return csentry;
        }
    }
}
