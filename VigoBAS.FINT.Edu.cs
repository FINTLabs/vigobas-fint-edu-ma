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

using FINT.Model.Administrasjon.Organisasjon;
using FINT.Model.Administrasjon.Personal;
using FINT.Model.Felles;
using FINT.Model.Felles.Kompleksedatatyper;
using FINT.Model.Resource;
using FINT.Model.Utdanning.Basisklasser;
using FINT.Model.Utdanning.Elev;
using FINT.Model.Utdanning.Utdanningsprogram;
using FINT.Model.Utdanning.Vurdering;
using HalClient.Net;
using HalClient.Net.Parser;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.MetadirectoryServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Vigo.Bas.ManagementAgent.Log;
using VigoBAS.FINT.Edu.Utilities;
using static VigoBAS.FINT.Edu.Constants;
using static VigoBAS.FINT.Edu.Utilities.Tools;

namespace VigoBAS.FINT.Edu
{
    public class EzmaExtension :
    IMAExtensible2CallExport,
    IMAExtensible2CallImport,
    IMAExtensible2GetSchema,
    IMAExtensible2GetCapabilities,
    IMAExtensible2GetParameters

    {
        private KeyedCollection<string, ConfigParameter> _exportConfigParameters;
        private KeyedCollection<string, ConfigParameter> _importConfigParameters;

        //For export for now maybe merge with elevDict in import
        private Dictionary<string, IEmbeddedResourceObject> _resourceDict = new Dictionary<string, IEmbeddedResourceObject>();

        private Dictionary<string, IEmbeddedResourceObject> _elevPersonDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _elevDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _elevforholdDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _undervisningsforholdDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _skoleressursDict = new Dictionary<string, IEmbeddedResourceObject>();

        private Dictionary<string, IEmbeddedResourceObject> _basisgruppeDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _basisgruppeMedlemskapDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _kontaktlarergruppeDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _kontaktlarergruppeMedlemskapDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _undervisningsgruppeDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _undervisningsgruppeMedlemskapDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _eksamensgruppeDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, List<String>> _basicGroupAndValidStudentRelationships = new Dictionary<string, List<string>>();
        private Dictionary<string, List<String>> _contactGroupAndValidStudentRelationships = new Dictionary<string, List<string>>();
        private Dictionary<string, List<String>> _studyGroupAndValidStudentRelationships = new Dictionary<string, List<string>>();
        private Dictionary<string, List<String>> _examGroupAndValidStudentRelationships = new Dictionary<string, List<string>>();
        private Dictionary<string, IEmbeddedResourceObject> _fagDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _arstrinnDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _programomradeDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _utdanningsprogramDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _skoleDict = new Dictionary<string, IEmbeddedResourceObject>();

        private Dictionary<string, IEmbeddedResourceObject> _ansattPersonDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _personalressursDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _arbeidsforholdDict = new Dictionary<string, IEmbeddedResourceObject>();
        private Dictionary<string, IEmbeddedResourceObject> _organisasjonselementDict = new Dictionary<string, IEmbeddedResourceObject>();

        private Dictionary<string, string> _orgelementIdMappingDict = new Dictionary<string, string>();
        private Dictionary<string, string> _skoleIdMappingDict = new Dictionary<string, string>();
        private Dictionary<string, string> _elevIdMappingDict = new Dictionary<string, string>();
        private Dictionary<string, string> _skoleressursIdMappingDict = new Dictionary<string, string>();

        private Dictionary<string, Grepkode> _grepkodeDict = new Dictionary<string, Grepkode>();

        #region Page Size

        public int ImportMaxPageSize { get; } = 50;
        public int ImportDefaultPageSize { get; } = 12;
        public int ExportDefaultPageSize { get; set; } = 10;
        public int ExportMaxPageSize { get; set; } = 20;

        #endregion
        //
        // Constructor
        //
        public EzmaExtension()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public MACapabilities Capabilities
        {
            get
            {
                MACapabilities myCapabilities = new MACapabilities
                {
                    ConcurrentOperation = true,
                    ObjectRename = false,
                    DeleteAddAsReplace = false,
                    DeltaImport = true,
                    FullExport = true,
                    DistinguishedNameStyle = MADistinguishedNameStyle.None,
                    ExportType = MAExportType.ObjectReplace,
                    //ExportType = MAExportType.AttributeUpdate;
                    //ExportType = MAExportType.AttributeReplace,
                    NoReferenceValuesInFirstExport = false,
                    Normalizations = MANormalizations.None,
                    ObjectConfirmation = MAObjectConfirmation.NoDeleteConfirmation
                };

                return myCapabilities;
            }
        }

        public IList<ConfigParameterDefinition> GetConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            List<ConfigParameterDefinition> configParametersDefinitions = new List<ConfigParameterDefinition>();

            switch (page)
            {
                case ConfigParameterPage.Connectivity:
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.username, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateEncryptedStringParameter(Param.password, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.clientId, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateEncryptedStringParameter(Param.openIdSecret, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.scope, String.Empty, FintValue.scope));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.idpUri, String.Empty, FintValue.accessTokenUri));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.assetId, String.Empty));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.felleskomponentUri, String.Empty, FintValue.felleskomponentUri));

                    break;

                case ConfigParameterPage.Global:

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("HTTP klientinnstillinger"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.httpClientTimeout, String.Empty, FintValue.httpClientTimeout));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.useLocalCache, false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.abortIfDownloadError, true));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.abortIfResourceTypeEmpty, true));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Organisasjonsinfo Feide"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.organisasjonsIdTopOrgElement, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.organisasjonsnummer, String.Empty, String.Empty));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Importparametre"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateTextParameter(Param.schoolnumbersToImport, string.Empty));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.importStudentsWithoutStudyRelationShip, false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Parametre elevforhold"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.daysBeforeStudentStarts, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.daysBeforeStudentEnds, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.importPurePrivateStudents, false));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Parametre grupper"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.daysBeforeGroupStarts, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.daysBeforeGroupEnds, String.Empty, String.Empty));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Aggregerte grupper"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.generateAggrStudentGroups, false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.generateAggrFacultyGroups, false));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.generateAggrEmployeeGroups, false));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Parametre eksamen"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.examCategoriesToImport, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.examCategoriesToAggregatePerDate, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.examgroupsVisibleFromDate, String.Empty, String.Empty));
                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.examPeriodStartDate, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.examgroupsVisibleToDate, String.Empty, String.Empty));

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateDividerParameter());

                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Parametre ansatte"));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.daysBeforeEmploymentStarts, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.daysBeforeEmploymentEnds, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.excludedResourceTypes, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.excludedEmploymentTypes, String.Empty, String.Empty));
                    configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.excludedPositionCodes, String.Empty, String.Empty));

                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateCheckBoxParameter(Param.generateProgAreaGroups, false));

                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateLabelParameter("Eksportparametre"));
                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.waitTime, String.Empty, String.Empty));
                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.lowerLimit, String.Empty, String.Empty));
                    //configParametersDefinitions.Add(ConfigParameterDefinition.CreateStringParameter(Param.upperLimit, String.Empty, String.Empty));

                    break;

                case ConfigParameterPage.Partition:
                case ConfigParameterPage.RunStep:
                    break;
            }

            return configParametersDefinitions;
        }


        public ParameterValidationResult ValidateConfigParameters(KeyedCollection<string, ConfigParameter> configParameters, ConfigParameterPage page)
        {
            // Configuration validation
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ParameterValidationResult myResults = new ParameterValidationResult();
            var TokenResult = TokenResponseHelper(configParameters);

            if (TokenResult.HttpStatusCode == HttpStatusCode.OK)
            {
                myResults.Code = ParameterValidationResultCode.Success;
            }
            else
            {
                myResults.Code = ParameterValidationResultCode.Failure;
                myResults.ErrorMessage = TokenResult.ErrorDescription;
            }

            return myResults;
        }

        public Schema GetSchema(KeyedCollection<string, ConfigParameter> configParameters)
        {
            SchemaType eduPerson = SchemaType.Create(CSObjectType.eduPerson, true);

            eduPerson.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(CSAttribute.ElevPersonalSystemId, AttributeType.String));

            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevSystemId, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevSystemIdUri, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevElevnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevBrukernavn, AttributeType.String, AttributeOperation.ImportExport));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevFeidenavn, AttributeType.String, AttributeOperation.ImportExport));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevKontaktinformasjonEpostadresse, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevKontaktinformasjonMobiltelefonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonBilde, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonFodselsdato, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonFodselsnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonNavnFornavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonNavnMellomnavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonNavnEtternavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.PersonKontaktinformasjonBostedsadresseAdresselinje, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonKontaktinformasjonBostedsadressePostnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonKontaktinformasjonBostedsadressePoststed, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.PersonKontaktinformasjonPostadresseAdresselinje, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonKontaktinformasjonPostadressePostnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonKontaktinformasjonPostadressePoststed, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonKontaktinformasjonMobiltelefonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonKontaktinformasjonEpostadresse, AttributeType.String, AttributeOperation.ImportOnly));

            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevforholdMedlemskap, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevforholdBasisgruppe, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevforholdBasisgruppeRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevforholdKontaktlarergruppe, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevforholdUndervisningsgruppe, AttributeType.Reference, AttributeOperation.ImportOnly));//
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevforholdEksamensgruppe, AttributeType.Reference, AttributeOperation.ImportOnly));//
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevforholdSkole, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevforholdKategori, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdHovedkategori, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdProgramomrade, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleressursSystemIdUri, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleressursFeidenavn, AttributeType.String, AttributeOperation.ImportExport));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonalBrukernavn, AttributeType.String, AttributeOperation.ImportExport));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonalAnsattnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonalSystemId, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonalKontaktinformasjonEpostadresse, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.PersonalKontaktinformasjonMobiltelefonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.PersonalKontaktinformasjonSip, AttributeType.String, AttributeOperation.ImportOnly));

            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.UndervisningsforholdSkole, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.UndervisningsforholdMedlemskap, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.AnsettelsesforholdSkole, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.RolleOgSkole, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.ElevkategoriOgSkole, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.Eksamensdatoer, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.AntallEksamener, AttributeType.Integer, AttributeOperation.ImportOnly));

            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.EduPersonEntitlement, AttributeType.String, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.EduPersonOrgDN, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.EduPersonPrimaryOrgUnitDN, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduPerson.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.EduPersonOrgUnitDN, AttributeType.Reference, AttributeOperation.ImportOnly));

            // Lag CS type Gruppe
            SchemaType eduGroup = SchemaType.Create(CSObjectType.eduGroup, true);

            // Anchor
            eduGroup.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(CSAttribute.GruppeSystemIdUri, AttributeType.String));

            // Attribute
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppeSystemId, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.EduGroupType, AttributeType.String, AttributeOperation.ImportOnly)); //
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.EduGroupExamCategory, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.EduGroupExamDate, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppeNavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppeBeskrivelse, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppePeriodeStart, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppePeriodeSlutt, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppePeriodeStartTime, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppePeriodeSluttTime, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppeSkoleRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppeSkoleSkolenummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.GruppeElevListe, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppeElevAntall, AttributeType.Integer, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.GruppeLarerListe, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.GruppeLarerOgElevListe, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.GruppeGruppeListe, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GruppeFagRef, AttributeType.Reference, AttributeOperation.ImportOnly));

            SchemaType eduOrgUnit = SchemaType.Create(CSObjectType.eduOrgUnit, true);

            // Anchor
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(CSAttribute.systemIdUri, AttributeType.String));

            // Attributes
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleSystemId, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleOrganisasjonsnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleSkolenummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleNavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleDomenenavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleJuridiskNavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleOrganisasjonsnavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.SkoleForretningsadresseAdresselinje, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleForretningsadressePostnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleForretningsadressePoststed, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleKontaktinformasjonEpostadresse, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleKontaktinformasjonTelefonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleKontaktinformasjonMobiltelefonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleKontaktinformasjonNettsted, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleKontaktinformasjonSip, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.SkoleElevforhold, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.SkoleUndervisningsforhold, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.SkoleAnsettelsesforhold, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.Skoleeier, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduOrgUnit.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SkoleOrganisasjonselement, AttributeType.Reference, AttributeOperation.ImportOnly));


            SchemaType eduOrg = SchemaType.Create(CSObjectType.eduOrg, true);

            // Anchor
            eduOrg.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(CSAttribute.OrganisasjonOrganisasjonsIdUri, AttributeType.String));

            // Attributes
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonOrganisasjonsId, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonOrganisasjonsnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonOrganisasjonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonNavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonDomenenavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonJuridiskNavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonOrganisasjonsnavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.OrganisasjonForretningsadresseAdresselinje, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonForretningsadressePostnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonForretningsadressePoststed, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonKontaktinformasjonEpostadresse, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonKontaktinformasjonTelefonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonKontaktinformasjonMobiltelefonnummer, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonKontaktinformasjonNettsted, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrg.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.OrganisasjonKontaktinformasjonSip, AttributeType.String, AttributeOperation.ImportOnly));

            SchemaType eduOrgUnitGroup = SchemaType.Create(CSObjectType.eduOrgUnitGroup, true);

            eduOrgUnitGroup.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(CSAttribute.GroupId, AttributeType.String));

            eduOrgUnitGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GroupName, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnitGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.GroupType, AttributeType.String, AttributeOperation.ImportOnly));
            eduOrgUnitGroup.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.SchoolRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduOrgUnitGroup.Attributes.Add(SchemaAttribute.CreateMultiValuedAttribute(CSAttribute.GroupMembers, AttributeType.Reference, AttributeOperation.ImportOnly));

            SchemaType eduSubject = SchemaType.Create(CSObjectType.eduSubject, true);

            eduSubject.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(CSAttribute.FagSystemIdUri, AttributeType.String));

            eduSubject.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.FagSystemId, AttributeType.String, AttributeOperation.ImportOnly));
            eduSubject.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.FagNavn, AttributeType.String, AttributeOperation.ImportOnly));
            eduSubject.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.FagBeskrivelse, AttributeType.String, AttributeOperation.ImportOnly));

            SchemaType eduStudentRelationship = SchemaType.Create(CSObjectType.eduStudentRelationship, true);

            eduStudentRelationship.Attributes.Add(SchemaAttribute.CreateAnchorAttribute(CSAttribute.ElevforholdSystemIdUri, AttributeType.String));

            eduStudentRelationship.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdSystemId, AttributeType.String, AttributeOperation.ImportOnly));
            eduStudentRelationship.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdGyldighetsperiodeStart, AttributeType.String, AttributeOperation.ImportOnly));
            eduStudentRelationship.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdGyldighetsperiodeSlutt, AttributeType.String, AttributeOperation.ImportOnly));
            eduStudentRelationship.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdHovedskole, AttributeType.Boolean, AttributeOperation.ImportOnly));
            eduStudentRelationship.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdElevkategori, AttributeType.String, AttributeOperation.ImportOnly));
            eduStudentRelationship.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdElevRef, AttributeType.Reference, AttributeOperation.ImportOnly));
            eduStudentRelationship.Attributes.Add(SchemaAttribute.CreateSingleValuedAttribute(CSAttribute.ElevforholdSkoleRef, AttributeType.Reference, AttributeOperation.ImportOnly));

            // Return schema
            Schema schema = Schema.Create();
            schema.Types.Add(eduPerson);
            schema.Types.Add(eduGroup);
            schema.Types.Add(eduSubject);
            schema.Types.Add(eduOrgUnit);
            schema.Types.Add(eduOrg);
            schema.Types.Add(eduOrgUnitGroup);
            schema.Types.Add(eduStudentRelationship);
            return schema;
        }

        #region Import methods

        List<ImportListItem> ImportedObjectsList = new List<ImportListItem>();
        int GetImportEntriesIndex, GetImportEntriesPageSize;

        public OpenImportConnectionResults OpenImportConnection(KeyedCollection<string, ConfigParameter> configParameters,
                                                Schema types,
                                                OpenImportConnectionRunStep importRunStep)
        {
            _importConfigParameters = configParameters;

            var elevPersonUri = FintValue.utdanningElevPersonUri;
            var elevUri = FintValue.utdanningElevElevUri;
            var elevforholdUri = FintValue.utdanningElevElevforholdUri;
            var undervisningsforholdUri = FintValue.utdanningElevUndervisningsforholdUri;
            //var medlemskapUri = DefaultValue.utdanningElevMedlemskapUri;
            var skoleressursUri = FintValue.utdanningElevSkoleressursUri;
            var basisgruppeUri = FintValue.utdanningElevBasisgruppeUri;
            var basisgruppeMedlemskapUri = FintValue.utdanningElevBasisgruppeMedlemskapUri;
            var kontaktlarergruppeUri = FintValue.utdanningElevKontaktlarergruppeUri;
            var kontaktlarergruppeMedlemskapUri = FintValue.utdanningElevKontaktlarergruppeMedlemskapUri;
            var undervisningsgruppeUri = FintValue.utdanningTimeplanUndervisningsgruppeUri;
            var undervisningsgruppeMedlemskapUri = FintValue.utdanningTimeplanUndervisningsgruppeMedlemskapUri;
            var eksamensgruppeUri = FintValue.utdanningVurderingEksamensgruppeUri;
            var eksamensgruppeMedlemskapUri = FintValue.utdanningVurderingEksamensgruppeMedlemskapUri;
            var fagUri = FintValue.utdanningTimeplanFagUri;
            var arstrinnUri = FintValue.utdanningUtdanningsprogramArstrinnUri;
            var programomradeUri = FintValue.utdanningUtdanningsprogramProgramomradeUri;
            var utdanningsprogramUri = FintValue.utdanningUtdanningsprogramUtdanningsprogramUri;
            var skoleUri = FintValue.utdanningUtdanningsprogramSkoleUri;
            var ansattPersonUri = FintValue.administrasjonPersonalPersonUri;
            var terminUri = FintValue.utdanningKodeverkTerminUri;
            var skolearUri = FintValue.utdanningKodeverkSkolearUri;
            var personalRessursUri = FintValue.administrasjonPersonalPersonalRessursUri;
            var arbeidsforholdUri = FintValue.administrasjonPersonalArbeidsforholdUri;
            var organisasjonselementUri = FintValue.administrasjonOrganisasjonOrganisasjonselementUri;

            var felleskomponentUri = configParameters[Param.felleskomponentUri].Value;
            var organisasjonsnummer = configParameters[Param.organisasjonsnummer].Value;

            bool abortIfResourceTypeEmpty = configParameters[Param.abortIfResourceTypeEmpty].Value == "1";
            //bool importStudentsWithoutStudyRelationShip = configParameters[Param.importStudentsWithoutStudyRelationShip].Value == "1";
            bool importStudentsWithoutStudyRelationShip = false;

            var periodComponentList = new List<string>() { terminUri, skolearUri };
            Dictionary<string, Periode> periodResourceDict = GetTerminAndSkolearPerioder(configParameters, periodComponentList);
            var groupPeriodDict = new Dictionary<string, Periode>();

            var componentList = new List<string>() {  elevPersonUri, elevUri, elevforholdUri, undervisningsforholdUri,
                                                skoleressursUri, basisgruppeUri,  kontaktlarergruppeUri,
                                                undervisningsgruppeUri, fagUri, arstrinnUri, utdanningsprogramUri, programomradeUri, eksamensgruppeUri,
                                                basisgruppeMedlemskapUri, kontaktlarergruppeMedlemskapUri, undervisningsgruppeMedlemskapUri, eksamensgruppeMedlemskapUri,
                                                skoleUri, ansattPersonUri, personalRessursUri, arbeidsforholdUri, organisasjonselementUri};

            var groupResourceUris = new List<string>() { basisgruppeUri, kontaktlarergruppeUri,
                                                undervisningsgruppeUri, eksamensgruppeUri };

            var itemsCountPerComponent = new Dictionary<string, int>();

            foreach (var component in componentList)
            {
                itemsCountPerComponent.Add(component, 0);
            }

            var resourceDict = new Dictionary<string, IEmbeddedResourceObject>();
            resourceDict = GetDataFromFINTApi(configParameters, componentList);

            string startStudentValue = _importConfigParameters[Param.daysBeforeStudentStarts].Value;
            int daysBeforeStudentStarts = (string.IsNullOrEmpty(startStudentValue)) ? 0 : Int32.Parse(startStudentValue);
            string endStudentValue = _importConfigParameters[Param.daysBeforeStudentEnds].Value;
            int daysBeforeStudentEnds = (string.IsNullOrEmpty(endStudentValue)) ? 0 : Int32.Parse(endStudentValue);

            string startGroupValue = _importConfigParameters[Param.daysBeforeGroupStarts].Value;
            int daysBeforeGroupStarts = (string.IsNullOrEmpty(startGroupValue)) ? 0 : Int32.Parse(startGroupValue);
            string endGroupValue = _importConfigParameters[Param.daysBeforeGroupEnds].Value;
            int daysBeforeGroupEnds = (string.IsNullOrEmpty(endGroupValue)) ? 0 : Int32.Parse(endGroupValue);

            string examCategoriesToImport = _importConfigParameters[Param.examCategoriesToImport].Value;
            string[] examCategoriesToImportList = (string.IsNullOrEmpty(examCategoriesToImport)) ? null : examCategoriesToImport.Split(',');

            string examCategoriesToAggregatePerDate = _importConfigParameters[Param.examCategoriesToAggregatePerDate].Value;
            string[] examCategoriesToAggregatePerDateList = (string.IsNullOrEmpty(examCategoriesToAggregatePerDate)) ? null : examCategoriesToAggregatePerDate.Split(',');

            string examgroupsVisibleFromDateValue = (string.IsNullOrEmpty(_importConfigParameters[Param.examgroupsVisibleFromDate].Value)) ? zeroDate: _importConfigParameters[Param.examgroupsVisibleFromDate].Value;
            DateTime examgroupsVisibleFromDate = DateTime.Parse(examgroupsVisibleFromDateValue);

            string examgroupsVisibleToDateValue = (string.IsNullOrEmpty(_importConfigParameters[Param.examgroupsVisibleToDate].Value)) ? infinityDate : _importConfigParameters[Param.examgroupsVisibleToDate].Value;
            DateTime examgroupsVisibleToDate = DateTime.Parse(examgroupsVisibleToDateValue).AddDays(1);

            var examGroupsIsVisible = ExamgroupsShouldBeVisible(examgroupsVisibleFromDate, examgroupsVisibleToDate);

            foreach (var uriKey in resourceDict.Keys)
            {
                var resourceType = GetUriPathForClass(uriKey);

                itemsCountPerComponent[resourceType]++;

                bool isExamGroupMembership = resourceType.Equals(FintValue.utdanningVurderingEksamensgruppeMedlemskapUri) ? true : false;

                if (groupResourceUris.Contains(resourceType))
                {
                    if (resourceDict.TryGetValue(uriKey, out IEmbeddedResourceObject gruppeResource))
                    {
                        var periode = new Periode();
                        var periodeSet = false;
                        if (gruppeResource.Links.TryGetValue(ResourceLink.termin, out IEnumerable<ILinkObject> terminLinks))
                        {
                            var terminPeriode = GetPeriodeFromLinks(terminLinks, periodResourceDict);

                            if (terminPeriode != null)
                            {
                                periode = terminPeriode;
                                periodeSet = true;
                            }
                        }
                        if (periode.Start == null && gruppeResource.Links.TryGetValue(ResourceLink.skolear, out IEnumerable<ILinkObject> skolearLink))
                        {
                            Logger.Log.InfoFormat("Gruppe {0} does not have valid termin info. Trying to set periode based on skolear", uriKey);
                            periode = GetPeriodeFromLinks(skolearLink, periodResourceDict);
                            periodeSet = true;
                        }

                        if (periodeSet)
                        {
                            if (PeriodIsValid(periode, daysBeforeGroupStarts, daysBeforeGroupEnds))
                            {
                                groupPeriodDict.Add(uriKey, periode);

                                switch (resourceType)
                                {
                                    case FintValue.utdanningElevBasisgruppeUri:
                                        {
                                            _basisgruppeDict.Add(uriKey, gruppeResource);
                                            break;

                                        }
                                    case FintValue.utdanningElevKontaktlarergruppeUri:
                                        {
                                            _kontaktlarergruppeDict.Add(uriKey, gruppeResource);
                                            break;
                                        }
                                    case FintValue.utdanningTimeplanUndervisningsgruppeUri:
                                        {
                                            _undervisningsgruppeDict.Add(uriKey, gruppeResource);
                                            break;
                                        }
                                    case FintValue.utdanningVurderingEksamensgruppeUri:
                                        {
                                            if (examGroupsIsVisible)
                                            {
                                                _eksamensgruppeDict.Add(uriKey, gruppeResource);
                                            }
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                Logger.Log.Debug($"Gruppe {uriKey} does not have a valid period");
                            }
                        }
                        else
                        {
                            Logger.Log.InfoFormat("Gruppe {0} does not have termin or skolear info. Gruppe not imported to CS", uriKey);
                        }
                    }
                }


                switch (resourceType)
                    {
                    case FintValue.utdanningElevUndervisningsforholdUri:
                    {
                        _undervisningsforholdDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                    }
                    case FintValue.utdanningElevSkoleressursUri:
                    {
                        var skoleressursResource = resourceDict[uriKey];

                        var systemIdUri = GetSystemIdUri(skoleressursResource, felleskomponentUri);

                        if (!_skoleressursDict.TryGetValue(systemIdUri, out IEmbeddedResourceObject existingSkoleressursResource))
                        {
                            _skoleressursDict.Add(systemIdUri, skoleressursResource);
                            UpdateResourceIdMappingDict(systemIdUri, skoleressursResource, ref _skoleressursIdMappingDict);
                        }
                        else
                        {
                            Logger.Log.ErrorFormat("Duplicate systemid {0} in skoleressurs items. Something is wrong in source system", systemIdUri);
                        }
                            break;
                    }
                    case FintValue.utdanningElevPersonUri:
                    {
                        _elevPersonDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                    }
                    case FintValue.utdanningElevElevUri:
                    {
                        var elevResource = resourceDict[uriKey];

                        var systemIdUri = GetSystemIdUri(elevResource, felleskomponentUri);

                        if (!_elevDict.TryGetValue(systemIdUri, out IEmbeddedResourceObject existingElevResource))
                        {
                            _elevDict.Add(systemIdUri, elevResource);

                            UpdateResourceIdMappingDict(systemIdUri, elevResource, ref _elevIdMappingDict);
                        }
                        else
                        {
                            Logger.Log.ErrorFormat("Duplicate systemid {0} in elev items. Something is wrong in source system", systemIdUri);
                        }
                            break;
                    }
                    case FintValue.utdanningElevElevforholdUri:
                    {
                        if (resourceDict.TryGetValue(uriKey, out IEmbeddedResourceObject elevforholdResource))
                        {
                            bool relationshipIsValid = true;

                            if (elevforholdResource.State.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue periodeValue))
                            {
                                relationshipIsValid = PeriodIsValid(periodeValue, daysBeforeStudentStarts, daysBeforeStudentEnds);
                            }
                            if (relationshipIsValid)
                            {
                                _elevforholdDict.Add(uriKey, elevforholdResource);
                            }
                            else if (periodeValue != null)
                            {
                                Logger.Log.DebugFormat("Elevforhold {0} does not have a valid period", uriKey);
                            }
                        }
                            break;
                        }
                    case FintValue.utdanningTimeplanFagUri:
                        {
                            _fagDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                        }
                    case FintValue.utdanningUtdanningsprogramArstrinnUri:
                        {
                            _arstrinnDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                        }
                    case FintValue.utdanningUtdanningsprogramProgramomradeUri:
                        {
                            _programomradeDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                        }
                    case FintValue.utdanningUtdanningsprogramUtdanningsprogramUri:
                        {
                            _utdanningsprogramDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                        }
                    case FintValue.utdanningElevBasisgruppeMedlemskapUri:
                    {
                        AddValidMembership(uriKey, resourceDict, daysBeforeStudentStarts, daysBeforeStudentEnds, isExamGroupMembership, ResourceLink.basicGroup, ref _basicGroupAndValidStudentRelationships);
                            break;
                    }
                    case FintValue.utdanningElevKontaktlarergruppeMedlemskapUri:
                    {
                        AddValidMembership(uriKey, resourceDict, daysBeforeStudentStarts, daysBeforeStudentEnds, isExamGroupMembership, ResourceLink.contactTeacherGroup, ref _contactGroupAndValidStudentRelationships);;
                            break;
                    }
                    case FintValue.utdanningTimeplanUndervisningsgruppeMedlemskapUri:
                    {
                        AddValidMembership(uriKey, resourceDict, daysBeforeStudentStarts, daysBeforeStudentEnds, isExamGroupMembership, ResourceLink.studyGroup, ref _studyGroupAndValidStudentRelationships);
                            break;
                    }
                    case FintValue.utdanningVurderingEksamensgruppeMedlemskapUri:
                        {
                            if (examGroupsIsVisible)
                            {
                                AddValidMembership(uriKey, resourceDict, daysBeforeStudentStarts, daysBeforeStudentEnds, isExamGroupMembership, ResourceLink.examGroup, ref _examGroupAndValidStudentRelationships);
                            }
                            break;
                        }
                    case FintValue.utdanningUtdanningsprogramSkoleUri:
                    {
                        var schoolResource = resourceDict[uriKey];
                        var systemIdUri = GetSystemIdUri(schoolResource, felleskomponentUri);

                        _skoleDict.Add(systemIdUri, schoolResource);
                        UpdateResourceIdMappingDict(systemIdUri, schoolResource, ref _skoleIdMappingDict);
                            break;
                    }
                    case FintValue.administrasjonPersonalPersonUri:
                    {
                        _ansattPersonDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                    }
                    case FintValue.administrasjonPersonalPersonalRessursUri:
                    {
                        _personalressursDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                    }
                    case FintValue.administrasjonPersonalArbeidsforholdUri:
                    {
                        _arbeidsforholdDict.Add(uriKey, resourceDict[uriKey]);
                            break;
                    }
                    case FintValue.administrasjonOrganisasjonOrganisasjonselementUri:
                    {
                        if (resourceDict.TryGetValue(uriKey, out IEmbeddedResourceObject orgElementObject))
                        {
                            _organisasjonselementDict.Add(uriKey, orgElementObject);

                            UpdateResourceIdMappingDict(uriKey, orgElementObject, ref _orgelementIdMappingDict);
                        }
                            break;
                    }
                }


            }

            foreach (var resource in itemsCountPerComponent.Keys)
            {
                var noOfItems = itemsCountPerComponent[resource];
                if (noOfItems == 0)
                {
                    if (abortIfResourceTypeEmpty)
                    {
                        var message = string.Format("No items returned for resource {0}. Aborting the import", resource);
                        Logger.Log.Error(message);

                        throw new FINTResourceEmptyException(message);
                    }
                    else
                    {
                        var message = string.Format("No items returned for resource {0}. Continuing because abortIfResourceTypeEmpty is set to false in MA config", resource);
                        Logger.Log.Info(message);
                    }
                }
                else
                {
                    var message = string.Format("{0} items returned for resource {1}", noOfItems.ToString(), resource);
                    Logger.Log.Info(message);
                }
            }
            //unneccesary?
            resourceDict = null;

            var grepHttpClient = new HttpClient();
            var grepTypes = new List<string> { Grep.fagkoder, Grep.utdanningsprogram, Grep.programomraader, Grep.aarstrinn };

            _grepkodeDict = GetGrepCodesForGrepTypes(grepHttpClient, Grep.baseDataUrl, grepTypes);

            var ssnToSystemId = new Dictionary<string, string>();

            var importedObjectsDict = new Dictionary<string, ImportListItem>();

            var topOrgEmbeddedObject = GetTopOrgElement(_organisasjonselementDict, _importConfigParameters);

            if (topOrgEmbeddedObject == null)
            {
                string errorMessage = "Top organisasjonselement must be present on the organisasjonselement endpoint. Aborting the import";
                Logger.Log.Error(errorMessage);
                throw new FINTTopOrgelementMissingException(errorMessage);
            }

            var selfLink = topOrgEmbeddedObject.Links[ResourceLink.self];
            var organisasjonIdUri = LinkToString(selfLink);

            var organisasjon = new Organisasjonselement();
            organisasjon = OrganisasjonselementFactory.Create(topOrgEmbeddedObject.State);

            var eduOrg = EduOrgFactory.Create(organisasjonIdUri, organisasjonsnummer, organisasjon);

            importedObjectsDict.Add(organisasjonIdUri, new ImportListItem { eduOrg = eduOrg });

            var memberTypes = new List<string>();

            bool generateAggrStudentGroups = configParameters[Param.generateAggrStudentGroups].Value == "1";
            bool generateAggrFacultyGroups = configParameters[Param.generateAggrFacultyGroups].Value == "1";
            bool generateAggrEmployeeGroups = configParameters[Param.generateAggrEmployeeGroups].Value == "1";
            //bool generateProgAreaGroups = configParameters[Param.generateProgAreaGroups].Value == "1";

            if (generateAggrStudentGroups)
            {
                memberTypes.Add(ResourceLink.studentRelationship);
            }
            if (generateAggrFacultyGroups)
            {
                memberTypes.Add(ResourceLink.teachingRelationship);
            }
            if (generateAggrEmployeeGroups)
            {
                memberTypes.Add(ResourceLink.schoolresource);
            }

            foreach (var memberType in memberTypes)
            {
                var eduOrgUnitGroup = new EduOrgUnitGroup();
                eduOrgUnitGroup = EduOrgUnitGroupFactory.Create(organisasjonIdUri, eduOrg, memberType);

                importedObjectsDict.Add(eduOrgUnitGroup.GroupId, new ImportListItem { eduOrgUnitGroup = eduOrgUnitGroup });
            }
            var schoolOrgElementsTable = new HashSet<string>();
            var principalForSchoolDict = new Dictionary<string, string>();

            // Når organisasjon-relasjonen kommer fra FINT bør dette kanskje gjøres om
            foreach (var skoleDictItem in _skoleDict)
            {
                var schoolUri = skoleDictItem.Key;
                var skoleData = skoleDictItem.Value;

                //var skole = new Skole();
                //skole = SkoleFactory.Create((IReadOnlyDictionary<string, IStateValue>)skoleData.State);

                var skoleResource = new SkoleResource();
                skoleResource = SkoleResourceFactory.Create(skoleData);

                var eduOrgUnit = EduOrgUnitFactory.Create(schoolUri, skoleResource, organisasjonIdUri);

                var schoolNumber = eduOrgUnit.SkoleSkolenummer;
                var schoolnumbersToImport = configParameters[Param.schoolnumbersToImport].Value;

                if (string.IsNullOrEmpty(schoolnumbersToImport) || schoolnumbersToImport.Contains(schoolNumber))
                {
                    if (!string.IsNullOrEmpty(schoolnumbersToImport))
                    {
                        Logger.Log.InfoFormat("School number {0} for school {1} is in schools to import list and will be added to CS", schoolNumber, schoolUri);
                    }
                    try
                    {
                        importedObjectsDict.Add(schoolUri, new ImportListItem { eduOrgUnit = eduOrgUnit });

                        if (skoleResource.Links.TryGetValue(ResourceLink.organization, out List<Link> orgLink))
                        {
                            string orgLinkUri = orgLink.First().href;
                            if (_orgelementIdMappingDict.TryGetValue(orgLinkUri, out string schoolOrgElementUri))
                            {
                                schoolOrgElementsTable.Add(schoolOrgElementUri);
                                eduOrgUnit.SkoleOrganisasjonselement = schoolOrgElementUri;

                                if (_organisasjonselementDict.TryGetValue(schoolOrgElementUri, out IEmbeddedResourceObject schoolOrgElement))
                                {
                                    string principalUri = GetPrincipalForSchool(schoolOrgElement);

                                    if (!string.IsNullOrEmpty(principalUri))
                                    {
                                        principalForSchoolDict.Add(schoolOrgElementUri, principalUri);
                                        Logger.Log.DebugFormat("Leader {0} for organisasjonselement {1} found as principal for school {2}", principalUri, schoolOrgElementUri, schoolUri);
                                    }
                                    else
                                    {
                                        Logger.Log.InfoFormat("Organisasjonselement {0} for school {1} is missing leader (ie principal)", schoolOrgElementUri, schoolUri);
                                    }
                                }
                                else
                                {
                                    Logger.Log.ErrorFormat("Organisasjonselement {0} for school {1} is missing from the organisasjonselement endpoint", schoolOrgElementUri, schoolUri);
                                }
                            }
                        }
                        else
                        {
                            Logger.Log.ErrorFormat("School {0} is missing organisasjon relation", schoolUri);
                        }
                    }
                    catch (Exception e)
                    {
                        var errorMessage = e.Message;
                        Logger.Log.ErrorFormat("Failed to add {0} to importedObjectsDict. Error message: {1}", schoolUri, errorMessage);
                    }
                }
                else
                {
                    Logger.Log.InfoFormat("School number {0} for school {1} is missing from schools to import list and will not be added to CS", schoolNumber, schoolUri);
                }
            }

            Logger.Log.DebugFormat("Generate WorkplaceOrgUnitToSchoolOrgUnit table started");
            Dictionary<string, string> workplaceOrgUnitToSchoolOrgUnit = GenerateWorkplaceOrgUnitToSchoolOrgUnit(schoolOrgElementsTable);

            Logger.Log.DebugFormat("Generate WorkplaceOrgUnitToSchoolOrgUnit table ended");

            string startValue = _importConfigParameters[Param.daysBeforeEmploymentStarts].Value;
            int daysBeforeEmploymentStarts = (string.IsNullOrEmpty(startValue)) ? 0 : Int32.Parse(startValue);

            string endValue = _importConfigParameters[Param.daysBeforeEmploymentEnds].Value;
            int daysAfterEmploymentEnds = (string.IsNullOrEmpty(endValue)) ? 0 : Int32.Parse(endValue);

            HashSet<string> excludedResourceTypes = new HashSet<string>();
            var paramExcludedResourceTypes = _importConfigParameters[Param.excludedResourceTypes].Value;

            excludedResourceTypes = GetExcludedItemCodes(paramExcludedResourceTypes);

            HashSet<string> excludedEmploymentTypes = new HashSet<string>();
            var paramExcludedEmploymentTypes = _importConfigParameters[Param.excludedEmploymentTypes].Value;

            excludedEmploymentTypes = GetExcludedItemCodes(paramExcludedEmploymentTypes);

            HashSet<string> excludedPositionCodes = new HashSet<string>();
            var paramExcludedPositionCodes = _importConfigParameters[Param.excludedPositionCodes].Value;

            excludedPositionCodes = GetExcludedItemCodes(paramExcludedPositionCodes);

            foreach (var skoleDictItem in _skoleDict)
            {
                var schoolUri = skoleDictItem.Key;
                var skoleData = skoleDictItem.Value;

                if (importedObjectsDict.TryGetValue(schoolUri, out ImportListItem importListItemSchool))
                {
                    Logger.Log.InfoFormat("Adding persons and groups assosiated with school {0}", schoolUri);

                    var eduOrgUnit = importListItemSchool.eduOrgUnit;

                    var schoolName = eduOrgUnit.SkoleNavn;

                    var skoleDataLinks = skoleData.Links;

                    var groupLinks = new Collection<string> { ResourceLink.basicGroup, ResourceLink.studyGroup, ResourceLink.contactTeacherGroup, ResourceLink.examGroup };

                    var levelGroupDictionary = new Dictionary<string, (List<string> studentmembers, List<string> teachermembers, List<string>basGroupmembers)>();

                    var examGroupUriList = new List<string>();

                    foreach (var groupLink in groupLinks)
                    {
                        if (skoleDataLinks.TryGetValue(groupLink, out IEnumerable<ILinkObject> groups))
                        {
                            var gruppeDict = new Dictionary<string, IEmbeddedResourceObject>();

                            switch (groupLink)
                            {
                                case ResourceLink.basicGroup:
                                    {
                                        gruppeDict = _basisgruppeDict;
                                        break;
                                    }
                                case ResourceLink.studyGroup:
                                    {
                                        gruppeDict = _undervisningsgruppeDict;
                                        break;
                                    }
                                case ResourceLink.contactTeacherGroup:
                                    {
                                        gruppeDict = _kontaktlarergruppeDict;
                                        break;
                                    }
                                case ResourceLink.examGroup:
                                    {
                                        gruppeDict = _eksamensgruppeDict;
                                        break;
                                    }
                            }
                            foreach (var group in groups)
                            {
                                var groupUri = LinkToString(group);

                                if (groupLink == ResourceLink.examGroup)
                                {
                                    examGroupUriList.Add(groupUri);
                                }

                                if (gruppeDict.TryGetValue(groupUri, out IEmbeddedResourceObject groupData))
                                {
                                    // importNoDaysAhead, examgroupsVisibleFromDate, examgroupsVisibleToDate,
                                    HandleGroup(groupLink, groupUri, schoolUri, organisasjonIdUri, groupData, null, ref levelGroupDictionary, ref ssnToSystemId, ref importedObjectsDict, ref groupPeriodDict); ;
                                }
                            }
                        }
                        else
                        {
                            Logger.Log.InfoFormat("Data for school {0} does not contain any {1} links", schoolName, groupLink);
                        }
                    }

                    if (examGroupUriList.Count > 0)
                    {
                        foreach (var groupUri in examGroupUriList)
                        {
                            if (importedObjectsDict.TryGetValue(groupUri, out ImportListItem examgroupItem))
                            {
                                var examCategory = examgroupItem.eduGroup.Eksamensform;

                                if (examCategoriesToAggregatePerDateList.Contains(examCategory))
                                {
                                    var examdate = examgroupItem.eduGroup.Eksamensdato;

                                    var aggregatedExamgroupUri = schoolUri + '_' + examCategory + '_' + examdate;


                                    if (!importedObjectsDict.TryGetValue(aggregatedExamgroupUri, out ImportListItem dummyItem))
                                    {
                                        importedObjectsDict.Add(aggregatedExamgroupUri, new ImportListItem { eduGroup = EduGroupFactory.Create(schoolUri, examdate, examCategory) });
                                    }
                                    if (importedObjectsDict.TryGetValue(aggregatedExamgroupUri, out ImportListItem aggregatedExamgroupItem))
                                    {
                                        var examgroupMembers = examgroupItem.eduGroup.GruppeElevListe;
                                        var aggredateExamgroupMembers = aggregatedExamgroupItem.eduGroup.GruppeElevListe;


                                        foreach (var member in examgroupMembers)
                                        {
                                            Logger.Log.InfoFormat("Trying to add student {0} to exam group {1}", member, aggregatedExamgroupUri);
                                            if (!aggredateExamgroupMembers.Contains(member))
                                            {
                                                aggredateExamgroupMembers.Add(member);
                                                // add exam date to member
                                                try
                                                {                                                    
                                                    if (importedObjectsDict.TryGetValue(member, out ImportListItem memberItem))
                                                    {
                                                        var student = memberItem.eduPerson;

                                                        if (!student.Eksamensdatoer.Contains(examdate))
                                                        {
                                                            student.Eksamensdatoer.Add(examdate);
                                                            student.AntallEksamener++;
                                                            student.ElevforholdEksamensgruppe.Add(aggregatedExamgroupUri);
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Logger.Log.ErrorFormat("Adding exam date {0} to student {0} failed with expection {2}", examdate, member, e.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (levelGroupDictionary.Count > 0)
                    {
                        foreach (var levelGroupUri in levelGroupDictionary.Keys)
                        {
                            var levelGroupMembers = levelGroupDictionary[levelGroupUri];
                            var levelEduGroup = EduGroupFactory.Create(levelGroupUri, eduOrgUnit, levelGroupMembers);
                            importedObjectsDict.Add(levelGroupUri, new ImportListItem { eduGroup = levelEduGroup });
                        }
                    }
                    if (skoleDataLinks.TryGetValue(ResourceLink.studyprogramme, out IEnumerable<ILinkObject> studyprogrammes))
                    {
                        foreach (var studyprogramme in studyprogrammes)
                        {
                            var studyprogrammeUri = LinkToString(studyprogramme);

                            if (_utdanningsprogramDict.TryGetValue(studyprogrammeUri, out IEmbeddedResourceObject studyprogrammeData))
                            {
                                // 0, examgroupsVisibleFromDate, examgroupsVisibleToDate, 
                                HandleGroup(
                                    ClassType.educationProgramme, 
                                    studyprogrammeUri, 
                                    schoolUri, 
                                    organisasjonIdUri, 
                                    studyprogrammeData, 
                                    null, 
                                    ref levelGroupDictionary, 
                                    ref ssnToSystemId, 
                                    ref importedObjectsDict, 
                                    ref groupPeriodDict
                                );

                                var studyProgramme = new EduGroup();

                                if (importedObjectsDict.TryGetValue(studyprogrammeUri, out ImportListItem importListItemEduGroup))
                                {
                                    studyProgramme = importListItemEduGroup.eduGroup;
                                }

                                var studyprogrammeDataLinks = studyprogrammeData.Links;

                                if (studyprogrammeDataLinks.TryGetValue(ResourceLink.programmearea, out IEnumerable<ILinkObject> programmeareas))
                                {
                                    foreach (var programmearea in programmeareas)
                                    {
                                        var groupUri = LinkToString(programmearea);
                                        if (_programomradeDict.TryGetValue(groupUri, out IEmbeddedResourceObject groupData))
                                        {
                                            // 0, examgroupsVisibleFromDate,
                                            HandleGroup(
                                                ClassType.programmeArea, 
                                                groupUri, 
                                                schoolUri, 
                                                organisasjonIdUri, 
                                                groupData, 
                                                studyProgramme, 
                                                ref levelGroupDictionary, 
                                                ref ssnToSystemId, 
                                                ref importedObjectsDict, 
                                                ref groupPeriodDict
                                            );
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (skoleDataLinks.TryGetValue(ResourceLink.schoolresource, out IEnumerable<ILinkObject> schoolResources))
                    {
                        foreach (var schoolResource in schoolResources)
                        {
                            var schoolResourceUri = LinkToString(schoolResource);

                            if (_skoleressursDict.TryGetValue(schoolResourceUri, out IEmbeddedResourceObject schoolResourceData))
                            {
                                var schoolresourceLinks = schoolResourceData.Links;

                                bool isTeacher;

                                if (schoolresourceLinks.TryGetValue(ResourceLink.teachingRelationship, out IEnumerable<ILinkObject> dummyValue))
                                {
                                    isTeacher = true;
                                }
                                else
                                {
                                    isTeacher = false;
                                }
                                if (schoolresourceLinks.TryGetValue(ResourceLink.personalResource, out IEnumerable<ILinkObject> personalResourceLink))
                                {
                                    var schoolOrganizationElementUri = eduOrgUnit.SkoleOrganisasjonselement;

                                    if (!String.IsNullOrEmpty(schoolOrganizationElementUri))
                                    {
                                        var personalResourceUri = LinkToString(personalResourceLink);

                                        if (_personalressursDict.TryGetValue(personalResourceUri, out IEmbeddedResourceObject personalResource))
                                        {
                                            var personalResourceLinks = personalResource.Links;
                                            if (personalResourceLinks.TryGetValue(ResourceLink.resourceCategory, out IEnumerable<ILinkObject> resourceCategoryLink))
                                            {
                                                string resourceCategoryId = GetIdValueFromLink(resourceCategoryLink);

                                                if (!excludedResourceTypes.Contains(resourceCategoryId))
                                                {
                                                    // check if this schoolresource has an active emplyment at this school
                                                    Logger.Log.InfoFormat("Checking active employments for schoolresource {0} at school {1}", schoolResourceUri, schoolUri);

                                                    (bool resourceIsActive, bool isMainSchool) = CheckResourceActiveAtSchool(personalResourceUri, schoolOrganizationElementUri,
                                                        principalForSchoolDict,
                                                        workplaceOrgUnitToSchoolOrgUnit,
                                                        _personalressursDict,
                                                        _arbeidsforholdDict,
                                                        _organisasjonselementDict,
                                                        daysBeforeEmploymentStarts,
                                                        daysAfterEmploymentEnds,
                                                        excludedEmploymentTypes,
                                                        excludedPositionCodes
                                                        );
                                                    if (resourceIsActive)
                                                    {
                                                        Logger.Log.InfoFormat("Resource {0} is active at school {1}", schoolResourceUri, schoolUri);

                                                        var newResourceUri = String.Empty;

                                                        if (!importedObjectsDict.TryGetValue(schoolResourceUri, out ImportListItem dummyImportListItem))
                                                        {
                                                            newResourceUri = AddNonStudentToCS(
                                                                schoolResourceUri,
                                                                organisasjonIdUri,
                                                                schoolResourceData,
                                                                ref ssnToSystemId,
                                                                ref importedObjectsDict);
                                                        }
                                                        else
                                                        {
                                                            Logger.Log.InfoFormat("Resource {0} already exist in CS with this anchor. Adding new orgunit to resource", schoolResourceUri);
                                                            newResourceUri = schoolResourceUri;
                                                        }
                                                        if (importedObjectsDict.TryGetValue(newResourceUri, out ImportListItem eduPersonData))
                                                        {
                                                            var eduPerson = eduPersonData.eduPerson;
                                                            AddPersonToOrgUnit(ClassType.schoolresource, string.Empty, isMainSchool, ref eduPerson, ref eduOrgUnit);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.Log.InfoFormat("**EMPLOYEE-FILTER-RESOURCECATEGORY** Resource {0} is linked to personalresource {1} with resource category {2} and is therefore not added to CS", schoolResourceUri, personalResourceUri, resourceCategoryId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Logger.Log.ErrorFormat("{0} linked to from resource {1} but not found on the /administrasjon/personal/personalressurs endpoint. The adapter is supplying inconsistent data", schoolResourceUri, personalResourceUri);
                                        }

                                    }
                                    else
                                    {
                                        Logger.Log.ErrorFormat("Skole resource {0} is missing the link {1}. Active employments could not be calculated for skoleressurs resource {2}"
                                            , schoolUri, ResourceLink.organization, schoolResourceUri);
                                    }
                                }
                                else
                                {
                                    Logger.Log.ErrorFormat("Resource {0} is missing the {1} link. {1} is mandatory for skoleressurs in the FINT model.", schoolResourceUri, ResourceLink.personalResource);
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.Log.InfoFormat("Data for school {0} does not contain any {1} links", schoolName, ResourceLink.schoolresource);
                    }

                    Logger.Log.InfoFormat("Trying to add students missing group membership to CS for school {0}", schoolUri);

                    if (skoleDataLinks.TryGetValue(ResourceLink.studentRelationship, out IEnumerable<ILinkObject> studentRelationshipLinks))
                    {
                        foreach (var studentRelationshipLink in studentRelationshipLinks)
                        {
                            var uriElevforholdKey = LinkToString(studentRelationshipLink);
                            Logger.Log.Debug($"Parsing {uriElevforholdKey}");

                            if (_elevforholdDict.TryGetValue(uriElevforholdKey, out IEmbeddedResourceObject elevForholdData))
                            {
                                var elevForholdDataLinks = elevForholdData.Links;
                                var studentCategory = string.Empty;

                                if (elevForholdDataLinks.TryGetValue(ResourceLink.studentCategory, out IEnumerable<ILinkObject> catecoryLink))
                                {
                                    studentCategory = LinkToString(catecoryLink);
                                }
                                bool isMainSchool = false;

                                if (elevForholdData.State.TryGetValue(FintAttribute.hovedskole, out IStateValue hovedskoleValue))
                                {
                                    isMainSchool = Convert.ToBoolean(hovedskoleValue.Value);
                                }
                                if (elevForholdDataLinks.TryGetValue(ResourceLink.student, out IEnumerable<ILinkObject> studentLink))
                                {                                
                                    var studentUri = LinkToString(studentLink);

                                    if (_elevIdMappingDict.TryGetValue(studentUri, out string uriElevKey))
                                    {
                                        if (importedObjectsDict.TryGetValue(uriElevKey, out ImportListItem elevImportListItem))
                                        {
                                            Logger.Log.Debug($"{uriElevKey} is already added to CS. Checking if student is already related in CS to school {schoolUri}");
                                            var orgUnits = elevImportListItem.eduPerson.EduPersonOrgUnitDN;

                                            var notInUnit = true;

                                            foreach (var orgUnitRef in orgUnits)
                                            {
                                                if (orgUnitRef == schoolUri)
                                                {
                                                    notInUnit = false;
                                                }
                                            }
                                            if (notInUnit)
                                            {
                                                Logger.Log.Debug($"{uriElevKey} not yet related in CS to school {schoolUri}, relation is added now");

                                                var eduPerson = elevImportListItem.eduPerson;
                                                AddPersonToOrgUnit(ClassType.studentRelationship, studentCategory, isMainSchool, ref eduPerson, ref eduOrgUnit);
                                            }
                                        }
                                        else
                                        {
                                            Logger.Log.Info($"{uriElevKey} is not connected to any groups at school {schoolUri}. Trying to add to CS based on the {FintValue.utdanningElevElevforholdUri} endpoint");

                                            if (_elevDict.TryGetValue(uriElevKey, out IEmbeddedResourceObject elevData))
                                            {
                                                if (elevData.Links.TryGetValue(ResourceLink.person, out IEnumerable<ILinkObject> uriElevPersonLink))
                                                {
                                                    var uriElevPersonKey = LinkToString(uriElevPersonLink);
                                                    if (_elevPersonDict.TryGetValue(uriElevPersonKey, out IEmbeddedResourceObject elevPersonData))
                                                    {
                                                        var newUriKey = AddStudentToCS(
                                                            uriElevKey,
                                                            elevData,
                                                            elevPersonData,
                                                            organisasjonIdUri,
                                                            ref ssnToSystemId,
                                                            ref importedObjectsDict
                                                            );

                                                        if (importedObjectsDict.TryGetValue(newUriKey, out ImportListItem eduPersonData))
                                                        {
                                                            var eduPerson = eduPersonData.eduPerson;
                                                            AddPersonToOrgUnit(ClassType.studentRelationship, studentCategory, isMainSchool, ref eduPerson, ref eduOrgUnit);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Logger.Log.ErrorFormat("{0} linked to from resource {1} but not found on the /utdanning/elev/person endpoint. The adapter is supplying inconsistent data", uriElevPersonKey, uriElevKey);
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.Log.Error($"{uriElevKey} is missing mandatory link to: {ResourceLink.person}");
                                                }

                                            }
                                            else
                                            {
                                                Logger.Log.ErrorFormat("{0} linked to from resource {1} but not found on the /utdanning/elev/elev endpoint. The adapter is supplying inconsistent data", uriElevKey, uriElevforholdKey);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Logger.Log.Error($"{studentUri} is referenced by {studentRelationshipLink.Href} but the resource is missing on the {FintValue.utdanningElevElevUri} endpoint");
                                    }
                                }
                                else
                                {
                                    Logger.Log.Error($"Elevforhold {uriElevforholdKey} is missing mandatory link {ResourceLink.student}");
                                }
                            }
                        }
                    }
                    foreach (var memberType in memberTypes)
                    {
                        if (skoleDataLinks.TryGetValue(memberType, out IEnumerable<ILinkObject> dummyValue))
                        {
                            var eduOrgUnitGroup = new EduOrgUnitGroup();
                            eduOrgUnitGroup = EduOrgUnitGroupFactory.Create(schoolUri, eduOrgUnit, memberType);

                            importedObjectsDict.Add(eduOrgUnitGroup.GroupId, new ImportListItem { eduOrgUnitGroup = eduOrgUnitGroup });

                            var orgGroupUri = organisasjonIdUri + '_' + memberType;

                            if (importedObjectsDict.TryGetValue(orgGroupUri, out ImportListItem orgGroupItem))
                            {
                                var orgUnitGroupMembers = eduOrgUnitGroup.GroupMembers;
                                var orgGroupMembers = orgGroupItem.eduOrgUnitGroup.GroupMembers;

                                foreach (var member in orgUnitGroupMembers)
                                {
                                    if (!orgGroupMembers.Contains(member))
                                    {
                                        orgGroupMembers.Add(member);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (importStudentsWithoutStudyRelationShip)
            {
                Logger.Log.Info("Trying to add students missing in the elevforhold endpoint to CS, ie students not assosiated with a school");

                foreach (var uriElevKey in _elevDict.Keys)
                {
                    if (!importedObjectsDict.TryGetValue(uriElevKey, out ImportListItem elevImportListItem))
                    {
                        Logger.Log.InfoFormat("{0} is not linked to from a elevforhold resource. Trying to add resource to CS based on the /utdanning/elev/elev endpoint", uriElevKey);

                        if (_elevDict.TryGetValue(uriElevKey, out IEmbeddedResourceObject elevData))
                        {
                            if (elevData.Links.TryGetValue(ResourceLink.person, out IEnumerable<ILinkObject> personLink))
                            {
                                var uriElevPersonKey = LinkToString(personLink);
                                if (_elevPersonDict.TryGetValue(uriElevPersonKey, out IEmbeddedResourceObject elevPersonData))
                                {
                                    var newUriKey = AddStudentToCS(
                                        uriElevKey,
                                        elevData,
                                        elevPersonData,
                                        organisasjonIdUri,
                                        ref ssnToSystemId,
                                        ref importedObjectsDict
                                        );
                                }
                                else
                                {
                                    Logger.Log.ErrorFormat("{0} linked to from resource {1} but not found on /utdanning/elev/person endpoint. The adapter is supplying inconsistent data", uriElevPersonKey, uriElevKey);
                                }
                            }
                            else
                            {
                                Logger.Log.ErrorFormat("{0} is missing mandatory link: {1}", uriElevKey, ResourceLink.person);
                            }

                            if (elevData.Links.TryGetValue(ResourceLink.studentRelationship, out IEnumerable<ILinkObject> studentRelationshipLink))
                            {
                                var studentRelationshipUri = LinkToString(studentRelationshipLink);
                                Logger.Log.ErrorFormat("{0} linked to from resource {1} but not found on /utdanning/elev/elevforhold endpoint. The adapter is supplying inconsistent data", studentRelationshipUri, uriElevKey);
                            }
                        }
                        else
                        {
                            Logger.Log.ErrorFormat("{0} linked to from resource {1} but not found on /utdanning/elev/elev endpoint. The adapter is supplying inconsistent data", uriElevKey, uriElevKey);
                        }
                    }
                }
            }

            foreach (var key in importedObjectsDict.Keys)
            {
                var objectToImport = importedObjectsDict[key];
                ImportedObjectsList.Add(objectToImport);
            }

            GetImportEntriesIndex = 0;
            GetImportEntriesPageSize = importRunStep.PageSize;

            return new OpenImportConnectionResults();
        }

        private Dictionary<string, Periode> GetTerminAndSkolearPerioder(KeyedCollection<string, ConfigParameter> configParameters, List<string> componentList)
        {
            var periodeDict = new Dictionary<string, Periode>();

            Dictionary<string, IEmbeddedResourceObject>resourceDict = GetDataFromFINTApi(configParameters, componentList);

            foreach (var urikey in resourceDict.Keys)
            {
                IEmbeddedResourceObject embeddedResourceObject = resourceDict[urikey];

                var periode = PeriodeFactory.Create(embeddedResourceObject.State);

                if (periode != null)
                {
                    Logger.Log.Info($"{urikey} has a valid period: {periode.Start} - {periode.Slutt}");
                    periodeDict.Add(urikey, periode);
                }
                else
                {
                    Logger.Log.Error($"{urikey} has an invalid period");
                }
            }
            return periodeDict;
        }
        private void AddValidMembership(string uriKey, Dictionary<string, IEmbeddedResourceObject> resourceDict, int dayBeforeStudentStarts, int dayBeforeStudentEnds, bool isExamGroupMembership, string groupRelation, ref Dictionary<string, List<string>> groupAndValidStudentRelationships)
        {
            if (resourceDict.TryGetValue(uriKey, out IEmbeddedResourceObject membershipResource))
            {
                bool membershipIsValid = true;

                if (!isExamGroupMembership && membershipResource.State.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue periodeValue))
                {
                    membershipIsValid = PeriodIsValid(periodeValue, dayBeforeStudentStarts, dayBeforeStudentEnds);
                }
                if (membershipIsValid)
                {
                    if (membershipResource.Links.TryGetValue(groupRelation, out IEnumerable<ILinkObject> groupLink))
                    {
                        var groupUri = LinkToString(groupLink);

                        if (membershipResource.Links.TryGetValue(ResourceLink.studentRelationship, out IEnumerable<ILinkObject> studentRelationshipLink))
                        {
                            var studentRelationshipUri = LinkToString(studentRelationshipLink);

                            if (groupAndValidStudentRelationships.TryGetValue(groupUri, out List<string> validStudentRelationships))
                            {
                                groupAndValidStudentRelationships[groupUri].Add(studentRelationshipUri);
                            }
                            else
                            {
                                var studentRelationshipList = new List<String> { studentRelationshipUri };

                                groupAndValidStudentRelationships.Add(groupUri, studentRelationshipList);
                            }
                        }
                    }
                }
                else
                {
                    Logger.Log.Debug($"{uriKey} has not a valid period");
                }
            }
        }

        private void AddValidMembershipsForGroup(string uriKey, Dictionary<string, IEmbeddedResourceObject> resourceDict, int dayBeforeStudentStarts, int dayBeforeStudentEnds, ref Dictionary<string, List<string>> groupAndValidStudentRelationships)
        {
            List<string> groupMembers = new List<string>();

            if (resourceDict.TryGetValue(uriKey, out IEmbeddedResourceObject groupObject))
            {
                bool isExamgroup = GetUriPathForClass(uriKey).Equals(FintValue.utdanningVurderingEksamensgruppeUri);

                if (groupObject.Links.TryGetValue(ResourceLink.studentRelationship, out IEnumerable<ILinkObject> studentRelationshipLinks))
                {
                    foreach (var studentRelationshipLink in studentRelationshipLinks)
                    {
                        var studentRelationshipUri = LinkToString(studentRelationshipLink);

                        if (resourceDict.TryGetValue(studentRelationshipUri, out IEmbeddedResourceObject studentRelationshipObject))
                        {
                            bool relationshipIsValid;

                            if (!isExamgroup && studentRelationshipObject.State.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue periodeValue))
                            {
                                relationshipIsValid = PeriodIsValid(periodeValue, dayBeforeStudentStarts, dayBeforeStudentEnds);
                            }
                            else
                            {
                                relationshipIsValid = isExamgroup;
                            }

                            if (relationshipIsValid)
                            {
                                groupMembers.Add(studentRelationshipUri);
                            }
                        }
                    }
                }
            }
            groupAndValidStudentRelationships.Add(uriKey, groupMembers);
        }
        private string GetPrincipalForSchool(IEmbeddedResourceObject schoolOrgElementUri)
        {
            var principalUri = string.Empty;

            var links = schoolOrgElementUri.Links;
            if (links.TryGetValue(ResourceLink.leader, out IEnumerable<ILinkObject> leaderlink))
            {
                principalUri = LinkToString(leaderlink);
            }
            return principalUri;
        }

        private HashSet<string> GetExcludedItemCodes(string configParameterValue)
        {
            var excludedItemCodes = new HashSet<string>();

            if (configParameterValue.Contains(","))
            {
                foreach (string _empType in configParameterValue.Split(','))
                {
                    excludedItemCodes.Add(_empType);
                }
            }
            else if (!(string.IsNullOrEmpty(configParameterValue)))
            {
                excludedItemCodes.Add(configParameterValue);
            }
            return excludedItemCodes;
        }

        public CloseImportConnectionResults CloseImportConnection(CloseImportConnectionRunStep importRunStepInfo)
        {
            return new CloseImportConnectionResults();
        }

        public GetImportEntriesResults GetImportEntries(GetImportEntriesRunStep importRunStep)
        {
            /* This method will be invoked multiple times, once for each "page" */

            List<CSEntryChange> csentries = new List<CSEntryChange>();
            GetImportEntriesResults importReturnInfo = new GetImportEntriesResults();

            // If no result, return empty success
            if (ImportedObjectsList == null || ImportedObjectsList.Count == 0)
            {
                importReturnInfo.CSEntries = csentries;
                return importReturnInfo;
            }


            // Parse a full page or to the end
            for (int currentPage = 0;
                GetImportEntriesIndex < ImportedObjectsList.Count && currentPage < GetImportEntriesPageSize;
                GetImportEntriesIndex++, currentPage++)
            {
                bool importPurePrivateStudents = _importConfigParameters[Param.importPurePrivateStudents].Value == "1";

                if (ImportedObjectsList[GetImportEntriesIndex].eduPerson != null)
                {

                    var anchor = ImportedObjectsList[GetImportEntriesIndex].eduPerson.Anchor();

                    if (!String.IsNullOrEmpty(ImportedObjectsList[GetImportEntriesIndex].eduPerson.PersonalAnsattnummer) || ImportedObjectsList[GetImportEntriesIndex].eduPerson.ElevforholdHovedkategori != "privatist" || importPurePrivateStudents)
                    {
                        Logger.Log.DebugFormat("Trying to add eduPerson csentry with anchor {0}", anchor);
                        csentries.Add(ImportedObjectsList[GetImportEntriesIndex].eduPerson.GetCSEntryChange());
                    }
                    else
                    {
                        Logger.Log.DebugFormat("Student {0} has 'privatist' as only student category and is filtered from import to CS", anchor);
                    }
                }
                if (ImportedObjectsList[GetImportEntriesIndex].eduGroup != null)
                {
                    var anchor = ImportedObjectsList[GetImportEntriesIndex].eduGroup.Anchor();
                    Logger.Log.DebugFormat("Trying to add eduGroup csentry with anchor {0}", anchor);
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].eduGroup.GetCSEntryChange());
                }
                if (ImportedObjectsList[GetImportEntriesIndex].eduSubject != null)
                {
                    var anchor = ImportedObjectsList[GetImportEntriesIndex].eduSubject.Anchor();
                    Logger.Log.DebugFormat("Trying to add eduSubject csentry with anchor {0}", anchor);
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].eduSubject.GetCSEntryChange());
                }
                if (ImportedObjectsList[GetImportEntriesIndex].eduOrgUnit != null)
                {
                    var anchor = ImportedObjectsList[GetImportEntriesIndex].eduOrgUnit.Anchor();
                    Logger.Log.DebugFormat("Trying to add eduOrgUnit csentry with anchor {0}", anchor);
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].eduOrgUnit.GetCSEntryChange());
                }
                if (ImportedObjectsList[GetImportEntriesIndex].eduOrgUnitGroup != null)
                {
                    var anchor = ImportedObjectsList[GetImportEntriesIndex].eduOrgUnitGroup.Anchor();
                    Logger.Log.DebugFormat("Trying to add eduOrgUnitGroup csentry with anchor {0}", anchor);
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].eduOrgUnitGroup.GetCSEntryChange());
                }
                if (ImportedObjectsList[GetImportEntriesIndex].eduOrg != null)
                {
                    var anchor = ImportedObjectsList[GetImportEntriesIndex].eduOrg.Anchor();
                    Logger.Log.DebugFormat("Trying to add eduOrg csentry with anchor {0}", anchor);
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].eduOrg.GetCSEntryChange());
                }
                if (ImportedObjectsList[GetImportEntriesIndex].eduStudentRelationship != null)
                {
                    var anchor = ImportedObjectsList[GetImportEntriesIndex].eduStudentRelationship.Anchor();
                    Logger.Log.DebugFormat("Trying to add eduStudentRelationship csentry with anchor {0}", anchor);
                    csentries.Add(ImportedObjectsList[GetImportEntriesIndex].eduStudentRelationship.GetCSEntryChange());
                }
            }

            // Store and return
            importReturnInfo.CSEntries = csentries;
            importReturnInfo.MoreToImport = GetImportEntriesIndex < ImportedObjectsList.Count;
            return importReturnInfo;
        }

        #endregion

        #region export       
        public void OpenExportConnection(KeyedCollection<string, ConfigParameter> configParameters, Schema types, OpenExportConnectionRunStep exportRunStep)
        {
            Logger.Log.Info("Starting export");
            _exportConfigParameters = configParameters;

            var elevUri = FintValue.utdanningElevElevUri;
            var skoleressursUri = FintValue.utdanningElevSkoleressursUri;
            var felleskomponentUri = _exportConfigParameters[Param.felleskomponentUri].Value;

            var uriPaths = new List<string> {
                elevUri,
                skoleressursUri
            };


            var resources = GetDataFromFINTApi(configParameters, uriPaths);

            foreach (var uriKey in resources.Keys)
            {
                Logger.Log.DebugFormat("Adding resource {0} to dictionary", uriKey);
                var resourceType = GetUriPathForClass(uriKey);

                if (resourceType == elevUri)
                {
                    var elevResource = resources[uriKey];
                    //var uriPath = felleskomponentUri + elevUri;

                    var systemIdUri = GetSystemIdUri(elevResource, felleskomponentUri);

                    if (!_resourceDict.TryGetValue(systemIdUri, out IEmbeddedResourceObject existingElevResource))
                    {
                        _resourceDict.Add(systemIdUri, elevResource);
                    }
                    else
                    {
                        Logger.Log.ErrorFormat("Duplicate systemid {0} in elev items. Something is wrong in source system", systemIdUri);
                    }
                }
                else
                {
                    //_resourceDict.Add(uriKey, resources[uriKey]);
                    var skoleressursResource = resources[uriKey];

                    var systemIdUri = GetSystemIdUri(skoleressursResource, felleskomponentUri);

                    if (!_resourceDict.TryGetValue(systemIdUri, out IEmbeddedResourceObject existingSkoleressursResource))
                    {
                        _resourceDict.Add(systemIdUri, skoleressursResource);
                    }
                    else
                    {
                        Logger.Log.ErrorFormat("Duplicate systemid {0} in skoleressurs items. Something is wrong in source system", systemIdUri);
                    }
                }
            }
        }

        public PutExportEntriesResults PutExportEntries(IList<CSEntryChange> csentries)
        {
            Logger.Log.Debug("Opening PutExportEntries");

            Dictionary<string, Dictionary<string, string>> personsAndAttributesToModify = new Dictionary<string, Dictionary<string, string>>();

            foreach (CSEntryChange csentry in csentries)
            {
                Logger.Log.DebugFormat("Exporting csentry {0} with modificationType {1}", csentry.DN, csentry.ObjectModificationType);

                switch (csentry.ObjectModificationType)
                {
                    case ObjectModificationType.Add:
                        Logger.Log.Debug("UpdateAdd hit");
                        break;
                    case ObjectModificationType.Delete:
                        Logger.Log.Debug("UpdateDelete hit");
                        break;
                    case ObjectModificationType.Replace:
                        Logger.Log.Debug("UpdateReplace hit");
                        GetExportDataToModify(csentry, ref personsAndAttributesToModify);
                        break;
                    case ObjectModificationType.Update:
                        Logger.Log.Debug("UpdateUpdate hit");
                        GetExportDataToModify(csentry, ref personsAndAttributesToModify);
                        break;
                    case ObjectModificationType.Unconfigured:
                        Logger.Log.Debug("UpdateUnconfigured hit");
                        break;
                    case ObjectModificationType.None:
                        Logger.Log.Debug("UpdateNone hit");
                        break;
                }
            }
            Logger.Log.Debug("Passed Foreach loop PutExportEntries");
            if (personsAndAttributesToModify != null)
            {
                UpdateFintData(personsAndAttributesToModify);
                PutExportEntriesResults exportEntriesResults = new PutExportEntriesResults();
                return exportEntriesResults;

            }
            return null;
        }

        public void CloseExportConnection(CloseExportConnectionRunStep exportRunStep)
        {
            Logger.Log.Info("Ending export");
        }
        #endregion

        #region private import methods

        private IEmbeddedResourceObject GetTopOrgElement(Dictionary<string, IEmbeddedResourceObject> organisasjonselementDict, KeyedCollection<string, ConfigParameter> importConfigParameters)
        {
            string topOrgElementUri = string.Empty;

            if (importConfigParameters.Contains(Param.organisasjonsIdTopOrgElement) && !string.IsNullOrWhiteSpace(importConfigParameters[Param.organisasjonsIdTopOrgElement].Value))
            {
                string organisasjonsId = importConfigParameters[Param.organisasjonsIdTopOrgElement].Value;

                // topOrgElementUri = Link.with(typeof(Organisasjonselement), FintAttribute.organisasjonsId, organisasjonsId).href;
                // Did not work, returned ${administrasjon.organisasjon.organisasjonselement}/organisasjonsId/6

                var felleskomponentUri = importConfigParameters[Param.felleskomponentUri].Value;
                var classPath = FintValue.administrasjonOrganisasjonOrganisasjonselementUri;
                topOrgElementUri = GetUriStringFromIdValue(felleskomponentUri, classPath, FintAttribute.organisasjonsId, organisasjonsId);
                Logger.Log.DebugFormat("GetTopOrgElement: Using top organisasjonselement from MA config. Top organisasjonselement uri is {0}", topOrgElementUri);
            }
            else
            {
                var initialElement = organisasjonselementDict.Values.First();
                var initialSelfLink = initialElement.Links[ResourceLink.self];
                var currentOrgElementUri = LinkToString(initialSelfLink);
                Logger.Log.DebugFormat("GetTopOrgElement: Using {0} as initial organisasjonselement", currentOrgElementUri);

                bool orgelementIsPresent = true;
                bool topElementFound = false;

                while (orgelementIsPresent && !topElementFound)
                {
                    if (organisasjonselementDict.TryGetValue(currentOrgElementUri, out IEmbeddedResourceObject currentOrgElement))
                    {
                        if (currentOrgElement.Links.TryGetValue(ResourceLink.parent, out IEnumerable<ILinkObject> parentLink))
                        {
                            var currentParentUri = LinkToString(parentLink);
                            Logger.Log.Debug($"Organisasjonselement {currentOrgElementUri} is linked to overordnet {currentParentUri}");

                            if (currentParentUri != currentOrgElementUri)
                            {
                                currentOrgElementUri = currentParentUri;
                            }
                            else
                            {
                                Logger.Log.DebugFormat("Overordnet organisasjonselement {0} is pointing to itself. Top organisasjonselement is found", currentOrgElementUri);
                                topElementFound = true;
                                topOrgElementUri = currentOrgElementUri;
                            }
                        }
                        else
                        {
                            Logger.Log.Error($"Organisasjonselement {currentOrgElementUri} is missing mandatory link {ResourceLink.parent}");
                        }                       
                    }
                    else
                    {
                        Logger.Log.Error($"Overordnet link {currentOrgElementUri} is pointing to an organisasjonselement not found on the organisasjon endpoint");
                        orgelementIsPresent = false;
                    }
                }
            }

            if (organisasjonselementDict.TryGetValue(topOrgElementUri, out IEmbeddedResourceObject topOrgObject))
            {
                return topOrgObject;
            }
            else
            {
                if (!string.IsNullOrEmpty(topOrgElementUri))
                {
                    Logger.Log.ErrorFormat("Top organisasjoonselement {0} not found on the organisasjon endpoint", topOrgElementUri);
                }
                return null;
            }

        }
        private void UpdateResourceIdMappingDict(string idUri, IEmbeddedResourceObject resourceObject, ref Dictionary<string, string> idMappingDict)
        {
            if (resourceObject.Links.TryGetValue(ResourceLink.self, out IEnumerable<ILinkObject> selfLinks))
            {
                foreach (var link in selfLinks)
                {
                    var selfUri = LinkToString(link);
                    Logger.Log.DebugFormat("UpdateResourceIdMappingDict: Adding key {0} and value {1} to dictionary", selfUri, idUri);
                    try
                    {
                        idMappingDict.Add(selfUri, idUri);
                    }
                    catch (Exception e)
                    {
                        Logger.Log.ErrorFormat("UpdateResourceIdMappingDict: Inconsistent self links. Adding key {0} and value {1} to dictionary failed. Error message {2}", selfUri, idUri, e.Message);
                    }
                }
            }
        }

        private Dictionary<string, string> GenerateWorkplaceOrgUnitToSchoolOrgUnit(HashSet<string> schoolOrgElementsTable)
        {
            var dict = new Dictionary<string, string>();

            foreach (var orgElementUri in _organisasjonselementDict.Keys)
            {
                string schoolOrgElementUri = GetSchoolOrgElement(orgElementUri, schoolOrgElementsTable);

                if (schoolOrgElementUri != string.Empty)
                {
                    dict.Add(orgElementUri, schoolOrgElementUri);
                    Logger.Log.DebugFormat("WorkplaceOrgUnitToSchoolOrgUnitTable: Added orgunit {0} to school {1}", orgElementUri, schoolOrgElementUri);
                }
            }
            return dict;
        }

        private string GetSchoolOrgElement(string orgElementUri, HashSet<string> schoolOrgElementsTable)
        {
            Logger.Log.DebugFormat("Trying to find corresponding school org element for {0}", orgElementUri);

            var schoolOrgElementUri = String.Empty;
            var currentOrgElementUri = String.Empty;

            var currentParentUri = orgElementUri;
            bool orgHierarchyOk = true;
            do
            {
                currentOrgElementUri = currentParentUri;

                if (_organisasjonselementDict.TryGetValue(currentOrgElementUri, out IEmbeddedResourceObject currentOrgElement))
                {
                    var currentOrgElementIdentifiers = GetAllIdentifiers(currentOrgElement);

                    if (schoolOrgElementsTable.Intersect(currentOrgElementIdentifiers).Any())
                    {
                        schoolOrgElementUri = currentOrgElementUri;
                        Logger.Log.DebugFormat("Corresponding school org element {0} found for {1}", schoolOrgElementUri, orgElementUri);
                    }
                    else
                    {
                        if (currentOrgElement.Links.TryGetValue(ResourceLink.parent, out IEnumerable<ILinkObject> parentLink))
                        {
                            currentParentUri = LinkToString(parentLink);
                            Logger.Log.DebugFormat("Parent link for {0} is {1}", currentOrgElementUri, currentParentUri);
                        }
                        else
                        {
                            orgHierarchyOk = false;
                            Logger.Log.ErrorFormat("{0} is missing overordnet link. overordnet is mandatory in FINT", currentOrgElementUri);
                        }
                    }
                }
                else
                {
                    Logger.Log.ErrorFormat("{0} is missing on the organisasjon endpoint", currentOrgElementUri);
                    orgHierarchyOk = false;
                }



            } while (orgHierarchyOk && schoolOrgElementUri == String.Empty && currentParentUri != currentOrgElementUri);

            return schoolOrgElementUri;
        }

        private HashSet<string> GetAllIdentifiers(IEmbeddedResourceObject resource)
        {
            var felleskomponentUri = _importConfigParameters[Param.felleskomponentUri].Value;
            var allIdentifiers = new HashSet<string>();

            var selfLinkUri = LinkToString(resource.Links[ResourceLink.self]);
            var uriClassPath = GetUriPathForClass(selfLinkUri);
            var uriPath = felleskomponentUri + uriClassPath;

            foreach (KeyValuePair<string, IStateValue> stateObject in resource.State)
            {
                IStateValue stateValue = stateObject.Value;
                try
                {
                    var idAttribute = JsonConvert.DeserializeObject<Identifikator>(stateValue.Value);
                    var idValue = idAttribute?.Identifikatorverdi;
                    if (!string.IsNullOrEmpty(idValue))
                    {
                        var idAttibuteName = stateObject.Key;
                        var idUri = (uriPath + Delimiter.path + idAttibuteName).ToLower() + Delimiter.path + idValue;
                        allIdentifiers.Add(idUri);
                    }
                }
                catch
                { }
            }

            return allIdentifiers;
        }

        private (bool resourceActiveAtSchool, bool isMainSchool) CheckResourceActiveAtSchool(
            string personalResourceUri,
            string schoolOrganizationElement,
            Dictionary<string, string> principalForSchoolDict,
            Dictionary<string, string> workplaceOrgUnitToSchoolOrgUnit,
            Dictionary<string, IEmbeddedResourceObject> personalressursDict,
            Dictionary<string, IEmbeddedResourceObject> arbeidsforholdDict,
            Dictionary<string, IEmbeddedResourceObject> organisasjonselementDict,
            int daysBeforeEmploymentStarts,
            int daysAfterEmploymentEnds,
            HashSet<string> excludedEmploymentTypes,
            HashSet<string> excludedPositionsCodes)
        {
            bool resourceActiveAtSchool = false;
            bool isMainSchool = false;

            if (principalForSchoolDict.TryGetValue(schoolOrganizationElement, out string principalResourceUri))
            {
                if (principalResourceUri == personalResourceUri)
                {
                    Logger.Log.DebugFormat("CheckResourceActiveAtSchool: Personalresource {0} is principal for school org element {1} and therefore considered active", personalResourceUri, schoolOrganizationElement);
                    resourceActiveAtSchool = true;

                    return (resourceActiveAtSchool, false);
                }
            }
            if (personalressursDict.TryGetValue(personalResourceUri, out IEmbeddedResourceObject personalResource))
            {
                var personalResourcelinks = personalResource.Links;
                if (personalResourcelinks.TryGetValue(ResourceLink.employment, out IEnumerable<ILinkObject> employmentLinks))
                {
                    foreach (ILinkObject employmentLink in employmentLinks)
                    {
                        var employmentUri = LinkToString(employmentLink);

                        Logger.Log.DebugFormat("CheckResourceActiveAtSchool: Checking employment {0}", employmentUri);
                        if (arbeidsforholdDict.TryGetValue(employmentUri, out IEmbeddedResourceObject employmentResource))
                        {
                            var employmentResourceLinks = employmentResource.Links;

                            if (employmentResourceLinks.TryGetValue(ResourceLink.workplace, out IEnumerable<ILinkObject> workplaceLinks))
                            {
                                var workplaceUri = LinkToString(workplaceLinks);

                                if (workplaceOrgUnitToSchoolOrgUnit.TryGetValue(workplaceUri, out string thisSchoolUri))
                                {
                                    if (_organisasjonselementDict.TryGetValue(thisSchoolUri, out IEmbeddedResourceObject schoolOrgElementObject))
                                    {
                                        var schoolidentifiers = GetAllIdentifiers(schoolOrgElementObject);
                                        if (schoolidentifiers.Contains(schoolOrganizationElement))
                                        {
                                            Logger.Log.InfoFormat("Employment {0} is at correct school {1}", employmentUri, thisSchoolUri);

                                            var employmentResourceState = employmentResource.State;
                                            if (employmentResourceState.TryGetValue(FintAttribute.gyldighetsperiode, out IStateValue validEmploymentPeriod))
                                            {
                                                bool periodIsValid = PeriodIsValid(validEmploymentPeriod, daysBeforeEmploymentStarts, daysAfterEmploymentEnds);

                                                if (periodIsValid)
                                                {
                                                    Logger.Log.InfoFormat("Employment {0} at school {1} has a valid period", employmentUri, thisSchoolUri);
                                                    if (employmentResourceLinks.TryGetValue(ResourceLink.employmentType, out IEnumerable<ILinkObject> employmentTypeLink))
                                                    {
                                                        var employmentTypeId = GetIdValueFromLink(employmentTypeLink);

                                                        if (!excludedEmploymentTypes.Contains(employmentTypeId))
                                                        {
                                                            if (employmentResourceLinks.TryGetValue(ResourceLink.positionCode, out IEnumerable<ILinkObject> positionCodeLink))
                                                            {
                                                                var positionCodeId = GetIdValueFromLink(positionCodeLink);
                                                                if (!excludedPositionsCodes.Contains(positionCodeId))
                                                                {
                                                                    if (employmentResourceState.TryGetValue(FintAttribute.hovedstilling, out IStateValue hovedstillingValue))
                                                                    {
                                                                        isMainSchool = Convert.ToBoolean(hovedstillingValue.Value);
                                                                    }

                                                                    resourceActiveAtSchool = true;

                                                                    Logger.Log.InfoFormat("Resource {0}, employment {1} has employment type {2} and position code {3}. This employment is considered active", personalResourceUri, employmentUri, employmentTypeId, positionCodeId);
                                                                }
                                                                else
                                                                {
                                                                    Logger.Log.InfoFormat("**EMPLOYEE-FILTER-POSITIONCODE** Resource {0}, employment {1} has excluded position code {2}. This employment is not considered active", personalResourceUri, employmentUri, positionCodeId);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Logger.Log.InfoFormat("**EMPLOYEE-FILTER-EMP-CATEGORY** Resource {0} has employment {1} with excluded employment category {2}. This employment is not considered active", personalResourceUri, employmentUri, employmentTypeId);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.Log.InfoFormat("Employment {0} at school {1} has a non valid period. This employment is not considered active", employmentUri, thisSchoolUri);
                                                }
                                            }
                                            else
                                            {
                                                Logger.Log.ErrorFormat("Employment {0} is lacking mandatory link {1}", employmentUri, FintAttribute.gyldighetsperiode);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var message = "CheckResourceActiveAtSchool: Arbeidssted {0} is not connected to any school organisasjonselement. ";
                                    message += "This may be caused by inconsistent data on the organisasjonselement endpoint ";
                                    message += "(overordnet link pointing to non existent organisasjonselement)";
                                    Logger.Log.ErrorFormat(message, workplaceUri);
                                }
                            }
                            else
                            {
                                Logger.Log.ErrorFormat("Employment {0} is lacking mandatory attribute {1}", employmentUri, ResourceLink.workplace);
                            }
                        }
                    }
                }
            }
            return (resourceActiveAtSchool, isMainSchool);
        }


        private void HandleGroup(
            string groupType,
            string groupUri,
            string schoolUri,
            string orgUri,
            IEmbeddedResourceObject groupData,
            //int importNoDaysAhead,
            //DateTime examgroupsVisibleFromDate,
            //DateTime examgroupsVisibleToDate,
            //Dictionary<string, Grepkode> grepkodeDict,
            EduGroup studyProgramme,
            ref Dictionary<string, (List<string> studentmembers, List<string> teachermembers, List<string> basGroupmembers)> levelGroupDictionary,
            ref Dictionary<string, string> ssnToSystemId,
            ref Dictionary<string, ImportListItem> importedObjectsDict,
            ref Dictionary<string, Periode> groupPeriodDict
        )
        {
            var eduGroup = new EduGroup();

            if (!importedObjectsDict.TryGetValue(groupUri, out ImportListItem eduGroupValue))
            {
                var groupState = groupData.State;
                var groupLinks = groupData.Links;
                Gruppe group = null;
                string examCategory = String.Empty;
                DateTime? examDate = null;

                switch (groupType)
                {
                    case ClassType.basicGroup:
                        {
                            //group = BasisgruppeFactory.Create(groupState);
                            group = KlasseFactory.Create(groupState);
                            break;
                        }
                    case ClassType.contactTeacherGroup:
                        {
                            group = KontaktlarergruppeFactory.Create(groupState);
                            break;
                        }
                    case ClassType.studyGroup:
                        {
                            group = UndervisningsgruppeFactory.Create(groupState);
                            break;
                        }
                    case ClassType.examGroup:
                        {
                            if (groupLinks.TryGetValue(ResourceLink.eksamensform, out IEnumerable<ILinkObject> eksamensformLink) &&
                                groupState.TryGetValue(FintAttribute.eksamensdato, out IStateValue eksamensdatoValue))
                            {                                
                                examCategory = GetIdValueFromLink(eksamensformLink);
                                examDate = DateTime.Parse(eksamensdatoValue.Value);
                                group = UtdanningsprogramFactory.Create(groupState);
                                
                            }
                            break;
                        }
                    case ClassType.programmeArea:
                        {
                            group = ProgramomradeFactory.Create(groupState);
                            break;
                        }
                    case ClassType.educationProgramme:
                        {
                            group = UtdanningsprogramFactory.Create(groupState);
                            break;
                        }
                }
                if (importedObjectsDict.TryGetValue(schoolUri, out ImportListItem eduOrgUnitItem))
                {
                    EduOrgUnit eduOrgUnit = eduOrgUnitItem.eduOrgUnit;

                    if (groupPeriodDict.TryGetValue(groupUri, out Periode validPeriod))
                    {
                        eduGroup = EduGroupFactory.Create(groupUri, group, validPeriod, groupType, examCategory, examDate, groupLinks, eduOrgUnit, studyProgramme);

                        Logger.Log.InfoFormat("Adding new resource to CS: {0}", groupUri);

                        var link = string.Empty;
                        var itemDict = new Dictionary<string, IEmbeddedResourceObject>();
                        var membershipDict = new Dictionary<string, List<string>>();

                        switch (groupType)
                        {
                            case ClassType.contactTeacherGroup:
                                {
                                    membershipDict = _contactGroupAndValidStudentRelationships;
                                    break;
                                }
                            case ClassType.basicGroup:
                                {
                                    link = ResourceLink.level;
                                    itemDict = _arstrinnDict;
                                    membershipDict = _basicGroupAndValidStudentRelationships;
                                    break;
                                }
                            case ClassType.studyGroup:
                                {
                                    link = ResourceLink.subject;
                                    itemDict = _fagDict;
                                    membershipDict = _studyGroupAndValidStudentRelationships;
                                    break;
                                }
                            case ClassType.examGroup:
                                {
                                    link = ResourceLink.examGroup;
                                    membershipDict = _examGroupAndValidStudentRelationships;
                                    break;
                                }
                            case ClassType.programmeArea:
                                {
                                    link = ResourceLink.programmearea;
                                    itemDict = _programomradeDict;
                                    break;
                                }
                            case ClassType.educationProgramme:
                                {
                                    link = ResourceLink.studyprogramme;
                                    itemDict = _utdanningsprogramDict;
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        if (!(groupType == ClassType.examGroup))
                        {
                            SetGrepkodeOnGroup(groupData, link, itemDict, ref eduGroup);
                        }
                        if (groupData.Links.TryGetValue(ResourceLink.subject, out IEnumerable<ILinkObject> subjectLink))
                        {
                            var subjectUri = LinkToString(subjectLink);
                            eduGroup.GruppeFagRef = subjectUri;

                            if (_fagDict.TryGetValue(subjectUri, out IEmbeddedResourceObject subjectValue))
                            {
                                if (!importedObjectsDict.TryGetValue(subjectUri, out ImportListItem dummyValue))
                                {
                                    var stateValue = subjectValue.State;
                                    var subject = FagFactory.Create(stateValue);
                                    var eduSubject = new EduSubject();
                                    eduSubject = EduSubjectFactory.Create(subjectUri, subject);

                                    Logger.Log.InfoFormat("Subject {0} referenced by group {1} and will be added to CS", subjectUri, groupUri);
                                    importedObjectsDict.Add(subjectUri, new ImportListItem() { eduSubject = eduSubject });
                                }
                            }
                        }
                        Logger.Log.InfoFormat("Group {0} is active. The group and members of the group will be added to CS", groupUri);

                        importedObjectsDict.Add(groupUri, new ImportListItem() { eduGroup = eduGroup });

                        if (groupData.Links.TryGetValue(ResourceLink.studentRelationship, out IEnumerable<ILinkObject> groupMembershipLinks))
                        {
                            //var studentRelationships = groupData.Links[ClassType.studentRelationship];
                            if (membershipDict.TryGetValue(groupUri, out List<string> validStudentRelationships))
                            {
                                AddEduPersonsInGroupToCS(
                                    ClassType.studentRelationship,
                                    groupUri,
                                    schoolUri,
                                    orgUri,
                                    validStudentRelationships,
                                    _elevforholdDict,
                                    _elevDict,
                                    _elevPersonDict,
                                    ref ssnToSystemId,
                                    ref importedObjectsDict);
                            }
                        }
                        if (groupData.Links.TryGetValue(ResourceLink.teachingRelationship, out IEnumerable<ILinkObject> teachingRelationshipLinks))
                        {
                            List<string> teachingRelationships = new List<string>();

                            foreach (var teachingRelationshipLink in teachingRelationshipLinks)
                            {
                                teachingRelationships.Add(LinkToString(teachingRelationshipLink));
                            }
                            AddEduPersonsInGroupToCS(
                                ClassType.teachingRelationship,
                                groupUri,
                                schoolUri,
                                orgUri,
                                teachingRelationships,
                                _undervisningsforholdDict,
                                _skoleressursDict,
                                _ansattPersonDict,
                                ref ssnToSystemId,
                                ref importedObjectsDict);
                        }
                        if (groupType == ClassType.basicGroup)
                        {
                            string levelGroupUri;

                            if (groupLinks.TryGetValue(ResourceLink.level, out IEnumerable<ILinkObject> levelLink))
                            {
                                levelGroupUri = LinkToString(levelLink) + Delimiter.levelAtSchool + GetIdValueFromUri(schoolUri);

                                if (!levelGroupDictionary.ContainsKey(levelGroupUri))
                                {
                                    var studentMembers = new List<string>();
                                    var teacherMembers = new List<string>();
                                    var basicGroupMembers = new List<string>();
                                    levelGroupDictionary.Add(levelGroupUri, (studentMembers, teacherMembers, basicGroupMembers));
                                }

                                var alreadyStudentMembers = levelGroupDictionary[levelGroupUri].studentmembers;
                                foreach (string member in eduGroup.GruppeElevListe)
                                {
                                    if (!alreadyStudentMembers.Contains(member))
                                    {
                                        alreadyStudentMembers.Add(member);
                                    }
                                }

                                var alreadyTeacherMembers = levelGroupDictionary[levelGroupUri].teachermembers;
                                foreach (string member in eduGroup.GruppeLarerListe)
                                {
                                    if (!alreadyTeacherMembers.Contains(member))
                                    {
                                        alreadyTeacherMembers.Add(member);
                                    }
                                }
                                var alreadyBacicGroupMembers = levelGroupDictionary[levelGroupUri].basGroupmembers;

                                if (!alreadyBacicGroupMembers.Contains(groupUri))
                                {
                                    alreadyBacicGroupMembers.Add(groupUri);
                                }
                            }
                        }
                    }

                }

            }
        }

        private string AddStudentToCS(
            string studentUri,
            IEmbeddedResourceObject studentData,
            IEmbeddedResourceObject personData,
            string orgUri,
            ref Dictionary<string, string> ssnToSystemId,
            ref Dictionary<string, ImportListItem> importedObjectsDict
        )
        {
            var returnedResourceUri = string.Empty;

            var student = new Elev();
            student = ElevFactory.Create(studentData.State);
            var person = new Person();
            if (personData != null)
            {
                person = PersonFactory.Create(personData.State);
            }

            var eduPerson = new EduPerson();
            eduPerson = EduPersonFactory.Create(studentUri, person, student, orgUri);

            var eduPersonSSN = eduPerson.PersonFodselsnummer;
            if (!ssnToSystemId.TryGetValue(eduPersonSSN, out string eduPersonSystemId))
            {
                Logger.Log.InfoFormat("Adding new eduPerson to CS with anchor: {0}", studentUri);
                importedObjectsDict.Add(studentUri, new ImportListItem() { eduPerson = eduPerson });
                ssnToSystemId.Add(eduPersonSSN, studentUri);

                returnedResourceUri = studentUri;
            }
            else
            {
                if (importedObjectsDict.TryGetValue(eduPersonSystemId, out ImportListItem importListItem))
                {
                    var existingEduPerson = importListItem.eduPerson;
                    existingEduPerson.ElevSystemIdUri = studentUri;
                    existingEduPerson.ElevFeidenavn = eduPerson?.ElevFeidenavn;
                    existingEduPerson.ElevElevnummer = eduPerson?.ElevElevnummer;

                    Logger.Log.InfoFormat("eduPerson with same ssn: {0} already exist in CS with anchor: {1}", eduPersonSSN, eduPersonSystemId);
                    Logger.Log.InfoFormat("Data for {0} will be added to the existing eduPerson", studentUri);
                }
                returnedResourceUri = eduPersonSystemId;
            }
            return returnedResourceUri;

        }

        private void AddEduPersonsInGroupToCS(
            string relationshipType,
            string groupUri,
            string schoolUri,
            string orgUri,
            List<string> relationshipLinks,
            Dictionary<string, IEmbeddedResourceObject> eduRelationShipDict,
            Dictionary<string, IEmbeddedResourceObject> eduPersonDict,
            Dictionary<string, IEmbeddedResourceObject> personDict,
            ref Dictionary<string, string> ssnToSystemId,
            ref Dictionary<string, ImportListItem> importedObjectsDict
            )
        {
            var eduGroup = new EduGroup();
            var eduGroupAnchor = String.Empty;

            var eduOrgUnit = new EduOrgUnit();
            var eduOrgUnitAnchor = string.Empty;

            if (importedObjectsDict.TryGetValue(groupUri, out ImportListItem eduGroupItem))
            {
                eduGroup = eduGroupItem.eduGroup;
                eduGroupAnchor = eduGroup.Anchor();
            }
            if (!string.IsNullOrEmpty(schoolUri))
            {
                if (importedObjectsDict.TryGetValue(schoolUri, out ImportListItem eduOrgUnitItem))
                {
                    eduOrgUnit = eduOrgUnitItem.eduOrgUnit;
                    eduOrgUnitAnchor = eduOrgUnit.Anchor();
                }
            }
            var studentCategoryUri = string.Empty;
            var newEduRescourceUri = string.Empty;

            var studyProgrammeGrepkode = new Grepkode();
            var programmeAreaGrepkode = new Grepkode();

            Logger.Log.Debug($"Relationshiptype is {relationshipType}");

            if (relationshipType == ClassType.studentRelationship)
            {
                foreach (var studentRelationshipUri in relationshipLinks)
                {
                    Logger.Log.Debug($"Parsing student relationship {studentRelationshipUri}");

                    if (eduRelationShipDict.TryGetValue(studentRelationshipUri, out IEmbeddedResourceObject studentRelationShipData))
                    {
                        IReadOnlyDictionary<string, IEnumerable<ILinkObject>> studentRelationShipLinks = studentRelationShipData.Links;

                        if (studentRelationShipLinks.TryGetValue(ResourceLink.studentCategory, out IEnumerable<ILinkObject> catecoryLink))
                        {
                            studentCategoryUri = LinkToString(catecoryLink);
                            Logger.Log.Debug($"Student relationship {studentRelationshipUri} has student category {studentCategoryUri}");
                        }
                        if (studentRelationShipData.Links.TryGetValue(ResourceLink.student, out IEnumerable<ILinkObject> studentLink))
                        {
                            var studentLinkUri = LinkToString(studentLink);

                            if (_elevIdMappingDict.TryGetValue(studentLinkUri, out string studentUri))
                            {
                                if (!importedObjectsDict.TryGetValue(studentUri, out ImportListItem existingImportListItemValue))
                                {
                                    Logger.Log.Debug($"Student {studentUri} not present in CS import dictionary.Trying to add it");

                                    if (eduPersonDict.TryGetValue(studentUri, out IEmbeddedResourceObject eduPersonData))
                                    {
                                        IReadOnlyDictionary<string, IEnumerable<ILinkObject>> eduPersonDataLinks = eduPersonData.Links;

                                        if (eduPersonDataLinks.TryGetValue(ResourceLink.person, out IEnumerable<ILinkObject> personLink))
                                        {
                                            var personUri = LinkToString(personLink);
                                            Logger.Log.Debug($"Parsing person {personUri}");

                                            if (personDict.TryGetValue(personUri, out IEmbeddedResourceObject personData))
                                            {
                                                newEduRescourceUri = AddStudentToCS(studentUri, eduPersonData, personData, orgUri, ref ssnToSystemId, ref importedObjectsDict);
                                            }
                                            else
                                            {
                                                Logger.Log.InfoFormat("Resource {0} has person link {1}, but this resource is not found. Resource {0} will be added to CS when person resource becomes available", studentUri, ResourceLink.person);
                                            }
                                        }
                                        else
                                        {
                                            Logger.Log.ErrorFormat("Resource {0} is lacking mandatory link {1}", studentUri, ResourceLink.person);
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.Log.Debug($"Student {studentUri} already present in CS import dictionary. Trying to add additional info to the import object");
                                    newEduRescourceUri = studentUri;
                                }

                                if (!string.IsNullOrEmpty(newEduRescourceUri))
                                {
                                    var programmeareaUri = string.Empty;
                                    var studentRelationshipResource = new ElevforholdResource();
                                    studentRelationshipResource = ElevforholdResourceFactory.Create(studentRelationShipData);

                                    if (!importedObjectsDict.TryGetValue(studentRelationshipUri, out ImportListItem dummyStudentRelationship))
                                    {
                                        if (studentRelationShipLinks.TryGetValue(ResourceLink.programmearea, out IEnumerable<ILinkObject> programmeareaLink))
                                        {
                                            programmeareaUri = LinkToString(programmeareaLink);
                                        }
                                        var eduStudentRelationship = EduStudentRelationshipFactory.Create(
                                            studentRelationshipUri, 
                                            newEduRescourceUri, 
                                            schoolUri, 
                                            studentCategoryUri, 
                                            programmeareaUri,
                                            studentRelationshipResource);
                                        importedObjectsDict.Add(studentRelationshipUri, new ImportListItem { eduStudentRelationship = eduStudentRelationship });
                                    }

                                    bool isMainSchool = false;

                                    if (studentRelationShipData.State.TryGetValue(FintAttribute.hovedskole, out IStateValue hovedskoleValue))
                                    {
                                        isMainSchool = Convert.ToBoolean(hovedskoleValue.Value);
                                    }
                                    //var programmeareaUri = string.Empty;

                                    //if (studentRelationShipLinks.TryGetValue(ResourceLink.programmearea, out IEnumerable<ILinkObject> programmeareaLink))
                                    //{
                                    //    programmeareaUri = LinkToString(programmeareaLink);
                                        
                                    //    if (false)
                                    //    {
                                    //        AddStudentToProgrammeAreaGroup(
                                    //            newEduRescourceUri,
                                    //            programmeareaUri,
                                    //            eduOrgUnit,
                                    //            ref importedObjectsDict
                                    //            );
                                    //    }
                                    //}
                                    AddMembershipOrgUnitAndEduEntitementInfo(
                                        newEduRescourceUri,
                                        relationshipType,
                                        studentCategoryUri,
                                        isMainSchool,
                                        programmeareaUri,
                                        eduGroupAnchor,
                                        eduGroup,
                                        eduOrgUnit,
                                        ref importedObjectsDict
                                        );
                                }
                            }
                            else
                            {
                                Logger.Log.Error($"{studentLinkUri} is referenced by {studentRelationshipUri} but not present on the {Constants.FintValue.utdanningElevElevUri} endpoint");
                            }
                        }
                        else
                        {
                            Logger.Log.ErrorFormat("Resource {0} is lacking mandatory link {1}", studentRelationshipUri, ResourceLink.student);
                        }
                    }
                }
            }
            else
            {
                foreach (var teachingRelationshipUri in relationshipLinks)
                {
                    Logger.Log.Debug($"Parsing teaching relationship {teachingRelationshipUri}");

                    if (_undervisningsforholdDict.TryGetValue(teachingRelationshipUri, out IEmbeddedResourceObject teachingRelationshipData))
                    {
                        if (teachingRelationshipData.Links.TryGetValue(ResourceLink.schoolresource, out IEnumerable<ILinkObject> schoolResourceLink))
                        {
                            var schoolResourceLinkUri = LinkToString(schoolResourceLink);

                            if (_skoleressursIdMappingDict.TryGetValue(schoolResourceLinkUri, out string schoolResourceUri))
                            {
                                Logger.Log.Debug($"Parsing schoolresource {schoolResourceUri}");

                                if (!importedObjectsDict.TryGetValue(schoolResourceUri, out ImportListItem dummySchoolResourceData))
                                {
                                    Logger.Log.Debug($"Schoolresource {schoolResourceUri} not present in CS import dictionary.Trying to add it");
                                    if (_skoleressursDict.TryGetValue(schoolResourceUri, out IEmbeddedResourceObject schoolResourceData))
                                    {
                                        newEduRescourceUri = AddNonStudentToCS(
                                                                schoolResourceUri,
                                                                orgUri,
                                                                schoolResourceData,
                                                                ref ssnToSystemId,
                                                                ref importedObjectsDict);
                                    }
                                }
                                else
                                {
                                    Logger.Log.Debug($"Schoolresource {schoolResourceUri} already present in CS import dictionary. Trying to add additional info on the import object");
                                    newEduRescourceUri = schoolResourceUri;
                                }
                                AddMembershipOrgUnitAndEduEntitementInfo(
                                    newEduRescourceUri,
                                    relationshipType,
                                    studentCategoryUri,
                                    false,
                                    null,
                                    eduGroupAnchor,
                                    eduGroup,
                                    eduOrgUnit,
                                    ref importedObjectsDict
                                    );
                            }
                            else
                            {
                                Logger.Log.Error($"{schoolResourceLinkUri} is referenced by {teachingRelationshipUri} but not present on the {FintValue.utdanningElevSkoleressursUri} endpoint");
                            }
                        }
                    }
                }
            }
        }

        private void AddStudentToProgrammeAreaGroup(string studentRescourceUri, string programmeareaUri, EduOrgUnit eduOrgUnit, ref Dictionary<string, ImportListItem> importedObjectsDict)
        {
            var eduOrgUnitId = eduOrgUnit.Anchor();
            var programmeareaId = programmeareaUri.Split('/').Last();

            var programmeareaGroupUri = eduOrgUnitId + '_' + programmeareaId;

            EduGroup programmeareaGroup = new EduGroup();

            if (importedObjectsDict.TryGetValue(programmeareaGroupUri, out ImportListItem programmeareaGroupItem))
            {
                programmeareaGroup = programmeareaGroupItem.eduGroup;
            }
            else
            {
                if (_programomradeDict.TryGetValue(programmeareaUri, out IEmbeddedResourceObject programmeareaObject))
                {
                    if (programmeareaObject.State.TryGetValue(FintAttribute.navn, out IStateValue nameValue))
                    {
                        var programmeareaName = nameValue.Value;
                        programmeareaGroup = EduGroupFactory.Create(programmeareaGroupUri, programmeareaName, eduOrgUnit);

                        importedObjectsDict.Add(programmeareaGroupUri, new ImportListItem() { eduGroup = programmeareaGroup });
                    }
                }
            }
            if (programmeareaGroup.GruppeElevListe != null && !programmeareaGroup.GruppeElevListe.Contains(studentRescourceUri))
            {
                programmeareaGroup.GruppeElevListe.Add(studentRescourceUri);
            }
        }

        private void AddMembershipOrgUnitAndEduEntitementInfo(
            string newEduRescourceUri,
            string relationshipType,
            string studentCategoryUri,
            bool isMainSchool,
            string programmeAreaUri,
            string eduGroupAnchor,
            EduGroup eduGroup,
            EduOrgUnit eduOrgUnit,
            ref Dictionary<string, ImportListItem> importedObjectsDict
            )
        {
            if (importedObjectsDict.TryGetValue(newEduRescourceUri, out ImportListItem eduPersonItem))
            {
                EduPerson eduPerson = eduPersonItem.eduPerson;
                string eduPersonAnchor = eduPerson.Anchor();
                AddGroupMembershipToCS(relationshipType, eduGroupAnchor, eduPersonAnchor, ref eduGroup, ref eduPerson);
                AddPersonToOrgUnit(relationshipType, studentCategoryUri, isMainSchool, ref eduPerson, ref eduOrgUnit);

                string feideRole = relationshipType == ClassType.studentRelationship ? Feide.RoleStudent : Feide.RoleFaculty;

                var orgnumber = eduOrgUnit.SkoleOrganisasjonsnummer;

                var entitlementItems = new List<string>();

                if (!string.IsNullOrEmpty(orgnumber))
                {
                    entitlementItems = GenerateEntitlementItems(feideRole, eduGroup, orgnumber);
                }
                else
                {
                    Logger.Log.Info($"{eduPersonAnchor}: {eduGroup.Anchor()} is missing orgNumber. No eduPersonEntitlement element added for this group");
                }



                if (!string.IsNullOrEmpty(programmeAreaUri))
                {
                    Grepkode studyProgrammeGrepkode;
                    Grepkode programmeAreaGrepkode;
                    (studyProgrammeGrepkode, programmeAreaGrepkode) = GetGrepCodesForStudyProgAndProgArea(programmeAreaUri);

                    var studyProgrammeGrepId = studyProgrammeGrepkode?.Id;

                    if (!string.IsNullOrEmpty(studyProgrammeGrepId))
                    {
                        entitlementItems.AddRange(GenerateEntitlementItems(feideRole, null, orgnumber, studyProgrammeGrepId));
                    }
                    var programmeAreaGrepId = programmeAreaGrepkode?.Id;

                    if (!string.IsNullOrEmpty(programmeAreaGrepId))
                    {
                        entitlementItems.AddRange(GenerateEntitlementItems(feideRole, null, orgnumber, programmeAreaGrepId));
                    }
                }
                foreach (var entitlementItem in entitlementItems)
                {
                    if (!eduPerson.EduPersonEntitlement.Contains(entitlementItem))
                    {
                        eduPerson.EduPersonEntitlement.Add(entitlementItem);
                    }
                }
            }
        }

        private string AddNonStudentToCS(
            string schoolResourceUri,
            string orgUri,
            IEmbeddedResourceObject schoolResourceData,
            ref Dictionary<string, string> ssnToSystemId,
            ref Dictionary<string, ImportListItem> importedObjectsDict
            )
        {
            var returnedResourceUri = String.Empty;

            if (schoolResourceData.Links.TryGetValue(ResourceLink.personalResource, out IEnumerable<ILinkObject> personalLink))
            {
                var personalressursUri = LinkToString(personalLink);

                if (_personalressursDict.TryGetValue(personalressursUri, out IEmbeddedResourceObject personalData))
                {
                    var eduPersonDataLinks = personalData.Links;
                    if (eduPersonDataLinks.TryGetValue(ResourceLink.person, out IEnumerable<ILinkObject> personLink))
                    {
                        var ansattPersonUri = LinkToString(personLink);
                        if (_ansattPersonDict.TryGetValue(ansattPersonUri, out IEmbeddedResourceObject personalPersonData))
                        {
                            returnedResourceUri = AddSchoolResourceToCS(schoolResourceUri, orgUri, schoolResourceData, personalPersonData, personalData, ref ssnToSystemId, ref importedObjectsDict);
                        }
                    }
                    else
                    {
                        Logger.Log.Error($"Personalressurs {personalressursUri} is lacking mandatory link { ResourceLink.person}");
                    }
                }
                else
                {
                    Logger.Log.Error($"Skoleressurs {schoolResourceUri} is linked to personalressurs {personalressursUri} but the resource is missing on the {FintValue.administrasjonPersonalPersonalRessursUri} endpoint");
                }
            }
            else
            {
                Logger.Log.Error($"Skoleressurs {schoolResourceUri} is lacking mandatory link { ResourceLink.personalResource}");
            }
            return returnedResourceUri;
        }

        private string AddSchoolResourceToCS(
            string schoolResourceUri,
            string orgUri,
            IEmbeddedResourceObject schoolResourceData,
            IEmbeddedResourceObject employeePersonData,
            IEmbeddedResourceObject personalResourceData,
            ref Dictionary<string, string> ssnToSystemId,
            ref Dictionary<string, ImportListItem> importedObjectsDict
        )
        {
            var returnedResourceUri = string.Empty;

            var schoolResource = new Skoleressurs();
            schoolResource = SkoleressursFactory.Create(schoolResourceData.State);
            var person = new Person();
            if (employeePersonData != null)
            {
                person = PersonFactory.Create(employeePersonData.State);
            }

            var personalResource = new Personalressurs();
            personalResource = PersonalressursFactory.Create(personalResourceData.State);

            var eduPerson = new EduPerson();
            eduPerson = EduPersonFactory.Create(schoolResourceUri, person, schoolResource, personalResource, orgUri);
            var eduPersonSSN = eduPerson.PersonFodselsnummer;

            if (!ssnToSystemId.TryGetValue(eduPersonSSN, out string eduPersonSystemId))
            {
                Logger.Log.InfoFormat("Adding new eduPerson to CS with anchor: {0}", schoolResourceUri);
                importedObjectsDict.Add(schoolResourceUri, new ImportListItem() { eduPerson = eduPerson });
                ssnToSystemId.Add(eduPersonSSN, schoolResourceUri);

                returnedResourceUri = schoolResourceUri;
            }
            else
            {
                if (importedObjectsDict.TryGetValue(eduPersonSystemId, out ImportListItem importListItem))
                {
                    var existingEduPerson = importListItem.eduPerson;
                    existingEduPerson.SkoleressursSystemIdUri = schoolResourceUri;
                    existingEduPerson.SkoleressursFeidenavn = eduPerson?.SkoleressursFeidenavn;

                    Logger.Log.InfoFormat("eduPerson with same ssn: {0} already exist in CS with anchor: {1}", eduPersonSSN, eduPersonSystemId);
                    Logger.Log.InfoFormat("Data for {0} will be added to the existing eduPerson", schoolResourceUri);
                    returnedResourceUri = eduPersonSystemId;
                }
            }
            return returnedResourceUri;
        }

        private void AddGroupMembershipToCS(string relationshipType, string eduGroupAnchor, string eduPersonAnchor, ref EduGroup eduGroup, ref EduPerson eduPerson)
        {
            Logger.Log.DebugFormat("Trying to add group membership, person {0} and group {1}", eduPersonAnchor, eduGroupAnchor);
            if (relationshipType == ClassType.studentRelationship)
            {
                var groupType = eduGroup.EduGroupType;

                if (!eduGroup.GruppeElevListe.Contains(eduPersonAnchor))
                {
                    switch (groupType)
                    {
                        case ClassType.basicGroup:
                            {
                                eduPerson.ElevforholdBasisgruppe.Add(eduGroupAnchor);
                                break;
                            }
                        case ClassType.contactTeacherGroup:
                            {
                                eduPerson.ElevforholdKontaktlarergruppe.Add(eduGroupAnchor);
                                break;
                            }
                        case ClassType.studyGroup:
                            {
                                eduPerson.ElevforholdUndervisningsgruppe.Add(eduGroupAnchor);
                                break;
                            }
                        case ClassType.examGroup:
                            {
                                eduPerson.ElevforholdEksamensgruppe.Add(eduGroupAnchor);
                                break;
                            }
                    }
                    eduGroup.GruppeElevListe.Add(eduPersonAnchor);
                    Logger.Log.DebugFormat("Adding group membership succeeded, person {0} and group {1}", eduPersonAnchor, eduGroupAnchor);
                }
                else
                {
                    Logger.Log.ErrorFormat("Adapter Error: Student {0} is already member of group {1}. There are duplicates in the student relationship links for this group", eduPersonAnchor, eduGroupAnchor);
                }
            }
            else
            {
                if (!eduGroup.GruppeLarerListe.Contains(eduPersonAnchor))
                {
                    eduPerson.UndervisningsforholdMedlemskap.Add(eduGroupAnchor);
                    eduGroup.GruppeLarerListe.Add(eduPersonAnchor);
                    Logger.Log.InfoFormat("Adding group membership succeeded, person {0} and group {1}", eduPersonAnchor, eduGroupAnchor);
                }
                else
                {
                    Logger.Log.ErrorFormat("Adapter Error: Teacher {0} is already member of group {1}. There are duplicates in the teacher relationship links for this group", eduPersonAnchor, eduGroupAnchor);
                }
            }

        }

        private void AddPersonToOrgUnit(
            string eduPersonType,
            string studentCategoryUri,
            bool isMainSchool,
            ref EduPerson eduPerson,
            ref EduOrgUnit eduOrgUnit
            )
        {
            var personRelationToSchool = new List<string>();
            var schoolRelationToPerson = new List<string>();

            var schoolId = eduOrgUnit.SkoleSystemId;

            List<string> eduPersonOrgUnitDN = eduPerson.EduPersonOrgUnitDN;
            List<string> roleAndSchool = eduPerson.RolleOgSkole;

            switch (eduPersonType)
            {
                case ClassType.studentRelationship:
                    {
                        personRelationToSchool = eduPerson.ElevforholdSkole;
                        schoolRelationToPerson = eduOrgUnit.SkoleElevforhold;

                        if (!string.IsNullOrEmpty(studentCategoryUri))
                        {
                            var studentCategory = studentCategoryUri.Split('/').Last();

                            if (!eduPerson.ElevforholdKategori.Contains(studentCategoryUri))
                            {
                                eduPerson.ElevforholdKategori.Add(studentCategoryUri);

                                if (string.IsNullOrEmpty(eduPerson.ElevforholdHovedkategori))
                                {
                                    eduPerson.ElevforholdHovedkategori = studentCategory;

                                }
                                else
                                {
                                    var currentMainCategory = eduPerson.ElevforholdHovedkategori;

                                    switch (currentMainCategory)
                                    {
                                        case "heltid":
                                            {
                                                break;
                                            }
                                        case "deltid":
                                            {
                                                if (studentCategory=="heltid")
                                                {
                                                    eduPerson.ElevforholdHovedkategori = studentCategory;
                                                }
                                                break;
                                            }
                                        case "privatist":
                                            {
                                                eduPerson.ElevforholdHovedkategori = studentCategory;
                                                break;
                                            }
                                    }
                                }
                            }
                            
                            var categorySchoolItem = studentCategory + Delimiter.categorySchool + schoolId;

                            if (!eduPerson.ElevkategoriOgSkole.Contains(categorySchoolItem))
                            {
                                eduPerson.ElevkategoriOgSkole.Add(categorySchoolItem);
                            }
                        }
                        break;
                    }
                case ClassType.teachingRelationship:
                    {
                        personRelationToSchool = eduPerson.UndervisningsforholdSkole;
                        schoolRelationToPerson = eduOrgUnit.SkoleUndervisningsforhold;
                        break;
                    }
                case ClassType.schoolresource:
                    {
                        personRelationToSchool = eduPerson.AnsettelsesforholdSkole;
                        schoolRelationToPerson = eduOrgUnit.SkoleAnsettelsesforhold;
                        break;
                    }
            }
            var eduPersonAnchor = eduPerson.Anchor();
            var eduOrgUnitAnchor = eduOrgUnit.Anchor();

            if (isMainSchool)
            {
                eduPerson.EduPersonPrimaryOrgUnitDN = eduOrgUnitAnchor;
            }

            if (!eduPersonOrgUnitDN.Contains(eduOrgUnitAnchor))
            {
                eduPersonOrgUnitDN.Add(eduOrgUnitAnchor);
            }

            if (!personRelationToSchool.Contains(eduOrgUnitAnchor))
            {
                personRelationToSchool.Add(eduOrgUnitAnchor);
                var item = eduPersonType + Delimiter.roleSchool + schoolId;
                roleAndSchool.Add(item);
            }

            if (!schoolRelationToPerson.Contains(eduPersonAnchor))
            {
                schoolRelationToPerson.Add(eduPersonAnchor);
            }
        }

        private string GetSystemIdUri(IEmbeddedResourceObject resource, string felleskomponentUri)
        {
            var systemIdValue = GetIdentifikatorValue(resource, FintAttribute.systemId);

            var selfLinkUri = LinkToString(resource.Links[ResourceLink.self]);

            var uriClassPath = GetUriPathForClass(selfLinkUri);

            var uriPath = felleskomponentUri + uriClassPath;

            var systemIdUri = (uriPath + Delimiter.path + FintAttribute.systemId).ToLower() + Delimiter.path + systemIdValue;

            return systemIdUri;

        }

        private string GetIdentifikatorValue(IEmbeddedResourceObject resource, string indentifikatorName)
        {
            var systemIdAttribute = resource.State[FintAttribute.systemId];
            var systemId = JsonConvert.DeserializeObject<Identifikator>(systemIdAttribute.Value);
            var systemIdValue = systemId.Identifikatorverdi;
            return systemIdValue;
        }


        private void SetGrepkodeOnGroup(
            IEmbeddedResourceObject groupData,
            string link,
            Dictionary<string, IEmbeddedResourceObject> groupDict,
            ref EduGroup eduGroup
            )
        {
            var groupLinks = groupData.Links;
            var grepUri = String.Empty;

            if (groupLinks.TryGetValue(ResourceLink.grepreference, out IEnumerable<ILinkObject> directGrepLink))
            {
                grepUri = LinkToString(directGrepLink);
            }
            else
            {
                if (groupLinks.TryGetValue(link, out IEnumerable<ILinkObject> linkedObjects))
                {
                    var groupUri = LinkToString(linkedObjects);

                    if (groupDict.TryGetValue(groupUri, out IEmbeddedResourceObject foundGroupData))
                    {
                        var foundGroupLinks = foundGroupData.Links;

                        if (foundGroupLinks.TryGetValue(ResourceLink.grepreference, out IEnumerable<ILinkObject> grepreferenceLink))
                        {
                            grepUri = LinkToString(grepreferenceLink);

                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(grepUri))
            {
                grepUri = grepUri.Replace("https", "http");

                if (_grepkodeDict.TryGetValue(grepUri, out Grepkode grepkodeFound))
                {
                    eduGroup.Grepkode = grepkodeFound;
                }
                else
                {
                    Logger.Log.ErrorFormat("Grepreference {0} does not exist in the grep database", grepUri);
                }
            }
            else
            {
                Logger.Log.InfoFormat("Group {0} does not have a grep reference link", eduGroup.Anchor());
            }
        }

        private (Grepkode StudyProgramme, Grepkode ProgramArea) GetGrepCodesForStudyProgAndProgArea(string programmeAreaUri)
        {
            var programmeAreaGrepkode = new Grepkode();
            var studyProgrammeGrepkode = new Grepkode();

            Logger.Log.DebugFormat("Finding grep codes for  {0}", programmeAreaUri);

            if (_programomradeDict.TryGetValue(programmeAreaUri, out IEmbeddedResourceObject programmeAreaObject))
            {
                var programmeAreaLinks = programmeAreaObject.Links;

                programmeAreaGrepkode = GetGrepkode(programmeAreaUri, programmeAreaLinks);

                if (programmeAreaLinks.TryGetValue(ResourceLink.studyprogramme, out IEnumerable<ILinkObject> studyProgrammeLink))
                {
                    var studyProgrammeUri = LinkToString(studyProgrammeLink);

                    if (_utdanningsprogramDict.TryGetValue(studyProgrammeUri, out IEmbeddedResourceObject studyProgrammeObject))
                    {
                        var studyProgrammeLinks = studyProgrammeObject.Links;

                        studyProgrammeGrepkode = GetGrepkode(studyProgrammeUri, studyProgrammeLinks);
                    }
                }
            }
            return (studyProgrammeGrepkode, programmeAreaGrepkode);
        }
        private Grepkode GetGrepkode(string programmeAreaUri, IReadOnlyDictionary<string, IEnumerable<ILinkObject>> programmeAreaLinks)
        {
            Grepkode programmeAreaGrepkode = new Grepkode();
            var grepUri = String.Empty;

            if (programmeAreaLinks.TryGetValue(ResourceLink.grepreference, out IEnumerable<ILinkObject> grepReferenceLink))
            {
                grepUri = LinkToString(grepReferenceLink);
            }
            if (!string.IsNullOrEmpty(grepUri))
            {
                grepUri = grepUri.Replace("https", "http");

                if (_grepkodeDict.TryGetValue(grepUri, out Grepkode grepkodeFound))
                {
                    programmeAreaGrepkode = grepkodeFound;
                }
                else
                {
                    Logger.Log.ErrorFormat("Grepreference {0} does not exist in the grep database", grepUri);
                }
            }
            else
            {
                Logger.Log.InfoFormat("Programme area {0} does not have a grep reference link", programmeAreaUri);
            }

            return programmeAreaGrepkode;
        }


        private Dictionary<string, Grepkode> GetGrepCodesForGrepTypes(HttpClient grepHttpClient, string grepBaseDataUrl, List<string> grepTypes)
        {
            var grepCodeDict = new Dictionary<string, Grepkode>();

            foreach (var grepType in grepTypes)
            {
                var grepUrl = grepBaseDataUrl + grepType;
                try
                {
                    Logger.Log.InfoFormat("Trying to get grepcodes from {0}", grepUrl);
                    var grepResponse = grepHttpClient.GetStringAsync(grepUrl).Result;

                    var grepCodeList = JsonConvert.DeserializeObject<List<Grepkode>>(grepResponse);

                    foreach (var grepkode in grepCodeList)
                    {
                        var uri = grepkode.Uri;
                        grepCodeDict.Add(uri, grepkode);
                        //Logger.Log.DebugFormat("Uri:{0}, grepkode: {1}", uri, grepkode.Kode);
                    }
                }
                catch (AggregateException aggregateEx)
                {
                    aggregateEx.Handle(e =>
                    {
                        if (e is HttpRequestException ex)
                        {
                            var message = ex?.Message;
                            var exceptionType = ex?.GetType().ToString();
                            var innerexception = ex?.InnerException?.Message;
                            var errorMessage = $"Getting grepcodes from {grepUrl} failed with error type {exceptionType}. Message: {message}, Inner exception: {innerexception}";
                            Logger.Log.ErrorFormat(errorMessage);
                            Logger.Log.ErrorFormat("Import is aborted");
                            throw new GrepCodeDownloadException(errorMessage);
                        }
                        return false;
                    });
                }
                catch (Exception ex)
                {
                    var message = ex?.Message;
                    var exceptionType = ex?.GetType().ToString();
                    var innerexception = ex?.InnerException?.Message;
                    var errorMessage = $"Getting grepcodes from {grepUrl} failed with error type {exceptionType}. Message: {message}, Inner exception: {innerexception}";
                    Logger.Log.ErrorFormat(errorMessage);
                    Logger.Log.ErrorFormat("Import is aborted");
                    throw new GrepCodeDownloadException(errorMessage);
                }
            }
            return grepCodeDict;
        }

        private List<string> GenerateEntitlementItems(string role, EduGroup group, string orgNumber, string grepIdAsParameter = null)
        {
            var eduPersonEntitlement = new List<string>();

            if (!string.IsNullOrEmpty(grepIdAsParameter))
            {
                string grepString = Feide.PrefixUrnGrep + grepIdAsParameter;
                eduPersonEntitlement.Add(grepString);
            }
            else
            {
                //var groupId = group.GruppeSystemId;
                var groupType = group.EduGroupType;
                var groupName = group.GruppeNavn;
                var groupStartDate = group.GruppePeriodeStart;
                var groupEndDate = group.GruppePeriodeSlutt;
                var grepId = group?.Grepkode.Id;
                var grepCode = group?.Grepkode.Kode;

                var prefixedOrgNumber = Feide.PrefixOrgNumber + orgNumber;
                var descriptionPrefix = (groupType == ClassType.basicGroup) ? "Basisgruppe " : "Undervisningsgruppe ";

                switch (groupType)
                {
                    case ClassType.basicGroup:
                    case ClassType.studyGroup:
                        {
                            string groupTypeCode = ((groupType == ClassType.basicGroup) ? Feide.Basisgruppekode : Feide.undervisningsgruppekode);
                            string urlEncodedGroupName = (groupName != null) ? EscapeUriDataStringRfc3986(groupName) : String.Empty;
                            string urlEncodedGroupNameLowerCase = (groupName != null) ? EscapeUriDataStringRfc3986(groupName.ToLower()) : String.Empty;
                            string urlEncodedGroupDescription = (groupName != null) ? EscapeUriDataStringRfc3986(descriptionPrefix + groupName) : String.Empty;
                            string timespan = groupStartDate + Feide.delimiter + groupEndDate;

                            if (groupTypeCode == Feide.undervisningsgruppekode && string.IsNullOrEmpty(grepId))
                            {
                                groupTypeCode = Feide.annengruppekode;
                            }

                            string groupstring = Feide.PrefixUrnGroup + groupTypeCode + Feide.delimiter;
                            groupstring += (groupType == ClassType.basicGroup ? String.Empty : grepCode) + Feide.delimiter;
                            groupstring += prefixedOrgNumber + Feide.delimiter + urlEncodedGroupName + Feide.delimiter;
                            groupstring += timespan + Feide.delimiter;
                            groupstring += role + Feide.delimiter + urlEncodedGroupDescription;
                            eduPersonEntitlement.Add(groupstring);

                            string groupidstring = Feide.PrefixUrnGroupId + groupTypeCode + Feide.delimiter;
                            groupidstring += prefixedOrgNumber + Feide.delimiter + urlEncodedGroupNameLowerCase + Feide.delimiter + timespan;
                            eduPersonEntitlement.Add(groupidstring);

                            if (!string.IsNullOrEmpty(grepId))
                            {
                                //string grepString = ((grepId.StartsWith("http")) ? Feide.PrefixUrnGrep : Feide.PrefixUrnGrepUuid) + grepId;
                                string grepString = Feide.PrefixUrnGrep + grepId;
                                eduPersonEntitlement.Add(grepString);
                            }

                            break;
                        }
                    case ClassType.level:
                    case ClassType.educationProgramme:
                    case ClassType.programmeArea:
                    case ClassType.subject:
                        {
                            if (!string.IsNullOrEmpty(grepId))
                            {
                                //string grepString = ((grepId.StartsWith("http")) ? Feide.PrefixUrnGrep : Feide.PrefixUrnGrepUuid) + grepId;
                                string grepString = Feide.PrefixUrnGrep + grepId;
                                eduPersonEntitlement.Add(grepString);
                            }
                            break;
                        }
                }
            }
            return eduPersonEntitlement;
        }

        private List<LastUpdateTimestamp> GetLastUpdatedTimestamps(HttpClient httpClient, string baseUri, List<string> resources)
        {
            var lastUpdatedTimestamps = new List<LastUpdateTimestamp>();

            foreach (var resource in resources)
            {
                var resourceUri = baseUri + resource;
                var lastUpdatedUrl = resourceUri + "/last-updated";
                try
                {
                    var response = httpClient.GetStringAsync(lastUpdatedUrl).Result;
                    var lastUpdatedTimestamp = JsonConvert.DeserializeObject<LastUpdateTimestamp>(response);

                    lastUpdatedTimestamp.Resource = resourceUri;

                    //.DateTime.ToLocalTime()
                    var lastUpdatedAsString = DateTimeOffset.FromUnixTimeMilliseconds(lastUpdatedTimestamp.LastUpdated).ToString();
                    lastUpdatedTimestamp.LastUpdatedAsString = lastUpdatedAsString;

                    lastUpdatedTimestamps.Add(lastUpdatedTimestamp);
                }
                catch (AggregateException ex)
                {
                    Logger.Log.ErrorFormat("Getting last-updated timestamp from {0} failed with response: {1}", resourceUri.ToString(), ex.Message);
                }
            }

            return lastUpdatedTimestamps;
        }

        private Dictionary<string, IEmbeddedResourceObject> GetDataFromFINTApi(KeyedCollection<string, ConfigParameter> configParameters, List<string> uriPaths)
        {
            var accessTokenUri = configParameters[Param.idpUri].Value;
            var clientId = configParameters[Param.clientId].Value;
            var clientSecret = Decrypt(configParameters[Param.openIdSecret].SecureValue);
            var username = configParameters[Param.username].Value;
            var password = Decrypt(configParameters[Param.password].SecureValue);
            var scope = configParameters[Param.scope].Value;
            var xOrgId = configParameters[Param.assetId].Value;
            //var xClient = configParameters[Param.xClient].Value;
            var felleskomponentUri = configParameters[Param.felleskomponentUri].Value;
            var httpClientTimeout = Double.Parse(configParameters[Param.httpClientTimeout].Value);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var bearerToken = GetBearenToken(accessTokenUri, clientId, clientSecret, username, password, scope);

            var parser = new HalJsonParser();
            var factory = new HalHttpClientFactory(parser);

            using (var client = factory.CreateClient())
            {
                client.HttpClient.SetBearerToken(bearerToken);

                client.HttpClient.DefaultRequestHeaders.Add(HttpHeader.X_Org_Id, xOrgId);
                //client.HttpClient.DefaultRequestHeaders.Add(HttpHeader.X_Client, xClient);

                client.HttpClient.Timeout = TimeSpan.FromMinutes(httpClientTimeout);

                //Logger.Log.InfoFormat("Getting lastupdated timestamps for all resources");
                //var lastUpdatedTimeStamps = GetLastUpdatedTimestamps(client.HttpClient, felleskomponentUri, uriPaths);

                //var jsonFolder = MAUtils.MAFolder;
                //var filePath = jsonFolder + "\\lastUpdated.json";

                //using (StreamWriter file = File.CreateText(filePath))
                //{
                //    JsonSerializer serializer = new JsonSerializer();
                //    serializer.Serialize(file, lastUpdatedTimeStamps);
                //}

                Logger.Log.Info("Get all resources started");

                bool useLocalCache = configParameters[Param.useLocalCache].Value == "1";
                bool abortIfDownloadError = configParameters[Param.abortIfDownloadError].Value == "1";

                if (useLocalCache)
                {
                    Logger.Log.Info($"Parameter {Param.useLocalCache} is set to true. All resources are fetched from local cache");
                }
                var (responseList, dataRetrievalStatus) = GetDataAsync(felleskomponentUri, uriPaths, client, useLocalCache, abortIfDownloadError).Result;

                Logger.Log.Info("Get all resources ended");
                if (abortIfDownloadError && dataRetrievalStatus != (int)DataRetrievalStatus.DownloadOK)
                {
                    var message = $"Import is aborted. There were download errors and parameter '{Param.abortIfDownloadError}' is set to true";
                    Logger.Log.Info(message);
                    var throwMessage = message + ". See MA log for further details";
                    throw new FINTDownloadFailedException(throwMessage);
                }
                if (dataRetrievalStatus == (int) DataRetrievalStatus.FileReadFailed)
                {
                    var message = "Import is aborted. Reading from local cache failed for at least one endpoint";
                    Logger.Log.Info(message);
                    var throwMessage = message + ". See MA log for further details";
                    throw new DataFromLocalCacheFaildeException(throwMessage);
                }
                var resourcesDict = new Dictionary<string, IEmbeddedResourceObject>();
                foreach (var response in responseList)
                {
                    if (response != null && response.EmbeddedResources != null)
                    {
                        var entries = response.EmbeddedResources;

                        foreach (var entry in entries)
                        {
                            if (entry.Links != null)
                            {
                                var links = entry.Links;
                                if (links.TryGetValue(ResourceLink.self, out IEnumerable<ILinkObject> selfLink))
                                {
                                    var resourceUri = LinkToString(selfLink);
                                    if (!(resourcesDict.ContainsKey(resourceUri)))
                                    {
                                        resourcesDict.Add(resourceUri, entry);
                                    }
                                }
                                else
                                {
                                    var entryAsJson = JsonConvert.SerializeObject(entry);
                                    Logger.Log.ErrorFormat("Resource with missing self link: {0}", entryAsJson);
                                }
                            }
                        }
                    }
                }
                return resourcesDict;
            }
        }

        static private async Task<(HalJsonParseResult[], int)> GetDataAsync(string felleskomponentUri, List<string> uriPaths, IHalHttpClient client, bool useLocalCache, bool abortIfDownloadError)
        {
            Logger.Log.Info("GetDataAsync started");

            IEnumerable<Task<(HalJsonParseResult, int)>> downloadTaskQuery =
                    from uriPath in uriPaths select ProcessURLAsync(uriPath, felleskomponentUri, client, useLocalCache, abortIfDownloadError);

            Task<(HalJsonParseResult, int)>[] downloadTasks = downloadTaskQuery.ToArray();

            (HalJsonParseResult parseResult, int dataRetrievalStatus)[] returnData = await Task.WhenAll(downloadTasks);

            HalJsonParseResult[] parseResults = returnData.Select(element => element.parseResult).ToArray();

            int dataRetrievalStatus;

            if (returnData.All(element => element.dataRetrievalStatus == (int) DataRetrievalStatus.DownloadOK))
            {
                dataRetrievalStatus = (int)DataRetrievalStatus.DownloadOK;
            }
            else if (returnData.Any(element => element.dataRetrievalStatus == (int)DataRetrievalStatus.FileReadFailed))
            {
                dataRetrievalStatus = (int)DataRetrievalStatus.FileReadFailed;
            }
            else
            {
                dataRetrievalStatus = (int)DataRetrievalStatus.FileReadOK;
            }

            Logger.Log.Info("GetDataAsync ended");

            return  (parseResults, dataRetrievalStatus);
        }

        static private async Task<(HalJsonParseResult, int)> ProcessURLAsync(string uriPath, string felleskomponentUri, IHalHttpClient client, bool useLocalCache, bool abortIfDownloadError)
        {
            HalJsonParseResult result = null;
            int dataRetrievalStatus = 0;

            string jsonFolder = MAUtils.MAFolder;
            string fileName = uriPath.Substring(1).Replace('/', '_');
            string filePath = jsonFolder + "\\" + fileName + ".json";

            if (useLocalCache)
            {
                if (File.Exists(filePath))
                {
                    Logger.Log.InfoFormat("Getting last saved response from file {0}", filePath);
                    result = GetDataFromFile(filePath);
                }
                else
                {
                    Logger.Log.InfoFormat("File {0} does not exist, no previous responses has been saved to disk", filePath);
                }
            }
            else
            {
                string uriString = felleskomponentUri + uriPath;
                Uri uri = new Uri(uriString);

                Logger.Log.InfoFormat("Getting data from {0}", uriString);
                try
                {
                    //var sizeUri = uriString + "/cache/size";

                    //string response = await client.HttpClient.GetStringAsync(sizeUri);
                    //var totalItems = JsonConvert.DeserializeObject<CacheSize>(response).Size;

                    var httpResponse = await client.HttpClient.GetStringAsync(uri);

                    Logger.Log.Info($"Data from {uri} returned. Parsing json response");
                    var halJsonParser = new HalJsonParser();
                    result = halJsonParser.Parse(httpResponse);

                    var totalItems = GetTotalItems(result.StateValues);

                    //Logger.Log.InfoFormat("Data from {0} returned with {1} items", uri.ToString(), totalItems.ToString());

                    if (totalItems > 0)
                    {
                        Logger.Log.InfoFormat("Writing response to file {0}", filePath);
                        var httpResponseAsJson = JObject.Parse(httpResponse);

                        WriteDataToFile(filePath, httpResponseAsJson);

                        Logger.Log.DebugFormat("Finished writing response to file {0}", filePath);
                    }
                    else
                    {
                        Logger.Log.Error($"Data from {uri} returned O items. Trying to get data from local cache instead");
                        if (File.Exists(filePath))
                        {
                            Logger.Log.InfoFormat("Getting last saved response from file {0}", filePath);
                            result = GetDataFromFile(filePath);
                            if (result != null)
                            {                                
                                Logger.Log.DebugFormat("Finished getting last saved response from file {0}", filePath);
                            }
                            else
                            {
                                Logger.Log.Error($"Reading from file {filePath} returned null");
                                dataRetrievalStatus = (int)DataRetrievalStatus.FileReadFailed;
                            }
                        }
                        else
                        {
                            Logger.Log.Error($"File {filePath} does not exist"); 
                            dataRetrievalStatus = (int)DataRetrievalStatus.FileReadFailed;
                        }
                    }
                }

                catch (HalHttpRequestException ex)
                {
                    (result, dataRetrievalStatus) = HandleRequestError(uri, ex, filePath, abortIfDownloadError);                    
                }
                catch (HttpRequestException ex)
                {
                    (result, dataRetrievalStatus) = HandleRequestError(uri, ex, filePath, abortIfDownloadError);
                }
                catch (TaskCanceledException ex)
                {
                    (result, dataRetrievalStatus) = HandleRequestError(uri, ex, filePath, abortIfDownloadError);
                }
                catch (WebException ex)
                {
                    (result, dataRetrievalStatus) = HandleRequestError(uri, ex, filePath, abortIfDownloadError);
                }
            }
            return (result, dataRetrievalStatus);
        }

        private static int GetTotalItems(IEnumerable<IStateValue> stateValues)
        {
            int totalItems = 0;

            foreach(IStateValue stateValue in stateValues)
            {
                if (stateValue.Name == "total_items")
                {
                    totalItems = Int32.Parse( stateValue.Value);
                }
            }
            return totalItems;
        }

        private static (HalJsonParseResult, int) HandleRequestError(Uri uri, Exception ex, string filePath, bool abortIfDownloadError)
        {
            HalJsonParseResult result;
            int readResult;
            
            var message = ex?.Message;
            var exceptionType = ex?.GetType().ToString();
            var innerexception = ex?.InnerException?.Message;
            var errorMessage = $"Getting resource uri: {uri} failed with error type {exceptionType}. HTTP Message: {message}, Inner exception: {innerexception}";
            Logger.Log.Error(errorMessage);

            if (File.Exists(filePath))
            {
                Logger.Log.InfoFormat("Getting last saved response from file {0}", filePath);
                result = GetDataFromFile(filePath);
                readResult = (int)DataRetrievalStatus.FileReadOK;
                Logger.Log.DebugFormat("Finished getting last saved response from file {0}", filePath);
            }
            else
            {
                readResult = (int)DataRetrievalStatus.FileReadFailed;
                Logger.Log.Error($"File {filePath} does not exist");
                result = null;
            }            
            return (result, readResult);

        }

        private static HalJsonParseResult GetDataFromFile(string filePath)
        {
            HalJsonParseResult result;
            using (StreamReader reader = new StreamReader(filePath))
            {
                var resourceJson = reader.ReadToEnd();
                var halJsonParser = new HalJsonParser();
                result = halJsonParser.Parse(resourceJson);
            }
            return result;
        }

        private static void WriteDataToFile(string filePath, object dataObject)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, dataObject);
            }
        }

        private void GetDataFromFile(string filePath, ref Dictionary<string, IEmbeddedResourceObject> resourcesDict)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                var resourceJson = reader.ReadToEnd();
                var halJsonParser = new HalJsonParser();
                var result = halJsonParser.Parse(resourceJson);
                var entries = result.EmbeddedResources;

                foreach (var entry in entries)
                {
                    var resourceUri = LinkToString(entry.Links[ResourceLink.self]);

                    if (!(resourcesDict.ContainsKey(resourceUri)))
                    {
                        resourcesDict.Add(resourceUri, entry);
                    }
                }
            }
        }
        private void GetDataFromFile(string filePath, string felleskomponentUri, ref Dictionary<string, IEmbeddedResourceObject> resourcesDict)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                var resourceJson = reader.ReadToEnd();
                var halJsonParser = new HalJsonParser();
                var result = halJsonParser.Parse(resourceJson);
                var entries = result.EmbeddedResources;

                foreach (var entry in entries)
                {
                    var resourceUri = GetSystemIdUri(entry, felleskomponentUri);

                    if (!(resourcesDict.ContainsKey(resourceUri)))
                    {
                        resourcesDict.Add(resourceUri, entry);
                    }
                }
            }
        }
        #endregion

        #region private export methods

        private void GetExportDataToModify(CSEntryChange csentry, ref Dictionary<string, Dictionary<string, string>> personsToModify)
        {
            string personId = csentry.AnchorAttributes[0].Value.ToString();
            Logger.Log.DebugFormat("FintEduPersonUpdate started for person: {0}", personId);

            string elevSystemIdUri = string.Empty;
            string skoleressursSystemIdUri = string.Empty;
            string personUri = string.Empty;

            if (csentry.AttributeChanges.Contains(CSAttribute.ElevSystemIdUri))
            {
                elevSystemIdUri = csentry.AttributeChanges[CSAttribute.ElevSystemIdUri].ValueChanges[0].Value.ToString();
            }

            if (csentry.AttributeChanges.Contains(CSAttribute.SkoleressursSystemIdUri))
            {
                skoleressursSystemIdUri = csentry.AttributeChanges[CSAttribute.SkoleressursSystemIdUri].ValueChanges[0].Value.ToString();
            }

            Dictionary<string, string> changedAttributes = new Dictionary<string, string>();
            foreach (var attributeChange in csentry.AttributeChanges)
            {
                string changedValue = string.Empty;
                var attributeName = attributeChange.Name;

                switch (attributeName)
                {
                    case CSAttribute.ElevFeidenavn:
                        {
                            if (!string.IsNullOrEmpty(elevSystemIdUri))
                            {
                                personUri = elevSystemIdUri;

                                changedValue = csentry.AttributeChanges[CSAttribute.ElevFeidenavn].ValueChanges[0].Value.ToString();
                                changedAttributes.Add(CSAttribute.ElevFeidenavn, changedValue);

                                Logger.Log.DebugFormat("Export {0}: {1}", CSAttribute.ElevFeidenavn, changedValue);
                                personsToModify.Add(personUri, changedAttributes);

                            }
                            break;
                        }
                    case CSAttribute.SkoleressursFeidenavn:
                        {
                            if (!string.IsNullOrEmpty(skoleressursSystemIdUri))
                            {
                                personUri = skoleressursSystemIdUri;

                                changedValue = csentry.AttributeChanges[CSAttribute.SkoleressursFeidenavn].ValueChanges[0].Value.ToString();
                                changedAttributes.Add(CSAttribute.SkoleressursFeidenavn, changedValue);

                                personsToModify.Add(personUri, changedAttributes);

                                Logger.Log.DebugFormat("Export {0}: {1}", CSAttribute.SkoleressursFeidenavn, changedValue);
                            }
                            break;
                        }
                }
            }
        }

        private void UpdateFintData(Dictionary<string, Dictionary<string, string>> resourcesAndAttributesToModify)
        {
            var updateDictionary = new Dictionary<string, JObject>();
            foreach (var resourceUri in resourcesAndAttributesToModify.Keys)
            {
                if (!updateDictionary.TryGetValue(resourceUri, out JObject jObject))
                {
                    var attributes = resourcesAndAttributesToModify[resourceUri];
                    if (attributes != null && attributes.Count > 0)
                    {
                        JObject updateObject = GetUpdateObject(resourceUri, attributes);

                        if (updateObject != null)
                        {
                            updateDictionary.Add(resourceUri, updateObject);
                        }
                    }
                    else
                    {
                        Logger.Log.InfoFormat("Resource {0} used in UpdateFintData but has no attributes to modify", resourceUri);
                    }
                }
            }

            if (updateDictionary.Count > 0)
            {
                UpdateResourcesAsync(updateDictionary).Wait();
            }
        }

        private async Task UpdateResourcesAsync(Dictionary<string, JObject> updates)
        {

            var accessTokenUri = _exportConfigParameters[Param.idpUri].Value;
            var clientId = _exportConfigParameters[Param.clientId].Value;
            var clientSecret = Decrypt(_exportConfigParameters[Param.openIdSecret].SecureValue);
            var username = _exportConfigParameters[Param.username].Value;
            var password = Decrypt(_exportConfigParameters[Param.password].SecureValue);
            var scope = _exportConfigParameters[Param.scope].Value;
            var xOrgId = _exportConfigParameters[Param.assetId].Value;
            //var xClient = _exportConfigParameters[Param.xClient].Value;
            var felleskomponentUri = _exportConfigParameters[Param.felleskomponentUri].Value;

            //int waitTime = Int32.Parse(_exportConfigParameters[Param.waitTime].Value);
            //int lowerLimit = Int32.Parse(_exportConfigParameters[Param.lowerLimit].Value); 
            //int upperLimit = Int32.Parse(_exportConfigParameters[Param.upperLimit].Value);

            int waitTime = 100;
            int lowerLimit = 4;
            int upperLimit = 7;

            var utdanningElevElevUri = FintValue.utdanningElevElevUri;
            var utdanningSkoleRessursUri = FintValue.utdanningElevSkoleressursUri;

            var fullUtdanningElevElevUri = felleskomponentUri + utdanningElevElevUri;
            var fullUtdanningSkoleRessursUri = felleskomponentUri + utdanningSkoleRessursUri;

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var bearerToken = GetBearenToken(accessTokenUri, clientId, clientSecret, username, password, scope);

            var parser = new HalJsonParser();
            var factory = new HalHttpClientFactory(parser);

            using (var client = factory.CreateClient())
            {
                client.HttpClient.SetBearerToken(bearerToken);
                client.HttpClient.DefaultRequestHeaders.Add(HttpHeader.X_Org_Id, xOrgId);

                Queue<String> statusQueue = new Queue<string>();

                foreach (var update in updates)
                {
                    string uri = update.Key;
                    JObject jsonObject = update.Value;
                    string statusUrl = await UpdateResourceAsync(uri, jsonObject, client).ConfigureAwait(false);

                    await Task.Delay(waitTime).ConfigureAwait(false);

                    HttpStatusCode statusCode = await CheckUpdateStatusAsync(statusUrl, client).ConfigureAwait(false);
                    Logger.Log.InfoFormat("Resource {0}: http status is {1} on first response from {2}", uri, statusCode, statusUrl);

                    if (statusCode == HttpStatusCode.Accepted)
                    {
                        statusQueue.Enqueue(statusUrl);
                        Logger.Log.InfoFormat("Resource {0}: Enqueuing location url {1}", uri, statusUrl);

                        if (statusQueue.Count >= upperLimit)
                        {
                            Logger.Log.InfoFormat("Status Queue reached upper element limit ({0}). Stopping new updates until status queue is below {1} elements", upperLimit.ToString(), lowerLimit.ToString());
                            int cycleCount = 1;
                            int count = 0;

                            double delay = waitTime;

                            int remainingQueueLength = statusQueue.Count;

                            while (statusQueue.Count >= lowerLimit && cycleCount <= retrylimit)
                            {
                                ++count;

                                await Task.Delay(waitTime).ConfigureAwait(false);

                                statusUrl = statusQueue.Dequeue();
                                Logger.Log.InfoFormat("Location url {0}: Dequeuing and checking status", statusUrl);

                                statusCode = await CheckUpdateStatusAsync(statusUrl, client).ConfigureAwait(false);

                                if (statusCode == HttpStatusCode.Accepted)
                                {
                                    statusQueue.Enqueue(statusUrl);
                                    Logger.Log.InfoFormat("Location url {0}: Statuscode {1}, enqueuing url again", statusUrl, statusCode.ToString());
                                }
                                else if (statusCode == HttpStatusCode.OK)
                                {
                                    Logger.Log.InfoFormat("Location url {0}: Statuscode {1} - Update confirmed", statusUrl, HttpStatusCode.OK);
                                }

                                if (count == remainingQueueLength)
                                {
                                    ++cycleCount;
                                    remainingQueueLength = statusQueue.Count;
                                    delay *= factor;
                                    var waitmilliseconds = (int)Math.Round(delay);

                                    Logger.Log.InfoFormat("The status queue was not emptied during this cycle, {0} elements remaining. Waiting {1} milliseconds before running cycle no {2}",
                                        remainingQueueLength.ToString(), waitmilliseconds.ToString(), cycleCount.ToString());

                                    await Task.Delay(waitmilliseconds).ConfigureAwait(false);

                                    count = 0;
                                }


                            }
                            Logger.Log.InfoFormat("Status Queue is below lower element limit: {0}. Starting new updates again", lowerLimit.ToString());
                        }
                    }
                    else if (statusCode == HttpStatusCode.OK)
                    {
                        Logger.Log.InfoFormat("Resource {0}: http status is {1} on {2}", uri, HttpStatusCode.OK, statusUrl);
                    }
                }
            }
        }

        private async Task<HttpStatusCode> CheckUpdateStatusAsync(string statusUri, IHalHttpClient client)
        {
            HttpStatusCode statusCode = 0;
            try
            {
                IHalHttpResponseMessage statusResponse = await InvokeRequestAsync(HttpVerb.GET, statusUri, null, client).ConfigureAwait(false);
                statusCode = statusResponse.Message.StatusCode;
            }
            catch (AggregateException aggregateEx)
            {
                aggregateEx.Handle(e =>
                {
                    if (e is HalHttpRequestException hal)
                    {
                        var halStatusCode = hal.StatusCode; // response status code

                        statusCode = halStatusCode;

                        var statusCodeDescription = string.Empty;

                        switch (halStatusCode)
                        {
                            case HttpStatusCode.BadRequest:
                                {
                                    statusCodeDescription = "400 Bad request";
                                    break;
                                }
                            case HttpStatusCode.NotFound:
                                {
                                    statusCodeDescription = "404 Not found";
                                    break;
                                }
                            case HttpStatusCode.InternalServerError:
                                {
                                    statusCodeDescription = "500 Internal server error";
                                    break;
                                }
                        }
                        var resource = hal.Resource;

                        var message = string.Empty;
                        var responsestatus = string.Empty;
                        var exception = string.Empty;

                        if (resource != null)
                        {
                            if (resource.State != null)
                            {
                                var state = resource.State;
                                foreach (var key in state.Keys)
                                {
                                    switch (key)
                                    {
                                        case "message":
                                            {
                                                message = state[key].Value;
                                                break;
                                            }

                                        case "responseStatus":
                                            {
                                                responsestatus = state[key].Value;
                                                break;
                                            }
                                        case "exception":
                                            {
                                                exception = state[key].Value;
                                                break;
                                            }
                                    }
                                }
                            }
                            Logger.Log.ErrorFormat("Location {0}: Getting update status failed. HTTP response: {1}, Message: {2}, Inner response status: {3}, Exception: {4}"
                                , statusUri, statusCodeDescription, message, responsestatus, exception);
                        }
                        return true;

                    }
                    return false;
                });

            }
            catch (HalHttpRequestException e)
            {
                var halStatusCode = e.StatusCode; // response status code
                var statusCodeDescription = string.Empty;

                switch (halStatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            statusCodeDescription = "400 Bad request";
                            break;
                        }
                    case HttpStatusCode.NotFound:
                        {
                            statusCodeDescription = "404 Not found";
                            break;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            statusCodeDescription = "500 Internal server error";
                            break;
                        }
                }
                var message = e?.Message;
                var innerexception = e?.InnerException?.Message;
                Logger.Log.ErrorFormat("Location {0}: Getting update status failed. HTTP response: {1}, Message: {2}, Inner exception: {3}"
                                , statusUri, statusCodeDescription, message, innerexception);

            }
            return statusCode;

        }

        private async Task<string> UpdateResourceAsync(string resourceUri, JObject jsonObject, IHalHttpClient client)
        {
            var statusUri = String.Empty;
            try
            {
                Logger.Log.InfoFormat("Resource {0}: starting async update", resourceUri);

                IHalHttpResponseMessage putResponse = await InvokeRequestAsync(HttpVerb.PUT, resourceUri, jsonObject, client).ConfigureAwait(false);

                statusUri = putResponse.Message.Headers.GetValues(HttpHeader.Location).FirstOrDefault();

                Logger.Log.InfoFormat("Resource {0}: status uri for PUT Response {1}", resourceUri, statusUri);


            }
            catch (AggregateException aggregateEx)
            {
                aggregateEx.Handle(e =>
                {
                    if (e is HalHttpRequestException hal)
                    {
                        var halStatusCode = hal.StatusCode; // response status code
                        var statusCodeDescription = string.Empty;

                        switch (halStatusCode)
                        {
                            case HttpStatusCode.BadRequest:
                                {
                                    statusCodeDescription = "400 Bad request";
                                    break;
                                }
                            case HttpStatusCode.InternalServerError:
                                {
                                    statusCodeDescription = "500 Internal server error";
                                    break;
                                }
                        }
                        var resource = hal.Resource;

                        var message = string.Empty;
                        var responsestatus = string.Empty;
                        var exception = string.Empty;

                        if (resource != null)
                        {
                            if (resource.State != null)
                            {
                                var state = resource.State;
                                foreach (var key in state.Keys)
                                {
                                    switch (key)
                                    {
                                        case "message":
                                            {
                                                message = state[key].Value;
                                                break;
                                            }

                                        case "responseStatus":
                                            {
                                                responsestatus = state[key].Value;
                                                break;
                                            }
                                        case "exception":
                                            {
                                                exception = state[key].Value;
                                                break;
                                            }
                                    }
                                }
                            }
                            Logger.Log.ErrorFormat("Resource {0}: update failed. HTTP response: {1}, Message: {2}, Inner response status: {3}, Exception: {4}"
                                , resourceUri, statusCodeDescription, message, responsestatus, exception);
                        }
                        return true;

                    }
                    return false;
                });
            }
            catch (HalHttpRequestException e)
            {
                var halStatusCode = e.StatusCode;
                var statusCodeDescription = string.Empty;

                switch (halStatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        {
                            statusCodeDescription = "400 Bad request";
                            break;
                        }
                    case HttpStatusCode.NotFound:
                        {
                            statusCodeDescription = "404 Not found";
                            break;
                        }
                    case HttpStatusCode.InternalServerError:
                        {
                            statusCodeDescription = "500 Internal server error";
                            break;
                        }
                }
                var message = e?.Message;
                var innerexception = e?.InnerException?.Message;
                Logger.Log.ErrorFormat("Resource {0}: update failed. HTTP response: {1}, Message: {2}, Inner exception: {3}"
                                , resourceUri, statusCodeDescription, message, innerexception);

            }
            return statusUri;
        }


        private JObject GetUpdateObject(string resourceUri, Dictionary<string, string> attributes)
        {
            var jsonObject = new JObject();

            if (_resourceDict.TryGetValue(resourceUri, out IEmbeddedResourceObject response))
            {
                var elevResourceAsJson = string.Empty;
                var attrvalues = string.Empty;

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new LowercaseContractResolver()
                };

                foreach (var attribute in attributes)
                {
                    var csAttributeName = attribute.Key;
                    var csAttributeValue = attribute.Value;
                    switch (csAttributeName)
                    {
                        case CSAttribute.ElevFeidenavn:
                            {
                                var elevResource = ElevResourceFactory.Create(response, FintAttribute.feidenavn);

                                elevResource.Feidenavn.Identifikatorverdi = csAttributeValue;

                                elevResourceAsJson = JsonConvert.SerializeObject(elevResource, settings);
                                //jsonObject = JObject.FromObject(elevResource);
                                jsonObject = JsonConvert.DeserializeObject<JObject>(elevResourceAsJson);

                                break;
                            }

                        case CSAttribute.ElevBrukernavn:
                            {
                                var elevResource = ElevResourceFactory.Create(response);

                                elevResource.Brukernavn.Identifikatorverdi = csAttributeValue;

                                elevResourceAsJson = JsonConvert.SerializeObject(elevResource, settings);
                                jsonObject = JsonConvert.DeserializeObject<JObject>(elevResourceAsJson);
                                break;
                            }
                        case CSAttribute.SkoleressursFeidenavn:
                            {
                                var skoleressursResource = SkoleressursResourceFactory.Create(response, FintAttribute.feidenavn);

                                skoleressursResource.Feidenavn.Identifikatorverdi = csAttributeValue;

                                elevResourceAsJson = JsonConvert.SerializeObject(skoleressursResource, settings);
                                jsonObject = JsonConvert.DeserializeObject<JObject>(elevResourceAsJson);
                                break;
                            }
                    }
                    attrvalues += " " + csAttributeName + "=" + csAttributeValue;
                    Logger.Log.InfoFormat("Updating resource {0} with: {1}", resourceUri, attrvalues);
                    Logger.Log.DebugFormat("JSON body to send: {0}", elevResourceAsJson);
                }
            }
            else
            {
                Logger.Log.ErrorFormat("Udating resource {0} failed. The resource is not available in FINT", resourceUri);
            }
            return jsonObject;
        }

        private JObject GetJsonLinks(IReadOnlyDictionary<string, IEnumerable<ILinkObject>> links)
        {
            dynamic jValue = new JObject();
            foreach (var linkKey in links.Keys)
            {
                switch (linkKey)
                {
                    case ResourceLink.person:
                        {
                            jValue.person = new JArray() as dynamic;

                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                dynamic link = new JObject();
                                var hrefValue = linkObject.Href.ToString();
                                link.href = hrefValue;
                                jValue.person.Add(link);
                            }
                            break;
                        }
                    case ResourceLink.studentRelationship:
                        {
                            jValue.elevforhold = new JArray() as dynamic;

                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                dynamic link = new JObject();
                                var hrefValue = linkObject.Href.ToString();
                                link.href = hrefValue;
                                jValue.elevforhold.Add(link);
                            }
                            break;
                        }
                    case ResourceLink.self:
                        {
                            jValue.self = new JArray() as dynamic;

                            var linkObjects = links[linkKey];
                            foreach (var linkObject in linkObjects)
                            {
                                dynamic link = new JObject();
                                var hrefValue = linkObject.Href.ToString();
                                link.href = hrefValue;
                                jValue.self.Add(link);
                            }
                            break;
                        }
                }
            }
            return jValue;
        }

        #endregion

        #region private helper methods

        private TokenResponse TokenResponseHelper(KeyedCollection<string, ConfigParameter> configParameters)
        {
            var accessTokenUri = configParameters[Param.idpUri].Value;
            var clientId = configParameters[Param.clientId].Value;
            var clientSecret = Decrypt(configParameters[Param.openIdSecret].SecureValue);
            var username = configParameters[Param.username].Value;
            var password = Decrypt(configParameters[Param.password].SecureValue);
            var scope = configParameters[Param.scope].Value;

            var TokenClient = new HttpClient();

            var TokenResponse = TokenClient.RequestTokenAsync(new TokenRequest
            {
                Address = accessTokenUri,

                ClientId = clientId,
                ClientSecret = clientSecret,
                GrantType = OidcConstants.GrantTypes.Password,

                Parameters =
                {
                    { OidcConstants.TokenRequest.UserName, username },
                    { OidcConstants.TokenRequest.Password, password },
                    { OidcConstants.TokenRequest.Scope, scope }
                }
            }).Result;

            return TokenResponse;
        }

        private string GetBearenToken(string accessTokenUri, string clientId, string clientSecret, string username, string password, string scope)
        {

            var tokenClient = new HttpClient();
            var tokenResponse = tokenClient.RequestTokenAsync(new TokenRequest
            {
                Address = accessTokenUri,

                ClientId = clientId,
                ClientSecret = clientSecret,
                GrantType = OidcConstants.GrantTypes.Password,

                Parameters =
                {
                    { OidcConstants.TokenRequest.UserName, username },
                    { OidcConstants.TokenRequest.Password, password },
                    { OidcConstants.TokenRequest.Scope, scope }
                }
            }).Result;

            var bearerToken = tokenResponse.AccessToken;

            return bearerToken;
        }

        private IHalHttpResponseMessage InvokeRequest(string method, string uriString, JObject json, IHalHttpClient client)
        {
            var uri = new Uri(uriString);
            var response = new HttpResponseMessage() as IHalHttpResponseMessage;

            switch (method)
            {
                case HttpVerb.GET:
                    {
                        response = client.GetAsync(uri).Result;
                        break;
                    }
                case HttpVerb.PUT:
                    {
                        response = client.PutAsync(uri, json).Result;
                        break;
                    }
            }
            return response;
        }

        private async Task<IHalHttpResponseMessage> InvokeRequestAsync(string method, string uriString, JObject json, IHalHttpClient client)
        {
            var escapedUriString = Uri.EscapeUriString(uriString);

            var uri = new Uri(escapedUriString);

            var response = new HttpResponseMessage() as IHalHttpResponseMessage;

            switch (method)
            {
                case HttpVerb.GET:
                    {
                        response = await client.GetAsync(uri);
                        break;
                    }
                case HttpVerb.PUT:
                    {
                        response = await client.PutAsync(uri, json);
                        break;
                    }
            }
            return response;
        }

        #endregion

    };

}
