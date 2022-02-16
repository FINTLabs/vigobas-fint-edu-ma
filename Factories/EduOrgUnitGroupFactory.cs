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
using static VigoBAS.FINT.Edu.Constants;


namespace VigoBAS.FINT.Edu
{    
    class EduOrgUnitGroupFactory
    {
        public static EduOrgUnitGroup Create(string schoolUri, EduOrgUnit eduOrgUnit, string memberType)
        {
            var groupId = schoolUri + "_" + memberType;            
            string schoolRef = eduOrgUnit.Anchor();
            var groupNameSuffix = string.Empty;
            var groupType = string.Empty;
            var members = new List<string>(); 

            switch (memberType)
            {
                case ResourceLink.studentRelationship:
                    {
                        groupNameSuffix = "elever";
                        groupType = GroupType.aggrStu;
                        members = eduOrgUnit.SkoleElevforhold;
                        break;
                    }
                case ResourceLink.teachingRelationship:
                    {
                        groupNameSuffix = "lærere";
                        groupType = GroupType.aggrFac;
                        members = eduOrgUnit.SkoleUndervisningsforhold;
                        break;
                    }
                case ResourceLink.schoolresource:
                    {
                        groupNameSuffix = "ansatte";
                        groupType = GroupType.aggrEmp;
                        members = eduOrgUnit.SkoleAnsettelsesforhold;
                        break;
                    }
            }
            var groupName = string.Format($"{eduOrgUnit.SkoleNavn} alle {groupNameSuffix}");

            return new EduOrgUnitGroup
            {
                GroupId = groupId,
                GroupName = groupName,
                SchoolRef = schoolRef,
                GroupType = groupType,
                GroupMembers = members
            };
        }

        public static EduOrgUnitGroup Create(string orgUri, EduOrg eduOrg, string memberType)
        {
            var groupId = orgUri + "_" + memberType;
            //string orgRef = eduOrg.Anchor();
            var groupNameSuffix = string.Empty;
            var groupType = string.Empty; 
            switch (memberType)
            {
                case ResourceLink.studentRelationship:
                    {
                        groupNameSuffix = "elever";
                        groupType = GroupType.aggrStu;
                        break;
                    }
            }
            var groupName = string.Format($"{eduOrg.OrganisasjonNavn} alle {groupNameSuffix}");

            var members = new List<string>();

            return new EduOrgUnitGroup
            {
                GroupId = groupId,
                GroupName = groupName,
                //SchoolRef = orgRef,
                GroupType = groupType,
                GroupMembers = members
            };
        }
    }
}
