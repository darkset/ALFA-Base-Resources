////////////////////////////////////////////////////////////////////////////////
//
//  System Name : ALFA Core Rules
//     Filename : acr_area_instance_onleave
//    $Revision:: 1          $ current version of the file
//        $Date:: 2012-01-08#$ date the file was created or modified
//       Author : Basilica
//
//    Var Prefix: AREA_INSTANCE
//  Dependencies: NWNX, MYSQL, CLRSCRIPT(acr_servermisc)
//
//  Description
//  This file contains the hooked event handler for the leave event on an
//  instanced area.
//
//  Revision History
//  2012/01/19  Basilica    - Created.
//
////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////
// Constants ///////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////
// Structures //////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////
// Global Variables ////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////
// Function Prototypes /////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

void main();

////////////////////////////////////////////////////////////////////////////////
// Includes ////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

#include "acr_area_instance_i"

////////////////////////////////////////////////////////////////////////////////
// Function Definitions ////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

void main()
{
	object ExitingObject = GetExitingObject();

	// If the exiting object was a player, trigger area cleanup if need be.
	if (GetIsPC(ExitingObject) && !ACR__IsAreaInstanceDeleting(OBJECT_SELF))
		ACR_AreaInstance_OnClientLeave(ExitingObject, TRUE);

	// Call the real OnLeave handler.
	ACR__CallOriginalOnLeaveHandler(OBJECT_SELF);
}

