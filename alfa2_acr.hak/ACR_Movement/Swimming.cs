﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLRScriptFramework;
using ALFA;
using NWScript;
using NWScript.ManagedInterfaceLayer.NWScriptManagedInterface;

using NWEffect = NWScript.NWScriptEngineStructure0;
using NWEvent = NWScript.NWScriptEngineStructure1;
using NWLocation = NWScript.NWScriptEngineStructure2;
using NWTalent = NWScript.NWScriptEngineStructure3;
using NWItemProperty = NWScript.NWScriptEngineStructure4;

namespace ACR_Movement
{
    class Swimming
    {
        public static Dictionary<uint, uint> CurrentSwimTrigger = new Dictionary<uint, uint>();

        public static Dictionary<uint, int> CurrentDrownStatus = new Dictionary<uint, int>();
        public static Dictionary<uint, int> CurrentDrownDC = new Dictionary<uint, int>();

        public const string ACR_SWIM_DC = "ACR_SWIM_DC";
        public const string ACR_NO_AIR = "ACR_NO_AIR";

        public static void SwimTriggerEnter(CLRScriptBase script, uint Creature, uint Trigger)
        {
            SwimHeartbeat(script, Creature, Trigger);
        }

        private static void SwimHeartbeat(CLRScriptBase script, uint Creature, uint Trigger)
        {
            foreach(uint contents in script.GetObjectsInPersistentObject(Trigger, CLRScriptBase.OBJECT_TYPE_CREATURE, CLRScriptBase.PERSISTENT_ZONE_ACTIVE))
            {
                if(contents == Creature)
                {
                    if (script.GetSubRace(Creature) != CLRScriptBase.RACIAL_SUBTYPE_WATER_GENASI)
                    {
                        int SwimDC = script.GetLocalInt(Trigger, ACR_SWIM_DC);
                        int SinkDC = SwimDC - 5;
                        int NoAir = script.GetLocalInt(Trigger, ACR_NO_AIR);
                        int Roll = script.d20(1);
                        int Bonus = script.GetSkillRank(CLRScriptBase.SKILL_SWIM, Creature, CLRScriptBase.FALSE);
                        if (10 + Bonus >= SwimDC)
                        {
                            // Can take 10 here.
                            Roll = 10;
                        }
                        if (Roll + Bonus >= SwimDC)
                        {
                            script.ApplyEffectToObject(CLRScriptBase.DURATION_TYPE_TEMPORARY, script.ExtraordinaryEffect(script.EffectMovementSpeedDecrease(50)), Creature, 6.0f);
                            script.SendMessageToPC(Creature, String.Format("*Swim: {0} + {1} = {2} v. DC {3} :: Success!*"));
                            if(NoAir == CLRScriptBase.FALSE)
                            {
                                CurrentDrownStatus.Remove(Creature);
                                CurrentDrownDC.Remove(Creature);
                            }
                            else
                            {
                                ProcessNoAir(script, Creature);
                            }
                        }
                        else if (Roll + Bonus >= SinkDC)
                        {
                            script.ApplyEffectToObject(CLRScriptBase.DURATION_TYPE_TEMPORARY, script.ExtraordinaryEffect(script.EffectMovementSpeedDecrease(75)), Creature, 6.0f);
                            script.SendMessageToPC(Creature, String.Format("*Swim: {0} + {1} = {2} v. DC {3} :: Failure!*"));
                            script.SendMessageToPC(Creature, String.Format("You struggle to move through the water."));
                            if (NoAir == CLRScriptBase.FALSE)
                            {
                                CurrentDrownStatus.Remove(Creature);
                                CurrentDrownDC.Remove(Creature);
                            }
                            else
                            {
                                ProcessNoAir(script, Creature);
                            }
                        }
                        else
                        {
                            script.ApplyEffectToObject(CLRScriptBase.DURATION_TYPE_TEMPORARY, script.ExtraordinaryEffect(script.EffectMovementSpeedDecrease(75)), Creature, 6.0f);
                            script.SendMessageToPC(Creature, String.Format("*Swim: {0} + {1} = {2} v. DC {3} :: Failure!*"));
                            script.SendMessageToPC(Creature, String.Format("You're completely overwhelmed by the pull of the water!"));
                            ProcessNoAir(script, Creature);
                        }
                    }
                    else
                    {
                        script.SendMessageToPC(Creature, "Your swim speed and capacity to breathe water allows you to move easily through the water.");
                        return;
                    }
                    script.DelayCommand(6.0f, delegate { SwimHeartbeat(script, Creature, Trigger); });
                    return;
                }
            }
            AppearanceTypes.characterMovement[Creature] = AppearanceTypes.MovementType.Walking;
            AppearanceTypes.RecalculateMovement(script, Creature);
        }

