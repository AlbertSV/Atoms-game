using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dva
{
    public enum GeneralParticleType
    {
        Electron,
        Proton,
        Neutron
    }

    public enum ParticleState
    {
        InAtom,
        Free
    }

    public enum AtomState
    {
        Neutral,
        Minus,
        Plus
    }

    public enum SpecialParticleType
    {
        BlackHole,
        TimeFast,
        TimeSlow,
        FastNeutron,
        FieldRise,
        FiledShrink
    }
}