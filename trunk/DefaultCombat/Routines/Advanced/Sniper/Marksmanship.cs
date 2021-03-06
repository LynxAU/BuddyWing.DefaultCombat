﻿// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Marksmanship : RotationBase
    {
        public override string Name
        {
            get { return "Sniper Marksmanship"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Coordination")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Escape", ret => Me.IsStunned),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 40),
                    Spell.Buff("Sniper Volley", ret => Me.EnergyPercent <= 60),
                    Spell.Buff("Entrench", ret => Me.CurrentTarget.StrongOrGreater() && Me.IsInCover()),
                    Spell.Buff("Laze Target"),
                    Spell.Buff("Target Acquired"),
                    Spell.Cast("Unity", ret => Me.Companion != null && Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    //Movement
                    CombatMovement.CloseDistance(Distance.Ranged),


                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Low Energy
                    new Decorator(ret => Me.EnergyPercent < 60,
                        new PrioritySelector(
                            Spell.Cast("Rifle Shot")
                            )),

                    //Rotation
                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Followthrough"),
                    Spell.Cast("Penetrating Blasts"),
                    Spell.Cast("Series of Shots", ret => Me.Level < 26),
                    Spell.DoT("Corrosive Dart", "Corrosive Dart"),
                    Spell.Cast("Ambush", ret => Me.BuffCount("Zeroing Shots") == 2),
                    Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Snipe")
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldAoe,
                    new PrioritySelector(
                        Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                        Spell.CastOnGround("Orbital Strike", ret => Me.IsInCover() && Me.EnergyPercent > 30),
                        Spell.Cast("Fragmentation Grenade"),
                        Spell.CastOnGround("Suppressive Fire", ret => Me.IsInCover() && Me.EnergyPercent > 10)
                        ));
            }
        }
    }
}