        private static void ProcessNoAir(CLRScriptBase script, uint Creature)
        {
            if (!GetNeedsToBreathe(script, Creature))
            {
                return;
            }

            if (!CurrentDrownStatus.ContainsKey(Creature))
            {
                CurrentDrownStatus.Add(Creature, 1);
            }
            else
            {
                CurrentDrownStatus[Creature]++;
            }

            if(CurrentDrownStatus[Creature] > script.GetAbilityScore(Creature, CLRScriptBase.ABILITY_CONSTITUTION, CLRScriptBase.FALSE))
            {
                if(!CurrentDrownDC.ContainsKey(Creature))
                {
                    CurrentDrownDC.Add(Creature, 10);
                }
                else
                {
                    CurrentDrownDC[Creature]++;
                }

                int conCheck = script.d20(1);
                int conMod = script.GetAbilityModifier(CLRScriptBase.ABILITY_CONSTITUTION, Creature);
                if(conCheck + conMod < CurrentDrownDC[Creature])
                {
                    script.SendMessageToPC(Creature, String.Format("*Constitution: {0} + {1} = {2} v. DC {3} :: Failure!*", conCheck, conMod, conCheck + conMod, CurrentDrownDC[Creature]));
                    int Damage = 0;
                    string Message;
                    int HP = script.GetCurrentHitPoints(Creature);
                    if(HP > 1)
                    {
                        Damage = script.GetCurrentHitPoints(Creature) - 1;
                        Message = "You're suffocating! If you don't find air NOW, you will DIE!";
                    }
                    else if(HP > 0)
                    {
                        Damage = 2;
                        Message = "You have lost consciousness. Unless someone comes to rescue you, you will die.";
                    }
                    else
                    {
                        Damage = 10;
                        Message = "You have suffocated";
                    }
                    script.ApplyEffectToObject(CLRScriptBase.DURATION_TYPE_INSTANT, script.EffectDamage(Damage, CLRScriptBase.DAMAGE_TYPE_MAGICAL, CLRScriptBase.DAMAGE_POWER_ENERGY, CLRScriptBase.TRUE), Creature, 0.0f);
                    script.SendMessageToPC(Creature, Message);
                    return;
                }
                else
                {
                    script.SendMessageToPC(Creature, "You are barely clutching to consciousness! Get air NOW!");
                    return;
                }
            }
            else
            {
                int SecondsLeft = ((script.GetAbilityScore(Creature, CLRScriptBase.ABILITY_CONSTITUTION, CLRScriptBase.FALSE) * 2) - CurrentDrownStatus[Creature]) * 6;
                if(SecondsLeft > 60)
                {
                    script.SendMessageToPC(Creature, String.Format("You can hold your breath for {0} more seconds.", SecondsLeft));
                }
                else if(SecondsLeft > 30)
                {
                    script.SendMessageToPC(Creature, String.Format("Careful! You can only hold your breath for {0} more seconds!", SecondsLeft));
                }
                else
                {
                    script.SendMessageToPC(Creature, String.Format("** WARNING ** If you don't find air within {0} seconds, you will DIE!", SecondsLeft));
                }
            }
        }
        
        private static bool GetNeedsToBreathe(CLRScriptBase script, uint Creature)
        {
            if(script.GetSubRace(Creature) == CLRScriptBase.RACIAL_SUBTYPE_AIR_GENASI ||
               script.GetSubRace(Creature) == CLRScriptBase.RACIAL_SUBTYPE_WATER_GENASI)
            {
                return false;
            }

            foreach(NWEffect effect in script.GetEffects(Creature))
            {
                int spellId = script.GetEffectSpellId(effect);
                if(spellId == CLRScriptBase.SPELL_WATER_BREATHING ||
                   spellId == CLRScriptBase.SPELL_STONE_BODY ||
                   spellId == CLRScriptBase.SPELL_LIVING_UNDEATH ||
                   spellId == CLRScriptBase.SPELL_IRON_BODY ||
                   script.GetEffectType(effect) == CLRScriptBase.EFFECT_TYPE_PETRIFY)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
