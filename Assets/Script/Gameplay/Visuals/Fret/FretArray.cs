using System;
using System.Collections.Generic;
using UnityEngine;
using YARG.Core;
using YARG.Core.Chart;
using YARG.Core.Game;
using YARG.Core.Logging;
using YARG.Themes;
using static YARG.Themes.ThemeManager;

namespace YARG.Gameplay.Visuals
{
    public class FretArray : MonoBehaviour
    {
        private const float WIDTH_NUMERATOR   = 2f;
        private const float WIDTH_DENOMINATOR = 5f;

        public int FretCount;
        public bool DontFlipColorsLeftyFlip;
        public bool UseKickFrets;

        public Dictionary<int, int> NoteToPosition;
        private Dictionary<int, List<int>> PositionToNotes;

        [SerializeField]
        private float _trackWidth = 2f;

        [Space]
        [SerializeField]
        private Transform _leftKickFretPosition;
        [SerializeField]
        private Transform _rightKickFretPosition;

        private readonly List<Fret> _frets = new();
        private readonly List<KickFret> _kickFrets = new();

        private bool[] _activeFrets;
        private bool[] _pulsingFrets;
        private float  _pulseDuration;

        public void Initialize(
            ThemePreset themePreset,
            VisualStyle style,
            ColorProfile.IFretColorProvider fretColorProvider,
            Dictionary<int, int> highwayOrdering, // Mapping from note number (fret/pad/key) to lateral position on the highway (0-indexed)
            bool leftyFlip,
            bool splitProTomsAndCymbals
        ) {
            NoteToPosition = highwayOrdering;
            PositionToNotes = new();
            foreach (var (note, position) in NoteToPosition)
            {
                if (PositionToNotes.ContainsKey(position))
                {
                    PositionToNotes[position].Add(note);
                } else
                {
                    PositionToNotes[position] = new() { note };
                }
            }


            var fretPrefab = ThemeManager.Instance.CreateFretPrefabFromTheme(
                themePreset, style);

            // Spawn in normal frets
            _frets.Clear();
            foreach (var (_, position) in NoteToPosition)
            {
                // Spawn
                var fret = Instantiate(fretPrefab, transform);
                fret.SetActive(true);

                // Position
                float x = _trackWidth / FretCount * position - _trackWidth / 2f + 1f / FretCount;
                fret.transform.localPosition = new Vector3(leftyFlip ? -x : x, 0f, 0f);

                // Scale
                float scale = (_trackWidth / WIDTH_NUMERATOR) / (FretCount / WIDTH_DENOMINATOR);
                fret.transform.localScale = new Vector3(scale, 1f, 1f);

                // Add
                var fretComp = fret.GetComponent<Fret>();
                _frets.Add(fretComp);
            }

            _kickFrets.Clear();
            if (UseKickFrets)
            {
                var kickFretPrefab = ThemeManager.Instance.CreateKickFretPrefabFromTheme(
                    themePreset, style);

                // Spawn in kick frets
                var leftKick = Instantiate(kickFretPrefab, transform);
                leftKick.SetActive(true);
                var rightKick = Instantiate(kickFretPrefab, transform);
                rightKick.SetActive(true);

                // Position kick frets
                leftKick.transform.localPosition = _leftKickFretPosition.localPosition;
                rightKick.transform.localPosition = _rightKickFretPosition.localPosition;
                rightKick.transform.localScale = rightKick.transform.localScale.InvertX();

                // Add kick frets
                _kickFrets.Add(leftKick.GetComponent<KickFret>());
                _kickFrets.Add(rightKick.GetComponent<KickFret>());
            }

            InitializeColor(fretColorProvider, leftyFlip, splitProTomsAndCymbals);

            _activeFrets = new bool[FretCount];
            _pulsingFrets = new bool[FretCount];
            // Start with all frets active, they will be set inactive once TrackPlayer figures itself out
            for (int i = 0; i < FretCount; i++)
            {
                _activeFrets[i] = true;
            }
        }

