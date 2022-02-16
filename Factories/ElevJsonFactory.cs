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
using FINT.Model.Felles;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Utdanning.Elev;
using HalClient.Net.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static VigoBAS.FINT.Edu.Constants;


namespace VigoBAS.FINT.Edu
{
    class ElevJsonFactory
    {
        public static JObject Update(Elev elev, string csAttributeName, string csAttributeValue )
        {

            // see https://weblog.west-wind.com/posts/2012/Aug/30/Using-JSONNET-for-dynamic-JSON-parsing
            // for creating json objects

            var jObject = new JObject();

            switch (csAttributeName)
            {
                case CSAttribute.ElevFeidenavn:
                    {
                        dynamic jValue = new JObject();
                        jValue.identifikatorverdi = csAttributeValue;
                        jObject.Add(FintAttribute.feidenavn, jValue);
                        break;
                    }
                case CSAttribute.ElevBrukernavn:
                    {
                        break;
                    }
            }
            if (!csAttributeName.Equals(CSAttribute.ElevFeidenavn))
            {
                var feidenavn = elev?.Feidenavn;
                if (feidenavn!=null)
                {
                    var jValue = GetJsonIdentifikator(feidenavn);
                    jObject.Add(FintAttribute.feidenavn, jValue);
                }
              
            }
            var brukernavn = elev?.Brukernavn;
            if (brukernavn != null)
            {
                var jValue = GetJsonIdentifikator(brukernavn);
                jObject.Add(FintAttribute.brukernavn, jValue);
            }
            var systemId = elev?.SystemId;
            if (systemId != null)
            {
                var jValue = GetJsonIdentifikator(systemId);
                jObject.Add(FintAttribute.systemId, jValue);
            }
            var elevnummer = elev?.Elevnummer;
            if (elevnummer != null)
            {
                var jValue = GetJsonIdentifikator(elevnummer);
                jObject.Add(FintAttribute.elevnummer, jValue);
            }
            var kontaktinformasjon = elev?.Kontaktinformasjon;
            if(kontaktinformasjon != null)
            {
                var jValue = GetJsonKontaktinformasjon(kontaktinformasjon);
                jObject.Add(FintAttribute.kontaktinformasjon, jValue);
            }

            return jObject;
        }

        private static JObject GetJsonKontaktinformasjon (Kontaktinformasjon kontaktinformasjon)
        {
            dynamic kontaktinformasjonValue = new JObject();

            var epostadresse = kontaktinformasjon?.Epostadresse;
            if (!string.IsNullOrEmpty(epostadresse))
            {
                kontaktinformasjonValue.epostadresse = epostadresse;
            }

            var mobiltelefonnummer = kontaktinformasjon?.Mobiltelefonnummer;
            if (!string.IsNullOrEmpty(mobiltelefonnummer))
            {
                kontaktinformasjonValue.mobiltelefonnummer = mobiltelefonnummer;
            }

            var nettsted = kontaktinformasjon?.Nettsted;
            if (!string.IsNullOrEmpty(nettsted))
            {
                kontaktinformasjonValue.nettsted = nettsted;
            }

            var sip = kontaktinformasjon?.Sip;
            if (!string.IsNullOrEmpty(sip))
            {
                kontaktinformasjonValue.sip = sip;
            }

            var telefonnummer = kontaktinformasjon?.Telefonnummer;
            if (!string.IsNullOrEmpty(telefonnummer))
            {
                kontaktinformasjonValue.telefonnummer = telefonnummer;
            }

            return kontaktinformasjonValue;
        }

        private static JObject GetJsonIdentifikator(Identifikator identifikator)
        {
            var identifikatorverdi = identifikator.Identifikatorverdi;

            dynamic identifikatorValue = new JObject();

            identifikatorValue.identifikatorverdi = identifikatorverdi;

            var gyldighetsperiodeStart = identifikator?.Gyldighetsperiode?.Start.ToString();
            var gyldighetsperiodeSlutt = identifikator?.Gyldighetsperiode?.Slutt.ToString();

            dynamic gyldighetsperiodeValue = new JObject();

            if (!string.IsNullOrEmpty(gyldighetsperiodeStart))
            {
                gyldighetsperiodeValue.start = gyldighetsperiodeStart;
            }
            if (!String.IsNullOrEmpty(gyldighetsperiodeSlutt))
            {
                gyldighetsperiodeValue.slutt = gyldighetsperiodeSlutt;
            }
            if (!string.IsNullOrEmpty(gyldighetsperiodeStart))
            {
                identifikatorValue.gyldighetsperiode = gyldighetsperiodeValue;
            }

            return identifikatorValue;
    }

      
    }
}
