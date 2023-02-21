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

using System.Linq;
using System.Collections.Generic;
using FINT.Model.Utdanning.Basisklasser;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using static VigoBAS.FINT.Edu.Constants;

namespace VigoBAS.FINT.Edu
{
    class EduGroupFactory
    {
        public static EduGroup Create(
            string systemIdUri,
            Gruppe basicGroup,
            string groupType,
            IReadOnlyDictionary<string, IEnumerable<ILinkObject>> groupLinks,
            EduOrgUnit  school,
            EduGroup studyProgramme
        )
        {
            var systemId = basicGroup.SystemId.Identifikatorverdi;
            var navn = basicGroup.Navn;
            var beskrivelse = basicGroup?.Beskrivelse;
            string periodeStart = string.Empty;
            string periodeSlutt = string.Empty;
            if (basicGroup.Periode.Count > 0)
            {
                periodeStart = basicGroup.Periode[0]?.Start.ToString(dateFormat);
                periodeSlutt = basicGroup.Periode[0]?.Slutt?.ToString(dateFormat);
            }
            var grepkode = new Grepkode();

            var schoolUri = school.SystemIdUri;

            var schoolNumber = school.SkoleSkolenummer;

            var elevListe = new List<string>();

            var larerListe = new List<string>();

            return new EduGroup
            {
                GruppeSystemIdUri = systemIdUri,
                GruppeSystemId = systemId,
                EduGroupType = groupType,
                GruppeNavn = navn,
                GruppeBeskrivelse = beskrivelse,
                GruppePeriodeStart = periodeStart,
                GruppePeriodeSlutt = periodeSlutt,
                Grepkode = grepkode,
                GruppeElevListe = elevListe,
                GruppeLarerListe = larerListe,
                GruppeSkoleRef = schoolUri,
                GruppeSkoleSkolenummer = schoolNumber,
                Utdanningsprogram = studyProgramme
            };
        }

        internal static EduGroup Create(string programmeareaGroupUri, string programmeareaName, EduOrgUnit school)
        {
            var gruppeSystemId = programmeareaGroupUri.Split('/').Last();
            var schoolUri = school.SystemIdUri;

            var schoolNumber = school.SkoleSkolenummer;

            var elevListe = new List<string>();

            return new EduGroup
            {
                GruppeSystemIdUri = programmeareaGroupUri,
                GruppeSystemId = gruppeSystemId,
                GruppeNavn = programmeareaName,
                EduGroupType = ClassType.programmeArea,
                GruppeElevListe = elevListe,
                GruppeSkoleRef = schoolUri,
                GruppeSkoleSkolenummer = schoolNumber
            };
        }
        internal static EduGroup Create(string levelGroupUri, 
            EduOrgUnit school, 
            (List<string> studentmembers, List<string> teachermembers, List<string> basisGroupmembers) memberships)
        {
            var gruppeSystemId = levelGroupUri.Split('/').Last();
            var schoolUri = school.SystemIdUri;
            var groupName = gruppeSystemId.Split(Delimiter.levelAtSchool)[0];
            var schoolNumber = school.SkoleSkolenummer;

            var elevListe = memberships.studentmembers;
            var larerListe = memberships.teachermembers;
            var basisgruppeListe = memberships.basisGroupmembers;

            return new EduGroup
            {
                GruppeSystemIdUri = levelGroupUri,
                GruppeSystemId = gruppeSystemId,
                GruppeNavn = groupName,
                EduGroupType = ClassType.level,
                GruppeElevListe = elevListe,
                GruppeLarerListe = larerListe,
                GruppeGruppeListe = basisgruppeListe,
                GruppeSkoleRef = schoolUri,
                GruppeSkoleSkolenummer = schoolNumber
            };
        }
    }
}