        public void InitializeColor(ColorProfile.IFretColorProvider fretColorProvider, bool leftyFlip, bool splitProTomsAndCymbals)
        {
            for (int i = 0; i < _frets.Count; i++)
            {
                // This needs unique lefty flip logic because it's the one case where
                // the fret order is different from the color profile order
                int index;
                if (splitProTomsAndCymbals)
                {
                    index = i switch
                    {
                        0 => leftyFlip ? 4 : 1,
                        1 => leftyFlip ? 7 : 6,
                        2 => leftyFlip ? 3 : 2,
                        3 => leftyFlip ? 6 : 7,
                        4 => leftyFlip ? 2 : 3,
                        5 => leftyFlip ? 5 : 8,
                        6 => leftyFlip ? 1 : 4,
                        _ => throw new Exception("Unreachable.")
                    };
                }
                else
                {
                    index = i + 1;
                }

                if (DontFlipColorsLeftyFlip && leftyFlip && !splitProTomsAndCymbals)
                {
                    index = _frets.Count - index + 1;
                }

                var noteTypeForFret = PositionToNotes[i][0];

                _frets[i].Initialize(
                    fretColorProvider.GetFretColor(noteTypeForFret),
                    fretColorProvider.GetFretInnerColor(noteTypeForFret),
                    fretColorProvider.GetParticleColor(noteTypeForFret),
                    fretColorProvider.GetParticleColor(0 /* open note */)
                );
            }

            foreach (var kick in _kickFrets)
            {
                kick.Initialize(fretColorProvider.GetFretColor(0));
            }
        }

        public void SetPressed(int note, bool pressed)
        {
            if (NoteToPosition.ContainsKey(note))
            {
                _frets[NoteToPosition[note]].SetPressed(pressed);
            }
        }

        public void SetPressedDrum(int note, bool pressed, Fret.AnimType animType)
        {
            if (NoteToPosition.ContainsKey(note))
            {
                _frets[NoteToPosition[note]].SetPressedDrum(pressed, animType);
            }
        }

        public void SetSustained(int note, bool sustained)
        {
            if (NoteToPosition.ContainsKey(note))
            {
                _frets[NoteToPosition[note]].SetSustained(sustained);
            }
        }

        public void PlayHitAnimation(int note)
        {
            if (NoteToPosition.ContainsKey(note))
            {
                _frets[NoteToPosition[note]].PlayHitAnimation();
                _frets[NoteToPosition[note]].PlayHitParticles();
            }
        }

        public void PlayCymbalHitAnimation(int note)
        {
            if (NoteToPosition.ContainsKey(note))
            {
                _frets[NoteToPosition[note]].PlayCymbalHitAnimation();
                _frets[NoteToPosition[note]].PlayHitParticles();
            }
        }

        public void PlayOpenHitAnimation()
        {
            foreach (var fret in _frets)
            {
                fret.PlayHitAnimation();
                fret.PlayOpenHitParticles();
            }
        }

        public void PlayMissAnimation(int index)
        {
            if (0 <= index && index <= _frets.Count)
            {
                _frets[index].PlayMissAnimation();
                _frets[index].PlayMissParticles();
            }
        }

        public void PlayOpenMissAnimation()
        {
            foreach (var fret in _frets)
            {
                fret.PlayOpenMissAnimation();
                fret.PlayOpenMissParticles();
            }
        }

        public void PlayKickFretAnimation()
        {
            foreach (var kick in _kickFrets)
            {
                kick.PlayHitAnimation();
            }
        }

        public void ResetAll()
        {
            foreach (var fret in _frets)
            {
                fret.SetSustained(false);
            }
        }

        public void UpdateAccentColorState(int note, bool shouldWhiten)
        {
            if (NoteToPosition.ContainsKey(note))
            {
                if (shouldWhiten)
                {
                    _frets[note].WhitenFretColor();
                }
                else
                {
                    _frets[note].RestoreFretColor();
                }
            }
        }

        public void SetFretColorPulse(int note, bool pulse, float duration)
        {
            if (NoteToPosition.ContainsKey(note))
            {
                _pulseDuration = duration;
                _pulsingFrets[note] = pulse;
            }
        }

        public void PulseFretColors()
        {
            for (int i = 0; i < _pulsingFrets.Length; i++)
            {
                if (!_pulsingFrets[i] || _activeFrets[i])
                {
                    continue;
                }

                _frets[i].FadeColor(_pulseDuration, true, false);
            }
        }

        public void UpdateFretActiveState(bool[] frets)
        {
            // We should always receive the same number of frets that we actually have, but...
            if (frets.Length != _frets.Count)
            {
                YargLogger.LogFormatDebug("Received inconsistent fret array. Got {0} flags, but we have {1} frets.", frets.Length, _frets.Count);
                return;
            }

            for (int i = 0; i < _frets.Count; i++)
            {
                if (_activeFrets[i] != frets[i])
                {
                    if (frets[i])
                    {
                        _frets[i].ResetColor(true);
                    }
                    else
                    {
                        _frets[i].DimColor(true);
                    }
                }

                _activeFrets[i] = frets[i];
            }
        }
    }
}