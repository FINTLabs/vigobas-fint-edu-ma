﻿// VIGOBAS Identity Management System 
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
            string schoolUri, 
            string examDate
        )
        {
            var groupUri = schoolUri + "_" + examDate;
            var systemId = schoolUri.Split('/').Last() + "_" + examDate;
            var groupName = examDate + " Alle kandidater";
            var elevListe = new List<string>();

            return new EduGroup
            {
                GruppeSystemIdUri = groupUri,
                GruppeSystemId = systemId,
                GruppeNavn = groupName,
                EduGroupType = ClassType.examGroup,
                GruppeSkoleRef = schoolUri,
                GruppeElevListe = elevListe
            };
        }
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
            string periodeStartTime = string.Empty;
            string periodeSluttTime = string.Empty;

            if (basicGroup.Periode.Count > 0)
            {
                periodeStart = basicGroup.Periode[0]?.Start.ToString(dateFormat);
                periodeSlutt = basicGroup.Periode[0]?.Slutt?.ToString(dateFormat);

                if (groupType == ClassType.examGroup)
                {
                    periodeStartTime = basicGroup.Periode[0]?.Start.ToString(hourFormat);
                    periodeSluttTime = basicGroup.Periode[0]?.Slutt?.ToString(hourFormat);
                }
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
                GruppePeriodeStartTime = periodeStartTime,
                GruppePeriodeSluttTime = periodeSluttTime,
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
    }
}
