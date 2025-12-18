# VIGOBAS FINT Edu MA

## Description

Management Agent for Microsoft Identity Manager (MIM). Syncs educational data with any school administration system (SAS) that is connected to FINT. Currently InSchool is the only SAS connected to FINT.

Version 1.7.0 is the first public versjon. For older versions see: https://vigobas.vigoiks.no/VigoBAS/systemdokumentasjon/releasenotes/ 

## Version 1.12.0
Released 2025-12-18

This is a "pre-information model version 4" release that might be used in production environments. All use of deprecated information elements are removed. With the exeption of Basisgruppe/Klasse, this version relies entirely on elements that will still be available in version 4. 

### Features
* Updated to information model 3.21
* All members in basic, study, level and exam groups are generated through medlemskaps-resources
* Valid period for groups are calculated via termin and skoleår instead of gyldighetsperiode

## Version 1.10.1
Released 2025-10-09
### Bugfixes
* ElevKontaktinformasjonEpostadresse is populated with correct info
* 
## Version 1.10.0
Released 2025-06-16
### Features
* Added exam dates on student cs object
### Bugfixes
* Proper handling of grep code download errors
* ElevKontaktinformasjonMobiltelfonnummer is populated with correct info

## Version 1.9.0 
Released 2024-05-27
### Features
* Added days before group starts and after groups end as config

## Version 1.8.99 (1.9.0 beta)
Released 2023-04-25
### Features
* Added import of private students (privatister)
* Added import of exam groups based on exam category (eksamensform)

## Version 1.8.0
Released 2023-03-07
### Features
* Added level groups (trinngrupper per skole)
* Added days after employment ends as config
* Updated alle dependencies to latest released version, eg FINT ver 3.13.10
### Bugfixes
Improved handling of of download errors

## Version 1.7.0 
Released 2022-03-08


Moved from Azure devops 

### Features
No new features
### Bugfixes
* Relation to more than one school for students without class group
* Multiple identifiers for skoleressurs
* Multiple identifiers for elev
* Handling of missing elev link in elevforhold
* Handling of missing person link in elev
* Handling of missing personalressurs link in skoleressurs
* Handling of missing overordnet link in organisasjonselement
