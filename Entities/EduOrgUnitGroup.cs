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
    class EduOrgUnitGroup
    {
        public string GroupId;
        public string GroupName;
        public string GroupType;
        public string SchoolRef;
        public List<string> GroupMembers;

        public string Anchor()
        {
            return GroupId;
        }

        public override string ToString()
        {
            return GroupId;
        }

        internal Microsoft.MetadirectoryServices.CSEntryChange GetCSEntryChange()
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = CSObjectType.eduOrgUnitGroup;

            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GroupId, GroupId));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GroupName, GroupName));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GroupType, GroupType));
            
            if (!string.IsNullOrEmpty(SchoolRef))
            {
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.SchoolRef, SchoolRef));
            }
            if (GroupMembers != null && GroupMembers.Count > 0)
            {
                IList<object> members = new List<object>();
                foreach (var member in GroupMembers)
                {
                    members.Add(member.ToString());
                }
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(CSAttribute.GroupMembers, members));
            }

            return csentry;
        }
    }
}
