using System;
using UnityEngine;
using YARG.Core.Chart;
using YARG.Helpers.Extensions;
using YARG.Settings;

namespace YARG.Gameplay.Visuals
{
    public sealed class SixLaneDrumsNoteElement : DrumsNoteElement
    {
        protected override void InitializeElement()
        {
            base.InitializeElement();

            var noteGroups = NoteRef.IsStarPower ? StarPowerNoteGroups : NoteGroups;

            if (NoteRef.Pad != 0)
            {
                // Deal with non-kick notes

                // Set the position
                transform.localPosition = new Vector3(GetElementX(NoteRef.Pad, 6), 0f, 0f) * LeftyFlipMultiplier;

                // Get which note model to use
                if (SettingsManager.Settings.UseCymbalModelsInSixLane.Value)
                {
                    bool isCymbal = (SixLaneDrumPad) NoteRef.Pad is SixLaneDrumPad.Red or SixLaneDrumPad.Silver or SixLaneDrumPad.Purple;

                    NoteGroup = noteGroups[GetNoteGroup(isCymbal)];
                }
                else
                {
                    NoteGroup = noteGroups[(int) NoteType.Normal];
                }
            }
            else
            {
                // Deal with kick notes
                transform.localPosition = Vector3.zero;
                NoteGroup = noteGroups[(int) NoteType.Kick];
            }

            // Show and set material properties
            NoteGroup.SetActive(true);
            NoteGroup.Initialize();

            // Set note color
            UpdateColor();
        }

        protected override void UpdateColor()
        {
            var colors = Player.Player.ColorProfile.SixLaneDrums;

            // Get pad index
            int pad = NoteRef.Pad;
            if (LeftyFlip)
            {
                pad = (SixLaneDrumPad) pad switch
                {
                    SixLaneDrumPad.Kick    => (int) SixLaneDrumPad.Kick,
                    SixLaneDrumPad.Red     => (int) SixLaneDrumPad.Purple,
                    SixLaneDrumPad.Silver  => (int) SixLaneDrumPad.Green,
                    SixLaneDrumPad.Yellow  => (int) SixLaneDrumPad.Blue,
                    SixLaneDrumPad.Blue    => (int) SixLaneDrumPad.Yellow,
                    SixLaneDrumPad.Green   => (int) SixLaneDrumPad.Silver,
                    SixLaneDrumPad.Purple  => (int) SixLaneDrumPad.Red,
                    _                      => throw new Exception("Unreachable.")
                };
            }

            // Get colors
            var colorNoStarPower = colors.GetNoteColor(pad);
            var color = colorNoStarPower;
            if (NoteRef.IsStarPowerActivator && Player.Engine.CanStarPowerActivate)
            {
                color = colors.ActivationNote;
            }
            else if (NoteRef.IsStarPower)
            {
                color = colors.GetNoteStarPowerColor(pad);
            }

            // Set the note color
            NoteGroup.SetColorWithEmission(color.ToUnityColor(), colorNoStarPower.ToUnityColor());
        }
    }
}