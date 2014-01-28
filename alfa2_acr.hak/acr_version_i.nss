////////////////////////////////////////////////////////////////////////////////
//
//  System Name : ALFA Core Rules
//     Filename : acr_version_i
//    $Revision:: 1          $ current version of the file
//        $Date:: 2012-01-07#$ date the file was created or modified
//       Author : Basilica
//
//    Var Prefix: ACR_VERSION
//  Dependencies: NWNX
//
//  Description
//  This file contains the version number string for the ACR.
//
//  Revision History
//  2012/01/07  Basilica    - Created.
//  2012/04/16  Basilica    - Return build date.
//
////////////////////////////////////////////////////////////////////////////////

#ifndef ACR_VERSION_I
#define ACR_VERSION_I

////////////////////////////////////////////////////////////////////////////////
// Constants ///////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

const string ACR_VERSION = "1.92";
const string ACR_HAK_BUILD_DATE = "ACR_HAK_BUILD_DATE";
const string ACR_HAK_VERSION = "ACR_HAK_VERSION";

////////////////////////////////////////////////////////////////////////////////
// Structures //////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////
// Global Variables ////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////
// Function Prototypes /////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

//! Get the version string of this ACR release.
//!  - Returns: The ACR version string constant.
string ACR_GetVersion();

//! Get the build date for this file.
//!  - Returns: The date time string that this file was compiled.
string ACR_GetBuildDate();

//!  Get the version string that the HAK was built with.
//!  - Returns: The HAK's ACR version string constant.
string ACR_GetHAKVersion();

//! Get the HAK build date.
//!  - Returns: The date time string that acr_version_check.nss (in the hak)
//              was compiled on.
string ACR_GetHAKBuildDate();

////////////////////////////////////////////////////////////////////////////////
// Includes ////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////
// Function Definitions ////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

string ACR_GetVersion()
{
	return ACR_VERSION;
}

string ACR_GetBuildDate()
{
	return __DATE__ + " " + __TIME__;
}

string ACR_GetHAKBuildDate()
{
	return GetGlobalString(ACR_HAK_BUILD_DATE);
}

string ACR_GetHAKVersion()
{
	return GetGlobalString(ACR_HAK_VERSION);
}

#endif