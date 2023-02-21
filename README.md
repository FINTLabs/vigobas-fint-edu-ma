# VIGOBAS FINT Edu MA

## Description

Management Agent for Microsoft Identity Manager (MIM). Syncs educational data with any school administration system (SAS) that is connected to FINT. Currently Visma InSchool is the only SAS connected to FINT.

Version 1.7.0 is the first public versjon. For older versions see: https://vigobas.vigoiks.no/VigoBAS/systemdokumentasjon/releasenotes/ 

## Version 1.7.99
This is the 1.8.0-beta
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
